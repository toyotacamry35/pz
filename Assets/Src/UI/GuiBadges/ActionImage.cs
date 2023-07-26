using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.UI;
using Assets.Src.ResourceSystem;
using ColonyShared.SharedCode.InputActions;
using JetBrains.Annotations;
using Src.Input;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class ActionImage : BindingViewModel
    {
        private Sprite _actionSprite;

        [Binding]
        public Sprite ActionSprite
        {
            get => _actionSprite;
            set
            {
                if (_actionSprite != value)
                {
                    _actionSprite = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Public ==========================================================

        public void SetKey(InputActionDef key)
        {
            ActionSprite = GetActionSpriteByKey(key);
        }


        //=== Private =========================================================

        private Sprite GetActionSpriteByKey(InputActionDef key)
        {
            var buttonSprite = InputManager.Instance.GetBindingForAction(key)?.FirstOrDefault()?.Icon.Target;
            return buttonSprite;
        }
    }
}