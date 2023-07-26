// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'unity_World2Shadow' with 'unity_WorldToShadow'

#ifndef __DS_RAYMARCH_LIB
#define __DS_RAYMARCH_LIB

#if !defined(kVolumeSamples)
#define kVolumeSamples 16
#endif

#if !defined(kDownsampleFactor)
#define kDownsampleFactor 2
#endif

sampler3D _SamplingOffsets;
uniform fixed4 _SamplingOffsets_TexelSize;

// Viewport info for ray construction
uniform fixed3 _ViewportCorner;
uniform fixed3 _ViewportRight;
uniform fixed3 _ViewportUp;

// UV offsets
uniform fixed4 _InterleavedOffset;

struct appdata_ray
{
	fixed4 vertex : POSITION;
	fixed4 texcoord : TEXCOORD0;
	fixed4 ray_ws : TEXCOORD1;
};

struct v2f_ray
{
	fixed4 pos : SV_POSITION;
	fixed3 uv : TEXCOORD0;
	fixed3 uv_interleaved : TEXCOORD1;
	fixed3 ray_interp : TEXCOORD2;
};

// Vertex program when using ray-marching. View rays are interpolated from the
// viewport corner, right and up directions calculated outside the shader.
v2f_ray DSVP_RayMarch(appdata_ray vtx)
{
	v2f_ray output;

	half index = vtx.vertex.z;
	vtx.vertex.z = 0.1;

#if defined (UNITY_5_4_OR_NEWER)
	output.pos = UnityObjectToClipPos(vtx.vertex.xyz);
#else
	output.pos = UnityObjectToClipPos(vtx.vertex);
#endif
	output.uv = fixed3(vtx.texcoord.xy, 1);
	output.uv_interleaved = fixed3(vtx.texcoord.xy * (_ScreenParams.xy / kDownsampleFactor / _SamplingOffsets_TexelSize.z), _InterleavedOffset.x);
	//output.uv_interleaved += fixed3(_InterleavedOffset.xy * _SamplingOffsets_TexelSize.x, 0);

#if UNITY_UV_STARTS_AT_TOP
	if (_MainTex_TexelSize.y < 0)
	{
		output.uv.y = 1 - output.uv.y;
		output.uv_interleaved.y = 1 - output.uv_interleaved.y;
	}
#endif

	output.ray_interp = _ViewportCorner + _ViewportRight * output.uv.x + _ViewportUp * output.uv.y;

	return output;
}

// [START] Main ray-march volumetric function.
fixed4 DS_RenderLightShafts(v2f_ray input)
{
	fixed sampleOffset = tex3D(_SamplingOffsets, input.uv_interleaved).r;
	fixed depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, input.uv);
	fixed linearDepth = Linear01Depth(depth);

	// World-space ray.
	fixed3 ray = input.ray_interp * linearDepth;
	fixed3 rayN = normalize(ray);
	fixed4 rayDt = fixed4(ray / kVolumeSamples, 0);
	fixed4 sampleWS = fixed4(_WorldSpaceCameraPos, 1) + rayDt * sampleOffset;
	fixed dT = length(rayDt);

	// Light-space rays. Transform the camera position and ray end into each of the shadow
	// cascades, then get the steps for marching. This avoids having four extra matrix
	// mul() per ray step to find the shadow map coordinates.
	fixed3 rayEnd = _WorldSpaceCameraPos + ray;

	fixed4 sampleLS_0 = mul(unity_WorldToShadow[0], fixed4(_WorldSpaceCameraPos, 1));
	fixed4 sampleEndLS_0 = mul(unity_WorldToShadow[0], fixed4(rayEnd, 1));
	fixed3 rayLS_0 = (sampleEndLS_0 - sampleLS_0).xyz;
	fixed4 rayDtLS_0 = fixed4(rayLS_0 / (kVolumeSamples - 1), 0);
	sampleLS_0 += rayDtLS_0 * sampleOffset;

	fixed4 sampleLS_1 = mul(unity_WorldToShadow[1], fixed4(_WorldSpaceCameraPos, 1));
	fixed4 sampleEndLS_1 = mul(unity_WorldToShadow[1], fixed4(rayEnd, 1));
	fixed3 rayLS_1 = (sampleEndLS_1 - sampleLS_1).xyz;
	fixed4 rayDtLS_1 = fixed4(rayLS_1 / (kVolumeSamples - 1), 0);
	sampleLS_1 += rayDtLS_1 * sampleOffset;

	fixed4 sampleLS_2 = mul(unity_WorldToShadow[2], fixed4(_WorldSpaceCameraPos, 1));
	fixed4 sampleEndLS_2 = mul(unity_WorldToShadow[2], fixed4(rayEnd, 1));
	fixed3 rayLS_2 = (sampleEndLS_2 - sampleLS_2).xyz;
	fixed4 rayDtLS_2 = fixed4(rayLS_2 / (kVolumeSamples - 1), 0);
	sampleLS_2 += rayDtLS_2 * sampleOffset;

	fixed4 sampleLS_3 = mul(unity_WorldToShadow[3], fixed4(_WorldSpaceCameraPos, 1));
	fixed4 sampleEndLS_3 = mul(unity_WorldToShadow[3], fixed4(rayEnd, 1));
	fixed3 rayLS_3 = (sampleEndLS_3 - sampleLS_3).xyz;
	fixed4 rayDtLS_3 = fixed4(rayLS_3 / (kVolumeSamples - 1), 0);
	sampleLS_3 += rayDtLS_3 * sampleOffset;

	// Radiance accumulation.
	fixed radiance = 0;
	fixed shadowFactor = 0;
	fixed extinction = 1;

	fixed mPhase = MiePhaseH(rayN);
	fixed irradianceM = kMBetaS * mPhase * dT; //<--- scattered light from haze per-step per-unit density.
	fixed irradianceR = kSphericalNormFactor * dT; //<--- scattered light from air per-step per-unit density.

	UNITY_UNROLL
		for (int s = 0; s < kVolumeSamples - 1; s++)
	{
#if defined (SHADOW_PROJ_CLOSE)
		fixed wz = dT * s - _ProjectionParams.y;
		fixed4 cascadeWeights = getCascadeWeights(sampleWS.xyz, wz);
#else
		fixed4 cascadeWeights = getCascadeWeights_splitSpheres(sampleWS.xyz);
#endif
		bool inside = dot(cascadeWeights, fixed4(1, 1, 1, 1)) < 4;
		fixed3 shadowUV = sampleLS_0 * cascadeWeights.x + sampleLS_1 * cascadeWeights.y + sampleLS_2 * cascadeWeights.z + sampleLS_3 * cascadeWeights.w;
		shadowFactor = inside ? UNITY_SAMPLE_SHADOW(_ShadowCascades, shadowUV) : 1.0f;

		// Haze.
		fixed rho = GetHazeHeightDensity(sampleWS.y);
		radiance += irradianceM * rho * shadowFactor * kMDirectIntensity;

		// Increment world/light-space sample positions.
		sampleWS += rayDt;
		sampleLS_0 += rayDtLS_0;
		sampleLS_1 += rayDtLS_1;
		sampleLS_2 += rayDtLS_2;
		sampleLS_3 += rayDtLS_3;
	}

	return fixed4(_DS_LightColour * radiance, 0);
}
// [END] Main ray-march volumetric function.

#endif // __DS_RAYMARCH_LIB