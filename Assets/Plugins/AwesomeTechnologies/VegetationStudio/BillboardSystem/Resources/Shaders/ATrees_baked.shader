Shader "AwesomeTechnologies/Billboards/BakedGroupBillboards"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Bump ("Bump", 2D) = "white" {}
		_HueVariation ("Hue variation", Color) = (1,1,1,.5)
		_Cutoff("Cutoff" , Range(0,1)) = .5
		_Brightness("Brightness" , Range(0,5)) = 1
		_MipmapBias("Mipmap bias" , Range(-3,0)) = -2
//		_CullDistance("Near cull distance",Float) = 0
//		_FarCullDistance("Near cull distance",Float) = 0
		_InRow("Frames in row", Int) = 8
		_InCol("Frames in column", Int) = 8
//		_CameraPosition("Camera position",Vector) = (0,0,0,0)

		[KeywordEnum(ON, OFF)] AT_HUE_VARIATION ("Use SpeedTree HUE variation", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="TransparentCutout" "Queue"="Alphatest" "DisableBatching"="True" "IgnoreProjector"="True"}
		LOD 200
		
		CGPROGRAM

		#pragma surface surf Lambert noforwardadd alphatest:_Cutoff vertex:vert addshadow
		#pragma multi_compile AT_HUE_VARIATION_ON AT_HUE_VARIATION_OFF
		
		#pragma target 3.0
		
		#define AT_CHUNK
				
		#include "a_tree_baked.cginc"
		

		ENDCG
	}
	FallBack "Diffuse"
}
