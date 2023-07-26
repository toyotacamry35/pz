using System;
using Assets.Src.Aspects;
using Assets.Src.ContainerApis;
using Assets.Src.GameObjectAssembler.Res;
using Core.Environment.Logging.Extension;
using NLog;
using NLog.Fluent;
using UnityEngine;

namespace Assets.Src.SpawnSystem
{
    //#Note: Is needed on every EntityApi owners to trigger EntityApi OnGot/LostClient methods, while task #PZ-6144 is not done.
    //.. Is added dynamically in EGO Awake.
    [DisallowMultipleComponent]
    public class TmpPlugEntityApiEgoComponentRetransator : EntityGameObjectComponent
    {
        protected static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private event Action GotClientAction;
        private event Action LostClientAction;

        public void Subscribe_TmpPlug_GotLostClient(EntityApi eApi, SubscribeUnsubscribe instruction)
        {
            Logger.If(LogLevel.Debug)
                ?.Message($"Subscribe_TmpPlug_GotLostClient({instruction}) for \"{gameObject.name}\"")
                .UnityObj(gameObject)
                .Write();

            switch (instruction)
            {
                case SubscribeUnsubscribe.Subscribe:
                    GotClientAction  += eApi.OnGotClient;
                    LostClientAction += eApi.OnLostClient;
                    if (IsClient)
                        eApi.OnGotClient();
                    break;

                case SubscribeUnsubscribe.Unsubscribe:
                    GotClientAction  -= eApi.OnGotClient;
                    LostClientAction -= eApi.OnLostClient;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
            }
        }

        protected override void GotClient()
        {
            GotClientAction?.Invoke();
        }

        protected override void LostClient()
        {
            LostClientAction?.Invoke();
        }

    }
}
