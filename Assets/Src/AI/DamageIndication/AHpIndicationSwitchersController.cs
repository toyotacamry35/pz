using System;
using UnityEngine;

namespace Assets.Src.AI.DamageIndication
{
    public abstract class AHpIndicationSwitchersController : MonoBehaviour
    {
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public HpIndicationShaderSwitcher[] _hpIndicationShaderSwitchers = Array.Empty<HpIndicationShaderSwitcher>();
        [Range(0, 1)]
        public float _blendingSmoothness = 0.2f;
        [Range(0, 1)]
        public float _blendingThresholdMin = 0.2f;
        [Range(0, 1)]
        public float _blendingThresholdMax = 1f;

        protected void SetBlendSmoothnessInShaderSwitchers()
        {
            foreach (var shaderSwitcher in _hpIndicationShaderSwitchers)
            {
                if (shaderSwitcher == null)
                {
                    Logger.Warn("You forgot to set one of the fields in '{0}' array of '{1}' component on {2} {3}",
                                nameof(_hpIndicationShaderSwitchers), nameof(HpIndicationSwitchersController),
                                nameof(GameObject), gameObject.name);
                    continue;
                }
                else
                    shaderSwitcher.SetBlendingParameters(_blendingSmoothness, _blendingThresholdMin, _blendingThresholdMax);
            }
        }

        protected void SetAllShaderSwitchers(float value)
        {
            foreach (var shaderSwitcher in _hpIndicationShaderSwitchers)
            {
                if (shaderSwitcher)
                    shaderSwitcher.SetHP(value);
            }
        }
    }
}