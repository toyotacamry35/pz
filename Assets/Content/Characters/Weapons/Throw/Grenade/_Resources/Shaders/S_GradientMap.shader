// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33129,y:32736,varname:node_3138,prsc:2|diff-70-OUT,spec-9455-OUT,emission-8090-OUT,transm-7988-OUT,lwrap-7988-OUT,alpha-5781-OUT;n:type:ShaderForge.SFN_Tex2d,id:6075,x:31161,y:32894,ptovrint:False,ptlb:flipbook,ptin:_flipbook,varname:node_6075,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:48a06df57aa439e4a8a35049a571a95a,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:6812,x:31968,y:32536,ptovrint:False,ptlb:blackBody,ptin:_blackBody,varname:node_6812,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e33dc32508955c94fb2bb9b5009cff77,ntxv:0,isnm:False|UVIN-838-OUT;n:type:ShaderForge.SFN_Append,id:838,x:31775,y:32536,varname:node_838,prsc:2|A-5248-OUT,B-5248-OUT;n:type:ShaderForge.SFN_Smoothstep,id:5033,x:31402,y:32688,varname:node_5033,prsc:2|A-4733-U,B-4117-OUT,V-6075-R;n:type:ShaderForge.SFN_VertexColor,id:5981,x:31886,y:32863,varname:node_5981,prsc:2;n:type:ShaderForge.SFN_Multiply,id:70,x:32217,y:32754,varname:node_70,prsc:2|A-6075-RGB,B-5981-RGB;n:type:ShaderForge.SFN_Multiply,id:1250,x:32092,y:32999,varname:node_1250,prsc:2|A-5981-A,B-6075-A;n:type:ShaderForge.SFN_Vector1,id:9455,x:32505,y:32598,varname:node_9455,prsc:2,v1:0;n:type:ShaderForge.SFN_Add,id:4117,x:31124,y:32687,varname:node_4117,prsc:2|A-4733-U,B-4777-OUT;n:type:ShaderForge.SFN_TexCoord,id:4733,x:30856,y:32473,varname:node_4733,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Slider,id:4777,x:30744,y:32751,ptovrint:False,ptlb:maskSoftness,ptin:_maskSoftness,varname:node_4777,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6,max:1;n:type:ShaderForge.SFN_Multiply,id:5248,x:31551,y:32546,varname:node_5248,prsc:2|A-2871-OUT,B-5033-OUT;n:type:ShaderForge.SFN_Slider,id:2871,x:31055,y:32398,ptovrint:False,ptlb:temperature,ptin:_temperature,varname:node_2871,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Multiply,id:8090,x:32254,y:32578,varname:node_8090,prsc:2|A-6812-RGB,B-4733-V;n:type:ShaderForge.SFN_Vector1,id:6579,x:32569,y:32713,varname:node_6579,prsc:2,v1:0;n:type:ShaderForge.SFN_Subtract,id:5914,x:32339,y:32999,varname:node_5914,prsc:2|A-1250-OUT,B-4733-Z;n:type:ShaderForge.SFN_Clamp01,id:7073,x:32534,y:32999,varname:node_7073,prsc:2|IN-5914-OUT;n:type:ShaderForge.SFN_Multiply,id:4829,x:32752,y:32999,varname:node_4829,prsc:2|A-7073-OUT,B-8244-OUT;n:type:ShaderForge.SFN_DepthBlend,id:8244,x:32586,y:33182,varname:node_8244,prsc:2|DIST-3463-OUT;n:type:ShaderForge.SFN_Slider,id:3463,x:32187,y:33273,ptovrint:False,ptlb:distance,ptin:_distance,varname:node_3463,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:3;n:type:ShaderForge.SFN_Power,id:5781,x:32947,y:33021,varname:node_5781,prsc:2|VAL-4829-OUT,EXP-9470-OUT;n:type:ShaderForge.SFN_Slider,id:9470,x:32654,y:33255,ptovrint:False,ptlb:opacityPow,ptin:_opacityPow,varname:node_9470,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:11;n:type:ShaderForge.SFN_Vector1,id:7988,x:32917,y:32887,varname:node_7988,prsc:2,v1:1;proporder:6075-6812-4777-2871-3463-9470;pass:END;sub:END;*/

Shader "Shader Forge/S_GradientMap" {
    Properties {
        _flipbook ("flipbook", 2D) = "white" {}
        _blackBody ("blackBody", 2D) = "white" {}
        _maskSoftness ("maskSoftness", Range(0, 1)) = 0.6
        _temperature ("temperature", Range(0, 2)) = 1
        _distance ("distance", Range(0, 3)) = 0
        _opacityPow ("opacityPow", Range(0, 11)) = 1
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
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _flipbook; uniform float4 _flipbook_ST;
            uniform sampler2D _blackBody; uniform float4 _blackBody_ST;
            uniform float _maskSoftness;
            uniform float _temperature;
            uniform float _distance;
            uniform float _opacityPow;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = 0.5;
                float perceptualRoughness = 1.0 - 0.5;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = 0.0;
                float specularMonochrome;
                float4 _flipbook_var = tex2D(_flipbook,TRANSFORM_TEX(i.uv0, _flipbook));
                float3 diffuseColor = (_flipbook_var.rgb*i.vertexColor.rgb); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float node_7988 = 1.0;
                float3 w = float3(node_7988,node_7988,node_7988)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_7988,node_7988,node_7988);
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotLWrap);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((forwardLight+backLight) + ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL)) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float node_5248 = (_temperature*smoothstep( i.uv1.r, (i.uv1.r+_maskSoftness), _flipbook_var.r ));
                float2 node_838 = float2(node_5248,node_5248);
                float4 _blackBody_var = tex2D(_blackBody,TRANSFORM_TEX(node_838, _blackBody));
                float3 emissive = (_blackBody_var.rgb*i.uv1.g);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                return fixed4(finalColor,pow((saturate(((i.vertexColor.a*_flipbook_var.a)-i.uv1.b))*saturate((sceneZ-partZ)/_distance)),_opacityPow));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
