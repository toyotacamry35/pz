using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace GeneratedCode.DeltaObjects
{
    public partial class PoiEntity : IHookOnInit
    {
        public Task OnInit()
        {
            //MovementSync.VisibilityOff = true;
            return Task.CompletedTask;
        }
    }
}
