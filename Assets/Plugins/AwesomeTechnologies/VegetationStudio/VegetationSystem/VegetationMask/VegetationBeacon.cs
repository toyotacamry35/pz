using System.Collections.Generic;
using AwesomeTechnologies.Utility;
using AwesomeTechnologies.VegetationStudio;
using UnityEngine;

namespace AwesomeTechnologies.Vegetation
{

    [HelpURL("http://www.awesometech.no/index.php/vegetation-beacon")]
    [ExecuteInEditMode]
    public class VegetationBeacon : MonoBehaviour
    {
        public float Radius = 5f;
        public AnimationCurve FalloffCurve = new AnimationCurve();
        public List<VegetationTypeSettings> VegetationTypeList = new List<VegetationTypeSettings>();

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;
        private BeaconMaskArea _currentMaskArea;

        public void UpdateVegetationMask()
        {
            BeaconMaskArea maskArea = new BeaconMaskArea
            {
                Radius = Radius,
                Position = transform.position,
                FalloffCurveArray = FalloffCurve.GenerateCurveArray()
            };
            maskArea.Init();

            AddVegetationTypes(maskArea);

            if (_currentMaskArea != null)
            {
                VegetationStudioManager.RemoveVegetationMask(_currentMaskArea);
                _currentMaskArea = null;
            }

            _currentMaskArea = maskArea;
            VegetationStudioManager.AddVegetationMask(maskArea);
        }


        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            _lastPosition = transform.position;
            _lastRotation = transform.rotation;
        }

        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            UpdateVegetationMask();
        }

        // ReSharper disable once UnusedMember.Local
        void Update()
        {
            if (_lastPosition != transform.position || _lastRotation != transform.rotation)
            {
                UpdateVegetationMask();
                _lastPosition = transform.position;
                _lastRotation = transform.rotation;
            }
        }

        public void AddVegetationTypes(BaseMaskArea maskArea)
        {
            for (int i = 0; i <= VegetationTypeList.Count - 1; i++)
            {
                maskArea.VegetationTypeList.Add(new VegetationTypeSettings(VegetationTypeList[i]));
            }
        }
        // ReSharper disable once UnusedMember.Local
        void Reset()
        {
            FalloffCurve.AddKey(0, 1);
            FalloffCurve.AddKey(1, 0);
        }

        // ReSharper disable once UnusedMember.Local
        void OnDisable()
        {
            if (_currentMaskArea != null)
            {
                VegetationStudioManager.RemoveVegetationMask(_currentMaskArea);
                _currentMaskArea = null;
            }
        }
    }
}

