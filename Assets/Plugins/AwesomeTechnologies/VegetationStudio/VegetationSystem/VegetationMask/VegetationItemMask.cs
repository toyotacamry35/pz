﻿using AwesomeTechnologies.VegetationStudio;
using UnityEngine;

namespace AwesomeTechnologies.Vegetation.Masks
{
    public class VegetationItemMask : MonoBehaviour
    {
        public Vector3 Position;
        public VegetationType VegetationType;
        public string VegetationMaskID;
        private bool _isDirty;
        private CircleMaskArea _currentMaskArea;

        // ReSharper disable once UnusedMember.Local
        void OnEnable()
        {
            if (VegetationMaskID == "") VegetationMaskID = System.Guid.NewGuid().ToString();
            _isDirty = true;
        }

        public void SetVegetationItemInstanceInfo(VegetationItemInstanceInfo vegetationItemInstanceInfo)
        {
            Position = vegetationItemInstanceInfo.Position;
            VegetationType = vegetationItemInstanceInfo.VegetationType;
            _isDirty = true;
        }

        public void SetVegetationItemInstanceInfo(Vector3 position, VegetationType vegetationType)
        {
            Position = position;
            VegetationType = vegetationType;
            _isDirty = true;
        }

        private void UpdateVegetationItemMask()
        {
            CircleMaskArea maskArea = new CircleMaskArea
            {
                Radius = 0.2f,
                Position = Position
            };
            maskArea.Init();
            maskArea.VegetationType = VegetationType;
            SetRemoveVegetationTypes(maskArea);

            if (_currentMaskArea != null)
            {
                VegetationStudioManager.RemoveVegetationMask(_currentMaskArea);
                _currentMaskArea = null;
            }

            _currentMaskArea = maskArea;
            VegetationStudioManager.AddVegetationMask(maskArea);
        }

        void SetRemoveVegetationTypes(CircleMaskArea circleMaskArea)
        {
            circleMaskArea.RemoveGrass = (VegetationType == VegetationType.Grass);
            circleMaskArea.RemovePlants = (VegetationType == VegetationType.Plant);
            circleMaskArea.RemoveTrees = (VegetationType == VegetationType.Tree);
            circleMaskArea.RemoveObjects = (VegetationType == VegetationType.Objects);
            circleMaskArea.RemoveLargeObjects = (VegetationType == VegetationType.LargeObjects);
        }

        public void SetDirty()
        {
            _isDirty = true;
        }

        // ReSharper disable once ArrangeTypeMemberModifiers
        void LateUpdate()
        {
            if (_isDirty)
            {
                _isDirty = false;
                UpdateVegetationItemMask();
            }
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
