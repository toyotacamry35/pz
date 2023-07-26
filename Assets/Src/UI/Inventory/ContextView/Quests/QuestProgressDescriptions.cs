using L10n;

namespace Uins
{
    public class QuestProgressDescriptions : ItemsCollection<QuestDescriptionViewModel, LocalizedStringContainer>
    {
        protected override bool DoItemsSorting => true;
    }
}