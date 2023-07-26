using Assets.Src.Aspects.VisualMarkers;
using EnumerableExtensions;
using ReactivePropsNs;
using Src.Effects;
using UnityEngine;

namespace Uins
{
    public class BadgePoint : BindingViewModel, IBadgePoint
    {
        [SerializeField]
        protected GuiBadge GuiBadgePrefab;

        protected AVisualMarker VisualMarker;


        //=== Props ===========================================================

        public Vector3 Position => transform.position;

        SharedCode.Utils.Vector3? IHasLookAtAnchor.Anchor => transform.position.ToShared();

        protected GuiBadge ConnectedGuiBadge { get; set; }

        public bool HasPoint => VisualMarker != null ? VisualMarker.HasPoint : true;

        public bool IsDebug => VisualMarker != null ? VisualMarker.IsDebug : false;

        public ReactiveProperty<bool> IsNeedForGuiRp { get; } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> IsSelectedRp { get; } = new ReactiveProperty<bool> {Value = false};

        /// <summary>
        /// Для временного отключения Gui-бейджа (через альтернативу IsNeedForGuiRp/IsSelectedRp)
        /// </summary>
        public ReactiveProperty<bool> IsVisibleLogicallyRp { get; } = new ReactiveProperty<bool>();

        // used for automatic badge generation
        public void SetPrefab(GuiBadge prefab)
        {
            GuiBadgePrefab = prefab;
        }


        //=== Unity ===========================================================

        protected virtual void Start()
        {
            if (GuiBadgePrefab.AssertIfNull(nameof(GuiBadgePrefab), gameObject))
                return;

            VisualMarker = GetComponentInParent<AVisualMarker>();
            if (VisualMarker == null)
            {
                UI.Logger.Error(
                    $"VisualMarker is null! '{transform.FullName()}' {nameof(gameObject.activeSelf)}{gameObject.activeSelf.AsSign()}, " +
                    $"{nameof(gameObject.activeInHierarchy)}{gameObject.activeInHierarchy.AsSign()}, " +
                    $"alternative get: {GetComponents<AVisualMarker>()?.ItemsToString()}");
                return;
            }

            IsVisibleLogicallyRp.Value = true; //По ум. у всех вкл.
            IsSelectedRp.Value = false;

            if (HasPoint)
            {
                IsNeedForGuiRp.Action(D, RegisterInGuiOrRelease);
            }
            else
            {
                IsNeedForGuiRp
                    .Zip(D, IsSelectedRp)
                    .Func(D, AndFunc)
                    .Action(D, OnConstantBadge);
            }

            // if (IsDebug) //DEBUG
            // {
            //     IsNeedForGuiRp.Log(D, $"{name}.{nameof(IsNeedForGuiRp)}");
            //     IsSelectedRp.Log(D, $"{name}.{nameof(IsSelectedRp)}");
            //     IsVisibleLogicallyRp.Log(D, $"{name}.{nameof(IsVisibleLogicallyRp)}");
            // }
        }

        protected override void OnDestroy()
        {
            RegisterInGuiOrRelease(false);
            base.OnDestroy();
        }


        //=== Public ==========================================================

        public void SetIsImportantBadgeShown(bool isOn)
        {
            if (VisualMarker != null)
                VisualMarker.IsImportantBadgeShownRp.Value = isOn;
        }


        //=== Protected =======================================================

        protected void RegisterInGuiOrRelease(bool isRegister)
        {
            if (BadgesGui.Instance == null ||
                isRegister == (ConnectedGuiBadge != null))
                return;

            if (isRegister)
            {
                ConnectedGuiBadge = BadgesGui.Instance.GetGuiBadge(GuiBadgePrefab);
                ConnectedGuiBadge.Connect(this);
            }
            else
            {
                ConnectedGuiBadge.Disconnect();
                BadgesGui.Instance?.ReleaseGuiBadge(GuiBadgePrefab, ConnectedGuiBadge);
                ConnectedGuiBadge = null;
            }
        }

        public static bool AndFunc(bool b1, bool b2)
        {
            return b1 && b2;
        }

        public static bool AndNotFunc(bool b1, bool b2)
        {
            return AndFunc(b1, !b2);
        }

        private void OnConstantBadge(bool isAdd)
        {
            var gui = BadgesGui.Instance;
            if (gui == null)
                return;

            if (isAdd)
            {
                if (!gui.ConstantsBadgePoints.Contains(this))
                    gui.ConstantsBadgePoints.Add(this);
            }
            else
            {
                if (gui.ConstantsBadgePoints.Contains(this))
                    gui.ConstantsBadgePoints.Remove(this);
            }
        }
    }
}