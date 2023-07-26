using System.Collections.Generic;
using UnityEngine;

namespace Assets.Src.Effects.PostEffectCamera
{
    public class FullscreenFxPostprocess : MonoBehaviour
    {
        private List<Material> _materialsToRender = new List<Material>(); // so there is no need to check for 'null' before running foreach loop

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            RenderTexture tempRT = RenderTexture.GetTemporary(source.descriptor);
            try
            {
                RenderTexture rt1 = source;
                RenderTexture rt2 = tempRT;

                foreach (var mat in _materialsToRender)
                {
                    Graphics.Blit(rt1, rt2, mat);
                    var temp = rt1;
                    rt1 = rt2;
                    rt2 = temp;
                }
                Graphics.Blit(rt1, destination);
            }
            finally
            {
                RenderTexture.ReleaseTemporary(tempRT);
            }
        }

        internal void SetMaterials(List<Material> materialsToRender)
        {
            _materialsToRender = materialsToRender;
        }

        internal void ResetMaterials()
        {
            _materialsToRender.Clear();
        }
    }
}