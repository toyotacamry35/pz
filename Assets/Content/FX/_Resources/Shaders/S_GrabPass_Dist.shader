// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Per pixel bumped refraction.
// Uses a normal map to distort the image behind, and
// an additional texture to tint the color.

Shader "Colony_FX/Basic/S_GrabPass_Dist" {
Properties {
	_BumpAmt  ("Distortion", range (0,128)) = 10
	[HDR] _Color ("Emissive Color", Color) = (0,0,0)
	_MainTex ("Tint Color (RGB)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_fresnelPow ("fresnelPow", Range(0, 10)) = 1
}

Category {

	// We must be transparent, so other objects are drawn before this one.
	Tags { "Queue"="Transparent+400" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha


	SubShader {

		// This pass grabs the screen behind the object into a texture.
		// We can access the result in the next pass as _GrabTexture
		GrabPass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
		}
		
		
		// Main pass: Take the texture grabbed above and use the bumpmap to perturb it
		// on to the screen
		Pass {
			Name "BASE"
			Tags { "LightMode" = "Always" }			
			Cull Off
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#include "UnityCG.cginc"


struct appdata_t {
	float4 vertex : POSITION;
	float4 texcoord: TEXCOORD0;
	float4 vertexColor : COLOR;
	float3 normal : NORMAL;
};

struct v2f {
	float4 vertex : SV_POSITION;
	float4 uvgrab : TEXCOORD0;
	float2 uvbump : TEXCOORD1;
	float2 uvmain : TEXCOORD2;
	float amount : TEXCOORD3;
	float4 projPos : TEXCOORD4;
	float3 normalDir : TEXCOORD5;
	float4 posWorld : TEXCOORD6;
	float4 vertexColor : COLOR;
	UNITY_FOG_COORDS(7)
};

float _BumpAmt;
float4 _BumpMap_ST;
float4 _MainTex_ST;
float _fresnelPow;

v2f vert (appdata_t v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.vertexColor = v.vertexColor;
	#if UNITY_UV_STARTS_AT_TOP
	float scale = -1.0;
	#else
	float scale = 1.0;
	#endif
	o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
	o.uvgrab.zw = o.vertex.zw;
	o.uvbump = TRANSFORM_TEX( v.texcoord, _BumpMap );
	o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
	o.amount = v.texcoord.z;
	o.normalDir = UnityObjectToWorldNormal(v.normal);
	o.posWorld = mul(unity_ObjectToWorld, v.vertex);
	o.vertex = UnityObjectToClipPos( v.vertex );
	UNITY_TRANSFER_FOG(o,o.vertex);
	o.projPos = ComputeScreenPos (o.vertex);
    COMPUTE_EYEDEPTH(o.projPos.z);
	return o;
}

sampler2D _GrabTexture;
float4 _GrabTexture_TexelSize;
sampler2D _BumpMap;
sampler2D _MainTex;
half4 _Color;
uniform sampler2D _CameraDepthTexture;

half4 frag (v2f i, float facing : VFACE) : SV_Target
{
	//values needed for fresnel
	float faceSign = ( facing >= 0 ? 1 : -1 );
	i.normalDir = normalize(i.normalDir);
	i.normalDir *= faceSign;
	float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
	float3 normalDirection = i.normalDir;
	//values needed for depth fade 
	float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
	float partZ = max(0,i.projPos.z - _ProjectionParams.g);
	//mask
	half4 mask = tex2D(_MainTex, i.uvmain);
	// calculate perturbed coordinates
	half2 bump = UnpackNormal(tex2D( _BumpMap, i.uvbump  )).rg; // we could optimize this by just reading the x & y without reconstructing the Z
	float2 offset = bump * _BumpAmt * _GrabTexture_TexelSize.xy * mask * i.amount * saturate((sceneZ-partZ)/20) * (1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_fresnelPow));	
	#ifdef UNITY_Z_0_FAR_FROM_CLIPSPACE //to handle recent standard asset package on older version of unity (before 5.5)
		i.uvgrab.xy = offset * UNITY_Z_0_FAR_FROM_CLIPSPACE(i.uvgrab.z) + i.uvgrab.xy;
	#else
		i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
	#endif
	
	half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
	
	col = clamp(col, 0, 1.02);
	
	UNITY_APPLY_FOG_COLOR(i.fogCoord, col, unity_FogColor);
	return col;
}
ENDCG
		}
	}

	// ------------------------------------------------------------------
	// Fallback for older cards and Unity non-Pro

	SubShader {
		Blend DstColor Zero
		Pass {
			Name "BASE"
			SetTexture [_MainTex] {	combine texture }
		}
	}
}

}
