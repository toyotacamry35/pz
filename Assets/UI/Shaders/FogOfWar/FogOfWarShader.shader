Shader "UI/FogOfWar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GridColor ("Grid Color", color) = (1, 1, 1, 1)
        _GridFogColor ("Grid Fog Color", color) = (1, 1, 1, 1)
        _GridCells ("Grid Cells Count", Int) = 10
        _GridLineThickness ("Grid Line Thickness", float) = 1        
        _GridOffset ("Grid Offset", Range (0, 1)) = 0.5
    }

    SubShader
    {
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "FogOfWar"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #include "UnityCG.cginc"

            Texture2D _MainTex;
            SamplerState point_clamp_sampler_MainTex;
            StructuredBuffer<int> _IndexBuffer : register(t0);

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord  : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                return OUT;
            }

            float4 frag(v2f IN) : SV_Target
            {
                uint index = round(_MainTex.Sample(point_clamp_sampler_MainTex, IN.texcoord).r * 32767);
                return _IndexBuffer[index];
            }
            ENDCG
        }

        Pass
        {
            Name "Fog and grid"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _FogTex;
            float4 _MainTex_ST;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord  : TEXCOORD0;
            };
            
            float4 _GridColor;
            float4 _GridFogColor;
            int _GridCells;
            float _GridLineThickness;
            float _GridOffset;


            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return OUT;
            }

            float4 frag(v2f IN) : SV_Target
            {
                float4 outColor = tex2D(_MainTex, IN.texcoord);
                float fogStrength = tex2D(_FogTex, IN.texcoord).r;
                outColor.rgb *= fogStrength;
                
                float2 uv = IN.texcoord * _GridCells;
                float2 wrapped = frac(uv) - _GridOffset;
                float2 range = abs(wrapped);

                float2 speeds;
                // Euclidean norm gives slightly more even thickness on diagonals
                /*float4 deltas = float4(ddx(uv), ddy(uv));
                speeds = sqrt(float2(
                dot(deltas.xz, deltas.xz),
                dot(deltas.yw, deltas.yw)
                ));*/
                // Cheaper Manhattan norm in fwidth slightly exaggerates thickness of diagonals
                speeds = fwidth(uv);

                float2 pixelRange = range/speeds;
                float lineWeight = saturate(min(pixelRange.x, pixelRange.y) - _GridLineThickness);
                float4 gridColor = lerp(_GridFogColor, _GridColor, fogStrength);
                gridColor.rgb = (1 / (1 + gridColor.a)) * (outColor.rgb + gridColor.a * gridColor.rgb);
                return float4(lerp(gridColor, outColor, lineWeight).rgb, outColor.a);
            }
            ENDCG
        }
    }
    Fallback Off
}