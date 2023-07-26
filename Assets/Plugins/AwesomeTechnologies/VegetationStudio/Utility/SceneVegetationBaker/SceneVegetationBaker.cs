using UnityEngine;

namespace AwesomeTechnologies.Utility.Baking
{
    [HelpURL("http://www.awesometech.no/index.php/scene-vegetation-baker")]
    public class SceneVegetationBaker : MonoBehaviour
    {
        public VegetationSystem VegetationSystem;
        public string SelectedVegetationID;
        public bool ExportStatic = true;
      
        private void Reset()
        {
            VegetationSystem = gameObject.GetComponent<VegetationSystem>();
        }
    }
} 
