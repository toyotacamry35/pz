using System;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using Newtonsoft.Json;
using L10n;

namespace SharedCode.Wizardry
{
    [Localized]
    public class SubSpell : BaseResource
    {
        public bool SyncWithMaster { get; set; } = false;
        //offset in milliseconds from start from _parent_ (not root!) 
        public float OffsetStart { get; set; } = 0;
        //start anchor relative to parent start
        public float RelativeOffsetStart { get; [UsedImplicitly] set; }
        [Obsolete("Use da RelativeOffsetStart")] public float AnchorLeft { get => RelativeOffsetStart; set => RelativeOffsetStart = value; }

//        //offset from right anchor (invalid if parent spell is indefinite)
//        public float OffsetEnd { get; set; } = 0;
//        //right anchor relative to parent start (invalid if parent spell is indefinite, will always be 0 then)
//        public float AnchorRight { get; set; } = 0;
//        //how many periods?
//        public int CountPerDuration { get; set; }

        // Сабспелл периодически повторяется через каждые Duration+PeriodDelay сек.
        public bool Periodic { get; set; }
        // Задержка перед повтором (только если Periodic)
        public float PeriodDelay { get; set; }
        //if this subspell fails root spell should be terminated
        public bool OffsetIsFromParentEnd { get; set; } = false;
        public bool MustNotFail { get; set; } = true;
        public bool OverrideDuration { get; set; } = false;
        public float OverridenDuration { get; set; } = 1;//say each subspell by default is 1 sec
        public bool OverrideDurationPercent { get; set; } = false;
        public float OverridenDurationPercent { get; set; } = 1;//say each subspell by default is 1 sec
        [JsonIgnore]
        public bool HasOverridenDuration => OverrideDuration | OverrideDurationPercent;
        [JsonProperty(Required = Required.Always)] public ResourceRef<SpellDef> Spell { get; set; }
    }
}
