using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.ResourcesSystem.Base;
using UnityEngine;

namespace ColonyShared.SharedCode.Input
{
    public abstract class InputSourceDef : BaseResource
    {
        public string Description;
        public UnityRef<Sprite> Icon;
    }

    public class InputKeyDef : InputSourceDef
    {
        public string Key;
    }

    public class InputAxisDef : InputSourceDef
    {
        public string Axis;
    }
    
    public class InputAxisRdpWorkaroundDef : InputSourceDef
    {
        public enum AxisType { X, Y }
        public AxisType Axis;
        public float Sensitivity = 1;
    }    
}