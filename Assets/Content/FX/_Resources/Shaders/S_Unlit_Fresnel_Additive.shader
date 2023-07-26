// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.2796003,fgcg:0.4513296,fgcb:0.5206271,fgca:1,fgde:0.005100179,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32917,y:32705,varname:node_3138,prsc:2|emission-5970-OUT;n:type:ShaderForge.SFN_Fresnel,id:2674,x:32154,y:32843,varname:node_2674,prsc:2|EXP-9528-OUT;n:type:ShaderForge.SFN_Slider,id:9528,x:31797,y:32881,ptovrint:False,ptlb:fresnelExponent,ptin:_fresnelExponent,varname:node_9528,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3.333333,max:5;n:type:ShaderForge.SFN_Multiply,id:3400,x:32440,y:32800,varname:node_3400,prsc:2|A-4896-RGB,B-2674-OUT;n:type:ShaderForge.SFN_Color,id:4896,x:32154,y:32682,ptovrint:False,ptlb:Tint,ptin:_Tint,varname:node_4896,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.8708369,c3:0.5990566,c4:1;n:type:ShaderForge.SFN_Multiply,id:5970,x:32706,y:32788,varname:node_5970,prsc:2|A-3730-OUT,B-3400-OUT;n:type:ShaderForge.SFN_Slider,id:3730,x:32328,y:32638,ptovrint:False,ptlb:brightness,ptin:_brightness,varname:node_3730,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3,max:25;proporder:9528-4896-3730;pass:END;sub:END;*/

Shader "Colony_FX/S_Unlit_Fresnel_Additive" {
    Properties {
        _fresnelExponent ("fresnelExponent", Range(0, 5)) = 3.333333
        _Tint ("Tint", Color) = (1,0.8708369,0.5990566,1)
        _brightness ("brightness", Range(0, 25)) = 3
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 4.0
            uniform float _fresnelExponent;
            uniform float4 _Tint;
            uniform float _brightness;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
				float3 air : TEXCOORD2;
				float3 hazeAndFog : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
				DS_Haze_Per_Vertex(v.vertex, o.air, o.hazeAndFog);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float3 emissive = (_brightness*(_Tint.rgb*pow(1.0-max(0,dot(normalDirection, viewDirection)),_fresnelExponent)));
                float3 finalColor = emissive;
				fixed4 finalRGBA = fixed4(finalColor,1);
				DS_Haze_Apply(i.air, i.hazeAndFog, finalRGBA, finalRGBA.a);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
