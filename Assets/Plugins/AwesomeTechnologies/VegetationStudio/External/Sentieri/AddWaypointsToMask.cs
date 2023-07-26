using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AwesomeTechnologies.External.Sentieri
{

    [ExecuteInEditMode]
    public class AddWaypointsToMask : MonoBehaviour
    {
        public AwesomeTechnologies.VegetationMaskLine vegetationMaskLine;
        public bool Refresh = false;
        void Start()
        {

        }


        void Update()
        {
            if (Refresh)
            {
                UpdateVegetationMask();
                Refresh = false;
            }
        }

        void UpdateVegetationMask()
        {
            int _numChildren = this.transform.childCount;
            vegetationMaskLine.ClearNodes();
            for (int i = 0; i < _numChildren; ++i)
            {
                Transform _pointTransform = this.transform.GetChild(i);
                vegetationMaskLine.AddNodeToEnd(_pointTransform.position);
            }
            // vegetationMaskLine.AddNodeToEnd(this.transform.GetChild(0).position);
        }
    }
}
