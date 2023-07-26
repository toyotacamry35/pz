// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.589902,fgcg:0.9053665,fgcb:0.9545916,fgca:1,fgde:0.00413295,fgrn:0,fgrf:600,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:36420,y:34116,varname:node_2865,prsc:2|emission-3997-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:31716,y:34880,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Desaturate,id:5119,x:33333,y:33116,varname:node_5119,prsc:2|COL-3394-RGB,DES-8731-OUT;n:type:ShaderForge.SFN_Slider,id:1175,x:32789,y:33426,ptovrint:False,ptlb:sceneDesaturation,ptin:_sceneDesaturation,varname:_sceneDesaturation,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:3997,x:36046,y:34786,varname:node_3997,prsc:2|A-6003-RGB,B-8752-OUT,T-9197-OUT;n:type:ShaderForge.SFN_Slider,id:9197,x:35569,y:35282,ptovrint:False,ptlb:finalMix,ptin:_finalMix,varname:_finalMix,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:4759,x:33510,y:32971,ptovrint:False,ptlb:scenePow,ptin:_scenePow,varname:_scenePow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:11;n:type:ShaderForge.SFN_Tex2d,id:6003,x:35659,y:34786,varname:node_6003,prsc:2,ntxv:0,isnm:False|TEX-4430-TEX;n:type:ShaderForge.SFN_Smoothstep,id:8619,x:33912,y:34113,varname:node_8619,prsc:2|A-5709-OUT,B-631-OUT,V-9709-OUT;n:type:ShaderForge.SFN_RemapRange,id:8317,x:33385,y:34212,varname:node_8317,prsc:2,frmn:0,frmx:1,tomn:-0.5,tomx:0.5|IN-5590-UVOUT;n:type:ShaderForge.SFN_ScreenPos,id:5590,x:33164,y:34212,varname:node_5590,prsc:2,sctp:2;n:type:ShaderForge.SFN_Vector1,id:631,x:33596,y:34132,varname:node_631,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:8584,x:32478,y:33908,ptovrint:False,ptlb:mainVignette,ptin:_mainVignette,varname:_vignette_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:2;n:type:ShaderForge.SFN_Power,id:1170,x:33385,y:33982,varname:node_1170,prsc:2|VAL-3096-OUT,EXP-6407-OUT;n:type:ShaderForge.SFN_Vector1,id:6407,x:33306,y:34123,varname:node_6407,prsc:2,v1:0.4;n:type:ShaderForge.SFN_OneMinus,id:5709,x:33596,y:33982,varname:node_5709,prsc:2|IN-1170-OUT;n:type:ShaderForge.SFN_Length,id:9709,x:33596,y:34212,varname:node_9709,prsc:2|IN-8317-OUT;n:type:ShaderForge.SFN_Tex2d,id:3394,x:32403,y:33523,varname:node_3394,prsc:2,ntxv:0,isnm:False|TEX-4430-TEX;n:type:ShaderForge.SFN_Lerp,id:8752,x:35128,y:33075,varname:node_8752,prsc:2|A-6071-OUT,B-8646-RGB,T-6765-OUT;n:type:ShaderForge.SFN_Color,id:8646,x:34513,y:33439,ptovrint:False,ptlb:vignetterCol,ptin:_vignetterCol,varname:node_8646,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Power,id:3769,x:34288,y:34002,varname:node_3769,prsc:2|VAL-8619-OUT,EXP-2214-OUT;n:type:ShaderForge.SFN_Slider,id:2214,x:34091,y:33711,ptovrint:False,ptlb:mainVignettePow,ptin:_mainVignettePow,varname:node_2214,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:4;n:type:ShaderForge.SFN_Multiply,id:6765,x:34858,y:33818,varname:node_6765,prsc:2|A-8646-A,B-3769-OUT,C-5447-OUT;n:type:ShaderForge.SFN_Time,id:5393,x:32422,y:34417,varname:node_5393,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7792,x:32661,y:34510,varname:node_7792,prsc:2|A-5393-T,B-7107-OUT;n:type:ShaderForge.SFN_Slider,id:7107,x:32330,y:34635,ptovrint:False,ptlb:vignettePulse,ptin:_vignettePulse,varname:node_7107,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Sin,id:9447,x:32855,y:34510,varname:node_9447,prsc:2|IN-7792-OUT;n:type:ShaderForge.SFN_Multiply,id:7747,x:33233,y:34506,varname:node_7747,prsc:2|A-7130-OUT,B-9646-OUT;n:type:ShaderForge.SFN_Slider,id:7130,x:32776,y:34672,ptovrint:False,ptlb:vignettePulseAmp,ptin:_vignettePulseAmp,varname:node_7130,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.4;n:type:ShaderForge.SFN_Add,id:3096,x:33164,y:33982,varname:node_3096,prsc:2|A-9460-OUT,B-7747-OUT;n:type:ShaderForge.SFN_RemapRange,id:9646,x:33032,y:34489,varname:node_9646,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-9447-OUT;n:type:ShaderForge.SFN_Slider,id:5174,x:34243,y:34237,ptovrint:False,ptlb:mainVignetteMult,ptin:_mainVignetteMult,varname:node_5174,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:5;n:type:ShaderForge.SFN_Power,id:6071,x:34141,y:33124,varname:node_6071,prsc:2|VAL-5119-OUT,EXP-5677-OUT;n:type:ShaderForge.SFN_Multiply,id:9460,x:32906,y:33910,varname:node_9460,prsc:2|A-8584-OUT,B-980-OUT;n:type:ShaderForge.SFN_Multiply,id:5447,x:34660,y:34203,varname:node_5447,prsc:2|A-980-OUT,B-5174-OUT;n:type:ShaderForge.SFN_Lerp,id:5677,x:33921,y:32962,varname:node_5677,prsc:2|A-9478-OUT,B-4759-OUT,T-980-OUT;n:type:ShaderForge.SFN_Vector1,id:9478,x:33655,y:32891,varname:node_9478,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:8731,x:33146,y:33432,varname:node_8731,prsc:2|A-1175-OUT,B-980-OUT;n:type:ShaderForge.SFN_Slider,id:980,x:32699,y:33680,ptovrint:False,ptlb:statScale,ptin:_statScale,varname:node_980,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:0.5,max:1;proporder:4430-1175-4759-8584-2214-5174-8646-7107-7130-9197-980;pass:END;sub:END;*/

Shader "Colony_FX/PostProcess/S_PP_Fatigue" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _sceneDesaturation ("sceneDesaturation", Range(0, 1)) = 0
        _scenePow ("scenePow", Range(0, 11)) = 1
        _mainVignette ("mainVignette", Range(0, 2)) = 0.8
        _mainVignettePow ("mainVignettePow", Range(0, 4)) = 1
        _mainVignetteMult ("mainVignetteMult", Range(0, 5)) = 2
        _vignetterCol ("vignetterCol", Color) = (0.5,0.5,0.5,1)
        _vignettePulse ("vignettePulse", Range(0, 5)) = 0
        _vignettePulseAmp ("vignettePulseAmp", Range(0, 0.4)) = 0
        _finalMix ("finalMix", Range(0, 1)) = 0
        _statScale ("statScale", Range(0.5, 1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Overlay+1"
            "RenderType"="Overlay"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZTest Always
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _sceneDesaturation;
            uniform float _finalMix;
            uniform float _scenePow;
            uniform float _mainVignette;
            uniform float4 _vignetterCol;
            uniform float _mainVignettePow;
            uniform float _vignettePulse;
            uniform float _vignettePulseAmp;
            uniform float _mainVignetteMult;
            uniform float _statScale;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 projPos : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
////// Lighting:
////// Emissive:
                float4 node_6003 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_3394 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_5119 = lerp(node_3394.rgb,dot(node_3394.rgb,float3(0.3,0.59,0.11)),(_sceneDesaturation*_statScale));
                float4 node_5393 = _Time;
                float3 emissive = lerp(node_6003.rgb,lerp(pow(node_5119,lerp(1.0,_scenePow,_statScale)),_vignetterCol.rgb,(_vignetterCol.a*pow(smoothstep( (1.0 - pow(((_mainVignette*_statScale)+(_vignettePulseAmp*(sin((node_5393.g*_vignettePulse))*0.5+0.5))),0.4)), 1.0, length((sceneUVs.rg*1.0+-0.5)) ),_mainVignettePow)*(_statScale*_mainVignetteMult))),_finalMix);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
