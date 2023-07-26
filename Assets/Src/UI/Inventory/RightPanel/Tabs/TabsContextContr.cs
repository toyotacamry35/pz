using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;

namespace Uins.Inventory
{
    public class TabsContextContr : BindingController<ContextViewWithParamsVmodel>
    {
        [SerializeField, UsedImplicitly]
        private InventoryTabContr[] _tabs;

        [SerializeField, UsedImplicitly]
        private InventoryContextMenuViewModel _inventoryContextMenuViewModel;

        private readonly ReactiveProperty<InventoryTabType> _currentTabStream = new ReactiveProperty<InventoryTabType>();


        //=== Props ===========================================================

        public InventoryTabContr CurrentTabContr { get; private set; }

        public IStream<InventoryTabType> CurrentTabStream => _currentTabStream;

        
        //=== Unity ===========================================================

        void Awake()
        {
            _tabs.IsNullOrEmptyOrHasNullElements(nameof(_tabs));
            _inventoryContextMenuViewModel.AssertIfNull(nameof(_inventoryContextMenuViewModel));

            Vmodel.Action(D, vm =>
            {
                if ((vm?.TabVmodels.Count ?? 0) > 0)
                {
                    _tabs.ForEach(tabContr =>
                    {
                        if (!vm.TabVmodels.TryGetValue(tabContr.TabType, out var tabVm))
                        {
                            UI.Logger.IfError()?.Message($"Unable to find Vmodel by type {tabContr.TabType}").Write();
                        }
                        else
                        {
                            tabContr.SetVmodel(tabVm);
                        }
                    });
                }
            });

            var currentInventoryTabVmStream = Vmodel
                .SubStream(D, vm => vm.CurrentTabRp);
            //закрывать открытое контекстное меню при любой смене таба
            currentInventoryTabVmStream.Action(D, vm => _inventoryContextMenuViewModel?.CloseContextMenuRequest());

            var currentTabContrStream = currentInventoryTabVmStream
                .Func(D, tabVmodel => tabVmodel != null ? _tabs.FirstOrDefault(tabContr => tabContr.Vmodel.Value == tabVmodel) : null);
            Bind(currentTabContrStream, () => CurrentTabContr);

            currentTabContrStream
                .Func(D, c => c != null ? c.TabType : InventoryTabType.Crafting)
                .Bind(D, _currentTabStream);
        }


        //=== Public ==============================================================

        public void InitModeStream(IStream<InventoryNode.WindowMode> modeStream)
        {
            foreach (var tabVmodel in Vmodel.Value.TabVmodels.Values)
                tabVmodel.SetInventoryModeStream(modeStream);
        }

        public Dictionary<InventoryTabType, InventoryTabVmodel> GetTabVmodels()
        {
            var tabVmodels = new Dictionary<InventoryTabType, InventoryTabVmodel>();

            foreach (var tab in _tabs)
            {
                var tabVmodel = tab.GetTabVmodel(this);
                if (tabVmodel.AssertIfNull(nameof(tabVmodel)))
                    continue;

                if (tabVmodels.ContainsKey(tabVmodel.TabType))
                {
                    UI.Logger.IfError()?.Message($"{nameof(ContextViewWithParamsVmodel)}: already contains {nameof(tabVmodel)} with type {tabVmodel.TabType}").Write();
                    continue;
                }

                tabVmodels.Add(tabVmodel.TabType, tabVmodel);
            }

            return tabVmodels;
        }

        public bool IsSomeListenerFired(bool toOpen, out HotkeyListener firedHotkeyListener, out bool isAlreadyOpened)
        {
            firedHotkeyListener = null;
            isAlreadyOpened = false;
            for (int i = 0; i < _tabs.Length; i++)
            {
                var tab = _tabs[i];
                if (tab.HotkeyListener != null && tab.HotkeyListener.IsFired() && (!toOpen || tab.CanOpenWindow))
                {
                    firedHotkeyListener = tab.HotkeyListener;
                    isAlreadyOpened = tab == CurrentTabContr;
                    return true;
                }
            }

            return false;
        }

        public void OpenTabByHotkey(HotkeyListener hotkeyListener)
        {
            var tabContrByHotkey = _tabs.FirstOrDefault(tabContr => tabContr.HotkeyListener == hotkeyListener);
            if (tabContrByHotkey != null && tabContrByHotkey != CurrentTabContr)
                tabContrByHotkey.OnTabClick();
        }

        public void OpenTabByInventoryTabType(InventoryTabType inventoryTabType)
        {
            var tabContr = _tabs.FirstOrDefault(contr => contr.TabType == inventoryTabType);
            if (tabContr != null && tabContr != CurrentTabContr)
                tabContr.OnTabClick();
        }

        public void CloseOpenedTab()
        {
            CurrentTabContr?.OnTabClick();
        }

        /// <summary>
        /// Клик по табу крафта
        /// </summary>
        [UsedImplicitly]
        public void OnUnselectContextView()
        {
            Vmodel.Value?.SetContext(null);
        }
    }
}