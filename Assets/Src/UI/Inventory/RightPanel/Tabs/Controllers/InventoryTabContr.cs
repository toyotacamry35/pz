using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityWeld.Binding;

namespace Uins.Inventory
{
    [Binding]
    public class InventoryTabContr : BindingController<InventoryTabVmodel>
    {
        public InventoryTabType TabType;

        public HotkeyListener HotkeyListener;

        [SerializeField, UsedImplicitly]
        private Button _tabButton;

        [SerializeField, UsedImplicitly]
        private GameObject _panel; //may be null

        [SerializeField, UsedImplicitly, FormerlySerializedAs("_contextTargetAsMonoBehaviour")]
        private MonoBehaviour _contextSingleTargetMb;

        /// <summary>
        /// Контекст синхронизации табов: д.б. открыт только один таб. 
        /// Такую роль не может взять _contextView, т.к. далеко не все табы завязаны на него
        /// </summary>
        private TabsContextContr _tabsContextContr;


        //=== Props ===========================================================

        [Binding]
        public bool IsOpen { get; protected set; }

        [Binding]
        public bool IsOpenable { get; protected set; }

        public bool CanOpenWindow => TabType != InventoryTabType.Machine;


        //=== Unity ===========================================================

        protected virtual void Awake()
        {
            if (HotkeyListener.AssertIfNull(nameof(HotkeyListener)) ||
                _tabButton.AssertIfNull(nameof(_tabButton)))
                return;

            _tabButton.onClick.RemoveAllListeners(); //игнорируем все что навешано в редакторе
            _tabButton.onClick.AddListener(OnTabClick); //регистрируем единственный метод на кнопке
            var isOpenTabStream = Vmodel.SubStream(D, vm => vm.IsOpenTabRp);
            Bind(isOpenTabStream, () => IsOpen);
            isOpenTabStream.Action(D, SwitchPanelVisibility);
            Bind(Vmodel.SubStream(D, vm => vm.IsOpenableRp), () => IsOpenable);
        }


        //=== Public ==========================================================

        public InventoryTabVmodel GetTabVmodel(TabsContextContr tabsContextContr)
        {
            _tabsContextContr = tabsContextContr;
            if (_tabsContextContr.AssertIfNull(nameof(_tabsContextContr)))
                return null;

            return new InventoryTabVmodel(TabType, _contextSingleTargetMb as IContextViewTargetWithParams);
        }

        [UsedImplicitly]
        public virtual void OnTabClick()
        {
            var prevIsOpen = IsOpen;
            _tabsContextContr.Vmodel.Value?.SetTabsContext(IsOpen ? null : Vmodel.Value); //TODOM
            OnButtonClickAdditionalActions(prevIsOpen);
        }


        //=== Protected =======================================================

        /// <summary>
        /// Доп. действия по клику / горячей клавише таба
        /// </summary>
        protected virtual void OnButtonClickAdditionalActions(bool prevIsOpen)
        {
        }


        private void SwitchPanelVisibility(bool isOn)
        {
            if (_panel != null)
                _panel.SetActive(isOn);
        }
    }
}