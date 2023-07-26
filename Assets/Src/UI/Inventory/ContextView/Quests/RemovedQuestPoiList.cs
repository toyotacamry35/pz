using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using EnumerableExtensions;
using ReactivePropsNs;

namespace Uins
{
    public class RemovedQuestPoiList : HasDisposablesMonoBehaviour
    {
        public static RemovedQuestPoiList Instance;

        public ListStream<PointOfInterestDef> RemovedQuestPoints = new ListStream<PointOfInterestDef>();


        //=== Unity ===========================================================

        private void Awake()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemovedQuestPoints.Dispose();
            if (Instance == this)
                Instance = null;
        }


        //=== Public ==========================================================

        public void AddPoiDef(PointOfInterestDef poiDef)
        {
            //UI.CallerLog($"poiDef={poiDef}"); //DEBUG
            if (poiDef.AssertIfNull(nameof(poiDef)) || RemovedQuestPoints.Contains(poiDef))
                return;

            RemovedQuestPoints.Add(poiDef);
        }

        public void AddPoiDefs(PointOfInterestDef[] poiDefs)
        {
            //UI.CallerLog($"poiDefs={poiDefs.ItemsToString()}"); //DEBUG
            if (poiDefs.AssertIfNull(nameof(poiDefs)))
                return;

            poiDefs.ForEach(AddPoiDef);
        }
    }
}