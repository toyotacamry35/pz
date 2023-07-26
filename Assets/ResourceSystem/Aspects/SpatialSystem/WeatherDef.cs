using System;
using System.Collections.Generic;
using Assets.Src.ResourcesSystem;
using Assets.Src.ResourcesSystem.Base;
using Assets.Src.RubiconAI;
using Assets.Src.Tools;

namespace Assets.Src.SpatialSystem
{
    public class WeatherDef : BaseResource
    {
        public class WeatherSchedule
        {
            public int TemperatureMod { get; set; }
            public int HumidityMod { get; set; }
            public int WindMod { get; set; }
            public int ToxicMod { get; set; }
            public int OxygenMod { get; set; }
        }
        public MinMax Duration { get; set; }
        public string Name { get; set; }
        public Dictionary<Int32, WeatherSchedule> Schedule { get; set; } = new Dictionary<Int32, WeatherSchedule>();
    }
}