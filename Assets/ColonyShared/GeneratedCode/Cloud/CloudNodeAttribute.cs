using System;

namespace SharedCode.Cloud
{
    public class CloudNodeAttribute : Attribute
    {
        public CloudNodeType CloudNodeType { get; private set; }

        public CloudNodeAttribute(CloudNodeType cloudNodeType)
        {
            CloudNodeType = cloudNodeType;
        }
    }
}
