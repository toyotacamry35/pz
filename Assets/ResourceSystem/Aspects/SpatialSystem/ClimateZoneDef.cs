using System;
using System.Collections.Generic;
using Assets.Src.ResourcesSystem;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI;
using SharedCode.Aspects.Regions;

namespace Assets.Src.SpatialSystem
{
    public class ClimateZoneDef : ARegionDataDef
    {
        public class ScheduleRecord
        {
            public int Temperature { get; set; }
            public int Humidity { get; set; }
            public float Wind { get; set; }
            public int Toxic { get; set; }
            public int Oxygen { get; set; }
        }
        public string AmbientName { get; set; }

        public Dictionary<Int32, ScheduleRecord> Schedule { get; set; } = new Dictionary<Int32, ScheduleRecord>();
    }
}