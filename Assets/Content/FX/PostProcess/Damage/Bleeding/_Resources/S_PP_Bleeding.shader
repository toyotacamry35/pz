// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.589902,fgcg:0.9053665,fgcb:0.9545916,fgca:1,fgde:0.00413295,fgrn:0,fgrf:600,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:40948,y:33751,varname:node_2865,prsc:2|emission-5047-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:36481,y:33621,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:node_9933,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:980,x:36713,y:33581,varname:node_980,prsc:2,ntxv:0,isnm:False|TEX-4430-TEX;n:type:ShaderForge.SFN_ScreenPos,id:7043,x:34515,y:35756,varname:node_7043,prsc:2,sctp:2;n:type:ShaderForge.SFN_Slider,id:5729,x:35011,y:35538,ptovrint:False,ptlb:vignette,ptin:_vignette,varname:node_5729,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Power,id:7097,x:36057,y:35443,varname:node_7097,prsc:2|VAL-3566-OUT,EXP-7953-OUT;n:type:ShaderForge.SFN_Vector1,id:7953,x:35849,y:35544,varname:node_7953,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Smoothstep,id:1879,x:36428,y:35444,varname:node_1879,prsc:2|A-7097-OUT,B-9317-OUT,V-8536-OUT;n:type:ShaderForge.SFN_Vector1,id:9317,x:36211,y:35526,varname:node_9317,prsc:2,v1:1;n:type:ShaderForge.SFN_Add,id:5708,x:35209,y:35891,varname:node_5708,prsc:2|A-7043-UVOUT,B-5376-OUT;n:type:ShaderForge.SFN_Lerp,id:8859,x:35487,y:35754,varname:node_8859,prsc:2|A-7043-UVOUT,B-5708-OUT,T-6001-OUT;n:type:ShaderForge.SFN_Slider,id:6001,x:35250,y:36105,ptovrint:False,ptlb:vignetteDist,ptin:_vignetteDist,varname:node_6001,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_RemapRange,id:6509,x:35761,y:35742,varname:node_6509,prsc:2,frmn:0,frmx:1,tomn:-0.7,tomx:0.7|IN-8859-OUT;n:type:ShaderForge.SFN_Length,id:8536,x:36199,y:35750,varname:node_8536,prsc:2|IN-6509-OUT;n:type:ShaderForge.SFN_Append,id:5376,x:34793,y:36094,varname:node_5376,prsc:2|A-3574-R,B-3574-G;n:type:ShaderForge.SFN_Tex2d,id:3574,x:34531,y:36074,ptovrint:False,ptlb:vignetteDistorter,ptin:_vignetteDistorter,varname:node_3574,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6249-OUT;n:type:ShaderForge.SFN_Lerp,id:4216,x:37483,y:33843,varname:node_4216,prsc:2|A-980-RGB,B-6864-RGB,T-3843-OUT;n:type:ShaderForge.SFN_Color,id:6864,x:37064,y:33864,ptovrint:False,ptlb:vignetteColor,ptin:_vignetteColor,varname:node_6864,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:4177,x:37786,y:34169,varname:node_4177,prsc:2|A-1879-OUT,B-6059-OUT,C-8206-OUT,D-2151-OUT,E-8867-OUT;n:type:ShaderForge.SFN_Lerp,id:6059,x:38504,y:34663,varname:node_6059,prsc:2|A-832-OUT,B-2773-A,T-6241-A;n:type:ShaderForge.SFN_Tex2d,id:2773,x:38101,y:34625,varname:node_2773,prsc:2,ntxv:0,isnm:False|TEX-3525-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:3525,x:37374,y:34470,ptovrint:False,ptlb:DrippingMask,ptin:_DrippingMask,varname:node_3525,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:6241,x:38101,y:34772,varname:node_6241,prsc:2,ntxv:0,isnm:False|UVIN-5276-OUT,TEX-3525-TEX;n:type:ShaderForge.SFN_Vector1,id:832,x:38140,y:34524,varname:node_832,prsc:2,v1:0;n:type:ShaderForge.SFN_TexCoord,id:6532,x:37641,y:34745,varname:node_6532,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:5276,x:37848,y:34826,varname:node_5276,prsc:2|A-6532-UVOUT,B-2918-OUT;n:type:ShaderForge.SFN_Time,id:9471,x:37383,y:34915,varname:node_9471,prsc:2;n:type:ShaderForge.SFN_Slider,id:4016,x:37314,y:35224,ptovrint:False,ptlb:bleedingSpeed,ptin:_bleedingSpeed,varname:node_4016,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Vector1,id:562,x:37395,y:35135,varname:node_562,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:9009,x:37632,y:35165,varname:node_9009,prsc:2|A-562-OUT,B-4016-OUT;n:type:ShaderForge.SFN_Multiply,id:2918,x:37654,y:34953,varname:node_2918,prsc:2|A-9471-T,B-9009-OUT;n:type:ShaderForge.SFN_Clamp01,id:4291,x:37976,y:34169,varname:node_4291,prsc:2|IN-4177-OUT;n:type:ShaderForge.SFN_Lerp,id:9890,x:38109,y:33840,varname:node_9890,prsc:2|A-4216-OUT,B-2519-RGB,T-4291-OUT;n:type:ShaderForge.SFN_Multiply,id:3843,x:37249,y:34047,varname:node_3843,prsc:2|A-6864-A,B-1879-OUT,C-2151-OUT,D-8867-OUT,E-8206-OUT;n:type:ShaderForge.SFN_Add,id:3566,x:35683,y:35358,varname:node_3566,prsc:2|A-6660-OUT,B-5138-OUT;n:type:ShaderForge.SFN_Time,id:244,x:35029,y:35353,varname:node_244,prsc:2;n:type:ShaderForge.SFN_Sin,id:7242,x:35237,y:35393,varname:node_7242,prsc:2|IN-244-T;n:type:ShaderForge.SFN_Multiply,id:6660,x:35427,y:35375,varname:node_6660,prsc:2|A-9954-OUT,B-7242-OUT;n:type:ShaderForge.SFN_Vector1,id:9954,x:35277,y:35302,varname:node_9954,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Tex2d,id:3373,x:40055,y:34576,varname:node_3373,prsc:2,tex:89d5032f351916f4ab0c5881c4d54e63,ntxv:0,isnm:False|UVIN-5184-OUT,TEX-1997-TEX;n:type:ShaderForge.SFN_TexCoord,id:9018,x:39821,y:34751,varname:node_9018,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_OneMinus,id:4613,x:40037,y:35041,varname:node_4613,prsc:2|IN-9018-V;n:type:ShaderForge.SFN_Power,id:3899,x:40260,y:34790,varname:node_3899,prsc:2|VAL-9018-V,EXP-5679-OUT;n:type:ShaderForge.SFN_Slider,id:5679,x:39944,y:34958,ptovrint:False,ptlb:rakeDecay,ptin:_rakeDecay,varname:node_5679,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:115;n:type:ShaderForge.SFN_Multiply,id:7453,x:40485,y:34679,varname:node_7453,prsc:2|A-3373-A,B-3899-OUT;n:type:ShaderForge.SFN_Subtract,id:9169,x:40720,y:34679,varname:node_9169,prsc:2|A-7453-OUT,B-4712-OUT;n:type:ShaderForge.SFN_Clamp01,id:7867,x:41086,y:34683,varname:node_7867,prsc:2|IN-7445-OUT;n:type:ShaderForge.SFN_Slider,id:4712,x:40414,y:34924,ptovrint:False,ptlb:rakeErosion,ptin:_rakeErosion,varname:node_4712,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_TexCoord,id:1428,x:39010,y:34502,varname:node_1428,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:8674,x:39224,y:34635,varname:node_8674,prsc:2|A-1428-U,B-8574-OUT;n:type:ShaderForge.SFN_Append,id:5184,x:39761,y:34530,varname:node_5184,prsc:2|A-8674-OUT,B-1428-V;n:type:ShaderForge.SFN_Slider,id:8574,x:38905,y:34747,ptovrint:False,ptlb:rakeOffsetRandom,ptin:_rakeOffsetRandom,varname:node_8574,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-0.2,cur:0.2,max:0.2;n:type:ShaderForge.SFN_Subtract,id:9171,x:39425,y:34663,varname:node_9171,prsc:2|A-8674-OUT,B-8457-OUT;n:type:ShaderForge.SFN_Vector1,id:8457,x:39276,y:34801,varname:node_8457,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Tex2d,id:9688,x:40079,y:34346,varname:node_9688,prsc:2,tex:89d5032f351916f4ab0c5881c4d54e63,ntxv:0,isnm:False|UVIN-5184-OUT,TEX-1997-TEX;n:type:ShaderForge.SFN_Lerp,id:5047,x:40622,y:33847,varname:node_5047,prsc:2|A-9890-OUT,B-9688-RGB,T-3348-OUT;n:type:ShaderForge.SFN_Multiply,id:7445,x:40909,y:34683,varname:node_7445,prsc:2|A-9169-OUT,B-4138-OUT;n:type:ShaderForge.SFN_Vector1,id:4138,x:40827,y:34812,varname:node_4138,prsc:2,v1:1.12;n:type:ShaderForge.SFN_Tex2d,id:2519,x:37815,y:33876,varname:node_2519,prsc:2,ntxv:0,isnm:False|TEX-3525-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:1997,x:39801,y:34244,ptovrint:False,ptlb:RakeMask,ptin:_RakeMask,varname:node_1997,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:89d5032f351916f4ab0c5881c4d54e63,ntxv:0,isnm:False;n:type:ShaderForge.SFN_OneMinus,id:5138,x:35427,y:35539,varname:node_5138,prsc:2|IN-5729-OUT;n:type:ShaderForge.SFN_Slider,id:2151,x:36648,y:34365,ptovrint:False,ptlb:drippingMaskStrength,ptin:_drippingMaskStrength,varname:node_2151,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Vector1,id:8867,x:37138,y:34260,varname:node_8867,prsc:2,v1:3;n:type:ShaderForge.SFN_Add,id:6249,x:34318,y:36183,varname:node_6249,prsc:2|A-6089-UVOUT,B-5915-OUT;n:type:ShaderForge.SFN_Vector2,id:7171,x:33945,y:36187,varname:node_7171,prsc:2,v1:3,v2:7;n:type:ShaderForge.SFN_Multiply,id:5915,x:34148,y:36256,varname:node_5915,prsc:2|A-7171-OUT,B-2673-OUT;n:type:ShaderForge.SFN_Slider,id:2673,x:33820,y:36353,ptovrint:False,ptlb:vignetteDistorterOffset,ptin:_vignetteDistorterOffset,varname:node_2673,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_TexCoord,id:6089,x:34025,y:36005,varname:node_6089,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:3348,x:41179,y:34503,varname:node_3348,prsc:2|A-8206-OUT,B-7867-OUT;n:type:ShaderForge.SFN_Slider,id:8206,x:36387,y:34233,ptovrint:False,ptlb:finalMix,ptin:_finalMix,varname:node_8206,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;proporder:4430-3574-6864-5729-6001-3525-4016-5679-4712-8574-1997-2151-2673-8206;pass:END;sub:END;*/

Shader "Colony_FX/PostProcess/S_PP_Bleeding" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _vignetteDistorter ("vignetteDistorter", 2D) = "white" {}
        _vignetteColor ("vignetteColor", Color) = (0.5,0.5,0.5,1)
        _vignette ("vignette", Range(0, 1)) = 0
        _vignetteDist ("vignetteDist", Range(0, 1)) = 0
        _DrippingMask ("DrippingMask", 2D) = "white" {}
        _bleedingSpeed ("bleedingSpeed", Range(-1, 1)) = 0
        _rakeDecay ("rakeDecay", Range(0, 115)) = 1
        _rakeErosion ("rakeErosion", Range(0, 1)) = 0
        _rakeOffsetRandom ("rakeOffsetRandom", Range(-0.2, 0.2)) = 0.2
        _RakeMask ("RakeMask", 2D) = "white" {}
        _drippingMaskStrength ("drippingMaskStrength", Range(0, 1)) = 0
        _vignetteDistorterOffset ("vignetteDistorterOffset", Range(0, 1)) = 0
        _finalMix ("finalMix", Range(0, 1)) = 1
        _transitionDelta("stateTransition", Range(0, 10000000000000000000000000000000000000000)) = 1000000000000000000000000000000000000000
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
            uniform float _vignette;
            uniform float _vignetteDist;
            uniform sampler2D _vignetteDistorter; uniform float4 _vignetteDistorter_ST;
            uniform float4 _vignetteColor;
            uniform sampler2D _DrippingMask; uniform float4 _DrippingMask_ST;
            uniform float _bleedingSpeed;
            uniform float _rakeDecay;
            uniform float _rakeErosion;
            uniform float _rakeOffsetRandom;
            uniform sampler2D _RakeMask; uniform float4 _RakeMask_ST;
            uniform float _drippingMaskStrength;
            uniform float _vignetteDistorterOffset;
            uniform float _finalMix;
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
                float4 node_980 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_244 = _Time;
                float2 node_6249 = (i.uv0+(float2(3,7)*_vignetteDistorterOffset));
                float4 _vignetteDistorter_var = tex2D(_vignetteDistorter,TRANSFORM_TEX(node_6249, _vignetteDistorter));
                float node_1879 = smoothstep( pow(((0.1*sin(node_244.g))+(1.0 - _vignette)),0.4), 1.0, length((lerp(sceneUVs.rg,(sceneUVs.rg+float2(_vignetteDistorter_var.r,_vignetteDistorter_var.g)),_vignetteDist)*1.4+-0.7)) );
                float node_8867 = 3.0;
                float4 node_2519 = tex2D(_DrippingMask,TRANSFORM_TEX(i.uv0, _DrippingMask));
                float4 node_2773 = tex2D(_DrippingMask,TRANSFORM_TEX(i.uv0, _DrippingMask));
                float4 node_9471 = _Time;
                float2 node_5276 = (i.uv0+(node_9471.g*float2(0.0,_bleedingSpeed)));
                float4 node_6241 = tex2D(_DrippingMask,TRANSFORM_TEX(node_5276, _DrippingMask));
                float node_8674 = (i.uv0.r+_rakeOffsetRandom);
                float2 node_5184 = float2(node_8674,i.uv0.g);
                float4 node_9688 = tex2D(_RakeMask,TRANSFORM_TEX(node_5184, _RakeMask));
                float4 node_3373 = tex2D(_RakeMask,TRANSFORM_TEX(node_5184, _RakeMask));
                float3 emissive = lerp(lerp(lerp(node_980.rgb,_vignetteColor.rgb,(_vignetteColor.a*node_1879*_drippingMaskStrength*node_8867*_finalMix)),node_2519.rgb,saturate((node_1879*lerp(0.0,node_2773.a,node_6241.a)*_finalMix*_drippingMaskStrength*node_8867))),node_9688.rgb,(_finalMix*saturate((((node_3373.a*pow(i.uv0.g,_rakeDecay))-_rakeErosion)*1.12))));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
