using UnityEngine;
using System.Collections.Generic;

namespace Assets.Src.Cartographer.Editor
{
    public class CartographerNameComparer : IComparer<string>
    {
        public static CartographerNameComparer Comparer { get; private set; } = new CartographerNameComparer();

        public int Compare(string left, string right)
        {
            Vector3Int _leftPoint;
            Vector3Int _rightPoint;
            var _left = CartographerCommon.IsSceneForStreaming(left, out _leftPoint);
            var _right = CartographerCommon.IsSceneForStreaming(right, out _rightPoint);
            if (_left && _right)
            {
                if (_leftPoint.z == _rightPoint.z)
                {
                    return _leftPoint.x.CompareTo(_rightPoint.x);
                }
                else
                {
                    return _leftPoint.z.CompareTo(_rightPoint.z);
                }
            }
            else if (!_right && !_left)
            {
                return left.CompareTo(right);
            }
            else
            {
                return _left.CompareTo(_right);
            }
        }
    }
};