// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Planet/Rings"
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "black" {}
	}
	SubShader 
	{
		Tags
		{
			"Queue"="Transparent+1"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"LightMode"="ForwardBase"
		}

		ZWrite On
		Blend One One
		//Blend SrcAlpha OneMinusSrcAlpha 
	
		Pass 
		{  
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile MOBILE_ON MOBILE_OFF
		
			#include "UnityCG.cginc"
			#include "ASkyPlanetShared.cginc"
			
			float4 _LightColor0;

			struct v2f 
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				float3 posWorld : COLOR0;
			};

			//float4 _MainTex_ST;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}
			
			fixed4 frag (v2f input) : SV_Target
			{
				float w = 0;
    			/*
				// SHADOW HANDLING
				#if MOBILE_OFF
				
				w = getShadow(input.posWorld, v3Translate, fInnerRadius);
				if (shadowNumber > 0)
				{
					w = max(w, getShadow(input.posWorld, planetShadowPos0, planetShadowSca0));
					if (shadowNumber > 1)
					{
						w = max(w, getShadow(input.posWorld, planetShadowPos1, planetShadowSca1));
						if (shadowNumber > 2)
						{
							w = max(w, getShadow(input.posWorld, planetShadowPos2, planetShadowSca2));
							if (shadowNumber > 3)
							{
								w = max(w, getShadow(input.posWorld, planetShadowPos3, planetShadowSca3));
								if (shadowNumber > 4)
								{
									w = max(w, getShadow(input.posWorld, planetShadowPos4, planetShadowSca4));
									if (shadowNumber > 5)
									{
										w = max(w, getShadow(input.posWorld, planetShadowPos5, planetShadowSca5));
										if (shadowNumber > 6)
										{
											w = max(w, getShadow(input.posWorld, planetShadowPos6, planetShadowSca6));
											if (shadowNumber > 7)
											{
												w = max(w, getShadow(input.posWorld, planetShadowPos7, planetShadowSca7));
												if (shadowNumber > 8)
												{
													w = max(w, getShadow(input.posWorld, planetShadowPos8, planetShadowSca8));
													if (shadowNumber > 9)
													{
														w = max(w, getShadow(input.posWorld, planetShadowPos9, planetShadowSca9));
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				_LightColor0.rgb *= (1 - w);

				#endif
				*/
				fixed4 col = tex2D(_MainTex, input.texcoord);
				col.a = col.r;
				col.rgb *= _LightColor0.rgb;
				return col;
			}
		ENDCG
		}
	}
}