using Assets.ResourceSystem.Account;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using NLog;
using ResourcesSystem.Loader;

namespace Assets.ColonyShared.SharedCode.Utils
{
    public static class GlobalConstsHolder
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("GlobalConstsHolder");

        private static ResourceIDFull _globalConstsDefPath = new ResourceIDFull("/UtilPrefabs/Res/Prototypes/GlobalConstsDef");
        private static ResourceIDFull _statResourcesPath   = new ResourceIDFull("/UtilPrefabs/Res/Prototypes/ConstsStatResources");
        private static ResourceIDFull _cheatsDataPath      = new ResourceIDFull("/UtilPrefabs/Res/Prototypes/CheatsDataDef");

        private static readonly ResourceRef<GlobalConstsDef>  _GlobalConstsDef = new ResourceRef<GlobalConstsDef>(_globalConstsDefPath);
        private static readonly ResourceRef<StatResourcesDef> _StatResources   = new ResourceRef<StatResourcesDef>(_statResourcesPath);
        private static readonly ResourceRef<CheatsDataDef>    _CheatsData      = new ResourceRef<CheatsDataDef>(_cheatsDataPath);
        
        
        public static GlobalConstsDef GlobalConstsDef => _GlobalConstsDef.Target;
        public static StatResourcesDef StatResources => _StatResources.Target;
        public static CheatsDataDef CheatsData => _CheatsData.Target;

        public static float DeadlyDamage = 9999999f;
        
        // it's only for debugging while development - ture at 'develop' is unexpected
        public static bool Dbg_Mobs7910 = false;
        public const bool Dbg_PZ_13761_DebuggingEnabled = false;


    // --- Verification: ---------------------
    #region Verification

        [VerifyAfterCompilation]
        ///#TODO: Плохо! - срабатывает в совершенно рнд мом-т (неконтролируемо) и в PlayMode срабатывает, но не во всех других режимах.
        /// Нужно либо перевести на PostImport** или лучше перевести на полноценный общий framework проверки jdb. Дожно быть релеватно только jdb и проверке (можно прям отдеьлным exe-шником)
        /// **) - LoadResourceHolder.RegisterObject - воткнуться под конец и проверить json, если тип наследник i-face'а.
        public static bool Verify()
        {
            if (DbgLog.Enabled) DbgLog.Log("Verify call");

            // Не получается проверить во время вызова аннотированных `VerifyAfterCompilation` методов, т.к. ресурсы ещё не загружены.
            // .. Поэтому тут вынужден вернуть true; Факт-ки проверка вызовется по callback'у после загрузки ресурсов, на который тут подписались.
            GameResources.OnAllResourcesAreLoaded += VerifyDo;
            return true;
        }
        public static /*bool*/void VerifyDo()
        {
            if (DbgLog.Enabled) DbgLog.Log("VerifyDo call");
            LevelUpDatasDef.Verify(GlobalConstsDef.LevelUpDatas);
        }


    #endregion Verification
    }
}
