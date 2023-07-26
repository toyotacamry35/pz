using Assets.Src.Aspects.Impl;
using static SharedCode.Wizardry.UnityEnvironmentMark;

namespace Assets.Src.Aspects
{
    public interface IDeathResurrectListenerComponent
    {
        // @param ServerOrClient ctx - where callbacks is called from
        void OnResurrect(ServerOrClient ctx, PositionRotation at);
        // @param ServerOrClient ctx - where callbacks is called from
        void OnDeath(ServerOrClient ctx);
    }
}