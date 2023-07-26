namespace Uins.Tooltips
{
    public interface ITooltipDescription
    {
        bool HasDescription { get; }
        object Description { get; }
    }
}