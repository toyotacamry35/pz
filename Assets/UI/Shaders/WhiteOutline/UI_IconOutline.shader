Shader "UI/IconOutline"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_OpacityStrength("Opacity Strength", Float) = 1
		_Outline("Outline", Float) = 0
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		_OutlineOpacity("Outline Opacity", Float) = 10
		
		_BackgroundColor("Background Color", Color) = (0,0,0,0)
		_BackgroundOpacity("BackgroundOpacity", Float) = 10
			
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Default"
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile _ PIXELSNAP_ON
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			float _Outline;
			float _OpacityStrength;
			fixed4 _BackgroundColor;
			float _BackgroundOpacity;
			fixed4 _OutlineColor;
			float _OutlineOpacity;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif
				return OUT;
			}

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float2 screenPixelSize;

			half4 flerp(half4 a, half4 b) //lerp a over b
			{
				half4 c = 0.0;
				c.rgb = lerp(b.rgb * b.a, a.rgb, a.a) / max( 0.0001, lerp(b.a, 1.0, a.a));
				c.a = lerp(b.a, 1.0, a.a);

				return c;
			}
			 
			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = tex2D(_MainTex, IN.texcoord) ;
				color.a = pow( color.a, 1.0 / _OpacityStrength);

				half4 backgroundPlate = half4( _BackgroundColor.rgb,  saturate(color.a * _BackgroundOpacity));
				
				half4 outColor = 0.0;
				
				outColor = flerp(color, backgroundPlate);
				
				if (_Outline > 0) {
					_MainTex_TexelSize *= _Outline;
					half4 contour = 0;

					half4 contourUp = tex2D(_MainTex, IN.texcoord - fixed2(0, _MainTex_TexelSize.y));
					half4 contourDown = tex2D(_MainTex, IN.texcoord + fixed2(0, _MainTex_TexelSize.y));
					half4 contourLeft = tex2D(_MainTex, IN.texcoord - fixed2(_MainTex_TexelSize.x, 0));
					half4 contourRight = tex2D(_MainTex, IN.texcoord + fixed2(_MainTex_TexelSize.x, 0));

					contour.a = max(contourRight.a, max(contourLeft.a, max(contourUp.a, contourDown.a)));
					contour.a = saturate(contour.a * _OutlineOpacity);
					contour.rgb = _OutlineColor.rgb;

					outColor = flerp( outColor, contour);
				}
				
				outColor.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (outColor.a - 0.001);
				#endif

				return outColor;
			}
		ENDCG
		}
	}
}
