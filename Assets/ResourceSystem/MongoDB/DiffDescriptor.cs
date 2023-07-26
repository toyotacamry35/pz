using System.Collections.Generic;

namespace SharedCode.Utils.BsonSerialization
{
    public class DiffDescriptor
    {
        public string Type;
        public string Seritalizer;
        public Dictionary<string, string> Added;
        public Dictionary<string, string> Removed;
        public Dictionary<string, DiffDescriptor> Changed;
    }
}