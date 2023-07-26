namespace Uins
{
    public class QuestPhases : ItemsCollection<QuestPhaseViewModel, QuestPhaseData>
    {
        protected override bool DoItemsSorting => true;
    }
}