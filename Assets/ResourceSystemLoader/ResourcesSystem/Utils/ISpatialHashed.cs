using System.Collections.Generic;

namespace SharedCode.Utils
{
    public interface ISpatialHashed
    {
        void GetHash(ISpatialHash spHash, ICollection<Vector3Int> resultHash);
    }
}
