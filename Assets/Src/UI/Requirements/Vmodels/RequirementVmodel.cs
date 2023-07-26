using Assets.Src.ResourcesSystem.Base;
using ReactivePropsNs;

namespace Uins
{
    public class RequirementVmodel<T> : BindingVmodel where T : BaseResource
    {
        public readonly T Resource;
        public readonly int RequiredCount;


        //=== Props ===========================================================

        public ReactiveProperty<bool> IsEnoughRp { get; } = new ReactiveProperty<bool>() {Value = true};


        //=== Ctor ============================================================

        public RequirementVmodel(T resource, int requiredCount, IDictionaryStream<T, int> availResourcesStream = null)
        {
            if (resource.AssertIfNull(nameof(resource)))
                return;

            RequiredCount = requiredCount;
            Resource = resource;

            if (availResourcesStream != null)
                availResourcesStream.KeyStream(D, Resource).Action(D, availCount => IsEnoughRp.Value = GetIsEnough(availCount));
        }


        //=== Public ==========================================================

        public override void Dispose()
        {
            base.Dispose();
            IsEnoughRp.Dispose();
        }

        public override string ToString()
        {
            return $"({GetType().NiceName()}: {nameof(Resource)}={Resource?.____GetDebugShortName()}, {nameof(RequiredCount)}={RequiredCount}, " +
                   $"{nameof(IsEnoughRp)}{IsEnoughRp.Value.AsSign()})";
        }


        //=== Protected =======================================================

        protected virtual bool GetIsEnough(int availCount)
        {
            return RequiredCount <= availCount;
        }
    }
}