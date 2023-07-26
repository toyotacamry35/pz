Shader "Custom/DecalFast"
{
    Properties
    {
        _MainTex("Albedo(RGB) Mask(A)", 2D) = "white" {}
        [HDR]
        _Color("Albedo (Multiplier)", Color) = (1,1,1,1)
        [PerRendererData]
        _MaskMultiplier("Mask (Multiplier)", Float) = 1.0
        [Normal]
        _NormalTex ("Normal", 2D) = "bump" {}
        _SpecularTex ("Specular(f0 in alpha)", 2D) = "white" {}
		[HDR]
        _SpecularMultiplier ("Specular (Multiplier)", Color) = (1.0, 1.0, 1.0, 1.0)
        _SmoothnessTex ("Roughness", 2D) = "black" {}
        _SmoothnessMultiplier ("Smoothness (Multiplier)", Range(0.0, 1.0)) = 0.5
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
            Name "DEFERRED PROJECTIVE DECAL"
            // blend color based on output alpha but leave occlusion as it is
            Blend 0 SrcAlpha OneMinusSrcAlpha, Zero One
            Blend 1 One OneMinusSrcAlpha, One OneMinusSrcAlpha
            Blend 2 SrcAlpha OneMinusSrcAlpha, Zero One
            Tags { "LightMode" = "Deferred" }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "UnityDeferredLibrary.cginc"
            #define COLOR_TEXTURE _MainTex
            #include "DecalCommon.cginc"

            sampler2D _MainTex;
            sampler2D _NormalTex;
            sampler2D _SpecularTex;
            sampler2D _SmoothnessTex;
            
            float4 _Color;
            float4 _SpecularMultiplier;
            float _SmoothnessMultiplier;

            // float4 _MainTex_ST; // no need for tiling and offset in decals


            void frag(v2f i, out float4 albedo : SV_TARGET0, out float4 specSmoothness : SV_TARGET1, out float4 normals : SV_TARGET2)
            {
                FRAGMENT_START
                albedo = color;
                float4 specularF0 = tex2D(_SpecularTex, texUV) * _SpecularMultiplier;
                
                // we want blended smoothness to be (smoothness * color.a + dst * (1 - color.a))
                // hovewer we can not blend it like that since smoothness is occupies the a channel of target
                // with Blend of the alpha channel configured as "One OneMinusSrcAlpha" we could get the following result:
                // (smoothness * color.a + dst * (1 - smoothness * color.a)) if we set 'src' equals to (smoothness * color.a)
                float smoothness = (1 - tex2D(_SmoothnessTex, texUV).r) * _SmoothnessMultiplier * color.a * specularF0.a;
                
                // really wrong way to do this but looks surprisingly okay for decals
                // TODO: find a way to blend it more properly
                float3 specular = specularF0.rgb * color.a * specularF0.a;
                
                specSmoothness = float4(specular, smoothness);
                albedo *= _Color;
            }
            ENDCG
        }
    }

    FallBack Off
}
