// Upgrade NOTE: unity_Scale shader variable was removed; replaced '_WorldSpaceCameraPos.w' with '1.0'

// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

        // material
        sampler2D	_MainTex;
        fixed4		_Color;
        fixed4		_ColorB;
        fixed		_RandomDarkening;
        fixed		_RootAmbient;
        fixed		_Speed;
        fixed		_WavesSpeed;
        float	    _Wetness;
        
        #ifdef FAR_CULL_ON
        fixed		_CullFarStart;
        fixed		_CullFarDistance;
        #endif
        
        fixed		_WindAffectDistance;
        float _CutoffMinDistance;
		float _CutoffMaxDistance;
		fixed _CutoffMax;
		fixed _CutoffMin;

        // color areas
        sampler2D	_AG_ColorNoiseTex;
        float4		_AG_ColorNoiseArea;
        
        sampler2D	_AW_WavesTex;
        float4		_AW_DIR;

		
		float4 _Bounds;
		float4 _InteractPosition;

		float2 _WindMultiplier;
		sampler2D _GrassMotionTexture;
		float2 _RenderTargetSize;
		float4 _WorldSpaceCameraDir;
		float4 _Test;
	
		CBUFFER_START(AtgGrass)
		sampler2D _AtgWindRT;
		float4 _AtgWindDirSize;
		float4 _AtgWindStrengthMultipliers;
		float4 _AtgSinTime;
		float4 _AtgGrassFadeProps;
		float4 _AtgGrassShadowFadeProps;
		CBUFFER_END

#if defined(EMISSION_GRASS)
		float _EmissionTodFrequency;
		float _EmissionTodSize;
		float _EmissionTodScale;
		float _EmissionTodScale2;
		float _EmissionTodMinimum;
		//float4 _EmissionColor;
		float _EmissionCurve;
		float4 _EmissionTodPower;
		
		float _EmissionWindPower01;
		float _EmissionWindSize01;
		float _EmissionWindPower02;
		float _EmissionWindSize02;
#endif
		sampler2D _SpecTex;
		float3 _ConstantScale;
		half _Smootness;

        struct Input
        {
            float2 uv_MainTex;
			fixed Cutoff : TEXCOORD5;
            float3 worldPos;
			fixed4 color;
			float scale;
			float bendPower;
        };
        
        //UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        //UNITY_INSTANCING_BUFFER_END(Props)
        


        inline fixed rand(float2 co)
        {
            return frac(sin(dot(co ,half2(12.9898, 78.233))) * 43758.5453);
        }
			
		inline void Wind(inout appdata_full v, out Input o, float4x4 OtW, float4x4 WtO)
		{
				UNITY_INITIALIZE_OUTPUT(Input, o);

				float3 mainPivot = mul(OtW, float4(0,0,0,1));
				float distanceToCamera = length(mainPivot - _WorldSpaceCameraPos.xyz);

#ifdef FAR_CULL_ON_SIMPLE
				float cull = 1.0 - saturate((distanceToCamera - _CullFarStart)/ _CullFarDistance);			 

				if(cull > 0)
				{
#endif
					
#if defined(MULTI_PIVOTS_ON)
					float3 localPivot = float3(v.texcoord2.x * _Bounds.x, 0, v.texcoord2.y * _Bounds.y);
					float3 worldPivot = mul(OtW, localPivot);
					//v.vertex.xyz = v.vertex.xyz - localPivot;
#endif						

					float originalLength = length(v.vertex.xyz);			
					float3 pos = mul(OtW, v.vertex);
                
#if defined(MULTI_PIVOTS_ON)
						fixed random = rand(worldPivot.xz);
#else
						fixed random = rand(mainPivot.xz);
#endif
						float3 unitvec = mul((float3x3)OtW, float3(1, 0, 0)); 
						float scale = length(unitvec);
						scale = lerp(0.6, 1.0, random);
						o.scale = scale;

						fixed4 color = 0;
						float4 pos2 = float4(pos, 1);
						color = lerp(_Color, _ColorB, random);	
						float4 waving = TerrainWaveGrassCustom (pos2, random, color);
						o.color = waving;
						
						float mainBending = AG_BEND_FORCE * color.a;				

						
#if defined(MULTI_PIVOTS_ON)
	#if defined(USE_PIVOT_ON)
						float4 wind = tex2Dlod(_AtgWindRT, float4(worldPivot.xz * _AtgWindDirSize.w + (AG_PHASE_SHIFT * 0.1).xx + scale * 0.025, 0, 0));
	#else
						float4 wind = tex2Dlod(_AtgWindRT, float4(worldPivot.xz * _AtgWindDirSize.w + (AG_PHASE_SHIFT * 0.1).xx + scale * 0.025, 0, 0));
	#endif
#else
	#if defined(USE_PIVOT_ON)
						float4 wind = tex2Dlod(_AtgWindRT, float4(mainPivot.xz * _AtgWindDirSize.w + (AG_PHASE_SHIFT * 0.1).xx + scale * 0.025, 0, 0));
	#else
						float4 wind = tex2Dlod(_AtgWindRT, float4(pos.xz * _AtgWindDirSize.w + (AG_PHASE_SHIFT * 0.1).xx + scale * 0.025, 0, 0));
	#endif
#endif
					wind.r = wind.r * (wind.g * 2.0f - 0.24376f);
        
					v.normal =  float3(0,1,0);
					v.tangent.xyz = cross(v.normal,float3(0,0,1));

					float windStrength = wind.r * _AtgWindStrengthMultipliers.x * 1	* mainBending;
					float3 bend = normalize(mul((float3x3)WtO, _AtgWindDirSize.xyz)) * windStrength;
					v.vertex.xz += bend.xz;


					float2 jitter = lerp(float2 (_AtgSinTime.x, 0), _AtgSinTime.yz, float2(random, windStrength));
					v.vertex.xz +=(jitter.x + jitter.y * 0.5f) * (0.075 + _AtgSinTime.w) * saturate(windStrength);

					
#if defined(MULTI_PIVOTS_ON)					
					v.vertex.xyz = localPivot + normalize(v.vertex.xyz) * originalLength;
#else
					v.vertex.xyz = normalize(v.vertex.xyz) * originalLength;
#endif

					o.worldPos = mul(OtW, v.vertex.xyz);
            
				#ifdef FAR_CULL_ON_SIMPLE
				}
				v.vertex.xyz *= cull;
				#endif	
		}
         
		 
		float3 GetInstanceScale();
		
        void vert (inout appdata_full v, out Input o)
        {
			#ifdef INSTANCENATOR_FOLIAGE
			v.vertex.xyz = v.vertex.xyz * GetInstanceScale();
			#endif
            
			v.vertex.xyz *= _ConstantScale;

			Wind(v,o, unity_ObjectToWorld, unity_WorldToObject);	
			
			float3 mainPivot = mul(unity_ObjectToWorld, float4(0,0,0,1));
			float distanceToCamera = length(mainPivot - _WorldSpaceCameraPos.xyz);

			float A = (_CutoffMax - _CutoffMin)/(_CutoffMinDistance - _CutoffMaxDistance);
			float B = _CutoffMin - A * _CutoffMaxDistance;
			float cutoff = A * distanceToCamera + B;
			o.Cutoff = clamp(cutoff, _CutoffMin, _CutoffMax);
        }

        void surf(Input IN, inout SurfaceOutputAFSSpecular o) //SurfaceOutputStandard
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			clip(c.a - IN.Cutoff);
			fixed4 rest = tex2D(_SpecTex, IN.uv_MainTex);
			o.Albedo = c.rgb * IN.color.rgb;
			o.Normal = float3(0,0,1);

#ifdef EMISSION_GRASS
			half posSin = (_EmissionTodSize * sin(_Time.w * _EmissionTodFrequency + IN.worldPos.x / _EmissionTodScale + IN.worldPos.z / _EmissionTodScale2) + _EmissionTodMinimum) * _EmissionTodPower[_EmissionCurve];
			fixed4 wind2 = tex2D(_AtgWindRT, IN.worldPos.xz *  _AtgWindDirSize.w * _EmissionTodScale);		
			o.Emission += rest.rgb * pow(wind2.r, _EmissionWindSize01)  * _EmissionWindPower01 * wind2.r * _EmissionTodPower[_EmissionCurve];
			o.Emission += rest.rgb * pow(wind2.g, _EmissionWindSize02)  * _EmissionWindPower02 * wind2.g * _EmissionTodPower[_EmissionCurve];
#else
			o.Emission = 0;
#endif
			o.Smoothness = (1 - rest.a) * _Smootness * IN.scale;
            o.Alpha = c.a;
        }
		
		
		
		