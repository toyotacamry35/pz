using Assets.ResourceSystem.Aspects.FX.FullScreenFx;
using Assets.Src.Camera;
using Assets.Src.Effects.PostEffectCamera;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Assets.Src.Lib.Extensions;
using Core.Environment.Logging.Extension;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Src.Character.Effects
{
    public class FXMaterialController : MonoBehaviour
    {
        protected static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private FullscreenFxPostprocess _fullscreenFx;

        private readonly Dictionary<Material, Material> _materialInstanceRepository = new Dictionary<Material, Material>();

        private readonly List<MaterialInfo> _existingMaterials = new List<MaterialInfo>();

        private readonly float _defaultTransitionDelta = 1f; // in Hz (1/s)
        private int finalMixId;

        public void Start()
        {
            var camera = GameCamera.Camera;
            if (camera)
                Init(camera);
            else
                GameCamera.OnCameraChanged += Init;

            finalMixId = Shader.PropertyToID("_finalMix");

            var components = GetComponentsInChildren<FXMaterialProperty>(includeInactive: false);
            foreach (var component in components)
            {
                var referenceMaterialDef = component.MaterialDef;
                if (referenceMaterialDef == null)
                {
                    Logger.IfWarn()?.Message("You forgot to set value for '{0}' field of component '{1}' on gameobject '{2}'", nameof(MaterialDef), component.name, component.gameObject.name).Write();
                    continue;
                }
                var referenceMaterialRef = referenceMaterialDef.Get<MaterialDef>()?.Material;
                if (referenceMaterialRef == null)
                {
                    Logger.IfWarn()?.Message("Wrong type of resource in '{0}' field of component '{1}' on gameobject '{2}'", nameof(MaterialDef), component.name, component.gameObject.name).Write();
                    continue;
                }
                var referenceMaterial = referenceMaterialRef.Target;
                if (referenceMaterial == null)
                {
                    Logger.IfWarn()?.Message("Value of field '{0}' is null in '{1}'", nameof(Material), referenceMaterialRef).Write();
                    continue;
                }
//                var instancedMaterial = GetMaterialInstance(referenceMaterial);
                var fieldsAndGetters = CollectClassFieldsWithGetters(component);
                var tDelta = _defaultTransitionDelta;
                if (referenceMaterial.HasProperty("_transitionDelta"))
                    tDelta = referenceMaterial.GetFloat("_transitionDelta");
                _existingMaterials.InsertSorted(new MaterialInfo { _fieldsWithGetters = fieldsAndGetters, _material = referenceMaterial, _transitionDelta = tDelta }, MaterialInfo.LayerComparer);
            }
        }

        private void Init(UnityEngine.Camera camera)
        {
            if (_fullscreenFx)
                _fullscreenFx.ResetMaterials();
            
            if (camera)
            {
                _fullscreenFx = camera.GetComponent<FullscreenFxPostprocess>();
                if (!_fullscreenFx)
                    if (Logger.IsWarnEnabled)
                        Logger.IfWarn()?.Message($"{nameof(FullscreenFxPostprocess)} not found on {camera.transform.FullName()}").Write();
            }
        }

        public Material GetMaterialInstance(Material requested)
        {
            if (!_materialInstanceRepository.TryGetValue(requested, out Material returned))
                _materialInstanceRepository.Add(requested, returned = Instantiate(requested));
            return returned;
        }

        public void AddMaterial(Material material, int layer)
        {
            var materialInfo = _existingMaterials.Find(x => x._material == material);
            if (materialInfo == null)
            {
                Logger.IfWarn()?.Message("No corresponding material '{0}' is found on prefab", material.name).Write();
                return;
            }
            materialInfo._layer = layer;
            materialInfo._isScheduled = true;
        }

        public void SetMaterialProperty(Material material, string propertyName, float propertyValue)
        {
            var materialInstance = GetMaterialInstance(material);
            materialInstance.SetFloat(propertyName, propertyValue);
        }

        public void RemoveMaterial(Material material, int layer)
        {
            var materialInfo = _existingMaterials.Find(x => x._material == material);
            if (materialInfo == null)
            {
                Logger.IfWarn()?.Message("No corresponding material '{0}' is found on prefab", material.name).Write();
                return;
            }
            materialInfo._isScheduled = false;
        }

        private readonly List<Material> _materialsToRender = new List<Material>();
        private void LateUpdate()
        {
            if (!_fullscreenFx)
                return;

            _materialsToRender.Clear();
            for (int i = 0; i < _existingMaterials.Count; i++)
            {
                var materialInfo = _existingMaterials[i];
                var change = materialInfo._transitionDelta * 0.1f;
                if (materialInfo._isScheduled)
                    change = -change;

                materialInfo._weight = Mathf.Clamp01(materialInfo._weight - change);

                if (materialInfo.IsDisplayed == true)
                {
                    var materialInstance = GetMaterialInstance(materialInfo._material);
                    foreach (var materialProperty in materialInfo._fieldsWithGetters)
                    {
                        materialInstance.SetFloat(materialProperty._fieldName, materialProperty._getter());
                    }
                    materialInstance.SetFloat(finalMixId, materialInfo._weight);
                    _materialsToRender.Add(materialInstance);
                }
            }
            _fullscreenFx.SetMaterials(_materialsToRender);
        }

        private void OnDestroy()
        {
            foreach (var mat in _materialInstanceRepository)
                Destroy(mat.Value);
            _materialInstanceRepository.Clear();

            if (_fullscreenFx)
                _fullscreenFx.ResetMaterials();
        }

        private static List<FieldNameWithGetter<T>> CollectClassFieldsWithGetters<T>(T _class)
        {
            List<FieldNameWithGetter<T>> list = new List<FieldNameWithGetter<T>>();
            Type type = _class.GetType();
            ParameterExpression genericClassExp = Expression.Parameter(typeof(T), "genericClass");
            UnaryExpression classExp = Expression.ConvertChecked(genericClassExp, type);
            IEnumerable<FieldInfo> fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField).Where(x => typeof(float).IsEquivalentTo(x.FieldType));
            foreach (var fieldInfo in fieldInfos)
            {
                MemberExpression fieldExp = Expression.Field(classExp, fieldInfo);
                var getter = Expression.Lambda<Func<T, float>>(fieldExp, new[] { genericClassExp }).Compile();
                list.Add(new FieldNameWithGetter<T> { _fieldName = fieldInfo.Name, _getter = () => getter(_class) });
            }
            return list;
        }

        private struct FieldNameWithGetter<T>
        {
            public string _fieldName;
            public Func<float> _getter;
        }

        private class MaterialInfo
        {
            public static IComparer<MaterialInfo> LayerComparer { get; } = new LayerRelationalComparer();

            public Material _material;
            public int _layer;
            public List<FieldNameWithGetter<FXMaterialProperty>> _fieldsWithGetters;
            public float _weight = 0;
            public float _transitionDelta;
            public bool _isScheduled;
            public bool IsDisplayed => _weight > 0.0f;

            private sealed class LayerRelationalComparer : IComparer<MaterialInfo>
            {
                public int Compare(MaterialInfo x, MaterialInfo y)
                {
                    if (ReferenceEquals(x, y)) return 0;
                    if (ReferenceEquals(null, y)) return 1;
                    if (ReferenceEquals(null, x)) return -1;
                    if (x._layer < y._layer)
                        return -1;
                    if (x._layer > y._layer)
                        return 1;
                    else
                        return 0;
                }
            }

        }
    }
}