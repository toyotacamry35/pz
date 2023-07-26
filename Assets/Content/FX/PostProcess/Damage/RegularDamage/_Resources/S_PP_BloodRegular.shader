// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.260621,fgcg:0.3751022,fgcb:0.464309,fgca:1,fgde:0.005089695,fgrn:0,fgrf:600,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:37781,y:33654,varname:node_2865,prsc:2|emission-4216-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:35426,y:33277,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:node_9933,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:980,x:35722,y:33193,varname:node_980,prsc:2,ntxv:0,isnm:False|UVIN-9289-OUT,TEX-4430-TEX;n:type:ShaderForge.SFN_Tex2d,id:5563,x:33046,y:33995,varname:node_5563,prsc:2,ntxv:0,isnm:False|UVIN-8240-OUT,TEX-806-TEX;n:type:ShaderForge.SFN_Tex2d,id:8819,x:34216,y:33202,ptovrint:False,ptlb:BloodNormal,ptin:_BloodNormal,varname:node_8819,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-8240-OUT;n:type:ShaderForge.SFN_Lerp,id:9289,x:34919,y:33183,varname:node_9289,prsc:2|A-2431-UVOUT,B-1922-OUT,T-5634-OUT;n:type:ShaderForge.SFN_TexCoord,id:2431,x:34408,y:32978,varname:node_2431,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Append,id:2605,x:34432,y:33219,varname:node_2605,prsc:2|A-8819-R,B-8819-G;n:type:ShaderForge.SFN_Add,id:1922,x:34645,y:33206,varname:node_1922,prsc:2|A-2431-UVOUT,B-2605-OUT;n:type:ShaderForge.SFN_Lerp,id:260,x:37103,y:33767,varname:node_260,prsc:2|A-980-RGB,B-5435-RGB,T-8721-OUT;n:type:ShaderForge.SFN_Multiply,id:9865,x:33928,y:34007,varname:node_9865,prsc:2|A-7347-OUT,B-1579-OUT;n:type:ShaderForge.SFN_Slider,id:1579,x:33597,y:34232,ptovrint:False,ptlb:bloodMaskBrightness,ptin:_bloodMaskBrightness,varname:node_1579,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:4;n:type:ShaderForge.SFN_Tex2d,id:5435,x:36275,y:33724,varname:node_5435,prsc:2,ntxv:0,isnm:False|UVIN-8240-OUT,TEX-806-TEX;n:type:ShaderForge.SFN_Subtract,id:9255,x:33477,y:34007,varname:node_9255,prsc:2|A-5563-A,B-9245-OUT;n:type:ShaderForge.SFN_Slider,id:9245,x:33223,y:34189,ptovrint:False,ptlb:bloodMaskErosion,ptin:_bloodMaskErosion,varname:node_9245,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Clamp01,id:7347,x:33706,y:34007,varname:node_7347,prsc:2|IN-9255-OUT;n:type:ShaderForge.SFN_Clamp01,id:8721,x:34655,y:33887,varname:node_8721,prsc:2|IN-5098-OUT;n:type:ShaderForge.SFN_Tex2d,id:4714,x:33508,y:33617,ptovrint:False,ptlb:randomMask,ptin:_randomMask,varname:node_4714,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9160-OUT;n:type:ShaderForge.SFN_Power,id:1819,x:33757,y:33634,varname:node_1819,prsc:2|VAL-4714-R,EXP-4392-OUT;n:type:ShaderForge.SFN_Slider,id:4392,x:33600,y:33796,ptovrint:False,ptlb:randomMaskPow,ptin:_randomMaskPow,varname:node_4392,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:55;n:type:ShaderForge.SFN_Multiply,id:5098,x:34406,y:33887,varname:node_5098,prsc:2|A-5700-OUT,B-9865-OUT;n:type:ShaderForge.SFN_Multiply,id:5700,x:34093,y:33650,varname:node_5700,prsc:2|A-1819-OUT,B-1759-OUT;n:type:ShaderForge.SFN_Slider,id:1759,x:33969,y:33800,ptovrint:False,ptlb:randomMaskMult,ptin:_randomMaskMult,varname:node_1759,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:22;n:type:ShaderForge.SFN_ScreenPos,id:7043,x:35143,y:34602,varname:node_7043,prsc:2,sctp:2;n:type:ShaderForge.SFN_Slider,id:5729,x:35138,y:34310,ptovrint:False,ptlb:vignette,ptin:_vignette,varname:node_5729,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Power,id:7097,x:36219,y:34357,varname:node_7097,prsc:2|VAL-7028-OUT,EXP-7953-OUT;n:type:ShaderForge.SFN_Vector1,id:7953,x:36011,y:34458,varname:node_7953,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Smoothstep,id:1879,x:36590,y:34358,varname:node_1879,prsc:2|A-7097-OUT,B-9317-OUT,V-8536-OUT;n:type:ShaderForge.SFN_Vector1,id:9317,x:36373,y:34440,varname:node_9317,prsc:2,v1:1;n:type:ShaderForge.SFN_Add,id:5708,x:35702,y:34720,varname:node_5708,prsc:2|A-7043-UVOUT,B-3574-R;n:type:ShaderForge.SFN_Lerp,id:8859,x:35958,y:34664,varname:node_8859,prsc:2|A-7043-UVOUT,B-5708-OUT,T-6001-OUT;n:type:ShaderForge.SFN_Slider,id:6001,x:35565,y:34921,ptovrint:False,ptlb:vignetteDist,ptin:_vignetteDist,varname:node_6001,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_RemapRange,id:6509,x:36167,y:34664,varname:node_6509,prsc:2,frmn:0,frmx:1,tomn:-0.7,tomx:0.7|IN-8859-OUT;n:type:ShaderForge.SFN_Length,id:8536,x:36361,y:34664,varname:node_8536,prsc:2|IN-6509-OUT;n:type:ShaderForge.SFN_Tex2d,id:3574,x:35266,y:34808,ptovrint:False,ptlb:vignetteDistorter,ptin:_vignetteDistorter,varname:node_3574,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9160-OUT;n:type:ShaderForge.SFN_Lerp,id:4216,x:37505,y:33775,varname:node_4216,prsc:2|A-260-OUT,B-6864-RGB,T-5915-OUT;n:type:ShaderForge.SFN_Color,id:6864,x:36665,y:33996,ptovrint:False,ptlb:vignetteColor,ptin:_vignetteColor,varname:node_6864,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:3843,x:36964,y:34088,varname:node_3843,prsc:2|A-6864-A,B-871-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:806,x:32727,y:33868,ptovrint:False,ptlb:BloodMask,ptin:_BloodMask,varname:node_806,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_OneMinus,id:7028,x:35757,y:34369,varname:node_7028,prsc:2|IN-4091-OUT;n:type:ShaderForge.SFN_ScreenPos,id:2172,x:32546,y:33484,varname:node_2172,prsc:2,sctp:0;n:type:ShaderForge.SFN_Multiply,id:8240,x:32787,y:33572,varname:node_8240,prsc:2|A-2172-UVOUT,B-623-OUT;n:type:ShaderForge.SFN_Slider,id:623,x:32380,y:33706,ptovrint:False,ptlb:bloodMaskTiling,ptin:_bloodMaskTiling,varname:node_623,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:3;n:type:ShaderForge.SFN_Multiply,id:5634,x:34841,y:33411,varname:node_5634,prsc:2|A-8731-OUT,B-8721-OUT;n:type:ShaderForge.SFN_Slider,id:8731,x:34383,y:33444,ptovrint:False,ptlb:bloodDistortion,ptin:_bloodDistortion,varname:node_8731,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Clamp01,id:871,x:36766,y:34358,varname:node_871,prsc:2|IN-1879-OUT;n:type:ShaderForge.SFN_Multiply,id:5915,x:37183,y:34107,varname:node_5915,prsc:2|A-3843-OUT,B-5028-OUT,C-3373-OUT;n:type:ShaderForge.SFN_Slider,id:5028,x:36587,y:34222,ptovrint:False,ptlb:vignetteLevel,ptin:_vignetteLevel,varname:node_5028,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_TexCoord,id:5131,x:32896,y:34718,varname:node_5131,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:9160,x:33183,y:34768,varname:node_9160,prsc:2|A-5131-UVOUT,B-3899-OUT;n:type:ShaderForge.SFN_Vector2,id:6140,x:32769,y:34887,varname:node_6140,prsc:2,v1:3,v2:7;n:type:ShaderForge.SFN_Multiply,id:3899,x:32978,y:34901,varname:node_3899,prsc:2|A-6140-OUT,B-5679-OUT;n:type:ShaderForge.SFN_Slider,id:5679,x:32612,y:35042,ptovrint:False,ptlb:randomOffset,ptin:_randomOffset,varname:node_5679,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:3373,x:35138,y:34439,ptovrint:False,ptlb:finalMix,ptin:_finalMix,varname:node_3373,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:4091,x:35567,y:34369,varname:node_4091,prsc:2|A-5729-OUT,B-3373-OUT;proporder:4430-806-623-1579-9245-8819-8731-4714-4392-1759-6864-5729-3574-6001-5028-5679-3373;pass:END;sub:END;*/

Shader "Colony_FX/PostProcess/S_PP_BloodRegular" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _BloodMask ("BloodMask", 2D) = "white" {}
        _bloodMaskTiling ("bloodMaskTiling", Range(0, 3)) = 2
        _bloodMaskBrightness ("bloodMaskBrightness", Range(0, 4)) = 1
        _bloodMaskErosion ("bloodMaskErosion", Range(0, 1)) = 0
        _BloodNormal ("BloodNormal", 2D) = "bump" {}
        _bloodDistortion ("bloodDistortion", Range(0, 1)) = 0
        _randomMask ("randomMask", 2D) = "white" {}
        _randomMaskPow ("randomMaskPow", Range(0, 55)) = 1
        _randomMaskMult ("randomMaskMult", Range(0, 22)) = 0
        _vignetteColor ("vignetteColor", Color) = (0.5,0.5,0.5,1)
        _vignette ("vignette", Range(0, 1)) = 0
        _vignetteDistorter ("vignetteDistorter", 2D) = "white" {}
        _vignetteDist ("vignetteDist", Range(0, 1)) = 0
        _vignetteLevel ("vignetteLevel", Range(0, 1)) = 0
        _randomOffset ("randomOffset", Range(0, 1)) = 0
        _finalMix ("finalMix", Range(0, 1)) = 0
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
            uniform sampler2D _BloodNormal; uniform float4 _BloodNormal_ST;
            uniform float _bloodMaskBrightness;
            uniform float _bloodMaskErosion;
            uniform sampler2D _randomMask; uniform float4 _randomMask_ST;
            uniform float _randomMaskPow;
            uniform float _randomMaskMult;
            uniform float _vignette;
            uniform float _vignetteDist;
            uniform sampler2D _vignetteDistorter; uniform float4 _vignetteDistorter_ST;
            uniform float4 _vignetteColor;
            uniform sampler2D _BloodMask; uniform float4 _BloodMask_ST;
            uniform float _bloodMaskTiling;
            uniform float _bloodDistortion;
            uniform float _vignetteLevel;
            uniform float _randomOffset;
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
                float2 node_8240 = ((sceneUVs * 2 - 1).rg*_bloodMaskTiling);
                float3 _BloodNormal_var = UnpackNormal(tex2D(_BloodNormal,TRANSFORM_TEX(node_8240, _BloodNormal)));
                float2 node_9160 = (i.uv0+(float2(3,7)*_randomOffset));
                float4 _randomMask_var = tex2D(_randomMask,TRANSFORM_TEX(node_9160, _randomMask));
                float4 node_5563 = tex2D(_BloodMask,TRANSFORM_TEX(node_8240, _BloodMask));
                float node_8721 = saturate(((pow(_randomMask_var.r,_randomMaskPow)*_randomMaskMult)*(saturate((node_5563.a-_bloodMaskErosion))*_bloodMaskBrightness)));
                float2 node_9289 = lerp(i.uv0,(i.uv0+float2(_BloodNormal_var.r,_BloodNormal_var.g)),(_bloodDistortion*node_8721));
                float4 node_980 = tex2D(_MainTex,TRANSFORM_TEX(node_9289, _MainTex));
                float4 node_5435 = tex2D(_BloodMask,TRANSFORM_TEX(node_8240, _BloodMask));
                float4 _vignetteDistorter_var = tex2D(_vignetteDistorter,TRANSFORM_TEX(node_9160, _vignetteDistorter));
                float3 emissive = lerp(lerp(node_980.rgb,node_5435.rgb,node_8721),_vignetteColor.rgb,((_vignetteColor.a*saturate(smoothstep( pow((1.0 - (_vignette*_finalMix)),0.4), 1.0, length((lerp(sceneUVs.rg,(sceneUVs.rg+_vignetteDistorter_var.r),_vignetteDist)*1.4+-0.7)) )))*_vignetteLevel*_finalMix));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
