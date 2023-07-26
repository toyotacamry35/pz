using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using AwesomeTechnologies;
using AwesomeTechnologies.Vegetation.PersistentStorage;
using AwesomeTechnologies.VegetationStudio;


    public class VegetationScenePostProcess : IProcessSceneWithReport
    {
		public int callbackOrder => 1;
		
        public void OnProcessScene(Scene scene, BuildReport report)
        {
            foreach (GameObject obj in CollectGameObjectsWithComponent<VegetationStudioManager>(scene))
            {
                VegetationStudioManager vcm = obj.GetComponent<VegetationStudioManager>();
				{
					if (vcm!=null)
						Object.DestroyImmediate(vcm);
				}
                VegetationSystem[] vs = obj.GetComponentsInChildren<VegetationSystem>();
                int vsCount = vs.Length;
                for (int i=0; i<vsCount; i++)
                {
					if (vs[i]!=null)
						Object.DestroyImmediate(vs[i]);
				}
                PersistentVegetationStorage[] pvs = obj.GetComponentsInChildren<PersistentVegetationStorage>();
                vsCount = pvs.Length;
                for (int i=0; i<vsCount; i++)
                {
					if (pvs[i]!=null)
						Object.DestroyImmediate(pvs[i]);
				}
            }
            
        }

        private static IEnumerable<GameObject> CollectGameObjectsWithComponent<T>(Scene scene) where T : Component
        {
            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                foreach (T cmp in obj.GetComponentsInChildren<T>(true))
                {
                    yield return cmp.gameObject;
                }
            }
        }
    }
