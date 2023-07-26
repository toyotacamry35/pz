// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0.01379863,fgcg:0.5973264,fgcb:0.640137,fgca:1,fgde:0.0042009,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:34155,y:32695,varname:node_4795,prsc:2|emission-3293-OUT;n:type:ShaderForge.SFN_Tex2d,id:3202,x:30126,y:32371,ptovrint:False,ptlb:pattern1,ptin:_pattern1,varname:_pattern1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-866-OUT;n:type:ShaderForge.SFN_TexCoord,id:3826,x:29082,y:31946,varname:node_3826,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:7295,x:29558,y:32126,varname:node_7295,prsc:2|A-3826-UVOUT,B-712-OUT;n:type:ShaderForge.SFN_Slider,id:7495,x:28914,y:32185,ptovrint:False,ptlb:pattern1_tilingX,ptin:_pattern1_tilingX,varname:_pattern1_tilingX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Slider,id:1194,x:28942,y:32313,ptovrint:False,ptlb:pattern1_tilingY,ptin:_pattern1_tilingY,varname:_pattern1_tilingY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Append,id:712,x:29311,y:32219,varname:node_712,prsc:2|A-7495-OUT,B-1194-OUT;n:type:ShaderForge.SFN_Time,id:8343,x:29334,y:32533,varname:node_8343,prsc:2;n:type:ShaderForge.SFN_Slider,id:7262,x:28961,y:32682,ptovrint:False,ptlb:pattern1_PanX,ptin:_pattern1_PanX,varname:_pattern1_tilingX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-8,cur:0,max:8;n:type:ShaderForge.SFN_Slider,id:7769,x:28961,y:32808,ptovrint:False,ptlb:pattern1_PanY,ptin:_pattern1_PanY,varname:_pattern1_tilingY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-8,cur:0,max:8;n:type:ShaderForge.SFN_Append,id:1002,x:29334,y:32725,varname:node_1002,prsc:2|A-7262-OUT,B-7769-OUT;n:type:ShaderForge.SFN_Multiply,id:1762,x:29579,y:32643,varname:node_1762,prsc:2|A-8343-T,B-1002-OUT;n:type:ShaderForge.SFN_Add,id:866,x:29857,y:32372,varname:node_866,prsc:2|A-434-OUT,B-1762-OUT;n:type:ShaderForge.SFN_Tex2d,id:8246,x:30091,y:33424,ptovrint:False,ptlb:pattern2,ptin:_pattern2,varname:_pattern2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8758-OUT;n:type:ShaderForge.SFN_TexCoord,id:3693,x:29046,y:32999,varname:node_3693,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:4051,x:29522,y:33179,varname:node_4051,prsc:2|A-3693-UVOUT,B-7422-OUT;n:type:ShaderForge.SFN_Append,id:7422,x:29275,y:33273,varname:node_7422,prsc:2|A-2755-OUT,B-8977-OUT;n:type:ShaderForge.SFN_Time,id:469,x:29267,y:33546,varname:node_469,prsc:2;n:type:ShaderForge.SFN_Append,id:7314,x:29267,y:33738,varname:node_7314,prsc:2|A-5648-OUT,B-4519-OUT;n:type:ShaderForge.SFN_Multiply,id:9818,x:29512,y:33656,varname:node_9818,prsc:2|A-469-T,B-7314-OUT;n:type:ShaderForge.SFN_Add,id:8758,x:29822,y:33425,varname:node_8758,prsc:2|A-6644-OUT,B-9818-OUT;n:type:ShaderForge.SFN_Multiply,id:694,x:30496,y:32961,varname:node_694,prsc:2|A-5322-OUT,B-8479-OUT;n:type:ShaderForge.SFN_Multiply,id:7932,x:30734,y:32961,varname:node_7932,prsc:2|A-694-OUT,B-251-OUT;n:type:ShaderForge.SFN_Slider,id:251,x:30323,y:33214,ptovrint:False,ptlb:multCompensate,ptin:_multCompensate,varname:node_251,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:3;n:type:ShaderForge.SFN_Tex2d,id:8430,x:32041,y:32314,varname:node_8430,prsc:2,ntxv:0,isnm:False|UVIN-6753-OUT,TEX-5533-TEX;n:type:ShaderForge.SFN_Multiply,id:442,x:32017,y:32955,varname:node_442,prsc:2|A-5007-OUT,B-9197-OUT;n:type:ShaderForge.SFN_Multiply,id:3994,x:32831,y:32897,varname:node_3994,prsc:2|A-5979-OUT,B-5930-OUT;n:type:ShaderForge.SFN_Slider,id:5979,x:32617,y:32798,ptovrint:False,ptlb:em,ptin:_em,varname:node_5979,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1000;n:type:ShaderForge.SFN_Tex2d,id:216,x:30199,y:31877,ptovrint:False,ptlb:maskDeformer,ptin:_maskDeformer,varname:node_216,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1585-OUT;n:type:ShaderForge.SFN_RemapRange,id:750,x:30386,y:32024,varname:node_750,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-216-R;n:type:ShaderForge.SFN_TexCoord,id:4656,x:29062,y:30891,varname:node_4656,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:1314,x:29534,y:31165,varname:node_1314,prsc:2|A-4656-UVOUT,B-4309-OUT;n:type:ShaderForge.SFN_Append,id:4309,x:29291,y:31165,varname:node_4309,prsc:2|A-3640-OUT,B-2017-OUT;n:type:ShaderForge.SFN_Time,id:5874,x:29314,y:31343,varname:node_5874,prsc:2;n:type:ShaderForge.SFN_Slider,id:189,x:28941,y:31493,ptovrint:False,ptlb:maskDeformer_PanX,ptin:_maskDeformer_PanX,varname:_pattern1_PanX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-8,cur:0,max:8;n:type:ShaderForge.SFN_Slider,id:1326,x:28941,y:31618,ptovrint:False,ptlb:maskDeformer_PanY,ptin:_maskDeformer_PanY,varname:_pattern1_PanY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-8,cur:0,max:8;n:type:ShaderForge.SFN_Append,id:6711,x:29314,y:31535,varname:node_6711,prsc:2|A-189-OUT,B-1326-OUT;n:type:ShaderForge.SFN_Multiply,id:855,x:29559,y:31453,varname:node_855,prsc:2|A-5874-T,B-6711-OUT;n:type:ShaderForge.SFN_Add,id:1585,x:29750,y:31188,varname:node_1585,prsc:2|A-1314-OUT,B-855-OUT;n:type:ShaderForge.SFN_Slider,id:2755,x:28875,y:33255,ptovrint:False,ptlb:pattern2_tilingX,ptin:_pattern2_tilingX,varname:node_2755,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Slider,id:8977,x:28875,y:33359,ptovrint:False,ptlb:pattern2_tilingY,ptin:_pattern2_tilingY,varname:node_8977,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Slider,id:5648,x:28858,y:33713,ptovrint:False,ptlb:pattern2_PanX,ptin:_pattern2_PanX,varname:node_5648,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-8,cur:0,max:8;n:type:ShaderForge.SFN_Slider,id:4519,x:28858,y:33803,ptovrint:False,ptlb:pattern2_PanY,ptin:_pattern2_PanY,varname:_node_5648_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-8,cur:0,max:8;n:type:ShaderForge.SFN_Slider,id:3640,x:28920,y:31126,ptovrint:False,ptlb:maskDeformer_TilingX,ptin:_maskDeformer_TilingX,varname:node_3640,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Slider,id:2017,x:28920,y:31233,ptovrint:False,ptlb:maskDeformer_TilingY,ptin:_maskDeformer_TilingY,varname:_node_3640_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Slider,id:722,x:30659,y:32071,ptovrint:False,ptlb:maskDeformation,ptin:_maskDeformation,varname:node_722,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_SwitchProperty,id:7937,x:30552,y:31897,ptovrint:False,ptlb:deformerRemap,ptin:_deformerRemap,varname:node_7937,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-216-R,B-750-OUT;n:type:ShaderForge.SFN_TexCoord,id:4956,x:30981,y:31704,varname:node_4956,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:6500,x:30981,y:31903,varname:node_6500,prsc:2|A-7937-OUT,B-722-OUT;n:type:ShaderForge.SFN_Add,id:6753,x:31221,y:31847,varname:node_6753,prsc:2|A-4956-UVOUT,B-6500-OUT;n:type:ShaderForge.SFN_Power,id:5322,x:30315,y:32570,varname:node_5322,prsc:2|VAL-3202-R,EXP-8470-OUT;n:type:ShaderForge.SFN_Power,id:8479,x:30323,y:33362,varname:node_8479,prsc:2|VAL-8246-R,EXP-8129-OUT;n:type:ShaderForge.SFN_Slider,id:8470,x:29917,y:32629,ptovrint:False,ptlb:pattern1_pow,ptin:_pattern1_pow,varname:node_8470,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Slider,id:8129,x:29945,y:33151,ptovrint:False,ptlb:pattern2_pow,ptin:_pattern2_pow,varname:_pattern1_pow_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Tex2d,id:6572,x:32041,y:32544,varname:_maskTex_copy,prsc:2,ntxv:0,isnm:False|TEX-5533-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:5533,x:31788,y:32394,ptovrint:False,ptlb:maskTex,ptin:_maskTex,varname:node_5533,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:5007,x:32350,y:32425,varname:node_5007,prsc:2|A-6572-R,B-8430-R;n:type:ShaderForge.SFN_Power,id:5930,x:32466,y:32966,varname:node_5930,prsc:2|VAL-1446-OUT,EXP-7960-OUT;n:type:ShaderForge.SFN_Slider,id:7960,x:32176,y:33183,ptovrint:False,ptlb:finalContrast,ptin:_finalContrast,varname:node_7960,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Subtract,id:5663,x:33115,y:32914,varname:node_5663,prsc:2|A-3994-OUT,B-7338-OUT;n:type:ShaderForge.SFN_Clamp01,id:1446,x:32227,y:32955,varname:node_1446,prsc:2|IN-442-OUT;n:type:ShaderForge.SFN_Multiply,id:8854,x:33474,y:32843,varname:node_8854,prsc:2|A-7211-OUT,B-6637-RGB,C-6637-A;n:type:ShaderForge.SFN_VertexColor,id:6637,x:33283,y:33156,varname:node_6637,prsc:2;n:type:ShaderForge.SFN_Divide,id:5283,x:33701,y:32843,varname:node_5283,prsc:2|A-8854-OUT,B-6474-OUT;n:type:ShaderForge.SFN_Slider,id:6474,x:33579,y:33110,ptovrint:False,ptlb:softness,ptin:_softness,varname:node_6474,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:3293,x:33900,y:32824,varname:node_3293,prsc:2|A-7365-RGB,B-5283-OUT;n:type:ShaderForge.SFN_Color,id:7365,x:33671,y:32597,ptovrint:False,ptlb:col,ptin:_col,varname:node_7365,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:434,x:29720,y:32253,varname:node_434,prsc:2|A-7295-OUT,B-1410-OUT;n:type:ShaderForge.SFN_Add,id:6644,x:29683,y:33295,varname:node_6644,prsc:2|A-4051-OUT,B-1410-OUT;n:type:ShaderForge.SFN_TexCoord,id:5737,x:28285,y:32453,varname:node_5737,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Multiply,id:1410,x:28524,y:32541,varname:node_1410,prsc:2|A-5737-U,B-2752-OUT;n:type:ShaderForge.SFN_Vector2,id:2752,x:28273,y:32694,varname:node_2752,prsc:2,v1:-3,v2:7;n:type:ShaderForge.SFN_TexCoord,id:6624,x:32707,y:33259,varname:node_6624,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Multiply,id:7338,x:32961,y:33286,varname:node_7338,prsc:2|A-6624-V,B-2427-OUT;n:type:ShaderForge.SFN_Vector1,id:2427,x:32743,y:33472,varname:node_2427,prsc:2,v1:1000;n:type:ShaderForge.SFN_Clamp01,id:7211,x:33297,y:32843,varname:node_7211,prsc:2|IN-5663-OUT;n:type:ShaderForge.SFN_Subtract,id:9197,x:31190,y:32976,varname:node_9197,prsc:2|A-7932-OUT,B-5737-W;proporder:3202-7495-1194-7262-7769-8246-2755-8977-5648-4519-216-3640-2017-189-1326-251-5979-722-7937-8470-8129-5533-7960-6474-7365;pass:END;sub:END;*/

Shader "Colony_FX/Basic/S_MultipliedTextures_Additive" {
    Properties {
        _pattern1 ("pattern1", 2D) = "white" {}
        _pattern1_tilingX ("pattern1_tilingX", Range(0, 10)) = 1
        _pattern1_tilingY ("pattern1_tilingY", Range(0, 10)) = 1
        _pattern1_PanX ("pattern1_PanX", Range(-8, 8)) = 0
        _pattern1_PanY ("pattern1_PanY", Range(-8, 8)) = 0
        _pattern2 ("pattern2", 2D) = "white" {}
        _pattern2_tilingX ("pattern2_tilingX", Range(0, 10)) = 1
        _pattern2_tilingY ("pattern2_tilingY", Range(0, 10)) = 1
        _pattern2_PanX ("pattern2_PanX", Range(-8, 8)) = 0
        _pattern2_PanY ("pattern2_PanY", Range(-8, 8)) = 0
        _maskDeformer ("maskDeformer", 2D) = "white" {}
        _maskDeformer_TilingX ("maskDeformer_TilingX", Range(0, 10)) = 1
        _maskDeformer_TilingY ("maskDeformer_TilingY", Range(0, 10)) = 1
        _maskDeformer_PanX ("maskDeformer_PanX", Range(-8, 8)) = 0
        _maskDeformer_PanY ("maskDeformer_PanY", Range(-8, 8)) = 0
        _multCompensate ("multCompensate", Range(0, 3)) = 0
        _em ("em", Range(0, 1000)) = 1
        _maskDeformation ("maskDeformation", Range(0, 2)) = 0
        [MaterialToggle] _deformerRemap ("deformerRemap", Float ) = 0
        _pattern1_pow ("pattern1_pow", Range(0, 15)) = 1
        _pattern2_pow ("pattern2_pow", Range(0, 15)) = 1
        _maskTex ("maskTex", 2D) = "white" {}
        _finalContrast ("finalContrast", Range(0, 5)) = 0
        _softness ("softness", Range(0, 1)) = 1
        _col ("col", Color) = (0.5,0.5,0.5,1)
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
            Blend One One
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
            uniform sampler2D _pattern1; uniform float4 _pattern1_ST;
            uniform float _pattern1_tilingX;
            uniform float _pattern1_tilingY;
            uniform float _pattern1_PanX;
            uniform float _pattern1_PanY;
            uniform sampler2D _pattern2; uniform float4 _pattern2_ST;
            uniform float _multCompensate;
            uniform float _em;
            uniform sampler2D _maskDeformer; uniform float4 _maskDeformer_ST;
            uniform float _maskDeformer_PanX;
            uniform float _maskDeformer_PanY;
            uniform float _pattern2_tilingX;
            uniform float _pattern2_tilingY;
            uniform float _pattern2_PanX;
            uniform float _pattern2_PanY;
            uniform float _maskDeformer_TilingX;
            uniform float _maskDeformer_TilingY;
            uniform float _maskDeformation;
            uniform fixed _deformerRemap;
            uniform float _pattern1_pow;
            uniform float _pattern2_pow;
            uniform sampler2D _maskTex; uniform float4 _maskTex_ST;
            uniform float _finalContrast;
            uniform float _softness;
            uniform float4 _col;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
				float3 air : TEXCOORD3;
				float3 hazeAndFog : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
				DS_Haze_Per_Vertex(v.vertex, o.air, o.hazeAndFog);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _maskTex_copy = tex2D(_maskTex,TRANSFORM_TEX(i.uv0, _maskTex));
                float4 node_5874 = _Time;
                float2 node_1585 = ((i.uv0*float2(_maskDeformer_TilingX,_maskDeformer_TilingY))+(node_5874.g*float2(_maskDeformer_PanX,_maskDeformer_PanY)));
                float4 _maskDeformer_var = tex2D(_maskDeformer,TRANSFORM_TEX(node_1585, _maskDeformer));
                float2 node_6753 = (i.uv0+(lerp( _maskDeformer_var.r, (_maskDeformer_var.r*2.0+-1.0), _deformerRemap )*_maskDeformation));
                float4 node_8430 = tex2D(_maskTex,TRANSFORM_TEX(node_6753, _maskTex));
                float2 node_1410 = (i.uv1.r*float2(-3,7));
                float4 node_8343 = _Time;
                float2 node_866 = (((i.uv0*float2(_pattern1_tilingX,_pattern1_tilingY))+node_1410)+(node_8343.g*float2(_pattern1_PanX,_pattern1_PanY)));
                float4 _pattern1_var = tex2D(_pattern1,TRANSFORM_TEX(node_866, _pattern1));
                float4 node_469 = _Time;
                float2 node_8758 = (((i.uv0*float2(_pattern2_tilingX,_pattern2_tilingY))+node_1410)+(node_469.g*float2(_pattern2_PanX,_pattern2_PanY)));
                float4 _pattern2_var = tex2D(_pattern2,TRANSFORM_TEX(node_8758, _pattern2));
                float3 emissive = (_col.rgb*((saturate(((_em*pow(saturate(((_maskTex_copy.r*node_8430.r)*(((pow(_pattern1_var.r,_pattern1_pow)*pow(_pattern2_var.r,_pattern2_pow))*_multCompensate)-i.uv1.a))),_finalContrast))-(i.uv1.g*1000.0)))*i.vertexColor.rgb*i.vertexColor.a)/_softness));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, unity_FogColor);
				DS_Haze_Apply(i.air, i.hazeAndFog, finalRGBA, finalRGBA.a);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
