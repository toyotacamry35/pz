// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

		
		sampler2D _MainTex;
		sampler2D _Bump;
		fixed4 _Color;
		float _YRotation;
		float _XTurnFix;
		float _VS_CullDistance;
		float _VS_FarCullDistance;
		float _Brightness;
		int _InRow;
		int _InCol;
		int _CameraType;
		float4 _VS_CameraPosition;
		
		#ifdef AT_HUE_VARIATION_ON
			half4 _HueVariation;
		#endif
		
		struct Input
		{
			float2 uv_MainTex;
			float4 d;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)
		
		//#define AT_CYLINDRICAL_MODE
		
		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			
			#ifdef AT_CHUNK
				float4 CENTER = v.vertex;
				float3 CORNER = v.normal * v.texcoord2.x;
			#else
				float4 CENTER = float4(0,0,0,1);
				float3 CORNER = v.vertex.xyz;	
			#endif
			
			#define cameraPos _WorldSpaceCameraPos.xyz

			float3 clipVect;
			clipVect = mul(unity_ObjectToWorld, CENTER).xyz - _VS_CameraPosition;

			#if defined(UNITY_PASS_SHADOWCASTER)
				
				//float3 camVect = _WorldSpaceLightPos0.w < 0.5 ? _WorldSpaceLightPos0.xyz : mul(unity_ObjectToWorld, CENTER).xyz - _WorldSpaceLightPos0.xyz;
				float3 camVect;

				if (unity_MatrixVP[3][3] == 1)
					camVect = _WorldSpaceLightPos0.w < 0.5 ? _WorldSpaceLightPos0.xyz : mul(unity_ObjectToWorld, CENTER).xyz - _WorldSpaceLightPos0.xyz;
				else
					camVect = mul(unity_ObjectToWorld, CENTER).xyz - cameraPos;

				#define camVectEvenInShadows (mul(unity_ObjectToWorld, CENTER).xyz - cameraPos)			
			#else
				
				float3 camVect = mul(unity_ObjectToWorld, CENTER).xyz - cameraPos;
				#define camVectEvenInShadows camVect
				
			#endif
			
			//if(length(camVectEvenInShadows) < _CullDistance || length(camVectEvenInShadows) > _FarCullDistance)
			if (length(clipVect) < _VS_CullDistance || length(clipVect) > _VS_FarCullDistance)
			{
				CORNER.xyz *= 0;
			}
			else
			{
				//#ifdef AT_CYLINDRICAL_ON
				//	camVect.y = 0;
				//#endif
				
				// Create LookAt matrix
				float3 zaxis = normalize(camVect);			
				float3 xaxis = normalize(cross(float3(0, 1, 0), zaxis));
				float3 yaxis = cross(zaxis, xaxis);

				float4x4 lookatMatrix = {
					xaxis.x,            yaxis.x,            zaxis.x,       0,
					xaxis.y,            yaxis.y,            zaxis.y,       0,
					xaxis.z,            yaxis.z,            zaxis.z,       0,
					0, 0, 0,  1
				};
				
				#ifdef AT_CHUNK
					v.vertex = mul(lookatMatrix, float4(CORNER.x, CORNER.y, (yaxis.y - 1.0) * v.texcoord2.y, 1));
				#else
					v.vertex = mul(lookatMatrix, float4(CORNER.x, CORNER.y, (yaxis.y - 1.0) * _XTurnFix, 1));
				#endif
			
				v.vertex.xyz += CENTER.xyz;
				
				v.normal = -zaxis;
				v.tangent.xyz = xaxis;
				v.tangent.w=-1;
				
				v.texcoord.x /= _InRow;
				v.texcoord.y /= _InCol;
				
				float angle;
				float step;
				
				//#ifdef AT_SINGLEROW_OFF
					float2 atanDir = normalize(float2(-zaxis.z, -zaxis.x));
					angle = (atan2(atanDir.y, atanDir.x) / 6.28319) + 0.5; // angle around Y in range 0....1
					
					#ifdef AT_CHUNK
						angle += v.texcoord1.x;
					#else
						angle += _YRotation;
					#endif
					
					angle -= (int)angle;
					
					step = 1.0 / _InRow;
					
					v.texcoord.x += step * ((int)((angle + step * 0.5) * _InRow));
				//#endif
				
				//#ifdef AT_SINGLECOLUMN_OFF
					step = 1.0 / _InCol;
					
					angle = saturate(dot(-zaxis,float3(0,1,0)));
					
					angle = clamp(angle,0,step*(_InCol-1));
					
					v.texcoord.y += step * ((int)((angle + step * 0.5) * _InCol));
				//#endif
				
				o.d.x = v.texcoord1.y;
				
				#ifdef AT_HUE_VARIATION_ON
					float hueVariationAmount = frac(CENTER.x + CENTER.y + CENTER.z);
					//hueVariationAmount += frac(IN.vertex.x + IN.normal.y + IN.normal.x) * 0.5 - 0.3;
					o.d.y = saturate(hueVariationAmount * _HueVariation.a);
				#endif
				
			}
		}
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 c = tex2Dbias(_MainTex, half4(IN.uv_MainTex,0,-2)) * _Color;
			
			#ifdef AT_HUE_VARIATION_ON
				half3 shiftedColor = lerp(c.rgb, _HueVariation.rgb, IN.d.y);
				half maxBase = max(c.r, max(c.g, c.b));
				half newMaxBase = max(shiftedColor.r, max(shiftedColor.g, shiftedColor.b));
				maxBase /= newMaxBase;
				maxBase = maxBase * 0.5f + 0.5f;
				// preserve vibrance
				shiftedColor.rgb *= maxBase;
				c.rgb = saturate(shiftedColor);
			#endif
			
			#ifdef AT_CHUNK
				o.Albedo = c.rgb * IN.d.x *_Color;
			#else
				o.Albedo = c.rgb * _Color;
			#endif
			
  		    o.Albedo = clamp(o.Albedo * _Brightness, 0, 1);

			o.Normal = tex2D (_Bump, IN.uv_MainTex).rgb  * 2.0 - 1.0;
			//o.Normal.z = abs(o.Normal.z);
			o.Alpha = c.a;
		}
