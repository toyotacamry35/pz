// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:34821,y:32743,varname:node_3138,prsc:2|emission-5785-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:33066,y:32617,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:9679,x:31405,y:32913,ptovrint:False,ptlb:Tex,ptin:_Tex,varname:node_9679,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5533-OUT;n:type:ShaderForge.SFN_TexCoord,id:9015,x:30539,y:32791,varname:node_9015,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:814,x:30478,y:33296,varname:node_814,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7753,x:30832,y:33314,varname:node_7753,prsc:2|A-814-T,B-5780-OUT;n:type:ShaderForge.SFN_Slider,id:196,x:30241,y:33508,ptovrint:False,ptlb:panX,ptin:_panX,varname:node_196,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:2388,x:30241,y:33614,ptovrint:False,ptlb:panY,ptin:_panY,varname:node_2388,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Append,id:5780,x:30631,y:33552,varname:node_5780,prsc:2|A-196-OUT,B-2388-OUT;n:type:ShaderForge.SFN_Multiply,id:3995,x:30788,y:32913,varname:node_3995,prsc:2|A-9015-UVOUT,B-2262-OUT;n:type:ShaderForge.SFN_Slider,id:8872,x:30143,y:32977,ptovrint:False,ptlb:tileX,ptin:_tileX,varname:node_8872,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Slider,id:9556,x:30143,y:33068,ptovrint:False,ptlb:tileY,ptin:_tileY,varname:node_9556,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Append,id:2262,x:30539,y:33012,varname:node_2262,prsc:2|A-8872-OUT,B-9556-OUT;n:type:ShaderForge.SFN_Add,id:5533,x:31097,y:32912,varname:node_5533,prsc:2|A-3995-OUT,B-7753-OUT;n:type:ShaderForge.SFN_Slider,id:2832,x:32929,y:32811,ptovrint:False,ptlb:emissive,ptin:_emissive,varname:node_2832,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1000;n:type:ShaderForge.SFN_Multiply,id:5785,x:33638,y:32748,varname:node_5785,prsc:2|A-7241-RGB,B-2832-OUT,C-806-OUT;n:type:ShaderForge.SFN_Power,id:8826,x:31856,y:32932,varname:node_8826,prsc:2|VAL-9679-R,EXP-9073-OUT;n:type:ShaderForge.SFN_Slider,id:9073,x:31474,y:33149,ptovrint:False,ptlb:pow,ptin:_pow,varname:node_9073,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Fresnel,id:778,x:32205,y:33109,varname:node_778,prsc:2|EXP-3848-OUT;n:type:ShaderForge.SFN_Slider,id:3848,x:31869,y:33181,ptovrint:False,ptlb:fresnel,ptin:_fresnel,varname:node_3848,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.393168,max:10;n:type:ShaderForge.SFN_OneMinus,id:9711,x:32401,y:33109,varname:node_9711,prsc:2|IN-778-OUT;n:type:ShaderForge.SFN_Multiply,id:2867,x:32326,y:32934,varname:node_2867,prsc:2|A-8826-OUT,B-9711-OUT;n:type:ShaderForge.SFN_Multiply,id:806,x:33862,y:33294,varname:node_806,prsc:2|A-2867-OUT,B-252-A,C-5289-OUT,D-9910-OUT,E-6055-OUT;n:type:ShaderForge.SFN_DepthBlend,id:5289,x:32865,y:33383,varname:node_5289,prsc:2|DIST-2666-OUT;n:type:ShaderForge.SFN_Slider,id:2666,x:32458,y:33492,ptovrint:False,ptlb:depthBlend,ptin:_depthBlend,varname:node_2666,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3721778,max:3;n:type:ShaderForge.SFN_ViewPosition,id:1319,x:32051,y:34082,varname:node_1319,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:1125,x:32051,y:33929,varname:node_1125,prsc:2;n:type:ShaderForge.SFN_Distance,id:3242,x:32292,y:34022,varname:node_3242,prsc:2|A-1125-XYZ,B-1319-XYZ;n:type:ShaderForge.SFN_Subtract,id:9609,x:32537,y:34022,varname:node_9609,prsc:2|A-3242-OUT,B-3547-OUT;n:type:ShaderForge.SFN_Slider,id:2368,x:32029,y:34298,ptovrint:False,ptlb:fadeDistance,ptin:_fadeDistance,varname:node_2368,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:50,max:150;n:type:ShaderForge.SFN_Divide,id:1143,x:33129,y:34028,varname:node_1143,prsc:2|A-6395-OUT,B-5456-OUT;n:type:ShaderForge.SFN_Multiply,id:496,x:32729,y:34022,varname:node_496,prsc:2|A-9609-OUT,B-9609-OUT;n:type:ShaderForge.SFN_Negate,id:6395,x:32902,y:34022,varname:node_6395,prsc:2|IN-496-OUT;n:type:ShaderForge.SFN_Add,id:3427,x:33346,y:34028,varname:node_3427,prsc:2|A-1143-OUT,B-4546-OUT;n:type:ShaderForge.SFN_Vector1,id:4546,x:33181,y:34203,varname:node_4546,prsc:2,v1:1;n:type:ShaderForge.SFN_Clamp01,id:9910,x:33568,y:34028,varname:node_9910,prsc:2|IN-3427-OUT;n:type:ShaderForge.SFN_Divide,id:3547,x:32382,y:34172,varname:node_3547,prsc:2|A-2368-OUT,B-3296-OUT;n:type:ShaderForge.SFN_Vector1,id:3296,x:32204,y:34210,varname:node_3296,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:5456,x:32691,y:34272,varname:node_5456,prsc:2|A-2368-OUT,B-7343-OUT;n:type:ShaderForge.SFN_Vector1,id:7343,x:32454,y:34374,varname:node_7343,prsc:2,v1:60;n:type:ShaderForge.SFN_VertexColor,id:252,x:32885,y:33213,varname:node_252,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:5842,x:33641,y:33504,varname:node_5842,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:2960,x:34059,y:33595,varname:node_2960,prsc:2|A-5842-V,B-1046-OUT;n:type:ShaderForge.SFN_Vector1,id:1046,x:33826,y:33699,varname:node_1046,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Vector1,id:4056,x:33970,y:32634,varname:node_4056,prsc:2,v1:1;n:type:ShaderForge.SFN_Power,id:6055,x:34687,y:33672,varname:node_6055,prsc:2|VAL-4016-OUT,EXP-6532-OUT;n:type:ShaderForge.SFN_Vector1,id:6532,x:34395,y:33842,varname:node_6532,prsc:2,v1:2;n:type:ShaderForge.SFN_OneMinus,id:9471,x:34246,y:33626,varname:node_9471,prsc:2|IN-2960-OUT;n:type:ShaderForge.SFN_Clamp01,id:4016,x:34419,y:33661,varname:node_4016,prsc:2|IN-9471-OUT;proporder:7241-2832-9679-196-2388-8872-9556-9073-3848-2666-2368;pass:END;sub:END;*/

Shader "Shader Forge/S_LootMesh" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _emissive ("emissive", Range(0, 1000)) = 0
        _Tex ("Tex", 2D) = "white" {}
        _panX ("panX", Range(-1, 1)) = 0
        _panY ("panY", Range(-1, 1)) = 0
        _tileX ("tileX", Range(0, 10)) = 0
        _tileY ("tileY", Range(0, 10)) = 0
        _pow ("pow", Range(0, 5)) = 1
        _fresnel ("fresnel", Range(0, 10)) = 2.393168
        _depthBlend ("depthBlend", Range(0, 3)) = 0.3721778
        _fadeDistance ("fadeDistance", Range(0, 150)) = 50
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
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _Color;
            uniform sampler2D _Tex; uniform float4 _Tex_ST;
            uniform float _panX;
            uniform float _panY;
            uniform float _tileX;
            uniform float _tileY;
            uniform float _emissive;
            uniform float _pow;
            uniform float _fresnel;
            uniform float _depthBlend;
            uniform float _fadeDistance;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float4 node_814 = _Time;
                float2 node_5533 = ((i.uv0*float2(_tileX,_tileY))+(node_814.g*float2(_panX,_panY)));
                float4 _Tex_var = tex2D(_Tex,TRANSFORM_TEX(node_5533, _Tex));
                float node_9609 = (distance(i.posWorld.rgb,_WorldSpaceCameraPos)-(_fadeDistance/2.0));
                float node_6055 = pow(saturate((1.0 - (i.uv0.g+0.1))),2.0);
                float3 emissive = (_Color.rgb*_emissive*((pow(_Tex_var.r,_pow)*(1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_fresnel)))*i.vertexColor.a*saturate((sceneZ-partZ)/_depthBlend)*saturate((((-1*(node_9609*node_9609))/(_fadeDistance*60.0))+1.0))*node_6055));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
