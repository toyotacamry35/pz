Shader "Hidden/ReindexMaterialMap"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "TerrainBakerUtils.cginc"


			struct appdata
			{
				float3 vertex : POSITION;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float _RemapIndeces[32];

			Texture2D _Materials;

			SamplerState layer_point_clamp_sampler;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex.xyz = v.vertex;
				o.vertex.w = 1.0;
				o.uv = 0.5 + v.vertex*float2(0.5, -0.5);
				return o;
			}


			fixed4 frag(v2f inp) : SV_Target
			{
				const float maxIndex = 31.5 / 255.0;

				float4 mat = _Materials.SampleLevel(layer_point_clamp_sampler, inp.uv, 0);

				int4 indeces = (mat < maxIndex) ? (int4)(mat * 255) : 0;

				float4 reindexedMat;
				reindexedMat.r = _RemapIndeces[indeces.r];
				reindexedMat.g = _RemapIndeces[indeces.g];
				reindexedMat.b = _RemapIndeces[indeces.b];
				reindexedMat.a = _RemapIndeces[indeces.a];
				reindexedMat = (mat < maxIndex) ? reindexedMat : 1.0;

				return reindexedMat;
			}
			ENDCG
		}
	}
}
