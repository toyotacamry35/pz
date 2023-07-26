using Assets.Src.ResourcesSystem.Base;

namespace GeneratedCode.DeltaObjects
{
    public class WizardWatchdogDef : BaseResource
    {
        public int MillisecondsTooMuchForStart { get; set; } = 200;
        public int MillisecondsCriticalForStart { get; set; } = 4000;
        public int MillisecondsTooMuchForUpdate { get; set; } = 100;
        public int MillisecondsTooMuchForOneWord { get; set; } = 100;
        public int MillisecondsTooMuchForOnePrediate { get; set; } = 100;
        public int LogHistoryCount { get; set; } = 200;
    }
}
