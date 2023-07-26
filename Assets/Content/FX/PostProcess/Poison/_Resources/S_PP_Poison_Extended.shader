// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.2693381,fgcg:0.4401155,fgcb:0.4970229,fgca:1,fgde:0.005086312,fgrn:0,fgrf:600,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:36933,y:33287,varname:node_2865,prsc:2|emission-3463-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:33405,y:33654,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3687,x:33795,y:33431,varname:node_3687,prsc:2,ntxv:0,isnm:False|UVIN-4000-OUT,TEX-4430-TEX;n:type:ShaderForge.SFN_Time,id:9086,x:31012,y:33016,varname:node_9086,prsc:2;n:type:ShaderForge.SFN_Desaturate,id:5119,x:34021,y:33431,varname:node_5119,prsc:2|COL-3687-RGB,DES-8960-OUT;n:type:ShaderForge.SFN_Slider,id:1175,x:33613,y:33776,ptovrint:False,ptlb:sceneDesaturation,ptin:_sceneDesaturation,varname:_sceneDesaturation,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:9197,x:32591,y:34285,ptovrint:False,ptlb:finalMix,ptin:_finalMix,varname:_finalMix,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:9403,x:33353,y:32802,ptovrint:False,ptlb:sceneMultiplicationR,ptin:_sceneMultiplicationR,varname:_sceneMultiplicationR,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_TexCoord,id:4459,x:32783,y:33158,varname:node_4459,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2d,id:628,x:32258,y:33379,ptovrint:False,ptlb:uvNoiseTex,ptin:_uvNoiseTex,varname:_uvNoiseTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:edecc509e768569459e378283355ae30,ntxv:0,isnm:False|UVIN-2988-OUT;n:type:ShaderForge.SFN_Multiply,id:9115,x:31698,y:33241,varname:node_9115,prsc:2|A-9086-T,B-3867-OUT;n:type:ShaderForge.SFN_Slider,id:2741,x:31050,y:33379,ptovrint:False,ptlb:noisePanY,ptin:_noisePanY,varname:_noisePanY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0.2661707,max:5;n:type:ShaderForge.SFN_Append,id:3867,x:31483,y:33241,varname:node_3867,prsc:2|A-1141-OUT,B-2741-OUT;n:type:ShaderForge.SFN_Frac,id:1549,x:31874,y:33241,varname:node_1549,prsc:2|IN-9115-OUT;n:type:ShaderForge.SFN_TexCoord,id:8886,x:31603,y:33495,varname:node_8886,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:2988,x:32081,y:33379,varname:node_2988,prsc:2|A-1549-OUT,B-150-OUT;n:type:ShaderForge.SFN_Multiply,id:150,x:31849,y:33495,varname:node_150,prsc:2|A-8886-UVOUT,B-294-OUT;n:type:ShaderForge.SFN_Slider,id:294,x:31493,y:33716,ptovrint:False,ptlb:uvNoiseTile,ptin:_uvNoiseTile,varname:_uvNoiseTile,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Append,id:1492,x:32478,y:33396,varname:node_1492,prsc:2|A-628-R,B-628-G;n:type:ShaderForge.SFN_Add,id:3914,x:32957,y:33312,varname:node_3914,prsc:2|A-4459-UVOUT,B-1492-OUT;n:type:ShaderForge.SFN_Lerp,id:4000,x:33315,y:33268,varname:node_4000,prsc:2|A-4459-UVOUT,B-3914-OUT,T-3311-OUT;n:type:ShaderForge.SFN_Slider,id:486,x:32448,y:33624,ptovrint:False,ptlb:distStrength,ptin:_distStrength,varname:_distStrength,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.1;n:type:ShaderForge.SFN_Slider,id:1141,x:30982,y:33240,ptovrint:False,ptlb:noisePanX,ptin:_noisePanX,varname:_noisePanX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0.2661707,max:5;n:type:ShaderForge.SFN_Multiply,id:748,x:34365,y:33415,varname:node_748,prsc:2|A-2647-OUT,B-5119-OUT;n:type:ShaderForge.SFN_Slider,id:9761,x:33353,y:32903,ptovrint:False,ptlb:sceneMultiplicationG,ptin:_sceneMultiplicationG,varname:_sceneMultiplicationG,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Slider,id:8697,x:33353,y:33007,ptovrint:False,ptlb:sceneMultiplicationB,ptin:_sceneMultiplicationB,varname:_sceneMultiplicationB,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Power,id:110,x:35405,y:33403,varname:node_110,prsc:2|VAL-748-OUT,EXP-7158-OUT;n:type:ShaderForge.SFN_Slider,id:4759,x:34771,y:33622,ptovrint:False,ptlb:scenePow,ptin:_scenePow,varname:_scenePow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:11;n:type:ShaderForge.SFN_Multiply,id:3311,x:32843,y:33629,varname:node_3311,prsc:2|A-486-OUT,B-9197-OUT;n:type:ShaderForge.SFN_Lerp,id:7158,x:35171,y:33506,varname:node_7158,prsc:2|A-6409-OUT,B-4759-OUT,T-9197-OUT;n:type:ShaderForge.SFN_Vector1,id:6409,x:34946,y:33457,varname:node_6409,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:8960,x:33976,y:33701,varname:node_8960,prsc:2|A-2824-OUT,B-1175-OUT,T-9197-OUT;n:type:ShaderForge.SFN_Vector1,id:2824,x:33732,y:33701,varname:node_2824,prsc:2,v1:0;n:type:ShaderForge.SFN_Lerp,id:3398,x:33753,y:32999,varname:node_3398,prsc:2|A-2933-OUT,B-8697-OUT,T-9197-OUT;n:type:ShaderForge.SFN_Lerp,id:6195,x:33749,y:32887,varname:node_6195,prsc:2|A-2933-OUT,B-9761-OUT,T-9197-OUT;n:type:ShaderForge.SFN_Lerp,id:8090,x:33749,y:32760,varname:node_8090,prsc:2|A-2933-OUT,B-9403-OUT,T-9197-OUT;n:type:ShaderForge.SFN_Vector1,id:2933,x:33463,y:32655,varname:node_2933,prsc:2,v1:1;n:type:ShaderForge.SFN_Append,id:2647,x:34018,y:33029,varname:node_2647,prsc:2|A-8090-OUT,B-6195-OUT,C-3398-OUT;n:type:ShaderForge.SFN_Lerp,id:7772,x:33111,y:33582,varname:node_7772,prsc:2|A-5220-OUT,B-3311-OUT,T-1597-OUT;n:type:ShaderForge.SFN_Vector1,id:5220,x:32860,y:33563,varname:node_5220,prsc:2,v1:0;n:type:ShaderForge.SFN_Time,id:9852,x:32104,y:33796,varname:node_9852,prsc:2;n:type:ShaderForge.SFN_Sin,id:5395,x:32483,y:33823,varname:node_5395,prsc:2|IN-1579-OUT;n:type:ShaderForge.SFN_Vector1,id:9673,x:32499,y:33963,varname:node_9673,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Vector1,id:9865,x:32680,y:33979,varname:node_9865,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:1579,x:32298,y:33823,varname:node_1579,prsc:2|A-9852-T,B-2519-OUT;n:type:ShaderForge.SFN_Vector1,id:2519,x:32104,y:33925,varname:node_2519,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Multiply,id:9932,x:32722,y:33823,varname:node_9932,prsc:2|A-5395-OUT,B-9673-OUT;n:type:ShaderForge.SFN_Add,id:1597,x:32946,y:33823,varname:node_1597,prsc:2|A-9932-OUT,B-9865-OUT;n:type:ShaderForge.SFN_Smoothstep,id:5092,x:35620,y:34297,varname:node_5092,prsc:2|A-5289-OUT,B-6529-OUT,V-543-OUT;n:type:ShaderForge.SFN_RemapRange,id:5935,x:35093,y:34396,varname:node_5935,prsc:2,frmn:0,frmx:1,tomn:-0.5,tomx:0.5|IN-1510-UVOUT;n:type:ShaderForge.SFN_ScreenPos,id:1510,x:34918,y:34404,varname:node_1510,prsc:2,sctp:2;n:type:ShaderForge.SFN_Vector1,id:6529,x:35330,y:34322,varname:node_6529,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:6447,x:34846,y:34107,ptovrint:False,ptlb:vignette,ptin:_vignette,varname:node_6447,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Power,id:3424,x:35147,y:34166,varname:node_3424,prsc:2|VAL-6447-OUT,EXP-1444-OUT;n:type:ShaderForge.SFN_Vector1,id:1444,x:34983,y:34198,varname:node_1444,prsc:2,v1:0.4;n:type:ShaderForge.SFN_OneMinus,id:5289,x:35315,y:34166,varname:node_5289,prsc:2|IN-3424-OUT;n:type:ShaderForge.SFN_Length,id:543,x:35315,y:34396,varname:node_543,prsc:2|IN-5935-OUT;n:type:ShaderForge.SFN_Lerp,id:4413,x:36144,y:33401,varname:node_4413,prsc:2|A-110-OUT,B-9198-OUT,T-7716-OUT;n:type:ShaderForge.SFN_Multiply,id:7716,x:35773,y:34174,varname:node_7716,prsc:2|A-1939-OUT,B-5092-OUT;n:type:ShaderForge.SFN_Vector1,id:1939,x:35595,y:34152,varname:node_1939,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Lerp,id:3463,x:36520,y:33396,varname:node_3463,prsc:2|A-4413-OUT,B-8991-RGB,T-3221-OUT;n:type:ShaderForge.SFN_Slider,id:2799,x:35416,y:33664,ptovrint:False,ptlb:vignetteDarken,ptin:_vignetteDarken,varname:node_2799,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:9198,x:35850,y:33471,varname:node_9198,prsc:2|A-110-OUT,B-5658-OUT;n:type:ShaderForge.SFN_OneMinus,id:5658,x:35723,y:33645,varname:node_5658,prsc:2|IN-2799-OUT;n:type:ShaderForge.SFN_Color,id:8991,x:36125,y:33571,ptovrint:False,ptlb:stainsColor,ptin:_stainsColor,varname:node_8991,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:6477,x:36434,y:33830,varname:node_6477,prsc:2|A-6503-OUT,B-7716-OUT;n:type:ShaderForge.SFN_Tex2d,id:6817,x:36000,y:33790,ptovrint:False,ptlb:stainsPattern,ptin:_stainsPattern,varname:node_6817,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3043-OUT;n:type:ShaderForge.SFN_Power,id:6503,x:36207,y:33830,varname:node_6503,prsc:2|VAL-6817-R,EXP-96-OUT;n:type:ShaderForge.SFN_Slider,id:96,x:35989,y:34019,ptovrint:False,ptlb:stainsPow,ptin:_stainsPow,varname:node_96,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:16;n:type:ShaderForge.SFN_Multiply,id:3221,x:36697,y:33824,varname:node_3221,prsc:2|A-6477-OUT,B-157-OUT;n:type:ShaderForge.SFN_Slider,id:157,x:36372,y:34053,ptovrint:False,ptlb:stainsMult,ptin:_stainsMult,varname:node_157,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:5;n:type:ShaderForge.SFN_Time,id:8969,x:33955,y:34764,varname:node_8969,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:6459,x:35201,y:35127,ptovrint:False,ptlb:stainsDistorter,ptin:_stainsDistorter,varname:_uvNoiseTex_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:edecc509e768569459e378283355ae30,ntxv:0,isnm:False|UVIN-9101-OUT;n:type:ShaderForge.SFN_Multiply,id:2816,x:34641,y:34989,varname:node_2816,prsc:2|A-8969-T,B-2069-OUT;n:type:ShaderForge.SFN_Append,id:2069,x:34359,y:35021,varname:node_2069,prsc:2|A-1164-OUT,B-2122-OUT;n:type:ShaderForge.SFN_Frac,id:1868,x:34817,y:34989,varname:node_1868,prsc:2|IN-2816-OUT;n:type:ShaderForge.SFN_TexCoord,id:1003,x:34546,y:35243,varname:node_1003,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:9101,x:35024,y:35127,varname:node_9101,prsc:2|A-1868-OUT,B-4080-OUT;n:type:ShaderForge.SFN_Multiply,id:4080,x:34792,y:35243,varname:node_4080,prsc:2|A-1003-UVOUT,B-7520-OUT;n:type:ShaderForge.SFN_TexCoord,id:2808,x:35290,y:34935,varname:node_2808,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:3043,x:35989,y:35072,varname:node_3043,prsc:2|A-2808-UVOUT,B-6782-OUT,T-7301-OUT;n:type:ShaderForge.SFN_Add,id:6782,x:35573,y:35146,varname:node_6782,prsc:2|A-2808-UVOUT,B-6459-R;n:type:ShaderForge.SFN_Slider,id:7301,x:35620,y:35394,ptovrint:False,ptlb:stainsDistortion,ptin:_stainsDistortion,varname:node_7301,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3,max:1;n:type:ShaderForge.SFN_Slider,id:1164,x:33896,y:35007,ptovrint:False,ptlb:stainsDistorterPanX,ptin:_stainsDistorterPanX,varname:node_1164,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:2122,x:33851,y:35171,ptovrint:False,ptlb:stainsDistorterPanY,ptin:_stainsDistorterPanY,varname:node_2122,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:7520,x:34463,y:35510,ptovrint:False,ptlb:stainsDistorterTile,ptin:_stainsDistorterTile,varname:node_7520,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:4;proporder:4430-628-1141-2741-294-486-9403-9761-8697-4759-1175-9197-6447-2799-8991-6817-96-157-6459-7301-1164-2122-7520;pass:END;sub:END;*/

Shader "Colony_FX/PostProcess/S_PP_Poison_Extended" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _uvNoiseTex ("uvNoiseTex", 2D) = "white" {}
        _noisePanX ("noisePanX", Range(-5, 5)) = 0.2661707
        _noisePanY ("noisePanY", Range(-5, 5)) = 0.2661707
        _uvNoiseTile ("uvNoiseTile", Range(0, 15)) = 1
        _distStrength ("distStrength", Range(0, 0.1)) = 0
        _sceneMultiplicationR ("sceneMultiplicationR", Range(0, 15)) = 1
        _sceneMultiplicationG ("sceneMultiplicationG", Range(0, 15)) = 1
        _sceneMultiplicationB ("sceneMultiplicationB", Range(0, 15)) = 1
        _scenePow ("scenePow", Range(0, 11)) = 1
        _sceneDesaturation ("sceneDesaturation", Range(0, 1)) = 0
        _finalMix ("finalMix", Range(0, 1)) = 0
        _vignette ("vignette", Range(0, 2)) = 0
        _vignetteDarken ("vignetteDarken", Range(0, 1)) = 0
        _stainsColor ("stainsColor", Color) = (0.5,0.5,0.5,1)
        _stainsPattern ("stainsPattern", 2D) = "white" {}
        _stainsPow ("stainsPow", Range(0, 16)) = 1
        _stainsMult ("stainsMult", Range(0, 5)) = 2
        _stainsDistorter ("stainsDistorter", 2D) = "white" {}
        _stainsDistortion ("stainsDistortion", Range(0, 1)) = 0.3
        _stainsDistorterPanX ("stainsDistorterPanX", Range(-1, 1)) = 0
        _stainsDistorterPanY ("stainsDistorterPanY", Range(-1, 1)) = 0
        _stainsDistorterTile ("stainsDistorterTile", Range(0, 4)) = 1
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
            uniform float _sceneMultiplicationR;
            uniform sampler2D _uvNoiseTex; uniform float4 _uvNoiseTex_ST;
            uniform float _noisePanY;
            uniform float _uvNoiseTile;
            uniform float _distStrength;
            uniform float _noisePanX;
            uniform float _sceneMultiplicationG;
            uniform float _sceneMultiplicationB;
            uniform float _scenePow;
            uniform float _vignette;
            uniform float _vignetteDarken;
            uniform float4 _stainsColor;
            uniform sampler2D _stainsPattern; uniform float4 _stainsPattern_ST;
            uniform float _stainsPow;
            uniform float _stainsMult;
            uniform sampler2D _stainsDistorter; uniform float4 _stainsDistorter_ST;
            uniform float _stainsDistortion;
            uniform float _stainsDistorterPanX;
            uniform float _stainsDistorterPanY;
            uniform float _stainsDistorterTile;
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
                float node_2933 = 1.0;
                float4 node_9086 = _Time;
                float2 node_2988 = (frac((node_9086.g*float2(_noisePanX,_noisePanY)))+(i.uv0*_uvNoiseTile));
                float4 _uvNoiseTex_var = tex2D(_uvNoiseTex,TRANSFORM_TEX(node_2988, _uvNoiseTex));
                float node_3311 = (_distStrength*_finalMix);
                float2 node_4000 = lerp(i.uv0,(i.uv0+float2(_uvNoiseTex_var.r,_uvNoiseTex_var.g)),node_3311);
                float4 node_3687 = tex2D(_MainTex,TRANSFORM_TEX(node_4000, _MainTex));
                float3 node_110 = pow((float3(lerp(node_2933,_sceneMultiplicationR,_finalMix),lerp(node_2933,_sceneMultiplicationG,_finalMix),lerp(node_2933,_sceneMultiplicationB,_finalMix))*lerp(node_3687.rgb,dot(node_3687.rgb,float3(0.3,0.59,0.11)),lerp(0.0,_sceneDesaturation,_finalMix))),lerp(1.0,_scenePow,_finalMix));
                float node_7716 = (1.5*smoothstep( (1.0 - pow(_vignette,0.4)), 1.0, length((sceneUVs.rg*1.0+-0.5)) ));
                float4 node_8969 = _Time;
                float2 node_9101 = (frac((node_8969.g*float2(_stainsDistorterPanX,_stainsDistorterPanY)))+(i.uv0*_stainsDistorterTile));
                float4 _stainsDistorter_var = tex2D(_stainsDistorter,TRANSFORM_TEX(node_9101, _stainsDistorter));
                float2 node_3043 = lerp(i.uv0,(i.uv0+_stainsDistorter_var.r),_stainsDistortion);
                float4 _stainsPattern_var = tex2D(_stainsPattern,TRANSFORM_TEX(node_3043, _stainsPattern));
                float3 emissive = lerp(lerp(node_110,(node_110*(1.0 - _vignetteDarken)),node_7716),_stainsColor.rgb,((pow(_stainsPattern_var.r,_stainsPow)*node_7716)*_stainsMult));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
