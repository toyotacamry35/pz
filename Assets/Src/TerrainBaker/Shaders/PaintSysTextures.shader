Shader "Hidden/PaintSysTextures"
{
	Properties
	{
	}
	SubShader
	{
		Tags 
		{ 
			"RenderType" = "Transparent"
			"Queue" = "Geometry+11"
		}
		LOD 100


		CGPROGRAM
		#pragma surface surf StandardSpecular

		#include "UnityCG.cginc"

		#define MAX_SYS_TEXTURES 4

		sampler2D SysTexture0;
		sampler2D SysTexture1;
		sampler2D SysTexture2;
		sampler2D SysTexture3;

		sampler2D Alphamap0;
		sampler2D Alphamap1;
		sampler2D Alphamap2;
		sampler2D Alphamap3;

		float4 AlphamapSelectMask0;
		float4 AlphamapSelectMask1;
		float4 AlphamapSelectMask2;
		float4 AlphamapSelectMask3;

		float2 InvTerrainSize;

		SamplerState readtex_point_clamp_sampler;

		struct Input {
			float3 worldPos;
		};

		float GetAlpha(sampler2D alphamap, float4 selectMask, float2 uv)
		{
			float4 amap = tex2D(alphamap, uv);
			return dot(amap, selectMask) > 0 ? 1 : 0;
		}		

		void surf(Input IN, inout SurfaceOutputStandardSpecular o)
		{
			float2 alphamapUV = frac(IN.worldPos.xz * InvTerrainSize);
			float2 textureUV = IN.worldPos.xz * (1.0 / 4.0);

			float alphas[MAX_SYS_TEXTURES];
			alphas[0] = GetAlpha(Alphamap0, AlphamapSelectMask0, alphamapUV);
			alphas[1] = GetAlpha(Alphamap1, AlphamapSelectMask1, alphamapUV);
			alphas[2] = GetAlpha(Alphamap2, AlphamapSelectMask2, alphamapUV);
			alphas[3] = GetAlpha(Alphamap3, AlphamapSelectMask3, alphamapUV);

			float4 colors[MAX_SYS_TEXTURES];
			colors[0] = tex2D(SysTexture0, textureUV);
			colors[1] = tex2D(SysTexture1, textureUV);
			colors[2] = tex2D(SysTexture2, textureUV);
			colors[3] = tex2D(SysTexture3, textureUV);


			float3 color = 0;
			float alpha = 0;
			
			[unroll]
			for (int i = 0; i < MAX_SYS_TEXTURES; i++)
			{
				float a = colors[i].a * alphas[i];
				color += colors[i].rgb * a;
				alpha += a;
			}

			clip(alpha - 0.5);

			o.Emission = color;
			o.Albedo = 0;
			o.Specular = 0;
			o.Normal = float3(0, 1, 0);
			o.Smoothness = 1;
			o.Occlusion = 1;
			o.Alpha = 1;
		}

		ENDCG
	}
	FallBack Off
}
