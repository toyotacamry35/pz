// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33864,y:32914,varname:node_3138,prsc:2|emission-2079-OUT,clip-562-R,voffset-349-OUT;n:type:ShaderForge.SFN_Tex2d,id:562,x:31470,y:33162,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_562,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:2079,x:32290,y:32720,varname:node_2079,prsc:2|A-556-OUT,B-2605-OUT,T-562-G;n:type:ShaderForge.SFN_TexCoord,id:816,x:30765,y:33450,varname:node_816,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:1778,x:31470,y:33471,varname:node_1778,prsc:2|A-816-U,B-3487-OUT;n:type:ShaderForge.SFN_Slider,id:3487,x:31116,y:33699,ptovrint:False,ptlb:edgeWidth,ptin:_edgeWidth,varname:node_3487,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.9020271,max:1;n:type:ShaderForge.SFN_Power,id:5800,x:31752,y:33470,varname:node_5800,prsc:2|VAL-1778-OUT,EXP-5009-OUT;n:type:ShaderForge.SFN_Slider,id:5009,x:31431,y:33697,ptovrint:False,ptlb:edgePow,ptin:_edgePow,varname:node_5009,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.94067,max:15;n:type:ShaderForge.SFN_OneMinus,id:6923,x:31195,y:33884,varname:node_6923,prsc:2|IN-816-U;n:type:ShaderForge.SFN_Add,id:9853,x:31469,y:33866,varname:node_9853,prsc:2|A-3487-OUT,B-6923-OUT;n:type:ShaderForge.SFN_Power,id:6476,x:31771,y:33867,varname:node_6476,prsc:2|VAL-9853-OUT,EXP-5009-OUT;n:type:ShaderForge.SFN_Clamp01,id:1723,x:31941,y:33470,varname:node_1723,prsc:2|IN-5800-OUT;n:type:ShaderForge.SFN_Clamp01,id:6984,x:31952,y:33867,varname:node_6984,prsc:2|IN-6476-OUT;n:type:ShaderForge.SFN_Multiply,id:3360,x:32126,y:33632,varname:node_3360,prsc:2|A-1723-OUT,B-6984-OUT;n:type:ShaderForge.SFN_OneMinus,id:8545,x:32316,y:33632,varname:node_8545,prsc:2|IN-3360-OUT;n:type:ShaderForge.SFN_Multiply,id:1055,x:32649,y:33631,varname:node_1055,prsc:2|A-8545-OUT,B-1200-OUT;n:type:ShaderForge.SFN_Time,id:4054,x:32126,y:33881,varname:node_4054,prsc:2;n:type:ShaderForge.SFN_Sin,id:1200,x:32714,y:33998,varname:node_1200,prsc:2|IN-228-OUT;n:type:ShaderForge.SFN_Multiply,id:228,x:32536,y:33998,varname:node_228,prsc:2|A-5431-OUT,B-3619-V;n:type:ShaderForge.SFN_Slider,id:4118,x:31749,y:34157,ptovrint:False,ptlb:freq,ptin:_freq,varname:node_4118,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4,max:45;n:type:ShaderForge.SFN_Multiply,id:3303,x:32955,y:33630,varname:node_3303,prsc:2|A-1055-OUT,B-5330-OUT;n:type:ShaderForge.SFN_Vector3,id:5330,x:32737,y:33813,varname:node_5330,prsc:2,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_Color,id:7878,x:30732,y:32071,ptovrint:False,ptlb:wingCol1,ptin:_wingCol1,varname:node_7878,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_TexCoord,id:3619,x:32134,y:34142,varname:node_3619,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Add,id:5431,x:32344,y:33998,varname:node_5431,prsc:2|A-4054-TTR,B-3619-U;n:type:ShaderForge.SFN_Multiply,id:349,x:33292,y:33631,varname:node_349,prsc:2|A-3303-OUT,B-7296-OUT;n:type:ShaderForge.SFN_Slider,id:7296,x:33052,y:33832,ptovrint:False,ptlb:amplitude,ptin:_amplitude,varname:node_7296,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2d,id:652,x:30667,y:32331,ptovrint:False,ptlb:wingPattern,ptin:_wingPattern,varname:node_652,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:556,x:31760,y:32505,varname:node_556,prsc:2|A-4407-OUT,B-8232-OUT,T-664-OUT;n:type:ShaderForge.SFN_Color,id:8805,x:30765,y:31723,ptovrint:False,ptlb:wingCol2,ptin:_wingCol2,varname:_wingCol2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:2580,x:30936,y:31613,ptovrint:False,ptlb:wingEmissive2,ptin:_wingEmissive2,varname:node_2580,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:7,max:10;n:type:ShaderForge.SFN_Power,id:664,x:31165,y:32390,varname:node_664,prsc:2|VAL-652-R,EXP-2398-OUT;n:type:ShaderForge.SFN_Slider,id:2398,x:30614,y:32558,ptovrint:False,ptlb:wingPatternPow,ptin:_wingPatternPow,varname:node_2398,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Multiply,id:4407,x:31186,y:31719,varname:node_4407,prsc:2|A-2580-OUT,B-8805-RGB;n:type:ShaderForge.SFN_Multiply,id:8232,x:31144,y:32067,varname:node_8232,prsc:2|A-980-OUT,B-7878-RGB;n:type:ShaderForge.SFN_Slider,id:980,x:30575,y:31947,ptovrint:False,ptlb:wingEmissive1,ptin:_wingEmissive1,varname:node_980,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3,max:10;n:type:ShaderForge.SFN_Lerp,id:2605,x:31716,y:32792,varname:node_2605,prsc:2|A-8412-OUT,B-1236-OUT,T-6126-OUT;n:type:ShaderForge.SFN_Tex2d,id:6186,x:30483,y:33054,ptovrint:False,ptlb:bodyPattern,ptin:_bodyPattern,varname:node_6186,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:6132,x:30556,y:33249,ptovrint:False,ptlb:bodyPatternPower,ptin:_bodyPatternPower,varname:node_6132,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2840222,max:5;n:type:ShaderForge.SFN_Power,id:6126,x:30935,y:33103,varname:node_6126,prsc:2|VAL-6186-R,EXP-6132-OUT;n:type:ShaderForge.SFN_Color,id:7345,x:30512,y:32655,ptovrint:False,ptlb:bodyColor2,ptin:_bodyColor2,varname:node_7345,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:260,x:30496,y:32839,ptovrint:False,ptlb:bodyColor1,ptin:_bodyColor1,varname:_bodyColor2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:8412,x:30957,y:32677,varname:node_8412,prsc:2|A-7345-RGB,B-9237-OUT;n:type:ShaderForge.SFN_Multiply,id:1236,x:30966,y:32860,varname:node_1236,prsc:2|A-260-RGB,B-9553-OUT;n:type:ShaderForge.SFN_Slider,id:9237,x:30177,y:32710,ptovrint:False,ptlb:bodyEmissive2,ptin:_bodyEmissive2,varname:node_9237,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Slider,id:9553,x:30140,y:32928,ptovrint:False,ptlb:bodyEmissive1,ptin:_bodyEmissive1,varname:_bodyEmissive2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:25;proporder:562-3487-5009-4118-7878-7296-652-8805-2580-2398-980-6186-6132-7345-9237-260-9553;pass:END;sub:END;*/

Shader "Colony_FX/S_Flies" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _edgeWidth ("edgeWidth", Range(0, 1)) = 0.9020271
        _edgePow ("edgePow", Range(0, 15)) = 4.94067
        _freq ("freq", Range(0, 45)) = 4
        _wingCol1 ("wingCol1", Color) = (0.5,0.5,0.5,1)
        _amplitude ("amplitude", Range(0, 1)) = 0
        _wingPattern ("wingPattern", 2D) = "white" {}
        _wingCol2 ("wingCol2", Color) = (0.5,0.5,0.5,1)
        _wingEmissive2 ("wingEmissive2", Range(0, 10)) = 7
        _wingPatternPow ("wingPatternPow", Range(0, 5)) = 0
        _wingEmissive1 ("wingEmissive1", Range(0, 10)) = 3
        _bodyPattern ("bodyPattern", 2D) = "white" {}
        _bodyPatternPower ("bodyPatternPower", Range(0, 5)) = 0.2840222
        _bodyColor2 ("bodyColor2", Color) = (0.5,0.5,0.5,1)
        _bodyEmissive2 ("bodyEmissive2", Range(0, 10)) = 0
        _bodyColor1 ("bodyColor1", Color) = (0.5,0.5,0.5,1)
        _bodyEmissive1 ("bodyEmissive1", Range(0, 25)) = 0
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
            uniform float _edgeWidth;
            uniform float _edgePow;
            uniform float4 _wingCol1;
            uniform float _amplitude;
            uniform sampler2D _wingPattern; uniform float4 _wingPattern_ST;
            uniform float4 _wingCol2;
            uniform float _wingEmissive2;
            uniform float _wingPatternPow;
            uniform float _wingEmissive1;
            uniform sampler2D _bodyPattern; uniform float4 _bodyPattern_ST;
            uniform float _bodyPatternPower;
            uniform float4 _bodyColor2;
            uniform float4 _bodyColor1;
            uniform float _bodyEmissive2;
            uniform float _bodyEmissive1;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                float4 node_4054 = _Time;
                v.vertex.xyz += ((((1.0 - (saturate(pow((o.uv0.r+_edgeWidth),_edgePow))*saturate(pow((_edgeWidth+(1.0 - o.uv0.r)),_edgePow))))*sin(((node_4054.a+o.uv1.r)*o.uv1.g)))*float3(0,1,0))*_amplitude);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip(_MainTex_var.r - 0.5);
////// Lighting:
////// Emissive:
                float4 _wingPattern_var = tex2D(_wingPattern,TRANSFORM_TEX(i.uv0, _wingPattern));
                float4 _bodyPattern_var = tex2D(_bodyPattern,TRANSFORM_TEX(i.uv0, _bodyPattern));
                float3 emissive = lerp(lerp((_wingEmissive2*_wingCol2.rgb),(_wingEmissive1*_wingCol1.rgb),pow(_wingPattern_var.r,_wingPatternPow)),lerp((_bodyColor2.rgb*_bodyEmissive2),(_bodyColor1.rgb*_bodyEmissive1),pow(_bodyPattern_var.r,_bodyPatternPower)),_MainTex_var.g);
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
            uniform float _edgeWidth;
            uniform float _edgePow;
            uniform float _amplitude;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                float4 node_4054 = _Time;
                v.vertex.xyz += ((((1.0 - (saturate(pow((o.uv0.r+_edgeWidth),_edgePow))*saturate(pow((_edgeWidth+(1.0 - o.uv0.r)),_edgePow))))*sin(((node_4054.a+o.uv1.r)*o.uv1.g)))*float3(0,1,0))*_amplitude);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                clip(_MainTex_var.r - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
