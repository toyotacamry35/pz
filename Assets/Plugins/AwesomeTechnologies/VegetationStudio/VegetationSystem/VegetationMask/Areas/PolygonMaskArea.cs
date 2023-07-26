using System;
using System.Collections.Generic;
using UnityEngine;

namespace AwesomeTechnologies
{
    public class PolygonMaskArea : BaseMaskArea
    {
        MathLib.Polygon2 _polygon;
        MathLib.Orientations _polygonOrientation;
        private bool _isConvex;
        private UnityEngine.Vector2[] _points2D;
        private Vector3[] _points3D;

        public void AddPolygon(List<Vector3> pointList)
        {
            _points2D = new UnityEngine.Vector2[pointList.Count];
            _points3D = new Vector3[pointList.Count];
            for (int i = 0; i <= pointList.Count - 1; i++)
            {
                _points2D[i] = new UnityEngine.Vector2(pointList[i].x, pointList[i].z);
                _points3D[i] = pointList[i];
            }

            _polygon = new MathLib.Polygon2(_points2D);

            _isConvex = _polygon.IsConvex(out _polygonOrientation,0.1f);
            MaskBounds = GetMaskBounds();
        }

        public override bool Contains(Vector3 point, VegetationType vegetationType, bool useAdditionalDistance, bool useExcludeFilter)
        {
            float additionalWidth = 0f;

            if (useExcludeFilter)
            {
                switch (vegetationType)
                {
                    case VegetationType.Grass:
                        if (!RemoveGrass) return false;
                        additionalWidth = Mathf.Lerp(AdditionalGrassWidth, AdditionalGrassWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));
                        break;
                    case VegetationType.Objects:
                        if (!RemoveObjects) return false;
                        additionalWidth = Mathf.Lerp(AdditionalObjectWidth, AdditionalObjectWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));
                        break;
                    case VegetationType.Plant:
                        if (!RemovePlants) return false;
                        additionalWidth = Mathf.Lerp(AdditionalPlantWidth, AdditionalPlantWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));

                        break;
                    case VegetationType.LargeObjects:
                        if (!RemoveLargeObjects) return false;
                        additionalWidth = Mathf.Lerp(AdditionalLargeObjectWidth, AdditionalLargeObjectWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));

                        break;
                    case VegetationType.Tree:
                        if (!RemoveTrees) return false;
                        additionalWidth = Mathf.Lerp(AdditionalTreeWidth, AdditionalTreeWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));

                        break;
                }
            }
            else
            {
                switch (vegetationType)
                {
                    case VegetationType.Grass:
                        additionalWidth = Mathf.Lerp(AdditionalGrassWidth, AdditionalGrassWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));
                        break;
                    case VegetationType.Objects:
                        additionalWidth = Mathf.Lerp(AdditionalObjectWidth, AdditionalObjectWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));
                        break;
                    case VegetationType.Plant:
                        additionalWidth = Mathf.Lerp(AdditionalPlantWidth, AdditionalPlantWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));
                        break;
                    case VegetationType.LargeObjects:
                        additionalWidth = Mathf.Lerp(AdditionalLargeObjectWidth, AdditionalLargeObjectWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));
                        break;
                    case VegetationType.Tree:
                        additionalWidth = Mathf.Lerp(AdditionalTreeWidth, AdditionalTreeWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));
                        break;
                }
            }

            UnityEngine.Vector2 point2D = new UnityEngine.Vector2(point.x, point.z);
            if (!useAdditionalDistance) additionalWidth = 0;

            if (_isConvex)
            {
                if (_polygonOrientation == MathLib.Orientations.CW)
                {
                    if (_polygon.ContainsConvexCW(point2D))
                    {
                        return true;
                    }
                    else
                    {
                        return IsWithinDistance(point2D,additionalWidth);
                    }

                }
                else
                {
                    if (_polygon.ContainsConvexCCW(point2D))
                    {
                        return true;
                    }
                    else
                    {
                        return IsWithinDistance(point2D, additionalWidth); 
                    }
                }                
            }
            else
            {
                if (_polygon.ContainsSimple(point2D))
                {
                    return true;
                }
                else
                {
                    return IsWithinDistance(point2D, additionalWidth); 
                }
            }
        }


        private bool IsWithinDistance(UnityEngine.Vector2 point2D, float distance)
        {
            if (Math.Abs(distance) < 0.01f)
            {
                return false;
            }
            else
            {
                float tempDistance = GetDistance(point2D);
                return tempDistance <= distance;
            }

        }

        private float GetDistance(UnityEngine.Vector2 point2D)
        {
            float distance = float.MaxValue;
            MathLib.Edge2[] edges = _polygon.Edges;

            for (int i = 0; i <= edges.Length - 1; i++)
            {
                MathLib.Segment2 tempSegment2 = new MathLib.Segment2(edges[i].Point0, edges[i].Point1);
                float tempDistance = tempSegment2.DistanceTo(point2D);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                }
            }

            return distance;
        }

        private Bounds GetMaskBounds()
        {
            var expandedBounds = _points3D.Length > 0 ? new Bounds(_points3D[0], new Vector3(1, 1, 1)) : new Bounds(new Vector3(0, 0, 0), new Vector3(1, 1, 1));

            for (int i = 0; i <= _points3D.Length - 1; i++)
            {
                expandedBounds.Encapsulate(_points3D[i]);
            }

            expandedBounds.Expand(GetMaxAdditionalDistance());
            return expandedBounds;
        }
    }
}
