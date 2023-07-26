using System.Threading.Tasks;
using Assets.Src.Cluster.Cheats;
using Assets.Src.Lib.Cheats;
using Assets.Src.SpawnSystem;
using Assets.Tools;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ShareCode.Threading;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using Uins;
using UnityAsyncAwaitUtil;

namespace Assets.Src.Aspects.Impl
{
    public class CharacterChatComponent : EntityGameObjectComponent
    {
        private const string ChatCommandChar = "/";
        

        //=== Protected =======================================================

        protected override void GotClient()
        {
            var repo = ClientRepo;
            var @ref = GetOuterRef<IEntity>();
            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repo.Get(@ref))
                {
                    var characterClientBroadcast = wrapper?.Get<IWorldCharacterClientBroadcast>(@ref, ReplicationLevel.ClientBroadcast);
                    if (!characterClientBroadcast.AssertIfNull(nameof(characterClientBroadcast)) &&
                        await characterClientBroadcast.Mortal.GetIsAlive())
                    {
                        characterClientBroadcast.NewChatMessageEvent += OnNewChatMessage;
                    }
                }
            });
        }

        protected override void LostClient()
        {
            var repo = ClientRepo;
            var @ref = GetOuterRef<IEntity>();
            if(repo != null)
                AsyncUtils.RunAsyncTask(async () =>
                {
                    using (var wrapper = await repo.Get(@ref))
                    {
                        var characterClientBroadcast = wrapper?.Get<IWorldCharacterClientBroadcast>(@ref, ReplicationLevel.ClientBroadcast);
                        if (characterClientBroadcast != null)
                        {
                            characterClientBroadcast.NewChatMessageEvent -= OnNewChatMessage;
                        }
                    }
                });
        }


        //=== Public ==========================================================

        public void SendChatMessage(string message)
        {
            if (!HasClientAuthority)
            {
                UI. Logger.IfError()?.Message("Unable to send message cause hasn't client authority: msg='{message}', {entityIdName}={EntityId}",  message, nameof(EntityId), EntityId).Write();
                return;
            }

            var repo = ClientRepo;
            var @ref = GetOuterRef<IEntity>();
            var allowCommands = ClientCheatsState.Chat;
            
            AsyncUtils.RunAsyncTask(async () =>
            {
                if (allowCommands && message.StartsWith(ChatCommandChar))
                {
                    var cheatCommand = message.Remove(0, 1);
                    await UnityQueueHelper.RunInUnityThread(async () =>
                    {
                        var res = await CheatExecutor.Execute(ClientRepo, System.Guid.Empty, cheatCommand);
                        await OnNewChatMessage("Cheat System", res.ToString());
                    });
                }
                else
                {
                    using (var wrapper = await repo.Get(@ref))
                    {
                        var characterClientFull = wrapper?.Get<IWorldCharacterClientFull>(@ref, ReplicationLevel.ClientFull);
                        if (characterClientFull.AssertIfNull(nameof(characterClientFull)))
                            return;

                        await characterClientFull.SendChatMessage(message);
                    }
                }
            });
        }


        //=== Private =========================================================

        private async Task OnNewChatMessage(string senderName, string message)
        {
            await UnityQueueHelper.RunInUnityThread(() =>
            {
                if (ChatPanel.Instance.AssertIfNull(nameof(ChatPanel)))
                    return;

                ChatPanel.Instance.AddNewMessage(senderName, message);
            });
        }
    }
}