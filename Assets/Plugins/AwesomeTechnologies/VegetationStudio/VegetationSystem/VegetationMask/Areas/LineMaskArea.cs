using UnityEngine;

namespace AwesomeTechnologies
{
    public class LineMaskArea : BaseMaskArea
    {
        //Dest.Math.Segment3 _line;
        MathLib.Segment2 _line2D;

        private Vector3 _point1;
        private Vector3 _point2;
        private Vector3 _centerPoint;
        //private float _lineLength;
        //private float _lineLength2D;
        private float _width;

        public void SetLineData(Vector3 point1, Vector3 point2, float width)
        {
            _centerPoint = Vector3.Lerp(point1, point2, 0.5f);
            _point1 = point1;
            _point2 = point2;
            _width = width;
            //_line = new Dest.Math.Segment3(new Vector3(point1.x, 0, point1.z), new Vector3(point2.x, 0, point2.z));
            _line2D = new MathLib.Segment2(new Vector3(point1.x, point1.z), new Vector3(point2.x, point2.z));
            //this._lineLength = Vector3.Distance(_line.P0, _line.P1);
            //this._lineLength2D = Vector2.Distance(_line2D.P0, _line2D.P1);

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
                        additionalWidth = Mathf.Lerp(AdditionalGrassWidth, AdditionalGrassWidthMax, SamplePerlinNoise(point,NoiseScaleGrass));
                        break;
                    case VegetationType.Objects:
                        if (!RemoveObjects) return false;
                        //_additionalWidth = additionalObjectWidth;
                        additionalWidth = Mathf.Lerp(AdditionalObjectWidth, AdditionalObjectWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));
                        break;
                    case VegetationType.Plant:
                        if (!RemovePlants) return false;
                        //_additionalWidth = additionalPlantWidth;
                        additionalWidth = Mathf.Lerp(AdditionalPlantWidth, AdditionalPlantWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));

                        break;
                    case VegetationType.LargeObjects:
                        if (!RemoveLargeObjects) return false;
                        //_additionalWidth = additionalLargeObjectWidth;
                        additionalWidth = Mathf.Lerp(AdditionalLargeObjectWidth, AdditionalLargeObjectWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));

                        break;
                    case VegetationType.Tree:
                        if (!RemoveTrees) return false;
                        //_additionalWidth = additionalTreeWidth;
                        additionalWidth = Mathf.Lerp(AdditionalTreeWidth, AdditionalTreeWidthMax, SamplePerlinNoise(point, NoiseScaleGrass));

                        break;
                }
            }
            else
            {
                switch (vegetationType)
                {
                    case VegetationType.Grass:
                        additionalWidth = Mathf.Lerp(AdditionalGrassWidth, AdditionalGrassWidthMax, SamplePerlinNoise(point,NoiseScaleGrass));
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

            float distance = _line2D.DistanceTo(point2D);
            if (distance < _width/2f + additionalWidth/2f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Bounds GetMaskBounds()
        {
            Bounds expandedBounds = new Bounds(_centerPoint, new Vector3(1, 1, 1));
            expandedBounds.Encapsulate(_point1);
            expandedBounds.Encapsulate(_point2);
            expandedBounds.Expand(_width);

            expandedBounds.Expand(GetMaxAdditionalDistance());
            return expandedBounds;
        }
    }
}
