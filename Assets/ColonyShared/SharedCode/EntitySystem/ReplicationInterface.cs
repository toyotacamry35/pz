using System;

namespace SharedCode.EntitySystem
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ReplicationInterfaceAttribute : Attribute
    {
        public ReplicationInterfaceAttribute(ReplicationLevel replicationLevel, int typeId, Type masterType)
        {
            ReplicationLevel = replicationLevel;
            TypeId = typeId;
            MasterType = masterType;
        }
        public ReplicationLevel ReplicationLevel { get; }
        public new int TypeId { get; }
        public Type MasterType { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ReplicationClassImplementationAttribute : Attribute
    {
        public ReplicationClassImplementationAttribute(Type interfaceType, int typeId)
        {
            InterfaceType = interfaceType;
            TypeId = typeId;
        }
        public new int TypeId { get; }
        public Type InterfaceType { get; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RpcMethodReceiverFuncAttribute : Attribute
    {
        public RpcMethodReceiverFuncAttribute(byte index, string displayName)
        {
            Index = index;
            DisplayName = displayName;
        }
        public byte Index { get; }
        public string DisplayName { get; }
    }

    public class RpcMethodExecuteFuncAttribute : Attribute
    {
        public RpcMethodExecuteFuncAttribute(byte index, string displayName)
        {
            Index = index;
            DisplayName = displayName;
        }
        public byte Index { get; }
        public string DisplayName { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RpcClassHashAttribute : Attribute
    {
        public RpcClassHashAttribute(int hash)
        {
            Hash = hash;
        }
        public int Hash { get; }
    }
}
