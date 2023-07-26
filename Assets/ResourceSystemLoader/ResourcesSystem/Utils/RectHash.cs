using System.Collections.Generic;

namespace SharedCode.Utils
{
    public struct RectHash : Utils.ISpatialHashed
    {
        private readonly Vector3 _pos;
        private readonly Vector3 _size;

        public RectHash(Vector3 pos, Vector3 size)
        {
            this._pos = pos;
            this._size = size;
        }

        public void GetHash(ISpatialHash spHash, ICollection<Vector3Int> resultHash)
        {
            //var center = spHash.PosHash(_pos);
            var topLeftCorner = spHash.PosHash(_pos - _size / 2);
            var bottomRightCorner = spHash.PosHash(_pos + _size / 2);
            for (int x = topLeftCorner.x; x <= bottomRightCorner.x; x++)
                for (int y = topLeftCorner.y; y <= bottomRightCorner.y; y++)
                    for (int z = topLeftCorner.z; z <= bottomRightCorner.z; z++)
                        resultHash.Add(new Vector3Int(x, y, z));
        }
    }
}
