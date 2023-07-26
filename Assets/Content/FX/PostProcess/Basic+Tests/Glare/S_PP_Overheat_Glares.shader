// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33970,y:33937,varname:node_2865,prsc:2|emission-2291-OUT;n:type:ShaderForge.SFN_TexCoord,id:4219,x:31483,y:33956,cmnt:Default coordinates,varname:node_4219,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:31442,y:33288,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:node_9933,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7542,x:32115,y:33969,varname:node_1672,prsc:2,ntxv:0,isnm:False|UVIN-5523-OUT,TEX-4430-TEX;n:type:ShaderForge.SFN_Multiply,id:9709,x:31677,y:34039,varname:node_9709,prsc:2|A-4116-OUT,B-4219-V;n:type:ShaderForge.SFN_Slider,id:4116,x:31554,y:34195,ptovrint:False,ptlb:sceneTexDistortedTiling,ptin:_sceneTexDistortedTiling,varname:node_4116,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:7;n:type:ShaderForge.SFN_Append,id:5523,x:31876,y:33969,varname:node_5523,prsc:2|A-4219-U,B-9709-OUT;n:type:ShaderForge.SFN_Lerp,id:2291,x:33598,y:34071,varname:node_2291,prsc:2|A-4790-OUT,B-5575-OUT,T-5237-OUT;n:type:ShaderForge.SFN_Tex2d,id:9746,x:32430,y:34619,varname:node_9746,prsc:2,ntxv:0,isnm:False|UVIN-4894-OUT,TEX-820-TEX;n:type:ShaderForge.SFN_Tex2d,id:7660,x:32592,y:33580,varname:node_7660,prsc:2,ntxv:0,isnm:False|TEX-4430-TEX;n:type:ShaderForge.SFN_Power,id:4467,x:32690,y:34605,varname:node_4467,prsc:2|VAL-9746-R,EXP-5567-OUT;n:type:ShaderForge.SFN_Slider,id:5567,x:32316,y:34872,ptovrint:False,ptlb:maskContrast,ptin:_maskContrast,varname:node_5567,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.891811,max:5;n:type:ShaderForge.SFN_Multiply,id:1335,x:32895,y:34605,varname:node_1335,prsc:2|A-4467-OUT,B-7991-OUT;n:type:ShaderForge.SFN_Slider,id:1383,x:33233,y:34654,ptovrint:False,ptlb:maskMult,ptin:_maskMult,varname:node_1383,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:10,max:10;n:type:ShaderForge.SFN_Multiply,id:5575,x:33227,y:34135,varname:node_5575,prsc:2|A-5885-RGB,B-8084-OUT,C-6908-OUT;n:type:ShaderForge.SFN_Slider,id:8084,x:32865,y:34245,ptovrint:False,ptlb:brightness,ptin:_brightness,varname:node_8084,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:111;n:type:ShaderForge.SFN_TexCoord,id:3994,x:31664,y:34631,varname:node_3994,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:2625,x:31962,y:34507,varname:node_2625,prsc:2|A-7970-OUT,B-7158-OUT,C-3994-U,D-8298-OUT;n:type:ShaderForge.SFN_Append,id:4894,x:32153,y:34685,varname:node_4894,prsc:2|A-2625-OUT,B-3994-V;n:type:ShaderForge.SFN_Multiply,id:7158,x:31644,y:34495,varname:node_7158,prsc:2|A-5603-OUT,B-3354-OUT;n:type:ShaderForge.SFN_Slider,id:3354,x:31270,y:34579,ptovrint:False,ptlb:pixelInfluence,ptin:_pixelInfluence,varname:node_3354,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Multiply,id:1022,x:32732,y:33963,varname:node_1022,prsc:2|A-7542-R,B-5885-RGB;n:type:ShaderForge.SFN_Color,id:5885,x:32509,y:34166,ptovrint:False,ptlb:col,ptin:_col,varname:node_5885,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Desaturate,id:4790,x:32866,y:33580,varname:node_4790,prsc:2|COL-7660-RGB,DES-6478-OUT;n:type:ShaderForge.SFN_Slider,id:6478,x:32709,y:33806,ptovrint:False,ptlb:desat,ptin:_desat,varname:node_6478,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Set,id:7418,x:32279,y:34033,varname:redPix,prsc:2|IN-7542-R;n:type:ShaderForge.SFN_Get,id:5603,x:31429,y:34495,varname:node_5603,prsc:2|IN-7418-OUT;n:type:ShaderForge.SFN_Color,id:620,x:30989,y:34241,ptovrint:False,ptlb:_camDirection,ptin:_camDirection,varname:node_620,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:7970,x:31270,y:34326,varname:node_7970,prsc:2|A-620-B,B-8278-OUT;n:type:ShaderForge.SFN_Slider,id:8278,x:30922,y:34514,ptovrint:False,ptlb:directionInfluence,ptin:_directionInfluence,varname:node_8278,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:5;n:type:ShaderForge.SFN_ConstantClamp,id:6908,x:32943,y:33953,varname:node_6908,prsc:2,min:0.5,max:1|IN-1022-OUT;n:type:ShaderForge.SFN_Time,id:7244,x:32112,y:35185,varname:node_7244,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7629,x:32851,y:34876,varname:node_7629,prsc:2|A-3922-OUT,B-7539-OUT;n:type:ShaderForge.SFN_Vector1,id:7539,x:32799,y:35032,varname:node_7539,prsc:2,v1:0.12;n:type:ShaderForge.SFN_Multiply,id:9944,x:32343,y:35247,varname:node_9944,prsc:2|A-7244-T,B-6059-OUT;n:type:ShaderForge.SFN_Slider,id:6059,x:31980,y:35353,ptovrint:False,ptlb:frequency,ptin:_frequency,varname:node_6059,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.5,max:7;n:type:ShaderForge.SFN_Cos,id:2078,x:31679,y:34951,varname:node_2078,prsc:2|IN-5574-OUT;n:type:ShaderForge.SFN_Sin,id:2410,x:32438,y:35470,varname:node_2410,prsc:2|IN-9944-OUT;n:type:ShaderForge.SFN_Cos,id:109,x:32836,y:35534,varname:node_109,prsc:2|IN-9351-OUT;n:type:ShaderForge.SFN_Multiply,id:9351,x:32638,y:35550,varname:node_9351,prsc:2|A-2410-OUT,B-1185-OUT;n:type:ShaderForge.SFN_Pi,id:2754,x:32454,y:35632,varname:node_2754,prsc:2;n:type:ShaderForge.SFN_Distance,id:3922,x:33051,y:35473,varname:node_3922,prsc:2|A-2410-OUT,B-109-OUT;n:type:ShaderForge.SFN_Multiply,id:1185,x:32638,y:35679,varname:node_1185,prsc:2|A-2754-OUT,B-2208-OUT;n:type:ShaderForge.SFN_Vector1,id:2208,x:32438,y:35745,varname:node_2208,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Tex2dAsset,id:820,x:31444,y:33591,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_820,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:5579,x:32418,y:34434,varname:node_5579,prsc:2,ntxv:0,isnm:False|UVIN-3910-OUT,TEX-820-TEX;n:type:ShaderForge.SFN_Power,id:2927,x:32662,y:34436,varname:node_2927,prsc:2|VAL-5579-R,EXP-5567-OUT;n:type:ShaderForge.SFN_Multiply,id:785,x:32865,y:34436,varname:node_785,prsc:2|A-2927-OUT,B-7991-OUT;n:type:ShaderForge.SFN_Clamp01,id:5237,x:33413,y:34428,varname:node_5237,prsc:2|IN-7509-OUT;n:type:ShaderForge.SFN_Multiply,id:7509,x:33212,y:34428,varname:node_7509,prsc:2|A-785-OUT,B-1335-OUT,C-1383-OUT;n:type:ShaderForge.SFN_Add,id:8573,x:31986,y:34302,varname:node_8573,prsc:2|A-2625-OUT,B-695-OUT;n:type:ShaderForge.SFN_Sin,id:4301,x:31664,y:34811,varname:node_4301,prsc:2|IN-5574-OUT;n:type:ShaderForge.SFN_Append,id:3910,x:32193,y:34302,varname:node_3910,prsc:2|A-8573-OUT,B-3994-V;n:type:ShaderForge.SFN_Multiply,id:5574,x:31415,y:34913,varname:node_5574,prsc:2|A-9944-OUT,B-9542-OUT;n:type:ShaderForge.SFN_Vector1,id:9542,x:31266,y:35035,varname:node_9542,prsc:2,v1:0.27;n:type:ShaderForge.SFN_Multiply,id:695,x:31869,y:34793,varname:node_695,prsc:2|A-4301-OUT,B-9877-OUT;n:type:ShaderForge.SFN_Vector1,id:9877,x:31824,y:34927,varname:node_9877,prsc:2,v1:0.05;n:type:ShaderForge.SFN_Multiply,id:8298,x:31861,y:34972,varname:node_8298,prsc:2|A-9877-OUT,B-2078-OUT;n:type:ShaderForge.SFN_Add,id:7991,x:33048,y:34861,varname:node_7991,prsc:2|A-7629-OUT,B-6152-OUT;n:type:ShaderForge.SFN_Vector1,id:6152,x:32919,y:34969,varname:node_6152,prsc:2,v1:0.05;proporder:4430-5885-8084-6478-6059-4116-820-5567-1383-3354-8278;pass:END;sub:END;*/

Shader "Colony_FX/S_PP_Overheat_Glares" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _col ("col", Color) = (0.5,0.5,0.5,1)
        _brightness ("brightness", Range(0, 111)) = 1
        _desat ("desat", Range(0, 1)) = 0
        _frequency ("frequency", Range(0, 7)) = 2.5
        _sceneTexDistortedTiling ("sceneTexDistortedTiling", Range(0, 7)) = 2
        _mask ("mask", 2D) = "white" {}
        _maskContrast ("maskContrast", Range(0, 5)) = 1.891811
        _maskMult ("maskMult", Range(0, 10)) = 10
        _pixelInfluence ("pixelInfluence", Range(0, 15)) = 1
        _directionInfluence ("directionInfluence", Range(0, 5)) = 2
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
            uniform float _sceneTexDistortedTiling;
            uniform float _maskContrast;
            uniform float _maskMult;
            uniform float _brightness;
            uniform float _pixelInfluence;
            uniform float4 _col;
            uniform float _desat;
            uniform float4 _camDirection;
            uniform float _directionInfluence;
            uniform float _frequency;
            uniform sampler2D _mask; uniform float4 _mask_ST;
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
                float4 node_7660 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float2 node_5523 = float2(i.uv0.r,(_sceneTexDistortedTiling*i.uv0.g));
                float4 node_1672 = tex2D(_MainTex,TRANSFORM_TEX(node_5523, _MainTex));
                float redPix = node_1672.r;
                float node_9877 = 0.05;
                float4 node_7244 = _Time;
                float node_9944 = (node_7244.g*_frequency);
                float node_5574 = (node_9944*0.27);
                float node_2625 = ((_camDirection.b*_directionInfluence)+(redPix*_pixelInfluence)+i.uv0.r+(node_9877*cos(node_5574)));
                float2 node_3910 = float2((node_2625+(sin(node_5574)*node_9877)),i.uv0.g);
                float4 node_5579 = tex2D(_mask,TRANSFORM_TEX(node_3910, _mask));
                float node_2410 = sin(node_9944);
                float node_7991 = ((distance(node_2410,cos((node_2410*(3.141592654*0.7))))*0.12)+0.05);
                float2 node_4894 = float2(node_2625,i.uv0.g);
                float4 node_9746 = tex2D(_mask,TRANSFORM_TEX(node_4894, _mask));
                float3 emissive = lerp(lerp(node_7660.rgb,dot(node_7660.rgb,float3(0.3,0.59,0.11)),_desat),(_col.rgb*_brightness*clamp((node_1672.r*_col.rgb),0.5,1)),saturate(((pow(node_5579.r,_maskContrast)*node_7991)*(pow(node_9746.r,_maskContrast)*node_7991)*_maskMult)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
