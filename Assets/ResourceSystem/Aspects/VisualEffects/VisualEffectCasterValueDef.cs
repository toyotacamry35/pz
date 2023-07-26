using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.Character.Events
{
    public abstract class VisualEffectCasterValueDef : BaseResource
    {
    }

    public class EventStatParam : VisualEffectCasterValueDef
    {
        public string Stat;
    }
}
