Shader "Hidden/Decalicious Layer ID"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		ZWrite Off
		ZTest Equal
		Blend One One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
	
			#include "UnityCG.cginc"

			int _LayerID;

			float4 vert(float4 vertex : POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}

			int4 frag(float4 vertex : SV_POSITION) : SV_Target
			{
				return (1 <<_LayerID);
			}
			ENDCG
		}
	}
}
