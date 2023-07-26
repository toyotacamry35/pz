// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0.01379863,fgcg:0.5973264,fgcb:0.640137,fgca:1,fgde:0.0042009,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:34302,y:32682,varname:node_4795,prsc:2|emission-3066-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:31213,y:32360,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4310-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:32713,y:32748,varname:node_2393,prsc:2|A-1189-OUT,B-2053-RGB,C-2053-A,D-6660-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32172,y:32771,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Slider,id:6660,x:32136,y:32922,ptovrint:False,ptlb:emissive,ptin:_emissive,varname:_emissive,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:255;n:type:ShaderForge.SFN_DepthBlend,id:9082,x:33855,y:32961,varname:node_9082,prsc:2|DIST-1527-OUT;n:type:ShaderForge.SFN_Slider,id:1527,x:33760,y:33165,ptovrint:False,ptlb:depthFade,ptin:_depthFade,varname:_depthFade,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:100;n:type:ShaderForge.SFN_Multiply,id:3066,x:33916,y:32788,varname:node_3066,prsc:2|A-462-OUT,B-9082-OUT;n:type:ShaderForge.SFN_Multiply,id:9437,x:33048,y:32756,varname:node_9437,prsc:2|A-2393-OUT,B-5540-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:5540,x:32869,y:32943,ptovrint:False,ptlb:useVerticalFadeIn,ptin:_useVerticalFadeIn,varname:_useVerticalFadeIn,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-338-OUT,B-400-OUT;n:type:ShaderForge.SFN_Vector1,id:338,x:32630,y:32956,varname:node_338,prsc:2,v1:1;n:type:ShaderForge.SFN_TexCoord,id:264,x:31799,y:33149,varname:node_264,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Subtract,id:5011,x:32072,y:33190,varname:node_5011,prsc:2|A-264-V,B-2848-OUT;n:type:ShaderForge.SFN_Vector1,id:2848,x:31848,y:33328,varname:node_2848,prsc:2,v1:1;n:type:ShaderForge.SFN_Add,id:525,x:32320,y:33190,varname:node_525,prsc:2|A-5011-OUT,B-4365-U;n:type:ShaderForge.SFN_Clamp01,id:400,x:32520,y:33190,varname:node_400,prsc:2|IN-525-OUT;n:type:ShaderForge.SFN_TexCoord,id:4365,x:31848,y:33404,varname:node_4365,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Power,id:1189,x:32467,y:32568,varname:node_1189,prsc:2|VAL-4662-OUT,EXP-8677-OUT;n:type:ShaderForge.SFN_TexCoord,id:474,x:32736,y:33489,varname:node_474,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Lerp,id:8677,x:33179,y:33458,varname:node_8677,prsc:2|A-3336-OUT,B-474-U,T-9878-OUT;n:type:ShaderForge.SFN_Slider,id:9878,x:32740,y:33686,ptovrint:False,ptlb:dynamicContrastBool,ptin:_dynamicContrastBool,varname:_dynamicContrastBool,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Vector1,id:3336,x:32750,y:33429,varname:node_3336,prsc:2,v1:1;n:type:ShaderForge.SFN_Divide,id:462,x:33636,y:32788,varname:node_462,prsc:2|A-4922-OUT,B-1759-OUT;n:type:ShaderForge.SFN_Vector1,id:1544,x:33169,y:32981,varname:node_1544,prsc:2,v1:1;n:type:ShaderForge.SFN_Lerp,id:1759,x:33361,y:33126,varname:node_1759,prsc:2|A-1544-OUT,B-6995-OUT,T-4387-OUT;n:type:ShaderForge.SFN_Slider,id:4387,x:32916,y:33231,ptovrint:False,ptlb:useSoftness,ptin:_useSoftness,varname:_useSoftness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:6995,x:32916,y:33144,ptovrint:False,ptlb:softness,ptin:_softness,varname:_softness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:4662,x:31866,y:32366,varname:node_4662,prsc:2|A-6074-R,B-6291-OUT,T-7665-OUT;n:type:ShaderForge.SFN_TexCoord,id:2730,x:31213,y:32535,varname:node_2730,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Subtract,id:3374,x:31439,y:32516,varname:node_3374,prsc:2|A-6074-R,B-2730-V;n:type:ShaderForge.SFN_Slider,id:7665,x:31411,y:32796,ptovrint:False,ptlb:useErosion,ptin:_useErosion,varname:_useErosion,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_ConstantClamp,id:6291,x:31620,y:32516,varname:node_6291,prsc:2,min:0,max:1|IN-3374-OUT;n:type:ShaderForge.SFN_Rotator,id:6875,x:30689,y:32379,varname:node_6875,prsc:2|UVIN-3230-UVOUT,PIV-489-OUT,ANG-5458-OUT;n:type:ShaderForge.SFN_TexCoord,id:4963,x:30072,y:32545,varname:node_4963,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_TexCoord,id:3230,x:30392,y:32209,varname:node_3230,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:4310,x:30978,y:32360,varname:node_4310,prsc:2|A-3230-UVOUT,B-6875-UVOUT,T-9137-OUT;n:type:ShaderForge.SFN_Slider,id:9137,x:30532,y:32639,ptovrint:False,ptlb:useRotator,ptin:_useRotator,varname:_useRotator,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:5458,x:30303,y:32602,varname:node_5458,prsc:2|A-4963-Z,B-6999-OUT;n:type:ShaderForge.SFN_Pi,id:5611,x:30034,y:32790,varname:node_5611,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6999,x:30178,y:32802,varname:node_6999,prsc:2|A-5611-OUT,B-3170-OUT;n:type:ShaderForge.SFN_Vector1,id:3170,x:30036,y:32898,varname:node_3170,prsc:2,v1:2;n:type:ShaderForge.SFN_TexCoord,id:472,x:31526,y:31709,varname:node_472,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:8736,x:31562,y:32110,ptovrint:False,ptlb:frames,ptin:_frames,varname:node_8736,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4,max:65;n:type:ShaderForge.SFN_Append,id:489,x:30392,y:32396,varname:node_489,prsc:2|A-7162-OUT,B-6827-OUT;n:type:ShaderForge.SFN_Vector1,id:6827,x:30176,y:32430,varname:node_6827,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Vector1,id:7162,x:30176,y:32370,varname:node_7162,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:4922,x:33315,y:32729,varname:node_4922,prsc:2|A-6850-OUT,B-9437-OUT;n:type:ShaderForge.SFN_Vector1,id:7198,x:32937,y:32290,varname:node_7198,prsc:2,v1:1;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:8670,x:31983,y:31960,varname:node_8670,prsc:2|IN-9029-OUT,IMIN-1461-OUT,IMAX-5306-OUT,OMIN-1461-OUT,OMAX-8736-OUT;n:type:ShaderForge.SFN_Vector1,id:1461,x:31722,y:31930,varname:node_1461,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:5306,x:31719,y:31990,varname:node_5306,prsc:2,v1:1;n:type:ShaderForge.SFN_Subtract,id:7148,x:32188,y:32090,varname:node_7148,prsc:2|A-8670-OUT,B-8736-OUT;n:type:ShaderForge.SFN_Add,id:9814,x:32773,y:32099,varname:node_9814,prsc:2|A-9498-OUT,B-4365-U;n:type:ShaderForge.SFN_TexCoord,id:6513,x:32157,y:31763,varname:node_6513,prsc:2,uv:2,uaff:False;n:type:ShaderForge.SFN_Clamp01,id:2135,x:33046,y:32067,varname:node_2135,prsc:2|IN-9814-OUT;n:type:ShaderForge.SFN_Lerp,id:6850,x:33231,y:32382,varname:node_6850,prsc:2|A-7198-OUT,B-2135-OUT,T-633-OUT;n:type:ShaderForge.SFN_Slider,id:633,x:32822,y:32536,ptovrint:False,ptlb:horizontalFadeIn,ptin:_horizontalFadeIn,varname:node_633,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_OneMinus,id:9029,x:31782,y:31731,varname:node_9029,prsc:2|IN-472-U;n:type:ShaderForge.SFN_Add,id:9498,x:32487,y:32071,varname:node_9498,prsc:2|A-9141-OUT,B-7148-OUT;n:type:ShaderForge.SFN_Floor,id:9141,x:32322,y:31793,varname:node_9141,prsc:2|IN-6513-U;proporder:6074-6660-1527-5540-9878-4387-6995-7665-9137-8736-633;pass:END;sub:END;*/

Shader "Colony_FX/Basic/S_Additive" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _emissive ("emissive", Range(0, 255)) = 1
        _depthFade ("depthFade", Range(0, 100)) = 0
        [MaterialToggle] _useVerticalFadeIn ("useVerticalFadeIn", Float ) = 1
        _dynamicContrastBool ("dynamicContrastBool", Range(0, 1)) = 0
        _useSoftness ("useSoftness", Range(0, 1)) = 0
        _softness ("softness", Range(0, 1)) = 0
        _useErosion ("useErosion", Range(0, 1)) = 0
        _useRotator ("useRotator", Range(0, 1)) = 0
        _frames ("frames", Range(0, 65)) = 4
        _horizontalFadeIn ("horizontalFadeIn", Range(0, 1)) = 0
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
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
			//#define DS_HAZE_FULL
			
            #include "UnityCG.cginc"
			#include "Assets/ASkyLighting/DeepSky Haze/Resources/DS_TransparentLib.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _emissive;
            uniform float _depthFade;
            uniform fixed _useVerticalFadeIn;
            uniform float _dynamicContrastBool;
            uniform float _useSoftness;
            uniform float _softness;
            uniform float _useErosion;
            uniform float _useRotator;
            uniform float _frames;
            uniform float _horizontalFadeIn;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD3;
                UNITY_FOG_COORDS(4)
				float3 air : TEXCOORD5;
				float3 hazeAndFog : TEXCOORD6;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
				DS_Haze_Per_Vertex(v.vertex, o.air, o.hazeAndFog);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float node_1461 = 0.0;
                float node_6875_ang = (i.uv1.b*(3.141592654*2.0));
                float node_6875_spd = 1.0;
                float node_6875_cos = cos(node_6875_spd*node_6875_ang);
                float node_6875_sin = sin(node_6875_spd*node_6875_ang);
                float2 node_6875_piv = float2(0.5,0.5);
                float2 node_6875 = (mul(i.uv0-node_6875_piv,float2x2( node_6875_cos, -node_6875_sin, node_6875_sin, node_6875_cos))+node_6875_piv);
                float2 node_4310 = lerp(i.uv0,node_6875,_useRotator);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4310, _MainTex));
                float3 emissive = (((lerp(1.0,saturate(((floor(i.uv2.r)+((node_1461 + ( ((1.0 - i.uv0.r) - node_1461) * (_frames - node_1461) ) / (1.0 - node_1461))-_frames))+i.uv1.r)),_horizontalFadeIn)*((pow(lerp(_MainTex_var.r,clamp((_MainTex_var.r-i.uv1.g),0,1),_useErosion),lerp(1.0,i.uv1.r,_dynamicContrastBool))*i.vertexColor.rgb*i.vertexColor.a*_emissive)*lerp( 1.0, saturate(((i.uv0.g-1.0)+i.uv1.r)), _useVerticalFadeIn )))/lerp(1.0,_softness,_useSoftness))*saturate((sceneZ-partZ)/_depthFade));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, unity_FogColor);
				//DS_Haze_Apply(i.air, i.hazeAndFog, finalRGBA, finalRGBA.a);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
