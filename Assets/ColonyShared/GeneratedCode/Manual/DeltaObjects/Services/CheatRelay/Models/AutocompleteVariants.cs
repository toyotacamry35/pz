using ProtoBuf;

namespace GeneratedCode.DeltaObjects
{
    [ProtoContract]
    public struct AutocompleteVariants
    {
        public AutocompleteVariants(string[] variants)
        {
            Variants = variants;
        }

        [ProtoMember(1)]
        public string[] Variants { get; set;}
    }
}