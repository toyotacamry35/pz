using Assets.Tools;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using SharedPosRot = SharedCode.Entities.GameObjectEntities.PositionRotation;

namespace Assets.Src.Aspects.Doings
{
    internal class ClientBotRespawner
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Bot");

        private static readonly ConcurrentDictionary<Guid, bool> knownBots = new ConcurrentDictionary<Guid, bool>();

        public static void Register(Guid botGuid)
        {
            if (knownBots.ContainsKey(botGuid))
                return;

            knownBots.TryAdd(botGuid, false);

            var repo = GameState.Instance.ClientClusterNode;
            AsyncUtils.RunAsyncTask(async () =>
            {
                try
                {
                    using (var wrapper = await repo.Get<IWorldCharacterClientFull>(botGuid))
                    {
                        var bot = wrapper?.Get<IWorldCharacterClientFull>(botGuid);
                        if (bot == null)
                            throw new InvalidOperationException($"No bot replica {botGuid}");

                        bot.Mortal.DieEvent += BotOnDie;
                        AsyncUtils.RunAsyncTask(() => CheckHealth(botGuid, repo, CancellationToken.None), repo); // типа на случай если DieEvent не сработает?
                    }
                }
                catch (Exception e)
                {
                    Logger.IfError()?.Message(e, "Bot {0}: Cant subscribe for the death event", botGuid).Write();
                }

            }, repo);
        }

        private static Task BotOnDie(Guid botGuid, int typeId, SharedPosRot corpsePlace)
        {
            var repo = GameState.Instance.ClientClusterNode;
            AsyncUtils.RunAsyncTask(() => CheckAndRespawn(botGuid, repo, CancellationToken.None), repo);
            return Task.CompletedTask;
        }
        static Random _rnd = new Random();
        private static async Task CheckHealth(Guid botGuid, IEntitiesRepository repo, CancellationToken ct)
        {
            try
            {
                bool wasAlive = await CheckIsAlive(botGuid, repo);
                while (!ct.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(100 + _rnd.Next(0, 100)), ct);
                    var isAlive = await CheckIsAlive(botGuid, repo);
                    if (!isAlive && !wasAlive)
                    {
                         Logger.IfError()?.Message("Bot {0} is dead too long time (100+ seconds), invoking forced revival",  botGuid).Write();
                        await Respawn(botGuid, repo);
                    }
                    wasAlive = isAlive;
                }
            }
            catch (RepositoryTimeoutException e)
            {
                Logger.IfError()?.Message(e, "Bot {0}: Timeout while checking health", botGuid).Write();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Bot {0}: Broken while checking health: {1}", botGuid, e).Write();
            }
        }
        private static async Task CheckAndRespawn(Guid botGuid, IEntitiesRepository repo, CancellationToken ct)
        {
             Logger.IfInfo()?.Message("Bot {0}: Dead, delaying respawn",  botGuid).Write();

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), ct);

                while (!await CheckIsAlive(botGuid, repo))
                {
                     Logger.IfInfo()?.Message("Bot {0}: Still dead, trying to respawn",  botGuid).Write();
                    await Respawn(botGuid, repo);
                    await Task.Delay(TimeSpan.FromSeconds(10), ct);
                }

                 Logger.IfInfo()?.Message("Bot {0}: Respawned",  botGuid).Write();
            }
            catch(RepositoryTimeoutException e)
            {
                Logger.IfError()?.Message(e, "Bot {0}: Timeout while respawning, trying again", botGuid).Write();
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Bot {0}: Broken, dropping", botGuid).Write();
            }
        }

        private static async Task<bool> CheckIsAlive(Guid botGuid, IEntitiesRepository repo)
        {
            using (var wrapper = await repo.Get<IWorldCharacterClientFull>(botGuid))
            {
                var bot = wrapper?.Get<IWorldCharacterClientFull>(botGuid);
                if (bot == null)
                    throw new InvalidOperationException($"Lost bot replica {botGuid}");

                return bot.Mortal.IsAlive;
            }
        }

        private static async Task Respawn(Guid botGuid, IEntitiesRepository repo)
        {
            try
            {
                using (var wrapper = await repo.Get<IWorldCharacterClientFull>(botGuid))
                {
                    var bot = wrapper?.Get<IWorldCharacterClientFull>(botGuid);
                    if (bot == null)
                        throw new InvalidOperationException($"No bot replica {botGuid}");

                    await bot.Respawn(false, false, default);
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "Bot {0}: Cant subscribe for the death event", botGuid).Write();
            }
        }
    }
}
