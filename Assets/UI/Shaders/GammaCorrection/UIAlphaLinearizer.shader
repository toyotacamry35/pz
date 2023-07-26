Shader "UI/DefaultGamma"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		/*ase_props*/
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
			/*ase_tags*/
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

		/*ase_pass*/
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
			
			/*ase_pragma*/
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				/*ase_vdata:p=p;uv0=tc0.xy;c=c*/
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				/*ase_interp(2,7):sp=sp.xyzw;uv0=tc0.xy;c=c*/
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			/*ase_globals*/
			


			v2f vert( appdata_t IN /*ase_vert_input*/ )
			{
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				/*ase_vert_code:IN=appdata_t;OUT=v2f*/
				
				OUT.worldPosition.xyz += /*ase_vert_out:Offset;Float3*/ float3( 0, 0, 0 ) /*end*/;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0) * float2(-1,1) * OUT.vertex.w;
				#endif
				
				OUT.color = IN.color;
				return OUT;
			}



			fixed4 frag(v2f IN /*ase_frag_input*/ ) : SV_Target
			{
				half4 color = tex2D(_MainTex, IN.texcoord);

				float alpha = pow(color.a + 0.01, 1.6) / 0.9;
				alpha *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				#ifdef UNITY_UI_ALPHACLIP
				clip (alpha - 0.001);
				#endif

				return float4(color.rgb, alpha) * IN.color;
			}
		ENDCG
		}
	}
}
