
Shader "ASkybox/ASkyRendering"
{
	Properties
	{
	 _Background ("BG", 2D) = "black" {}
	 _BackgroundSPSR ("BG Single Pass", 2D) = "black" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		//Blend One OneMinusSrcAlpha
		Tags { "Queue"="Background" "RenderType"="Background" }
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _Clouds;
			sampler2D _CloudsSPSR;
			sampler2D _Sky;
			sampler2D _SkySPSR;
			sampler2D _Background;
			sampler2D _BackgroundSPSR;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 cloud;
				fixed4 sky;
				fixed4 bg;

				#if UNITY_SINGLE_PASS_STEREO
    			if (unity_StereoEyeIndex == 0)
    			{
    				cloud = tex2D(_Clouds, i.uv);
    				sky = tex2D(_Sky, i.uv);
    				bg = tex2D(_Background, i.uv);
    			}else{
    				cloud = tex2D(_CloudsSPSR, i.uv);
    				sky = tex2D(_SkySPSR, i.uv);}
    				bg = tex2D(_BackgroundSPSR, i.uv);
    			#else
    				cloud = tex2D(_Clouds, i.uv);
    				sky = tex2D(_Sky, i.uv);
    				bg = tex2D(_Background, i.uv);
    			#endif

				cloud = tex2D(_Clouds, i.uv);
				//sat = tex2D(_Satellites, i.uv);
				bg = tex2D(_Background, i.uv);

				fixed blending = lerp(0,10,cloud.a);
				blending = clamp(blending,0,1);

				float4 skyAndBg = lerp(sky,bg,bg.a);
				float4 final = lerp(skyAndBg,cloud,blending);

				return final;
			}
			ENDCG
		}
	}
}
