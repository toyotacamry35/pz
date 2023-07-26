using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldObjectPositionInformation
    {
        public Task<bool> SetPositionImpl(Vector3 position)
        {
            Position = position;
            return Task.FromResult(true);
        }
    }
}
