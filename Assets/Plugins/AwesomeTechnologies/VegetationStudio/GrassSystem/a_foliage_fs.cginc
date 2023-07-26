#ifndef A_FOLIAGE_FS
#define A_FOLIAGE_FS

		sampler2D _MainTex;
	#if defined(NORMAL_MAP_ON)
		sampler2D _BumpMap;
	#endif
		sampler2D _SpecTex;
			
		fixed3 _SpecularReflectivity;
		fixed _BackfaceSmoothness;
		fixed _Cutoff2;
		fixed _Smoothness;
	
	#if defined(EMISSION_GRASS_ON)
		float _EmissionTodFrequency;
		float _EmissionTodSize;
		float _EmissionTodScale;
		float _EmissionTodMinimum;
		float4 _EmissionColor;
		float4 _EmissionTodPower;
		float _EmissionCurve;
	#endif
	
 		
        void surf(Input IN, inout SurfaceOutputAFSSpecular o)
        {
			half4 c = tex2D (_MainTex, IN.uv_MainTex.xy);

			clip(c.a - IN.Cutoff);
				
			o.Albedo = c.rgb * IN.color.rgb;
			o.Alpha = c.a;
			
			fixed4 specTex = tex2D(_SpecTex, IN.uv_MainTex.xy);
			o.Smoothness = (1 - specTex.a) * _Smoothness;
			
			o.Specular = _SpecularReflectivity;// * (1-o.Smoothness);
			//o.Smoothness = (IN.facingSign > 0) ? o.Smoothness : o.Smoothness * _BackfaceSmoothness;
				
		#if defined(NORMAL_MAP_ON)
			fixed4 normal = tex2D(_BumpMap, IN.uv_MainTex.xy);
			o.Normal = UnpackNormalDXT5nm(normal);
				#if defined(NORMAL_ON)
					o.VertexNormal = WorldNormalVector(IN, half3(0,0,1) );
				#endif
		#endif
				
				
		#if defined(EMISSION_GRASS_ON)
			half posSin = (_EmissionTodSize * sin(pivotOffset + _Time.w * _EmissionTodFrequency + IN.worldPos.x / _EmissionTodScale + IN.worldPos.z / _EmissionTodScale) + _EmissionTodMinimum) * _EmissionTodPower[_EmissionCurve];
			o.Emission = _EmissionColor * specTex.rgb * posSin;
		#else
			o.Emission = 0;
		#endif

        }
		
		
		
#endif
		
		