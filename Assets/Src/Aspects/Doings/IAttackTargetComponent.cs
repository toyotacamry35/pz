using System;
using ResourceSystem.Utils;

namespace Src.Aspects.Doings
{
    public interface IAttackTargetComponent
    {
        OuterRef EntityId { get; }
        
        Guid SubObjectId { get; }
    }
}