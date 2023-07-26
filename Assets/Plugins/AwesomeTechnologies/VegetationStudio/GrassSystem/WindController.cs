using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies.Grass
{
    [ExecuteInEditMode]
    public class WindController : MonoBehaviour
    {
        public Texture WindWavesTexture;
        public float WindWavesSize = 10;
        public float WindSpeedFactor = 1;

        public WindZone WindZone;

#if UNITY_EDITOR
        void Reset()
        {
            FindWindZone();
            SetupWind();
        }

        void SetupWind()
        {
            if (WindWavesTexture == null)
            {
                WindWavesTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/AwesomeTechnologies/VegetationStudio/Common/_Resources/PerlinSeamless.png");
            }
        }
#endif

        void OnEnable()
        {
            FindWindZone();
        }

        private void OnRenderObject()
        {
            UpdateWind();
        }

        void UpdateWind()
        {
            if (WindZone)
            {
                var dir = WindZone.transform.forward;
                var dir4 = new Vector4(dir.x, Mathf.Abs(WindZone.windMain) * WindSpeedFactor * 10, dir.z,
                    WindWavesSize);
                Shader.SetGlobalVector("_AW_DIR", dir4);
                if (WindWavesTexture)
                {
                    Shader.SetGlobalTexture("_AW_WavesTex", WindWavesTexture);
                }
            }
        }

        void Update()
        {
            UpdateWind();
        }

        private void FindWindZone()
        {
            if (!WindZone)
            {
                WindZone = (WindZone)FindObjectOfType(typeof(WindZone));
            }
        }
    }
}
