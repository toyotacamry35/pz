using SharedCode.Aspects.Science;

namespace Uins.Inventory
{
    public class RecipeState
    {
        public bool IsAvailable;
        public KnowledgeDef ParentKnowledgeDef { get; }

        public RecipeState(KnowledgeDef parentKnowledgeDef)
        {
            ParentKnowledgeDef = parentKnowledgeDef;
            ParentKnowledgeDef.AssertIfNull(nameof(ParentKnowledgeDef));
        }
    }
}