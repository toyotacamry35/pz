Shader "Unlit/UnlitTest"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
 
		Pass
		{
		CGPROGRAM
  
		#pragma vertex vert
		#pragma fragment frag
		// make fog work
		#pragma multi_compile_fog
  
		#include "UnityCG.cginc"
 
		struct appdata
		{
			float4 vertex : POSITION;
			float4 col:COLOR;
		};
 
		struct v2f
		{
			float4 vertex : SV_POSITION;
			float4 col:COLOR;
		};
 
		sampler2D _MainTex;
		float4 _MainTex_ST;
  
		v2f vert (appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.col = v.col;
			return o;
		}
  
		fixed4 frag (v2f i) : SV_Target
		{
			
			return i.col;
		}
		ENDCG
		}
	}
}