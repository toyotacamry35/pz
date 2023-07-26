using Assets.ColonyShared.SharedCode.Aspects.WorldObjects;
using ReactivePropsNs;

namespace Uins
{
    public interface IQuestTrackingContext
    {
        ReactiveProperty<QuestItemViewModel> TrackedQuestRp { get; }
        ReactiveProperty<PointOfInterestDef[]> TrackedQuestPoiList { get; }
    }
}