using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers;
using Assets.ColonyShared.SharedCode.Arithmetic.Templates.Predicates;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.Aspects.Impl.Stats;
using Assets.Src.ResourcesSystem.Base;
using ResourcesSystem.Loader;
using Assets.Src.ResourceSystem;
using SharedCode.Utils.BsonSerialization;
using SharedCode.Wizardry;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.ResourceSystem
{
    public class JdbReflectionDrawer
    {
        private const int LabelWidth = 200;
        private const int IndentSize = 10;
        private const int FoldoutWidth = 10;
        private const int FieldMinWidth = 50;
        private static readonly Type[] DontShowTypeFor = {typeof(SpellDef), typeof(SubSpell)};
        private static readonly Type[] RemovePrefixesFor = {typeof(CalcerDef<>), typeof(PredicateDef)};
        private static readonly Type[] HideStructureFor = {typeof(StatResource)};
        private static readonly MemberInfo[] HiddenFields = {typeof(SpellWordDef).GetProperty(nameof(SpellWordDef.Enabled)), typeof(BaseResource).GetProperty(nameof(SaveableBaseResource.Id))};

        //   private JdbMetadata _jdbMetadata;
        private readonly string _jdbAddress;
        private readonly int _initialIndent;
        private readonly bool _showFilenameInHeader;
        private readonly int _initialFoldoutLevel;
        private readonly Dictionary<object, bool> _foldout = new Dictionary<object, bool>();
        private readonly Dictionary<Type, bool> _hideStructureFor = new Dictionary<Type, bool>();
        private string _jdbTypeName;
        private IResource _resourceInEditor;
        private Vector2 _scroll;

        public JdbReflectionDrawer(JdbMetadata jdbMetadata, int initialIndent = 0, int initialFoldoutLevel = -1, bool showFilenameInHeader = true)
            : this(jdbMetadata ? jdbMetadata.RootPath : null, initialIndent, initialFoldoutLevel, showFilenameInHeader)
        {
        }

        public JdbReflectionDrawer(string jdbAddress, int initialIndent = 0, int initialFoldoutLevel = -1, bool showFilenameInHeader = true)
        {
            _initialIndent = initialIndent;
            _showFilenameInHeader = showFilenameInHeader;
            _initialFoldoutLevel = initialFoldoutLevel < 0 ? _initialIndent : initialFoldoutLevel;
            _jdbAddress = jdbAddress;
            _jdbTypeName = Resource?.GetType().Name;
        }

        public string JdbTypeName => _jdbTypeName;

        public string JdbAddress => _jdbAddress;
 
        public void Draw(bool editable)
        {
            if (string.IsNullOrEmpty(_jdbAddress))
            {
                EditorGUILayout.HelpBox("JDB not selected or broken", MessageType.Info);
                return;
            }

            var resource = Resource;
            if (resource == null)
            {
                EditorGUILayout.HelpBox($"Can't load [{_jdbAddress}]", MessageType.Error);
                return;
            }

            if (_showFilenameInHeader)
                EditorGUILayout.LabelField(_jdbAddress, GUI.skin.box, GUILayout.ExpandWidth(true));

            _jdbTypeName = resource.GetType().Name;
            
         //   if (!_showFilenameInHeader)
           //     EditorGUILayout.LabelField(resource.GetType().Name, GUI.skin.box, GUILayout.ExpandWidth(true));

           try
           {
               EditorGUI.indentLevel = 0;
               using (var scrollScope = new EditorGUILayout.ScrollViewScope(_scroll, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)))
               {
                   _scroll = scrollScope.scrollPosition;
                   //using (new EditorGUI.DisabledScope(!editable))
                   {
//                    var indentLevel = EditorGUI.indentLevel;
//                    EditorGUI.indentLevel = _initialIndent;
                       var labelWidth = EditorGUIUtility.labelWidth;
                       EditorGUIUtility.labelWidth = LabelWidth;
                       DrawResource(resource, _initialIndent, editable);
//                    EditorGUI.indentLevel = indentLevel;
                       EditorGUIUtility.labelWidth = labelWidth;
                   }
               }
           }
           catch (Exception e)
           {
               EditorGUILayout.HelpBox(e.ToString(), MessageType.Error);
           }
        }

        private void DrawResource(IResource resource, int indent, bool editable)
        {
            foreach (var member in GetProperties(resource))
            {
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    if (member is PropertyInfo)
                    {
                        var property = (PropertyInfo) member;
                        if (property.CanRead && property.CanWrite)
                        {
                            object value = property.GetValue(resource);
                            value = DrawProperty(property.Name, property.PropertyType, value, resource, indent, editable);
                            if (changeScope.changed)
                                property.SetValue(resource, value);
                        }
                    }
                    else if (member is FieldInfo)
                    {
                        var field = (FieldInfo) member;
                        object value = field.GetValue(resource);
                        value = DrawProperty(field.Name, field.FieldType, value, resource, indent, editable);
                        if (changeScope.changed)
                            field.SetValue(resource, value);
                    }
                }
            }
        }

        private IResource Resource {
            get {
                if (Application.isPlaying) // в рантайме нужен тот же инстанс ресурса с которым работает игра, чтобы его изменение имело влияние    
                    return GameResourcesHolder.Instance.LoadResource<IResource>(_jdbAddress);
                return _resourceInEditor ?? (_resourceInEditor = EditorGameResourcesForMonoBehaviours.GetNew().LoadResource<IResource>(_jdbAddress));
            }
        }

        private object DrawProperty(string propertyName, Type propertyType, object propertyValue, IResource resource, int indent, bool editable)
        {
            if (typeof(IResource).IsAssignableFrom(propertyType))
            {
                var subResource = (propertyValue as IResource);
                DrawSubResource(propertyName, subResource, resource, indent, editable);
            }
            else if (typeof(IRefBase).IsAssignableFrom(propertyType))
            {
                var subResource = (propertyValue as IRefBase)?.TargetBase;
                DrawSubResource(propertyName, subResource, resource, indent, editable);
            }
            else if (typeof(IUnityRef).IsAssignableFrom(propertyType))
            {
                var @ref = propertyValue as IUnityRef;
                var refType = GetRefTypeOfUnityRef(propertyType);
                if (refType != null)
                    DrawProperty(propertyName, refType, @ref?.TargetObj, resource, indent, editable);
            }
            else if (propertyType == typeof(float))
            {
                DrawEntry(propertyName, indent, editable, drawer: () =>
                    propertyValue = EditorGUILayout.FloatField((float) propertyValue, GUILayout.MinWidth(FieldMinWidth))
                );
            }
            else if (propertyType == typeof(int))
            {
                DrawEntry(propertyName, indent, editable, drawer: () =>
                    propertyValue = EditorGUILayout.IntField((int) propertyValue, GUILayout.MinWidth(FieldMinWidth))
                );
            }
            else if (propertyType == typeof(bool))
            {
                DrawEntry(propertyName, indent, editable, drawer: () =>
                    propertyValue = EditorGUILayout.Toggle((bool) propertyValue, GUILayout.MinWidth(FieldMinWidth))
                );
            }
            else if (propertyType == typeof(string))
            {
                DrawEntry(propertyName, indent, editable, drawer: () =>
                    propertyValue = EditorGUILayout.TextField((string) propertyValue, GUILayout.MinWidth(FieldMinWidth))
                );
            }
            else if (propertyType == typeof(Curve))
            {
                DrawEntry(propertyName, indent, true, drawer: () =>
                    {
                        var curveProp = (Curve) propertyValue;
                        if (curveProp)
                            curveProp.curve = EditorGUILayout.CurveField(curveProp.curve, GUILayout.MinWidth(FieldMinWidth));
                    }
                );
            }
            else if (propertyType == typeof(AnimationCurve))
            {
                DrawEntry(propertyName, indent, editable, drawer: () =>
                    propertyValue = EditorGUILayout.CurveField((AnimationCurve) propertyValue, GUILayout.MinWidth(FieldMinWidth))
                );
            }
            // else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            // {
            //     var types = propertyType.GenericTypeArguments;
            //     var key = propertyType.GetProperty("Key").GetValue(propertyValue);
            //     var value = propertyType.GetProperty("Value").GetValue(propertyValue);
            //     DrawProperty("Key", types[0], key, resource, indent, editable);
            //     DrawProperty("Value", types[1], value, resource, indent, editable);
            // }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(propertyType))
            {
                DrawEntry(propertyName, indent, editable, drawer: () =>
                    propertyValue = EditorGUILayout.ObjectField((UnityEngine.Object) propertyValue, propertyType, false, GUILayout.MinWidth(FieldMinWidth))
                );
            }
            else if (propertyType.GetInterface(nameof(IDictionary)) != null)
            {
                var dictionary = propertyValue as IDictionary;
                if (dictionary != null)
                {
                    bool foldout = GetFoldout(propertyValue, indent);
                    foldout = DrawEntryWithFoldout(propertyName, foldout, indent, editable,
                        drawer: () => EditorGUILayout.LabelField($"({FriendlyName(propertyType)})"));
                    if (foldout)
                    {
                        var (keyType, valueType) = GetElementTypeOfDictionary(propertyType);
                        foreach (DictionaryEntry e in dictionary)
                        {
                            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                            {
                                DrawProperty("Key", keyType, e.Key, resource, indent + 1, editable);
                                DrawProperty("Value", valueType, e.Value, resource, indent + 1, editable);
                            }
                        }
                    }
                    _foldout[propertyValue] = foldout;
                }
            }
            else if (propertyType.GetInterface(nameof(IEnumerable)) != null)
            {
                var enumerable = propertyValue as IEnumerable;
                if (enumerable != null)
                {
                    // bool foldout = GetFoldout(propertyValue, indent);
                    // foldout = DrawEntryWithFoldout(propertyName, foldout, indent, editable,
                    //     drawer: () => EditorGUILayout.LabelField($"({FriendlyName(propertyType)})"));
                    // if (foldout)
                    // {
                        var elementType = GetElementTypeOfEnumerable(propertyType);
                        int i = 0;
                        foreach (var e in enumerable)
                            DrawProperty($"{propertyName}[{i++}]", elementType, e, resource, indent + 1, editable);
                    // }
                    //_foldout[propertyValue] = foldout;
                }
            }
            else if ((propertyType.IsValueType || propertyType.IsClass) && propertyValue != null)
            {
                bool foldout = GetFoldout(propertyValue, indent);
                foldout = DrawEntryWithFoldout(propertyName, foldout, indent, editable, drawer: () => EditorGUILayout.LabelField($"({FriendlyName(propertyType)})"));
                if (foldout)
                    DrawPlainStructure(propertyValue, resource, indent + 1, editable);
                _foldout[propertyValue] = foldout;
            }
            else
                DrawEntry(propertyName, indent, editable, drawer: () =>
                    EditorGUILayout.TextField(propertyValue != null ? propertyValue.ToString() : "<null>", GUILayout.MinWidth(FieldMinWidth))
                );


            return propertyValue;
        }

        private void DrawSubResource(string name, IResource resource, IResource parent, int indent, bool editable)
        {
            if (resource is CalcerConstantDef<float>)
            {
                var calcer = (CalcerConstantDef<float>) resource;
                calcer.Value = (float) DrawProperty(name, typeof(float), calcer.Value, parent, indent, editable);
            }
            else
            if (resource is CalcerConstantDef<bool>)
            {
                var calcer = (CalcerConstantDef<bool>) resource;
                calcer.Value = (bool) DrawProperty(name, typeof(bool), calcer.Value, parent, indent, editable);
            }
            else if (resource is CalcerStatDef)
            {
                DrawEntry(name, indent, true, drawer: () => DrawJdbRef(((CalcerStatDef) resource).Stat.Target, editable));
            }
            else
            {
                DrawSubResourceImpl(name, resource, parent, indent, editable);
            }
        }

        private void DrawSubResourceImpl(string name, IResource resource, IResource parent, int indent, bool editable)
        {
            bool foldout = false;
            using (new EditorGUILayout.HorizontalScope())
            {
                Action drawer = () =>
                {
                    if (resource == null || !string.IsNullOrEmpty(resource.Address.Root) && parent.Address.Root != resource.Address.Root)
                        DrawJdbRef(resource, editable);
                    else
                        using (new EditorGUI.DisabledScope(!editable))
                            EditorGUILayout.LabelField($"({GetResourceTypeName(resource)})");
                };

                if (!IsStructureHidden(resource))
                {
                    foldout = GetFoldout(resource, indent);
                    foldout = DrawEntryWithFoldout(name, foldout, indent, true, drawer);
                    _foldout[resource] = foldout;
                }
                else
                    DrawEntry(name, indent, true, drawer);
            }

            if (resource == null)
                return;

            if (!foldout)
                return;

//            using (new EditorGUI.IndentLevelScope())
//            EditorGUI.indentLevel++;
            {
//                var labelWidth = EditorGUIUtility.labelWidth;
                //   EditorGUIUtility.labelWidth += LabelIndent;
                DrawResource(resource, indent + 1, editable);
//                EditorGUIUtility.labelWidth = labelWidth;
            }
//            EditorGUI.indentLevel--;
        }
        
        private void DrawPlainStructure(object structure, IResource parent, int indent, bool editable)
        {
            foreach (var member in GetProperties(structure))
            {
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    if (member is PropertyInfo)
                    {
                        var property = (PropertyInfo) member;
                        if (property.CanRead && property.CanWrite)
                        {
                            object value = property.GetValue(structure);
                            value = DrawProperty(property.Name, property.PropertyType, value, parent, indent, editable);
                            if (changeScope.changed)
                                property.SetValue(structure, value);
                        }
                        else
                        if (property.CanRead)
                        {
                            object value = property.GetValue(structure);
                            DrawProperty(property.Name, property.PropertyType, value, parent, indent, false);
                        }
                    }
                    else if (member is FieldInfo)
                    {
                        var field = (FieldInfo) member;
                        object value = field.GetValue(structure);
                        value = DrawProperty(field.Name, field.FieldType, value, parent, indent, editable);
                        if (changeScope.changed)
                            field.SetValue(structure, value);
                    }
                }
            }
        }


        private void DrawEntry(string name, int indent, bool enabled, Action drawer)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(indent * IndentSize);
                EditorGUILayout.LabelField(name, GUILayout.Width(EditorGUIUtility.labelWidth));
                using (new EditorGUI.DisabledScope(!enabled))
                    drawer();
            }
        }

        private bool DrawEntryWithFoldout(string name, bool foldout, int indent, bool enabled, Action drawer)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(indent * IndentSize);
                //              foldout = EditorGUILayout.Foldout(foldout, name, true, style);
                foldout = EditorGUILayout.Toggle(foldout, EditorStyles.foldout, GUILayout.Width(FoldoutWidth));
                EditorGUILayout.LabelField(name, GUILayout.Width(EditorGUIUtility.labelWidth - FoldoutWidth));
                using (new EditorGUI.DisabledScope(!enabled))
                    drawer();
            }

            return foldout;
        }

        private void DrawJdbRef(IResource resource, bool enabled)
        {
            var file = resource != null ? Path.GetFileName(resource.Address.Root) : "<null>";
            var content = new GUIContent(file, $"{resource?.Address.Root} ({GetResourceTypeName(resource)})");
            using (new EditorGUI.DisabledScope(resource == null))
            if (GUILayout.Button(content, new GUIStyle("ObjectField"), GUILayout.MinWidth(FieldMinWidth)) && resource != null)
            {
                var jdbAsset = AssetDatabase.LoadAssetAtPath(JdbAssetPath(resource.Address.Root), typeof(JdbMetadata));
                if (jdbAsset)
                    EditorGUIUtility.PingObject(jdbAsset);
            }
        }

        private IEnumerable<MemberInfo> GetProperties(object resource)
        {
            return resource?.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(member => (member is PropertyInfo || member is FieldInfo) && !HiddenFields.Any(x => x != null && x.Name == member.Name && x.DeclaringType == member.DeclaringType));
        }
        
        private bool IsStructureHidden(IResource resource)
        {
            if (resource == null)
                return true;
            if (Array.IndexOf(HideStructureFor, resource.GetType()) != -1)
                return true;
            var resType = resource.GetType();
            bool isHidden;
            if (!_hideStructureFor.TryGetValue(resType, out isHidden))
                _hideStructureFor.Add(resType, isHidden = !GetProperties(resource).Any());
            return isHidden;
        }

        private static string JdbAssetPath(string rootAddress)
        {
            if (string.IsNullOrWhiteSpace(rootAddress))
                return null;
            var jdbPath = Path.Combine("Assets", rootAddress.TrimStart('/')).Replace('\\', '/') + ".jdb";
            return jdbPath;
        }

        private static Type GetElementTypeOfEnumerable(Type type)
        {
            var interfaces = type.GetInterfaces();
            return interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(i => i.GetGenericArguments()[0])
                .FirstOrDefault();
        }

        private static (Type, Type) GetElementTypeOfDictionary(Type type)
        {
            var interfaces = type.GetInterfaces();
            return interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                .Select(i => (i.GetGenericArguments()[0], i.GetGenericArguments()[1]))
                .FirstOrDefault();
        }
        
        private static Type GetRefTypeOfUnityRef(Type unityRefType)
        {
            if (unityRefType.IsGenericType && unityRefType.GetGenericTypeDefinition() == typeof(UnityRef<>))
                return unityRefType.GetGenericArguments()[0];
            var baseType = unityRefType.BaseType;
            if (baseType != null)
                return GetRefTypeOfUnityRef(baseType);
            return null;
        }

        private static string GetResourceTypeName(IResource resource)
        {
            if (resource == null)
                return string.Empty;

         //   if (DontShowTypeFor.Any(x => x.IsInstanceOfType(resource)))
          //      return string.Empty;

            var name = FriendlyName(resource.GetType());

            var type = RemovePrefixesFor.FirstOrDefault(x => x.IsInstanceOfType(resource));
            if (type != null)
                name = RemovePrefix(name, type.Name);

            return name;
        }

        private static string RemovePrefix(string str, string prefix)
        {
            if (!string.IsNullOrEmpty(prefix) && str.StartsWith(prefix))
                return str.Substring(prefix.Length);
            return str;
        }

        private static string RemoveSuffix(string str, string suffix)
        {
            if (!string.IsNullOrEmpty(suffix) && str.EndsWith(suffix))
                return str.Substring(0, str.Length - suffix.Length);
            return str;
        }

        private static string FriendlyName(Type type)
        {
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(ResourceRef<>))
                    return FriendlyName(type.GetGenericArguments()[0]);
                string friendlyName = type.Name;
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName = RemoveSuffix(friendlyName, "Def");
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = FriendlyName(typeParameters[i]);
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
                return friendlyName.Replace("+", ".");
            }
            else
                return RemoveSuffix(type.Name, "Def");
        }

        private bool GetFoldout(object propertyValue, int indent) => !_foldout.TryGetValue(propertyValue, out var foldout) ? indent < _initialFoldoutLevel : foldout;
    }
}