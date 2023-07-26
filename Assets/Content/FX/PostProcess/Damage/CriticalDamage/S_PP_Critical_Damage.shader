// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.2693381,fgcg:0.4401155,fgcb:0.4970229,fgca:1,fgde:0.005086312,fgrn:0,fgrf:600,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:36933,y:33287,varname:node_2865,prsc:2|emission-5231-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:33529,y:33430,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3687,x:33780,y:33391,varname:node_3687,prsc:2,ntxv:0,isnm:False|TEX-4430-TEX;n:type:ShaderForge.SFN_Desaturate,id:5119,x:34257,y:33393,varname:node_5119,prsc:2|COL-3687-RGB,DES-8960-OUT;n:type:ShaderForge.SFN_Slider,id:1175,x:33372,y:33693,ptovrint:False,ptlb:sceneDesaturation,ptin:_sceneDesaturation,varname:_sceneDesaturation,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:9197,x:33443,y:33956,ptovrint:False,ptlb:finalMix,ptin:_finalMix,varname:_finalMix,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Power,id:110,x:35405,y:33403,varname:node_110,prsc:2|VAL-5119-OUT,EXP-7158-OUT;n:type:ShaderForge.SFN_Slider,id:4759,x:34424,y:33533,ptovrint:False,ptlb:scenePow,ptin:_scenePow,varname:_scenePow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:11;n:type:ShaderForge.SFN_Lerp,id:7158,x:35171,y:33506,varname:node_7158,prsc:2|A-6409-OUT,B-5139-OUT,T-9197-OUT;n:type:ShaderForge.SFN_Vector1,id:6409,x:34946,y:33457,varname:node_6409,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:8960,x:34001,y:33673,varname:node_8960,prsc:2|A-2824-OUT,B-198-OUT,T-9197-OUT;n:type:ShaderForge.SFN_Vector1,id:2824,x:33706,y:33616,varname:node_2824,prsc:2,v1:0;n:type:ShaderForge.SFN_Smoothstep,id:5092,x:35620,y:34297,varname:node_5092,prsc:2|A-5289-OUT,B-6529-OUT,V-543-OUT;n:type:ShaderForge.SFN_RemapRange,id:5935,x:35093,y:34396,varname:node_5935,prsc:2,frmn:0,frmx:1,tomn:-0.5,tomx:0.5|IN-1510-UVOUT;n:type:ShaderForge.SFN_ScreenPos,id:1510,x:34918,y:34404,varname:node_1510,prsc:2,sctp:2;n:type:ShaderForge.SFN_Vector1,id:6529,x:35330,y:34322,varname:node_6529,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:6447,x:34510,y:34038,ptovrint:False,ptlb:vignette,ptin:_vignette,varname:node_6447,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Power,id:3424,x:35147,y:34166,varname:node_3424,prsc:2|VAL-871-OUT,EXP-1444-OUT;n:type:ShaderForge.SFN_Vector1,id:1444,x:34983,y:34198,varname:node_1444,prsc:2,v1:0.4;n:type:ShaderForge.SFN_OneMinus,id:5289,x:35315,y:34166,varname:node_5289,prsc:2|IN-3424-OUT;n:type:ShaderForge.SFN_Length,id:543,x:35315,y:34396,varname:node_543,prsc:2|IN-5935-OUT;n:type:ShaderForge.SFN_Lerp,id:4413,x:36144,y:33401,varname:node_4413,prsc:2|A-110-OUT,B-9198-OUT,T-7716-OUT;n:type:ShaderForge.SFN_Multiply,id:7716,x:35773,y:34174,varname:node_7716,prsc:2|A-1939-OUT,B-5092-OUT;n:type:ShaderForge.SFN_Vector1,id:1939,x:35595,y:34152,varname:node_1939,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Slider,id:2799,x:35359,y:33664,ptovrint:False,ptlb:vignetteDarken,ptin:_vignetteDarken,varname:node_2799,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:9198,x:35872,y:33471,varname:node_9198,prsc:2|A-110-OUT,B-5658-OUT;n:type:ShaderForge.SFN_OneMinus,id:5658,x:35686,y:33664,varname:node_5658,prsc:2|IN-2799-OUT;n:type:ShaderForge.SFN_Lerp,id:5231,x:36714,y:33393,varname:node_5231,prsc:2|A-4413-OUT,B-8056-RGB,T-3714-OUT;n:type:ShaderForge.SFN_Color,id:8056,x:36258,y:33137,ptovrint:False,ptlb:Col,ptin:_Col,varname:node_8056,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:1292,x:35885,y:33850,ptovrint:False,ptlb:redLevel,ptin:_redLevel,varname:node_1292,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.15,max:1;n:type:ShaderForge.SFN_Multiply,id:3714,x:36554,y:33507,varname:node_3714,prsc:2|A-9197-OUT,B-3154-OUT;n:type:ShaderForge.SFN_Lerp,id:5139,x:34847,y:33511,varname:node_5139,prsc:2|A-260-OUT,B-4759-OUT,T-1236-OUT;n:type:ShaderForge.SFN_Vector1,id:260,x:34533,y:33412,varname:node_260,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:1236,x:32605,y:34291,ptovrint:False,ptlb:statScale,ptin:_statScale,varname:node_1236,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:0.5,max:1;n:type:ShaderForge.SFN_Multiply,id:198,x:33783,y:33689,varname:node_198,prsc:2|A-1175-OUT,B-1236-OUT;n:type:ShaderForge.SFN_Multiply,id:871,x:34843,y:34038,varname:node_871,prsc:2|A-6447-OUT,B-1236-OUT;n:type:ShaderForge.SFN_Multiply,id:3154,x:36301,y:33755,varname:node_3154,prsc:2|A-1236-OUT,B-1292-OUT;proporder:4430-4759-1175-9197-6447-2799-8056-1292-1236;pass:END;sub:END;*/

Shader "Colony_FX/PostProcess/S_PP_Critical_Damage" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _scenePow ("scenePow", Range(0, 11)) = 1
        _sceneDesaturation ("sceneDesaturation", Range(0, 1)) = 0
        _finalMix ("finalMix", Range(0, 1)) = 0
        _vignette ("vignette", Range(0, 2)) = 0
        _vignetteDarken ("vignetteDarken", Range(0, 1)) = 0
        _Col ("Col", Color) = (0.5,0.5,0.5,1)
        _redLevel ("redLevel", Range(0, 1)) = 0.15
        _statScale ("statScale", Range(0.5, 1)) = 0.5
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
            uniform float _sceneDesaturation;
            uniform float _finalMix;
            uniform float _scenePow;
            uniform float _vignette;
            uniform float _vignetteDarken;
            uniform float4 _Col;
            uniform float _redLevel;
            uniform float _statScale;
            struct VertexInput {
                float4 vertex : POSITION;
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
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
////// Lighting:
////// Emissive:
                float4 node_3687 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_110 = pow(lerp(node_3687.rgb,dot(node_3687.rgb,float3(0.3,0.59,0.11)),lerp(0.0,(_sceneDesaturation*_statScale),_finalMix)),lerp(1.0,lerp(1.0,_scenePow,_statScale),_finalMix));
                float3 emissive = lerp(lerp(node_110,(node_110*(1.0 - _vignetteDarken)),(1.5*smoothstep( (1.0 - pow((_vignette*_statScale),0.4)), 1.0, length((sceneUVs.rg*1.0+-0.5)) ))),_Col.rgb,(_finalMix*(_statScale*_redLevel)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
