using System;
using System.Threading;
using System.Threading.Tasks;
using ColonyShared.SharedCode.Utils;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Entities;
using SharedCode.EntitySystem;
using SharedCode.Repositories;
using SharedCode.Serializers;
using SharedCode.Wizardry;
using static SharedCode.EntitySystem.ReplicationLevel;
using static SharedCode.Utils.ReplicationMaskUtils;

namespace GeneratedCode.DeltaObjects
{
    public partial class SlaveWizardHolder : IHookOnReplicationLevelChanged //, IHookOnDestroy
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private SpellDoerAsync _spellDoer;
        private OuterRef<IWizardEntity> _slaveWizardRef;
        private UnityEnvironmentMark _mark;
        private readonly object _taskLock = new object();
        private TaskCompletionSource<bool> _prevTask;
        private CancellationTokenSource _initCancellation;

        public ISpellDoer SpellDoer => _spellDoer;

        protected override void constructor()
        {
            _spellDoer = new SpellDoerAsync();
        }
        
//        public Task OnDestroy() // Убрано так как OnDestroy вызывается ПЕРЕД OnReplicationLevelChanged :(
//        {
//            if (_slaveWizardRef.IsValid) Logger.IfError()?.Message(ParentEntityId, $"Not deinitialized {nameof(SlaveWizardHolder)}").Write();   
//            return Task.CompletedTask;
//        }
        
        public void OnReplicationLevelChanged(long oldReplicationMask, long newReplicationMask)
        {
            var ownerRef = new OuterRef<IEntity>(ParentEntityId, ParentTypeId);
            var isPlayer = (ReplicaTypeRegistry.GetMasterTypeByReplicationLevelType(ReplicaTypeRegistry.GetTypeById(ownerRef.TypeId)) == typeof(IWorldCharacter));

            Logger.IfDebug()
                ?.Message(ownerRef.Guid, $"OnReplicationLevelChanged from {(ReplicationLevel)oldReplicationMask} to {(ReplicationLevel)newReplicationMask} | Entity:{ParentEntityId} Repository:{EntitiesRepository.Id}.{EntitiesRepository.CloudNodeType}")
                .Write();
            
            if (IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ClientBroadcast) && !HasReplicationLevel(newReplicationMask, Master) ||
                IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, Master) && HasReplicationLevel(newReplicationMask, ClientBroadcast))
            {
                RunInitTask(async (token) =>
                {
                    Logger.IfDebug()
                        ?.Message(ownerRef.Guid, $"Get Host Wizard | Owner:{ownerRef} Repo:{EntitiesRepository.Id}.{EntitiesRepository.CloudNodeType}")
                        .Write();
                    var hostWizardRef = (await GetHostWizard(ownerRef, EntitiesRepository, token));
                    _mark = new UnityEnvironmentMark(HasReplicationLevel(newReplicationMask, Server) ? UnityEnvironmentMark.ServerOrClient.Server : UnityEnvironmentMark.ServerOrClient.Client);
                    Logger.IfDebug()
                        ?.Message(ownerRef.Guid, $"Create Slave Wizard | Owner:{ownerRef} HostWizard:{hostWizardRef} Repo:{EntitiesRepository.Id}.{EntitiesRepository.CloudNodeType} Mark:{_mark} isPlayer:{isPlayer}")
                        .Write();
                    _slaveWizardRef = await CreateSlaveWizard(hostWizardRef.To<IWizardEntityClientBroadcast>(), ownerRef, EntitiesRepository, _mark, isPlayer);
                });
            }
            else
            if (IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, ClientBroadcast) && !HasReplicationLevel(oldReplicationMask, Master) ||
                IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, Master) && HasReplicationLevel(oldReplicationMask, ClientBroadcast))
            {
                RunDestroyTask(async () =>
                {
                    if (_slaveWizardRef.IsValid)
                    {
                        Logger.IfDebug()
                            ?.Message(ownerRef.Guid, $"Destroy Slave Wizard | Owner:{ownerRef} Repo:{EntitiesRepository.Id}.{EntitiesRepository.CloudNodeType} Mark:{_mark} isPlayer:{isPlayer}")
                            .Write();
                        await EntitiesRepository.Destroy<IWizardEntity>(_slaveWizardRef.Guid);
                        _slaveWizardRef = default;
                    }
                    _mark?.Dispose();
                    _mark = null;
                });
            }

            if (IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, ClientFull) && !HasReplicationLevel(newReplicationMask, Server) ||
                IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, Server) && HasReplicationLevel(newReplicationMask, ClientFull))
            {
                RunInitTask(async token =>
                {
                    var hostWizardRef = (await GetHostWizard(ownerRef, EntitiesRepository, token));
                    Logger.IfDebug()
                        ?.Message(ownerRef.Guid, $"Activate Spell Doer | HostWizard:{hostWizardRef} SlaveWizard:{_slaveWizardRef} Repo:{EntitiesRepository.Id}.{EntitiesRepository.CloudNodeType} Mark:{_mark} isPlayer:{isPlayer}")
                        .Write();
                    _mark?.SetClientAuthority(true);
                    if (!_slaveWizardRef.IsValid) throw new Exception($"{nameof(_slaveWizardRef)} is not valid");
                    _spellDoer.Activate(_slaveWizardRef, ownerRef, EntitiesRepository, _mark, isPlayer);
                    await _spellDoer.SubscribeToWizard(hostWizardRef.To<IWizardEntityClientBroadcast>(), EntitiesRepository);
                });
            }
            else
            if (IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, ClientFull) && !HasReplicationLevel(oldReplicationMask, Server) ||
                IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, Server) && HasReplicationLevel(oldReplicationMask, ClientFull))
            {
                RunDestroyTask(async () =>
                {
                    Logger.IfDebug()
                        ?.Message(ownerRef.Guid, $"Deactivate Spell Doer | Owner:{ownerRef} SlaveWizard:{_slaveWizardRef} Repo:{EntitiesRepository.Id}.{EntitiesRepository.CloudNodeType} Mark:{_mark} isPlayer:{isPlayer}")
                        .Write();
                    _mark?.SetClientAuthority(false);
                    await _spellDoer.UnsubscribeFromWizard();
                    _spellDoer.Deactivate();
                });
            }
            if (IsReplicationLevelAdded(oldReplicationMask, newReplicationMask, Master))
            {
                RunInitTask(async token =>
                {
                    var hostWizardRef = (await GetHostWizard(ownerRef, EntitiesRepository, token));
                    Logger.IfDebug()
                        ?.Message(ownerRef.Guid, $"Activate Spell Doer | HostWizard:{hostWizardRef} SlaveWizard:{_slaveWizardRef} Repo:{EntitiesRepository.Id}.{EntitiesRepository.CloudNodeType} Mark:{_mark} isPlayer:{isPlayer}")
                        .Write();
                    _slaveWizardRef = hostWizardRef;
                    _mark = new UnityEnvironmentMark(HasReplicationLevel(newReplicationMask, Server) ? UnityEnvironmentMark.ServerOrClient.Server : UnityEnvironmentMark.ServerOrClient.Client);
                    if (!_slaveWizardRef.IsValid) throw new Exception($"{nameof(_slaveWizardRef)} is not valid");
                    _spellDoer.Activate(_slaveWizardRef, ownerRef, EntitiesRepository, _mark, isPlayer);
                    await _spellDoer.SubscribeToWizard(hostWizardRef.To<IWizardEntityClientBroadcast>(), EntitiesRepository);
                });
            }
            else
           if (IsReplicationLevelRemoved(oldReplicationMask, newReplicationMask, Master))
            {
                RunDestroyTask(async () =>
                {
                    Logger.IfDebug()
                        ?.Message(ownerRef.Guid, $"Deactivate Spell Doer | Owner:{ownerRef} SlaveWizard:{_slaveWizardRef} Repo:{EntitiesRepository.Id}.{EntitiesRepository.CloudNodeType} Mark:{_mark} isPlayer:{isPlayer}")
                        .Write();
                    await _spellDoer.UnsubscribeFromWizard();
                    _spellDoer.Deactivate();
                });
            }

        }

        private void RunInitTask(Func<CancellationToken, Task> task)
        {
            lock (_taskLock)
            {
                var cancellationToken = (_initCancellation = _initCancellation ?? new CancellationTokenSource()).Token;
                var currentTask = new TaskCompletionSource<bool>();
                var prevTask = _prevTask;
                _prevTask = currentTask;
                AsyncUtils.RunAsyncTask(async () =>
                {
                    try { if (prevTask != null) await prevTask.Task; } catch (OperationCanceledException) {}
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await task(cancellationToken);
                        currentTask.SetResult(true);
                    }
                    catch (OperationCanceledException) { currentTask.SetCanceled(); }
                    catch (Exception e) { Logger.IfError()?.Exception(e).Write(); currentTask.SetCanceled(); }
                });
            }
        }

        private void RunDestroyTask(Func<Task> task)
        {
            lock (_taskLock)
            {
                CancelAndDispose(ref _initCancellation);
                var currentTask = new TaskCompletionSource<bool>();
                var prevTask = _prevTask;
                _prevTask = currentTask;
                AsyncUtils.RunAsyncTask(async () =>
                {
                    try { if (prevTask != null) await prevTask.Task; } catch (OperationCanceledException) {}
                    try
                    {
                        await task();
                        currentTask.SetResult(true);
                    }
                    catch (Exception e) { Logger.IfError()?.Exception(e).Write(); currentTask.SetCanceled(); }
                });
            }
        }

        private static void CancelAndDispose(ref CancellationTokenSource cts)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }

        private static async Task<OuterRef<IWizardEntity>> GetHostWizard(OuterRef<IEntity> ownerRef, IEntitiesRepository repo, CancellationToken cancellationToken)
        {
            OuterRef<IWizardEntity> hostWizardRef = default;
            using (CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, repo.StopToken))
            {
                for (; hostWizardRef == default; await Task.Delay(TimeSpan.FromMilliseconds(100), linkedCts.Token)) // TODO: переделать с подпиской на появление энтити в репе?
                {
                    linkedCts.Token.ThrowIfCancellationRequested();
                    using (var hasWizard = await repo.Get(ownerRef.TypeId, ownerRef.Guid))
                    {
                        var has = hasWizard.Get<IHasWizardEntityClientBroadcast>(ownerRef.TypeId, ownerRef.Guid, ClientBroadcast);
                        if (has != null)
                        {
                            if (has.Wizard == null)
                                throw new Exception($"No Wizard in {ownerRef}");
                            hostWizardRef = new OuterRef<IWizardEntity>(has.Wizard.Id, has.Wizard.TypeId);
                        }
                    }
                }
            }
            return hostWizardRef;
        }

        private static async Task<OuterRef<IWizardEntity>> CreateSlaveWizard(OuterRef<IWizardEntityClientBroadcast> hostWizardRef, OuterRef<IEntity> ownerRef, IEntitiesRepository repo, UnityEnvironmentMark mark, bool isPlayer)
        {
            var slaveWizard = await repo.Create<IWizardEntity>(Guid.NewGuid(), w =>
            {
                w.IsInterestingEnoughToLog = isPlayer && Constants.WorldConstants.IsCharacterInterestingEnoughtToLogWizardEvents;
                w.Owner = ownerRef;
                w.SlaveWizardMark = mark;
                return Task.CompletedTask;
            });
            using (var localWizardContainer = await repo.Get<IWizardEntity>(slaveWizard.Id))
            {
                var wizard = localWizardContainer.Get<IWizardEntity>(slaveWizard.Id);
                await wizard.ConnectToHostAsReplica(hostWizardRef.To<IWizardEntity>());
            }
            return slaveWizard.OuterRef.To<IWizardEntity>();
        }
    }
}