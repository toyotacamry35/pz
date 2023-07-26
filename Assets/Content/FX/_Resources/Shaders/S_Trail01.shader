// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33544,y:32647,varname:node_3138,prsc:2|emission-3423-OUT,alpha-1409-OUT;n:type:ShaderForge.SFN_Tex2d,id:665,x:31831,y:32825,ptovrint:False,ptlb:mainTex,ptin:_mainTex,varname:node_665,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6171-OUT;n:type:ShaderForge.SFN_Multiply,id:3423,x:32507,y:32759,varname:node_3423,prsc:2|A-6717-OUT,B-9980-OUT;n:type:ShaderForge.SFN_Slider,id:6717,x:32429,y:32579,ptovrint:False,ptlb:emissiveIntensity,ptin:_emissiveIntensity,varname:node_6717,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:33;n:type:ShaderForge.SFN_TexCoord,id:4058,x:31133,y:32842,varname:node_4058,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:8794,x:31321,y:32712,varname:node_8794,prsc:2|A-8076-U,B-4058-U;n:type:ShaderForge.SFN_Append,id:6171,x:31561,y:32863,varname:node_6171,prsc:2|A-8794-OUT,B-4058-V;n:type:ShaderForge.SFN_TexCoord,id:134,x:31087,y:31684,varname:node_134,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:2454,x:31319,y:31668,varname:node_2454,prsc:2|A-9316-OUT,B-134-UVOUT;n:type:ShaderForge.SFN_Slider,id:9316,x:31028,y:31545,ptovrint:False,ptlb:noiseTile,ptin:_noiseTile,varname:node_9316,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Append,id:8787,x:31429,y:31937,varname:node_8787,prsc:2|A-8076-V,B-9569-OUT;n:type:ShaderForge.SFN_Add,id:1451,x:31621,y:31842,varname:node_1451,prsc:2|A-2454-OUT,B-8787-OUT;n:type:ShaderForge.SFN_Tex2d,id:6326,x:31955,y:31872,ptovrint:False,ptlb:noiseTex,ptin:_noiseTex,varname:node_6326,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1451-OUT;n:type:ShaderForge.SFN_Multiply,id:9980,x:32305,y:32779,varname:node_9980,prsc:2|A-9782-OUT,B-665-RGB,C-3286-RGB,D-7286-A;n:type:ShaderForge.SFN_Multiply,id:7606,x:32191,y:31872,varname:node_7606,prsc:2|A-9552-OUT,B-6326-R;n:type:ShaderForge.SFN_Slider,id:9552,x:32034,y:31769,ptovrint:False,ptlb:noiseIntensity,ptin:_noiseIntensity,varname:node_9552,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Color,id:3286,x:32070,y:32900,ptovrint:False,ptlb:col,ptin:_col,varname:node_3286,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:6272,x:32527,y:33010,ptovrint:False,ptlb:normalTex,ptin:_normalTex,varname:node_6272,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-1451-OUT;n:type:ShaderForge.SFN_ComponentMask,id:207,x:32736,y:33017,varname:node_207,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6272-RGB;n:type:ShaderForge.SFN_Slider,id:1544,x:33125,y:33219,ptovrint:False,ptlb:refIntensity,ptin:_refIntensity,varname:node_1544,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Multiply,id:726,x:33241,y:33005,varname:node_726,prsc:2|A-9561-OUT,B-1544-OUT;n:type:ShaderForge.SFN_ComponentMask,id:1409,x:32910,y:32806,varname:node_1409,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-3423-OUT;n:type:ShaderForge.SFN_Lerp,id:9561,x:32959,y:33004,varname:node_9561,prsc:2|A-6062-OUT,B-207-OUT,T-1409-OUT;n:type:ShaderForge.SFN_Vector1,id:6062,x:32716,y:32933,varname:node_6062,prsc:2,v1:0;n:type:ShaderForge.SFN_TexCoord,id:8076,x:30805,y:32320,varname:node_8076,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_VertexColor,id:7286,x:32070,y:33073,varname:node_7286,prsc:2;n:type:ShaderForge.SFN_Power,id:9782,x:32467,y:31881,varname:node_9782,prsc:2|VAL-7606-OUT,EXP-4596-OUT;n:type:ShaderForge.SFN_Slider,id:4596,x:32359,y:31771,ptovrint:False,ptlb:noiseContrast,ptin:_noiseContrast,varname:node_4596,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Vector1,id:9569,x:31219,y:31922,varname:node_9569,prsc:2,v1:0;proporder:665-6717-9316-6326-9552-3286-6272-1544-4596;pass:END;sub:END;*/

Shader "Colony_FX/S_Trail02" {
    Properties {
        _mainTex ("mainTex", 2D) = "white" {}
        _emissiveIntensity ("emissiveIntensity", Range(0, 33)) = 0
        _noiseTile ("noiseTile", Range(0, 10)) = 1
        _noiseTex ("noiseTex", 2D) = "white" {}
        _noiseIntensity ("noiseIntensity", Range(0, 5)) = 1
        _col ("col", Color) = (0.5,0.5,0.5,1)
        _normalTex ("normalTex", 2D) = "bump" {}
        _refIntensity ("refIntensity", Range(0, 10)) = 1
        _noiseContrast ("noiseContrast", Range(0, 10)) = 1
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
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _mainTex; uniform float4 _mainTex_ST;
            uniform float _emissiveIntensity;
            uniform float _noiseTile;
            uniform sampler2D _noiseTex; uniform float4 _noiseTex_ST;
            uniform float _noiseIntensity;
            uniform float4 _col;
            uniform float _noiseContrast;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float2 node_1451 = ((_noiseTile*i.uv0)+float2(i.uv1.g,0.0));
                float4 _noiseTex_var = tex2D(_noiseTex,TRANSFORM_TEX(node_1451, _noiseTex));
                float2 node_6171 = float2((i.uv1.r*i.uv0.r),i.uv0.g);
                float4 _mainTex_var = tex2D(_mainTex,TRANSFORM_TEX(node_6171, _mainTex));
                float3 node_3423 = (_emissiveIntensity*(pow((_noiseIntensity*_noiseTex_var.r),_noiseContrast)*_mainTex_var.rgb*_col.rgb*i.vertexColor.a));
                float3 emissive = node_3423;
                float3 finalColor = emissive;
                float node_1409 = node_3423.r;
                return fixed4(finalColor,node_1409);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
