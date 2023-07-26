using Assets.Src.ResourcesSystem.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using System.Collections;
using NLog;
using Core.Reflection;
using ResourcesSystem.Loader;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;

namespace ResourceSystemTools
{
    public static class SchemaGenerator
    {
        internal static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(SchemaGenerator));

        private const string ObjectFieldKey = "ObjectField";
        private const string FileName = "DEFS_SCHEMA.json";


#if UNITY_EDITOR
        [UnityEditor.MenuItem("Build/Utility/Generate Json Schema", false, 1)]
        public static void GenerateSchema()
        {
            var dirInfoAssets = new DirectoryInfo(UnityEngine.Application.dataPath);
            GenerateSchemaImpl(dirInfoAssets.Parent);
        }
#endif

        public static void GenerateSchemaImpl(DirectoryInfo targetPath)
        {
            //relative to sln
            var jobj = GenerateSchemaObj();
            var filePath = Path.Combine(targetPath.FullName, FileName);
            var fileInfo = new FileInfo(filePath);
            using (StreamWriter file = File.CreateText(fileInfo.FullName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.None;
                serializer.Serialize(file, jobj);
            }
            Logger.IfInfo()?.Message($"{nameof(GenerateSchemaImpl)} Done. Size: {fileInfo.Length} B").Write();
        }

        static HashSet<Type> _defs;
        static HashSet<Type> _serializableTypes;
        static HashSet<Type> _allSerializableTypes;
        static HashSet<Type> _typesThatHaveSubclasses;
        static HashSet<Type> _types;
        private static JObject GenerateSchemaObj()
        {
            _defs = new HashSet<Type>();
            _types = new HashSet<Type>();
            _serializableTypes = new HashSet<Type>();
            _allSerializableTypes = new HashSet<Type>();
            _typesThatHaveSubclasses = new HashSet<Type>();

            var assemblyResourceSystem = AppDomain.CurrentDomain.GetAssembliesSafe().Single(v => v.GetName().Name == "ResourceSystem");
            var assemblyLoader = AppDomain.CurrentDomain.GetAssembliesSafe().Single(v => v.GetName().Name == "ResourceSystemLoader");
            var allTypes = assemblyResourceSystem.GetTypesSafe().Concat(assemblyLoader.GetTypesSafe()).OrderBy(v => v.FullName).ToArray();

            foreach (var type in allTypes)
            {
                if (typeof(IResource).IsAssignableFrom(type) && !type.IsGenericType)
                {
                    _defs.Add(type);
                    if(type.BaseType != null && type.BaseType.IsGenericType)
                    {
                        if (!_defs.Contains(type.BaseType))
                            _defs.Add(type.BaseType);
                    }
                }
                if (type.GetCustomAttribute<KnownToGameResourcesAttribute>() != null && type.GetCustomAttribute<KnownToGameResourcesAttribute>().Serializable)
                {
                    _allSerializableTypes.Add(type);
                    _serializableTypes.Add(type);
                }
                if (type.FullName.StartsWith("Assets") && !type.IsGenericType || _defs.Contains(type) || _allSerializableTypes.Contains(type))
                    _types.Add(type);
            }

            JObject schema = new JObject();
            var props = new JObject();
            schema.Add("properties", props);

            var typeConstraints = new JObject();
            props.Add("$type", typeConstraints);
            //schema.Add("required", new JArray(new[] { "$type" }));
            typeConstraints.Add("type", JToken.FromObject("string"));
            List<string> validNames = new List<string>();
            foreach (var defType in _defs)
            {
                validNames.Add(defType.NiceName());
                if (defType.NiceName().EndsWith("Def"))
                    validNames.Add(defType.NiceName().Substring(0, defType.NiceName().Length - 3));
            }
            typeConstraints.Add("enum", new JArray(validNames.ToArray()));

            var definitions = new JObject();
            schema.Add("definitions", definitions);
            foreach (var type in _defs.ToList())
            {
                definitions.Add(type.NiceName(), GenerateDefForClass(type));
                definitions.Add(type.NiceName() + "Field", GenerateDefForClassField(type));
            }
            {
                var refType = new JObject();
                refType.Add("type", JToken.FromObject("string"));
                var enumArray = new JArray();
                foreach (var eName in _types)
                    enumArray.Add(JToken.FromObject(eName.NiceName()));
                refType.Add("enum", enumArray);
                enumArray.Add("float");
                enumArray.Add("int");
                enumArray.Add("bool");
                enumArray.Add("string");
                definitions.Add("TYPES_ENUM", refType);
            }

            var subTypesValidators = new JArray();
            schema.Add("allOf", subTypesValidators);
            subTypesValidators.Add(RefObject("BaseResourceField"));

            while (_serializableTypes.Count > 0)
            {
                var types = _serializableTypes.ToList();
                _serializableTypes.Clear();
                foreach (var serClass in types)
                {
                    try
                    {
                        definitions.Add(serClass.NiceName(), GenerateDefForClass(serClass));
                        definitions.Add(serClass.NiceName() + "Field", GenerateDefForClassField(serClass));
                    }
                    catch (Exception e)
                    {
                        Logger.IfError()?.Exception(e).Write();
                    }
                }
            }
            {
                var objectDef = new JObject();
                var oneOf = new JArray();
                objectDef.Add("oneOf", oneOf);
                foreach (var def in definitions)
                    if (!def.Key.Contains("Field"))
                        oneOf.Add(RefObject(def.Key));
                {
                    var numberConstraint = new JObject();
                    GeneratePrimitiveTypeConstraint(numberConstraint, typeof(float));
                    oneOf.Add(numberConstraint);
                    var booleanConstraint = new JObject();
                    GeneratePrimitiveTypeConstraint(booleanConstraint, typeof(bool));
                    oneOf.Add(booleanConstraint);
                    var stringConstraint = new JObject();
                    GeneratePrimitiveTypeConstraint(stringConstraint, typeof(string));
                    oneOf.Add(stringConstraint);
                }
                definitions.Add(ObjectFieldKey, objectDef);
            }
            return schema;
        }
        static JToken Ref(string to) => JToken.FromObject($"#/definitions/{to}");
        static JObject RefObject(string to)
        {
            var obj = new JObject();
            obj.Add("$ref", Ref(to));
            return obj;
        }
        static JToken StringType => JToken.FromObject("string");
        static JObject StringTypeObject()
        {
            var obj = new JObject();
            obj.Add("type", StringType);
            return obj;
        }
        static JObject ObjectTypeObject()
        {
            var obj = new JObject();
            obj.Add("type", JToken.FromObject("object"));
            return obj;
        }
        static JObject ObjectTypeObjectWithFields(string refToAddProps)
        {
            var obj = new JObject();
            obj.Add("type", JToken.FromObject("object"));
            obj.Add("additionalProperties", RefObject(refToAddProps));
            return obj;
        }
        static JObject GenerateDefForClass(Type classType)
        {
            var obj = new JObject();
            obj.Add("type", "object");
            obj.Add("additionalProperties", false);
            var properties = new JObject();
            obj.Add("properties", properties);
            if (typeof(IResource).IsAssignableFrom(classType))
            {
                properties.Add("$id", StringTypeObject());
                var protoObj = new JObject();
                protoObj.Add("allOf", new JArray(RefObject(classType.NiceName() + "Field")));
                protoObj.Add("dependecies", new JArray("$overrideVars"));

                properties.Add("$proto", protoObj);
                properties.Add("$overrideVars", ObjectTypeObjectWithFields(ObjectFieldKey));
                obj.Add("required", new JArray(new[] { "$type" }));
                properties.Add("$vars", ObjectTypeObject());
            }
            var typeObj = new JObject();
            properties.Add("$type", typeObj);
            typeObj.Add("type", StringType);
            if (classType.NiceName().EndsWith("Def"))
                typeObj.Add("enum", new JArray(new[] { classType.NiceName(), classType.NiceName().Substring(0, classType.NiceName().Length - 3) }));
            else
                typeObj.Add("enum", new JArray(new[] { classType.NiceName() }));
            var props = classType.GetProperties().Where(x => !x.CustomAttributes.Any(v => v.AttributeType == typeof(NotInSchemaAttribute)) && x.CanWrite).OrderBy(v => v.Name);
            foreach (var prop in props)
            {
                properties.Add(prop.Name, GenerateSchemaForProp(prop.PropertyType));
            }
            var fields = classType.GetFields().Where(x => !x.CustomAttributes.Any(v => v.AttributeType == typeof(NotInSchemaAttribute))).OrderBy(v => v.Name);
            foreach (var prop in fields)
            {
                properties.Add(prop.Name, GenerateSchemaForProp(prop.FieldType));
            }
            return obj;
        }
        static List<string> _names = new List<string>();
        static JObject GenerateDefForClassField(Type classType)
        {
            var obj = new JObject();
            var oneOf = new JArray();
            var properties = new JObject();
            obj.Add("properties", properties);
            var type = new JObject();
            properties.Add("$type", type);
            type.Add("type", StringType);
            _names.Clear();
            foreach (var def in _defs.Where(x => classType.IsAssignableFrom(x)))
            {
                _names.Add(def.NiceName());
                if (def.NiceName().EndsWith("Def"))
                {
                    _names.Add(def.NiceName().Substring(0, def.NiceName().Length - 3));
                }
            }
            foreach (var def in _allSerializableTypes.Where(x => classType.IsAssignableFrom(x)))
                _names.Add(def.NiceName());
            type.Add("enum", new JArray(_names.ToArray()));
            if (!_allSerializableTypes.Contains(classType) || (_allSerializableTypes.Contains(classType) && _typesThatHaveSubclasses.Contains(classType)))
                obj.Add("required", new JArray(new[] { "$type" }));
            obj.Add("oneOf", oneOf);
            oneOf.Add(StringTypeObject());
            foreach (var def in _defs)
                if (classType.IsAssignableFrom(def))
                    oneOf.Add(RefObject(def.NiceName()));
            foreach (var serType in _allSerializableTypes)
                if (classType.IsAssignableFrom(serType))
                    oneOf.Add(RefObject(serType.NiceName()));
            if (typeof(IResource).IsAssignableFrom(classType))
            {
                var vars = new JObject();
                properties.Add("$vars", vars);
                vars.Add("additionalProperties", RefObject("TemplateVariableField"));
                if (classType.GetCustomAttribute<CanBeCreatedFromAliasedPrimitiveAttribute>(inherit: true) != null)
                {
                    var attr = classType.GetCustomAttribute<CanBeCreatedFromAliasedPrimitiveAttribute>(inherit: true);
                    var alias = new JObject();
                    GeneratePrimitiveTypeConstraint(alias, attr.PrimitiveType);
                    if (attr.PrimitiveType != typeof(string))
                        oneOf.Add(alias);
                }
            }
            return obj;
        }
        static JObject GenerateDefForClassProp(Type classType)
        {
            if (!typeof(IResource).IsAssignableFrom(classType) && classType != typeof(object))
                if (_allSerializableTypes.Add(classType))
                    if (_serializableTypes.Add(classType))
                    {
                        foreach (var type in _types)
                        {
                            if (classType.IsAssignableFrom(type) && classType != type)
                            {
                                _typesThatHaveSubclasses.Add(classType);
                                if (!typeof(IResource).IsAssignableFrom(type) && type != typeof(object))
                                    if (_allSerializableTypes.Add(type))
                                        _serializableTypes.Add(type);
                            }
                        }
                    }
            if (classType.IsGenericType && classType.GetGenericTypeDefinition() == typeof(UnityRef<>))
                return RefObject("UnityRef" + "Field");
            else
                return RefObject(classType.NiceName() + "Field");
        }
        static JToken GenerateSchemaForProp(Type propType)
        {
            var type = propType;
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                if (!type.IsGenericType)
                    type = type.BaseType;
                var constraints = GenerateSchemaForProp(type.GetGenericArguments()[1]);
                var obj = new JObject();
                obj.Add("type", JToken.FromObject("object"));
                obj.Add("additionalProperties", constraints);
                return obj;
            }
            else if (typeof(IList).IsAssignableFrom(type) && !type.IsArray)
            {
                var obj = new JObject();
                obj.Add("type", JToken.FromObject("array"));
                if (!type.IsGenericType)
                    return obj;
                var constraints = GenerateSchemaForProp(type.GetGenericArguments()[0]);
                obj.Add("items", constraints);
                return obj;
            }
            else if (type.IsArray)
            {
                var obj = new JObject();
                obj.Add("type", JToken.FromObject("array"));
                var constraints = GenerateSchemaForProp(type.GetElementType());
                obj.Add("items", constraints);
                return obj;
            }

            return GetDefForProp(type);
        }
        static JToken GetDefForProp(Type type)
        {
            if (typeof(Type) == type)
            {

                var refType = new JObject();
                refType.Add("$ref", JToken.FromObject("#/definitions/TYPES_ENUM"));
                return refType;
            }
            else if (type.IsPrimitive || type == typeof(string) || type == typeof(float?))
            {
                var propObj = new JObject();
                GeneratePrimitiveTypeConstraintWithVarSupport(propObj, type);
                return propObj;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ResourceRef<>))
            {
                return GenerateDefForRefField(type.GetGenericArguments()[0]);
            }
            else if (type.IsClass || (type.IsValueType && !type.IsPrimitive && !type.IsEnum))
            {
                return GenerateDefForClassProp(type);
            }
            else if (type.IsEnum)
            {
                var refType = new JObject();
                refType.Add("type", JToken.FromObject("string"));
                var enumArray = new JArray();
                foreach (var eName in Enum.GetNames(type))
                    enumArray.Add(JToken.FromObject(eName));
                refType.Add("enum", enumArray);
                return refType;

            }
            return new JObject();
        }
        static JToken GenerateDefForRefField(Type fieldType)
        {
            if(!_defs.Contains(fieldType))
            {
                _defs.Add(fieldType);
                _serializableTypes.Add(fieldType);
            }
            if(!_defs.Contains(fieldType.BaseType) && fieldType.BaseType != null && fieldType.BaseType != typeof(object))
            {
                _defs.Add(fieldType.BaseType);
                _serializableTypes.Add(fieldType.BaseType);
            }
            return RefObject(fieldType.NiceName() + "Field");
        }


        private static void GenerateSchemaForProp(PropertyInfo prop, JObject subProps)
        {
            try
            {

                var type = prop.PropertyType;
                if (prop.GetCustomAttribute<NotInSchemaAttribute>() != null)
                    return;


            }
            catch (Exception e)
            {
                Logger.IfError()?.Exception(e).Write();
            }
        }
        private static void GeneratePrimitiveTypeConstraintWithVarSupport(JObject refType, Type type)
        {

            if (typeof(string) == type)
            {
                refType.Add("type", JToken.FromObject("string"));
                return;
            }
            var oneOf = new JArray();
            refType.Add("oneOf", oneOf);
            var stringConstraint = new JObject();
            stringConstraint.Add("type", JToken.FromObject("string"));
            oneOf.Add(stringConstraint);
            var prim = new JObject();
            oneOf.Add(prim);
            if (typeof(float) == type)
            {
                prim.Add("type", JToken.FromObject("number"));
            }
            else if (typeof(float?) == type)
            {
                prim.Add("type", JToken.FromObject("number"));
            }
            else if (typeof(double) == type)
            {
                prim.Add("type", JToken.FromObject("number"));
            }
            else if (typeof(int) == type)
            {
                prim.Add("type", JToken.FromObject("integer"));
            }
            else if (typeof(uint) == type)
            {
                prim.Add("type", JToken.FromObject("integer"));
            }
            else if (typeof(ulong) == type)
            {
                prim.Add("type", JToken.FromObject("integer"));
            }
            else if (typeof(long) == type)
            {
                prim.Add("type", JToken.FromObject("integer"));
            }
            else if (typeof(bool) == type)
            {
                prim.Add("type", JToken.FromObject("boolean"));
            }
        }
        private static void GeneratePrimitiveTypeConstraint(JObject refType, Type type)
        {

            if (typeof(string) == type)
            {
                refType.Add("type", JToken.FromObject("string"));
            }
            else if (typeof(float) == type)
            {
                refType.Add("type", JToken.FromObject("number"));
            }
            else if (typeof(float?) == type)
            {
                refType.Add("type", JToken.FromObject("number"));
            }
            else if (typeof(double) == type)
            {
                refType.Add("type", JToken.FromObject("number"));
            }
            else if (typeof(int) == type)
            {
                refType.Add("type", JToken.FromObject("integer"));
            }
            else if (typeof(uint) == type)
            {
                refType.Add("type", JToken.FromObject("integer"));
            }
            else if (typeof(ulong) == type)
            {
                refType.Add("type", JToken.FromObject("integer"));
            }
            else if (typeof(long) == type)
            {
                refType.Add("type", JToken.FromObject("integer"));
            }
            else if (typeof(bool) == type)
            {
                refType.Add("type", JToken.FromObject("boolean"));
            }
        }
    }

}
