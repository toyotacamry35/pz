Shader "Custom/DecalFastOnlyNormal"
{
    Properties
    {
        _MaskTex("Mask", 2D) = "white" {}
        [PerRendererData]
        _MaskMultiplier("Mask (Multiplier)", Float) = 1.0
        [Normal]
        _NormalTex ("Normal", 2D) = "bump" {}
    }

    CustomEditor "Assets.Src.Camera.Effects.DeferredProjectiveDecals.FastDecalShaderGUI"

    SubShader
    {
        LOD 200
        Cull Front
        ZTest GEqual
        ZWrite Off
        Tags { "Queue" = "Geometry" }

        Pass
        {
            Name "DEFERRED PROJECTIVE NORMALS DECAL"
            Blend SrcAlpha OneMinusSrcAlpha, Zero One
            Tags { "LightMode" = "Deferred" }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "UnityDeferredLibrary.cginc"
            #define COLOR_TEXTURE _MaskTex
            #include "DecalCommon.cginc"

            sampler2D _MaskTex;
            sampler2D _NormalTex;

            void frag(v2f i, out float4 normals : SV_TARGET2)
            {
                FRAGMENT_START
            }
            ENDCG
        }
    }

    FallBack Off
}
