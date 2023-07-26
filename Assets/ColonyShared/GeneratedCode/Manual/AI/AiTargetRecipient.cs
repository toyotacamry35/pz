using System.Threading.Tasks;
using ResourceSystem.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class AiTargetRecipient
    {
        public ValueTask<bool> SetTargetImpl(OuterRef targetRef)
        {
            Target = targetRef;
            return new ValueTask<bool>(true);
        }
    }
}