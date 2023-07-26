Shader "AwesomeTechnologies/Billboards/SingleBillboard"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Bump ("Bump", 2D) = "white" {}
		_HueVariation ("Hue variation", Color) = (1,1,1,.5)
		_Cutoff("Cutoff" , Range(0,1)) = .5
		_YRotation("Billboard Y Rotation" , Range(0,1)) = 0
		_XTurnFix("Billboard X Rotation fix" , Range(0,10)) = .1
		_CullDistance("Near cull distance",Float) = 0
		_InRow("Frames in row", Int) = 8
		_InCol("Frames in column", Int) = 8
		_CameraPosition("Camera position",Vector) = (0,0,0,0)
		
		[KeywordEnum(OFF, ON)] AT_SINGLEROW ("Single in row", Float) = 0
		[KeywordEnum(OFF, ON)] AT_SINGLECOLUMN ("Single in column", Float) = 0
		[KeywordEnum(ON, OFF)] AT_HUE_VARIATION ("Use SpeedTree HUE variation", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="TransparentCutout" "Queue"="Geometry" "DisableBatching"="True" "IgnoreProjector"="True"}
		LOD 200
	
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert noforwardadd alphatest:_Cutoff vertex:vert addshadow		
		#pragma multi_compile AT_HUE_VARIATION_ON AT_HUE_VARIATION_OFF
		
		#include "a_tree.cginc"
		
		ENDCG
	}
	FallBack "Diffuse"
}
