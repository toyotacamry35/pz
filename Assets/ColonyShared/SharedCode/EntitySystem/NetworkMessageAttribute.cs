using System;

namespace SharedCode.EntitySystem
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    internal class NetworkMessageAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
