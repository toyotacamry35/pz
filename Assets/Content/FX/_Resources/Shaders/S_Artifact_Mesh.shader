// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:1,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:37249,y:33113,varname:node_3138,prsc:2|diff-6377-RGB,spec-5058-OUT,gloss-2432-OUT,normal-8764-RGB,emission-1477-OUT,difocc-9087-R;n:type:ShaderForge.SFN_Fresnel,id:4146,x:32151,y:33111,varname:node_4146,prsc:2|EXP-2031-OUT;n:type:ShaderForge.SFN_Slider,id:2031,x:31811,y:33117,ptovrint:False,ptlb:fresnelPow,ptin:_fresnelPow,varname:node_2031,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.591459,max:15;n:type:ShaderForge.SFN_Multiply,id:3032,x:35520,y:33393,varname:node_3032,prsc:2|A-241-OUT,B-4542-RGB;n:type:ShaderForge.SFN_Color,id:4542,x:34940,y:33662,ptovrint:False,ptlb:emCol,ptin:_emCol,varname:node_4542,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:9369,x:32703,y:33118,varname:node_9369,prsc:2|A-4146-OUT,B-386-OUT;n:type:ShaderForge.SFN_Slider,id:386,x:32383,y:33302,ptovrint:False,ptlb:em,ptin:_em,varname:node_386,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:15,max:155;n:type:ShaderForge.SFN_Multiply,id:4414,x:33270,y:33125,varname:node_4414,prsc:2|A-9369-OUT,B-5501-OUT;n:type:ShaderForge.SFN_Sin,id:5762,x:32809,y:33500,varname:node_5762,prsc:2|IN-6069-OUT;n:type:ShaderForge.SFN_Time,id:7235,x:32419,y:33459,varname:node_7235,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:5501,x:33136,y:33456,varname:node_5501,prsc:2,frmn:-1,frmx:1,tomn:0.05,tomx:0.3|IN-5762-OUT;n:type:ShaderForge.SFN_Tex2d,id:1381,x:34807,y:32571,ptovrint:False,ptlb:metalness,ptin:_metalness,varname:node_1381,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8552,x:34644,y:32860,ptovrint:False,ptlb:roughness,ptin:_roughness,varname:node_8552,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9087,x:36976,y:34031,ptovrint:False,ptlb:ao,ptin:_ao,varname:node_9087,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8764,x:35581,y:33117,ptovrint:False,ptlb:nm,ptin:_nm,varname:node_8764,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:6377,x:34950,y:32361,ptovrint:False,ptlb:diffuse,ptin:_diffuse,varname:node_6377,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:5058,x:35066,y:32716,varname:node_5058,prsc:2|A-1381-R,B-1939-OUT;n:type:ShaderForge.SFN_Slider,id:1939,x:34728,y:32781,ptovrint:False,ptlb:met,ptin:_met,varname:node_1939,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:9059,x:34644,y:33059,ptovrint:False,ptlb:rough,ptin:_rough,varname:node_9059,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:2432,x:35003,y:32958,varname:node_2432,prsc:2|A-8552-R,B-9059-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:9818,x:32039,y:34057,varname:node_9818,prsc:2;n:type:ShaderForge.SFN_Time,id:5687,x:31928,y:34453,varname:node_5687,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9678,x:32212,y:34566,varname:node_9678,prsc:2|A-5687-T,B-4780-OUT;n:type:ShaderForge.SFN_Slider,id:4780,x:31831,y:34678,ptovrint:False,ptlb:speed,ptin:_speed,varname:node_4780,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:-1.03115,max:2;n:type:ShaderForge.SFN_Divide,id:3709,x:32884,y:34230,varname:node_3709,prsc:2|A-7193-OUT,B-4233-OUT;n:type:ShaderForge.SFN_Slider,id:4233,x:32617,y:34433,ptovrint:False,ptlb:scale,ptin:_scale,varname:node_4233,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.410891,max:4;n:type:ShaderForge.SFN_Power,id:6432,x:33243,y:34234,varname:node_6432,prsc:2|VAL-5140-OUT,EXP-681-OUT;n:type:ShaderForge.SFN_Slider,id:681,x:32941,y:34532,ptovrint:False,ptlb:bottomEdgeHardness,ptin:_bottomEdgeHardness,varname:node_681,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.36,max:15;n:type:ShaderForge.SFN_OneMinus,id:4425,x:33412,y:34081,varname:node_4425,prsc:2|IN-6432-OUT;n:type:ShaderForge.SFN_Multiply,id:5592,x:34034,y:34266,varname:node_5592,prsc:2|A-1830-OUT,B-6432-OUT;n:type:ShaderForge.SFN_Add,id:7193,x:32676,y:34230,varname:node_7193,prsc:2|A-1534-OUT,B-5910-OUT;n:type:ShaderForge.SFN_Tan,id:1534,x:32377,y:34566,varname:node_1534,prsc:2|IN-9678-OUT;n:type:ShaderForge.SFN_Power,id:1830,x:33810,y:34081,varname:node_1830,prsc:2|VAL-6345-OUT,EXP-4173-OUT;n:type:ShaderForge.SFN_Slider,id:4173,x:33430,y:34334,ptovrint:False,ptlb:topEdgeHardness,ptin:_topEdgeHardness,varname:node_4173,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3.5,max:4;n:type:ShaderForge.SFN_Clamp01,id:6345,x:33601,y:34081,varname:node_6345,prsc:2|IN-4425-OUT;n:type:ShaderForge.SFN_ObjectPosition,id:6938,x:32039,y:34214,varname:node_6938,prsc:2;n:type:ShaderForge.SFN_Subtract,id:5910,x:32279,y:34200,varname:node_5910,prsc:2|A-9818-Y,B-6938-Y;n:type:ShaderForge.SFN_Multiply,id:3190,x:34517,y:34280,varname:node_3190,prsc:2|A-5592-OUT,B-2115-OUT;n:type:ShaderForge.SFN_Clamp01,id:5140,x:33060,y:34234,varname:node_5140,prsc:2|IN-3709-OUT;n:type:ShaderForge.SFN_Slider,id:2115,x:34017,y:34508,ptovrint:False,ptlb:bandEm,ptin:_bandEm,varname:node_2115,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:455;n:type:ShaderForge.SFN_Add,id:241,x:35028,y:33349,varname:node_241,prsc:2|A-4414-OUT,B-8381-OUT;n:type:ShaderForge.SFN_Multiply,id:8381,x:34522,y:33445,varname:node_8381,prsc:2|A-4146-OUT,B-3190-OUT;n:type:ShaderForge.SFN_Add,id:2005,x:36048,y:33460,varname:node_2005,prsc:2|A-3032-OUT,B-2971-OUT;n:type:ShaderForge.SFN_Tex2d,id:1500,x:35524,y:33998,ptovrint:False,ptlb:coreEmMask,ptin:_coreEmMask,varname:node_1500,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2971,x:35810,y:34115,varname:node_2971,prsc:2|A-1500-R,B-4542-RGB,C-9708-OUT,D-4498-OUT,E-7468-OUT;n:type:ShaderForge.SFN_Slider,id:9708,x:35278,y:34202,ptovrint:False,ptlb:coreEm,ptin:_coreEm,varname:node_9708,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:150;n:type:ShaderForge.SFN_Tex2d,id:8567,x:35266,y:34568,ptovrint:False,ptlb:coreEmPattern,ptin:_coreEmPattern,varname:node_8567,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7387-UVOUT;n:type:ShaderForge.SFN_Power,id:4498,x:35558,y:34645,varname:node_4498,prsc:2|VAL-8567-R,EXP-2054-OUT;n:type:ShaderForge.SFN_Slider,id:2054,x:35245,y:34774,ptovrint:False,ptlb:coreEmPatternPow,ptin:_coreEmPatternPow,varname:node_2054,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Panner,id:7387,x:34969,y:34602,varname:node_7387,prsc:2,spu:0.01,spv:0.4|UVIN-2236-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:2236,x:34727,y:34608,varname:node_2236,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Fresnel,id:9217,x:35349,y:34350,varname:node_9217,prsc:2|EXP-9595-OUT;n:type:ShaderForge.SFN_Slider,id:9595,x:35032,y:34381,ptovrint:False,ptlb:coreFresnelPow,ptin:_coreFresnelPow,varname:node_9595,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.500244,max:5;n:type:ShaderForge.SFN_OneMinus,id:7468,x:35566,y:34382,varname:node_7468,prsc:2|IN-9217-OUT;n:type:ShaderForge.SFN_Add,id:1477,x:36831,y:33469,varname:node_1477,prsc:2|A-2005-OUT,B-6327-OUT;n:type:ShaderForge.SFN_Slider,id:3834,x:36192,y:33584,ptovrint:False,ptlb:uniformEm,ptin:_uniformEm,varname:node_3834,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:155;n:type:ShaderForge.SFN_Multiply,id:6327,x:36576,y:33679,varname:node_6327,prsc:2|A-3834-OUT,B-4542-RGB,C-3415-OUT,D-1243-OUT;n:type:ShaderForge.SFN_RemapRange,id:3415,x:33177,y:33641,varname:node_3415,prsc:2,frmn:-1,frmx:1,tomn:0.2,tomx:1|IN-5762-OUT;n:type:ShaderForge.SFN_Dot,id:1243,x:35873,y:33227,varname:node_1243,prsc:2,dt:0|A-8764-RGB,B-4406-OUT;n:type:ShaderForge.SFN_Vector3,id:4406,x:35617,y:33279,varname:node_4406,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Multiply,id:6069,x:32660,y:33529,varname:node_6069,prsc:2|A-7235-TDB,B-1032-OUT;n:type:ShaderForge.SFN_Slider,id:1032,x:32362,y:33661,ptovrint:False,ptlb:uniformEmModulationFreq,ptin:_uniformEmModulationFreq,varname:node_1032,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;proporder:2031-386-4542-1381-8552-9087-8764-6377-1939-9059-4780-4233-681-4173-2115-1500-9708-8567-2054-9595-3834-1032;pass:END;sub:END;*/

Shader "Colony_FX/Unique/S_Artifact_Mesh" {
    Properties {
        _fresnelPow ("fresnelPow", Range(0, 15)) = 2.591459
        _em ("em", Range(0, 155)) = 15
        _emCol ("emCol", Color) = (0.5,0.5,0.5,1)
        _metalness ("metalness", 2D) = "white" {}
        _roughness ("roughness", 2D) = "bump" {}
        _ao ("ao", 2D) = "white" {}
        _nm ("nm", 2D) = "bump" {}
        _diffuse ("diffuse", 2D) = "white" {}
        _met ("met", Range(0, 1)) = 0
        _rough ("rough", Range(0, 1)) = 0
        _speed ("speed", Range(-2, 2)) = -1.03115
        _scale ("scale", Range(0, 4)) = 2.410891
        _bottomEdgeHardness ("bottomEdgeHardness", Range(0, 15)) = 4.36
        _topEdgeHardness ("topEdgeHardness", Range(0, 4)) = 3.5
        _bandEm ("bandEm", Range(0, 455)) = 1
        _coreEmMask ("coreEmMask", 2D) = "white" {}
        _coreEm ("coreEm", Range(0, 150)) = 0
        _coreEmPattern ("coreEmPattern", 2D) = "white" {}
        _coreEmPatternPow ("coreEmPatternPow", Range(0, 5)) = 0
        _coreFresnelPow ("coreFresnelPow", Range(0, 5)) = 1.500244
        _uniformEm ("uniformEm", Range(0, 155)) = 0
        _uniformEmModulationFreq ("uniformEmModulationFreq", Range(0, 5)) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "Assets/Alloy/AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _fresnelPow;
            uniform float4 _emCol;
            uniform float _em;
            uniform sampler2D _metalness; uniform float4 _metalness_ST;
            uniform sampler2D _roughness; uniform float4 _roughness_ST;
            uniform sampler2D _ao; uniform float4 _ao_ST;
            uniform sampler2D _nm; uniform float4 _nm_ST;
            uniform sampler2D _diffuse; uniform float4 _diffuse_ST;
            uniform float _met;
            uniform float _rough;
            uniform float _speed;
            uniform float _scale;
            uniform float _bottomEdgeHardness;
            uniform float _topEdgeHardness;
            uniform float _bandEm;
            uniform sampler2D _coreEmMask; uniform float4 _coreEmMask_ST;
            uniform float _coreEm;
            uniform sampler2D _coreEmPattern; uniform float4 _coreEmPattern_ST;
            uniform float _coreEmPatternPow;
            uniform float _coreFresnelPow;
            uniform float _uniformEm;
            uniform float _uniformEmModulationFreq;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD9;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
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
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _nm_var = UnpackNormal(tex2D(_nm,TRANSFORM_TEX(i.uv0, _nm)));
                float3 normalLocal = _nm_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _roughness_var = tex2D(_roughness,TRANSFORM_TEX(i.uv0, _roughness));
                float gloss = 1.0 - (_roughness_var.r*_rough); // Convert roughness to gloss
                float perceptualRoughness = (_roughness_var.r*_rough);
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
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float4 _metalness_var = tex2D(_metalness,TRANSFORM_TEX(i.uv0, _metalness));
                float3 specularColor = (_metalness_var.r*_met);
                float specularMonochrome;
                float4 _diffuse_var = tex2D(_diffuse,TRANSFORM_TEX(i.uv0, _diffuse));
                float3 diffuseColor = _diffuse_var.rgb; // Need this for specular when using metallic
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
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float4 _ao_var = tex2D(_ao,TRANSFORM_TEX(i.uv0, _ao));
                indirectDiffuse *= _ao_var.r; // Diffuse AO
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float node_4146 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_fresnelPow);
                float4 node_7235 = _Time;
                float node_5762 = sin((node_7235.b*_uniformEmModulationFreq));
                float4 node_5687 = _Time;
                float node_6432 = pow(saturate(((tan((node_5687.g*_speed))+(i.posWorld.g-objPos.g))/_scale)),_bottomEdgeHardness);
                float4 _coreEmMask_var = tex2D(_coreEmMask,TRANSFORM_TEX(i.uv0, _coreEmMask));
                float4 node_5329 = _Time;
                float2 node_7387 = (i.uv0+node_5329.g*float2(0.01,0.4));
                float4 _coreEmPattern_var = tex2D(_coreEmPattern,TRANSFORM_TEX(node_7387, _coreEmPattern));
                float3 emissive = ((((((node_4146*_em)*(node_5762*0.125+0.175))+(node_4146*((pow(saturate((1.0 - node_6432)),_topEdgeHardness)*node_6432)*_bandEm)))*_emCol.rgb)+(_coreEmMask_var.r*_emCol.rgb*_coreEm*pow(_coreEmPattern_var.r,_coreEmPatternPow)*(1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_coreFresnelPow))))+(_uniformEm*_emCol.rgb*(node_5762*0.4+0.6)*dot(_nm_var.rgb,float3(0,0,1))));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #include "UnityCG.cginc"
            #include "Assets/Alloy/AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _fresnelPow;
            uniform float4 _emCol;
            uniform float _em;
            uniform sampler2D _metalness; uniform float4 _metalness_ST;
            uniform sampler2D _roughness; uniform float4 _roughness_ST;
            uniform sampler2D _nm; uniform float4 _nm_ST;
            uniform sampler2D _diffuse; uniform float4 _diffuse_ST;
            uniform float _met;
            uniform float _rough;
            uniform float _speed;
            uniform float _scale;
            uniform float _bottomEdgeHardness;
            uniform float _topEdgeHardness;
            uniform float _bandEm;
            uniform sampler2D _coreEmMask; uniform float4 _coreEmMask_ST;
            uniform float _coreEm;
            uniform sampler2D _coreEmPattern; uniform float4 _coreEmPattern_ST;
            uniform float _coreEmPatternPow;
            uniform float _coreFresnelPow;
            uniform float _uniformEm;
            uniform float _uniformEmModulationFreq;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                LIGHTING_COORDS(7,8)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _nm_var = UnpackNormal(tex2D(_nm,TRANSFORM_TEX(i.uv0, _nm)));
                float3 normalLocal = _nm_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _roughness_var = tex2D(_roughness,TRANSFORM_TEX(i.uv0, _roughness));
                float gloss = 1.0 - (_roughness_var.r*_rough); // Convert roughness to gloss
                float perceptualRoughness = (_roughness_var.r*_rough);
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float4 _metalness_var = tex2D(_metalness,TRANSFORM_TEX(i.uv0, _metalness));
                float3 specularColor = (_metalness_var.r*_met);
                float specularMonochrome;
                float4 _diffuse_var = tex2D(_diffuse,TRANSFORM_TEX(i.uv0, _diffuse));
                float3 diffuseColor = _diffuse_var.rgb; // Need this for specular when using metallic
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
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor * 1,0);
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _fresnelPow;
            uniform float4 _emCol;
            uniform float _em;
            uniform sampler2D _metalness; uniform float4 _metalness_ST;
            uniform sampler2D _roughness; uniform float4 _roughness_ST;
            uniform sampler2D _nm; uniform float4 _nm_ST;
            uniform sampler2D _diffuse; uniform float4 _diffuse_ST;
            uniform float _met;
            uniform float _rough;
            uniform float _speed;
            uniform float _scale;
            uniform float _bottomEdgeHardness;
            uniform float _topEdgeHardness;
            uniform float _bandEm;
            uniform sampler2D _coreEmMask; uniform float4 _coreEmMask_ST;
            uniform float _coreEm;
            uniform sampler2D _coreEmPattern; uniform float4 _coreEmPattern_ST;
            uniform float _coreEmPatternPow;
            uniform float _coreFresnelPow;
            uniform float _uniformEm;
            uniform float _uniformEmModulationFreq;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float node_4146 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_fresnelPow);
                float4 node_7235 = _Time;
                float node_5762 = sin((node_7235.b*_uniformEmModulationFreq));
                float4 node_5687 = _Time;
                float node_6432 = pow(saturate(((tan((node_5687.g*_speed))+(i.posWorld.g-objPos.g))/_scale)),_bottomEdgeHardness);
                float4 _coreEmMask_var = tex2D(_coreEmMask,TRANSFORM_TEX(i.uv0, _coreEmMask));
                float4 node_7549 = _Time;
                float2 node_7387 = (i.uv0+node_7549.g*float2(0.01,0.4));
                float4 _coreEmPattern_var = tex2D(_coreEmPattern,TRANSFORM_TEX(node_7387, _coreEmPattern));
                float3 _nm_var = UnpackNormal(tex2D(_nm,TRANSFORM_TEX(i.uv0, _nm)));
                o.Emission = ((((((node_4146*_em)*(node_5762*0.125+0.175))+(node_4146*((pow(saturate((1.0 - node_6432)),_topEdgeHardness)*node_6432)*_bandEm)))*_emCol.rgb)+(_coreEmMask_var.r*_emCol.rgb*_coreEm*pow(_coreEmPattern_var.r,_coreEmPatternPow)*(1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_coreFresnelPow))))+(_uniformEm*_emCol.rgb*(node_5762*0.4+0.6)*dot(_nm_var.rgb,float3(0,0,1))));
                
                float4 _diffuse_var = tex2D(_diffuse,TRANSFORM_TEX(i.uv0, _diffuse));
                float3 diffColor = _diffuse_var.rgb;
                float specularMonochrome;
                float3 specColor;
                float4 _metalness_var = tex2D(_metalness,TRANSFORM_TEX(i.uv0, _metalness));
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, (_metalness_var.r*_met), specColor, specularMonochrome );
                float4 _roughness_var = tex2D(_roughness,TRANSFORM_TEX(i.uv0, _roughness));
                float roughness = (_roughness_var.r*_rough);
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
