// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class SessionHolderEntity
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                _SessionsByGuid.Clear();
                var SessionsByGuiditem0__rnd = System.Guid.NewGuid();
                System.Guid __key0__ = System.Guid.Parse("73bbdc34-1087-ceec-d84a-21b31f2e9212");
                SessionsByGuid[__key0__] = SessionsByGuiditem0__rnd;
                var SessionsByGuiditem1__rnd = System.Guid.NewGuid();
                System.Guid __key1__ = System.Guid.Parse("c9d3cf56-7b74-2246-6226-c49ded204e48");
                SessionsByGuid[__key1__] = SessionsByGuiditem1__rnd;
                var SessionsByGuiditem2__rnd = System.Guid.NewGuid();
                System.Guid __key2__ = System.Guid.Parse("e0e2443b-e7de-7f9d-c41e-5f750b93af06");
                SessionsByGuid[__key2__] = SessionsByGuiditem2__rnd;
                var SessionsByGuiditem3__rnd = System.Guid.NewGuid();
                System.Guid __key3__ = System.Guid.Parse("8c096fe6-4fba-b923-9598-10604c4ee0b7");
                SessionsByGuid[__key3__] = SessionsByGuiditem3__rnd;
                var SessionsByGuiditem4__rnd = System.Guid.NewGuid();
                System.Guid __key4__ = System.Guid.Parse("db77d8fc-b24c-6e34-3153-05561bd016cf");
                SessionsByGuid[__key4__] = SessionsByGuiditem4__rnd;
                var SessionsByGuiditem5__rnd = System.Guid.NewGuid();
                System.Guid __key5__ = System.Guid.Parse("7eb7bd77-c117-ec8d-2028-4a45d63c3375");
                SessionsByGuid[__key5__] = SessionsByGuiditem5__rnd;
                var SessionsByGuiditem6__rnd = System.Guid.NewGuid();
                System.Guid __key6__ = System.Guid.Parse("3279a83e-d31b-34ba-3a9a-5a6649cd9dc4");
                SessionsByGuid[__key6__] = SessionsByGuiditem6__rnd;
            }
        }
    }
}