using System.Collections.Generic;
using Assets.Src.ResourceSystem.L10n;
using ColonyDI;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class BadgesGui : DependencyEndNode
    {
        [SerializeField, UsedImplicitly]
        private WindowId[] _alowedWindowIds;

        [SerializeField, UsedImplicitly]
        private InteractiveGuiBadge _constantInteractiveGuiBadge;

        [SerializeField, UsedImplicitly]
        private Transform _guiBadgesTransform;

        private Dictionary<GuiBadge, List<GuiBadge>> _guiBadgesPool = new Dictionary<GuiBadge, List<GuiBadge>>();
        private static int _guiBadgesCount;

        [SerializeField, UsedImplicitly]
        private LocalizationKeyPairsDefRef _defaultActionsLkpdRef;

        [SerializeField, UsedImplicitly]
        private LocalizationKeyPairsDefRef _commonNamesLkpdRef;

        [SerializeField, UsedImplicitly]
        private HudVisibilityRepeater _hudVisibilityRepeater;


        //=== Props ===============================================================

        [Dependency]
        private GameState GameState { get; set; }

        public static BadgesGui Instance { get; private set; }

        [Binding]
        public bool IsShown { get; private set; }

        public LocalizedString AttackActionLs => _defaultActionsLkpdRef.Target.Ls1;

        public LocalizedString UnknownObjectLs => _commonNamesLkpdRef.Target.Ls1;

        public LocalizedString ObjectLs => _commonNamesLkpdRef.Target.Ls2;

        public ListStream<BadgePoint> ConstantsBadgePoints { get; } = new ListStream<BadgePoint>();


        //=== Unity ===============================================================

        private void Awake()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);
            _guiBadgesTransform.AssertIfNull(nameof(_guiBadgesTransform));
            _alowedWindowIds.IsNullOrEmptyOrHasNullElements(nameof(_alowedWindowIds));
            _constantInteractiveGuiBadge.AssertIfNull(nameof(_constantInteractiveGuiBadge));
            _defaultActionsLkpdRef.Target.AssertIfNull(nameof(_defaultActionsLkpdRef));
            _hudVisibilityRepeater.AssertIfNull(nameof(_hudVisibilityRepeater));

            ConstantsBadgePoints.InsertStream.Action(D, OnInsertConstantBadgePoint);
            ConstantsBadgePoints.RemoveStream.Action(D, OnRemoveConstantBadgePoint);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (Instance == this)
                Instance = null;
        }


        //=== Public ==============================================================

        public GuiBadge GetGuiBadge(GuiBadge guiBadgePrefab)
        {
            if (!_guiBadgesPool.TryGetValue(guiBadgePrefab, out var freeGuiBadges))
            {
                freeGuiBadges = new List<GuiBadge>();
                _guiBadgesPool.Add(guiBadgePrefab, freeGuiBadges);
            }

            if (freeGuiBadges.Count == 0)
            {
                var newGuiBadge = Instantiate(guiBadgePrefab, _guiBadgesTransform);
                newGuiBadge.name = guiBadgePrefab.name + _guiBadgesCount++;
                return newGuiBadge;
            }

            var lastPos = freeGuiBadges.Count - 1;
            var freeGuiBadge = freeGuiBadges[lastPos];
            freeGuiBadges.RemoveAt(lastPos);
            return freeGuiBadge;
        }

        public void ReleaseGuiBadge(GuiBadge guiBadgePrefab, GuiBadge guiBadge)
        {
            if (guiBadgePrefab.AssertIfNull(nameof(guiBadgePrefab)) ||
                guiBadge.AssertIfNull(nameof(guiBadge)))
                return;

            guiBadge.SetOutPosition();
            if (!_guiBadgesPool.TryGetValue(guiBadgePrefab, out var guiBadges))
            {
                UI.Logger.IfError()?.Message($"Not found {nameof(guiBadgePrefab)}={guiBadgePrefab} as key").Write();
                return;
            }

            guiBadges.Add(guiBadge);
        }


        //=== Protected ===========================================================

        public override void AfterDependenciesInjectedOnAllProviders()
        {
            IStream<bool> anyAlowedWindowIsOpenStream = null;
            for (int i = 0; i < _alowedWindowIds.Length; i++)
            {
                var alowedGuiWindow = WindowsManager.GetWindow(_alowedWindowIds[i]);
                if (anyAlowedWindowIsOpenStream == null)
                {
                    anyAlowedWindowIsOpenStream = alowedGuiWindow.State.Func(D, state => state == GuiWindowState.Opened);
                }
                else
                {
                    anyAlowedWindowIsOpenStream = anyAlowedWindowIsOpenStream
                        .Zip(D, alowedGuiWindow.State.Func(D, state => state == GuiWindowState.Opened))
                        .Func(D, (b1, b2) => b1 || b2);
                }
            }

            var noOverlayWindowsAndInGame = WindowsManager.HasOpenedOverlayWindowsStream
                .Zip(D, GameState.IsInGameRp)
                .Func(D, (hasOverlayWindows, isInGame) => !hasOverlayWindows && isInGame);

            var isShownStream = noOverlayWindowsAndInGame
                .Zip(D, anyAlowedWindowIsOpenStream)
                .Func(D, (isTimeFor, anyAlowedOpen) => isTimeFor && anyAlowedOpen);
            Bind(isShownStream, () => IsShown);

            _hudVisibilityRepeater.Init();
        }


        //=== Private =========================================================

        private void OnRemoveConstantBadgePoint(RemoveEvent<BadgePoint> removeEvent)
        {
            //Отсоединяем уходящего, если подсоединен
            if (_constantInteractiveGuiBadge.IsConnected && _constantInteractiveGuiBadge.BadgePoint == removeEvent.Item)
                _constantInteractiveGuiBadge.Disconnect();
        }

        private void OnInsertConstantBadgePoint(InsertEvent<BadgePoint> insertEvent)
        {
            if (insertEvent.Item.AssertIfNull(nameof(insertEvent)))
                return;

            //Безусловно коннектим новый BadgePoint, если нужно, отсоединяем предыдущий
            if (_constantInteractiveGuiBadge.IsConnected)
                _constantInteractiveGuiBadge.Disconnect();

            _constantInteractiveGuiBadge.Connect(insertEvent.Item);
        }
    }
}