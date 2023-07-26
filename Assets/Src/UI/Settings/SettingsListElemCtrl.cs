using System;
using System.Collections.Generic;
using System.Linq;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using NLog;
using ReactivePropsNs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityWeld.Binding;

namespace Uins.Settings
{
    /// <summary>
    /// Эта пара Ctrl и VM классов лишь обёртка-посредник. Нужна только для того, чтобы можно было сложить разнотипные хэндлеры (Int, Bool, Float, ..) в `BindingControllersPool`.
    /// </summary>
    // #note: Expected lifetime - unlimited ('cos of pool)
    [Binding]
    internal class SettingsListElemCtrl : BindingController<SettingsListElemVM>, IApplyableCancelableDefaultableCanBeChanged, IPointerEnterHandler, IPointerExitHandler
    {
        //#Dbg:
        //static int DBG_counter_static;
        //internal int DBG_counter;

        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [SerializeField, UsedImplicitly] private List<SettingHandlerTypeToPrefab> _settingSwitchersPrefabsMapSerialized = new List<SettingHandlerTypeToPrefab>();
        private Dictionary<SettingHandlerType, GameObject> _settingSwitchersPrefabsMap;
        [SerializeField, UsedImplicitly] private Transform _rootForHandler;
        [SerializeField, UsedImplicitly] private SettingHandlerTypeToPrefab _test;
        [SerializeField, UsedImplicitly] private GameObject _highlightedIndicator;

        [UsedImplicitly] //is set via reflection by Weld inside .Bind
        [Binding] public LocalizedString Name { get; protected set; }

        private ISettingSwitcherCtrlBase _ctrl;

        [UsedImplicitly]
        private void Awake()
        {
            //var prev = DBG_counter;
            //DBG_counter = DBG_counter_static++;
            //if (DbgLog.Enabled) Debug.Log($"Awk: {prev} --> <{DBG_counter}>");

            System.Diagnostics.Debug.Assert(_settingSwitchersPrefabsMapSerialized.Count > 0);
            _highlightedIndicator.AssertIfNull(nameof(_highlightedIndicator));

            try
            {
                _settingSwitchersPrefabsMap = _settingSwitchersPrefabsMapSerialized.ToDictionary(x => x.HandlerType, x => x.Prefab);
            }
            catch (Exception e)
            {
                Logger.Error($"Ill-formed `{nameof(SettingsListElemCtrl)}.{nameof(_settingSwitchersPrefabsMapSerialized)}` - check prefab! Possibly settings window 'll not work, until it fixed." +
                             $"\n Got exception: \"{e}\"");
                _settingSwitchersPrefabsMap = new Dictionary<SettingHandlerType, GameObject>();
            }

            var definitionStream = Vmodel.SubStream(D, vm => vm.Definition);
            definitionStream.Action(D, InstantiateHandler);
            var nameStream = Vmodel.SubStream(D, vm => vm.Name);
            Bind(nameStream, () => Name);
        }

        public bool WasChanged => _ctrl?.WasChanged ?? false;

        GameObject _handler;
        private void InstantiateHandler(SettingHandlerDefinition definition)
        {
            //if (DbgLog.Enabled) DbgLog.Log($"SeLiEl_Ctrl  <{DBG_counter}>  .InstantiateHandler({definition}).");

            if (!definition.IsValid)
            {
                Logger.IfDebug()?.Message("Should handle dflt definition").Write();

                if (_handler != null)
                    Destroy(_handler);

                return;
            }

            if (!_settingSwitchersPrefabsMap.TryGetValue(definition.HandlerType, out var typeAndPrefab))
            {
                Logger.IfError()?.Message($"!_settingSwitchersPrefabsMap.TryGetValue by definition.HandlerType({definition.HandlerType})").Write();
                return;
            }

            if (_handler != null)
                Destroy(_handler);

            _handler = Instantiate(typeAndPrefab, _rootForHandler);

            switch (definition.HandlerType)
            {
                case SettingHandlerType.Bool:
                    var definitionB = (SettingSwitcherBoolVM.Definition)definition.Definition;
                    var ctrlB = _handler.GetComponent<SettingSwitcherBoolCtrl>();
                    var vmB = new SettingSwitcherBoolVM(definitionB, definitionB.AppliedInstantly
                                                                        ? (IApplyableCancelableProxy<bool>)new AppliedInstantlyCancelByCommandProxy<bool>()
                                                                        : (IApplyableCancelableProxy<bool>)new AppliedByCommandProxy<bool>());
                    ctrlB.SetVmodel(vmB);
                    _ctrl = ctrlB;
                    break;

                case SettingHandlerType.Int:
                    var definitionI = (SettingSwitcherIntVM.Definition)definition.Definition;
                    var ctrlI = _handler.GetComponent<SettingSwitcherIntCtrl>();
                    var vmI = new SettingSwitcherIntVM(definitionI, definitionI.AppliedInstantly
                                                                        ? (IApplyableCancelableProxy<int>)new AppliedInstantlyCancelByCommandProxy<int>()
                                                                        : (IApplyableCancelableProxy<int>)new AppliedByCommandProxy<int>());
                    ctrlI.SetVmodel(vmI);
                    _ctrl = ctrlI;   
                    break;

                case SettingHandlerType.Float:
                    var definitionF = (SettingSwitcherFloatVM.Definition)definition.Definition;
                    var ctrlF = _handler.GetComponent<SettingSwitcherFloatCtrl>();
                    var vmF = new SettingSwitcherFloatVM(definitionF, definitionF.AppliedInstantly 
                                                                        ? (IApplyableCancelableProxy<float>) new AppliedInstantlyCancelByCommandProxy<float>() 
                                                                        : (IApplyableCancelableProxy<float>) new AppliedByCommandProxy<float>());
                    ctrlF.SetVmodel(vmF);
                    _ctrl = ctrlF;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(definition.HandlerType.ToString());
            }

        }


        // --- IApplyableCancelable: ---------------------------------
        public bool Apply() => _ctrl.Apply();
        public bool Cancel() => _ctrl.Cancel();
        public bool SetToDefault() => _ctrl.SetToDefault();

        protected override void OnDestroy()
        {
            //if (DbgLog.Enabled) DbgLog.Log($"SeLiEl_Ctrl  <{DBG_counter}>  .DESTROY.");

            if (_handler != null)
                Destroy(_handler); 
            
            base.OnDestroy();
        }

        // --- IPointerEnterHandler, IPointerExitHandler: -----------------
        public void OnPointerEnter(PointerEventData eventData)
        {
            _highlightedIndicator.SetActive(true);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _highlightedIndicator.SetActive(false);
        }


        // --- Util types: ---------------------------
        [Serializable]
        public struct SettingHandlerTypeToPrefab
        {
            public SettingHandlerType HandlerType;
            public GameObject Prefab;
        }

    }
}

