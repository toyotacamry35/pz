using Core.Environment.Logging.Extension;
using UnityEngine;

namespace Assets.Src.AI.DamageIndication
{
    [ExecuteInEditMode]
    public class HpIndicationShaderSwitcher : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private float _blendLen = 0.2f;
        private float _blendShiftMin = 0.2f;
        private float _blendShiftMax = 1f;
        private int _hpSlope;
        private int _hpYintercept;
        private MaterialPropertyBlock _propBlock;
        private Renderer _renderer;
        private float _hpLevelNormalized = 1;
        private void Awake()
        {
            _hpSlope = Shader.PropertyToID("_ClampedDecalSlope");
            _hpYintercept = Shader.PropertyToID("_ClampedDecalYintercept");
            _renderer = GetComponent<Renderer>();
        }

        internal void SetHP(float hpLevel) => _hpLevelNormalized = hpLevel;

        internal void SetBlendingParameters(float blendLength, float blendShiftMin, float blendShiftMax)
        {
            _blendLen = blendLength;
            _blendShiftMin = blendShiftMin;
            _blendShiftMax = blendShiftMax;
        }

        private void OnWillRenderObject()
        {
            if (_renderer != null)
            {
                /// We do not need to avoid division by zero for 'float' type variables (IEEE-754).
                /// https://docs.microsoft.com/en-us/dotnet/api/system.single?view=netframework-4.8
                /// https://docs.microsoft.com/en-us/windows/win32/direct3d11/floating-point-rules#honored-ieee-754-rules
                /// 
                var slope = 1 / _blendLen; 
                var xIntercept = (_hpLevelNormalized * (_blendShiftMax + _blendLen - _blendShiftMin)) - _blendLen + _blendShiftMin;
                var yIntercept = -slope * xIntercept;
                if (_propBlock == default)
                    _propBlock = new MaterialPropertyBlock();
                _renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetFloat(_hpSlope, slope);
                _propBlock.SetFloat(_hpYintercept, yIntercept);
                _renderer.SetPropertyBlock(_propBlock);
            }
            else
                Logger.IfWarn()?.Message("No Renderer found on Game Object '{0}'. You propably need to delete '{1}' component.", gameObject.name, nameof(HpIndicationShaderSwitcher)).Write();
        }
    }
}