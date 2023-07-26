using GeneratorAnnotations;
using ResourceSystem.ContentKeys;
using SharedCode.Cloud;
using SharedCode.Config;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneratedCode.Manual.Repositories;
using Core.Cheats;
using GeneratedCode.Manual.AsyncStack;
using SharedCode.Serializers;

namespace GeneratedCode.ContentKeys
{
    [EntityService(replicateToNodeType: CloudNodeType.Server | CloudNodeType.Client)]
    [GenerateDeltaObjectCode]
    public interface IContentKeyServiceEntity : IEntity
    {
        IDeltaList<ContentKeyDef> Keys { get; set; }

        [CheatRpc(SharedCode.Entities.Service.AccountType.GameMaster)]
        [Cheat]
        Task<string> EnableKey(string key);

        [CheatRpc(SharedCode.Entities.Service.AccountType.GameMaster)]
        [Cheat]
        Task<string> DisableKey(string key);
    }
}

namespace GeneratedCode.DeltaObjects
{

    public static class ContentKeyRequiremenetExt
    {
        public static bool IsEnabled(this ContentKeyRequirement req) => !req.NotIncludedByDefault ||
            (req.NotIncludedByDefault && ContentKeyServiceEntity.ContainsKey(req.Key));
    }
    public partial class ContentKeyServiceEntity : IHasLoadFromJObject, IHookOnReplicationLevelChanged
    {
        private static IReadOnlyDictionary<ContentKeyDef, ContentKeyDef> _keys = new Dictionary<ContentKeyDef, ContentKeyDef>();

        public static bool ContainsKey(ContentKeyDef key) => _keys.ContainsKey(key);

        public static ContentKeyDef[] AllKeys => _keys.Keys.ToArray();

        public Task Load(CloudSharedDataConfig sharedConfig, CustomConfig config, IEntitiesRepository entitiesRepository)
        {
            var cfg = (ContentKeyConfig)config;
            
            foreach (var key in cfg.Keys)
                Keys.Add(key);

            return Task.CompletedTask;
        }

        public async void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            await AsyncUtils.RunAsyncTask(async () =>
            {
                using (await this.GetThisRead())
                {
                    if (oldReplicationMask == 0)
                    {
                        await OnKeysChanged(null);
                        Subscribe(nameof(Keys), OnKeysChanged);
                    }

                    if (newReplicationMask == 0)
                        Unsubscribe(nameof(Keys), OnKeysChanged);
                }
            });
        }

        private Task OnKeysChanged(EntityEventArgs args)
        {
            _keys = Keys.ToDictionary(v => v, v => v);

            return Task.CompletedTask;
        }

        public Task<string> EnableKeyImpl(string key)
        {
            throw new NotImplementedException();
        }
        public Task<string> DisableKeyImpl(string key)
        {
            throw new NotImplementedException();
        }
    }
}
