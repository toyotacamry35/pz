// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32860,y:32711,varname:node_3138,prsc:2|diff-9503-OUT,emission-9503-OUT,alpha-4371-OUT;n:type:ShaderForge.SFN_Tex2d,id:1367,x:31749,y:33025,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_1367,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_VertexColor,id:2808,x:31712,y:32703,varname:node_2808,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4371,x:32220,y:33037,varname:node_4371,prsc:2|A-2808-A,B-1367-R;n:type:ShaderForge.SFN_Multiply,id:2730,x:32248,y:32711,varname:node_2730,prsc:2|A-2808-RGB,B-943-OUT;n:type:ShaderForge.SFN_Multiply,id:8746,x:32439,y:32711,varname:node_8746,prsc:2|A-2730-OUT,B-8093-OUT;n:type:ShaderForge.SFN_Slider,id:8093,x:32220,y:32883,ptovrint:False,ptlb:brightness,ptin:_brightness,varname:node_8093,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Clamp01,id:9503,x:32621,y:32711,varname:node_9503,prsc:2|IN-8746-OUT;n:type:ShaderForge.SFN_Vector1,id:7442,x:32626,y:33175,varname:node_7442,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Multiply,id:5974,x:32516,y:33066,varname:node_5974,prsc:2|A-4371-OUT,B-636-OUT;n:type:ShaderForge.SFN_Vector1,id:636,x:32374,y:33113,varname:node_636,prsc:2,v1:5;n:type:ShaderForge.SFN_Power,id:943,x:32056,y:32742,varname:node_943,prsc:2|VAL-1367-R,EXP-6529-OUT;n:type:ShaderForge.SFN_Vector1,id:6529,x:31969,y:32906,varname:node_6529,prsc:2,v1:1;proporder:1367-8093;pass:END;sub:END;*/

Shader "Colony_FX/Basic/S_AlphaBlend_Unlit" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _brightness ("brightness", Range(0, 5)) = 1
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
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _brightness;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
				float3 air : TEXCOORD1;
				float3 hazeAndFog : TEXCOORD2;
				float4 projPos : TEXCOORD3;
				UNITY_FOG_COORDS(4)

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
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_9503 = saturate(((i.vertexColor.rgb*pow(_MainTex_var.r,1.0))*_brightness));
                float3 emissive = node_9503;
                float3 finalColor = emissive;
                float node_4371 = (i.vertexColor.a*_MainTex_var.r);
				fixed4 finalRGBA = fixed4(finalColor,node_4371);
				UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, unity_FogColor);
				DS_Haze_Apply(i.air, i.hazeAndFog, finalRGBA, finalRGBA.a);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
