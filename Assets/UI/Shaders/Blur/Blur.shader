Shader "UI/Blur"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _InverseResolution("Inverse Resolution", Vector) = (256, 256, 0, 0)
        _Direction ("Direction", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            Texture2D _MainTex;
            SamplerState linear_repeat_sampler_MainTex;
            float2 _Direction;
            float2 _InverseResolution;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 offset1 = float2(1.3846153846, 1.3846153846) * _Direction;
                float2 offset2 = float2(3.2307692308, 3.2307692308) * _Direction;
                float4 color = _MainTex.Sample(linear_repeat_sampler_MainTex, IN.texcoord) * 0.2270270270;
                color += _MainTex.Sample(linear_repeat_sampler_MainTex, IN.texcoord + (offset1 * _InverseResolution)) * 0.3162162162;
                color += _MainTex.Sample(linear_repeat_sampler_MainTex, IN.texcoord - (offset1 * _InverseResolution)) * 0.3162162162;
                color += _MainTex.Sample(linear_repeat_sampler_MainTex, IN.texcoord + (offset2 * _InverseResolution)) * 0.0702702703;
                color += _MainTex.Sample(linear_repeat_sampler_MainTex, IN.texcoord - (offset2 * _InverseResolution)) * 0.0702702703;
                return color;
            }
            
            ENDCG
        }
    }
    FallBack Off
}
