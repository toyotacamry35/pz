// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
//  Copyright (c) 2015 José Guerreiro. All rights reserved.
//
//  MIT license, see http://www.opensource.org/licenses/mit-license.php
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
*/

Shader "Hidden/OutlineEffect" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		
	}
	SubShader 
	{
		Pass
		{
			Tags{ "RenderType" = "Opaque" }
			LOD 200
			ZTest Always
			ZWrite Off
			Cull Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _OutlineSource;
			float4 _TestVector;

			struct v2f
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_img v)
			{
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;

				return o;
			}

			float _LineThicknessX;
			float _LineThicknessY;
			int _FlipY;
			uniform float4 _MainTex_TexelSize;

			half4 frag(v2f input) : COLOR
			{
				float2 uv = input.uv;
				if (_FlipY == 1)
					uv.y = uv.y;
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					uv.y = 1 - uv.y;
				#endif

				//half4 originalPixel = tex2D(_MainTex,input.uv, UnityStereoScreenSpaceUVAdjust(input.uv, _MainTex_ST));
				half4 outlineSource = tex2D(_OutlineSource, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST));

				const float h = .95f;

				half4 sample1 = tex2D(_OutlineSource, uv + float2(_LineThicknessX,0.0));
				half4 sample2 = tex2D(_OutlineSource, uv + float2(-_LineThicknessX,0.0));
				half4 sample3 = tex2D(_OutlineSource, uv + float2(.0,_LineThicknessY));
				half4 sample4 = tex2D(_OutlineSource, uv + float2(.0,-_LineThicknessY));

				bool red = sample1.r > h || sample2.r > h || sample3.r > h || sample4.r > h;
				bool green = sample1.g > h || sample2.g > h || sample3.g > h || sample4.g > h;
				bool blue = sample1.b > h || sample2.b > h || sample3.b > h || sample4.b > h;
				 
				if ((red && blue) || (green && blue) || (red && green))
					return float4(0,0,0,0);
				else
					return outlineSource;
			}

			ENDCG
		}

		Pass
		{
			Tags { "RenderType"="Opaque" }
			LOD 200
			ZTest Always
			ZWrite Off
			Cull Off
			
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _OutlineSource;
			float4 _TestVector;

			struct v2f {
			   float4 position : SV_POSITION;
			   float2 uv : TEXCOORD0;
			};
			
			v2f vert(appdata_img v)
			{
			   	v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				
			   	return o;
			}

			sampler2D _ShieldPattern;
			sampler2D _ShieldPatternWaves;
			float _LineThicknessX;
			float _LineThicknessY;
			float _LineIntensity;
			half4 _LineColor1;
			half4 _LineColor2;
			half4 _LineColor3;
			int _FlipY;
			int _Dark;
			float _FillAmount;
			int _CornerOutlines;
			uniform float4 _MainTex_TexelSize;

			half4 frag (v2f input) : COLOR
			{	
				float2 uv = input.uv;
				if (_FlipY == 1)
					uv.y = 1 - uv.y;
				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						uv.y = 1 - uv.y;
				#endif

				half4 originalPixel = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(input.uv, _MainTex_ST));
				half4 outlineSource = tex2D(_OutlineSource, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST));
								
				const float h = .95f;
				half4 outline = 0;
				bool hasOutline = false;

				half4 sample1 = tex2D(_OutlineSource, uv + float2(_LineThicknessX,0.0));
				half4 sample2 = tex2D(_OutlineSource, uv + float2(-_LineThicknessX,0.0));
				half4 sample3 = tex2D(_OutlineSource, uv + float2(.0,_LineThicknessY));
				half4 sample4 = tex2D(_OutlineSource, uv + float2(.0,-_LineThicknessY));
				
				bool outside = outlineSource.a < h;
				bool outsideDark = outside && _Dark;

				//float4 ShieldSpeed84 = (_Time * _TestVector.x);
				//float2 appendResult46 = (float2(1, ShieldSpeed84.x));
				//float2 uv_TexCoord41 = uv * float2(5, 5) +appendResult46;
				//float4 ShieldPattern17 = tex2D(_ShieldPattern, uv_TexCoord41);
				//float2 appendResult91 = (float2(1, (1.0 - (ShieldSpeed84 / 5.0)).x));
				//float2 uv_TexCoord87 = uv * float2(1, 1) + appendResult91;
				//float waves94 = tex2D(_ShieldPatternWaves, uv_TexCoord87).r;
				//float quadro = ShieldPattern17.x * clamp(frac(_Time.w), 0, 1) + ShieldPattern17.y * clamp(1 - frac(_Time.w), 0, 1);
				//float frequency = (sin(_Time.w * _TestVector.x + _ScreenParams.z / _TestVector.y + _ScreenParams.w / _TestVector.y)) * _TestVector.w + _TestVector.z;
				//float intensity = (_LineIntensity - quadro) * frequency;// lerp((_LineIntensity - ShieldPattern17) * waves94, _LineIntensity * waves94, frequency);
				//_LineIntensity *= frequency;
				//float intensity = _LineIntensity * (sin(_Time.w * _TestVector.x + _ScreenParams.x / _TestVector.y + _ScreenParams.y / _TestVector.y) + _TestVector.z) * _TestVector.w;
				if (_CornerOutlines)
				{
					// TODO: Conditional compile
					half4 sample5 = tex2D(_OutlineSource, uv + float2(_LineThicknessX, _LineThicknessY));
					half4 sample6 = tex2D(_OutlineSource, uv + float2(-_LineThicknessX, -_LineThicknessY));
					half4 sample7 = tex2D(_OutlineSource, uv + float2(_LineThicknessX, -_LineThicknessY));
					half4 sample8 = tex2D(_OutlineSource, uv + float2(-_LineThicknessX, _LineThicknessY));

					if (sample1.r > h || sample2.r > h || sample3.r > h || sample4.r > h ||
						sample5.r > h || sample6.r > h || sample7.r > h || sample8.r > h)
					{
						outline = _LineColor1 * _LineIntensity * _LineColor1.a;
						if (outsideDark)
							originalPixel *= 1 - _LineColor1.a;
						hasOutline = true;
					}
					else if (sample1.g > h || sample2.g > h || sample3.g > h || sample4.g > h ||
						sample5.g > h || sample6.g > h || sample7.g > h || sample8.g > h)
					{
						outline = _LineColor2 * _LineIntensity * _LineColor2.a;
						if (outsideDark)
							originalPixel *= 1 - _LineColor2.a;
						hasOutline = true;
					}
					else if (sample1.b > h || sample2.b > h || sample3.b > h || sample4.b > h ||
						sample5.b > h || sample6.b > h || sample7.b > h || sample8.b > h)
					{
						outline = _LineColor3 * _LineIntensity * _LineColor3.a;
						if (outsideDark)
							originalPixel *= 1 - _LineColor3.a;
						hasOutline = true;
					}

					if (!outside)
						outline *= _FillAmount;// *intensity;
				}
				else
				{
					if (sample1.r > h || sample2.r > h || sample3.r > h || sample4.r > h)
					{
						outline = _LineColor1 * _LineIntensity * _LineColor1.a;
						if (outsideDark)
							originalPixel *= 1 - _LineColor1.a;
						hasOutline = true;
					}
					else if (sample1.g > h || sample2.g > h || sample3.g > h || sample4.g > h)
					{
						outline = _LineColor2 * _LineIntensity * _LineColor2.a;
						if (outsideDark)
							originalPixel *= 1 - _LineColor2.a;
						hasOutline = true;
					}
					else if (sample1.b > h || sample2.b > h || sample3.b > h || sample4.b > h)
					{
						outline = _LineColor3 * _LineIntensity * _LineColor3.a;
						if (outsideDark)
							originalPixel *= 1 - _LineColor3.a;
						hasOutline = true;
					}

					if (!outside)
						outline *= _FillAmount;// *intensity;
				}					
					
				//return outlineSource;		
				if (hasOutline)
					return lerp(originalPixel + outline, outline, _FillAmount);// *intensity);
				else
					return originalPixel;
			}
			
			ENDCG
		}
	} 

	FallBack "Diffuse"
}