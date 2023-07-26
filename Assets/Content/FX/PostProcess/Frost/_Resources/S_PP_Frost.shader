// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.260621,fgcg:0.3751022,fgcb:0.464309,fgca:1,fgde:0.005089695,fgrn:0,fgrf:600,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:36002,y:31272,varname:node_2865,prsc:2|emission-7343-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:33677,y:31198,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ScreenPos,id:3623,x:31123,y:32519,varname:node_3623,prsc:2,sctp:2;n:type:ShaderForge.SFN_Subtract,id:5033,x:31972,y:32714,varname:node_5033,prsc:2|A-338-OUT,B-6951-OUT;n:type:ShaderForge.SFN_Vector2,id:6951,x:31896,y:32885,varname:node_6951,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Smoothstep,id:6397,x:32460,y:32503,varname:node_6397,prsc:2|A-9775-OUT,B-6745-OUT,V-4376-OUT;n:type:ShaderForge.SFN_Slider,id:1502,x:30909,y:32030,ptovrint:False,ptlb:mainVignette,ptin:_mainVignette,varname:node_1502,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:15;n:type:ShaderForge.SFN_Vector1,id:6745,x:32025,y:32525,varname:node_6745,prsc:2,v1:1;n:type:ShaderForge.SFN_Power,id:7601,x:31937,y:32204,varname:node_7601,prsc:2|VAL-6071-OUT,EXP-9361-OUT;n:type:ShaderForge.SFN_Vector1,id:9361,x:31728,y:32330,varname:node_9361,prsc:2,v1:0.4;n:type:ShaderForge.SFN_OneMinus,id:9775,x:32138,y:32192,varname:node_9775,prsc:2|IN-7601-OUT;n:type:ShaderForge.SFN_Length,id:4376,x:32147,y:32728,varname:node_4376,prsc:2|IN-5033-OUT;n:type:ShaderForge.SFN_OneMinus,id:2786,x:32891,y:32503,varname:node_2786,prsc:2|IN-6759-OUT;n:type:ShaderForge.SFN_Clamp01,id:6759,x:32680,y:32503,varname:node_6759,prsc:2|IN-6397-OUT;n:type:ShaderForge.SFN_Tex2d,id:7169,x:31013,y:32715,ptovrint:False,ptlb:vignetteDistorter,ptin:_vignetteDistorter,varname:node_7169,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9073-OUT;n:type:ShaderForge.SFN_Append,id:7435,x:31195,y:32743,varname:node_7435,prsc:2|A-7169-R,B-7169-G;n:type:ShaderForge.SFN_Slider,id:201,x:31086,y:33006,ptovrint:False,ptlb:vignetteDistorterIntensity,ptin:_vignetteDistorterIntensity,varname:node_201,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:0.5;n:type:ShaderForge.SFN_TexCoord,id:3186,x:30613,y:32722,varname:node_3186,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:9073,x:30821,y:32722,varname:node_9073,prsc:2|A-3186-UVOUT,B-3519-OUT;n:type:ShaderForge.SFN_Slider,id:3519,x:30483,y:32936,ptovrint:False,ptlb:vignetteDistorterTiling,ptin:_vignetteDistorterTiling,varname:node_3519,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Set,id:9714,x:33126,y:32503,varname:mainVignInt,prsc:2|IN-2786-OUT;n:type:ShaderForge.SFN_Tex2d,id:784,x:33796,y:31511,varname:node_784,prsc:2,ntxv:0,isnm:False|TEX-7611-TEX;n:type:ShaderForge.SFN_Tex2d,id:2261,x:32083,y:31562,ptovrint:False,ptlb:frostMaskNormal,ptin:_frostMaskNormal,varname:node_2261,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5213,x:34419,y:30873,varname:node_5213,prsc:2,ntxv:0,isnm:False|UVIN-8390-OUT,TEX-7611-TEX;n:type:ShaderForge.SFN_Append,id:4209,x:32353,y:31583,varname:node_4209,prsc:2|A-2261-R,B-2261-G;n:type:ShaderForge.SFN_Tex2d,id:5589,x:33932,y:31380,varname:node_5589,prsc:2,ntxv:0,isnm:False|UVIN-3610-OUT,TEX-4430-TEX;n:type:ShaderForge.SFN_TexCoord,id:7146,x:32867,y:31382,varname:node_7146,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:3490,x:32756,y:31577,varname:node_3490,prsc:2|A-4209-OUT,B-6720-OUT;n:type:ShaderForge.SFN_Slider,id:6720,x:32336,y:31782,ptovrint:False,ptlb:sceneDistortion,ptin:_sceneDistortion,varname:node_6720,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.576,max:1;n:type:ShaderForge.SFN_Lerp,id:3610,x:33352,y:31382,varname:node_3610,prsc:2|A-7146-UVOUT,B-3396-OUT,T-3480-OUT;n:type:ShaderForge.SFN_Add,id:3396,x:33096,y:31450,varname:node_3396,prsc:2|A-7146-UVOUT,B-3490-OUT;n:type:ShaderForge.SFN_Slider,id:2274,x:34342,y:31716,ptovrint:False,ptlb:diffMaskPow,ptin:_diffMaskPow,varname:node_2274,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_TexCoord,id:1999,x:32734,y:30878,varname:node_1999,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:8390,x:34143,y:30872,varname:node_8390,prsc:2|A-1999-UVOUT,B-8925-OUT,T-1322-OUT;n:type:ShaderForge.SFN_Slider,id:1322,x:33729,y:31019,ptovrint:False,ptlb:diffuseOverlayDistortion,ptin:_diffuseOverlayDistortion,varname:node_1322,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.36,max:1;n:type:ShaderForge.SFN_Lerp,id:6004,x:35173,y:31402,varname:node_6004,prsc:2|A-5589-RGB,B-5213-RGB,T-7639-OUT;n:type:ShaderForge.SFN_Multiply,id:4983,x:34803,y:31543,varname:node_4983,prsc:2|A-8952-OUT,B-7056-OUT;n:type:ShaderForge.SFN_Power,id:8952,x:34578,y:31534,varname:node_8952,prsc:2|VAL-6426-OUT,EXP-2274-OUT;n:type:ShaderForge.SFN_Slider,id:7056,x:34691,y:31728,ptovrint:False,ptlb:diffMaskMult,ptin:_diffMaskMult,varname:node_7056,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Subtract,id:672,x:33405,y:31771,varname:node_672,prsc:2|A-1504-OUT,B-4000-OUT;n:type:ShaderForge.SFN_Slider,id:4000,x:33655,y:32104,ptovrint:False,ptlb:erosion,ptin:_erosion,varname:node_4000,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.113,max:1;n:type:ShaderForge.SFN_Get,id:8400,x:33828,y:31733,varname:node_8400,prsc:2|IN-9714-OUT;n:type:ShaderForge.SFN_RemapRange,id:3173,x:31521,y:32869,varname:node_3173,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-2530-OUT;n:type:ShaderForge.SFN_Lerp,id:338,x:31697,y:32716,varname:node_338,prsc:2|A-3623-UVOUT,B-3173-OUT,T-201-OUT;n:type:ShaderForge.SFN_Add,id:2530,x:31369,y:32743,varname:node_2530,prsc:2|A-3623-UVOUT,B-7435-OUT;n:type:ShaderForge.SFN_Clamp01,id:3480,x:33595,y:31757,varname:node_3480,prsc:2|IN-672-OUT;n:type:ShaderForge.SFN_Add,id:8925,x:32964,y:30983,varname:node_8925,prsc:2|A-1999-UVOUT,B-1246-OUT;n:type:ShaderForge.SFN_Multiply,id:1504,x:34090,y:31534,varname:node_1504,prsc:2|A-784-A,B-444-OUT;n:type:ShaderForge.SFN_OneMinus,id:444,x:34022,y:31719,varname:node_444,prsc:2|IN-8400-OUT;n:type:ShaderForge.SFN_RemapRange,id:1246,x:32663,y:31227,varname:node_1246,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-4209-OUT;n:type:ShaderForge.SFN_Clamp01,id:7639,x:34992,y:31543,varname:node_7639,prsc:2|IN-4983-OUT;n:type:ShaderForge.SFN_Subtract,id:6426,x:34342,y:31534,varname:node_6426,prsc:2|A-1504-OUT,B-23-OUT;n:type:ShaderForge.SFN_Subtract,id:23,x:34089,y:32097,varname:node_23,prsc:2|A-4000-OUT,B-4995-OUT;n:type:ShaderForge.SFN_Vector1,id:4995,x:33916,y:32232,varname:node_4995,prsc:2,v1:0.05;n:type:ShaderForge.SFN_Time,id:988,x:30584,y:31735,varname:node_988,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2428,x:30965,y:31787,varname:node_2428,prsc:2|A-1673-OUT,B-1604-OUT;n:type:ShaderForge.SFN_Vector1,id:1604,x:30792,y:31942,varname:node_1604,prsc:2,v1:0.023;n:type:ShaderForge.SFN_Sin,id:1673,x:30765,y:31767,varname:node_1673,prsc:2|IN-988-T;n:type:ShaderForge.SFN_Add,id:975,x:31213,y:31833,varname:node_975,prsc:2|A-2428-OUT,B-1502-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:7611,x:33361,y:31046,ptovrint:False,ptlb:frostMask,ptin:_frostMask,varname:node_7611,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:7343,x:35658,y:31357,varname:node_7343,prsc:2|A-4969-RGB,B-6004-OUT,T-563-OUT;n:type:ShaderForge.SFN_Tex2d,id:4969,x:35060,y:31070,varname:node_4969,prsc:2,ntxv:0,isnm:False|TEX-4430-TEX;n:type:ShaderForge.SFN_Slider,id:563,x:35288,y:31625,ptovrint:False,ptlb:finalMix,ptin:_finalMix,varname:node_563,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:6071,x:31530,y:32000,varname:node_6071,prsc:2|A-975-OUT,B-623-OUT;n:type:ShaderForge.SFN_Slider,id:2172,x:30909,y:32147,ptovrint:False,ptlb:statVignetteScale,ptin:_statVignetteScale,varname:node_2172,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.38,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:623,x:31255,y:32182,varname:node_623,prsc:2|A-2172-OUT,B-9197-OUT;n:type:ShaderForge.SFN_Vector1,id:9197,x:31038,y:32271,varname:node_9197,prsc:2,v1:2.6;proporder:4430-7611-7056-2274-2261-6720-1322-7169-3519-201-1502-4000-563-2172;pass:END;sub:END;*/

Shader "Colony_FX/PostProcess/S_PP_Frost" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _frostMask ("frostMask", 2D) = "white" {}
        _diffMaskMult ("diffMaskMult", Range(0, 5)) = 1
        _diffMaskPow ("diffMaskPow", Range(0, 5)) = 1
        _frostMaskNormal ("frostMaskNormal", 2D) = "white" {}
        _sceneDistortion ("sceneDistortion", Range(0, 1)) = 0.576
        _diffuseOverlayDistortion ("diffuseOverlayDistortion", Range(0, 1)) = 0.36
        _vignetteDistorter ("vignetteDistorter", 2D) = "white" {}
        _vignetteDistorterTiling ("vignetteDistorterTiling", Range(0, 3)) = 1
        _vignetteDistorterIntensity ("vignetteDistorterIntensity", Range(0, 0.5)) = 0.2
        _mainVignette ("mainVignette", Range(0, 15)) = 0.1
        _erosion ("erosion", Range(0, 1)) = 0.113
        _finalMix ("finalMix", Range(0, 1)) = 0
        _statVignetteScale ("statVignetteScale", Range(0.38, 1)) = 1
		_transitionDelta("stateTransition", Range(0, 10)) = 0.6
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
            uniform float _mainVignette;
            uniform sampler2D _vignetteDistorter; uniform float4 _vignetteDistorter_ST;
            uniform float _vignetteDistorterIntensity;
            uniform float _vignetteDistorterTiling;
            uniform sampler2D _frostMaskNormal; uniform float4 _frostMaskNormal_ST;
            uniform float _sceneDistortion;
            uniform float _diffMaskPow;
            uniform float _diffuseOverlayDistortion;
            uniform float _diffMaskMult;
            uniform float _erosion;
            uniform sampler2D _frostMask; uniform float4 _frostMask_ST;
            uniform float _finalMix;
            uniform float _statVignetteScale;
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
                float4 node_4969 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 _frostMaskNormal_var = tex2D(_frostMaskNormal,TRANSFORM_TEX(i.uv0, _frostMaskNormal));
                float2 node_4209 = float2(_frostMaskNormal_var.r,_frostMaskNormal_var.g);
                float4 node_784 = tex2D(_frostMask,TRANSFORM_TEX(i.uv0, _frostMask));
                float4 node_988 = _Time;
                float2 node_9073 = (i.uv0*_vignetteDistorterTiling);
                float4 _vignetteDistorter_var = tex2D(_vignetteDistorter,TRANSFORM_TEX(node_9073, _vignetteDistorter));
                float mainVignInt = (1.0 - saturate(smoothstep( (1.0 - pow((((sin(node_988.g)*0.023)+_mainVignette)*(_statVignetteScale*2.6)),0.4)), 1.0, length((lerp(sceneUVs.rg,((sceneUVs.rg+float2(_vignetteDistorter_var.r,_vignetteDistorter_var.g))*2.0+-1.0),_vignetteDistorterIntensity)-float2(0.5,0.5))) )));
                float node_1504 = (node_784.a*(1.0 - mainVignInt));
                float2 node_3610 = lerp(i.uv0,(i.uv0+(node_4209*_sceneDistortion)),saturate((node_1504-_erosion)));
                float4 node_5589 = tex2D(_MainTex,TRANSFORM_TEX(node_3610, _MainTex));
                float2 node_8390 = lerp(i.uv0,(i.uv0+(node_4209*2.0+-1.0)),_diffuseOverlayDistortion);
                float4 node_5213 = tex2D(_frostMask,TRANSFORM_TEX(node_8390, _frostMask));
                float3 emissive = lerp(node_4969.rgb,lerp(node_5589.rgb,node_5213.rgb,saturate((pow((node_1504-(_erosion-0.05)),_diffMaskPow)*_diffMaskMult))),_finalMix);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
