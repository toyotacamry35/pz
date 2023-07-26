Shader "Hidden/DS_HazeLightVolume"
{
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 300

		Pass // 0 - Point light.
		{
			Cull Front
			ZTest Always
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment DSFP_PointVolume
			#pragma multi_compile __ SHADOWS_CUBE
			#pragma multi_compile __ POINT_COOKIE
			#pragma multi_compile __ DENSITY_TEXTURE
			#pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

			#define POINT
			#define SHADOWS_NATIVE
            #define DS_FALLOFF_UNITY
			#include "DS_LightVolumeLib.cginc"
			ENDCG
		}

		Pass // 1 - Spot light (Inside the cone)
		{
			Cull Front
			ZTest Always
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment DSFP_SpotVolumeInterior
			#pragma multi_compile __ SPOT_COOKIE			
			#pragma multi_compile __ SHADOWS_DEPTH			
			#pragma multi_compile __ DENSITY_TEXTURE
			#pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

			#define SPOT
			#define SHADOWS_NATIVE
            #define DS_FALLOFF_UNITY
			#include "DS_LightVolumeLib.cginc"
			ENDCG
		}

		Pass // 2 - Spot light (Outside the cone)
		{
			Cull Front
			ZTest Always
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment DSFP_SpotVolumeExterior
			#pragma multi_compile __ SPOT_COOKIE
			#pragma multi_compile __ SHADOWS_DEPTH
			#pragma multi_compile __ DENSITY_TEXTURE
			#pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

			#define SPOT
			#define SHADOWS_NATIVE
            #define DS_FALLOFF_UNITY
			#include "DS_LightVolumeLib.cginc"
			ENDCG
		}

		Pass // 3 - Point light, 1/4.
		{
			Cull Front
			ZTest Always
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment DSFP_PointVolume
			#pragma multi_compile __ SHADOWS_CUBE
			#pragma multi_compile __ POINT_COOKIE
			#pragma multi_compile __ DENSITY_TEXTURE
			#pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

			#define POINT
			#define SHADOWS_NATIVE
			#define kDownsampleFactor 4
            #define DS_FALLOFF_UNITY
			#include "DS_LightVolumeLib.cginc"
			ENDCG
		}

		Pass // 4 - Spot light (Inside the cone), 1/4
		{
			Cull Front
			ZTest Always
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment DSFP_SpotVolumeInterior
			#pragma multi_compile __ SPOT_COOKIE			
			#pragma multi_compile __ SHADOWS_DEPTH			
			#pragma multi_compile __ DENSITY_TEXTURE
			#pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

			#define SPOT
			#define SHADOWS_NATIVE
            #define DS_FALLOFF_UNITY
			#define kDownsampleFactor 4
			#include "DS_LightVolumeLib.cginc"
			ENDCG
		}

		Pass // 5 - Spot light (Outside the cone), 1/4
		{
			Cull Front
			ZTest Always
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment DSFP_SpotVolumeExterior
			#pragma multi_compile __ SPOT_COOKIE
			#pragma multi_compile __ SHADOWS_DEPTH
			#pragma multi_compile __ DENSITY_TEXTURE
			#pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

			#define SPOT
			#define SHADOWS_NATIVE
            #define DS_FALLOFF_UNITY
			#define kDownsampleFactor 4
			#include "DS_LightVolumeLib.cginc"
			ENDCG
		}

        // EXPONENTIAL FALLOFF
        Pass // 6 - Point light (Exp).
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_PointVolume
            #pragma multi_compile __ SHADOWS_CUBE
            #pragma multi_compile __ POINT_COOKIE
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define POINT
            #define SHADOWS_NATIVE
            #include "DS_LightVolumeLib.cginc"
            ENDCG
	}

        Pass // 7 - Spot light (Inside the cone) (Exp)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_SpotVolumeInterior
            #pragma multi_compile __ SPOT_COOKIE			
            #pragma multi_compile __ SHADOWS_DEPTH			
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define SPOT
            #define SHADOWS_NATIVE
            #include "DS_LightVolumeLib.cginc"
            ENDCG
}

        Pass // 8 - Spot light (Outside the cone) (Exp)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_SpotVolumeExterior
            #pragma multi_compile __ SPOT_COOKIE
            #pragma multi_compile __ SHADOWS_DEPTH
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define SPOT
            #define SHADOWS_NATIVE
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        Pass // 9 - Point light, 1/4 (Exp)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_PointVolume
            #pragma multi_compile __ SHADOWS_CUBE
            #pragma multi_compile __ POINT_COOKIE
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define POINT
            #define SHADOWS_NATIVE
            #define kDownsampleFactor 4
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        Pass // 10 - Spot light (Inside the cone), 1/4 (Exp)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_SpotVolumeInterior
            #pragma multi_compile __ SPOT_COOKIE			
            #pragma multi_compile __ SHADOWS_DEPTH			
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define SPOT
            #define SHADOWS_NATIVE
            #define kDownsampleFactor 4
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        Pass // 11 - Spot light (Outside the cone), 1/4 (Exp)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_SpotVolumeExterior
            #pragma multi_compile __ SPOT_COOKIE
            #pragma multi_compile __ SHADOWS_DEPTH
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define SPOT
            #define SHADOWS_NATIVE
            #define kDownsampleFactor 4
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        // USE FOG
        Pass // 12 - Point light (Fog).
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_PointVolume
            #pragma multi_compile __ SHADOWS_CUBE
            #pragma multi_compile __ POINT_COOKIE
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define POINT
            #define SHADOWS_NATIVE
            #define DS_FALLOFF_UNITY
            #define DS_LIGHT_USE_FOG
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        Pass // 13 - Spot light (Inside the cone) (Fog)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_SpotVolumeInterior
            #pragma multi_compile __ SPOT_COOKIE			
            #pragma multi_compile __ SHADOWS_DEPTH			
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define SPOT
            #define SHADOWS_NATIVE
            #define DS_FALLOFF_UNITY
            #define DS_LIGHT_USE_FOG
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        Pass // 14 - Spot light (Outside the cone) (Fog)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_SpotVolumeExterior
            #pragma multi_compile __ SPOT_COOKIE
            #pragma multi_compile __ SHADOWS_DEPTH
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define SPOT
            #define SHADOWS_NATIVE
            #define DS_FALLOFF_UNITY
            #define DS_LIGHT_USE_FOG
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        Pass // 15 - Point light, 1/4 (Fog)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_PointVolume
            #pragma multi_compile __ SHADOWS_CUBE
            #pragma multi_compile __ POINT_COOKIE
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define POINT
            #define SHADOWS_NATIVE
            #define kDownsampleFactor 4
            #define DS_FALLOFF_UNITY
            #define DS_LIGHT_USE_FOG
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        Pass // 16 - Spot light (Inside the cone), 1/4 (Fog)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_SpotVolumeInterior
            #pragma multi_compile __ SPOT_COOKIE			
            #pragma multi_compile __ SHADOWS_DEPTH			
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define SPOT
            #define SHADOWS_NATIVE
            #define DS_FALLOFF_UNITY
            #define kDownsampleFactor 4
            #define DS_LIGHT_USE_FOG
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        Pass // 17 - Spot light (Outside the cone), 1/4 (Fog)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_SpotVolumeExterior
            #pragma multi_compile __ SPOT_COOKIE
            #pragma multi_compile __ SHADOWS_DEPTH
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define SPOT
            #define SHADOWS_NATIVE
            #define DS_FALLOFF_UNITY
            #define kDownsampleFactor 4
            #define DS_LIGHT_USE_FOG
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        // EXPONENTIAL FALLOFF
        Pass // 18 - Point light (Exp) (Fog)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_PointVolume
            #pragma multi_compile __ SHADOWS_CUBE
            #pragma multi_compile __ POINT_COOKIE
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define POINT
            #define SHADOWS_NATIVE
            #define DS_LIGHT_USE_FOG
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        Pass // 19 - Spot light (Inside the cone) (Exp) (Fog)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_SpotVolumeInterior
            #pragma multi_compile __ SPOT_COOKIE			
            #pragma multi_compile __ SHADOWS_DEPTH			
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define SPOT
            #define SHADOWS_NATIVE
            #define DS_LIGHT_USE_FOG
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        Pass // 20 - Spot light (Outside the cone) (Exp) (Fog)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_SpotVolumeExterior
            #pragma multi_compile __ SPOT_COOKIE
            #pragma multi_compile __ SHADOWS_DEPTH
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define SPOT
            #define SHADOWS_NATIVE
            #define DS_LIGHT_USE_FOG
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        Pass // 21 - Point light, 1/4 (Exp) (Fog)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_PointVolume
            #pragma multi_compile __ SHADOWS_CUBE
            #pragma multi_compile __ POINT_COOKIE
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define POINT
            #define SHADOWS_NATIVE
            #define kDownsampleFactor 4
            #define DS_LIGHT_USE_FOG
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        Pass // 22 - Spot light (Inside the cone), 1/4 (Exp) (Fog)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_SpotVolumeInterior
            #pragma multi_compile __ SPOT_COOKIE			
            #pragma multi_compile __ SHADOWS_DEPTH			
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define SPOT
            #define SHADOWS_NATIVE
            #define kDownsampleFactor 4
            #define DS_LIGHT_USE_FOG
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }

        Pass // 23 - Spot light (Outside the cone), 1/4 (Exp) (Fog)
        {
            Cull Front
            ZTest Always
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment DSFP_SpotVolumeExterior
            #pragma multi_compile __ SPOT_COOKIE
            #pragma multi_compile __ SHADOWS_DEPTH
            #pragma multi_compile __ DENSITY_TEXTURE
            #pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

            #define SPOT
            #define SHADOWS_NATIVE
            #define kDownsampleFactor 4
            #define DS_LIGHT_USE_FOG
            #include "DS_LightVolumeLib.cginc"
            ENDCG
        }
    }
}