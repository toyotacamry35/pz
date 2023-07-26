// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:False,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.7404731,fgcg:0.557321,fgcb:0.3975021,fgca:1,fgde:0.004234665,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:34029,y:32724,varname:node_2865,prsc:2|diff-7576-OUT,spec-7343-OUT,gloss-8665-OUT,normal-9169-OUT,emission-5028-OUT,transm-5376-OUT,lwrap-5376-OUT,alpha-2638-OUT,voffset-1230-OUT;n:type:ShaderForge.SFN_Tex2d,id:9245,x:30259,y:32724,ptovrint:False,ptlb:D,ptin:_D,varname:_D,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8545-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:7347,x:32244,y:33392,ptovrint:False,ptlb:N,ptin:_N,varname:_N,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-8545-UVOUT;n:type:ShaderForge.SFN_Multiply,id:7576,x:31891,y:32742,varname:node_7576,prsc:2|A-1743-OUT,B-9245-R;n:type:ShaderForge.SFN_Slider,id:8665,x:32213,y:32853,ptovrint:False,ptlb:roughness,ptin:_roughness,varname:_roughness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Subtract,id:5817,x:31378,y:32940,varname:node_5817,prsc:2|A-9245-R,B-6957-U;n:type:ShaderForge.SFN_Divide,id:8721,x:32185,y:32970,varname:node_8721,prsc:2|A-5817-OUT,B-7730-OUT;n:type:ShaderForge.SFN_Slider,id:7730,x:31895,y:33229,ptovrint:False,ptlb:softness,ptin:_softness,varname:_softness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_TexCoord,id:6957,x:29117,y:32700,varname:node_6957,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_VertexColor,id:2233,x:30301,y:32073,varname:node_2233,prsc:2;n:type:ShaderForge.SFN_Clamp01,id:4425,x:32442,y:32972,varname:node_4425,prsc:2|IN-8721-OUT;n:type:ShaderForge.SFN_Multiply,id:9169,x:32524,y:33348,varname:node_9169,prsc:2|A-7347-RGB,B-7867-OUT;n:type:ShaderForge.SFN_Slider,id:6509,x:32150,y:33628,ptovrint:False,ptlb:nmInt,ptin:_nmInt,varname:_nmInt,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:13;n:type:ShaderForge.SFN_Append,id:8536,x:32504,y:33598,varname:node_8536,prsc:2|A-6509-OUT,B-6509-OUT;n:type:ShaderForge.SFN_Append,id:7867,x:32687,y:33598,varname:node_7867,prsc:2|A-8536-OUT,B-4712-OUT;n:type:ShaderForge.SFN_Vector1,id:4712,x:32545,y:33756,varname:node_4712,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:5376,x:32802,y:32930,varname:node_5376,prsc:2,v1:1;n:type:ShaderForge.SFN_Rotator,id:8545,x:30149,y:32967,varname:node_8545,prsc:2|UVIN-1055-UVOUT,PIV-1200-OUT,ANG-5195-OUT;n:type:ShaderForge.SFN_TexCoord,id:1055,x:29507,y:32959,varname:node_1055,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector2,id:1200,x:29796,y:32981,varname:node_1200,prsc:2,v1:0.5,v2:0.5;n:type:ShaderForge.SFN_Pi,id:2398,x:29073,y:33226,varname:node_2398,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4118,x:29253,y:33195,varname:node_4118,prsc:2|A-2398-OUT,B-3303-OUT;n:type:ShaderForge.SFN_Vector1,id:3303,x:29054,y:33373,varname:node_3303,prsc:2,v1:2;n:type:ShaderForge.SFN_Vector1,id:7343,x:32513,y:32797,varname:node_7343,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:5028,x:32681,y:32692,varname:node_5028,prsc:2|A-2673-OUT,B-1743-OUT;n:type:ShaderForge.SFN_Slider,id:2673,x:32271,y:32505,ptovrint:False,ptlb:em,ptin:_em,varname:_em,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:155;n:type:ShaderForge.SFN_Desaturate,id:1999,x:30572,y:32235,varname:node_1999,prsc:2|COL-2233-RGB,DES-5448-OUT;n:type:ShaderForge.SFN_Slider,id:5448,x:30156,y:32293,ptovrint:False,ptlb:desaturatedCol,ptin:_desaturatedCol,varname:_desaturatedCol,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Lerp,id:1743,x:31419,y:32077,varname:node_1743,prsc:2|A-2233-RGB,B-8931-OUT,T-8699-OUT;n:type:ShaderForge.SFN_Slider,id:5904,x:30235,y:32389,ptovrint:False,ptlb:desaturatedColBrightness,ptin:_desaturatedColBrightness,varname:_desaturatedColBrightness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:4;n:type:ShaderForge.SFN_Slider,id:4664,x:30452,y:32505,ptovrint:False,ptlb:desaturatedColMaskPow,ptin:_desaturatedColMaskPow,varname:_desaturatedColMaskPow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Slider,id:9612,x:30605,y:32610,ptovrint:False,ptlb:desaturatedColMaskMult,ptin:_desaturatedColMaskMult,varname:_desaturatedColMaskMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:5;n:type:ShaderForge.SFN_Power,id:8876,x:30784,y:32440,varname:node_8876,prsc:2|VAL-5817-OUT,EXP-4664-OUT;n:type:ShaderForge.SFN_Multiply,id:3869,x:30967,y:32440,varname:node_3869,prsc:2|A-9612-OUT,B-8876-OUT;n:type:ShaderForge.SFN_Clamp01,id:8699,x:31141,y:32440,varname:node_8699,prsc:2|IN-3869-OUT;n:type:ShaderForge.SFN_Multiply,id:2638,x:32670,y:33104,varname:node_2638,prsc:2|A-4425-OUT,B-2233-A;n:type:ShaderForge.SFN_Lerp,id:1154,x:29574,y:33545,varname:node_1154,prsc:2|A-1349-OUT,B-6957-Z,T-3729-OUT;n:type:ShaderForge.SFN_Slider,id:3729,x:29173,y:33722,ptovrint:False,ptlb:useShaderRotationRate(Z),ptin:_useShaderRotationRateZ,varname:node_3729,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Vector1,id:1349,x:29319,y:33545,varname:node_1349,prsc:2,v1:0;n:type:ShaderForge.SFN_Add,id:5195,x:29822,y:33189,varname:node_5195,prsc:2|A-6964-OUT,B-1154-OUT;n:type:ShaderForge.SFN_Multiply,id:6964,x:29594,y:33189,varname:node_6964,prsc:2|A-6957-V,B-4118-OUT;n:type:ShaderForge.SFN_ScreenPos,id:8571,x:35271,y:34730,varname:node_8571,prsc:2,sctp:2;n:type:ShaderForge.SFN_Slider,id:1726,x:35515,y:34494,ptovrint:False,ptlb:vignette,ptin:_vignette,varname:node_5729,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Power,id:5705,x:36347,y:34485,varname:node_5705,prsc:2|VAL-1042-OUT,EXP-2596-OUT;n:type:ShaderForge.SFN_Vector1,id:2596,x:36139,y:34586,varname:node_2596,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Smoothstep,id:1169,x:36718,y:34486,varname:node_1169,prsc:2|A-5705-OUT,B-636-OUT,V-7889-OUT;n:type:ShaderForge.SFN_Vector1,id:636,x:36501,y:34568,varname:node_636,prsc:2,v1:1;n:type:ShaderForge.SFN_Add,id:7711,x:35830,y:34848,varname:node_7711,prsc:2|A-8571-UVOUT,B-4007-R;n:type:ShaderForge.SFN_Lerp,id:2269,x:36086,y:34792,varname:node_2269,prsc:2|A-8571-UVOUT,B-7711-OUT,T-563-OUT;n:type:ShaderForge.SFN_Slider,id:563,x:35693,y:35049,ptovrint:False,ptlb:vignetteDist,ptin:_vignetteDist,varname:node_6001,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_RemapRange,id:6567,x:36295,y:34792,varname:node_6567,prsc:2,frmn:0,frmx:1,tomn:-0.7,tomx:0.7|IN-2269-OUT;n:type:ShaderForge.SFN_Length,id:7889,x:36489,y:34792,varname:node_7889,prsc:2|IN-6567-OUT;n:type:ShaderForge.SFN_Tex2d,id:4007,x:35394,y:34936,ptovrint:False,ptlb:vignetteDistorter,ptin:_vignetteDistorter,varname:node_3574,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:3919,x:36793,y:34124,ptovrint:False,ptlb:vignetteColor,ptin:_vignetteColor,varname:node_6864,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:8240,x:37092,y:34216,varname:node_8240,prsc:2|A-3919-A,B-9460-OUT;n:type:ShaderForge.SFN_OneMinus,id:1042,x:35885,y:34497,varname:node_1042,prsc:2|IN-1726-OUT;n:type:ShaderForge.SFN_Clamp01,id:9460,x:36894,y:34486,varname:node_9460,prsc:2|IN-1169-OUT;n:type:ShaderForge.SFN_Multiply,id:5447,x:37311,y:34235,varname:node_5447,prsc:2|A-8240-OUT,B-5677-OUT;n:type:ShaderForge.SFN_Slider,id:5677,x:36715,y:34350,ptovrint:False,ptlb:vignetteLevel,ptin:_vignetteLevel,varname:node_5028,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_TexCoord,id:5634,x:34564,y:34836,varname:node_5634,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:8232,x:34888,y:34900,varname:node_8232,prsc:2|A-5634-UVOUT,B-5563-OUT;n:type:ShaderForge.SFN_Vector2,id:7518,x:34437,y:35005,varname:node_7518,prsc:2,v1:3,v2:7;n:type:ShaderForge.SFN_Multiply,id:5563,x:34646,y:35019,varname:node_5563,prsc:2|A-7518-OUT,B-9289-OUT;n:type:ShaderForge.SFN_Slider,id:9289,x:34280,y:35160,ptovrint:False,ptlb:randomOffset,ptin:_randomOffset,varname:node_5679,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_ComponentMask,id:1922,x:32982,y:33335,varname:node_1922,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9169-OUT;n:type:ShaderForge.SFN_Multiply,id:6186,x:33293,y:33326,varname:node_6186,prsc:2|A-1922-OUT,B-6132-OUT,C-2638-OUT;n:type:ShaderForge.SFN_Slider,id:6132,x:33029,y:33492,ptovrint:False,ptlb:refract,ptin:_refract,varname:node_6132,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:1230,x:33834,y:33184,varname:node_1230,prsc:2|A-1251-OUT,B-6250-OUT;n:type:ShaderForge.SFN_Multiply,id:8931,x:30790,y:32243,varname:node_8931,prsc:2|A-1999-OUT,B-5904-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6250,x:33664,y:33377,ptovrint:False,ptlb:node_6250,ptin:_node_6250,varname:node_6250,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Normalize,id:1251,x:33680,y:33129,varname:node_1251,prsc:2|IN-8336-OUT;n:type:ShaderForge.SFN_NormalVector,id:8336,x:33458,y:33199,prsc:2,pt:False;proporder:9245-7347-8665-7730-6509-2673-5448-5904-4664-9612-3729-6132-6250;pass:END;sub:END;*/

Shader "Shader Forge/S_Liquid_Translucent" {
    Properties {
        _D ("D", 2D) = "white" {}
        _N ("N", 2D) = "bump" {}
        _roughness ("roughness", Range(0, 1)) = 0
        _softness ("softness", Range(0, 1)) = 1
        _nmInt ("nmInt", Range(0, 13)) = 1
        _em ("em", Range(0, 155)) = 0
        _desaturatedCol ("desaturatedCol", Range(0, 2)) = 0
        _desaturatedColBrightness ("desaturatedColBrightness", Range(0, 4)) = 1
        _desaturatedColMaskPow ("desaturatedColMaskPow", Range(0, 15)) = 1
        _desaturatedColMaskMult ("desaturatedColMaskMult", Range(0, 5)) = 2
        _useShaderRotationRateZ ("useShaderRotationRate(Z)", Range(0, 1)) = 0
        _refract ("refract", Range(0, 1)) = 0
        _node_6250 ("node_6250", Float ) = 0
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
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _D; uniform float4 _D_ST;
            uniform sampler2D _N; uniform float4 _N_ST;
            uniform float _roughness;
            uniform float _softness;
            uniform float _nmInt;
            uniform float _em;
            uniform float _desaturatedCol;
            uniform float _desaturatedColBrightness;
            uniform float _desaturatedColMaskPow;
            uniform float _desaturatedColMaskMult;
            uniform float _useShaderRotationRateZ;
            uniform float _node_6250;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
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
                float3 tangentDir : TEXCOORD4;
                float3 bitangentDir : TEXCOORD5;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                v.vertex.xyz += (normalize(v.normal)*_node_6250);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float node_8545_ang = ((i.uv1.g*(3.141592654*2.0))+lerp(0.0,i.uv1.b,_useShaderRotationRateZ));
                float node_8545_spd = 1.0;
                float node_8545_cos = cos(node_8545_spd*node_8545_ang);
                float node_8545_sin = sin(node_8545_spd*node_8545_ang);
                float2 node_8545_piv = float2(0.5,0.5);
                float2 node_8545 = (mul(i.uv0-node_8545_piv,float2x2( node_8545_cos, -node_8545_sin, node_8545_sin, node_8545_cos))+node_8545_piv);
                float3 _N_var = UnpackNormal(tex2D(_N,TRANSFORM_TEX(node_8545, _N)));
                float3 node_9169 = (_N_var.rgb*float3(float2(_nmInt,_nmInt),1.0));
                float3 normalLocal = node_9169;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _roughness;
                float perceptualRoughness = 1.0 - _roughness;
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
                float3 node_1999 = lerp(i.vertexColor.rgb,dot(i.vertexColor.rgb,float3(0.3,0.59,0.11)),_desaturatedCol);
                float4 _D_var = tex2D(_D,TRANSFORM_TEX(node_8545, _D));
                float node_5817 = (_D_var.r-i.uv1.r);
                float3 node_1743 = lerp(i.vertexColor.rgb,(node_1999*_desaturatedColBrightness),saturate((_desaturatedColMaskMult*pow(node_5817,_desaturatedColMaskPow))));
                float3 diffuseColor = (node_1743*_D_var.r); // Need this for specular when using metallic
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
                float node_5376 = 1.0;
                float3 w = float3(node_5376,node_5376,node_5376)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_5376,node_5376,node_5376);
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotLWrap);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((forwardLight+backLight) + ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL)) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
////// Emissive:
                float3 emissive = (_em*node_1743);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                float node_2638 = (saturate((node_5817/_softness))*i.vertexColor.a);
                fixed4 finalRGBA = fixed4(finalColor,node_2638);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
