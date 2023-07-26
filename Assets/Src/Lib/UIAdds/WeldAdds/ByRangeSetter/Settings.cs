using System;
using UnityEngine;

namespace WeldAdds
{
    [Serializable]
    public class Settings
    {
        public Color Color;
        public float Float;

        public override string ToString()
        {
            return $"Settings: Color={Color}, Float={Float}";
        }
    }
}