#ifndef A_FOLIAGE_VS
#define A_FOLIAGE_VS

		fixed4 _Color;
		fixed4 _ColorVariation;	
			
		sampler2D _MainTex;
	#if defined(NORMAL_MAP_ON)
		sampler2D _BumpMap;
	#endif
		sampler2D _SpecTex;
			
		fixed3 _SpecularReflectivity;
		fixed _BackfaceSmoothness;
		fixed _Cutoff2;
		fixed _Smoothness;
	
	/*
	#if defined(DEBUG)
		float4 _DebugLight;
		float _DebugMode;
	#endif
	*/
	#if defined(EMISSION_GRASS_ON)
		float _EmissionTodFrequency;
		float _EmissionTodSize;
		float _EmissionTodScale;
		float _EmissionTodMinimum;
		float4 _EmissionColor;
		float4 _EmissionTodPower;
		float _EmissionCurve;
	#endif
	
		float4 _Bounds;
		float _LeafTurbulence;
		float pivotOffset;
		
		float 	_AfsFoliageWaveSize;
		float 	_WindFrequency;
		float4 	_AfsFoliageTimeFrequency; 				// x: time * frequency, y: time, zw: turbulence for 2nd bending
		float4 	_AfsFoliageWind;
		float4 	_AfsFoliageWindPulsMagnitudeFrequency;
		float2 	_AfsFoliageWindMultiplier; 

	#define WIND _AfsFoliageWind
        
    #ifdef FAR_CULL_ON
        fixed		_CullFarStart;
        fixed		_CullFarDistance;
    #endif
        


		struct Input 
		{
			float2 uv_MainTex;
			fixed4 color : COLOR0;	// color.a = AO
			fixed4 colorSecond;
			float3 worldNormal;
			//#if defined(DEBUG)
			//fixed colorThird;
			//#endif
			float3 worldPos;
			float facingSign : VFACE;
			INTERNAL_DATA
		};
		       
		


        inline fixed rand(float2 co)
        {
            return frac(sin(dot(co ,half2(12.9898, 78.233))) * 43758.5453);
        }
		
		inline float4 SmoothCurve( float4 x ) 
		{
			return x * x *( 3.0 - 2.0 * x );
		}
		inline float4 TriangleWave( float4 x ) 
		{
			return abs( frac( x + 0.5 ) * 2.0 - 1.0 );
		}
		inline float4 SmoothTriangleWave( float4 x ) 
		{
			return SmoothCurve( TriangleWave( x ) );
		}		

		inline void AfsAnimateVertex (inout float4 pos, inout float3 normal, float4 tangent, float3 pivot, float4 animParams, inout float variation)
		{	

	#if defined (MULTI_PIVOTS_ON)
			pos.xyz = pos.xyz - pivot;
	#endif
			
			float originalLength = length(pos.xyz);
				
		// 	All computation is done in worldspace
			pos.xyz = mul(unity_ObjectToWorld, pos).xyz;
			
		#if defined(WIND_ON)	
			float3 wpos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
			
		#if defined (MULTI_PIVOTS_ON)
			float wpivot = mul(unity_ObjectToWorld, pivot).xyz;
			wpos = wpivot + wpos;
		#endif
			
		//	based on original wind bending
			const float fDetailAmp = 0.1;
			const float fBranchAmp = 0.3;

		//	Phases (object, vertex, branch)
			float3 variations = abs( frac( wpos.xyz * _AfsFoliageWaveSize) - 0.5 );
			float fObjPhase = dot(variations, float3(1,1,1) ) + variation;
			
			float fObjPhase01 = 1.0 + (frac(fObjPhase) - 0.5) * 0.1;

			variation = saturate(fObjPhase * 0.6665);
			fObjPhase *= 10;

			float fBranchPhase = fObjPhase.x + animParams.r;
			float fVtxPhase = dot(pos.xyz, animParams.g + fBranchPhase);

		//	Animate Wind – as we do not get per instance data we have to create variation within the shader
			float sinuswave = _Time.y * 0.5; //_SinTime.z; // _SinTime does not allow much difference between the instances
			
		// Seems to be ok to not use domainTime which would be: float sinuswave = _AfsTimeFrequency.x + fObjPhase;
			float4 TriangleWaves = SmoothTriangleWave( float4( fObjPhase + sinuswave, fObjPhase + sinuswave * 0.8 * fObjPhase01, 0.0, 0.0) );
			float Oscillation = (TriangleWaves.x + (TriangleWaves.y * TriangleWaves.y)) * 0.5; 

		//	Now factor in wind pulse magnitude
			WIND.xyz = WIND.xyz + float4(_AfsFoliageWindPulsMagnitudeFrequency.xyz, 0.25) * (1.0 + Oscillation * _AfsFoliageWindPulsMagnitudeFrequency.w) * 0.5;

		//	x is used for edges; y is used for branches
			float2 vWavesIn = _Time.yy + float2(fVtxPhase, fBranchPhase); // Seems to be ok to not use domainTime which would be: float2 vWavesIn = _AfsTimeFrequency.y + float2(fVtxPhase, fBranchPhase);

		//	Calculate waves and factor in _LeafTurbulence
			float4 vWaves = (frac( vWavesIn.xxyy * (fObjPhase01) * float4(1.975, 0.793, lerp(float2(0.375, 0.193), _AfsFoliageTimeFrequency.zw, _LeafTurbulence ) ) ) * 2.0 - 1.0);

			vWaves = SmoothTriangleWave( vWaves );
			float2 vWavesSum = vWaves.xz + vWaves.yw;

		//	Edge (xz) controlled by vertex green and branch bending (y) controled by vertex alpha
			float3 bend = animParams.g * fDetailAmp * normal.xyz * sign(normal.xyz); // sign important to match normals on double sided geometry
			bend.y = animParams.a * fBranchAmp;

		//	Secondary bending and edge fluttering
			float3 offset = ( (vWavesSum.xyx * bend) + ( WIND.xyz * vWavesSum.y * animParams.a) )  * WIND.w * _AfsFoliageWindMultiplier.y;

		//	Primary bending / Displace position
			offset += animParams.b * Oscillation * WIND.xyz * _AfsFoliageWindMultiplier.x;

		//	Apply Wind Animation	
			pos.xyz += offset;
		#endif

			pos.xyz = mul( unity_WorldToObject, pos).xyz;
			
		//	Preserve length - forward is unfortunately not precise enough...
		#if defined (PRESERVE_LENGHT_ON)
			#if defined (MULTI_PIVOTS_ON)
				pos.xyz = normalize(pos.xyz) * originalLength + pivot;
			#else
				pos.xyz = normalize(pos.xyz) * originalLength;
			#endif
		#else
			#if defined (MULTI_PIVOTS_ON)
				pos.xyz = pos.xyz + pivot;
			#endif
		#endif
		}
                
        void vert (inout appdata_full v, out Input o)
        {
			UNITY_INITIALIZE_OUTPUT(Input,o);
		#ifdef DEBUG
			o.colorSecond = v.color;
		#endif
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			float distanceToCamera = length(worldPos - _WorldSpaceCameraPos.xyz);
			
		#ifdef FAR_CULL_ON_SIMPLE
			float cull = 1.0 - saturate((distanceToCamera - _CullFarStart)/ _CullFarDistance);			 

			if(cull > 0)
			{
		#endif	
				float4 bendingCoords = v.color.rgab;	
				float3 pivot = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
				float3 localPivot = float3(0,0,0);
			
			#if defined(MULTI_PIVOTS_ON)
				localPivot = float3(v.texcoord2.x * _Bounds.x,0,v.texcoord2.y * _Bounds.y);
				pivot = pivot + localPivot;
			#endif
	
				pivotOffset = rand(pivot.xz);
				fixed4 color = lerp(_Color, _ColorVariation, pivotOffset);
				float4 pos2 = float4(worldPos, 1);
				
        		float4 waving = TerrainWaveGrassCustom (pos2, pivotOffset, color);
			#ifndef DEBUG
				o.colorSecond = waving;
			#endif
				pivotOffset = 0;
				float variation = 0;
				
			#if defined(WIND_ON)
				AfsAnimateVertex (v.vertex, v.normal, v.tangent, localPivot, bendingCoords, variation);
			#endif
							
			#if defined(NORMAL_ON)
				v.normal = normalize(v.normal);
				v.tangent.xyz = normalize(v.tangent.xyz);
			#else
				v.normal = float3(0,1,0);
				v.tangent.xyz = cross(v.normal,float3(0,0,1));
			#endif
			
			#ifdef DEBUG
				o.colorThird = 0;
				if (_DebugLight.x == 1 && v.texcoord2.x == _DebugLight.y && v.texcoord2.y == _DebugLight.z)
						o.colorThird = 1;
			#endif
			
		#ifdef FAR_CULL_ON_SIMPLE
			}
			v.vertex.xyz *= cull;
		#endif	
        }
		

		
#ifndef DEBUG
        void surf(Input IN, inout SurfaceOutputAFSSpecular o) //SurfaceOutputStandard
#else
	void surf(Input IN, inout SurfaceOutputAFSUnlit o) //SurfaceOutputStandard
#endif
        {
			half4 c = tex2D (_MainTex, IN.uv_MainTex.xy);

			clip(c.a - _Cutoff2);
				
			o.Albedo = c.rgb * IN.colorSecond.rgb;
			o.Alpha = c.a;
		#if defined(NORMAL_MAP_ON)
			fixed4 normal = tex2D(_BumpMap, IN.uv_MainTex.xy);
		#endif
			
			fixed4 specTex = tex2D(_SpecTex, IN.uv_MainTex.xy);
			o.Smoothness = (1 - specTex.a) * _Smoothness;
			
		#ifndef DEBUG
			o.Specular = _SpecularReflectivity * (1-o.Smoothness);
		#endif
			//	Backface Smoothness
			o.Smoothness = (IN.facingSign > 0) ? o.Smoothness : o.Smoothness * _BackfaceSmoothness;
				
		#if defined(NORMAL_MAP_ON)
			o.Normal = UnpackNormalDXT5nm(normal);//
				#if defined(NORMAL_ON)
				o.Normal *= half3(1,1,IN.facingSign);
					#ifndef DEBUG
					o.VertexNormal = WorldNormalVector(IN, half3(0,0,IN.facingSign) );
					#endif
				#endif
		#endif
				
				
		#if defined(EMISSION_GRASS_ON)
			half posSin = (_EmissionTodSize * sin(pivotOffset + _Time.w * _EmissionTodFrequency + IN.worldPos.x / _EmissionTodScale + IN.worldPos.z / _EmissionTodScale) + _EmissionTodMinimum) * _EmissionTodPower[_EmissionCurve];
			o.Emission = _EmissionColor * specTex.rgb * posSin;
			//o.Emission += IN.emissionPower * _EmissionColor.rgb * _ActiveEmission * rest.r * _EmissionTodPower;// blendColorDodge(second.rgb * _ActiveEmission, second.rgb * _ActiveEmission);
		#else
			o.Emission = 0;
		#endif
		
		#ifdef DEBUG		
			float debugLight = IN.colorThird * clamp(sin(_Time.w * 2),0,1);
			// red
			if (_DebugMode == 1) {
				o.Albedo = half3(lerp(IN.colorSecond.r,0.5f,debugLight),  0, 0);
			}
			// green
			if (_DebugMode == 2) {
				o.Albedo = half3(0, lerp(IN.colorSecond.g,0.5f,debugLight), 0);
			}
			// blue
			if (_DebugMode == 3) {
				o.Albedo = half3(0, 0, lerp(IN.colorSecond.b,0.5f,debugLight));
			}
			// alpha
			if (_DebugMode == 4) {
				o.Albedo = lerp(IN.colorSecond.a,0.5f,debugLight);
			}
		#endif

        }
		
		
		
#endif
		
		