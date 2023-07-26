namespace Assets.Src.SpatialSystem
{
    public class ClimateParams
    {
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public float Wind { get; set; }
        public int Toxic { get; set; }
        public int Oxygen { get; set; }

        public override bool Equals(object obj)
        {
            var climateParams = obj as ClimateParams; 
            if (climateParams != null)
            {
                return 
                    Temperature == climateParams.Temperature &&
                    Humidity == climateParams.Humidity &&
                    Wind == climateParams.Wind &&
                    Toxic == climateParams.Toxic &&
                    Oxygen == climateParams.Oxygen;
            }

            return false;
        }
    }
}