using SharedCode.Aspects.Item.Templates;

namespace Uins
{
    public interface IFactionStagePerksResolver
    {
        bool GetIsFactionStagePerk(BaseItemResource perkItemResource);
    }
}