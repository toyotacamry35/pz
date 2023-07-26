Shader "AwesomeTechnologies/Vegetation/TightenAlpha"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Background("Texture", 2D) = "white" {}
	}
		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _MainTex;
	sampler2D _Background;
	float4 _MainTex_TexelSize;

	half4 frag(v2f i) : SV_Target
	{
		half4 background = tex2D(_Background, i.uv);
		half4 v = tex2D(_MainTex, i.uv);
		half t = 0;
		for (int x = -1; x <= 1; ++x)
		{
			for (int y = -1; y <= 1; ++y)
			{
				t += tex2D(_MainTex, i.uv + half2(x,y) * _MainTex_TexelSize.xy).a;
			}
		}
		t /= 9.0;
		v.a = min(t, v.a) * 3;  // maybe expose the amplitude used here?
		v.a = clamp(v.a, 0, 1);
		v.rgb = lerp(background.rgb, v.rgb, v.a);
#ifndef UNITY_COLORSPACE_GAMMA
			v.rgb = LinearToGammaSpace(v.rgb);
#endif
		return v;
	}
		ENDCG
	}
	}
}