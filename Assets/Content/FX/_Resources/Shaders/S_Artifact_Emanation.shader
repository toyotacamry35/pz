// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.02642458,fgcg:0.5738795,fgcb:0.6129846,fgca:1,fgde:0.004212498,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:35225,y:32671,varname:node_3138,prsc:2|emission-4633-OUT,alpha-8713-OUT;n:type:ShaderForge.SFN_TexCoord,id:1553,x:31567,y:32618,varname:node_1553,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Power,id:2169,x:32325,y:32749,varname:node_2169,prsc:2|VAL-8766-OUT,EXP-6374-OUT;n:type:ShaderForge.SFN_OneMinus,id:7226,x:31998,y:32749,varname:node_7226,prsc:2|IN-8949-OUT;n:type:ShaderForge.SFN_Add,id:8949,x:31815,y:32749,varname:node_8949,prsc:2|A-1553-V,B-6904-OUT;n:type:ShaderForge.SFN_Clamp01,id:8766,x:32165,y:32749,varname:node_8766,prsc:2|IN-7226-OUT;n:type:ShaderForge.SFN_Slider,id:6374,x:31982,y:32974,ptovrint:False,ptlb:mainShapePow,ptin:_mainShapePow,varname:node_6374,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:7.905983,max:25;n:type:ShaderForge.SFN_Slider,id:6904,x:31523,y:33026,ptovrint:False,ptlb:mainShapeOffset,ptin:_mainShapeOffset,varname:node_6904,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:2,max:2;n:type:ShaderForge.SFN_OneMinus,id:7341,x:32721,y:33215,varname:node_7341,prsc:2|IN-2169-OUT;n:type:ShaderForge.SFN_Lerp,id:6254,x:32802,y:32978,varname:node_6254,prsc:2|A-2605-OUT,B-7317-OUT,T-7341-OUT;n:type:ShaderForge.SFN_Vector1,id:2605,x:32589,y:32978,varname:node_2605,prsc:2,v1:0;n:type:ShaderForge.SFN_Tex2d,id:4233,x:32065,y:33237,ptovrint:False,ptlb:pattern,ptin:_pattern,varname:node_4233,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5738-OUT;n:type:ShaderForge.SFN_TexCoord,id:7274,x:31601,y:33182,varname:node_7274,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:5738,x:31834,y:33237,varname:node_5738,prsc:2|A-7274-UVOUT,B-469-OUT,C-346-OUT;n:type:ShaderForge.SFN_Time,id:2588,x:31413,y:33359,varname:node_2588,prsc:2;n:type:ShaderForge.SFN_Multiply,id:469,x:31700,y:33434,varname:node_469,prsc:2|A-2588-T,B-8934-OUT;n:type:ShaderForge.SFN_Slider,id:241,x:31146,y:33649,ptovrint:False,ptlb:panY,ptin:_panY,varname:node_241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:0,max:2;n:type:ShaderForge.SFN_Append,id:8934,x:31508,y:33574,varname:node_8934,prsc:2|A-39-OUT,B-241-OUT;n:type:ShaderForge.SFN_Subtract,id:2905,x:32983,y:32741,varname:node_2905,prsc:2|A-3816-R,B-6254-OUT;n:type:ShaderForge.SFN_Power,id:8654,x:32280,y:33254,varname:node_8654,prsc:2|VAL-4233-R,EXP-5109-OUT;n:type:ShaderForge.SFN_Divide,id:7317,x:32463,y:33254,varname:node_7317,prsc:2|A-8654-OUT,B-1132-OUT;n:type:ShaderForge.SFN_Slider,id:1132,x:32232,y:33611,ptovrint:False,ptlb:patternSoftness,ptin:_patternSoftness,varname:node_1132,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:5109,x:31880,y:33613,ptovrint:False,ptlb:patternPow,ptin:_patternPow,varname:_patternSoftness_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:15;n:type:ShaderForge.SFN_Multiply,id:1747,x:33830,y:32776,varname:node_1747,prsc:2|A-2128-OUT,B-8349-OUT;n:type:ShaderForge.SFN_TexCoord,id:5582,x:33091,y:32926,varname:node_5582,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:7839,x:33012,y:33214,ptovrint:False,ptlb:node_7839,ptin:_node_7839,varname:node_7839,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.058856,max:155;n:type:ShaderForge.SFN_Power,id:7706,x:33478,y:32984,varname:node_7706,prsc:2|VAL-1487-OUT,EXP-7839-OUT;n:type:ShaderForge.SFN_Add,id:1487,x:33283,y:32984,varname:node_1487,prsc:2|A-5582-V,B-2546-OUT;n:type:ShaderForge.SFN_Vector1,id:2546,x:33084,y:33120,varname:node_2546,prsc:2,v1:0.95;n:type:ShaderForge.SFN_Clamp01,id:8349,x:33690,y:32984,varname:node_8349,prsc:2|IN-7706-OUT;n:type:ShaderForge.SFN_Multiply,id:4633,x:34270,y:32628,varname:node_4633,prsc:2|A-1747-OUT,B-446-OUT,C-1572-RGB;n:type:ShaderForge.SFN_Slider,id:446,x:33741,y:32517,ptovrint:False,ptlb:em,ptin:_em,varname:node_446,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:155;n:type:ShaderForge.SFN_Clamp01,id:6291,x:33169,y:32741,varname:node_6291,prsc:2|IN-2905-OUT;n:type:ShaderForge.SFN_Slider,id:39,x:31146,y:33530,ptovrint:False,ptlb:panX,ptin:_panX,varname:node_39,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:0,max:2;n:type:ShaderForge.SFN_Tex2d,id:3816,x:32384,y:32478,ptovrint:False,ptlb:shape,ptin:_shape,varname:node_3816,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:9616,x:31759,y:32439,varname:node_9616,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:346,x:32127,y:32429,varname:node_346,prsc:2|A-4676-OUT,B-9616-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:3402,x:31671,y:32094,varname:node_3402,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Multiply,id:4676,x:31908,y:32207,varname:node_4676,prsc:2|A-3402-U,B-6441-OUT;n:type:ShaderForge.SFN_Vector2,id:6441,x:31644,y:32275,varname:node_6441,prsc:2,v1:3,v2:7;n:type:ShaderForge.SFN_Subtract,id:1071,x:33366,y:32741,varname:node_1071,prsc:2|A-6291-OUT,B-7638-V;n:type:ShaderForge.SFN_Clamp01,id:2128,x:33566,y:32741,varname:node_2128,prsc:2|IN-1071-OUT;n:type:ShaderForge.SFN_TexCoord,id:7638,x:33028,y:32505,varname:node_7638,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_VertexColor,id:1572,x:34080,y:32343,varname:node_1572,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4467,x:34303,y:32887,varname:node_4467,prsc:2|A-1747-OUT,B-1237-OUT;n:type:ShaderForge.SFN_Slider,id:1237,x:33876,y:32950,ptovrint:False,ptlb:op,ptin:_op,varname:node_1237,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_DepthBlend,id:7900,x:34364,y:33284,varname:node_7900,prsc:2|DIST-337-OUT;n:type:ShaderForge.SFN_Slider,id:337,x:33972,y:33258,ptovrint:False,ptlb:distance,ptin:_distance,varname:node_337,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:3;n:type:ShaderForge.SFN_Multiply,id:6969,x:34575,y:32923,varname:node_6969,prsc:2|A-4467-OUT,B-7900-OUT;n:type:ShaderForge.SFN_ObjectPosition,id:5239,x:34767,y:33289,varname:node_5239,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:6280,x:34779,y:33456,varname:node_6280,prsc:2;n:type:ShaderForge.SFN_Distance,id:6526,x:34980,y:33391,varname:node_6526,prsc:2|A-5239-XYZ,B-6280-XYZ;n:type:ShaderForge.SFN_Multiply,id:8713,x:34848,y:32838,varname:node_8713,prsc:2|A-6969-OUT,B-1572-A;proporder:6374-6904-4233-241-5109-1132-7839-446-39-3816-1237-337;pass:END;sub:END;*/

Shader "Colony_FX/Unique/S_Artifact_Emanation" {
    Properties {
        _mainShapePow ("mainShapePow", Range(0, 25)) = 7.905983
        _mainShapeOffset ("mainShapeOffset", Range(-2, 2)) = 2
        _pattern ("pattern", 2D) = "white" {}
        _panY ("panY", Range(-2, 2)) = 0
        _patternPow ("patternPow", Range(0, 15)) = 0
        _patternSoftness ("patternSoftness", Range(0, 1)) = 0
        _node_7839 ("node_7839", Range(0, 155)) = 4.058856
        _em ("em", Range(0, 155)) = 0
        _panX ("panX", Range(-2, 2)) = 0
        _shape ("shape", 2D) = "white" {}
        _op ("op", Range(0, 1)) = 0
        _distance ("distance", Range(0, 3)) = 0
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
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float _mainShapePow;
            uniform float _mainShapeOffset;
            uniform sampler2D _pattern; uniform float4 _pattern_ST;
            uniform float _panY;
            uniform float _patternSoftness;
            uniform float _patternPow;
            uniform float _node_7839;
            uniform float _em;
            uniform float _panX;
            uniform sampler2D _shape; uniform float4 _shape_ST;
            uniform float _op;
            uniform float _distance;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float4 _shape_var = tex2D(_shape,TRANSFORM_TEX(i.uv0, _shape));
                float4 node_2588 = _Time;
                float2 node_5738 = (i.uv0+(node_2588.g*float2(_panX,_panY))+((i.uv1.r*float2(3,7))+i.uv0));
                float4 _pattern_var = tex2D(_pattern,TRANSFORM_TEX(node_5738, _pattern));
                float node_1747 = (saturate((saturate((_shape_var.r-lerp(0.0,(pow(_pattern_var.r,_patternPow)/_patternSoftness),(1.0 - pow(saturate((1.0 - (i.uv0.g+_mainShapeOffset))),_mainShapePow)))))-i.uv1.g))*saturate(pow((i.uv0.g+0.95),_node_7839)));
                float3 emissive = (node_1747*_em*i.vertexColor.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor,(((node_1747*_op)*saturate((sceneZ-partZ)/_distance))*i.vertexColor.a));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
