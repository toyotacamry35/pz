Shader "ASky/Planet" 
{
	Properties
	{
		_MainTex("Diffuse Map", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_SpecMap("Specular Map", 2D) = "white" {}
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_Shininess("Shininess", Range(1, 50)) = 20
		_LightMap("Night lights Map", 2D) = "black" {}
		_LightMapIntensity("Night lights Intensity", Float) = 1.0
		_CloudMap("Cloud Map", 2D) = "black" {}
		
		_CloudSpeed("Cloud speed", Float) = 0.01
		_CloudStrength("Cloud strength", Range(0, 5)) = 1.0
		_CloudShadowsStrength("Cloud shadows strength", Range(0, 1)) = 0.0
		
		_CloudMap02("Cloud Map 02", 2D) = "black" {}
		_CloudSpeed02("Cloud speed 02", Float) = 0.01
		_CloudStrength02("Cloud strength 02", Range(0, 5)) = 1.0
		_CloudShadowsStrength02("Cloud shadows strength 02", Range(0, 1)) = 0.0
		
		_HighlightOffset("_HighlightOffset", Range(0.005, 0.1)) = 0.025
		_DistanceBlending("_DistanceBlending", Range(0.0001, 0.01)) = 0.0025
		[HDR]
		_SunColor("Sun Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags
		{
			"LightMode" = "ForwardBase"
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Pass
		{
			Cull Back
			ZWrite On

			Blend One One
			CGPROGRAM
			#include "UnityCG.cginc"

			#include "ASkyPlanetShared.cginc"
			#pragma vertex vert  
			#pragma fragment frag
			#pragma target 4.6
			
			struct vertexInput
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct vertexOutput {
				float4	pos : SV_POSITION;
				float4	posWorld : TEXCOORD0;
				float4	tex : TEXCOORD1;
				float3	tangentWorld : TEXCOORD2;
				float3	normalWorld : TEXCOORD3;
				float3	binormalWorld : TEXCOORD4;
				float3	c0 : COLOR0;
				float3	c1 : COLOR1;		
			};
			
			

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;

				output.tangentWorld = normalize(mul(unity_ObjectToWorld, float4(input.tangent.xyz, 0.0)).xyz);
				output.normalWorld = normalize(mul(float4(input.normal, 0.0), unity_WorldToObject).xyz);
				output.binormalWorld = normalize(cross(output.normalWorld, output.tangentWorld)	* input.tangent.w); 

				float3 objectPos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
				
				output.posWorld = mul(unity_ObjectToWorld, input.vertex);
				output.tex = input.texcoord;
				output.pos = UnityObjectToClipPos(input.vertex);
				
				float3 v3CameraPos = _WorldSpaceCameraPos - objectPos;
				float fCameraHeight = length(v3CameraPos);
				float fCameraHeight2 = fCameraHeight*fCameraHeight;

				float3 v3Pos = mul(unity_ObjectToWorld, input.vertex).xyz - objectPos;
				float3 v3Ray = v3Pos - v3CameraPos;
				float fFar = length(v3Ray);
				v3Ray /= fFar;

				float B = 2.0 * dot(v3CameraPos, v3Ray);
				float C = fCameraHeight2 - fOuterRadius * fOuterRadius;
				float fDet = max(0.0, B*B - 4.0 * C);
				float fNear = 0.5 * (-B - sqrt(fDet));

				float3 v3Start = v3CameraPos + v3Ray * fNear;
				fFar -= fNear;
				float fDepth = exp((fInnerRadius - fOuterRadius) / fScaleDepth);
				float fCameraAngle = dot(-v3Ray, v3Pos) / length(v3Pos);
				float fLightAngle = dot(normalize(_SunDirection), v3Pos) / length(v3Pos);
				float fCameraScale = scale(fCameraAngle);
				float fLightScale = scale(fLightAngle);
				float fCameraOffset = fDepth*fCameraScale;
				float fTemp = (fLightScale + fCameraScale);

				const float fSamples = 2.0;

				float fSampleLength = fFar / fSamples;
				float fScaledLength = fSampleLength * fScale;
				float3 v3SampleRay = v3Ray * fSampleLength;
				float3 v3SamplePoint = v3Start + v3SampleRay * 0.5;

				float3 v3FrontColor = float3(0.0, 0.0, 0.0);
				float3 v3Attenuate;
				for (int i = 0; i<int(fSamples); i++)
				{
					float fHeight = length(v3SamplePoint);
					float fDepth = exp(fScaleOverScaleDepth * (fInnerRadius - fHeight));
					float fScatter = fDepth*fTemp - fCameraOffset;
					v3Attenuate = exp(-fScatter * (invWavelength.xxx * fKr4PI + fKm4PI));
					v3FrontColor += v3Attenuate * (fDepth * fScaledLength);
					v3SamplePoint += v3SampleRay;
				}
				output.c0 = v3FrontColor * (invWavelength.xxx * fKrESun + fKmESun);
				output.c1 = v3Attenuate;
				
				return output;
			}

			float4 frag(vertexOutput input) : COLOR
			{
				
				half3 ray = normalize(input.posWorld.xyz);	
				sunColor = _SunColor;
				
				float2 w = float2(0,0);
				w = GetPlanetShadows(input.posWorld);
				
				sunColor *= (1.2 - w.x);
				sunColor *= (1.2 - w.y);
										
				float4 encodedNormal;
				float3 localCoords;

				encodedNormal = tex2D(_BumpMap,	_BumpMap_ST.xy *  input.tex.xy + _BumpMap_ST.zw);
				localCoords = float3(2.0 * encodedNormal.a - 1.0, 2.0 * encodedNormal.g - 1.0, 0.0);
				localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));

				float3x3 local2WorldTranspose = float3x3(input.tangentWorld,input.binormalWorld,input.normalWorld);
				float3 normalDirection = normalize(mul(localCoords, local2WorldTranspose));

				float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
				float3 lightDirection;

				lightDirection = normalize(_SunDirection);

				float3 diff = sunColor * max(0, dot(lightDirection, normalize(normalDirection + (0.2 * lightDirection))));
				float3 spec;

				spec = _SpecColor.rgb * pow(max(0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);

				float3 diffWn = sunColor * max(0, dot(lightDirection, normalize(input.normalWorld + (0.2 * lightDirection))));

				float3 c = diff * pow(tex2D(_MainTex, input.tex.xy), _AtmoThickness);
				c += spec * diff * pow(tex2D(_SpecMap, input.tex.xy).r, _AtmoThickness);

				float2 uvCloud = input.tex.xy;
				float2 uvCloud02 = input.tex.xy;
				uvCloud.x += _Time * _CloudSpeed;
				uvCloud02.x += _Time * _CloudSpeed02;
				fixed3 cloud = -pow(tex2D(_CloudMap, uvCloud), _AtmoThickness) * _CloudShadowsStrength * _CloudStrength; // add cloud shadows
				fixed3 cloud2 = -pow(tex2D(_CloudMap02, uvCloud02), _AtmoThickness) * _CloudShadowsStrength02 * _CloudStrength02; // add cloud shadows
				uvCloud.x += 0.001;
				uvCloud02.x += 0.001;
				cloud += pow(tex2D(_CloudMap, uvCloud), _AtmoThickness) * (1 + _CloudShadowsStrength) * _CloudStrength; // add cloud
				cloud2 += pow(tex2D(_CloudMap02, uvCloud02), _AtmoThickness) * (1 + _CloudShadowsStrength02) * _CloudStrength02; // add cloud
				cloud *= diffWn;
				cloud2 *= diffWn;
				
				#if ATMO_ON
					cloud *= input.c1;
					cloud2 *= input.c1;
				#endif
				
				c += cloud;
				c += cloud2;
				
				#if ATMO_ON
					c += input.c0 * sunColor;
				#endif
				
				c += saturate(0.2 - diff) * _LightMapIntensity * pow(tex2D(_LightMap, input.tex.xy), _AtmoThickness);
				half3 result = clamp(c,0,3);
				return half4(result, 1.0);

			}
		ENDCG
		}
		
		
		Pass
		{
			Blend One One
			ZWrite On
			Cull Front

			CGPROGRAM
			#include "UnityCG.cginc"
			#include "ASkyPlanetShared.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile ATMO_ON __
			#pragma target 4.6

			struct v2f
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				float3 	t0 : TEXCOORD1;
				float3 	c0 : COLOR0;
				float3 	c1 : COLOR1;
				float3 	posWorld : COLOR2;
			};

			v2f vert(appdata_base v)
			{
				v2f OUT;

				OUT.c0 = 0;
				OUT.c1 = 0;
				OUT.t0 = 0;
			#if ATMO_ON
				
				float3 objectPos = float3(unity_ObjectToWorld[0].w, unity_ObjectToWorld[1].w, unity_ObjectToWorld[2].w);
				v.vertex.xyz += v.normal * _HighlightOffset;
				float3 v3CameraPos = _WorldSpaceCameraPos - objectPos;
				float fCameraHeight = length(v3CameraPos);
				float fCameraHeight2 = fCameraHeight * fCameraHeight;

				float3 v3Pos = mul(unity_ObjectToWorld, v.vertex).xyz - objectPos;
				float3 v3Ray = v3Pos - v3CameraPos;
				float fFar = length(v3Ray);
				v3Ray /= fFar;

				float B = 2.0 * dot(v3CameraPos, v3Ray);
				float C = fCameraHeight2 - fOuterRadius * fOuterRadius;
				float fDet = max(0.0, B*B - 4.0 * C);
				float fNear = 0.5 * (-B - sqrt(fDet));

				float3 v3Start = v3CameraPos + v3Ray * fNear;
				fFar -= fNear;
				float fStartAngle = dot(v3Ray, v3Start) / fOuterRadius;
				float fStartDepth = exp(-1.0 / fScaleDepth);
				float fStartOffset = fStartDepth*scale(fStartAngle);

				const float fSamples = 2.0;

				float fSampleLength = fFar / fSamples;
				float fScaledLength = fSampleLength * fScale;
				float3 v3SampleRay = v3Ray * fSampleLength;
				float3 v3SamplePoint = v3Start + v3SampleRay * 0.5;

				float3 v3FrontColor = float3(0.0, 0.0, 0.0);
				for (int i = 0; i<int(fSamples); i++)
				{
					float fHeight = length(v3SamplePoint);
					float fDepth = exp(fScaleOverScaleDepth * (fInnerRadius - fHeight));
					float fLightAngle = dot(normalize(_SunDirection), v3SamplePoint) / fHeight;
					float fCameraAngle = dot(v3Ray, v3SamplePoint) / fHeight;
					float fScatter = (fStartOffset + fDepth*(scale(fLightAngle) - scale(fCameraAngle)));
					float3 v3Attenuate = exp(-fScatter * (invWavelength.xxx * fKr4PI + fKm4PI));
					v3FrontColor += v3Attenuate * (fDepth * fScaledLength);
					v3SamplePoint += v3SampleRay;
				}
				OUT.c0 = v3FrontColor * (invWavelength.xxx * fKrESun);
				OUT.c1 = v3FrontColor * fKmESun;
				OUT.t0 = (v3CameraPos - v3Pos);
			#endif

				OUT.pos = UnityObjectToClipPos(v.vertex);
				OUT.uv = v.texcoord.xy;
				OUT.posWorld = mul(unity_ObjectToWorld, v.vertex).xyz;

				return OUT;
			}

			half4 frag(v2f input) : COLOR
			{
				half3	col = 0;
				float	a = 0;
				sunColor = _SunColor;
					
				float2 w = float2(0,0);
				w = GetPlanetShadows(input.posWorld);
				
				sunColor *= (1.2 - w.x);
				sunColor *= (1.2 - w.y);
				
			#if ATMO_ON
				float fCos = dot(_SunDirection, input.t0) / length(input.t0);
				float fCos2 = fCos*fCos;
				col = getRayleighPhase(fCos2) * input.c0 + getMiePhase(fCos, fCos2) * input.c1;
				col *= sunColor;
			#endif
			
				half3 result = clamp(col,0,3);		
				a = Luminance(result);

				return half4(result, a);
			}
		ENDCG
		}
		
	}
}