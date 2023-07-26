Shader "Colony_FX/PostProcess/S_PP_HeatDistortion" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _DistTex ("NoiseTex", 2D) = "white" {}
        _distPanX ("distPanX", Range(-3, 3)) = 0
        _distPanY ("distPanY", Range(-3, 3)) = 0
        _distTile ("distTile", Range(0, 100)) = 1
        _distIntensity ("distIntensity", Range(0, 1)) = 0
		_depthPow ("depthPow", Range(0, 1)) = 1
		_depthCutoff("depthCutoff", Range(0,1)) = 1
		_doubleVision("doubleVision", Range(0,5)) = 0
		_overallIntensity("finalMix", Range(0,1)) = 1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Overlay+1"
            "RenderType"="Overlay"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZTest Always
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
			uniform sampler2D _CameraDepthTexture; 
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _DistTex; uniform float4 _DistTex_ST;

            uniform float _distPanX;
            uniform float _distPanY;
            uniform float _distTile;
            uniform float _distIntensity;
			uniform float _depthPow;
			uniform float _depthCutoff;
			uniform float _doubleVision;
			uniform float _overallIntensity;
			
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
				float4 projPos : TEXCOORD1; 
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
				o.projPos = ComputeScreenPos(o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {

                //depth tex from camera 
				float depth = Linear01Depth (tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(i.projPos)).r);
				//depth modification to cut off far and near values
				depth = clamp( pow (depth, _depthPow), 0, _depthCutoff);
				
				//uvs for distortion texture
                float2 uv2DistTex = ((i.uv0*_distTile)+frac((_Time.g*float2(_distPanX,_distPanY))));
                
				//dist texture sample
				float4 dist = tex2D(_DistTex,TRANSFORM_TEX( uv2DistTex, _DistTex));
                
				//distorted uvs for scene texture, later fed into main tex uvs
				float2 uv2MainTex = _overallIntensity * depth*(_distIntensity*.1*dist.rg)+i.uv0;
                
				//distorted scene texture 
				float4 main = tex2D(_MainTex,TRANSFORM_TEX(uv2MainTex, _MainTex));
				//original scene texture, offset by a small value
				float4 main0 = tex2D(_MainTex, TRANSFORM_TEX(i.uv0 + float2(.0035,.0035), _MainTex));
				
				//distorted scene + original scene with depth mask
                float3 emissive = main.rgb + main0 * depth * _doubleVision * _overallIntensity;				              
                return fixed4(emissive,1);
            }
            ENDCG
        }
    }
}
