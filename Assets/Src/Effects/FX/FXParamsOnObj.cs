using Assets.ResourceSystem.Aspects.Effects;
using UnityEngine;

namespace 
    Assets.Src.Effects.FX
{
    public class FXParamsOnObj : MonoBehaviour
    {
        [System.Serializable]
        public class FXParamsValue
        {
            public FXParamsDef param;
            public float value;

            public FXParamsValue(FXParamsValue set)
            {
                param = set.param;
                value = set.value;
            }
        }

        public FXParamsValue[] pair;
    }    
}
