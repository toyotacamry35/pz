using System;
using System.Collections.Generic;
using System.Text;
using ColonyShared.GeneratedCode;

namespace SharedCode.Utils
{
    public abstract class BindingHolder<TBindingIface, TDef>
    {
        public TBindingIface Instance;
    }
    
    public abstract class BindingCollectorBase<TBindingInterface, TBindingHolder, TDef> : ITypesCollectorAgent 
        where TBindingHolder : BindingHolder<TBindingInterface, TDef>
    {
        private readonly Type _bindingInterfaceGenericType; // TBindingInterface<>
        private readonly Type _bindingHolderGenericType; // TBindingHolder<>

        protected BindingCollectorBase(Type bindingInterfaceGenericType, Type bindingHolderGenericType)
        {
            _bindingInterfaceGenericType = bindingInterfaceGenericType ?? throw new ArgumentNullException(nameof(bindingInterfaceGenericType));
            _bindingHolderGenericType = bindingHolderGenericType ?? throw new ArgumentNullException(nameof(bindingHolderGenericType));

            if (!typeof(TBindingHolder).IsAssignableFrom(_bindingHolderGenericType))
                throw new Exception($"{_bindingHolderGenericType} must be inherited from {typeof(TBindingHolder)}");
            
            if (!typeof(TBindingInterface).IsAssignableFrom(_bindingInterfaceGenericType))
                throw new Exception($"{_bindingHolderGenericType} must be inherited from {typeof(TBindingInterface)}");
        }
        
        public Dictionary<Type, TBindingHolder> Collection { get; private set; }

        public void Init()
        {
            Collection = new Dictionary<Type, TBindingHolder>();
            AdditionalInit();
        }
        
        public void CollectType(Type type)
        {
            if (type.IsInterface || type.IsAbstract)
                return;
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (!interfaceType.IsGenericType)
                    continue;
                if (!typeof(TBindingInterface).IsAssignableFrom(interfaceType))
                    continue;
                var defType = interfaceType.GetGenericArguments()[0];
                if (!Collection.ContainsKey(defType))
                {
                    var holder = CreateBindingHolder(type, defType, _bindingHolderGenericType);
                    Collection.Add(defType, holder);
                    TypeAdded(defType, type);
                }
                else
                    throw new Exception($"[{GetType().NiceName()}] Def already registered | Def:{defType.NiceName()} OldBinding:{Collection[defType].GetType().NiceName()} NewBinding:{type.NiceName()}");
            }
        }

        public void Dump(StringBuilder sb)
        {
            if (Collection != null)
                foreach (var pair in Collection)
                {
                    DumpType(pair.Key, pair.Value, sb);
                    sb.Append("\n");
                }
            else
                sb.Append("null");
        }

        protected abstract void AdditionalInit();
        
        protected abstract TBindingHolder CreateBindingHolder(Type implType, Type defType, Type holderGenericType);

        protected abstract void TypeAdded(Type defType, Type implType);
        
        protected abstract void DumpType(Type defType, TBindingHolder holder, StringBuilder sb);
        
        protected static TBindingHolder CreateBindingHolderStatic(Type implType, Type defType, Type holderGenericType)
        {
            var holder = (TBindingHolder) Activator.CreateInstance(holderGenericType.MakeGenericType(defType));
            holder.Instance = (TBindingInterface) Activator.CreateInstance(implType);
            return holder;
        }
        
        protected static void DumpTypeStatic(Type defType, TBindingHolder holder, StringBuilder sb)
        {
            sb.Append(defType.NiceName()).Append(" => ").Append(holder.GetType().NiceName());
        }
    }
    
    
    
    public abstract class BindingCollector<TBindingInterface, TBindingHolder, TDef> : BindingCollectorBase<TBindingInterface, TBindingHolder, TDef> 
        where TBindingHolder : BindingHolder<TBindingInterface, TDef>
    {
        protected BindingCollector(Type bindingInterfaceGenericType, Type bindingHolderGenericType) : base(bindingInterfaceGenericType, bindingHolderGenericType)
        {}

        protected override TBindingHolder CreateBindingHolder(Type implType, Type defType, Type holderGenericType)
        {
            return CreateBindingHolderStatic(implType, defType, holderGenericType);
        }

        protected override void TypeAdded(Type defType, Type implType)
        {}

        protected override void DumpType(Type defType, TBindingHolder holder, StringBuilder sb)
        {
            DumpTypeStatic(defType, holder, sb);
        }
        
        protected override void AdditionalInit()
        {}
    }
}