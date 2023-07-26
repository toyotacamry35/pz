using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies.External.MapMagicInterface
{
    public class MapMagicInfiniteTerrain : MonoBehaviour
    {
        public AwesomeTechnologies.VegetationSystem vegetationSystem;

        void Awake()
        {
#if MAPMAGIC
            MapMagic.MapMagic.OnGenerateCompleted += OnGenerateCompleted;
#endif
        }

        void OnGenerateCompleted(Terrain terrain)
        {
            if (vegetationSystem)
            {
                if (terrain.gameObject.GetComponent<AwesomeTechnologies.VegetationSystem>() == null)
                {
                    GameObject newVegetationSystemObject = Instantiate(vegetationSystem.gameObject);
                    newVegetationSystemObject.transform.SetParent(terrain.transform);

                    AwesomeTechnologies.VegetationSystem tempVegetationSystem = newVegetationSystemObject.GetComponent<AwesomeTechnologies.VegetationSystem>();
                    tempVegetationSystem.AutoselectTerrain = false;
                    tempVegetationSystem.currentTerrain = terrain;
                }
            }
        }
    }
}
