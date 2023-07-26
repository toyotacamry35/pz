// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33054,y:33288,varname:node_2865,prsc:2|emission-4676-OUT;n:type:ShaderForge.SFN_TexCoord,id:4219,x:30822,y:32920,cmnt:Default coordinates,varname:node_4219,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Relay,id:4676,x:32702,y:33404,cmnt:Modify color here,varname:node_4676,prsc:2|IN-7542-RGB;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:32160,y:33470,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:node_9933,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7542,x:32380,y:33378,varname:node_1672,prsc:2,ntxv:0,isnm:False|UVIN-1161-OUT,TEX-4430-TEX;n:type:ShaderForge.SFN_Multiply,id:7744,x:31107,y:32706,varname:node_7744,prsc:2|A-6739-OUT,B-4219-UVOUT;n:type:ShaderForge.SFN_Vector2,id:6739,x:30822,y:32702,varname:node_6739,prsc:2,v1:1920,v2:1080;n:type:ShaderForge.SFN_Slider,id:8479,x:30822,y:32527,ptovrint:False,ptlb:resample,ptin:_resample,varname:node_8479,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1024;n:type:ShaderForge.SFN_Multiply,id:3398,x:31357,y:32873,varname:node_3398,prsc:2|A-4219-U,B-8479-OUT;n:type:ShaderForge.SFN_Floor,id:2536,x:31541,y:32873,varname:node_2536,prsc:2|IN-3398-OUT;n:type:ShaderForge.SFN_Divide,id:8426,x:31749,y:32873,varname:node_8426,prsc:2|A-2536-OUT,B-8479-OUT;n:type:ShaderForge.SFN_Multiply,id:5236,x:31220,y:33056,varname:node_5236,prsc:2|A-8479-OUT,B-5735-OUT;n:type:ShaderForge.SFN_Divide,id:5735,x:31014,y:33076,varname:node_5735,prsc:2|A-9285-OUT,B-3965-OUT;n:type:ShaderForge.SFN_Vector1,id:9285,x:30813,y:33076,varname:node_9285,prsc:2,v1:9;n:type:ShaderForge.SFN_Vector1,id:3965,x:30813,y:33153,varname:node_3965,prsc:2,v1:16;n:type:ShaderForge.SFN_Multiply,id:6713,x:31433,y:33030,varname:node_6713,prsc:2|A-4219-V,B-5236-OUT;n:type:ShaderForge.SFN_Floor,id:3935,x:31656,y:33070,varname:node_3935,prsc:2|IN-6713-OUT;n:type:ShaderForge.SFN_Divide,id:2999,x:31852,y:33070,varname:node_2999,prsc:2|A-3935-OUT,B-5236-OUT;n:type:ShaderForge.SFN_Append,id:1161,x:32089,y:32975,varname:node_1161,prsc:2|A-8426-OUT,B-2999-OUT;proporder:4430-8479;pass:END;sub:END;*/

Shader "Shader Forge/S_8bit" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _resample ("resample", Range(0, 1024)) = 1
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
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _resample;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float node_5236 = (_resample*(9.0/16.0));
                float2 node_1161 = float2((floor((i.uv0.r*_resample))/_resample),(floor((i.uv0.g*node_5236))/node_5236));
                float4 node_1672 = tex2D(_MainTex,TRANSFORM_TEX(node_1161, _MainTex));
                float3 emissive = node_1672.rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
