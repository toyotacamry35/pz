// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33544,y:32647,varname:node_3138,prsc:2|emission-5994-OUT;n:type:ShaderForge.SFN_Tex2d,id:665,x:31831,y:32825,ptovrint:False,ptlb:mainTex,ptin:_mainTex,varname:node_665,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6171-OUT;n:type:ShaderForge.SFN_Multiply,id:3423,x:32507,y:32759,varname:node_3423,prsc:2|A-6717-OUT,B-9980-OUT;n:type:ShaderForge.SFN_Slider,id:6717,x:32429,y:32579,ptovrint:False,ptlb:emissiveIntensity,ptin:_emissiveIntensity,varname:node_6717,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:33;n:type:ShaderForge.SFN_TexCoord,id:4058,x:31114,y:32803,varname:node_4058,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:8794,x:31409,y:32722,varname:node_8794,prsc:2|A-774-OUT,B-4058-U;n:type:ShaderForge.SFN_Time,id:5425,x:30458,y:32496,varname:node_5425,prsc:2;n:type:ShaderForge.SFN_Sin,id:8889,x:30794,y:32546,varname:node_8889,prsc:2|IN-620-OUT;n:type:ShaderForge.SFN_Add,id:2463,x:31005,y:32525,varname:node_2463,prsc:2|A-2007-OUT,B-8889-OUT;n:type:ShaderForge.SFN_Vector1,id:2007,x:30823,y:32453,varname:node_2007,prsc:2,v1:2;n:type:ShaderForge.SFN_Append,id:6171,x:31613,y:32809,varname:node_6171,prsc:2|A-8794-OUT,B-4058-V;n:type:ShaderForge.SFN_Multiply,id:774,x:31240,y:32506,varname:node_774,prsc:2|A-4556-OUT,B-2463-OUT;n:type:ShaderForge.SFN_Slider,id:4556,x:30936,y:32410,ptovrint:False,ptlb:amp,ptin:_amp,varname:node_4556,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:15;n:type:ShaderForge.SFN_Multiply,id:620,x:30635,y:32556,varname:node_620,prsc:2|A-5425-TTR,B-231-OUT;n:type:ShaderForge.SFN_Slider,id:231,x:30365,y:32685,ptovrint:False,ptlb:testerFreq,ptin:_testerFreq,varname:node_231,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:20;n:type:ShaderForge.SFN_Time,id:1628,x:30864,y:31837,varname:node_1628,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5061,x:31185,y:31861,varname:node_5061,prsc:2|A-1628-T,B-7610-OUT;n:type:ShaderForge.SFN_Multiply,id:3068,x:31185,y:32002,varname:node_3068,prsc:2|A-1628-T,B-494-OUT;n:type:ShaderForge.SFN_Slider,id:7610,x:30829,y:32012,ptovrint:False,ptlb:panX,ptin:_panX,varname:node_7610,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:494,x:30829,y:32097,ptovrint:False,ptlb:panY,ptin:_panY,varname:_panX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_TexCoord,id:134,x:31185,y:31700,varname:node_134,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:2454,x:31401,y:31700,varname:node_2454,prsc:2|A-9316-OUT,B-134-UVOUT;n:type:ShaderForge.SFN_Slider,id:9316,x:31028,y:31545,ptovrint:False,ptlb:noiseTile,ptin:_noiseTile,varname:node_9316,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Append,id:8787,x:31401,y:31930,varname:node_8787,prsc:2|A-5061-OUT,B-3068-OUT;n:type:ShaderForge.SFN_Add,id:1451,x:31606,y:31868,varname:node_1451,prsc:2|A-2454-OUT,B-8787-OUT;n:type:ShaderForge.SFN_Tex2d,id:6326,x:31955,y:31872,ptovrint:False,ptlb:noiseTex,ptin:_noiseTex,varname:node_6326,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1451-OUT;n:type:ShaderForge.SFN_Multiply,id:9980,x:32305,y:32779,varname:node_9980,prsc:2|A-7606-OUT,B-665-RGB,C-3286-RGB,D-7286-A;n:type:ShaderForge.SFN_Multiply,id:7606,x:32165,y:31872,varname:node_7606,prsc:2|A-9552-OUT,B-6326-R;n:type:ShaderForge.SFN_Slider,id:9552,x:32034,y:31769,ptovrint:False,ptlb:noiseIntensity,ptin:_noiseIntensity,varname:node_9552,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Color,id:3286,x:32070,y:32900,ptovrint:False,ptlb:col,ptin:_col,varname:node_3286,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:6272,x:32527,y:33010,ptovrint:False,ptlb:normalTex,ptin:_normalTex,varname:node_6272,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-1451-OUT;n:type:ShaderForge.SFN_ComponentMask,id:207,x:32739,y:33010,varname:node_207,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6272-RGB;n:type:ShaderForge.SFN_Slider,id:1544,x:32830,y:33206,ptovrint:False,ptlb:refIntensity,ptin:_refIntensity,varname:node_1544,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Multiply,id:726,x:33135,y:33004,varname:node_726,prsc:2|A-9561-OUT,B-1544-OUT;n:type:ShaderForge.SFN_ComponentMask,id:1409,x:32910,y:32806,varname:node_1409,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-3423-OUT;n:type:ShaderForge.SFN_Lerp,id:9561,x:32974,y:33004,varname:node_9561,prsc:2|A-6062-OUT,B-207-OUT,T-1409-OUT;n:type:ShaderForge.SFN_Vector1,id:6062,x:32797,y:32957,varname:node_6062,prsc:2,v1:0;n:type:ShaderForge.SFN_TexCoord,id:8076,x:31779,y:32400,varname:node_8076,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_VertexColor,id:7286,x:32070,y:33073,varname:node_7286,prsc:2;n:type:ShaderForge.SFN_Fresnel,id:8026,x:33120,y:32569,varname:node_8026,prsc:2|EXP-2091-OUT;n:type:ShaderForge.SFN_Multiply,id:5994,x:33317,y:32754,varname:node_5994,prsc:2|A-3423-OUT,B-6242-OUT;n:type:ShaderForge.SFN_Vector1,id:2091,x:32930,y:32569,varname:node_2091,prsc:2,v1:3;n:type:ShaderForge.SFN_OneMinus,id:2687,x:33302,y:32569,varname:node_2687,prsc:2|IN-8026-OUT;n:type:ShaderForge.SFN_Power,id:6242,x:33503,y:32509,varname:node_6242,prsc:2|VAL-2687-OUT,EXP-3544-OUT;n:type:ShaderForge.SFN_Vector1,id:3544,x:33315,y:32493,varname:node_3544,prsc:2,v1:3;proporder:665-6717-4556-231-7610-494-9316-6326-9552-3286-6272-1544;pass:END;sub:END;*/

Shader "Colony_FX/S_SlowDownFX" {
    Properties {
        _mainTex ("mainTex", 2D) = "white" {}
        _emissiveIntensity ("emissiveIntensity", Range(0, 33)) = 0
        _amp ("amp", Range(0, 15)) = 0
        _testerFreq ("testerFreq", Range(0, 20)) = 1
        _panX ("panX", Range(-5, 5)) = 0
        _panY ("panY", Range(-5, 5)) = 0
        _noiseTile ("noiseTile", Range(0, 10)) = 1
        _noiseTex ("noiseTex", 2D) = "white" {}
        _noiseIntensity ("noiseIntensity", Range(0, 5)) = 1
        _col ("col", Color) = (0.5,0.5,0.5,1)
        _normalTex ("normalTex", 2D) = "bump" {}
        _refIntensity ("refIntensity", Range(0, 10)) = 1
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
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _mainTex; uniform float4 _mainTex_ST;
            uniform float _emissiveIntensity;
            uniform float _amp;
            uniform float _testerFreq;
            uniform float _panX;
            uniform float _panY;
            uniform float _noiseTile;
            uniform sampler2D _noiseTex; uniform float4 _noiseTex_ST;
            uniform float _noiseIntensity;
            uniform float4 _col;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float4 node_1628 = _Time;
                float2 node_1451 = ((_noiseTile*i.uv0)+float2((node_1628.g*_panX),(node_1628.g*_panY)));
                float4 _noiseTex_var = tex2D(_noiseTex,TRANSFORM_TEX(node_1451, _noiseTex));
                float4 node_5425 = _Time;
                float2 node_6171 = float2(((_amp*(2.0+sin((node_5425.a*_testerFreq))))*i.uv0.r),i.uv0.g);
                float4 _mainTex_var = tex2D(_mainTex,TRANSFORM_TEX(node_6171, _mainTex));
                float3 emissive = ((_emissiveIntensity*((_noiseIntensity*_noiseTex_var.r)*_mainTex_var.rgb*_col.rgb*i.vertexColor.a))*pow((1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),3.0)),3.0));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
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
