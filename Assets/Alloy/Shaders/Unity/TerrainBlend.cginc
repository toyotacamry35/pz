#include "UnityCG.cginc"
#include "UnityStandardUtils.cginc"

#ifndef TERRAIN_BLEND_CGINC
	#define TERRAIN_BLEND_CGINC

	sampler2D _WorldNormalsLeftUp;
	float4 _WorldNormalsLeftUp_TexelSize;

	sampler2D _WorldNormalsLeftDown;
	float4 _WorldNormalsLeftDown_TexelSize;

	sampler2D _WorldNormalsRightUp;
	float4 _WorldNormalsRightUp_TexelSize;

	sampler2D _WorldNormalsRightDown;
	float4 _WorldNormalsRightDown_TexelSize;


	float4 _TerrainsCenter;

	float3 _TerrainSize;
	bool _SingleTerrain;

	float4 _TerrainOffset;
	float baseOcclusion;

	half _IScale;
	half _IScaleDouble;
	half _DoubleUp;
	float _FadeHeight;
	float _FadeScale;
	float _FadeStart;

	float _SeamDepth;
	float _SeamPower;

	sampler2D _MaskOne;
	half _MaskOneScale;
	half _MaskOneStrength;

	float3 _DoubleVector;

	float _HeightAO;
	float _MaskPower;
	float _MaskContrast;
	float _uv_Relief_z;


	struct TerrainSurf
	{
		float3 normal;
		float height;
	};

	TerrainSurf SampleTerrainNew(float3 worldPos)
	{
		float2 worldCoord = frac((worldPos.xz) / (_TerrainSize.xz));	

		float4 hn;
		float4 hnRightUp = tex2Dlod(_WorldNormalsRightUp, float4(worldCoord, 0, 0));
		if (!_SingleTerrain)
		{	
			float4 hnLeftUp = tex2Dlod(_WorldNormalsLeftUp, float4(worldCoord, 0, 0));
			float4 hnLeftDown = tex2Dlod(_WorldNormalsLeftDown, float4(worldCoord, 0, 0));
			float4 hnRightDown = tex2Dlod(_WorldNormalsRightDown, float4(worldCoord, 0, 0));

			float4 hnUp = (worldPos.x < _TerrainsCenter.x) ? hnLeftUp : hnRightUp;
			float4 hnDown = (worldPos.x < _TerrainsCenter.x) ? hnLeftDown : hnRightDown;

			hn = (worldPos.z >= _TerrainsCenter.z) ? hnUp : hnDown;
		}
		else
		{
			hn = hnRightUp;
		}
		
		TerrainSurf ts;

		ts.normal.xz = (hn.rg - 127.0 / 255.0) * 2.0;
		ts.normal.y = sqrt(1 - dot(ts.normal.xz, ts.normal.xz));
		ts.normal = normalize(ts.normal);

		float heihgt01 = hn.b*(1.0/256.0) + hn.a;
		ts.height = heihgt01 *_TerrainsCenter.w;

		return ts;
	}


	float CalculateHeightTransition(float h1, float h2, float mark)
	{
		float2 tHA = saturate(float2(h1, h2) + 0.001);
		float2 sc1 = float2(mark, 1 - mark);
		sc1 *= tHA;
		
		// What we are trying to achieve:
		// float2 sc1_mid = sc1 * sc1; // to make mask more sharp
		// sc1_mid /= max(0.0001, dot(sc1_mid, 1)); // to make sum of components equal to 1
		// float2 sc1_close = sc1_mid*sc1_mid;
		// sc1_close /= max(0.0001, dot(sc1_close, 1));
		// return sc1_close.x;

		// Simplified version:
		sc1 = pow(sc1, 4);
		return saturate(sc1.x / (sc1.x + sc1.y));
	}

	struct Layer
	{
		half3 albedo;
		half height;
		float3 normals;
		half4 parameters;
	};

	void CalculateMaterial(out Layer material, float2 uv, float mip, ShaderLayer shaderLayer)
	{
		half4 albedoHeight = UNITY_SAMPLE_TEX2DARRAY_LOD(TerrainSplatsAlbedo, float3(uv, shaderLayer.albedoIndex), mip + TERRAIN_TEX_MIP_ALBEDO);
		material.albedo = albedoHeight.rgb;
		material.height = albedoHeight.a;
		
		material.normals = float3(UNITY_SAMPLE_TEX2DARRAY_LOD(TerrainSplatsNormal, float3(uv, shaderLayer.normalsIndex), mip +  TERRAIN_TEX_MIP_NORMALS).rg,0.5);
		material.normals.xy = material.normals.xy * 2 - 1;
		material.parameters = UNITY_SAMPLE_TEX2DARRAY_LOD(TerrainSplatsParameters, float3(uv, shaderLayer.parametersIndex), mip + TERRAIN_TEX_MIP_PARAMETERS);
	}

	float CalculateBaseMipMap(float2 coords)
	{
		float2 dx = ddx(coords.xy);
		float2 dy = ddy(coords.xy);
		float d = max(dot(dx, dx), dot(dy, dy));
		return 0.5*log2(d);
	}

	void BlendMaterials(inout Layer material, Layer materialSecond, float diff)
	{
		material.albedo = lerp(materialSecond.albedo, material.albedo, diff);
		material.normals = lerp(materialSecond.normals, material.normals, diff);
		material.height = lerp(materialSecond.height, material.height, diff);
		material.parameters = lerp(materialSecond.parameters, material.parameters, diff);
	}

	#ifdef _RNM_BLEND
		// Reoriented Normal Mapping for Unity3d
		// http://discourse.selfshadow.com/t/blending-in-detail/21/18
		float3 rnmBlendUnpacked(float3 n1, float3 n2)
		{
			n1 += float3( 0,  0, 1);
			n2 *= float3(-1, -1, 1);
			return n1*dot(n1, n2)/n1.z - n2;
		}
	#endif

	#if _BLEND_TERRAIN_DOUBLE
		float3 CalculateMaterialPart(inout Layer materialMain, inout Layer materialFoundation, in ShaderLayer shaderLayer, out float layerMip, float2 coords, half diffZ, half blend)
	#else
		float3 CalculateMaterialPart(inout Layer materialMain, inout Layer materialFoundation, in ShaderLayer shaderLayer, out float layerMip, float2 coords, half blend)
	#endif
	{
		float baseMipLevel = CalculateBaseMipMap(coords);
		float2 uvTRI = float2(coords * _IScale);
		layerMip = baseMipLevel + shaderLayer.textureMipBias;//_LayersParams[_NormalSwitch.x * 3].y;
		
		CalculateMaterial(materialMain, uvTRI, layerMip, shaderLayer);

		#if _BLEND_TERRAIN_DOUBLE
			
			ShaderLayer shaderLayerSecond = GetTerrainAtlasLayer(_NormalSwitch.y);
			
			float3 uvTRISecond = float3(coords * _IScaleDouble, _NormalSwitch.y);
			float layerMip02 = baseMipLevel + shaderLayerSecond.textureMipBias;
			Layer materialSecond;
			
			CalculateMaterial(materialSecond, uvTRISecond.xy, layerMip02, shaderLayerSecond);
			
			diffZ = CalculateHeightTransition(saturate(1-materialSecond.height), saturate(1-materialMain.height), diffZ);
			BlendMaterials(materialMain, materialSecond, diffZ);
		#endif
		
		materialMain.normals.z = sqrt(1 - saturate(dot(materialMain.normals.xy, materialMain.normals.xy)));
		
		materialFoundation.albedo += blend * materialMain.albedo;
		materialFoundation.height += blend * materialMain.height;
		materialFoundation.parameters += blend * materialMain.parameters;
		materialFoundation.normals = float3(0,0,0);
		return materialMain.normals;
	}

	void Planar(out Layer materialMain, float3 worldPos, float2 intersectionUVs, float maskScale, inout float mask)
	{
		float2 dx = ddx(intersectionUVs);
		float2 dy = ddy(intersectionUVs);
		float d = max(dot(dx, dx), dot(dy, dy));
		float baseMipLevel = 0.5*log2(d);
		mask = 0;

		ShaderLayer shaderLayer = GetTerrainAtlasLayer(_NormalSwitch.x);	
		float3 mainUV = float3(intersectionUVs * _IScale, _NormalSwitch.x);
		float layerMip = baseMipLevel + shaderLayer.textureMipBias;
		CalculateMaterial(materialMain, mainUV.xz, layerMip, shaderLayer);


		#if _BLEND_TERRAIN_DOUBLE
			
			ShaderLayer shaderLayerSecond = GetTerrainAtlasLayer(_NormalSwitch.x);
			float3 doubleUV = float3(intersectionUVs * _IScaleDouble, _NormalSwitch.y);
			float layerMip02 = baseMipLevel + shaderLayerSecond.textureMipBias;
			Layer materialSecond;
			
			float3 doubleVec = normalize(_DoubleVector);
			half diffZ = saturate(worldPos.x * doubleVec.x + worldPos.y * doubleVec.y + worldPos.z * doubleVec.z + _DoubleUp);
			
			CalculateMaterial(materialSecond, doubleUV.xy, layerMip02, shaderLayerSecond);
			
			diffZ = CalculateHeightTransition(saturate(1-materialSecond.height), saturate(1-materialMain.height), diffZ);
			BlendMaterials(materialMain, materialSecond, diffZ);
		#endif

		materialMain.normals.z = sqrt(1 - saturate(dot(materialMain.normals.xy, materialMain.normals.xy)));
		materialMain.normals = normalize(materialMain.normals);

		mask = UNITY_SAMPLE_TEX2DARRAY(TerrainSplatsAlbedo, float3(intersectionUVs * _MaskOneScale, shaderLayer.albedoIndex)).a;
	}

	void Triplanar(inout ASurface s, out Layer materialMain, float3 terrainNormalWorld, float blendMask, float maskScale)
	{
		float3 _INPUT_uv = s.positionWorld.xyz;
		float _INPUT_distance = distance(_WorldSpaceCameraPos, s.positionWorld);

		// create mask based on linear distance to camera: from 25m and above (0) to 20m and below (1)
		// _uv_Relief_z=saturate((_INPUT_distance - 20) / 5);
		// _uv_Relief_z=1-_uv_Relief_z;
		
		#if _BLEND_TERRAIN_DOUBLE
			float3 doubleVec = normalize(_DoubleVector);
			half diffZ = saturate(s.positionWorld.x * doubleVec.x + s.positionWorld.y * doubleVec.y + s.positionWorld.z * doubleVec.z + _DoubleUp);
		#endif

		float3 absVertexNormal = abs(s.vertexNormalWorld);
		half3 triplanar_axis_sign = sign(s.vertexNormalWorld);

		float3 triplanar_blend = pow(absVertexNormal, 8);
		//float3 triplanar_blend = absVertexNormal;
		// results in triplanar_blend.x + triplanar_blend.y + triplanar_blend.z equal to 1
		triplanar_blend /= dot(triplanar_blend, 1);


		float3 normalX = float3(0, 0, 1);
		float3 normalY = float3(0, 0, 1);
		float3 normalZ = float3(0, 0, 1);
		
		float maskX = 0;
		float maskY = 0;
		float maskZ = 0;

		Layer materialFoundation;
		materialFoundation.albedo = float3(0,0,0);
		materialFoundation.height = 0;
		materialFoundation.parameters = float4(0,0,0,0);
		materialFoundation.normals = float3(0, 0, 1);

		ShaderLayer shaderLayer = GetTerrainAtlasLayer(_NormalSwitch.x);
		float layerMip = 0;

		if (triplanar_blend.x > 0.02)
		{
			#if _BLEND_TERRAIN_DOUBLE
				normalX = CalculateMaterialPart(materialMain, materialFoundation, shaderLayer, layerMip, _INPUT_uv.zy, diffZ, triplanar_blend.x);
			#else
				normalX = CalculateMaterialPart(materialMain, materialFoundation, shaderLayer, layerMip, _INPUT_uv.zy, triplanar_blend.x);
			#endif
			
			maskX = UNITY_SAMPLE_TEX2DARRAY_LOD(TerrainSplatsAlbedo, float3(_INPUT_uv.zy * _MaskOneScale, shaderLayer.albedoIndex), layerMip).a;
		}

		if (triplanar_blend.y > 0.02)
		{
			#if _BLEND_TERRAIN_DOUBLE
				normalY = CalculateMaterialPart(materialMain, materialFoundation, shaderLayer, layerMip, _INPUT_uv.xz, diffZ, triplanar_blend.y);
			#else
				normalY = CalculateMaterialPart(materialMain, materialFoundation, shaderLayer, layerMip, _INPUT_uv.xz, triplanar_blend.y);
			#endif	
			
			maskY = UNITY_SAMPLE_TEX2DARRAY_LOD(TerrainSplatsAlbedo, float3(_INPUT_uv.xz * _MaskOneScale, shaderLayer.albedoIndex), layerMip).a;
		}
		
		if (triplanar_blend.z > 0.02)
		{
			#if _BLEND_TERRAIN_DOUBLE
				normalZ = CalculateMaterialPart(materialMain, materialFoundation, shaderLayer, layerMip, _INPUT_uv.xy, diffZ, triplanar_blend.z);
			#else
				normalZ = CalculateMaterialPart(materialMain, materialFoundation, shaderLayer, layerMip, _INPUT_uv.xy, triplanar_blend.z);
			#endif	

			maskZ= UNITY_SAMPLE_TEX2DARRAY_LOD(TerrainSplatsAlbedo, float3(_INPUT_uv.xy * _MaskOneScale, shaderLayer.albedoIndex), layerMip).a;
		}

		float heightMask = (triplanar_blend.x * maskX + triplanar_blend.y * maskY + triplanar_blend.z * maskZ);
		
		#ifdef _BLEND_TERRAIN_TEMPLATE
			float maskOne = lerp(_MaskContrast, pow(heightMask, _MaskOneStrength), blendMask);
			float diff = saturate(maskOne + pow(blendMask, _MaskPower));
		#else
			float diff = blendMask;
		#endif

		materialMain.albedo = saturate(materialFoundation.albedo);
		materialMain.height = materialFoundation.height;
		float defaultHeight = 1 - s.roughness;
		float heightDiff = CalculateHeightTransition(materialMain.height, defaultHeight, diff) * _BlendingWeight;
		
		float heightAO = 2 - _HeightAO - heightDiff;

		s.baseColor = lerp(s.baseColor * saturate(heightAO), materialMain.albedo, heightDiff);
		s.metallic = lerp(s.metallic, materialMain.parameters.r, heightDiff);
		s.specularity = lerp(s.specularity, materialMain.parameters.b, heightDiff);
		s.ambientOcclusion = lerp(s.ambientOcclusion, materialMain.parameters.g, heightDiff);
		s.specularTint = lerp(s.specularTint, 0, heightDiff);
		s.roughness = lerp(s.roughness, materialMain.parameters.a, heightDiff);

		#ifdef _MESH_BLENDING_USE_COATING
			float diff2 = lerp(0, max((1-s.mask), diff), heightDiff);
			float coatingDiff = s.mask;
			//apply coating
			s.baseColor = lerp(s.baseColor, materialMain.albedo/*lerp(materialMain.albedo,0, materialMain.parameters.b)*/, coatingDiff);
			s.metallic = lerp(s.metallic, _Metal * materialMain.parameters.r, coatingDiff);
			s.specularity = lerp(s.specularity, materialMain.parameters.b, coatingDiff);
			s.roughness = lerp(s.roughness, materialMain.parameters.a, coatingDiff);
			s.ambientOcclusion = s.ambientOcclusion;//min(s.ambientOcclusion, 0);
			s.specularTint = lerp(s.specularTint, 0, coatingDiff);
			float blendingDiff = diff2;
			float normalsDiff = saturate(blendingDiff + coatingDiff);
			s.mask = saturate(1 - diff2 - s.mask);
		#else
			float blendingDiff = heightDiff;
			float normalsDiff = heightDiff;
			s.mask = 1 - heightDiff;
		#endif

		float3 blendingNormal = normalize(lerp(s.vertexNormalWorld, terrainNormalWorld, blendingDiff));
		#ifdef _RNM_BLEND
			// apply Reoriented Normal Mapping blending
			normalX = rnmBlendUnpacked(float3(blendingNormal.zy, abs(blendingNormal.x)), normalX);
			normalY = rnmBlendUnpacked(float3(blendingNormal.xz, abs(blendingNormal.y)), normalY);
			normalZ = rnmBlendUnpacked(float3(blendingNormal.xy, abs(blendingNormal.z)), normalZ);
			
			normalX.z *= triplanar_axis_sign.x;
			//normalY.z *= triplanar_axis_sign.y;
			normalZ.z *= triplanar_axis_sign.z;
		#else
			// apply whiteout blending
			normalX = float3(normalX.xy + blendingNormal.zy, abs(normalX.z) * blendingNormal.x);
			normalY = float3(normalY.xy + blendingNormal.xz, abs(normalY.z) * blendingNormal.y);
			normalZ = float3(normalZ.xy + blendingNormal.xy, abs(normalZ.z) * blendingNormal.z);
		#endif
		float3 normalsWorld = (normalX.zyx * triplanar_blend.x + normalY.xzy * triplanar_blend.y + normalZ.xyz * triplanar_blend.z);
		float3 materialNormalsTangent = aWorldToTangent(s, normalsWorld);
		materialNormalsTangent.xy *= normalsDiff;
		materialNormalsTangent.z = sqrt(1.0 - saturate(dot(materialNormalsTangent.xy, materialNormalsTangent.xy)));
		materialMain.normals = materialNormalsTangent;
	}

	void aMeshBlending(inout ASurface s)
	{
		TerrainSurf terrainSurfData = SampleTerrainNew(s.positionWorld.xyz);
		float height = terrainSurfData.height;

		// create a mask that is 0 below terrain level and going up to 1 from terrain level to terrain level + _FadeStart
		float blendMask = (s.positionWorld.y - height) + _FadeStart;//
		blendMask = saturate((blendMask) / _FadeHeight);
		
		// invert mask and use pow for non-linear transition between 0 and 1
		blendMask = 1 - pow(blendMask, _FadeScale);

		half3 terrainNormal = terrainSurfData.normal;
		
		Layer mainMaterial;
		
		Triplanar(s, mainMaterial, terrainNormal, blendMask, _MaskOneScale);

		s.normalTangent = BlendNormals(s.normalTangent, mainMaterial.normals);
		s.normalWorld = aTangentToWorld(s, s.normalTangent);
		s.objectNormalWorld = s.normalWorld;
		s.ambientNormalWorld = s.normalWorld;
	}
#endif