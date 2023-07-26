// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class WrapperCounter
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                ((ResourcesSystem.Base.IHasRandomFill)SubCounter)?.Fill(__count__, __random__, __withReadOnly__);
            }

            {
                var QuestDef__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.Src.Aspects.Impl.Factions.Template.QuestDef>(__random__);
                QuestDef = QuestDef__rnd;
            }

            if (__withReadOnly__)
            {
                var CounterDef__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<Assets.Src.Aspects.Impl.Factions.Template.QuestCounterDef>(__random__);
                CounterDef = CounterDef__rnd;
            }

            {
                var Count__rnd = (int)__random__.Next(100);
                Count = Count__rnd;
            }

            {
                var CountForClient__rnd = (int)__random__.Next(100);
                CountForClient = CountForClient__rnd;
            }

            {
                var Completed__rnd = __random__.Next() % 2 == 1;
                Completed = Completed__rnd;
            }

            {
                var PreventOnComplete__rnd = __random__.Next() % 2 == 1;
                PreventOnComplete = PreventOnComplete__rnd;
            }
        }
    }
}