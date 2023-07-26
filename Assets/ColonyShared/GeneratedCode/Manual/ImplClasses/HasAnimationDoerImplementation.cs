using System.Threading;
using System.Threading.Tasks;
using Assets.ColonyShared.SharedCode.Entities;

namespace GeneratedCode.DeltaObjects
{
    public partial class AnimationDoerOwner
    {
        private IAnimationDoer _animationDoer;

        public IAnimationDoer AnimationDoer => _animationDoer;
        
        public Task SetAnimationDoerImpl(IAnimationDoer doer)
        {
            _animationDoer = doer;
            return Task.CompletedTask;
        }
        
        public Task UnsetAnimationDoerImpl(IAnimationDoer doer)
        {
            Interlocked.CompareExchange(ref _animationDoer, null, doer);
            return Task.CompletedTask;
        }
    }
}
