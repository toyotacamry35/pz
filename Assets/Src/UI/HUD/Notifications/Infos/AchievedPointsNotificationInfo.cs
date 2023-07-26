using System.Linq;
using EnumerableExtensions;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;
using Utilities;

namespace Uins
{
    public class AchievedPointsNotificationInfo : HudNotificationInfo
    {
        public TechPointCount[] TechPointCounts;
        public ScienceCount[] ScienceCounts;


        //=== Ctors ===========================================================

        public AchievedPointsNotificationInfo(TechPointCount[] techPointCounts, ScienceCount[] scienceCounts = null)
        {
            TechPointCounts = techPointCounts;
            ScienceCounts = scienceCounts;
        }

        public AchievedPointsNotificationInfo(ScienceCount[] scienceCounts) : this(null, scienceCounts)
        {
        }


        //=== Props ===========================================================

        public bool HasTechPointCounts => TechPointCounts != null && TechPointCounts.Any(tpc => tpc.TechPoint != null);

        public bool HasScienceCounts => ScienceCounts != null && ScienceCounts.Any(tpc => tpc.Science != null);

        public bool IsMerged(AchievedPointsNotificationInfo other)
        {
            if (other.AssertIfNull(nameof(other)))
                return false;

            if (other.HasTechPointCounts)
            {
                if (!HasTechPointCounts)
                {
                    TechPointCounts = other.TechPointCounts;
                }
                else
                {
                    foreach (var otherTechPointCount in other.TechPointCounts)
                    {
                        var indexOf = TechPointCounts.IndexOf(tpc => tpc.TechPoint == otherTechPointCount.TechPoint);

                        if (indexOf < 0)
                        {
                            var list = TechPointCounts.ToList();
                            list.Add(otherTechPointCount);
                            TechPointCounts = list.ToArray();
                        }
                        else
                        {
                            TechPointCounts[indexOf].Count += otherTechPointCount.Count;
                        }
                    }
                }
            }

            if (other.HasScienceCounts)
            {
                if (!HasScienceCounts)
                {
                    ScienceCounts = other.ScienceCounts;
                }
                else
                {
                    foreach (var otherScienceCount in other.ScienceCounts)
                    {
                        var indexOf = ScienceCounts.IndexOf(sc => sc.Science == otherScienceCount.Science);

                        if (indexOf < 0)
                        {
                            var list = ScienceCounts.ToList();
                            list.Add(otherScienceCount);
                            ScienceCounts = list.ToArray();
                        }
                        else
                        {
                            ScienceCounts[indexOf].Count += otherScienceCount.Count;
                        }
                    }
                }
            }

            return true;
        }

        public override string ToString()
        {
            var tpInfo = HasTechPointCounts ? TechPointCounts.ItemsToString() : "none";
            var scInfo = HasScienceCounts ? ScienceCounts.ItemsToString() : "none";
            return $"({GetType()}: Currencies: {tpInfo}, Sciences: {scInfo})";
        }
    }
}