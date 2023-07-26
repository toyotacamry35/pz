using System.Collections.Generic;
using System.Linq;
using EnumerableExtensions;
using JetBrains.Annotations;
using Src.Input;
using Src.InputActions;
using UnityEngine;
using UnityWeld.Binding;
using ReactivePropsNs;
using NLog;

namespace Uins
{
    [Binding]
    public class DialogSelfGuiBadge : GuiBadge
    {
        private static readonly NLog.Logger Logger = LogManager.GetLogger(nameof(DialogSelfGuiBadge));

        [SerializeField, UsedImplicitly]
        private HotkeyListener _exit;

        [SerializeField, UsedImplicitly]
        private InputActionTriggerRef[] _variants;

        [SerializeField, UsedImplicitly]
        /// <summary> Блокировки и/или переопределения привязок клавиш к командам активируемые при открытии данного окна </summary>    
        private InputBindingsRef _inputBindings;

        //=== Props ===========================================================

        [Binding]
        public ObservableList<DialogVariantVmodel> DialogVariantViewModels { get; } = new ObservableList<DialogVariantVmodel>();


        //=== Unity  ==========================================================

        private void Update()
        {
            if (!IsVisibleFinally)
                return;

            if (_exit.IsFired())
            {
                ((DialogSelfBadgePoint)BadgePoint).OnEscButton();
                return;
            }
            // Всю остальную обработку заменил на стримы
        }


        //=== Public ==========================================================

        public void UpdateDialogVariants(IEnumerable<DialogVariantVmodel> dialogVariantViewModels)
        {
            DialogVariantViewModels.Clear();
            dialogVariantViewModels?.Where(vm => vm != null)
                .ForEach(vm => DialogVariantViewModels.Add(vm));
        }

        public override void Connect([NotNull] IBadgePoint badgePoint)
        {
            if (IsConnected && BadgePoint == badgePoint)
                return;

            base.Connect(badgePoint);

            if (_inputBindings != null)
                InputManager.Instance.PushBindings(this, _inputBindings.Target);

            for (int i = 0; i < _variants.Length; i++)
                if (_variants[i] != null && _variants[i].Target != null)
                {
                    int capturedI = i;
                    var stream = InputManager.Instance.Stream(_variants[i].Target);

                    stream.ThreadSafeToStream(ConnectionD)
                        .Where(ConnectionD, state => IsVisibleFinally)
                        .Where(ConnectionD, state => state.Activated)
                        .Where(ConnectionD, state => DialogVariantViewModels != null && capturedI < DialogVariantViewModels.Count)
                        .Action(ConnectionD, state => ((DialogSelfBadgePoint)BadgePoint).OnVariantButton(capturedI));
                }
        }
        public override void Disconnect()
        {
            if (!IsConnected)
                return;

            base.Disconnect();

            if (_inputBindings != null)
                InputManager.Instance.PopBindings(this);
        }

        //=== Protected ==========================================================

        protected override void Awake()
        {
            base.Awake();
            _inputBindings.AssertIfNull("_inputBindings");
        }
    }
}