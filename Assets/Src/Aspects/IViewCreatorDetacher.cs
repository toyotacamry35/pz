using Assets.Src.Aspects.Impl;
using SharedCode.Wizardry;
using UnityEngine;

namespace Assets.Src.Aspects
{
    public interface IViewCreatorDetacher
    {
        GameObject DetachView();
        void OnDieU(UnityEnvironmentMark.ServerOrClient context);
        void OnResurrectU(UnityEnvironmentMark.ServerOrClient context, PositionRotation pose);
        //     Animator Animator { get; }
        //       IStream<GameObject> ViewGameObject { get; }
    }
}