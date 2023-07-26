using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using Assets.Src.ResourceSystem;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;

namespace Uins
{
    public class PointOfInterestNotifier : NavigationIndicatorNotifierBase
    {
        private static readonly bool IsDebug = true;

        [SerializeField, UsedImplicitly]
        private JdbMetadata _identifier;

        [SerializeField, UsedImplicitly]
        private PointOfInterestNotifierSettings _settings;

        public bool IsAllowMultipleIdentifierInstances;


        //=== Props ===========================================================

        public PointOfInterestDef Identifier => _identifier != null ? _identifier.Get<PointOfInterestDef>() : null;

        public static ListStream<PointOfInterestNotifier> Notifiers { get; } = new ListStream<PointOfInterestNotifier>();

        public bool Active
        {
            set => IsDisplayable = value;
        }

        public override INavigationIndicatorSettings NavigationIndicatorSettings => _settings;

        public override IMapIndicatorSettings MapIndicatorSettings => _settings;

        public PointOfInterestNotifierSettings PointOfInterestSettings => _settings;


        //=== Unity ===========================================================

        private void Awake()
        {
            //Не допускается к регистрации компонент с пустым Identifier
            if (Identifier == null)
            {
                UI.Logger.IfError()?.Message($"{this}:  {nameof(Identifier)} is null", gameObject).Write();
                return;
            }

            //Не допускаются к регистрации компоненты с одним и тем же Identifier без галочки подтверждения IsAllowMultipleIdentifierInstances
            var sameNotifiers = Notifiers.Where(n => n.Identifier == Identifier).ToArray();
            if (sameNotifiers.Length > 0)
            {
                if (!IsAllowMultipleIdentifierInstances || sameNotifiers.Any(n => !n.IsAllowMultipleIdentifierInstances))
                {
                    UI.Logger.Error($"{this} {nameof(Identifier)} has multiple instances on scene, but it's not alowed!. " +
                                    $"Other instances: {sameNotifiers.ItemsToString()}", gameObject);
                    return;
                }
            }

            Notifiers.Add(this);
        }

        protected override void OnDestroy()
        {
            D.Dispose();

            if (IsDisplayable)
                IsDisplayable = false;

            if (Notifiers.Contains(this))
                Notifiers.Remove(this);
        }


        //=== Public ==========================================================

        public static int GetMinDistanceFromTargetToNotifier(PointOfInterestDef[] poiDefs, Transform target)
        {
            if (poiDefs == null || poiDefs.Length == 0 || target == null)
                return 0;

            var minDist = int.MaxValue;
            foreach (var poiDef in poiDefs)
            {
                var poiNotifier = Notifiers.FirstOrDefault(n => n.Identifier == poiDef);
                if (poiNotifier == null)
                    continue;

                minDist = Mathf.Min(minDist, Mathf.RoundToInt(NavigationIndicator.GetDistanceWithoutZ(poiNotifier.transform, target)));
            }

            if (minDist == int.MaxValue)
                return 0;

            return minDist;
        }

        public override string ToString()
        {
            return $"({nameof(PointOfInterestNotifier)} '{transform.FullName()}' {nameof(Identifier)}='{Identifier?.____GetDebugRootName()}', " +
                   $"{nameof(IsAllowMultipleIdentifierInstances)}{IsAllowMultipleIdentifierInstances.AsSign()})";
        }

        protected override void OnDisplayableChanged()
        {
            base.OnDisplayableChanged();
            if (IsDebug)
                UI.Logger.IfInfo()?.Message($"{nameof(IsDisplayable)} => {IsDisplayable} -- {this}").Write(); //Для отладки геймдизайнерам
        }
    }
}