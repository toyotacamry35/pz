using Assets.Src.Aspects.Impl;
using Assets.Src.SpawnSystem;
using SharedCode.Wizardry;
using Uins.Sound;

namespace Assets.Src.Aspects
{
    public class MortalCharacter : EntityGameObjectComponent, IDeathResurrectListenerComponent
    {
        void IDeathResurrectListenerComponent.OnResurrect(UnityEnvironmentMark.ServerOrClient ctx, PositionRotation at)
        {}

        void IDeathResurrectListenerComponent.OnDeath(UnityEnvironmentMark.ServerOrClient ctx)
        {
            if (ctx != UnityEnvironmentMark.ServerOrClient.Client || !HasClientAuthority)
                return;

            SoundControl.Instance?.OnDeath();
        }
    }
}
