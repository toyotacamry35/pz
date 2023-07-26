using SharedCode.Entities.Regions;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProtoBuf;
using SharedCode.Entities;
using GeneratorAnnotations;
using Assets.ResourceSystem.Arithmetic.Templates.Predicates;
using GeneratedCode.DeltaObjects;
using ReactivePropsNs.ThreadSafe;
using ResourceSystem.Aspects;

namespace SharedCode.Wizardry
{

    public delegate Task SpellFinished(long timeStamp, SpellId id, SpellFinishReason reason);
    public delegate Task SpellStarted(long timeStamp, SpellId id, ISpellCast cast);
    [GenerateDeltaObjectCode]
    public interface IWizardEntity : IEntity, IHasPingDiagnostics
    {
        [ReplicationLevel(ReplicationLevel.Master)] OuterRef<IWizardEntity> HostWizard { get; }
        [ReplicationLevel(ReplicationLevel.Master)] bool IsDead { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] bool IsInterestingEnoughToLog { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)] SpellId Counter { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)] SpellId SlaveCounter { get; set; }

        [ReplicationLevel(ReplicationLevel.Master)] Task<SpellId> NewSpellId();
        
        [ReplicationLevel(ReplicationLevel.Master)] Task<bool> ConnectToHostAsReplica(OuterRef<IWizardEntity> host);
        [ReplicationLevel(ReplicationLevel.Master)] Task<bool> CastSpellFromHost(SpellId id, SpellCast spell);
        [ReplicationLevel(ReplicationLevel.Master)] Task<bool> StopSpellFromHost(SpellId id, SpellFinishReason reason, long timeStamp);
        [ReplicationLevel(ReplicationLevel.Master)] Task<bool> SpellFinishedDelay(SpellId spell);
        [ReplicationLevel(ReplicationLevel.Master)] Task<bool> OnLostPossiblyImportantEntity(OuterRef<IEntity> ent);
        [ReplicationLevel(ReplicationLevel.Master)] Task<bool> WatchdogUpdate();
        [ReplicationLevel(ReplicationLevel.Master)] IDeltaList<SpellThatMustBeStoppedAtStart> SpellsThatMustBeStoppedAtStart { get; set; }
        [ReplicationLevel(ReplicationLevel.Master)] IDeltaDictionary<SpellId,bool> CanceledSpells { get; set; }


        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<bool> StopAllSpellsOfGroup(SpellGroupDef group, SpellId except, SpellFinishReason reason);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<bool> StopSpellByDef(SpellDef spellDef, SpellId except, SpellFinishReason reason);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<bool> StopSpellByCauser(SpellPartCastId causer, SpellFinishReason reason);
        
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] [ReplicationLevel(ReplicationLevel.ClientBroadcast)] ValueTask<bool> HasActiveSpell(SpellDef spell);
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] [ReplicationLevel(ReplicationLevel.ClientBroadcast)] ValueTask<bool> HasActiveSpellGroup(SpellGroupDef group);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<string> DumpEvents();

        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] OuterRef<IEntity> Owner { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientBroadcast)] IDeltaDictionary<SpellId, ISpell> Spells { get; set; }
        [ReplicationLevel(ReplicationLevel.ClientFull)] IDeltaDictionary<CooldownGroupDef, long> CooldownsUntil { get; set; }

        //immutable as in "doesn't change any serialized data"
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] Task<bool> LocalUpdateTimeLineData();
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] Task<bool> CheckSpellCastPredicates(long currentTime, SpellCast spell, List<SpellPredicateDef> failedPredicates, PredicateIgnoreGroupDef predicateIgnoreGroupDef = null);
        [EntityMethodCallType(EntityMethodCallType.ImmutableLocal)] Task<bool> HasSpellsPreventingThisFromStart(SpellCast spell);

        [RuntimeData]
        UnityEnvironmentMark SlaveWizardMark { get; set; }

        Task<SpellId> CastSpell(SpellCast spell);
        Task<SpellId> CastSpell(SpellCast spell, SpellId clientSpellId);
        Task<SpellId> CastSpell(SpellCast spell, SpellId clientSpellId, SpellId prevSpell);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<bool> StopCastSpell(SpellId spell, SpellFinishReason reason);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task<bool> StopCastSpell(SpellId spell);
        Task<long> Update();
        Task<bool> Update(SpellId spellId);
        Task<WizardDebugData> GetDebugData();
        Task<bool> WizardHasDied();
        Task<SpellId> WizardHasRisen();
        [ReplicationLevel(ReplicationLevel.Master)]
        Task GetBackFromIdleMode();
        [ReplicationLevel(ReplicationLevel.Master)]
        Task GoIntoIdleMode();
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task CancelSpell(SpellId spellId);
        [ReplicationLevel(ReplicationLevel.ClientFull)] Task SetIsInterestingEnoughToLog(bool enable);
    }

    [ProtoContract]
    public struct SpellThatMustBeStoppedAtStart
    {
        [ProtoMember(1)] public SpellId SpellId;
        [ProtoMember(2)] public long ExpiredAt;
        [ProtoMember(3)] public SpellFinishReason Reason;
    }

    // --- Util types: ---------------------------------------------------------------------

    public interface IUnityEnvironmentMark
    {
        bool HasClientAuthority { get; }
        IStream<bool> HasClientAuthorityStream { get; }
        bool OnServer { get; }
        bool OnClient { get; }
    }

    public class UnityEnvironmentMark : IUnityEnvironmentMark, IDisposable
    {
        private readonly ReactiveProperty<bool> _hasClientAuthorityStream = new ReactiveProperty<bool>();
        private readonly ServerOrClient _whereItIs;
        
        public enum ServerOrClient
        {
            None = 0,
            Server = 1,
            Client = 2
        }

        public UnityEnvironmentMark(ServerOrClient whereItIs)
        {
            _whereItIs = whereItIs;
        }

        public bool HasClientAuthority { get; private set; }
        public IStream<bool> HasClientAuthorityStream => _hasClientAuthorityStream;
        public bool OnServer => (_whereItIs & ServerOrClient.Server) == ServerOrClient.Server;
        public bool OnClient => (_whereItIs & ServerOrClient.Client) == ServerOrClient.Client;

        public void SetClientAuthority(bool hasAuthority)
        {
            HasClientAuthority = hasAuthority;
            _hasClientAuthorityStream.Value = hasAuthority;
        }

        public override string ToString()
        {
            return _whereItIs.ToString() + (HasClientAuthority ? " Authority" : String.Empty);
        }

        public void Dispose()
        {
            _hasClientAuthorityStream.Dispose();
        }
    }

    public static class UnityEnvironmentMaskExtensions
    {
        public static bool IsSlave(this IUnityEnvironmentMark mark) => mark != null;
        
        public static bool OnClient(this IUnityEnvironmentMark mark) => mark != null && mark.OnClient;

        public static bool OnServerMaster(this IUnityEnvironmentMark mark) => mark == null;

        public static bool OnServerSlave(this IUnityEnvironmentMark mark) => mark != null && mark.OnServer && !mark.OnClient;
    }
    
    public class WizardDebugData
    {
        public List<SpellTimeLineData> CurrentSpellStatuses = new List<SpellTimeLineData>();
        public List<SpellTimeLineData> SlaveCurrentSpellStatuses = new List<SpellTimeLineData>();
    }
}
