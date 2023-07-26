#ifndef A_FOLIAGE_VS
#define A_FOLIAGE_VS

		fixed4 _Color;
		fixed4 _ColorVariation;	
		float4 _Bounds;
		float _LeafTurbulence;
		float pivotOffset;
		
		float 	_AfsFoliageWaveSize;
		float 	_WindFrequency;
		float4 	_AfsFoliageTimeFrequency;
		float4 	_AfsFoliageWind;
		float4 	_AfsFoliageWindPulsMagnitudeFrequency;
		float2 	_AfsFoliageWindMultiplier; 
		float3 _ConstantScale;
		float _CutoffMinDistance;
		float _CutoffMaxDistance;
		fixed _CutoffMax;
		fixed _CutoffMin;
	#define WIND _AfsFoliageWind
        
    #ifdef FAR_CULL_ON
        fixed		_CullFarStart;
        fixed		_CullFarDistance;
    #endif
        


		struct Input 
		{
			float2 uv_MainTex;
			fixed Cutoff : TEXCOORD5;
			fixed4 color : COLOR0;
			float3 worldPos;
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

		inline void AfsAnimateVertex (inout float4 pos, inout float3 normal, float4 tangent, float3 pivot, float4 animParams)
		{	
		
			pos.xyz = pos.xyz - pivot;
			
			float originalLength = length(pos.xyz);	
			pos.xyz = mul(unity_ObjectToWorld, pos).xyz;
			float3 wpos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
					
		#if defined (MULTI_PIVOTS_ON)
			float wpivot = mul(unity_ObjectToWorld, pivot).xyz;
			wpos = wpivot + wpos;
		#endif
			
			
			
			const float fDetailAmp = 0.1;
			const float fBranchAmp = 0.3;

			float3 variations = abs( frac( wpos.xyz * _AfsFoliageWaveSize) - 0.5 );
			float fObjPhase = dot(variations, float3(1,1,1) );
			
			float fObjPhase01 = 1.0 + (frac(fObjPhase) - 0.5) * 0.1;

			fObjPhase *= 10;

			float fBranchPhase = fObjPhase.x + animParams.r;
			float fVtxPhase = dot(pos.xyz, animParams.g + fBranchPhase);

			float sinuswave = _Time.y * 0.5;
			
			float4 TriangleWaves = SmoothTriangleWave( float4( fObjPhase + sinuswave, fObjPhase + sinuswave * 0.8 * fObjPhase01, 0.0, 0.0) );
			float Oscillation = (TriangleWaves.x + (TriangleWaves.y * TriangleWaves.y)) * 0.5; 

			WIND.xyz = WIND.xyz + float4(_AfsFoliageWindPulsMagnitudeFrequency.xyz, 0.25) * (1.0 + Oscillation * _AfsFoliageWindPulsMagnitudeFrequency.w) * 0.5;

			float2 vWavesIn = _Time.yy + float2(fVtxPhase, fBranchPhase);

			float4 vWaves = (frac( vWavesIn.xxyy * (fObjPhase01) * float4(1.975, 0.793, lerp(float2(0.375, 0.193), _AfsFoliageTimeFrequency.zw, _LeafTurbulence ) ) ) * 2.0 - 1.0);

			vWaves = SmoothTriangleWave( vWaves );
			float2 vWavesSum = vWaves.xz + vWaves.yw;

			float3 bend = animParams.g * fDetailAmp * normal.xyz * sign(normal.xyz); 
			bend.y = animParams.a * fBranchAmp;

			float3 offset = ( (vWavesSum.xyx * bend) + ( WIND.xyz * vWavesSum.y * animParams.a) )  * WIND.w * _AfsFoliageWindMultiplier.y;
			offset += animParams.b * Oscillation * WIND.xyz * _AfsFoliageWindMultiplier.x;

			pos.xyz += offset;

			
			pos.xyz = mul(unity_WorldToObject, pos).xyz;	
			pos.xyz = normalize(pos.xyz) * originalLength + pivot;

		}
        
		inline void Wind(inout appdata_full v)
		{
			//#ifndef INSTANCENATOR_FOLIAGE
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
				//v.texcoord4 = pos2;
				
        		float4 waving = TerrainWaveGrassCustom (pos2, pivotOffset, color);
				v.color = waving;
				pivotOffset = 0;
				
			#if defined(WIND_ON)
				AfsAnimateVertex (v.vertex, v.normal, v.tangent, localPivot, bendingCoords);
			#endif
				
			
			#if defined(NORMAL_ON)
				v.normal = normalize(v.normal);
				v.tangent.xyz = normalize(v.tangent.xyz);
			#else
				v.normal = float3(0,1,0);
				v.tangent.xyz = cross(v.normal,float3(0,0,1));
			#endif
			
			
			
		#ifdef FAR_CULL_ON_SIMPLE
			}
			v.vertex.xyz *= cull;
		#endif	
		}

		float3 GetInstanceScale();

		float4x4 CalcInverseMatrix(float4x4 mtxIn)
		{
			float3x3 w2oRotation;
			w2oRotation[0] = mtxIn[1].yzx * mtxIn[2].zxy - mtxIn[1].zxy * mtxIn[2].yzx;
			w2oRotation[1] = mtxIn[0].zxy * mtxIn[2].yzx - mtxIn[0].yzx * mtxIn[2].zxy;
			w2oRotation[2] = mtxIn[0].yzx * mtxIn[1].zxy - mtxIn[0].zxy * mtxIn[1].yzx;

			float det = dot(mtxIn[0], w2oRotation[0]);

			w2oRotation = transpose(w2oRotation);

			w2oRotation *= rcp(det);

			float3 w2oPosition = mul(w2oRotation, -mtxIn._14_24_34);

			float4x4 matrixNew;

			matrixNew._11_21_31_41 = float4(w2oRotation._11_21_31, 0.0f);
			matrixNew._12_22_32_42 = float4(w2oRotation._12_22_32, 0.0f);
			matrixNew._13_23_33_43 = float4(w2oRotation._13_23_33, 0.0f);
			matrixNew._14_24_34_44 = float4(w2oPosition, 1.0f);

			return matrixNew;			
		}
		
        void vert (inout appdata_full v, out Input o)
        {

		#ifdef INSTANCENATOR_FOLIAGE		
			v.vertex.xyz *= GetInstanceScale();			
		#endif

			v.vertex.xyz *= _ConstantScale;
			
			Wind(v);
			
			
			
			//#ifdef INSTANCENATOR_FOLIAGE
			//v.vertex.xyz = v.vertex.xyz * GetInstanceVertexScale();
			//#endif
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			float distanceToCamera = length(worldPos - _WorldSpaceCameraPos.xyz);

			float A = (_CutoffMax - _CutoffMin)/(_CutoffMinDistance - _CutoffMaxDistance);
			float B = _CutoffMin - A * _CutoffMaxDistance;
			float cutoff = A * distanceToCamera + B;
			o.Cutoff = clamp(cutoff, _CutoffMin, _CutoffMax);
        }


		
#endif
		
		