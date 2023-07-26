using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Assets.Src.GameObjectAssembler
{
    internal static class FieldAssigners
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger("JsonToGO");

        private static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> CreateTransferValueDelegateForField<TSrcType, TDstType, TSrcValueType, TDstValueType>(PropertyInfo srcProp, FieldInfo dstField) where TDstType : Component
        {
            var getDel = DelegateCreator.CreateGetPropertyDelegate<TSrcType, TSrcValueType>(srcProp);
            var setDel = DelegateCreator.CreateSetFieldDelegate<TDstType, TSrcValueType>(dstField);

            return (IComponentDef src, Component dst, IReadOnlyDictionary<IComponentDef, Component> index) => setDel((TDstType)dst, getDel((TSrcType)src));
        }

        private static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> CreateTransferRefValueDelegateForField<TSrcType, TDstType, TSrcValueType, TDstValueType>(PropertyInfo srcProp, FieldInfo dstField) where TSrcValueType : class, IResource where TDstValueType : TSrcValueType where TDstType : Component
        {
            var getDel = DelegateCreator.CreateGetPropertyDelegate<TSrcType, ResourceRef<TSrcValueType>>(srcProp);
            var setDel = DelegateCreator.CreateSetFieldDelegate<TDstType, TDstValueType>(dstField);

            return (IComponentDef src, Component dst, IReadOnlyDictionary<IComponentDef, Component> index) => setDel((TDstType)dst, (TDstValueType)getDel((TSrcType)src).Target);
        }

        private static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> CreateTransferComponentValueDelegateForField<TSrcType, TDstType, TDefType, TInstanceType>(PropertyInfo srcProp, FieldInfo dstField) where TDefType : class, IComponentDef where TInstanceType : class where TDstType : Component
        {
            var getDel = DelegateCreator.CreateGetPropertyDelegate<TSrcType, ResourceRef<TDefType>>(srcProp);
            var setDel = DelegateCreator.CreateSetFieldDelegate<TDstType, TInstanceType>(dstField);

            Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> result = (IComponentDef src, Component dst, IReadOnlyDictionary<IComponentDef, Component> index) =>
            {
                var val = getDel((TSrcType)src).Target;
                if (val == null)
                    return; // ok, null ref

                Component dstComp;
                if (!index.TryGetValue(val, out dstComp))
                {
                    Logger.Error("Field {0} of type {1} of component {2} has ref to component not in list of components",
                        srcProp.Name, srcProp.PropertyType, typeof(TSrcType));
                    return;
                }
                setDel((TDstType)dst, dstComp as TInstanceType);
            };

            return result;
        }

        private static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> CreateInexactTransferValueDelegateForField(PropertyInfo srcProp, FieldInfo dstField)
        {
            var valueType = srcProp.PropertyType;
            MethodInfo generic;
            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(ResourceRef<>))
            {
                valueType = valueType.GetGenericArguments()[0];

                if (typeof(IComponentDef).IsAssignableFrom(valueType))
                {
                    generic = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(CreateTransferComponentValueDelegateForField), BindingFlags.Static | BindingFlags.NonPublic);
                }
                else
                {
                    generic = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(CreateTransferRefValueDelegateForField), BindingFlags.Static | BindingFlags.NonPublic);

                    if (!dstField.FieldType.IsAssignableFrom(valueType))
                    {
                        throw new InvalidCastException($"Cannot assign field {dstField} to value of {srcProp}");
                    }
                }
            }
            else
            {
                generic = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(CreateTransferValueDelegateForField), BindingFlags.Static | BindingFlags.NonPublic);

                if (!dstField.FieldType.IsAssignableFrom(valueType))
                {
                    throw new InvalidCastException($"Cannot assign field {dstField} to value of {srcProp}");
                }
            }

            var strict = generic.MakeGenericMethod(srcProp.DeclaringType, dstField.DeclaringType, valueType, dstField.FieldType);
            var result = (Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>>)strict.Invoke(null, new object[] { srcProp, dstField });
            return result;
        }

        private struct KeyForField
        {
            public readonly PropertyInfo Src;
            public readonly FieldInfo Dst;

            public KeyForField(PropertyInfo src, FieldInfo dst)
            {
                Src = src;
                Dst = dst;
            }
        }


        private static readonly Dictionary<KeyForField, Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>>> TransferDelegatesForFields = new Dictionary<KeyForField, Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>>>();

        private static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> GetOrCreateInexactTransferValueDelegateForField(PropertyInfo srcProp, FieldInfo dstField)
        {
            Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> result;
            var key = new KeyForField(srcProp, dstField);
            if (TransferDelegatesForFields.TryGetValue(key, out result))
                return result;

            result = CreateInexactTransferValueDelegateForField(srcProp, dstField);
            TransferDelegatesForFields.Add(key, result);
            return result;
        }





        private static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> CreateTransferValueDelegateForProperty<TSrcType, TDstType, TSrcValueType, TDstValueType>(PropertyInfo srcProp, PropertyInfo dstField) where TDstType : Component
        {
            var getDel = DelegateCreator.CreateGetPropertyDelegate<TSrcType, TSrcValueType>(srcProp);
            var setDel = DelegateCreator.CreateSetPropertyDelegate<TDstType, TSrcValueType>(dstField);

            return (IComponentDef src, Component dst, IReadOnlyDictionary<IComponentDef, Component> index) => setDel((TDstType)dst, getDel((TSrcType)src));
        }

        private static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> CreateTransferValueDelegateForPropertyWithConverter<TSrcType, TDstType, TSrcValueType, TDstValueType>(PropertyInfo srcProp, PropertyInfo dstField, Func<TSrcValueType,TDstValueType> converter) where TDstType : Component
        {
            var getDel = DelegateCreator.CreateGetPropertyDelegate<TSrcType, TSrcValueType>(srcProp);
            var setDel = DelegateCreator.CreateSetPropertyDelegate<TDstType, TDstValueType>(dstField);

            return (IComponentDef src, Component dst, IReadOnlyDictionary<IComponentDef, Component> index) => setDel((TDstType)dst, converter.Invoke(getDel((TSrcType)src)));
        }
        
        private static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> CreateTransferRefValueDelegateForProperty<TSrcType, TDstType, TSrcValueType, TDstValueType>(PropertyInfo srcProp, PropertyInfo dstField) where TSrcValueType : class, IResource where TDstValueType : TSrcValueType where TDstType : Component
        {
            var getDel = DelegateCreator.CreateGetPropertyDelegate<TSrcType, ResourceRef<TSrcValueType>>(srcProp);
            var setDel = DelegateCreator.CreateSetPropertyDelegate<TDstType, TDstValueType>(dstField);

            return (IComponentDef src, Component dst, IReadOnlyDictionary<IComponentDef, Component> index) => setDel((TDstType)dst, (TDstValueType)getDel((TSrcType)src).Target);
        }
        
        private static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> CreateTransferRefValueDelegateForPropertyWithConverter<TSrcType, TDstType, TSrcValueType, TDstValueType>(PropertyInfo srcProp, PropertyInfo dstField, Func<TSrcValueType,TDstValueType> converter) where TSrcValueType : class, IResource where TDstValueType : TSrcValueType where TDstType : Component
        {
            var getDel = DelegateCreator.CreateGetPropertyDelegate<TSrcType, ResourceRef<TSrcValueType>>(srcProp);
            var setDel = DelegateCreator.CreateSetPropertyDelegate<TDstType, TDstValueType>(dstField);

            return (IComponentDef src, Component dst, IReadOnlyDictionary<IComponentDef, Component> index) => setDel((TDstType)dst, converter(getDel((TSrcType)src).Target));
        }

        private static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> CreateTransferComponentValueDelegateForProperty<TSrcType, TDstType, TDefType, TInstanceType>(PropertyInfo srcProp, PropertyInfo dstProp) where TDefType : class, IComponentDef where TInstanceType : class where TDstType : Component
        {
            var getDel = DelegateCreator.CreateGetPropertyDelegate<TSrcType, ResourceRef<TDefType>>(srcProp);
            var setDel = DelegateCreator.CreateSetPropertyDelegate<TDstType, TInstanceType>(dstProp);

            Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> result = (IComponentDef src, Component dst, IReadOnlyDictionary<IComponentDef, Component> index) =>
            {
                var val = getDel((TSrcType)src).Target;
                if (val == null)
                    return; // ok, null ref

                Component dstComp;
                if (!index.TryGetValue(val, out dstComp))
                {
                    Logger.Error("Field {0} of type {1} of component {2} has ref to component not in list of components",
                        srcProp.Name, srcProp.PropertyType, typeof(TSrcType));
                    return;
                }
                setDel((TDstType)dst, dstComp as TInstanceType);
            };

            return result;
        }

        private static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> CreateInexactTransferValueDelegateForProperty(PropertyInfo srcProp, PropertyInfo dstField)
        {
            var valueType = srcProp.PropertyType;
            MethodInfo generic;
            Delegate converter = null;
            if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(ResourceRef<>))
            {
                valueType = valueType.GetGenericArguments()[0];

                if (typeof(IComponentDef).IsAssignableFrom(valueType))
                {
                    generic = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(CreateTransferComponentValueDelegateForProperty), BindingFlags.Static | BindingFlags.NonPublic);
                }
                else
                {
                    if (dstField.PropertyType.IsAssignableFrom(valueType))
                        generic = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(CreateTransferRefValueDelegateForProperty), BindingFlags.Static | BindingFlags.NonPublic);
                    else
                    if (Converters.TryGet((valueType, dstField.PropertyType), out converter))
                        generic = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(CreateTransferRefValueDelegateForPropertyWithConverter), BindingFlags.Static | BindingFlags.NonPublic);
                    else
                        throw new InvalidCastException($"Cannot assign field {dstField} to value of {srcProp}");
                }
            }
            else
            {
                if (dstField.PropertyType.IsAssignableFrom(valueType))
                    generic = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(CreateTransferValueDelegateForProperty), BindingFlags.Static | BindingFlags.NonPublic);
                else
                if (Converters.TryGet((valueType, dstField.PropertyType), out converter))
                    generic = MethodBase.GetCurrentMethod().DeclaringType.GetMethod(nameof(CreateTransferValueDelegateForPropertyWithConverter), BindingFlags.Static | BindingFlags.NonPublic);
                else
                    throw new InvalidCastException($"Cannot assign field {dstField} to value of {srcProp}");
            }

            try
            {
                var strict = generic.MakeGenericMethod(srcProp.DeclaringType, dstField.DeclaringType, valueType, dstField.PropertyType);
                var result = converter == null
                        ? (Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>>) strict.Invoke(null, new object[] {srcProp, dstField})
                        : (Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>>) strict.Invoke(null, new object[] {srcProp, dstField, converter});
                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Cannot create property transfer delegate {srcProp.DeclaringType.FullName}.{srcProp.Name} -> {dstField.DeclaringType.FullName}.{dstField.DeclaringType.FullName}: {e}");
            }
        }

        private struct KeyForProp
        {
            public readonly PropertyInfo Src;
            public readonly PropertyInfo Dst;

            public KeyForProp(PropertyInfo src, PropertyInfo dst)
            {
                Src = src;
                Dst = dst;
            }
        }

        private static readonly Dictionary<KeyForProp, Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>>> TransferDelegatesForProperties = new Dictionary<KeyForProp, Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>>>();

        private static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> GetOrCreateInexactTransferValueDelegateForProperty(PropertyInfo srcProp, PropertyInfo dstField)
        {
            Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> result;
            var key = new KeyForProp(srcProp, dstField);
            if (TransferDelegatesForProperties.TryGetValue(key, out result))
                return result;

            result = CreateInexactTransferValueDelegateForProperty(srcProp, dstField);
            TransferDelegatesForProperties.Add(key, result);
            return result;
        }

        [CanBeNull]
        private static FieldInfo SearchForField([NotNull] Type type, [NotNull] string name)
        {
            var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null)
                return field;
            field = type.GetField("m_" + name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null)
                return field;
            field = type.GetField("_" + name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null)
                return field;
            return null;
        }

        [CanBeNull]
        private static PropertyInfo SearchForProperty([NotNull]Type type, [NotNull] string name)
        {
            var field = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null)
                return field;
            field = type.GetProperty("m_" + name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null)
                return field;
            field = type.GetProperty("_" + name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null)
                return field;
            return null;
        }

        [NotNull]
        public static Action<IComponentDef, Component, IReadOnlyDictionary<IComponentDef, Component>> CreateAssignFieldOrProperty([NotNull] PropertyInfo srcField, [NotNull] Type dstType)
        {
            var name = srcField.Name;

            var dstField = SearchForField(dstType, name);
            if (dstField != null)
            {
                return GetOrCreateInexactTransferValueDelegateForField(srcField, dstField);
            }

            var dstProp = SearchForProperty(dstType, name);
            if (dstProp != null)
            {
                return GetOrCreateInexactTransferValueDelegateForProperty(srcField, dstProp);
            }
            Logger.IfError()?.Message("Failed to match field {0} of component {1} to {2}",  srcField.Name, srcField.DeclaringType, dstType).Write();
            return (IComponentDef def, Component comp, IReadOnlyDictionary<IComponentDef, Component> index) => { };
        }
    }
}
