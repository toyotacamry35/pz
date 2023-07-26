using Assets.ResourceSystem.Custom.Config;
using Assets.Src.RubiconAI;
using GeneratedCode.Custom.Config;
using GeneratedCode.EntityModel.Bots;
using SharedCode.AI;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Config;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    public partial class BotCoordinator : IHasLoadFromJObject
    {
        public static bool IsActive =
            #if DEBUG
                Constants.WorldConstants.SpawnBots;
            #else
                false;
            #endif

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private AIWorld AIWorld;
        private bool IsInitialized;
        private BotCoordinatorConfig _config;

        public Task Load(CloudSharedDataConfig sharedConfig, CustomConfig config, IEntitiesRepository entitiesRepository)
        {
            _config = (BotCoordinatorConfig)config;
            BotSpawnPointTypeDef = _config?.BotSpawnPointTypeDef.Target;
            return Task.CompletedTask;
        }

        public Task RegisterImpl()
        {
            if (!IsInitialized)
            {
                if (AIWorld == null)
                {
                    AIWorld = new AIWorld(EntitiesRepository, true, AIWorldMode.Bot);
                    AIWorld.Register(EntitiesRepository.Id);
                }
            }

            return Task.CompletedTask;
        }

        public async Task InitializeImpl(MapDef mapDef, List<OuterRef<IEntity>> botsRefs, LegionaryEntityDef botConfig)
        {
            if (AIWorld == null)
            {
                Logger.IfError()?.Message($"Bots System is NOT initialized! Trying to run late initialisation...").Write();
                await RegisterImpl();
            }

            if (!IsInitialized)
            {
                IsInitialized = true;
                AIWorld.Run();
                AIWorld.SetBots(botsRefs.ToDictionary(k => k, v => botConfig));
            }
        }

        public Task ActivateBotsImpl(Guid account, List<Guid> botsIds)
        {
            if (!BotsByAccount.TryGetValue(account, out IBotsHolder bots))
            {
                bots = new BotsHolder() { Bots = new DeltaDictionary<Guid, bool>() };
                BotsByAccount.Add(account, bots);
            }

            foreach (var botId in botsIds)
            {
                if (bots.Bots.ContainsKey(botId))
                    bots.Bots[botId] = true;
                else
                    bots.Bots.Add(botId, true);
            }

            return Task.CompletedTask;
        }

        public Task DeactivateBotsImpl(Guid account)
        {
            if (BotsByAccount.TryGetValue(account, out IBotsHolder bots))
            {
                foreach (var bot in bots.Bots.Keys.ToArray())
                {
                    bots.Bots[bot] = false;
                }
            }

            return Task.CompletedTask;
        }
    }
}
