using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Aspects;
using ColonyShared.SharedCode.Aspects.Combat;
using ResourceSystem.Aspects.Misc;
using Src.Animation;
using Src.Tools;

namespace Src.Aspects.Doings
{
    public interface IAttackDoerSupport
    {
        Task<AttackAnimationInfo> FetchAttackAnimationInfo(object playId);
        bool TryGetTrajectory(AnimationStateDef stateDef, GameObjectMarkerDef bodyPart, out AnimationTrajectory trajectory);
    }

  
}