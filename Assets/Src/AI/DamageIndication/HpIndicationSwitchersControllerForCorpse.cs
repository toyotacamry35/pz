using System;

namespace Assets.Src.AI.DamageIndication
{
    public class HpIndicationSwitchersControllerForCorpse : AHpIndicationSwitchersController
    {
        private void Awake()
        {
            SetBlendSmoothnessInShaderSwitchers();
            SetAllShaderSwitchers(0);
        }
    }
}