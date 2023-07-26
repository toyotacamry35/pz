// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/ScaleTilingShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
				float3 worldNormal;
				float3 worldPos;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o) {


				float2 UV;
				fixed4 c;
				/*float4 modelX = float4(1.0, 0.0, 0.0, 0.0);
				float4 modelY = float4(0.0, 1.0, 0.0, 0.0);
				float4 modelZ = float4(0.0, 0.0, 1.0, 0.0);

				float4 modelXInWorld = mul(unity_ObjectToWorld, modelX);
				float4 modelYInWorld = mul(unity_ObjectToWorld, modelY);
				float4 modelZInWorld = mul(unity_ObjectToWorld, modelZ);

				float scaleX = length(modelXInWorld);
				float scaleY = length(modelYInWorld);
				float scaleZ = length(modelZInWorld);*/
				if (abs(IN.worldNormal.x) > 0.5)
				{
					UV = IN.worldPos.yz; // side
					c = tex2D(_MainTex, UV);//* scaleX); // use WALLSIDE texture
				}
				else if (abs(IN.worldNormal.z) > 0.5)
				{
					UV = IN.worldPos.xy; // front
					c = tex2D(_MainTex, UV);//* scaleY); // use WALL texture
				}
				else
				{
					UV = IN.worldPos.xz; // top
					c = tex2D(_MainTex, UV);//* scaleZ); // use FLR texture
				}

				// Albedo comes from a texture tinted by color
				o.Albedo = c.rgb  * _Color;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
