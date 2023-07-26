using UnityEngine;
using UnityEngine.UI;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.Src.Inventory;
using Core.Environment.Logging.Extension;
using SharedCode.Aspects.Item.Templates;
using Uins.Slots;

namespace Uins.Inventory
{
    /// <summary>
    /// Слот предмета рецепта в ContextView
    /// </summary>
    public class CraftRecipeElementUI : MonoBehaviour
    {
        public Type type;
        [HideInInspector] public CraftRecipeModifier CraftRecipeModifier { get; private set; }

        [HideInInspector] public int index { get; set; } //индекс слота
        [HideInInspector] public int indexItem { get; private set; } = -1; // индекс предмета

        public System.Action<CraftRecipeElementUI> ClickEvent;
        public System.Action<CraftRecipeElementUI> PointerEnterEvent;
        public System.Action<CraftRecipeElementUI> PointerExitEvent;

        public bool IsEmpty { get; private set; }

        public enum Type
        {
            BaseType,
            Extra,
            ToChange
        }

        public void Init(ICharacterItemsNotifier characterItemsNotifier, CraftRecipeContextView craftRecipeContextView)
        {
            if (!_wasInit)
            {
                _characterItemsNotifier = characterItemsNotifier;
                _characterItemsNotifier.CharacterItemsChanged += OnCharacterItemsChanged;

                _craftRecipeContextView = craftRecipeContextView;
                _craftRecipeContextView.ChangeCountEvent += ChangeCount;

                _wasInit = true;
            }
        }

        void OnDisable()
        {
            indexItem = -1;
        }

        void OnDestroy()
        {
            if (_wasInit)
            {
                _characterItemsNotifier.CharacterItemsChanged -= OnCharacterItemsChanged;
                _craftRecipeContextView.ChangeCountEvent -= ChangeCount;
            }
        }

        public void Show(CraftRecipeModifier modifier, int index, bool canChange) //показать кокретный слот
        {
            indexItem = index;
            CraftRecipeModifier = modifier;
            _use.SetActive(true);
            _notUse.SetActive(false);
            SetActive(null);
            _canChange.SetActive(canChange);
            _button.interactable = canChange;
            _notItem.SetActive(false);
            IsEmpty = false;
            var clusterIR = modifier.Item.Item.Target;
            if (clusterIR != null)
            {
                _icon.gameObject.SetActive(true);
                _icon.sprite = clusterIR.Icon.Target;
            }
            else
            {
                UI.Logger.IfError()?.Message($"Empty {nameof(BaseItemResource)} in {modifier.Item.Item}").Write();
                _icon.sprite = _emptyIcon;
            }

            OnCharacterItemsChanged(null, 0);
        }

        public void ShowEmpty() //показать пустой слот
        {
            IsEmpty = true;
            CraftRecipeModifier = default;
            indexItem = -1;
            _use.SetActive(true);
            _notUse.SetActive(false);
            SetActive(null);
            _canChange.SetActive(false);
            _button.interactable = true;
            _notItem.SetActive(true);
            _icon.gameObject.SetActive(false);

            SetAvailable(true);
            _countText.text = " ";
        }

        public void Hide() //в этом рецепте слот не используется
        {
            indexItem = -1;
            _use.SetActive(false);
            _notUse.SetActive(true);
            SetActive(null);
            _button.interactable = false;
        }

        public void Click() //нажатие на просмотр дополнительных вариантов заполенения слота
        {
            ClickEvent?.Invoke(this);
        }

        public void OnPointerEnter()
        {
            PointerEnterEvent?.Invoke(this);
        }

        public void OnPointerExit()
        {
            PointerExitEvent?.Invoke(this);
        }

        public void SetActive(CraftRecipeElementUI element)
        {
            _active.SetActive(element == this);
        }

        void ChangeCount(int value) //изменилось количество создаваемых предметов
        {
            if (indexItem >= 0)
            {
                bool available = _currentCount >= _needCount * value;
                _countText.text = string.Format(available ? _availableFormat : _notAvailableFormat, _currentCount, _needCount * value);
                SetAvailable(available);
            }
        }

        void OnCharacterItemsChanged(SlotViewModel slotViewModel, int stackDelta) //изменился инвентарь, надо изменить количество
        {
            if (_characterItemsNotifier != null && indexItem >= 0)
            {
                _currentCount = CraftRecipeModifier.Item.Item.Target == null 
                    ? 0 
                    :_characterItemsNotifier.GetItemResourceCount(CraftRecipeModifier.Item.Item.Target);
                _needCount = CraftRecipeModifier.Item.Count;
                ChangeCount(_craftRecipeContextView.Count);
            }
        }

        void SetAvailable(bool available)
        {
            SpriteState spriteState = _button.spriteState;
            spriteState.highlightedSprite = available ? _availableOver : _notAvailableOver;
            spriteState.pressedSprite = available ? _availableOver : _notAvailableOver;
            spriteState.disabledSprite = available ? _available : _notAvailable;
            _button.spriteState = spriteState;

            _buttonImage.sprite = available ? _available : _notAvailable;
        }

        ICharacterItemsNotifier _characterItemsNotifier;
        CraftRecipeContextView _craftRecipeContextView;
        bool _wasInit;
        int _currentCount;
        int _needCount;

        [SerializeField] Button _button;
        [SerializeField] Image _icon;

        [SerializeField] Image _buttonImage;
        [SerializeField] Sprite _available;
        [SerializeField] Sprite _notAvailable;
        [SerializeField] Sprite _availableOver;
        [SerializeField] Sprite _notAvailableOver;

        [SerializeField] TMPro.TextMeshProUGUI _countText;
        [SerializeField] string _availableFormat;
        [SerializeField] string _notAvailableFormat;

        [SerializeField] GameObject _canChange; //иконка показывающая есть ли варианты для изменения
        [SerializeField] GameObject _notItem; //если не null то слот может быть пустым
        [SerializeField] GameObject _use; //включается при show, этот слот используется для рецепта
        [SerializeField] GameObject _notUse; //если слот не используется, показывается этот объект
        [SerializeField] GameObject _active;

        [SerializeField] Sprite     _emptyIcon;
    }
}

