using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.RubiconAI.Editor
{
    public class TestResource : BaseResource
    {
        public int OtherData { get; set; } = 4;
        public ResourceRef<TestResource> OtherResource { get; set; }
        public List<ResourceRef<TestResource>> OtherResources { get; } = new List<ResourceRef<TestResource>>();
        public ResourceRef<SomeClass> InternalData { get; set; }
        public ResourceRef<SomeClass> InternalData2 { get; set; }
        public List<ResourceRef<SomeClass>> Classes { get; } = new List<ResourceRef<SomeClass>>();
    }

    public struct MyStruct
    {
        public string Value { get; set; }
        public string Value2 { get; set; }
    }

    public class OtherTestResource : BaseResource
    {
        public ResourceRef<TestResource> OtherResource { get; set; }
        public List<ResourceRef<TestResource>> OtherResources { get; set; }
        public string Data { get; set; }
        public int OtherData { get; set; }
        public MyStruct StructData { get; set; }
    }

    public class ResChildA : TestResource
    {
        public string ChildAData { get; set; }
    }
    public class ResChildB : TestResource
    {
        public string ChildBData { get; set; }

    }
    public class ResChildC : TestResource
    {
        public string ChildCData { get; set; }
    }

    public class SomeClass : BaseResource
    {
        public string Data { get; set; }
        public int OtherData { get; set; }
    }

    public class SomeChildA : SomeClass
    {

    }
    public class SomeChildB : SomeClass
    {

    }
}
