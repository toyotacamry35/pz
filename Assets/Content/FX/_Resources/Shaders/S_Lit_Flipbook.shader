// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33048,y:32689,varname:node_4795,prsc:2|spec-10-OUT,emission-2393-OUT,alpha-3762-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31346,y:32877,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e213b3ad093afba4ca521603fa3c2115,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2393,x:32450,y:32661,varname:node_2393,prsc:2|A-8434-OUT,B-7256-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:31373,y:32554,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Multiply,id:798,x:31956,y:32967,varname:node_798,prsc:2|A-6074-B,B-2053-A;n:type:ShaderForge.SFN_Lerp,id:8434,x:32162,y:32617,varname:node_8434,prsc:2|A-841-RGB,B-6314-RGB,T-6074-R;n:type:ShaderForge.SFN_Slider,id:7256,x:32084,y:32786,ptovrint:False,ptlb:emissive,ptin:_emissive,varname:node_7256,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.68,max:2;n:type:ShaderForge.SFN_Slider,id:4279,x:32302,y:33396,ptovrint:False,ptlb:distance,ptin:_distance,varname:node_4279,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2205665,max:1;n:type:ShaderForge.SFN_DepthBlend,id:6371,x:32513,y:33145,varname:node_6371,prsc:2|DIST-4279-OUT;n:type:ShaderForge.SFN_Multiply,id:5969,x:32513,y:32971,varname:node_5969,prsc:2|A-3572-OUT,B-6371-OUT;n:type:ShaderForge.SFN_Color,id:841,x:31676,y:32432,ptovrint:False,ptlb:Color1,ptin:_Color1,varname:node_841,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:6314,x:31676,y:32608,ptovrint:False,ptlb:Color2,ptin:_Color2,varname:node_6314,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:10,x:32590,y:32790,varname:node_10,prsc:2,v1:0;n:type:ShaderForge.SFN_Clamp01,id:3762,x:32750,y:32971,varname:node_3762,prsc:2|IN-5969-OUT;n:type:ShaderForge.SFN_Multiply,id:3572,x:32207,y:32977,varname:node_3572,prsc:2|A-798-OUT,B-2970-OUT;n:type:ShaderForge.SFN_Slider,id:2970,x:31907,y:33173,ptovrint:False,ptlb:opacity,ptin:_opacity,varname:node_2970,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;proporder:6074-7256-4279-841-6314-2970;pass:END;sub:END;*/

Shader "Colony_FX/Garbage/S_Lit_Flipbook" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _emissive ("emissive", Range(0, 2)) = 0.68
        _distance ("distance", Range(0, 1)) = 0.2205665
        _Color1 ("Color1", Color) = (0.5,0.5,0.5,1)
        _Color2 ("Color2", Color) = (0.5,0.5,0.5,1)
        _opacity ("opacity", Range(0, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
			#define DS_HAZE_FULL
            #include "UnityCG.cginc"
			#include "Assets/ASkyLighting/DeepSky Haze/Resources/DS_TransparentLib.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _emissive;
            uniform float _distance;
            uniform float4 _Color1;
            uniform float4 _Color2;
            uniform float _opacity;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
				float3 air : TEXCOORD3;
				float3 hazeAndFog : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
				DS_Haze_Per_Vertex(v.vertex, o.air, o.hazeAndFog);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 emissive = (lerp(_Color1.rgb,_Color2.rgb,_MainTex_var.r)*_emissive);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,saturate((((_MainTex_var.b*i.vertexColor.a)*_opacity)*saturate((sceneZ-partZ)/_distance))));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
				DS_Haze_Apply(i.air, i.hazeAndFog, finalRGBA, finalRGBA.a);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
