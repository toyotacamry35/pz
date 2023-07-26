using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Src.GameObjectAssembler.Res;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.Utils;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.Src.GameObjectAssembler
{
    public interface IJsonToGoTemplateComponentInit
    {
        void InitForJsonToGoTemplate();
    }
    public interface IJsonToGoTemplateComponentAfterAllInit
    {
        void InitAfterAllForJsonToGoTemplate();
    }
    public class JsonToGO
    {
        private struct Key
        {
            public readonly GameObject Go;
            public readonly UnityGameObjectDef Def;

            public Key(GameObject go, UnityGameObjectDef def)
            {
                Go = go;
                Def = def;
            }
        }

        private static readonly NLog.Logger Logger = LogManager.GetLogger("JsonToGO");
        [CanBeNull]
        private static JsonToGO _instance;

        [NotNull]
        public static JsonToGO Instance => _instance ?? (_instance = new JsonToGO());

        private readonly Dictionary<UnityGameObjectDef, GameObject> _knownAssembledTemplates = new Dictionary<UnityGameObjectDef, GameObject>();

        [NotNull]
        private static readonly IReadOnlyDictionary<Type, Type> KnownComponents;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            // Calling .cctor()
        }

        static JsonToGO()
        {
            var components = new Dictionary<Type, Type>();
            var q = DefToType.GetAllDefsToInstanceNoInterface<ComponentDef>();
            //key = def, value = component
            foreach (var pair in q)
            {
                if (!components.ContainsKey(pair.Key))
                {
                    if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message($"Add Known Component: {pair.Key} -> {pair.Value}").Write();
                    components.Add(pair.Key, pair.Value);
                }
            }
            KnownComponents = components;

            foreach (var comp in components)
                ComponentAssigners.CreateAssignComponentDelegate(comp.Key, comp.Value);
        }

        private JsonToGO()
        {
        }

        private GameObject _loadedObject;
        private UnityGameObjectDef _loadedDef;
        private HashSet<UnityGameObjectDef> _alreadyMetDefs = new HashSet<UnityGameObjectDef>();
        private Stack<DefStackElement> _defsStack = new Stack<DefStackElement>();
        struct DefStackElement
        {
            public string FieldName;
            public UnityGameObjectDef Def;
        }

        private static CustomSampler _instantiateAndMergeWithSampler;
        private static CustomSampler _mergeWithSampler;

        private static CustomSampler _componentCreationSampler;
        private static CustomSampler _fieldSetterSampler;

        private readonly static UnityGameObjectDef _defaultDef = new UnityGameObjectDef();
        private static readonly IDictionary<Key, JsonToGOIndexComponent> Templates = new Dictionary<Key, JsonToGOIndexComponent>();

        public GameObject InstantiateAndMerge([NotNull] GameObject obj, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, bool autoEnable = true, Type autoAddComponent = null)
        {
            var refHolder = obj.GetComponent<JsonRefHolder>();

            var resource = refHolder?.Definition;

            if (resource == null)
                resource = _defaultDef;
            return InstantiateAndMergeWith(obj, resource, position, rotation, autoEnable);
        }
        static List<IJsonToGoTemplateComponentInit> _componentsCache = new List<IJsonToGoTemplateComponentInit>();
        public GameObject InstantiateAndMergeWith([CanBeNull] GameObject obj, UnityGameObjectDef resource, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, bool autoEnable = true, Type autoAddComponent = null, bool isStatic = false)
        {
            if (_instantiateAndMergeWithSampler == null)
                _instantiateAndMergeWithSampler = CustomSampler.Create("JsonToGO.InstantiateAndMergeWith");

            _instantiateAndMergeWithSampler.Begin();

            _defsStack.Clear();
            _alreadyMetDefs.Clear();

            var key = new Key(obj, resource);
            JsonToGOIndexComponent existing;
            if (!Templates.TryGetValue(key, out existing))
            {
                Dictionary<IComponentDef, Component> index = new Dictionary<IComponentDef, Component>();
                GameObject tmp;
                if (obj != null)
                {
                    obj.SetActive(false);
                    tmp = UnityEngine.Object.Instantiate(obj);
                }
                else
                {
                    tmp = new GameObject(resource.ToString());
                    tmp.SetActive(false);
                }
                if (autoAddComponent != null)
                    if (tmp.GetComponent(autoAddComponent) == null)
                        tmp.AddComponent(autoAddComponent);
                _loadedObject = obj;
                _loadedDef = resource;

                AssembleNewTemplateRecursive(tmp, resource, index);
                existing = tmp.AddComponent<JsonToGOIndexComponent>();
                existing.Mappings = index;
                if (Application.isPlaying)
                    UnityEngine.Object.DontDestroyOnLoad(existing.gameObject);

                var prefabJsonHolder = existing.GetComponent<JsonRefHolder>();
                if (prefabJsonHolder != null)
                    prefabJsonHolder.InitializedByItself = false;
                existing.GetComponent<IJsonToGoTemplateComponentAfterAllInit>()?.InitAfterAllForJsonToGoTemplate();
                _componentsCache.Clear();
                existing.GetComponentsInChildren<IJsonToGoTemplateComponentInit>(_componentsCache);
                foreach (var c in _componentsCache)
                    c.InitForJsonToGoTemplate();
                Templates.Add(key, existing);
            }

            var result = UnityEngine.Object.Instantiate(existing.gameObject, position, rotation).GetComponent<JsonToGOIndexComponent>();

            FillComponents(result.gameObject, result.Mappings);

            // `InitializedByItself` is no need any more:
            var holder = result.GetComponent<JsonRefHolder>();
            if (holder != null)
                holder.InitializedByItself = false;

            if (autoEnable)
                result.gameObject.SetActive(true);

            _instantiateAndMergeWithSampler.End();

            //if (isStatic)
            //    result.gameObject.isStatic = true;
            return result.gameObject;
        }

        public void Merge([NotNull] GameObject obj)
        {
            var resource = obj.GetComponent<JsonRefHolder>()?.Definition;
            if (resource == null)
                return;

            MergeWith(obj, resource);
        }

        public void MergeWith([NotNull] GameObject obj, [NotNull]UnityGameObjectDef def)
        {
            if (_mergeWithSampler == null)
                _mergeWithSampler = CustomSampler.Create("JsonToGO.MergeWith");

            _mergeWithSampler.Begin(obj);

            _loadedObject = obj;
            _defsStack.Clear();
            _alreadyMetDefs.Clear();
            _loadedDef = def;


            Dictionary<IComponentDef, Component> index = new Dictionary<IComponentDef, Component>();
            AssembleNewTemplateRecursive(obj, def, index);
            FillComponents(obj, index);

            var holder = obj.GetComponent<JsonRefHolder>();
            if (holder != null)
                holder.InitializedByItself = false;

            _mergeWithSampler.End();
        }

        [NotNull]
        public GameObject Assemble([NotNull] UnityGameObjectDef definition)
        {
            return InstantiateAndMergeWith(null, definition, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
        }

        public T AddOrGetComponent<T>([NotNull] GameObject obj, [NotNull] IComponentDef componentDef) where T : Component
        {
            var component = AddOrGetComponentByType(obj, componentDef) as T;
            if (component)
            {
                var del = ComponentAssigners.GetAssignComponentDelegate(componentDef.GetType(), component.GetType());
                del(componentDef, component, new Dictionary<IComponentDef, Component>());
            }
            return component;
        }

        private void AssembleNewTemplateRecursive([NotNull] GameObject obj, [NotNull] UnityGameObjectDef definition, Dictionary<IComponentDef, Component> index)
        {
            if (definition == null)
                return;
            if (!_alreadyMetDefs.Add(definition))
            {
                StringBuilder stackBuilder = new StringBuilder();
                foreach (var defStackElement in _defsStack)
                    stackBuilder.Append(defStackElement.FieldName).Append(":").Append(defStackElement.Def != null ? defStackElement.Def.ToString() : "");

                Logger.If(LogLevel.Error)
                    ?.Message($"Recursive reference encountered during loading of {_loadedObject.name} {_loadedDef.Name} {_loadedDef.ToString()} in def named {definition.Name}\n{stackBuilder}")
                    .UnityObj(obj)
                    .Write();
                throw new JsonRecursionException();
            }
            foreach (var child in definition.Children)
            {
                _defsStack.Push(new DefStackElement() { Def = definition, FieldName = child.Target.Name });
                var go = new GameObject(child.Target.Name);
                go.transform.parent = obj.transform;
                AssembleNewTemplateRecursive(go, child, index);
                _defsStack.Pop();
            }
            AddComponents(obj, definition.Components.Select(x => x.Target).Where(x => x != null && !(x is EntityGameObjectDef)), index);
        }

        private void AddComponents([NotNull] GameObject obj, [NotNull] IEnumerable<IComponentDef> components, Dictionary<IComponentDef, Component> index)
        {
            if (_componentCreationSampler == null)
                _componentCreationSampler = CustomSampler.Create("JsonToGO.AddComponents");

            _componentCreationSampler.Begin();
            foreach (var comp in components)
            {
                var c = AddOrGetComponentByType(obj, comp);
                if (!c) throw new Exception($"AddOrGetComponentByType returns null for {comp.GetType()}");
                index.Add(comp, c);
            }
            _componentCreationSampler.End();
        }

        private void FillComponents([NotNull] GameObject obj, IReadOnlyDictionary<IComponentDef, Component> index)
        {
            if (_fieldSetterSampler == null)
                _fieldSetterSampler = CustomSampler.Create("JsonToGO.FillComponents");

            _fieldSetterSampler.Begin();
            foreach (var comp in index)
            {
                var del = ComponentAssigners.GetAssignComponentDelegate(comp.Key.GetType(), comp.Value.GetType());
                del(comp.Key, comp.Value, index);
            }
            _fieldSetterSampler.End();
        }

        private Component AddOrGetComponentByType([NotNull] GameObject obj, [NotNull] IComponentDef component)
        {
            if (!obj) throw new ArgumentNullException($"obj");

            Type unityCompType;
            if (component.ReuseExisting)
            {
                if (!KnownComponents.TryGetValue(component.GetType(), out unityCompType))
                    Logger.IfError()?.Message("Failed to find unity component type for Resource {0} {1}", component.GetType().Name, obj.name).Write();

                var unityComp = obj.GetComponent(unityCompType);
                if (unityComp == null)
                {
                    Logger.IfError()?.Message("Failed to find unity component for Resource {0} {1}", component.GetType().Name, obj.name).Write();
                }
                return unityComp;
            }

            if (!KnownComponents.TryGetValue(component.GetType(), out unityCompType))
            {
                Logger.IfError()?.Message("Failed to find unity component for Resource {0} {1}", component.GetType().Name, obj.name).Write();
                return null;
            }
            var comp = obj.AddComponent(unityCompType);
            return comp;
        }
    }

    // If you inherit this i-face, you 'll have def plugged into `Def` prop. (& be able to do with it whatever you want)
    public interface IFromDef<T> where T : IComponentDef
    {
        T Def { get; set; }
    }
}

public class JsonRecursionException : Exception
{ }