using Assets.Src.Lib.Cheats;
using System;
using Telemetry;
using GeneratedCode.Custom.Config;
using ResourcesSystem.Loader;
using SharedCode.Utils;
using Core.Cheats;

namespace Assets.Src.Cluster.Cheats
{
    public static class TelemetryCheats
    {
        public class ConsoleTest
        {
            private const string IndexName = "test_console";

            public DateTime Time { get; set; } = DateTime.UtcNow;
            public MapDef Map { get; set; }
            public Vector3 Position { get; set; }

            public void Index() => ElasticAccessor.Index(this, IndexName);
        }

        [Cheat]
        public static void ElasticTestEvent()
        {
            var data = new ConsoleTest
            {
                Map = GameResourcesHolder.Instance.LoadResource<MapDef>("/Scenes/TestObjMap"),
                Position = new Vector3(1, 2, 3)
            };

            data.Index();
        }
    }
}
