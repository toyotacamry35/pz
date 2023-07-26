using Assets.Src.BuildingSystem;
using SharedCode.EntitySystem;
using Core.Reflection;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Assets.Src.BuildingSystem
{
    public class BindAttribute : Attribute
    {
        public string Name { get; set; }
        public BindAttribute()
        {
            Name = string.Empty;
        }

        public BindAttribute(string name)
        {
            Name = name;
        }

    }

    public static class PropertyDataHelper
    {
        public class CopyInfo
        {
            public string PropertyName { get; private set; }
            public Func<object, object> Get { get; private set; }
            public Action<object, object> Set { get; private set; }
            public Action<object, object> Copy { get; private set; }

            public CopyInfo(string propertyName, Func<object, object> get, Action<object, object> set, Action<object, object> copy)
            {
                PropertyName = propertyName;
                Get = get;
                Set = set;
                Copy = copy;
            }
        }

        private static bool debugMode = false;
        private static readonly Dictionary<KeyValuePair<Type, Type>, MethodInfo> explicitOperators = new Dictionary<KeyValuePair<Type, Type>, MethodInfo>();
        private static readonly Dictionary<KeyValuePair<Type,Type>, Dictionary<string, CopyInfo>> copyActions = new Dictionary<KeyValuePair<Type, Type>, Dictionary<string, CopyInfo>>();

        // get delegate -----------------------------------------------------------------------
        private static Func<object, object> Private_GetDelegateHelper<TDestination, TProperty>(PropertyInfo destinationPropertyInfo)
            where TDestination : class
        {
            var methodInfoGet = destinationPropertyInfo.GetGetMethod(true);

            var getDelegate = (Func<TDestination,TProperty>)Delegate.CreateDelegate(typeof(Func<TDestination, TProperty>), null, methodInfoGet);

            return (object destination) => { return getDelegate((TDestination)destination); };
        }

        private static Func<object, object> GetGetDelegate(Type destination, PropertyInfo destinationPropertyInfo)
        {
            var genericHelper = typeof(PropertyDataHelper).GetMethod(nameof(Private_GetDelegateHelper), BindingFlags.Static | BindingFlags.NonPublic);
            var constructedHelper = genericHelper.MakeGenericMethod(destination, destinationPropertyInfo.PropertyType);
            return (Func<object, object>)constructedHelper.Invoke(null, new object[] { destinationPropertyInfo });
        }

        // set delegate -----------------------------------------------------------------------
        private static Action<object, object> Private_SetDelegateHelper<TDestination, TProperty>(PropertyInfo destinationPropertyInfo)
            where TDestination : class
        {
            var methodInfoSet = destinationPropertyInfo.GetSetMethod(true);

            var setDelegate = (Action<TDestination,TProperty>)Delegate.CreateDelegate(typeof(Action<TDestination, TProperty>), null, methodInfoSet);

            return (object destination, object value) => setDelegate((TDestination)destination, (TProperty)value);
        }

        private static Action<object, object> GetSetDelegate(Type destination, PropertyInfo destinationPropertyInfo)
        {
            var genericHelper = typeof(PropertyDataHelper).GetMethod(nameof(Private_SetDelegateHelper), BindingFlags.Static | BindingFlags.NonPublic);
            var constructedHelper = genericHelper.MakeGenericMethod(destination, destinationPropertyInfo.PropertyType);
            return (Action<object, object>)constructedHelper.Invoke(null, new object[] { destinationPropertyInfo });
        }

        // copy delegate ----------------------------------------------------------------------
        private static Action<object, object> Private_CopyDelegateHelper<TSource, TDestination, TProperty>(PropertyInfo sourcePropertyInfo, PropertyInfo destinationPropertyInfo)
            where TSource : class
            where TDestination : class
        {
            var methodInfoGet = sourcePropertyInfo.GetGetMethod(true);
            var methodInfoSet = destinationPropertyInfo.GetSetMethod(true);

            var getDelegate = (Func<TSource,TProperty>)Delegate.CreateDelegate(typeof(Func<TSource, TProperty>), null, methodInfoGet);
            var setDelegate = (Action<TDestination,TProperty>)Delegate.CreateDelegate(typeof(Action<TDestination, TProperty>), null, methodInfoSet);

            return (object source, object destination) => setDelegate((TDestination)destination, getDelegate((TSource)source));
        }

        private static Action<object, object> GetCopyDelegate(Type source, string sourcePropertyName, Type destination, PropertyInfo destinationPropertyInfo)
        {
            var sourcePropertyInfo = source.GetProperty(sourcePropertyName);
            if (sourcePropertyInfo == null)
            {
                BuildUtils.Error?.Report($"source - type: {source}, property name: {sourcePropertyName}, Destination - type: {destination}, property name: {destinationPropertyInfo.Name}, property type: {destinationPropertyInfo.PropertyType} message: can't find source property", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return null;
            }
            if (sourcePropertyInfo.PropertyType != destinationPropertyInfo.PropertyType)
            {
                BuildUtils.Error?.Report($"source - type: {source}, property name: {sourcePropertyName}, property type: {sourcePropertyInfo.PropertyType}, Destination - type: {destination}, property name: {destinationPropertyInfo.Name}, property type: {destinationPropertyInfo.PropertyType} message: source property type don't match destination", MethodBase.GetCurrentMethod().DeclaringType.Name);
                return null;
            }
            var genericHelper = typeof(PropertyDataHelper).GetMethod(nameof(Private_CopyDelegateHelper), BindingFlags.Static | BindingFlags.NonPublic);
            var constructedHelper = genericHelper.MakeGenericMethod(source, destination, destinationPropertyInfo.PropertyType);
            return (Action<object, object>)constructedHelper.Invoke(null, new object[] { sourcePropertyInfo, destinationPropertyInfo });
        }

        // helpers ----------------------------------------------------------------------------
        public static void CreateCopyList(KeyValuePair<Type, Type> key)
        {
            if (!copyActions.ContainsKey(key))
            {
                //BuildUtils.Debug?.Report(true, $"source type: {key.Key}, destination type: {key.Value}, message: create copy list", MethodBase.GetCurrentMethod().DeclaringType.Name);
                var actions = new Dictionary<string, CopyInfo>();
                var destinationProperties = key.Value.GetProperties().Where(prop => prop.IsDefined(typeof(BindAttribute), false));
                foreach (var destinationProperty in destinationProperties)
                {
                    if (destinationProperty != null)
                    {
                        var attribute = destinationProperty.GetCustomAttribute<BindAttribute>();
                        var sourcePropertyName = string.IsNullOrEmpty(attribute.Name) ? destinationProperty.Name : attribute.Name;
                        var getDelegate = GetGetDelegate(key.Value, destinationProperty);
                        var setDelegate = GetSetDelegate(key.Value, destinationProperty);
                        var copyDelegate = GetCopyDelegate(key.Key, sourcePropertyName, key.Value, destinationProperty);
                        if ((getDelegate != null) && (setDelegate != null) && (copyDelegate != null))
                        {
                            actions.Add(sourcePropertyName, new CopyInfo(destinationProperty.Name, getDelegate, setDelegate, copyDelegate));
                        }
                    }
                }
                copyActions.Add(key, actions);
            }
            else
            {
                BuildUtils.Debug?.Report(true, $"source type: {key.Key}, destination type: {key.Value}, message: copy list already created", MethodBase.GetCurrentMethod().DeclaringType.Name);
            }
        }

        // interface --------------------------------------------------------------------------
        public static void GetCopyList(object source, object destination, ref Dictionary<string, CopyInfo> copyActionsCache)
        {
            if (copyActionsCache == null)
            {
                var key = new KeyValuePair<Type, Type>(source.GetType(), destination.GetType());
                if (!copyActions.ContainsKey(key))
                {
                    CreateCopyList(key);
                }
                copyActionsCache = copyActions[key];
            }
        }

        //TODO change for IL CODE generation
        public static MethodInfo GetExplicitOperator(Type fromType, Type toType)
        {
            var key = new KeyValuePair<Type, Type>(fromType, toType);
            MethodInfo methodInfo = null;
            if (!explicitOperators.TryGetValue(key, out methodInfo))
            {
                methodInfo = fromType.GetMethodsSafe(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(x => (x.Name == "op_Explicit") && (x.ReturnType == toType));
                explicitOperators.Add(key, methodInfo);
            }
            return methodInfo;
        }
    }

    public abstract class PropertyData
    {
        protected abstract Dictionary<string, PropertyDataHelper.CopyInfo> GetСopyActionsCache();
        protected abstract void SetСopyActionsCache(Dictionary<string, PropertyDataHelper.CopyInfo> cache);

        public class PropertyArgs : EventArgs
        {
            public string PropertyName { get; private set; }
            public object OldValue { get; private set; }
            public object NewValue { get; private set; }

            public PropertyArgs(string propertyName, object oldValue, object newValue)
            {
                PropertyName = propertyName;
                OldValue = oldValue;
                NewValue = newValue;
            }
        }

        public event EventHandler BindFinished;
        public event EventHandler UnbindFinished;
        public event EventHandler<PropertyArgs> BindPropertyChanged;

        protected void InvokeBindFinished()
        {
            BindFinished?.Invoke(this, EventArgs.Empty);
        }

        protected void InvokeUnbindFinished()
        {
            UnbindFinished?.Invoke(this, EventArgs.Empty);
        }

        public void BindProperties(PropertyReplica elementToBindReplica, IDeltaObject elementToBind)
        {
            foreach (var propertyName in elementToBindReplica.PropertyNames)
            {
                elementToBind.SubscribePropertyChanged(propertyName, AsyncPropertyChanged);
            }
        }

        public void UnbindProperties(PropertyReplica elementToBindReplica, IDeltaObject elementToBind)
        {
            foreach (var propertyName in elementToBindReplica.PropertyNames)
            {
                elementToBind.UnsubscribePropertyChanged(propertyName, AsyncPropertyChanged);
            }
        }

        protected void SetInitialState(PropertyReplica elementToBindReplica)
        {
            var copyActions = GetСopyActionsCache();
            if (copyActions == null)
            {
                PropertyDataHelper.GetCopyList(elementToBindReplica, this, ref copyActions);
                SetСopyActionsCache(copyActions);
            }
            foreach (var copyAction in copyActions)
            {
                var oldValue = copyAction.Value.Get(this);
                copyAction.Value.Copy(elementToBindReplica, this);
                var newValue = copyAction.Value.Get(this);
                if (!Equals(oldValue, newValue))
                {
                    BindPropertyChanged?.Invoke(this, new PropertyArgs(copyAction.Value.PropertyName, oldValue, newValue));
                }
            }
        }

        public void ChangeProperty(string propertyName, object newValue)
        {
            var copyActions = GetСopyActionsCache();
            if (copyActions != null)
            {
                PropertyDataHelper.CopyInfo copyInfo;
                if (copyActions.TryGetValue(propertyName, out copyInfo))
                {
                    var _oldValue = copyInfo.Get(this);
                    copyInfo.Set(this, newValue);
                    var _newValue = copyInfo.Get(this);
                    if (!Equals(_oldValue, _newValue))
                    {
                        BindPropertyChanged?.Invoke(this, new PropertyArgs(copyInfo.PropertyName, _oldValue, _newValue));
                    }
                }
                else
                {
                    BuildUtils.Error?.Report($"can't find property corresponded to attribute [Bind({propertyName})] in type {GetType().Name}", MethodBase.GetCurrentMethod().DeclaringType.Name);
                }
            }
            else
            {
                BuildUtils.Error?.Report($"can't find СopyActionsCache corresponded to type {GetType().Name}, while property corresponded to attribute [Bind({propertyName})] changing", MethodBase.GetCurrentMethod().DeclaringType.Name);
            }
        }

        protected async Task AsyncPropertyChanged(EntityEventArgs args)
        {
            var propertyName = args.PropertyName;
            var newValue = args.NewValue;
            await UnityQueueHelper.RunInUnityThread(() =>
            {
                ChangeProperty(propertyName, newValue);
            });
        }
    }
}

// for future reference ---------------------------------------------------------------------------
// types converter
//protected void Set(PropertyInfo property, object value)
//{
//    object oldValue = property.GetValue(this);
//    if (value == null)
//    {
//        if (!property.PropertyType.IsValueType)
//        {
//            property.SetValue(this, value);
//        }
//        else
//        {
//            BuildUtils.Error?.Report($"[Building System]\t {MethodBase.GetCurrentMethod().Name}, Can't set value type property {property.Name} to null");
//        }
//    }
//    else if (value.GetType() == property.PropertyType)
//    {
//        property.SetValue(this, value);
//    }
//    else
//    {
//        var methodInfo = GetImplicitOperator(value.GetType(), property.PropertyType);
//        if (methodInfo != null)
//        {
//            property.SetValue(this, methodInfo.Invoke(null, new[] { value }));
//        }
//        else
//        {
//            property.SetValue(this, Convert.ChangeType(value, property.PropertyType));
//        }
//    }
//    BindPropertyChanged?.Invoke(this, new PropertyArgs(property.Name, oldValue, value));
//}