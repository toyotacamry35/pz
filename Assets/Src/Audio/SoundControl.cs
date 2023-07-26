using Assets.Src.Lib.Cheats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using L10n;
using NLog;
using UnityEditor;
using UnityEngine;
using Event = AK.Wwise.Event;
using Assets.Tools.EditorCamera;
using ReactivePropsNs;
using Core.Cheats;
using SharedCode.Logging;
using Core.Environment.Logging.Extension;

namespace Uins.Sound
{
    public class SoundControl : MonoBehaviour
    {
        //DEBUG
        private static readonly bool reportResults = false;
        private static readonly bool reportStatEvents = false;

        public static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly WaitForSeconds Delay = new WaitForSeconds(5);
        private static readonly Vector2 VolumeBounds = new Vector2(0, 100);
        public static readonly float MaxVolume = VolumeBounds.y;
        public const float DefaultVolume = 100;

        private static readonly float MaxStat = 100;

        private const string MuteID = "mute";
        private const string MasterVolumeID = "Master_volume";
        private const string SFXVolumeID = "SFX_volume";
        private const string MusicVolumeID = "Music_volume";

        public const string HealthCurrentStatID = "HealthCurrent_stat";
        public const string StaminaCurrentStatID = "StaminaCurrent_stat";
        public const string SatietyCurrentStatID = "SatietyCurrent_stat";
        public const string WaterBalanceCurrentStatID = "WaterBalanceCurrent_stat";
        public const string IntoxicationStatID = "Intoxication_stat";
        public const string OverheatStatID = "Overheat_stat";
        public const string HypothermiaStatID = "Hypothermia_stat";
        public const string EnvTemperatureStatID = "EnvTemperature_stat";
        public const string EnvToxicStatID = "EnvToxic_stat";
        public const string EnvWindStatID = "EnvWind_stat";

        public const int MaxDollSlotIndexForSound = 13;

        private static readonly float[] HealthThresholds = new float[] {30};
        private static readonly float[] StaminaThresholds = new float[] {99};


        private static string[] IngameSoundBanks = {"Character", "Music", "SFX_UI"};
        private bool IngameBanksLoaded = false;

        [SerializeField, UsedImplicitly]
        private string[] _wwiseLocalizedBankNames = {"localized"};

        [Space]
        [Header("Music")]
        [Space]
        public Event AmbientMusicEvent = null;

        public Event MainThemeMusicEvent = null;
        public Event DropZoneMusicEvent = null;
        public Event PlayerDeathEvent = null;
        public Event PlayerResurrectEvent = null;

        [Space]
        [Header("State Events")]
        [Space]
        public Event StaminaLow = null;

        public Event HealthLow = null;
        public Event HealthRestore = null;

        [Space]
        [Header("Heard by player")]
        [Space]
        public Event ClothPutOnEvent = null;

        public Event ClothTakeOffEvent = null;

        [Space]
        public Event InventoryOpenEvent = null;

        public Event InventoryCloseEvent = null;
        public Event ButtonBig = null;
        public Event ButtonSmall = null;
        public Event BuildingUpgradeInventoryOpenEvent = null;
        public Event BuildingUpgradeInventoryCloseEvent = null;
        public Event MachineInventoryOpenEvent = null; //для автостанка
        public Event MachineInventoryCloseEvent = null; //для автостанка
        public Event BenchInventoryOpenEvent = null;
        public Event BenchInventoryCloseEvent = null;
        public Event ChestInventoryOpenEvent = null;
        public Event ChestInventoryCloseEvent = null;
        public Event OpenedContainerInventoryOpenEvent = null;
        public Event ContainerOpenEvent = null;
        public Event ContainerCloseEvent = null;
        public Event ItemDrop = null;

        [Space]
        public Event ItemPickupEvent = null;

        public Event KnowledgeBaseNewNote = null;
        public Event TraumaNotification = null;

        [Space]
        public Event CraftTaskStartEvent = null;

        public Event CraftTaskAddEvent = null;
        public Event CraftTaskCancelEvent = null;
        public Event CraftTaskSuccessEndEvent = null;
        public Event CraftQueueCompleteEvent = null;
        public Event CraftQueuePauseEvent = null;
        public Event HandcraftTaskStartEvent = null;
        public Event CraftOverloadCancelEvent = null;
        public Event CraftBuildingPlacementDeniedEvent = null;

        [Space]
        public Event PerkObtain = null;

        public Event PerkParse = null;
        public Event PerkSlotHover = null;
        public Event PerkToPermanent = null;
        public Event PerkToSaved = null;
        public Event PerkUpgradeSlot = null;

        [Space]
        public Event TechHover = null;

        public Event TechActivate = null;

        [Space]
        public Event QuestObtain = null;

        public Event QuestProgress = null;
        public Event QuestComplete = null;

        [Space]
        public Event NewRegion = null;

        [Space]
        [Header("Heard by world")]
        [Space]
        public Event BenchQueueStartEvent = null;

        public Event BenchQueueCompleteEvent = null;
        public Event BenchQueueCancelEvent = null;

        [Space]
        public Event CraftBuildingPlacementEvent = null;

        public Event CraftBuildingBreakdownEvent = null;

        [Space]
        public Event AmbienceEvent = null;

        [Space]
        public Event LocalizedEvent = null; //DEBUG

        public static SoundControl Instance { get; private set; }

        private string daytime = null;

        private float previousHealthStat = -1.0f;

        private Event activeHealthEvent = null;

        private float previousStaminaStat = -1.0f;

        private Event activeStaminaEvent = null;

        private Event[] healthEvents = null;

        private Event[] staminaEvents = null;

        private GameObject mainCameraGameObject = null;

        private AkGameObj pawnAkGameObj = null;

        private Event activeMusicEvent = null;

        private Event activeEmbienceEvent = null;

        private bool _isAmbPlaying = false;

        private DisposableComposite D = new DisposableComposite();


        //=== Props ===========================================================

        public Dictionary<SoundEvents, Event> EnumEvents { get; set; } = null;

        public ReactiveProperty<bool> IsMutedRp { get; } = new ReactiveProperty<bool>() {Value = false};

        public ReactiveProperty<float> MasterVolumeRp { get; } = new ReactiveProperty<float>() {Value = MaxVolume};

        public ReactiveProperty<float> SfxVolumeRp { get; } = new ReactiveProperty<float>() {Value = MaxVolume};

        public ReactiveProperty<float> MusicVolumeRp { get; } = new ReactiveProperty<float>() {Value = MaxVolume};

        private bool SavedIsMuted
        {
            get => UniquePlayerPrefs.GetBool(MuteID, false);
            set => UniquePlayerPrefs.SetBool(MuteID, value);
        }

        public static float SavedMasterVolume
        {
            get => UniquePlayerPrefs.GetFloat(MasterVolumeID, DefaultVolume);
            set => UniquePlayerPrefs.SetFloat(MasterVolumeID, value);
        }

        private float SavedSfxVolume
        {
            get => UniquePlayerPrefs.GetFloat(SFXVolumeID, DefaultVolume);
            set => UniquePlayerPrefs.SetFloat(SFXVolumeID, value);
        }

        private float SavedMusicVolume
        {
            get => UniquePlayerPrefs.GetFloat(MusicVolumeID, DefaultVolume);
            set => UniquePlayerPrefs.SetFloat(MusicVolumeID, value);
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"<{GetType()}> Is excess Instance: '{transform.FullName()}'\nalready exists: '{Instance.transform.FullName()}'", gameObject);
                return;
            }

            Log.StartupStopwatch.Milestone("SoundControl Awake start");

            Instance = this;

            MainCamera.OnCameraCreated += MainCameraNotifier_OnCameraCreated;
            MainCamera.OnCameraDestroyed += MainCameraNotifier_OnCameraDestroyed;

            //КОСТЫЛЬ загружаем все банки сразу 
            LoadIngameBanks();

            //Единственный раз читаем сохраненные значения
            IsMutedRp.Value = SavedIsMuted;
            MasterVolumeRp.Value = SavedMasterVolume;
            SfxVolumeRp.Value = SavedSfxVolume;
            MusicVolumeRp.Value = SavedMusicVolume;

            //Связываем изменения значений Rp с 1) изменением громкости в движке, 2) сохранением в UniquePlayerPrefs (реестре)
            MasterVolumeRp.Action(D, vol =>
            {
                SavedMasterVolume = vol;
                SetSoundEngineVolume(MasterVolumeID, vol);
            });
            SfxVolumeRp.Action(D, vol =>
            {
                SavedSfxVolume = vol;
                SetSoundEngineVolume(SFXVolumeID, vol);
            });
            MusicVolumeRp.Action(D, vol =>
            {
                SavedMusicVolume = vol;
                SetSoundEngineVolume(MusicVolumeID, vol);
            });
            IsMutedRp.Action(D, b =>
            {
                SavedIsMuted = b;
                SetSoundEngineMute(b);
            });

            FillStatEvents();
            this.StartInstrumentedCoroutine(SetDaytimeCoroutine());

#if UNITY_EDITOR
            //фикс для EditorCamera
            MainCamera.InvokeCameraCreatedIfNeeded();
            var editorCamera = Assets.Src.Camera.EditorCamera.Camera;
            if (editorCamera != null)
            {
                LoadIngameBanks();
                PlayAmbienceEvent(this.AmbienceEvent, true);
            }
#endif
            Log.StartupStopwatch.Milestone("SoundControl Awake DONE!");
        }

        private void Start()
        {
            L10nHolder.Instance.LocalizationChanged += OnLocalizationChanged;
            LocalizationInit(L10nHolder.Instance.CurrentLocalization.CultureData.Code);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (EditorUtility.audioMasterMute != IsMutedRp.Value)
                IsMutedRp.Value = EditorUtility.audioMasterMute;
        }
#endif
        private void OnDestroy()
        {
            if (Instance != this)
                return;

            Instance = null;

            MainCamera.OnCameraCreated -= MainCameraNotifier_OnCameraCreated;
            MainCamera.OnCameraDestroyed -= MainCameraNotifier_OnCameraDestroyed;

            StopCoroutine(SetDaytimeCoroutine());

            D.Clear(); //Убирает все связи с ReactiveProp<>

            AkSoundEngine.StopAll(gameObject);

            activeHealthEvent = null;
            activeStaminaEvent = null;
            activeMusicEvent = null;
            activeEmbienceEvent = null;

            if (L10nHolder.Instance != null)
                L10nHolder.Instance.LocalizationChanged -= OnLocalizationChanged;
        }


        //=== Public ==============================================================


        //--- Console commands

        [Cheat]
        public static void Mute()
        {
            if (!Instance.AssertIfNull(nameof(Instance)))
                Instance.IsMutedRp.Value = true;
        }

        [Cheat]
        public static void Unmute()
        {
            if (!Instance.AssertIfNull(nameof(Instance)))
                Instance.IsMutedRp.Value = false;
        }

        [Cheat]
        public static void MasterVolume(float volume = -1)
        {
            if (!Instance.AssertIfNull(nameof(Instance)))
                Instance.GetOrSetVolumeFromConsole(Instance.MasterVolumeRp, volume, MasterVolumeID);
        }

        [Cheat]
        public static void SfxVolume(float volume = -1)
        {
            if (!Instance.AssertIfNull(nameof(Instance)))
                Instance.GetOrSetVolumeFromConsole(Instance.SfxVolumeRp, volume, SFXVolumeID);
        }

        [Cheat]
        public static void MusicVolume(float volume = -1)
        {
            if (!Instance.AssertIfNull(nameof(Instance)))
                Instance.GetOrSetVolumeFromConsole(Instance.MusicVolumeRp, volume, MusicVolumeID);
        }

        public static void UpdateStat(string stat, float value)
        {
            if (Instance != null)
            {
                AKRESULT result_set = AKRESULT.AK_IDNotFound;
                string statID = string.Empty;
                switch (stat)
                {
                    case HealthCurrentStatID:
                        value = ClampVolume(value * MaxStat);
                        Instance.CheckForHealthCurrentStatSpecialEvents(value);
                        statID = HealthCurrentStatID;
                        break;
                    case StaminaCurrentStatID:
                        value = ClampVolume(value * MaxStat);
                        Instance.CheckForStaminaCurrentStatSpecialEvents(value);
                        statID = StaminaCurrentStatID;
                        break;
                    case SatietyCurrentStatID:
                        value = ClampVolume(value * MaxStat);
                        statID = SatietyCurrentStatID;
                        break;
                    case WaterBalanceCurrentStatID:
                        value = ClampVolume(value * MaxStat);
                        statID = WaterBalanceCurrentStatID;
                        break;
                    case IntoxicationStatID:
                        value = ClampVolume(value * MaxStat);
                        statID = IntoxicationStatID;
                        break;
                    case OverheatStatID:
                        value = ClampVolume(value * MaxStat);
                        statID = OverheatStatID;
                        break;
                    case HypothermiaStatID:
                        value = ClampVolume(value * MaxStat);
                        statID = HypothermiaStatID;
                        break;
                    case EnvTemperatureStatID:
                        statID = EnvTemperatureStatID;
                        break;
                    case EnvToxicStatID:
                        statID = EnvToxicStatID;
                        break;
                    case EnvWindStatID:
                        //statID = EnvWindStatID;
                        //KOSTYL!!!!
                        statID = null;
                        break;
                }

                if (!string.IsNullOrEmpty(statID))
                {
                    result_set = AkSoundEngine.SetRTPCValue(statID, value);
                    if (reportResults || (result_set != AKRESULT.AK_Success))
                    {
                        Debug.LogWarning($"SetRTPCValue({statID}, {value})\t{result_set}");
                    }
                }
            }
        }

        //--- Ambience

        public void OnResurrect(AkGameObj pawnGameObj)
        {
            pawnAkGameObj = pawnGameObj;
            PlayAmbience();
            PlayerResurrectEvent.Post(pawnGameObj.gameObject);
        }

        public void OnDeath()
        {
            activeStaminaEvent?.Stop(pawnAkGameObj.gameObject);
            activeStaminaEvent = null;
            
            AkSoundEngine.StopAll(pawnAkGameObj.gameObject);
            PlayerDeathEvent.Post(pawnAkGameObj.gameObject);
            pawnAkGameObj = null;
        }

        public void OnTeleport()
        {
            if (pawnAkGameObj != null)
                activeStaminaEvent?.Stop(pawnAkGameObj.gameObject);
            activeStaminaEvent = null;
           
            if (pawnAkGameObj != null)
                AkSoundEngine.StopAll(pawnAkGameObj.gameObject);
            //pawnAkGameObj = null;
        }

        public void PlayMusicEvent(Event musicEvent, bool force)
        {
            if (force || (activeMusicEvent != musicEvent))
            {
                activeMusicEvent?.Stop(gameObject);
                activeMusicEvent = musicEvent;
                if (musicEvent != null)
                {
                    if (musicEvent == MainThemeMusicEvent)
                        AkSoundEngine.StopAll(gameObject);
                    activeMusicEvent?.Post(gameObject);
                }
            }
        }

        public void PlayAmbienceEvent(Event ambienceEvent, bool force)
        {
            if (pawnAkGameObj != null && pawnAkGameObj.gameObject != null) 
                activeEmbienceEvent?.Stop(pawnAkGameObj.gameObject);

            activeEmbienceEvent = ambienceEvent;

            if (activeEmbienceEvent != null && pawnAkGameObj != null && pawnAkGameObj.gameObject != null)
            {
                activeEmbienceEvent.Post(pawnAkGameObj.gameObject);
            }
            
        }


        //=== Private =============================================================================

        private void GetOrSetVolumeFromConsole(ReactiveProperty<float> reactiveProperty, float volume, string volumeName)
        {
            if (volume >= 0)
                reactiveProperty.Value = ClampVolume(volume);

            Logger.IfInfo()?.Message($"{volumeName}: {reactiveProperty.Value:f2}{(IsMutedRp.Value ? " MUTED" : "")}").Write();
        }

        private void SetSoundEngineVolume(string volumeId, float volume)
        {
            var result = AkSoundEngine.SetRTPCValue(volumeId, volume);
            if (reportResults || result != AKRESULT.AK_Success)
                Debug.LogWarning($"{nameof(AkSoundEngine.SetRTPCValue)}('{volumeId}', {volume}) => result={result}");
        }

        private void SetSoundEngineMute(bool isMuted)
        {
#if UNITY_EDITOR
            EditorUtility.audioMasterMute = isMuted;
#endif
            var isMutedString = isMuted.ToString().ToLower();
            AkSoundEngine.SetState(MuteID, isMutedString);
        }

        private static float ClampVolume(float volume)
        {
            return Math.Max(VolumeBounds.x, Math.Min(VolumeBounds.y, volume));
        }

        private void LoadIngameBanks()
        {
            if(!IngameBanksLoaded)
            {
                foreach (string bankName in IngameSoundBanks)
                {
                    uint bankID = AkSoundEngine.GetIDFromString(bankName);

                    uint bankId;
                    try

                    {
                        AkSoundEngine.LoadBank(bankName, AkSoundEngine.AK_DEFAULT_POOL_ID, out bankId);
                    }
                    catch (Exception e)
                    {
                        UI.LoggerDefault.Error($"{e.Message}\n{e.StackTrace}");
                    }
                }

                IngameBanksLoaded = true;
            }

        }

        private void UnloadIngameBanks()
        {
            foreach (string bankName in IngameSoundBanks)
            {
                uint bankID = AkSoundEngine.GetIDFromString(bankName);
                try
                {
                    AkSoundEngine.UnloadBank(bankName, IntPtr.Zero);
                }
                catch (Exception e)
                {
                    UI.LoggerDefault.Error($"{e.Message}\n{e.StackTrace}");
                }
            }

            IngameBanksLoaded = false;
        }

        //--- Localization

        private void LocalizationInit(string localizationCode)
        {
            // ChangeSoundEngineLanguage(localizationName);
            foreach (var bankName in _wwiseLocalizedBankNames)
                LoadLocalizedBank(bankName);
        }

        private void OnLocalizationChanged(string localizationName)
        {
            // ChangeSoundEngineLanguage(localizationName);
            foreach (var bankName in _wwiseLocalizedBankNames)
            {
                UnloadLocalizedBank(bankName);
                LoadLocalizedBank(bankName);
            }
        }

        private void ChangeSoundEngineLanguage(string localizationName)
        {
            localizationName = localizationName.Substring(0, 2); //TODOM Сменить имена локализации на  en-US, ru-RU
            UI.LoggerDefault.IfDebug()?.Message($"Change Wwise localization: '{localizationName}'").Write();
            AkSoundEngine.SetCurrentLanguage(localizationName);
        }

        private void LoadLocalizedBank(string bankName)
        {
            UI.LoggerDefault.IfDebug()?.Message($"Load localized bank '{bankName}'...").Write();
            var start = DateTime.Now;
            uint bankId;
            try
            {
                AkSoundEngine.LoadBank(bankName, AkSoundEngine.AK_DEFAULT_POOL_ID, out bankId);
            }
            catch (Exception e)
            {
                UI.LoggerDefault.Error($"{e.Message}\n{e.StackTrace}");
            }

            UI.LoggerDefault.IfDebug()?.Message($"Done: {(DateTime.Now - start).TotalSeconds:f1}s").Write();
        }

        private void UnloadLocalizedBank(string bankName)
        {
            UI.LoggerDefault.IfDebug()?.Message($"Unload localized bank '{bankName}'...").Write();
            var start = DateTime.Now;
            try
            {
                AkSoundEngine.UnloadBank(bankName, IntPtr.Zero);
            }
            catch (Exception e)
            {
                UI.LoggerDefault.IfError()?.Message($"{e.Message}\n{e.StackTrace}").Write();
            }

            UI.LoggerDefault.IfDebug()?.Message($"Done: {(DateTime.Now - start).TotalSeconds:f1}s").Write();
        }

        //--- Stats

        private void FillStatEvents()
        {
            healthEvents = new Event[] {HealthLow, HealthRestore};
            staminaEvents = new Event[] {StaminaLow, null};
        }

        private void CheckStatEvents(ref float holderValue, float currentValue, float[] thresholds, Event[] events, ref Event activeEvent,
            string statID)
        {
            if ((thresholds == null) || (events == null) || (pawnAkGameObj == null) || (thresholds.Length == 0) || (events.Length == 0) ||
                (events.Length != (thresholds.Length + 1)))
            {
                return;
            }

            // переход от большего к меньшему
            for (int index = 0; index < thresholds.Length; ++index)
            {
                float threshold = thresholds[index];
                if (((holderValue < 0.0f) || (holderValue > threshold)) && (currentValue <= threshold))
                {
                    if (reportStatEvents)
                    {
                        Debug.LogWarning($"StatEvent.Post\t{statID}\tmore: {holderValue} -> less: {currentValue}\t{index}");
                    }

                    //отключаем евент если не совпал с текущим
                    if ((activeEvent != null) && (activeEvent != events[index]))
                    {
                        //activeEvent.Stop(_gameObject, (int)3, AkCurveInterpolation.AkCurveInterpolation_Linear);
                        activeEvent.ExecuteAction(pawnAkGameObj.gameObject, AkActionOnEventType.AkActionOnEventType_Break, 0,
                            AkCurveInterpolation.AkCurveInterpolation_Linear);
                    }

                    activeEvent = events[index];
                    activeEvent?.Post(pawnAkGameObj.gameObject);
                    holderValue = currentValue;
                    return;
                }
            }

            // переход от меньшего к большему
            for (int index = (thresholds.Length - 1); (index >= 0); --index)
            {
                float threshold = thresholds[index];
                if (((holderValue < 0.0f) || (holderValue <= threshold)) && (currentValue > threshold))
                {
                    if (reportStatEvents)
                    {
                        Debug.LogWarning($"StatEvent.Post\t{statID}\tless: {holderValue} -> more: {currentValue}\t{index + 1}");
                    }

                    //отключаем евент если не совпал с текущим
                    if ((activeEvent != null) && (activeEvent != events[index + 1]))
                    {
                        //activeEvent.Stop(_gameObject, (int)1, AkCurveInterpolation.AkCurveInterpolation_Linear);
                        activeEvent.ExecuteAction(pawnAkGameObj.gameObject, AkActionOnEventType.AkActionOnEventType_Break, 0,
                            AkCurveInterpolation.AkCurveInterpolation_Linear);
                    }

                    activeEvent = events[index + 1];
                    activeEvent?.Post(pawnAkGameObj.gameObject);
                    holderValue = currentValue;
                    return;
                }
            }

            holderValue = currentValue;
        }

        private void CheckForHealthCurrentStatSpecialEvents(float value)
        {
            CheckStatEvents(ref previousHealthStat, value, HealthThresholds, healthEvents, ref activeHealthEvent, HealthCurrentStatID);
        }

        private void CheckForStaminaCurrentStatSpecialEvents(float value)
        {
            CheckStatEvents(ref previousStaminaStat, value, StaminaThresholds, staminaEvents, ref activeStaminaEvent, StaminaCurrentStatID);
        }

        //--- Ambience

        private void MainCameraNotifier_OnCameraCreated(Camera camera)
        {
            mainCameraGameObject = (camera == null) ? null : camera.gameObject;
            //suitable for EditorCamera
            if (camera.gameObject.GetComponentInParent<EditorCamera>() != null)
            {
                pawnAkGameObj = this.GetComponent<AkGameObj>();
                PlayAmbience();
            }
        }

        //suitable for EditorCamera
        private void MainCameraNotifier_OnCameraDestroyed()
        {
            //костыль - если AmbienceEvent нулевой, значит мы в лобби и у нас пошел экран загрузки. выгружать банки при этом не нужно
            if (activeEmbienceEvent != null)
            {
                StopAmbience();
                UnloadIngameBanks();
            }
        }


        private void PlayAmbience()
        {
            if (!IngameBanksLoaded)
                LoadIngameBanks();
            PlayMusicEvent(AmbientMusicEvent, false);
            PlayAmbienceEvent(AmbienceEvent, false);
        }

        private void StopAmbience()
        {
            PlayMusicEvent(null, false);
            PlayAmbienceEvent(null, false);
        }

        private IEnumerator SetDaytimeCoroutine()
        {
            while (true)
            {
                if (TOD.ASkyLighting._instance != null)
                {
                    string newdaytime = TOD.Utils.GetCurrentDayTime().ToString();
                    if (newdaytime != daytime)
                    {
                        AkSoundEngine.PostEvent(newdaytime, gameObject);
                        daytime = newdaytime;
                    }
                }

                yield return Delay;
            }
        }
    }
}