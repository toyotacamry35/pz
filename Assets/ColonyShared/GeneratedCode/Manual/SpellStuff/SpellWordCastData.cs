using System;
using System.Collections.Generic;
using System.Linq;
using ColonyShared.SharedCode.Modifiers;
using SharedCode.Wizardry;
using SharedCode.EntitySystem;
using ColonyShared.SharedCode.Utils;
using ResourceSystem.Aspects;
using ResourceSystem.Utils;
using SharedCode.Utils;
using Scripting;

namespace GeneratedCode.DeltaObjects
{
    public class SpellCastData
    {
        public readonly OuterRef<IEntity> Caster;
        public readonly ISpellCast CastData;
        
        public SpellCastData(ISpellCast castData, OuterRef<IEntity> caster)
        {
            CastData = castData;
            Caster = caster;
        }
        
        public override string ToString()
        {
            return StringBuildersPool.Get
                .Append("[")
                .Append("Caster:").Append(Caster).Append(" ")
                .Append("CastData:").Append(CastData).Append(" ")
                .Append("]")
                .ToStringAndReturn();                
        }
    }
    
    
    public class SpellPredCastData : SpellCastData // Переделать на struct нельзя, так как в асинхронные методы нельзя передавать параметры по ссылке (in), а передвать копированием, скорее всего, снизит перфоманс, да и скорее всего памяти будет тратится ещё больше при захвате data в кложуры в методах эффектов и импактов   
    {
        public readonly IUnityEnvironmentMark SlaveMark;
        public readonly long CurrentTime;
        public readonly OuterRef<IWizardEntity> Wizard;
        public readonly bool Canceled;
        public readonly IReadOnlyList<SpellModifierDef> Modifiers;
        protected IEntitiesRepository Repo;

        public SpellPredCastData(ISpellCast castData, long currentTime, OuterRef<IWizardEntity> wizard, OuterRef<IEntity> caster, IUnityEnvironmentMark slaveMark, IReadOnlyList<SpellModifierDef> modifiers, bool canceled, IEntitiesRepository repo)
        : base(castData, caster)
        {
            Repo = repo;
            CurrentTime = currentTime;
            Wizard = wizard;
            SlaveMark = slaveMark;
            Canceled = canceled;
            Modifiers = modifiers;
        }

        public bool IsSlave => SlaveMark.IsSlave();
        public bool OnClient() => SlaveMark.OnClient();
        public bool OnServerMaster() => SlaveMark.OnServerMaster();
        public bool OnServerSlave() => SlaveMark.OnServerSlave();
        public bool OnClientWithAuthority() => OnClient() && Caster.HasClientAuthority(Repo);
        
        public string WhereAmI => SpellWordCastDataExtensions.WhereAmI(SlaveMark, Caster, Repo);

        public override string ToString()
        {
            return StringBuildersPool.Get
                .Append("[")
                .Append("Caster:").Append(Caster).Append(" ")
                .Append("CastData:").Append(CastData).Append(" ")
                .Append("CurrentTime:").Append(CurrentTime).Append(" ")
                .Append(Canceled ? "Canceled " : String.Empty)
                .Append("Wizard:").Append(Wizard.Guid).Append(" ")
                .Append(WhereAmI)
                .Append("Modifiers:").AppendExt(Modifiers)
                .Append("]")
                .ToStringAndReturn();                
        }
    }

    public class SpellWordCastData : SpellPredCastData
    {
        public readonly ScriptingContext Context;
        public readonly SpellId SpellId;
        public readonly long SpellStartTime; // время старта корневого спелла
        public readonly long ParentSubSpellStartTime; // время старта сабспела являющегося родителем сабспелла в котром находится слово 
        public readonly TimeRange WordTimeRange; // Время когда слово ДОЛЖНО БЫЛО быть запущено, и время когда оно ПРЕДПОЛОЖИТЕЛЬНО будет остановлено согласно SyncTime.Now
        public readonly int SubSpellCount; // Номер активации (для периодических сабспеллов)
        public readonly bool FirstOrLast;


        public SpellWordCastData(
            SpellId spellId,
            int subSpellCount,
            ISpellCast castData,
            long currentTime,
            long spellStartTime,
            long parentSubSpellStartTime,
            TimeRange wordTimeRange,
            OuterRef<IWizardEntity> wizard,
            OuterRef<IEntity> caster,
            IUnityEnvironmentMark slaveMark,
            IReadOnlyList<SpellModifierDef> modifiers,
            bool firstOrLast,
            bool canceled,
            ScriptingContext context,
            IEntitiesRepository repo
            )
        : base(castData, currentTime, wizard, caster, slaveMark, modifiers, canceled, repo)
        {
            SpellId = spellId;
            SpellStartTime = spellStartTime;
            ParentSubSpellStartTime = parentSubSpellStartTime;
            WordTimeRange = wordTimeRange;
            SubSpellCount = subSpellCount;
            FirstOrLast = firstOrLast;
            Context = context;
        }

        public bool IsSlave => SlaveMark.IsSlave();
        public bool OnClient() => SlaveMark.OnClient();
        public bool OnServerMaster() => SlaveMark.OnServerMaster();
        public bool OnServerSlave() => SlaveMark.OnServerSlave();
        public bool OnClientWithAuthority() => OnClient() && Caster.HasClientAuthority(Repo);
        public SpellPartCastId WordCastId(SpellWordDef word) => new SpellPartCastId(SpellId, SubSpellCount, word);
        public SpellPartGlobalCastId WordGlobalCastId(SpellWordDef word) => new SpellPartGlobalCastId(Wizard.Guid, SpellId, SubSpellCount, word);

        public string WhereAmI => SpellWordCastDataExtensions.WhereAmI(SlaveMark, Caster, Repo);

        public override string ToString()
        {
            return StringBuildersPool.Get
                .Append("[")
                .Append("Id:").Append(SpellId).Append("/").Append(SubSpellCount).Append(" ")
                .Append("Caster:").Append(Caster).Append(" ")
                .Append("CastData:").Append(CastData).Append(" ")
                .Append("CurrentTime:").Append(CurrentTime).Append(" ")
                .Append("SpellStartTime:").Append(SpellStartTime.TimeToString(CurrentTime)).Append(" ")
                .Append("ParentSubSpellStartTime:").Append(ParentSubSpellStartTime.TimeToString(SpellStartTime)).Append(" ")            
                .Append("WordTimeRange:").Append(WordTimeRange.ToString(SpellStartTime)).Append(" ")
                .Append(Canceled ? "Canceled " : String.Empty)
                .Append("Wizard:").Append(Wizard.Guid).Append(" ")
                .Append(WhereAmI)
                .Append("Modifiers:").AppendExt(Modifiers)
                .Append("]")
                .ToStringAndReturn();                
        }
    }
    
    public static class SpellWordCastDataExtensions
    {
        public static bool HasClientAuthority(this OuterRef data, IEntitiesRepository repo) => AuthorityOwner.CheckClientAuthority(repo, data.Guid);
        public static bool HasClientAuthority(this OuterRef<IEntity> data, IEntitiesRepository repo) => AuthorityOwner.CheckClientAuthority(repo, data.Guid);
        
        public static string WhereAmI(IUnityEnvironmentMark mark, OuterRef caster, IEntitiesRepository repo)
        {
            if (@mark.OnServerMaster()) return "Server Master";
            if (@mark.OnServerSlave()) return "Server Slave";
            if (@mark.OnClient() && caster.HasClientAuthority(repo)) return "Client with Auth";
            if (@mark.OnClient()) return "Client";
            return "Unknown";
        }
    }

//    public static class ProvisionalSpellInterfaceProvider
//    {
//        public static IProvisionalSpellInterface Interface;
//    }
//
//    public interface IProvisionalSpellInterface
//    {
//        Task<bool> CastImpact(SpellWordCastData data, object impact, SpellWordDef def);
//        Task<bool> CastEffectStart(SpellWordCastData data, object effect, SpellWordDef def);
//        Task<bool> CastEffectEnd(SpellWordCastData data, object effect, SpellWordDef def);
//        Task<bool> CheckOldPredicate(SpellWordCastData data, object predicate, SpellWordDef def);
//        object GetOldWordForDef(SpellWordDef def);
//    }
}
