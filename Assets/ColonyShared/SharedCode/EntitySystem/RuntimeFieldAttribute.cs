using System;

namespace SharedCode.EntitySystem
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RuntimeDataAttribute: Attribute
    {
        public bool ThreadSafe { get; set; }

        public bool SkipField { get; set; }
    }   
}    
