// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldNodeServiceEntity
    {
        protected override void RandomFill(int __count__, System.Random __random__, bool __withReadOnly__)
        {
            __count__--;
            if (__count__ <= 0)
                return;
            base.RandomFill(__count__, __random__, __withReadOnly__);
            var random = new System.Random(System.Guid.NewGuid().ToString().GetHashCode());
            {
                var ExternalAddress__rnd = GeneratedCode.EntitySystem.RandomFillHelper.CrateInstance<SharedCode.Entities.Cloud.EndpointAddress>();
                ExternalAddress = ExternalAddress__rnd;
            }

            {
                var Map__rnd = ((ResourcesSystem.Loader.IGameResourcesRandomExtension)ResourcesSystem.Loader.GameResourcesHolder.Instance).LoadRandomResourceByType<GeneratedCode.Custom.Config.MapDef>(__random__);
                Map = Map__rnd;
            }

            {
                var MapInstanceId__rnd = System.Guid.NewGuid();
                MapInstanceId = MapInstanceId__rnd;
            }

            {
                var MapChunkId__rnd = System.Guid.NewGuid();
                MapChunkId = MapChunkId__rnd;
            }

            {
                var allValuesWorldNodeServiceState = System.Enum.GetValues(typeof(SharedCode.Entities.Service.WorldNodeServiceState));
                var valueWorldNodeServiceState = (SharedCode.Entities.Service.WorldNodeServiceState)allValuesWorldNodeServiceState.GetValue(__random__.Next() % allValuesWorldNodeServiceState.Length);
                var State__rnd = valueWorldNodeServiceState;
                State = State__rnd;
            }

            {
                var ClientNode__rnd = __random__.Next() % 2 == 1;
                ClientNode = ClientNode__rnd;
            }
        }
    }
}