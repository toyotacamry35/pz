using System;
using SharedCode.Aspects.Sessions;
using UnityEngine;

namespace Uins
{
    [Serializable]
    public struct SemanticColorsHolder
    {
        public Color PrimaryColor;
        public Color InfoColor;
        public Color SuccessColor;
        public Color DangerColor;

        public Color GetColor(SemanticContext defSemanticContext)
        {
            switch (defSemanticContext)
            {
                case SemanticContext.Success:
                    return SuccessColor;
                case SemanticContext.Danger:
                    return DangerColor;
                case SemanticContext.Info:
                    return InfoColor;
                case SemanticContext.Primary:
                    return PrimaryColor;
                default:
                    return PrimaryColor;
            }
        }
    }
}