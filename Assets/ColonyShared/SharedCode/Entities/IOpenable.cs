using ResourceSystem.Utils;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.Entities
{
    public interface IOpenable
    {
        [ReplicationLevel(ReplicationLevel.Server)] Task<OuterRef> GetOpenOuterRef(OuterRef oref);
    }
}
