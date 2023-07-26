using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies
{
    public class BroadCircleMaskArea : CircleMaskArea
    {
        public bool MaskGrass = false;
        public bool MaskPlants = false;
        public bool MaskTrees = false;
        public bool MaskObjects = false;
        public bool MaskLargeObjects = false;

        public new void Init()
        {
            base.Init();
            RemoveGrass = MaskGrass;
            RemovePlants = MaskPlants;
            RemoveTrees = MaskTrees;
            RemoveObjects = MaskObjects;
            RemoveLargeObjects = MaskLargeObjects;
        }

        public override bool Contains(Vector3 point, VegetationType vegetationType, bool useAdditionalDistance, bool useExcludeFilter)
        {
            if (MaskGrass && vegetationType != VegetationType.Grass) return false;
            if (MaskPlants && vegetationType != VegetationType.Plant) return false;
            if (MaskTrees && vegetationType != VegetationType.Tree) return false;
            if (MaskObjects && vegetationType != VegetationType.Objects) return false;
            if (MaskLargeObjects && vegetationType != VegetationType.LargeObjects) return false;
            float distance = Vector2.Distance(new Vector2(point.x, point.z), new Vector2(Position.x, Position.z));
            return distance < Radius;
        }
    }
}
