// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:34161,y:32907,varname:node_3138,prsc:2|emission-2079-OUT,clip-563-OUT,voffset-1902-OUT;n:type:ShaderForge.SFN_Tex2d,id:562,x:31733,y:33115,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_562,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:2079,x:32500,y:32727,varname:node_2079,prsc:2|A-2605-OUT,B-8232-OUT,T-562-R;n:type:ShaderForge.SFN_Multiply,id:1055,x:31582,y:33737,varname:node_1055,prsc:2|A-9071-OUT,B-1200-OUT;n:type:ShaderForge.SFN_Time,id:4054,x:30704,y:33799,varname:node_4054,prsc:2;n:type:ShaderForge.SFN_Sin,id:1200,x:31292,y:33916,varname:node_1200,prsc:2|IN-228-OUT;n:type:ShaderForge.SFN_Multiply,id:228,x:31114,y:33916,varname:node_228,prsc:2|A-5431-OUT,B-4118-OUT;n:type:ShaderForge.SFN_Slider,id:4118,x:30935,y:34144,ptovrint:False,ptlb:freq,ptin:_freq,varname:node_4118,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4,max:45;n:type:ShaderForge.SFN_Multiply,id:3303,x:31888,y:33736,varname:node_3303,prsc:2|A-1055-OUT,B-5330-OUT;n:type:ShaderForge.SFN_Vector3,id:5330,x:31670,y:33919,varname:node_5330,prsc:2,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_Color,id:7878,x:30791,y:32279,ptovrint:False,ptlb:wingCol1,ptin:_wingCol1,varname:node_7878,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_TexCoord,id:3619,x:30712,y:34060,varname:node_3619,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Add,id:5431,x:30922,y:33916,varname:node_5431,prsc:2|A-4054-TTR,B-3619-U;n:type:ShaderForge.SFN_Multiply,id:349,x:32216,y:33734,varname:node_349,prsc:2|A-3303-OUT,B-7296-OUT;n:type:ShaderForge.SFN_Slider,id:7296,x:31985,y:33938,ptovrint:False,ptlb:amplitude,ptin:_amplitude,varname:node_7296,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:8232,x:31203,y:32275,varname:node_8232,prsc:2|A-980-OUT,B-7878-RGB;n:type:ShaderForge.SFN_Slider,id:980,x:30634,y:32155,ptovrint:False,ptlb:wingEmissive1,ptin:_wingEmissive1,varname:node_980,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3,max:10;n:type:ShaderForge.SFN_Lerp,id:2605,x:31692,y:32737,varname:node_2605,prsc:2|A-8412-OUT,B-1236-OUT,T-6126-OUT;n:type:ShaderForge.SFN_Tex2d,id:6186,x:30483,y:33054,ptovrint:False,ptlb:bodyPattern,ptin:_bodyPattern,varname:node_6186,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:6132,x:30556,y:33249,ptovrint:False,ptlb:bodyPatternPower,ptin:_bodyPatternPower,varname:node_6132,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2840222,max:5;n:type:ShaderForge.SFN_Power,id:6126,x:30935,y:33103,varname:node_6126,prsc:2|VAL-6186-R,EXP-6132-OUT;n:type:ShaderForge.SFN_Color,id:7345,x:30512,y:32655,ptovrint:False,ptlb:bodyColor2,ptin:_bodyColor2,varname:node_7345,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:260,x:30496,y:32839,ptovrint:False,ptlb:bodyColor1,ptin:_bodyColor1,varname:_bodyColor2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:8412,x:30957,y:32677,varname:node_8412,prsc:2|A-7345-RGB,B-9237-OUT;n:type:ShaderForge.SFN_Multiply,id:1236,x:30966,y:32860,varname:node_1236,prsc:2|A-260-RGB,B-9553-OUT;n:type:ShaderForge.SFN_Slider,id:9237,x:30177,y:32710,ptovrint:False,ptlb:bodyEmissive2,ptin:_bodyEmissive2,varname:node_9237,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Slider,id:9553,x:30140,y:32928,ptovrint:False,ptlb:bodyEmissive1,ptin:_bodyEmissive1,varname:_bodyEmissive2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:25;n:type:ShaderForge.SFN_VertexColor,id:4714,x:31258,y:33397,varname:node_4714,prsc:2;n:type:ShaderForge.SFN_Power,id:9071,x:31508,y:33508,varname:node_9071,prsc:2|VAL-4714-R,EXP-2233-OUT;n:type:ShaderForge.SFN_Slider,id:2233,x:31149,y:33596,ptovrint:False,ptlb:vertexRedPow,ptin:_vertexRedPow,varname:node_2233,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Time,id:3967,x:30968,y:34537,varname:node_3967,prsc:2;n:type:ShaderForge.SFN_Sin,id:9802,x:31556,y:34654,varname:node_9802,prsc:2|IN-6889-OUT;n:type:ShaderForge.SFN_Multiply,id:6889,x:31378,y:34654,varname:node_6889,prsc:2|A-9000-OUT,B-977-OUT;n:type:ShaderForge.SFN_Slider,id:977,x:31266,y:34888,ptovrint:False,ptlb:freqTail,ptin:_freqTail,varname:_freq_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4,max:45;n:type:ShaderForge.SFN_TexCoord,id:473,x:30976,y:34798,varname:node_473,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Add,id:9000,x:31186,y:34654,varname:node_9000,prsc:2|A-3967-TTR,B-473-U;n:type:ShaderForge.SFN_Multiply,id:891,x:31882,y:34574,varname:node_891,prsc:2|A-9644-OUT,B-9802-OUT;n:type:ShaderForge.SFN_Power,id:9644,x:31633,y:34374,varname:node_9644,prsc:2|VAL-4714-G,EXP-4049-OUT;n:type:ShaderForge.SFN_Slider,id:4049,x:31287,y:34445,ptovrint:False,ptlb:vertexGreenPow,ptin:_vertexGreenPow,varname:node_4049,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Multiply,id:5700,x:32094,y:34574,varname:node_5700,prsc:2|A-891-OUT,B-1759-OUT;n:type:ShaderForge.SFN_Vector3,id:1759,x:31917,y:34714,varname:node_1759,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Multiply,id:4425,x:32341,y:34474,varname:node_4425,prsc:2|A-2270-OUT,B-5700-OUT;n:type:ShaderForge.SFN_Slider,id:2270,x:32002,y:34445,ptovrint:False,ptlb:tailAmplitude,ptin:_tailAmplitude,varname:node_2270,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Add,id:1902,x:32594,y:34052,varname:node_1902,prsc:2|A-349-OUT,B-4425-OUT;n:type:ShaderForge.SFN_Add,id:563,x:32324,y:33139,varname:node_563,prsc:2|A-562-R,B-562-G;n:type:ShaderForge.SFN_Multiply,id:5708,x:32296,y:32507,varname:node_5708,prsc:2|A-8232-OUT,B-562-G;proporder:562-4118-7878-7296-980-6186-6132-7345-9237-260-9553-2233-977-4049-2270;pass:END;sub:END;*/

Shader "Colony_FX/S_Flies02" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _freq ("freq", Range(0, 45)) = 4
        _wingCol1 ("wingCol1", Color) = (0.5,0.5,0.5,1)
        _amplitude ("amplitude", Range(0, 1)) = 0
        _wingEmissive1 ("wingEmissive1", Range(0, 10)) = 3
        _bodyPattern ("bodyPattern", 2D) = "white" {}
        _bodyPatternPower ("bodyPatternPower", Range(0, 5)) = 0.2840222
        _bodyColor2 ("bodyColor2", Color) = (0.5,0.5,0.5,1)
        _bodyEmissive2 ("bodyEmissive2", Range(0, 10)) = 0
        _bodyColor1 ("bodyColor1", Color) = (0.5,0.5,0.5,1)
        _bodyEmissive1 ("bodyEmissive1", Range(0, 25)) = 0
        _vertexRedPow ("vertexRedPow", Range(0, 5)) = 0
        _freqTail ("freqTail", Range(0, 45)) = 4
        _vertexGreenPow ("vertexGreenPow", Range(0, 5)) = 1
        _tailAmplitude ("tailAmplitude", Range(0, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _freq;
            uniform float4 _wingCol1;
            uniform float _amplitude;
            uniform float _wingEmissive1;
            uniform sampler2D _bodyPattern; uniform float4 _bodyPattern_ST;
            uniform float _bodyPatternPower;
            uniform float4 _bodyColor2;
            uniform float4 _bodyColor1;
            uniform float _bodyEmissive2;
            uniform float _bodyEmissive1;
            uniform float _vertexRedPow;
            uniform float _freqTail;
            uniform float _vertexGreenPow;
            uniform float _tailAmplitude;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                float4 node_4054 = _Time;
                float4 node_3967 = _Time;
                v.vertex.xyz += ((((pow(o.vertexColor.r,_vertexRedPow)*sin(((node_4054.a+o.uv1.r)*_freq)))*float3(0,1,0))*_amplitude)+(_tailAmplitude*((pow(o.vertexColor.g,_vertexGreenPow)*sin(((node_3967.a+o.uv1.r)*_freqTail)))*float3(0,0,1))));
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip((_MainTex_var.r+_MainTex_var.g) - 0.5);
////// Lighting:
////// Emissive:
                float4 _bodyPattern_var = tex2D(_bodyPattern,TRANSFORM_TEX(i.uv0, _bodyPattern));
                float3 node_8232 = (_wingEmissive1*_wingCol1.rgb);
                float3 emissive = lerp(lerp((_bodyColor2.rgb*_bodyEmissive2),(_bodyColor1.rgb*_bodyEmissive1),pow(_bodyPattern_var.r,_bodyPatternPower)),node_8232,_MainTex_var.r);
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
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _freq;
            uniform float _amplitude;
            uniform float _vertexRedPow;
            uniform float _freqTail;
            uniform float _vertexGreenPow;
            uniform float _tailAmplitude;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                float4 node_4054 = _Time;
                float4 node_3967 = _Time;
                v.vertex.xyz += ((((pow(o.vertexColor.r,_vertexRedPow)*sin(((node_4054.a+o.uv1.r)*_freq)))*float3(0,1,0))*_amplitude)+(_tailAmplitude*((pow(o.vertexColor.g,_vertexGreenPow)*sin(((node_3967.a+o.uv1.r)*_freqTail)))*float3(0,0,1))));
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip((_MainTex_var.r+_MainTex_var.g) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
