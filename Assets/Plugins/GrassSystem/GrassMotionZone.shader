Shader "Hidden/GrassMotionZone"
{
	Properties
	{
		_Intensity("Intensity", float) = 0
		_Origin("Origin", Vector) = (0, 0, 0, 0)
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float2 normal : TEXCOORD1;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float4 direction : TEXCOORD1;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float2 _RenderTargetSize;
	float4 _WorldSpaceCameraDir;

	float _Intensity;
	float4 _Origin;

	v2f vert(appdata_full v)
	{
		v2f o;

		float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
		float4 currentWorldPos = worldPos - float4(_WorldSpaceCameraPos + _WorldSpaceCameraDir.xyz * _WorldSpaceCameraDir.w, 1) + float4(32,0,32,1);
		o.vertex.xy = (currentWorldPos.xz / _RenderTargetSize.xy);

#if UNITY_UV_STARTS_AT_TOP
		o.vertex.y = 1 - o.vertex.y;
#endif

		o.vertex.xy = o.vertex.xy * 2 - 1;
		o.vertex.z = 1;
		o.vertex.w = 1;

		float4 direction = (worldPos - _Origin);
		direction.y = 0;
		float normalizedLength = dot(float3(0, 1, 0), v.normal);
		direction = normalize(direction) * normalizedLength;

		o.direction = direction * _Intensity;

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = i.direction.xyzz * 0.5 + 0.5;
	col.w = 1;
	return col;
	}
		ENDCG
	}
	}
}