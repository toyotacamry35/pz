using SharedCode.DeltaObjects.Building;
using SharedCode.Utils;
using System.Reflection;

namespace Assets.Src.BuildingSystem
{

    public static class BuildingPlaceHelper
    {
        private static UnityEngine.Vector3 shiftForward = new UnityEngine.Vector3(1.0f, 0.0f, 0.0f);
        private static UnityEngine.Vector3 shiftCenter = new UnityEngine.Vector3(0.0f, 0.0f, 0.0f);
        private static UnityEngine.Vector3 shiftDown = new UnityEngine.Vector3(0.0f, -1.0f, 0.0f);
        private static UnityEngine.Vector3 shiftUp = new UnityEngine.Vector3(0.0f, 1.0f, 0.0f);

        private static UnityEngine.Quaternion rotationForward = UnityEngine.Quaternion.Euler(0.0f, 0.0f, 0.0f);
        private static UnityEngine.Quaternion rotationLeft = UnityEngine.Quaternion.Euler(0.0f, 270.0f, 0.0f);
        private static UnityEngine.Quaternion rotationBackward = UnityEngine.Quaternion.Euler(0.0f, 180.0f, 0.0f);
        private static UnityEngine.Quaternion rotationRight = UnityEngine.Quaternion.Euler(0.0f, 90.0f, 0.0f);

        private class HorizontalDirectionPicker
        {
            private readonly float start = 0.0f;
            private readonly float delimiter = 0.0f;
            private readonly UnityEngine.Vector3Int[] shift = null;
            private readonly BuildingElementSide[] rotation = null;

            public HorizontalDirectionPicker(int _count, UnityEngine.Vector3Int[] _shift, BuildingElementSide[] _rotation)
            {
                Count = _count;
                start = 180.0f / Count;
                delimiter = 360.0f / Count;
                shift = _shift;
                rotation = _rotation;
            }

            public int GetDirection(UnityEngine.Quaternion placeRotation, UnityEngine.Quaternion localRotation)
            {
                var direction = (localRotation.eulerAngles.y - placeRotation.eulerAngles.y + start);
                direction = direction / delimiter;
                return (((UnityEngine.Mathf.FloorToInt(direction) % Count) + Count) % Count);
            }
            public int Count { get; } = 0;
            public UnityEngine.Vector3Int Shift(int index) { return shift[index]; }
            public BuildingElementSide Rotation(int index) { return rotation[index]; }
        }

        private static HorizontalDirectionPicker[] horizontalDirectionPickers = new HorizontalDirectionPicker[]
        {
            new HorizontalDirectionPicker
            (
                4,
                new UnityEngine.Vector3Int[]
                {
                    new UnityEngine.Vector3Int(  0, 0,  1 ),     // 0       0
                    new UnityEngine.Vector3Int(  1, 0,  0 ),     // 1     3 c 1
                    new UnityEngine.Vector3Int(  0, 0, -1 ),     // 2       2
                    new UnityEngine.Vector3Int( -1, 0,  0 ),     // 3
                },
                new BuildingElementSide[]
                {
                    BuildingElementSide.Left,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Right,
                    BuildingElementSide.Backward,
                }
            ),
            new HorizontalDirectionPicker
            (
                8,
                new UnityEngine.Vector3Int[]
                {
                    new UnityEngine.Vector3Int(  0, 0,  1 ),     // 0     7 0 1
                    new UnityEngine.Vector3Int(  1, 0,  1 ),     // 1     6 c 2
                    new UnityEngine.Vector3Int(  1, 0,  0 ),     // 2     5 4 3
                    new UnityEngine.Vector3Int(  1, 0, -1 ),     // 3
                    new UnityEngine.Vector3Int(  0, 0, -1 ),     // 4
                    new UnityEngine.Vector3Int( -1, 0, -1 ),     // 5
                    new UnityEngine.Vector3Int( -1, 0,  0 ),     // 6
                    new UnityEngine.Vector3Int( -1, 0,  1 ),     // 7
                },
                new BuildingElementSide[]
                {
                    BuildingElementSide.Left,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Right,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Backward,
                }
            ),
            new HorizontalDirectionPicker
            (
                12,
                new UnityEngine.Vector3Int[]
                {
                    new UnityEngine.Vector3Int(  0, 0,  1 ),     // 00        11 00 01
                    new UnityEngine.Vector3Int(  1, 0,  1 ),     // 01      10   cl   02
                    new UnityEngine.Vector3Int(  1, 0,  1 ),     // 02      09cb cc cf03
                    new UnityEngine.Vector3Int(  1, 0,  0 ),     // 03      08   cr   04
                    new UnityEngine.Vector3Int(  1, 0, -1 ),     // 04        07 06 05
                    new UnityEngine.Vector3Int(  1, 0, -1 ),     // 05
                    new UnityEngine.Vector3Int(  0, 0, -1 ),     // 06
                    new UnityEngine.Vector3Int( -1, 0, -1 ),     // 07
                    new UnityEngine.Vector3Int( -1, 0, -1 ),     // 08
                    new UnityEngine.Vector3Int( -1, 0,  0 ),     // 09
                    new UnityEngine.Vector3Int( -1, 0,  1 ),     // 10
                    new UnityEngine.Vector3Int( -1, 0,  1 ),     // 11
                },
                new BuildingElementSide[]
                {
                    BuildingElementSide.Left,
                    BuildingElementSide.Left,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Right,
                    BuildingElementSide.Right,
                    BuildingElementSide.Right,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Left,
                }
            ),
            new HorizontalDirectionPicker
            (
                16,
                new UnityEngine.Vector3Int[]
                {
                    new UnityEngine.Vector3Int(  0, 0,  2 ),    // 00    14 15 00 01 02
                    new UnityEngine.Vector3Int(  1, 0,  2 ),    // 01    13    cl    03
                    new UnityEngine.Vector3Int(  2, 0,  2 ),    // 02    12 cb cc cf 04
                    new UnityEngine.Vector3Int(  2, 0,  1 ),    // 03    11    cr    05
                    new UnityEngine.Vector3Int(  2, 0,  0 ),    // 04    10 09 08 07 06
                    new UnityEngine.Vector3Int(  2, 0, -1 ),    // 05
                    new UnityEngine.Vector3Int(  2, 0, -2 ),    // 06
                    new UnityEngine.Vector3Int(  1, 0, -2 ),    // 07
                    new UnityEngine.Vector3Int(  0, 0, -2 ),    // 08
                    new UnityEngine.Vector3Int( -1, 0, -2 ),    // 09
                    new UnityEngine.Vector3Int( -2, 0, -2 ),    // 10
                    new UnityEngine.Vector3Int( -2, 0, -1 ),    // 11
                    new UnityEngine.Vector3Int( -2, 0,  0 ),    // 12
                    new UnityEngine.Vector3Int( -2, 0,  1 ),    // 13
                    new UnityEngine.Vector3Int( -2, 0,  2 ),    // 14
                    new UnityEngine.Vector3Int( -1, 0,  2 ),    // 15
                },
                new BuildingElementSide[]
                {
                    BuildingElementSide.Left,
                    BuildingElementSide.Left,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Right,
                    BuildingElementSide.Right,
                    BuildingElementSide.Right,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Left,
                }
            ),
            new HorizontalDirectionPicker
            (
                20,
                new UnityEngine.Vector3Int[]
                {
                    new UnityEngine.Vector3Int(  0, 0,  2 ),    // 00      18 19 00 01 02
                    new UnityEngine.Vector3Int(  1, 0,  2 ),    // 01    17              03
                    new UnityEngine.Vector3Int(  2, 0,  2 ),    // 02    16      cl      04
                    new UnityEngine.Vector3Int(  2, 0,  2 ),    // 03    15   cb cc cf   05
                    new UnityEngine.Vector3Int(  2, 0,  1 ),    // 04    14      cr      06
                    new UnityEngine.Vector3Int(  2, 0,  0 ),    // 05    13              07
                    new UnityEngine.Vector3Int(  2, 0, -1 ),    // 06      12 11 10 09 08
                    new UnityEngine.Vector3Int(  2, 0, -2 ),    // 07
                    new UnityEngine.Vector3Int(  2, 0, -2 ),    // 08
                    new UnityEngine.Vector3Int(  1, 0, -2 ),    // 09
                    new UnityEngine.Vector3Int(  0, 0, -2 ),    // 10
                    new UnityEngine.Vector3Int( -1, 0, -2 ),    // 11
                    new UnityEngine.Vector3Int( -2, 0, -2 ),    // 12
                    new UnityEngine.Vector3Int( -2, 0, -2 ),    // 13
                    new UnityEngine.Vector3Int( -2, 0, -1 ),    // 14
                    new UnityEngine.Vector3Int( -2, 0,  0 ),    // 15
                    new UnityEngine.Vector3Int( -2, 0,  1 ),    // 16
                    new UnityEngine.Vector3Int( -2, 0,  2 ),    // 17
                    new UnityEngine.Vector3Int( -2, 0,  2 ),    // 18
                    new UnityEngine.Vector3Int( -1, 0,  2 ),    // 19
                },
                new BuildingElementSide[]
                {
                    BuildingElementSide.Left,
                    BuildingElementSide.Left,
                    BuildingElementSide.Left,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Forward,
                    BuildingElementSide.Right,
                    BuildingElementSide.Right,
                    BuildingElementSide.Right,
                    BuildingElementSide.Right,
                    BuildingElementSide.Right,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Backward,
                    BuildingElementSide.Left,
                    BuildingElementSide.Left,
                }
            ),
        };

        private static int fourDirections = 0;
        private static int eightDirections = 1;
        private static int twelveDirections = 2;
        private static int sixteenDirections = 3;
        private static int twentyDirections = 4;

        private class VerticalDirectionPicker
        {
            public enum Direction
            {
                Up    = 0,
                Over  = 1,
                Under = 2,
                Down  = 3,
            }

            public static UnityEngine.Vector3Int Up = new UnityEngine.Vector3Int(0, 1, 0);

            public static bool Under(Direction direction)
            {
                return (direction > Direction.Over);
            }

            private readonly UnityEngine.Vector3Int[] shift = null;

            public VerticalDirectionPicker(UnityEngine.Vector3Int[] _shift)
            {
                shift = _shift;
            }

            public Direction GetDirection(UnityEngine.Quaternion localRotation, int shiftXZ)
            {
                var buildParamsDef = BuildUtils.BuildParamsDef;
                var angle = (shiftXZ == 0) ? buildParamsDef.BuildingShiftXZ0VerticalAngle : ((shiftXZ == 1) ? buildParamsDef.BuildingShiftXZ1VerticalAngle : buildParamsDef.BuildingShiftXZ2VerticalAngle);
                var directionIndex = Direction.Over;
                //TODO building: calculate direction factor for vertical angle (0.0f ... 1.0f)
                if (localRotation.eulerAngles.x < angle)
                {
                    directionIndex = Direction.Under;
                }
                else if (localRotation.eulerAngles.x < 180.0f)
                {
                    directionIndex = Direction.Down;
                }
                else if (localRotation.eulerAngles.x < (360.0f - angle))
                {
                    directionIndex = Direction.Up;
                }
                return directionIndex;
            }

            public UnityEngine.Vector3Int Shift(Direction verticalDirection) { return shift[(int)(verticalDirection)]; }
        }

        private static VerticalDirectionPicker verticalDirectionPicker = new VerticalDirectionPicker(new UnityEngine.Vector3Int[]
        {
            new UnityEngine.Vector3Int( 0, 1, 0 ),
            new UnityEngine.Vector3Int( 0, 0, 0 ),
            new UnityEngine.Vector3Int( 0, 0, 0 ),
            new UnityEngine.Vector3Int( 0, -1, 0 ),
        });

        // Helpers --------------------------------------------------------------------------------
        //---------------- входные параметры
        // placePosition - позиция центральной точки
        // placeRotation - поворот центральной точки (реально используется только поворот по оси Y)
        // structure     - информация о поставленных элементах, будет использоваться для подклейки элементов
        // localPosition - позиция игрока
        // localRotation - поворот камеры (используетcя поворот по оcи Y и наклон (поворот по X))
        // start         - положение центра блока с координатами (0, 0, 0) относительно центральной точки placePosition
        // blockSize     - размер блока
        // thickness     - толщина стенки (непонятно для чего, но пусть пока будет)
        // side          - какой элемент вычислять (грубо говоря: сенка, пол/потолок, центральная часть)
        // rotation      - поворот элемента (для пола/потолка и центральных элементов) (возможно в дальнейшем сделаем как в Fortnite дополнительным поворотом)
        //---------------- возвращаемые значения
        // block         - индекс блока вида (X, X, X), пока может быть отрицательным
        // face          - сторона блока
        public static bool Calculate(this BuildingPlaceholderData data, BuildingPlaceData place)
        {
            data.CacheInvalidOverride = false;
            if (place == null)
            {
                return false;
            }
            var placeDef = (place.PlaceDef != null) ? place.PlaceDef : BuildUtils.DefaultBuildingPlaceDef;
            if (placeDef == null)
            {
                return false;
            }

            var placePosition = UnityEngine.Vector3.zero;
            var placeRotation = UnityEngine.Quaternion.identity;
            if (place.IsEmpty)
            {
                if (!BuildSystem.Builder.IsSimpleMode)
                {
                    return false;
                }
                placeRotation = UnityEngine.Quaternion.Euler(0.0f, data.InterfaceRotation.eulerAngles.y, 0.0f);
                var blockShift = ((placeDef.Size - 1) & 1) * (placeDef.BlockSize / 2.0f);
                var toPlaceShift = new UnityEngine.Vector3(blockShift * (-1.0f), 0.0f, blockShift);
                toPlaceShift = placeRotation * toPlaceShift;
                placePosition = data.InterfacePosition + toPlaceShift;
                data.CacheInvalidOverride = true;
                data.PlacePosition = placePosition;
                data.PlaceRotation = placeRotation;
            }
            else
            {
                placePosition = (UnityEngine.Vector3)place.Position;
                placeRotation = (UnityEngine.Quaternion)place.Rotation;
            }

            var placeRadius = (placeDef.Size - 1) * placeDef.BlockSize / 2.0f;
            var placeHeight = (placeDef.Height - 1) * placeDef.BlockSize;
            var placeStart = new UnityEngine.Vector3(-placeRadius, 0.0f, -placeRadius);
            var characterCenterPoint = (UnityEngine.Vector3)(BuildUtils.BuildParamsDef.CharacterCenterPoint);

            var currentBlock = UnityEngine.Quaternion.Inverse(placeRotation) * (data.InterfacePosition + characterCenterPoint - placePosition);
            currentBlock = (currentBlock - placeStart) / placeDef.BlockSize + UnityEngine.Vector3.one / 2.0f;

            var currentBlockIndex = new UnityEngine.Vector3Int(UnityEngine.Mathf.FloorToInt(currentBlock.x), UnityEngine.Mathf.FloorToInt(currentBlock.y), UnityEngine.Mathf.FloorToInt(currentBlock.z));

            var verticalDirection = verticalDirectionPicker.GetDirection(data.InterfaceRotation, data.ShiftXZ);
            var verticalShift = verticalDirectionPicker.Shift(verticalDirection);

            var horizontalShift = UnityEngine.Vector3Int.zero;
            var horizontalRotation = data.PlaceholderSide;

            if (data.PlaceholderType == BuildingElementType.Wall)
            {
                var flip = (horizontalRotation == BuildingElementSide.Left) || (horizontalRotation == BuildingElementSide.Right);
                if (data.ShiftXZ == 1)
                {
                    var horizontalDirectionIndex = horizontalDirectionPickers[twelveDirections].GetDirection(placeRotation, data.InterfaceRotation);
                    horizontalShift = horizontalDirectionPickers[twelveDirections].Shift(horizontalDirectionIndex);
                    horizontalRotation = horizontalDirectionPickers[twelveDirections].Rotation(horizontalDirectionIndex);
                }
                else if (data.ShiftXZ == 2)
                {
                    var horizontalDirectionIndex = horizontalDirectionPickers[twentyDirections].GetDirection(placeRotation, data.InterfaceRotation);
                    horizontalShift = horizontalDirectionPickers[twentyDirections].Shift(horizontalDirectionIndex);
                    horizontalRotation = horizontalDirectionPickers[twentyDirections].Rotation(horizontalDirectionIndex);
                }
                else
                {
                    int horizontalDirectionIndex = horizontalDirectionPickers[fourDirections].GetDirection(placeRotation, data.InterfaceRotation);
                    horizontalRotation = horizontalDirectionPickers[fourDirections].Rotation(horizontalDirectionIndex);
                }
                data.Block = currentBlockIndex + verticalShift + horizontalShift + (VerticalDirectionPicker.Up * data.ShiftY);
                data.Type = data.PlaceholderType;
                data.Face = BuildingStructure.BitHelper.GetFace(horizontalRotation);
                data.Side = flip ? BuildingStructure.BitHelper.Flip(horizontalRotation) : horizontalRotation;
            }
            else if (data.PlaceholderType == BuildingElementType.Floor)
            {
                if (data.ShiftXZ == 1)
                {
                    int horizontalDirectionIndex = horizontalDirectionPickers[eightDirections].GetDirection(placeRotation, data.InterfaceRotation);
                    horizontalShift = horizontalDirectionPickers[eightDirections].Shift(horizontalDirectionIndex);
                }
                else if (data.ShiftXZ == 2)
                {
                    int horizontalDirectionIndex = horizontalDirectionPickers[sixteenDirections].GetDirection(placeRotation, data.InterfaceRotation);
                    horizontalShift = horizontalDirectionPickers[sixteenDirections].Shift(horizontalDirectionIndex);
                }
                data.Block = currentBlockIndex + verticalShift + horizontalShift + (VerticalDirectionPicker.Up * data.ShiftY);
                data.Type = data.PlaceholderType;
                data.Face = VerticalDirectionPicker.Under(verticalDirection) ? BuildingElementFace.Bottom : BuildingElementFace.Top;
                data.Side = horizontalRotation;
            }
            else if (data.PlaceholderType == BuildingElementType.Center)
            {
                if (data.ShiftXZ == 1)
                {
                    int horizontalDirectionIndex = horizontalDirectionPickers[eightDirections].GetDirection(placeRotation, data.InterfaceRotation);
                    horizontalShift = horizontalDirectionPickers[eightDirections].Shift(horizontalDirectionIndex);
                }
                else if (data.ShiftXZ == 2)
                {
                    int horizontalDirectionIndex = horizontalDirectionPickers[sixteenDirections].GetDirection(placeRotation, data.InterfaceRotation);
                    horizontalShift = horizontalDirectionPickers[sixteenDirections].Shift(horizontalDirectionIndex);
                }
                data.Block = currentBlockIndex + verticalShift + horizontalShift + (VerticalDirectionPicker.Up * data.ShiftY);
                data.Type = data.PlaceholderType;
                data.Face = BuildingElementFace.Center;
                data.Side = horizontalRotation;
            }
            else
            {
                BuildUtils.Error?.Report($"unknown Placeholder side: {data.Side}", MethodBase.GetCurrentMethod().DeclaringType.Name);
            }

            var checkResult = place.Structure.Check(data.Block.x, data.Block.y, data.Block.z, data.BuildRecipeDef, data.Face, data.Side, place.IsEmpty);
            data.Block = new UnityEngine.Vector3Int(checkResult.X, checkResult.Y, checkResult.Z);
            data.Face = checkResult.Face;
            data.Side = checkResult.Side;
            data.CanPlace.Clear(BuildingStructure.CheckResult.ReasonMask);
            data.CanPlace.Set(checkResult.Reason);

            var blockPosition = new UnityEngine.Vector3((data.Block.x * placeDef.BlockSize) + placeStart.x,
                                                        (data.Block.y * placeDef.BlockSize) + placeStart.y,
                                                        (data.Block.z * placeDef.BlockSize) + placeStart.z);
            var outOfPlace = false;
            if (placeDef.Square)
            {
                outOfPlace = (UnityEngine.Mathf.Abs(blockPosition.x) > placeRadius) || (UnityEngine.Mathf.Abs(blockPosition.z) > placeRadius) || (blockPosition.y < 0) || (blockPosition.y > placeHeight);
            }
            else
            {
                outOfPlace = (((blockPosition.x * blockPosition.x) + (blockPosition.z * blockPosition.z)) > (placeRadius * placeRadius)) || (blockPosition.y < 0) || (blockPosition.y > placeHeight);
            }
            data.CanPlace.Switch(outOfPlace, CanPlaceData.REASON_OUT_OF_PLACE);

            var localPosition = UnityEngine.Vector3.zero;
            var localRotation = UnityEngine.Quaternion.identity;

            var result = true;

            var effectiveRotation = UnityEngine.Quaternion.identity;
            if (data.Side == BuildingElementSide.Forward)
            {
                effectiveRotation = rotationForward;
            }
            else if (data.Side == BuildingElementSide.Left)
            {
                effectiveRotation = rotationLeft;
            }
            else if (data.Side == BuildingElementSide.Backward)
            {
                effectiveRotation = rotationBackward;
            }
            else if (data.Side == BuildingElementSide.Right)
            {
                effectiveRotation = rotationRight;
            }
            else
            {
                result = false;
            }

            var elementPosition = blockPosition;
            var shift = placeDef.BlockSize / 2.0f;
            if (data.Type == BuildingElementType.Wall)
            {
                var elementRotation = UnityEngine.Quaternion.identity;
                if (data.Face == BuildingElementFace.Forward)
                {
                    elementRotation = rotationForward;
                }
                else if (data.Face == BuildingElementFace.Left)
                {
                    elementRotation = rotationLeft;
                }
                else if (data.Face == BuildingElementFace.Backward)
                {
                    elementRotation = rotationBackward;
                }
                else if (data.Face == BuildingElementFace.Right)
                {
                    elementRotation = rotationRight;
                }
                else
                {
                    result = false;
                }
                elementPosition += elementRotation * (shiftForward * shift + localPosition);
            }
            else if (data.Type == BuildingElementType.Center)
            {
                elementPosition += shiftCenter * shift + effectiveRotation * localPosition;
            }
            else if (data.Type == BuildingElementType.Floor)
            {
                if (data.Face == BuildingElementFace.Bottom)
                {
                    elementPosition += shiftDown * shift + effectiveRotation * localPosition;
                }
                else if (data.Face == BuildingElementFace.Top)
                {
                    elementPosition += shiftUp * shift + effectiveRotation * localPosition;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            data.Position = placePosition + placeRotation * elementPosition;
            data.Rotation = placeRotation * effectiveRotation * localRotation;

            //if (BuildSystem.Builder.DebugMode)
            //{
            //    if (!data.CanPlace.Result)
            //    {
            //        data.CanPlace.PrintReason();
            //    }
            //}
            return result;
        }

        public static bool Calculate(this BuildingElementData data, BuildingPlaceData place, out UnityEngine.Vector3 resultPosition, out UnityEngine.Quaternion resultRotation)
        {
            resultPosition = (UnityEngine.Vector3)data.Position;
            resultRotation = (UnityEngine.Quaternion)data.Rotation;
            return true;
        }
    }
}
