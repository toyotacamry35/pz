// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.278808,fgcg:0.4464591,fgcb:0.5178908,fgca:1,fgde:0.005101306,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33473,y:33561,varname:node_2865,prsc:2|diff-8952-OUT,spec-7269-OUT,gloss-4157-OUT,normal-7678-OUT,emission-4713-OUT,transm-7455-OUT,lwrap-7455-OUT,alpha-8886-OUT;n:type:ShaderForge.SFN_Vector1,id:7269,x:33147,y:33522,varname:node_7269,prsc:2,v1:0;n:type:ShaderForge.SFN_TexCoord,id:3630,x:29473,y:34619,varname:node_3630,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Length,id:1276,x:29827,y:34619,varname:node_1276,prsc:2|IN-9553-OUT;n:type:ShaderForge.SFN_RemapRange,id:9553,x:29647,y:34619,varname:node_9553,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-3630-UVOUT;n:type:ShaderForge.SFN_OneMinus,id:9855,x:30270,y:34622,varname:node_9855,prsc:2|IN-2682-OUT;n:type:ShaderForge.SFN_Power,id:2131,x:30467,y:34622,varname:node_2131,prsc:2|VAL-9855-OUT,EXP-5576-OUT;n:type:ShaderForge.SFN_Vector1,id:5576,x:30097,y:34838,varname:node_5576,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Time,id:4972,x:29938,y:33313,varname:node_4972,prsc:2;n:type:ShaderForge.SFN_Slider,id:9325,x:29723,y:33578,ptovrint:False,ptlb:panX,ptin:_panX,varname:node_9325,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0.1,max:5;n:type:ShaderForge.SFN_Slider,id:5304,x:29723,y:33714,ptovrint:False,ptlb:panY,ptin:_panY,varname:node_5304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:-0.15,max:5;n:type:ShaderForge.SFN_Append,id:6238,x:30075,y:33629,varname:node_6238,prsc:2|A-9325-OUT,B-5304-OUT;n:type:ShaderForge.SFN_Multiply,id:2174,x:30292,y:33483,varname:node_2174,prsc:2|A-4972-T,B-6238-OUT;n:type:ShaderForge.SFN_Frac,id:6089,x:30537,y:33483,varname:node_6089,prsc:2|IN-2174-OUT;n:type:ShaderForge.SFN_TexCoord,id:7316,x:29942,y:33996,varname:node_7316,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:4170,x:30243,y:34039,varname:node_4170,prsc:2|A-7316-UVOUT,B-3012-OUT;n:type:ShaderForge.SFN_Slider,id:3012,x:29827,y:34171,ptovrint:False,ptlb:tile,ptin:_tile,varname:node_3012,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.352174,max:10;n:type:ShaderForge.SFN_Add,id:8598,x:30833,y:33816,varname:node_8598,prsc:2|A-6089-OUT,B-4170-OUT,C-1309-OUT;n:type:ShaderForge.SFN_Tex2d,id:2377,x:31168,y:34146,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_2377,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c3cfb6d2ac44d4d49bc0b0462aab159c,ntxv:0,isnm:False|UVIN-8598-OUT;n:type:ShaderForge.SFN_Set,id:9441,x:30869,y:34622,varname:mask,prsc:2|IN-3013-OUT;n:type:ShaderForge.SFN_Multiply,id:5428,x:32377,y:34148,varname:node_5428,prsc:2|A-2075-A,B-8671-OUT,C-3750-R;n:type:ShaderForge.SFN_Add,id:1325,x:31741,y:34168,varname:node_1325,prsc:2|A-5287-OUT,B-2010-OUT;n:type:ShaderForge.SFN_Clamp01,id:3013,x:30669,y:34622,varname:node_3013,prsc:2|IN-2131-OUT;n:type:ShaderForge.SFN_VertexColor,id:2075,x:31522,y:33555,varname:node_2075,prsc:2;n:type:ShaderForge.SFN_Vector1,id:7455,x:33224,y:33692,varname:node_7455,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Clamp01,id:2682,x:30042,y:34622,varname:node_2682,prsc:2|IN-1276-OUT;n:type:ShaderForge.SFN_Vector2,id:9221,x:30231,y:33802,varname:node_9221,prsc:2,v1:3,v2:8;n:type:ShaderForge.SFN_Multiply,id:1309,x:30439,y:33853,varname:node_1309,prsc:2|A-9221-OUT,B-9798-U;n:type:ShaderForge.SFN_TexCoord,id:9798,x:29911,y:33846,cmnt:Dynamic parameters,varname:node_9798,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Slider,id:2010,x:31584,y:34359,ptovrint:False,ptlb:addToNoise,ptin:_addToNoise,varname:node_2010,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Tex2d,id:3750,x:32117,y:34521,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_3750,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8342,x:31316,y:34817,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_8342,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-8598-OUT;n:type:ShaderForge.SFN_Subtract,id:3584,x:31974,y:34168,varname:node_3584,prsc:2|A-1325-OUT,B-9798-V;n:type:ShaderForge.SFN_Clamp01,id:8671,x:32148,y:34168,varname:node_8671,prsc:2|IN-3584-OUT;n:type:ShaderForge.SFN_Multiply,id:5287,x:31494,y:34168,varname:node_5287,prsc:2|A-2377-R,B-7540-OUT;n:type:ShaderForge.SFN_Slider,id:7540,x:31278,y:34371,ptovrint:False,ptlb:mult,ptin:_mult,varname:node_7540,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5,max:5;n:type:ShaderForge.SFN_Smoothstep,id:2605,x:32657,y:33319,varname:node_2605,prsc:2|A-3855-OUT,B-7557-OUT,V-2377-R;n:type:ShaderForge.SFN_Vector1,id:1980,x:32160,y:33298,varname:node_1980,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Subtract,id:3855,x:32360,y:33298,varname:node_3855,prsc:2|A-1980-OUT,B-6935-OUT;n:type:ShaderForge.SFN_Slider,id:6935,x:32003,y:33412,ptovrint:False,ptlb:colorRange,ptin:_colorRange,varname:node_6935,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:2;n:type:ShaderForge.SFN_Add,id:7557,x:32360,y:33469,varname:node_7557,prsc:2|A-6935-OUT,B-1980-OUT;n:type:ShaderForge.SFN_Multiply,id:8952,x:32838,y:33131,varname:node_8952,prsc:2|A-5031-RGB,B-2075-RGB,C-2605-OUT;n:type:ShaderForge.SFN_Color,id:5031,x:32576,y:33037,ptovrint:False,ptlb:baseColor,ptin:_baseColor,varname:node_5031,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.6764706,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Lerp,id:504,x:32887,y:34794,varname:node_504,prsc:2|A-8188-OUT,B-8342-RGB,T-5428-OUT;n:type:ShaderForge.SFN_Vector3,id:8188,x:32621,y:34730,varname:node_8188,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_ValueProperty,id:4956,x:29495,y:33704,ptovrint:False,ptlb:_PanDirection,ptin:_PanDirection,varname:node_4956,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_SwitchProperty,id:4713,x:32946,y:33682,ptovrint:False,ptlb:useEmissive,ptin:_useEmissive,varname:node_4713,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8830-OUT,B-8029-OUT;n:type:ShaderForge.SFN_Multiply,id:8029,x:32734,y:33718,varname:node_8029,prsc:2|A-8952-OUT,B-5054-OUT;n:type:ShaderForge.SFN_Slider,id:5054,x:32398,y:33781,ptovrint:False,ptlb:emissive,ptin:_emissive,varname:node_5054,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Vector1,id:8830,x:32766,y:33647,varname:node_8830,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:8886,x:32848,y:34093,varname:node_8886,prsc:2|A-5428-OUT,B-8235-OUT;n:type:ShaderForge.SFN_DepthBlend,id:8235,x:32640,y:34266,varname:node_8235,prsc:2|DIST-8051-OUT;n:type:ShaderForge.SFN_Slider,id:8051,x:32308,y:34331,ptovrint:False,ptlb:distance,ptin:_distance,varname:node_8051,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:12;n:type:ShaderForge.SFN_Slider,id:4157,x:33137,y:33608,ptovrint:False,ptlb:gloss,ptin:_gloss,varname:node_4157,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6,max:1;n:type:ShaderForge.SFN_Multiply,id:7678,x:33133,y:34794,varname:node_7678,prsc:2|A-8342-RGB,B-8310-OUT;n:type:ShaderForge.SFN_Slider,id:4371,x:32686,y:35024,ptovrint:False,ptlb:normalIntensity,ptin:_normalIntensity,varname:node_4371,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:12;n:type:ShaderForge.SFN_Append,id:8310,x:33020,y:35004,varname:node_8310,prsc:2|A-4371-OUT,B-4371-OUT,C-7199-OUT;n:type:ShaderForge.SFN_Vector1,id:7199,x:32789,y:35122,varname:node_7199,prsc:2,v1:1;proporder:9325-5304-3012-2377-2010-3750-8342-7540-6935-5031-4713-5054-8051-4157-4371;pass:END;sub:END;*/

Shader "Colony_FX/Basic/S_AlphaBlend_NormalMap_Lit" {
    Properties {
        _panX ("panX", Range(-5, 5)) = 0.1
        _panY ("panY", Range(-5, 5)) = -0.15
        _tile ("tile", Range(0, 10)) = 1.352174
        _MainTex ("MainTex", 2D) = "white" {}
        _addToNoise ("addToNoise", Range(0, 2)) = 0
        _Mask ("Mask", 2D) = "white" {}
        _Normal ("Normal", 2D) = "bump" {}
        _mult ("mult", Range(0, 5)) = 5
        _colorRange ("colorRange", Range(0, 2)) = 2
        _baseColor ("baseColor", Color) = (0.6764706,0,0,1)
        [MaterialToggle] _useEmissive ("useEmissive", Float ) = 0
        _emissive ("emissive", Range(0, 15)) = 1
        _distance ("distance", Range(0, 12)) = 0
        _gloss ("gloss", Range(0, 1)) = 0.6
        _normalIntensity ("normalIntensity", Range(0, 12)) = 1
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
			#define DS_HAZE_FULL
			
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
			#include "Assets/ASkyLighting/DeepSky Haze/Resources/DS_TransparentLib.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float _panX;
            uniform float _panY;
            uniform float _tile;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _addToNoise;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _mult;
            uniform float _colorRange;
            uniform float4 _baseColor;
            uniform fixed _useEmissive;
            uniform float _emissive;
            uniform float _distance;
            uniform float _gloss;
            uniform float _normalIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
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
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD7;
                UNITY_FOG_COORDS(8)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD9;
                #endif
				float3 air : TEXCOORD10;
				float3 hazeAndFog : TEXCOORD11;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
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
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_4972 = _Time;
                float2 node_8598 = (frac((node_4972.g*float2(_panX,_panY)))+(i.uv0*_tile)+(float2(3,8)*i.uv1.r));
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_8598, _Normal)));
                float3 normalLocal = (_Normal_var.rgb*float3(_normalIntensity,_normalIntensity,1.0));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
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
                float gloss = _gloss;
                float perceptualRoughness = 1.0 - _gloss;
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
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
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
                float node_1980 = 0.5;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_8598, _MainTex));
                float3 node_8952 = (_baseColor.rgb*i.vertexColor.rgb*smoothstep( (node_1980-_colorRange), (_colorRange+node_1980), _MainTex_var.r ));
                float3 diffuseColor = node_8952; // Need this for specular when using metallic
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
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float node_7455 = 0.8;
                float3 w = float3(node_7455,node_7455,node_7455)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_7455,node_7455,node_7455);
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotLWrap);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((forwardLight+backLight) + ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL)) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = lerp( 0.0, (node_8952*_emissive), _useEmissive );
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float node_5428 = (i.vertexColor.a*saturate((((_MainTex_var.r*_mult)+_addToNoise)-i.uv1.g))*_Mask_var.r);
                fixed4 finalRGBA = fixed4(finalColor,(node_5428*saturate((sceneZ-partZ)/_distance)));
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, unity_FogColor);
				DS_Haze_Apply(i.air, i.hazeAndFog, finalRGBA, finalRGBA.a);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Assets/Alloy/AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float _panX;
            uniform float _panY;
            uniform float _tile;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _addToNoise;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _mult;
            uniform float _colorRange;
            uniform float4 _baseColor;
            uniform fixed _useEmissive;
            uniform float _emissive;
            uniform float _distance;
            uniform float _gloss;
            uniform float _normalIntensity;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
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
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_4972 = _Time;
                float2 node_8598 = (frac((node_4972.g*float2(_panX,_panY)))+(i.uv0*_tile)+(float2(3,8)*i.uv1.r));
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_8598, _Normal)));
                float3 normalLocal = (_Normal_var.rgb*float3(_normalIntensity,_normalIntensity,1.0));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _gloss;
                float perceptualRoughness = 1.0 - _gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = 0.0;
                float specularMonochrome;
                float node_1980 = 0.5;
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_8598, _MainTex));
                float3 node_8952 = (_baseColor.rgb*i.vertexColor.rgb*smoothstep( (node_1980-_colorRange), (_colorRange+node_1980), _MainTex_var.r ));
                float3 diffuseColor = node_8952; // Need this for specular when using metallic
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
                float node_7455 = 0.8;
                float3 w = float3(node_7455,node_7455,node_7455)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_7455,node_7455,node_7455);
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotLWrap);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((forwardLight+backLight) + ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL)) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float node_5428 = (i.vertexColor.a*saturate((((_MainTex_var.r*_mult)+_addToNoise)-i.uv1.g))*_Mask_var.r);
                fixed4 finalRGBA = fixed4(finalColor * (node_5428*saturate((sceneZ-partZ)/_distance)),0);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, unity_FogColor);
                return finalRGBA;
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _panX;
            uniform float _panY;
            uniform float _tile;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _colorRange;
            uniform float4 _baseColor;
            uniform fixed _useEmissive;
            uniform float _emissive;
            uniform float _gloss;
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
                float4 posWorld : TEXCOORD3;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : SV_Target {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float node_1980 = 0.5;
                float4 node_4972 = _Time;
                float2 node_8598 = (frac((node_4972.g*float2(_panX,_panY)))+(i.uv0*_tile)+(float2(3,8)*i.uv1.r));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_8598, _MainTex));
                float3 node_8952 = (_baseColor.rgb*i.vertexColor.rgb*smoothstep( (node_1980-_colorRange), (_colorRange+node_1980), _MainTex_var.r ));
                o.Emission = lerp( 0.0, (node_8952*_emissive), _useEmissive );
                
                float3 diffColor = node_8952;
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, 0.0, specColor, specularMonochrome );
                float roughness = 1.0 - _gloss;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
