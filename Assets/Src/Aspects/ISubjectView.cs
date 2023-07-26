namespace Assets.Src.Aspects
{
    public interface ISubjectView : IEntityView, IDetachableView, IAnimatedView
    {
        bool HasAuthority { get; }
    }
}