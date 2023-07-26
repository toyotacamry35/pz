using UnityEngine;

namespace AwesomeTechnologies
{
    public partial class VegetationSystem
    {
        public WindZone SelectedWindZone;
        public Texture2D WindWavesTexture;
        public float WindWavesSize = 10;
        public float WindSpeedFactor = 1;
        public bool LimitWindRange = true;

        //CTI wind
        private int TerrainLODWindPID;
        private Vector3 WindDirection;
        private float WindStrength;
        private float WindTurbulence;
        public float WindMultiplier = 1.0f;

        private void UpdateWind()
        {
            if (!SelectedWindZone) return;
           
            var dir = SelectedWindZone.transform.forward;
            var dir4 = new Vector4(dir.x, Mathf.Abs(SelectedWindZone.windMain) * WindSpeedFactor * 10, dir.z, WindWavesSize);
            Shader.SetGlobalVector("_AW_DIR", dir4);

            InjectCTIWind();
        }

        public void SetupWind()
        {
            FindWindZone();

            //CTI wind
            TerrainLODWindPID = Shader.PropertyToID("_TerrainLODWind");
        
            if (WindWavesTexture)
            {
                Shader.SetGlobalTexture("_AW_WavesTex", WindWavesTexture);
            }
        }

        void UpdateMaterialPropertyBlockWind(MaterialPropertyBlock materialPropertyBlock)
        {
            if (SelectedWindZone)
            {

                materialPropertyBlock.SetVector("_Wind", new Vector4(SelectedWindZone.transform.forward.x, SelectedWindZone.transform.forward.y, SelectedWindZone.transform.forward.z, SelectedWindZone.windMain));
            }
            else
            {
                materialPropertyBlock.SetVector("_Wind", new Vector4(1, 0, 0, 0.2f));
            }
        }

        private void InjectCTIWind()
        {
            WindDirection = SelectedWindZone.transform.forward;  
            WindStrength = SelectedWindZone.windMain * WindMultiplier;
            WindStrength += SelectedWindZone.windPulseMagnitude * (1.0f + Mathf.Sin(Time.time * SelectedWindZone.windPulseFrequency) + 1.0f + Mathf.Sin(Time.time * SelectedWindZone.windPulseFrequency * 3.0f)) * 0.5f;
            WindTurbulence = SelectedWindZone.windTurbulence * SelectedWindZone.windMain * WindMultiplier;

            WindDirection.x *= WindStrength;
            WindDirection.y *= WindStrength;
            WindDirection.z *= WindStrength;

            Shader.SetGlobalVector(TerrainLODWindPID, new Vector4(WindDirection.x, WindDirection.y, WindDirection.z, WindTurbulence));
        }
        


        private void FindWindZone()
        {
            if (!SelectedWindZone)
            {
                SelectedWindZone = (WindZone)FindObjectOfType(typeof(WindZone));
            }
        }
    }
}