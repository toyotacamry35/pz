// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class KnowledgeEngine
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                _KnownTechnologies.Clear();
                var KnownTechnologiesitem0__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.TechnologyDef>(__random__);
                KnownTechnologies.Add(KnownTechnologiesitem0__rnd);
                var KnownTechnologiesitem1__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.TechnologyDef>(__random__);
                KnownTechnologies.Add(KnownTechnologiesitem1__rnd);
                var KnownTechnologiesitem2__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.TechnologyDef>(__random__);
                KnownTechnologies.Add(KnownTechnologiesitem2__rnd);
                var KnownTechnologiesitem3__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.TechnologyDef>(__random__);
                KnownTechnologies.Add(KnownTechnologiesitem3__rnd);
                var KnownTechnologiesitem4__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.TechnologyDef>(__random__);
                KnownTechnologies.Add(KnownTechnologiesitem4__rnd);
                var KnownTechnologiesitem5__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.TechnologyDef>(__random__);
                KnownTechnologies.Add(KnownTechnologiesitem5__rnd);
                var KnownTechnologiesitem6__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.TechnologyDef>(__random__);
                KnownTechnologies.Add(KnownTechnologiesitem6__rnd);
                var KnownTechnologiesitem7__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.TechnologyDef>(__random__);
                KnownTechnologies.Add(KnownTechnologiesitem7__rnd);
                var KnownTechnologiesitem8__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.TechnologyDef>(__random__);
                KnownTechnologies.Add(KnownTechnologiesitem8__rnd);
            }

            {
                _KnownKnowledges.Clear();
                var KnownKnowledgesitem0__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeDef>(__random__);
                KnownKnowledges.Add(KnownKnowledgesitem0__rnd);
                var KnownKnowledgesitem1__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeDef>(__random__);
                KnownKnowledges.Add(KnownKnowledgesitem1__rnd);
                var KnownKnowledgesitem2__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeDef>(__random__);
                KnownKnowledges.Add(KnownKnowledgesitem2__rnd);
                var KnownKnowledgesitem3__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeDef>(__random__);
                KnownKnowledges.Add(KnownKnowledgesitem3__rnd);
                var KnownKnowledgesitem4__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeDef>(__random__);
                KnownKnowledges.Add(KnownKnowledgesitem4__rnd);
                var KnownKnowledgesitem5__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeDef>(__random__);
                KnownKnowledges.Add(KnownKnowledgesitem5__rnd);
                var KnownKnowledgesitem6__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeDef>(__random__);
                KnownKnowledges.Add(KnownKnowledgesitem6__rnd);
                var KnownKnowledgesitem7__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeDef>(__random__);
                KnownKnowledges.Add(KnownKnowledgesitem7__rnd);
                var KnownKnowledgesitem8__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeDef>(__random__);
                KnownKnowledges.Add(KnownKnowledgesitem8__rnd);
                var KnownKnowledgesitem9__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeDef>(__random__);
                KnownKnowledges.Add(KnownKnowledgesitem9__rnd);
            }

            {
                _ShownKnowledgeRecords.Clear();
                var ShownKnowledgeRecordsitem0__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeRecordDef>(__random__);
                ShownKnowledgeRecords.Add(ShownKnowledgeRecordsitem0__rnd);
                var ShownKnowledgeRecordsitem1__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeRecordDef>(__random__);
                ShownKnowledgeRecords.Add(ShownKnowledgeRecordsitem1__rnd);
                var ShownKnowledgeRecordsitem2__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeRecordDef>(__random__);
                ShownKnowledgeRecords.Add(ShownKnowledgeRecordsitem2__rnd);
                var ShownKnowledgeRecordsitem3__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<SharedCode.Aspects.Science.KnowledgeRecordDef>(__random__);
                ShownKnowledgeRecords.Add(ShownKnowledgeRecordsitem3__rnd);
            }

            {
                ((ResourcesSystem.Base.IHasRandomFill)OwnerInformation)?.Fill(__count__, __random__, __withReadOnly__);
            }
        }
    }
}