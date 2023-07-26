// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.589902,fgcg:0.9053665,fgcb:0.9545916,fgca:1,fgde:0.00413295,fgrn:0,fgrf:600,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33868,y:33260,varname:node_2865,prsc:2|emission-2371-OUT;n:type:ShaderForge.SFN_TexCoord,id:4219,x:30448,y:32523,cmnt:Default coordinates,varname:node_4219,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Relay,id:8397,x:32163,y:33237,cmnt:Refract here,varname:node_8397,prsc:2|IN-3074-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:31869,y:33675,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:node_9933,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:7542,x:32283,y:33332,varname:node_1672,prsc:2,ntxv:0,isnm:False|UVIN-8397-OUT,TEX-4430-TEX;n:type:ShaderForge.SFN_Lerp,id:1300,x:31551,y:32594,varname:node_1300,prsc:2|A-4219-UVOUT,B-7720-OUT,T-2968-OUT;n:type:ShaderForge.SFN_Add,id:7720,x:30714,y:32700,varname:node_7720,prsc:2|A-4219-U,B-2008-OUT;n:type:ShaderForge.SFN_Tex2d,id:1038,x:30242,y:32772,ptovrint:False,ptlb:normal,ptin:_normal,varname:node_1038,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Append,id:2008,x:30473,y:32789,varname:node_2008,prsc:2|A-1038-R,B-1038-G;n:type:ShaderForge.SFN_Slider,id:5634,x:30397,y:33238,ptovrint:False,ptlb:dist,ptin:_dist,varname:node_5634,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:9519,x:30830,y:33235,varname:node_9519,prsc:2|A-5634-OUT,B-1778-R,C-562-OUT;n:type:ShaderForge.SFN_Tex2d,id:1778,x:29918,y:33519,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_1778,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:534,x:31342,y:33025,varname:node_534,prsc:2|A-3343-OUT,B-8135-OUT,T-1778-G;n:type:ShaderForge.SFN_Add,id:3074,x:31837,y:32945,varname:node_3074,prsc:2|A-1300-OUT,B-4609-OUT;n:type:ShaderForge.SFN_Vector1,id:3343,x:30922,y:32974,varname:node_3343,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:8135,x:31071,y:33049,varname:node_8135,prsc:2|A-5369-OUT,B-3343-OUT;n:type:ShaderForge.SFN_Slider,id:5369,x:30670,y:33053,ptovrint:False,ptlb:offset,ptin:_offset,varname:node_5369,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-0.1,cur:0,max:0.1;n:type:ShaderForge.SFN_TexCoord,id:652,x:29675,y:33764,varname:node_652,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Power,id:556,x:30210,y:33836,varname:node_556,prsc:2|VAL-8805-OUT,EXP-2580-OUT;n:type:ShaderForge.SFN_Add,id:8805,x:29951,y:33835,varname:node_8805,prsc:2|A-652-V,B-9079-OUT;n:type:ShaderForge.SFN_Slider,id:9079,x:29550,y:33973,ptovrint:False,ptlb:crackMovement,ptin:_crackMovement,varname:node_9079,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Clamp01,id:562,x:30377,y:33802,varname:node_562,prsc:2|IN-556-OUT;n:type:ShaderForge.SFN_Vector1,id:2580,x:30035,y:34073,varname:node_2580,prsc:2,v1:15;n:type:ShaderForge.SFN_Multiply,id:2968,x:31124,y:33235,varname:node_2968,prsc:2|A-9519-OUT,B-2079-OUT,C-1495-OUT;n:type:ShaderForge.SFN_Slider,id:2079,x:30733,y:33383,ptovrint:False,ptlb:distMultiplier,ptin:_distMultiplier,varname:node_2079,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:4229,x:33105,y:33360,varname:node_4229,prsc:2|A-7542-RGB,B-816-RGB,T-9227-OUT;n:type:ShaderForge.SFN_Color,id:816,x:32628,y:33382,ptovrint:False,ptlb:col,ptin:_col,varname:node_816,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:9227,x:32847,y:33744,varname:node_9227,prsc:2|A-1778-B,B-816-A,C-562-OUT,D-5634-OUT,E-1495-OUT;n:type:ShaderForge.SFN_ScreenPos,id:7343,x:31211,y:34649,varname:node_7343,prsc:2,sctp:2;n:type:ShaderForge.SFN_Slider,id:2960,x:31455,y:34413,ptovrint:False,ptlb:vignette,ptin:_vignette,varname:node_5729,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Power,id:4056,x:32287,y:34404,varname:node_4056,prsc:2|VAL-3995-OUT,EXP-6179-OUT;n:type:ShaderForge.SFN_Vector1,id:6179,x:32079,y:34505,varname:node_6179,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Smoothstep,id:2482,x:32658,y:34405,varname:node_2482,prsc:2|A-4056-OUT,B-3292-OUT,V-6755-OUT;n:type:ShaderForge.SFN_Vector1,id:3292,x:32441,y:34487,varname:node_3292,prsc:2,v1:1;n:type:ShaderForge.SFN_Add,id:6140,x:31770,y:34767,varname:node_6140,prsc:2|A-7343-UVOUT,B-9015-R;n:type:ShaderForge.SFN_Lerp,id:6373,x:32026,y:34711,varname:node_6373,prsc:2|A-7343-UVOUT,B-6140-OUT,T-6144-OUT;n:type:ShaderForge.SFN_Slider,id:6144,x:31633,y:34968,ptovrint:False,ptlb:vignetteDist,ptin:_vignetteDist,varname:node_6001,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_RemapRange,id:7851,x:32235,y:34711,varname:node_7851,prsc:2,frmn:0,frmx:1,tomn:-0.7,tomx:0.7|IN-6373-OUT;n:type:ShaderForge.SFN_Length,id:6755,x:32429,y:34711,varname:node_6755,prsc:2|IN-7851-OUT;n:type:ShaderForge.SFN_Tex2d,id:9015,x:31334,y:34855,ptovrint:False,ptlb:vignetteDistorter,ptin:_vignetteDistorter,varname:node_3574,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-778-OUT;n:type:ShaderForge.SFN_Color,id:7753,x:32733,y:34043,ptovrint:False,ptlb:vignetteColor,ptin:_vignetteColor,varname:node_6864,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:2388,x:33032,y:34135,varname:node_2388,prsc:2|A-7753-A,B-9556-OUT;n:type:ShaderForge.SFN_OneMinus,id:3995,x:31825,y:34416,varname:node_3995,prsc:2|IN-2960-OUT;n:type:ShaderForge.SFN_Clamp01,id:9556,x:32834,y:34405,varname:node_9556,prsc:2|IN-2482-OUT;n:type:ShaderForge.SFN_Multiply,id:5533,x:33251,y:34174,varname:node_5533,prsc:2|A-2388-OUT,B-2832-OUT,C-1495-OUT;n:type:ShaderForge.SFN_Slider,id:2832,x:32655,y:34269,ptovrint:False,ptlb:vignetteLevel,ptin:_vignetteLevel,varname:node_5028,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_TexCoord,id:8826,x:30504,y:34755,varname:node_8826,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:778,x:30828,y:34819,varname:node_778,prsc:2|A-8826-UVOUT,B-806-OUT;n:type:ShaderForge.SFN_Vector2,id:9711,x:30377,y:34924,varname:node_9711,prsc:2,v1:3,v2:7;n:type:ShaderForge.SFN_Multiply,id:806,x:30586,y:34938,varname:node_806,prsc:2|A-9711-OUT,B-8355-OUT;n:type:ShaderForge.SFN_Slider,id:8355,x:30220,y:35079,ptovrint:False,ptlb:randomOffset,ptin:_randomOffset,varname:node_5679,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:2371,x:33519,y:33392,varname:node_2371,prsc:2|A-4229-OUT,B-7753-RGB,T-5533-OUT;n:type:ShaderForge.SFN_Slider,id:1495,x:29692,y:33017,ptovrint:False,ptlb:finalMix,ptin:_finalMix,varname:node_1495,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:4609,x:31582,y:33025,varname:node_4609,prsc:2|A-534-OUT,B-1495-OUT;proporder:4430-1038-5634-1778-5369-9079-2079-816-7753-2960-6144-9015-2832-8355-1495;pass:END;sub:END;*/

Shader "Shader Forge/S_PP_Crack" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _normal ("normal", 2D) = "white" {}
        _dist ("dist", Range(0, 1)) = 0
        _mask ("mask", 2D) = "white" {}
        _offset ("offset", Range(-0.1, 0.1)) = 0
        _crackMovement ("crackMovement", Range(0, 1)) = 0
        _distMultiplier ("distMultiplier", Range(0, 1)) = 0
        _col ("col", Color) = (0.5,0.5,0.5,1)
        _vignetteColor ("vignetteColor", Color) = (0.5,0.5,0.5,1)
        _vignette ("vignette", Range(0, 1)) = 0
        _vignetteDist ("vignetteDist", Range(0, 1)) = 0
        _vignetteDistorter ("vignetteDistorter", 2D) = "white" {}
        _vignetteLevel ("vignetteLevel", Range(0, 1)) = 0
        _randomOffset ("randomOffset", Range(0, 1)) = 0
        _finalMix ("finalMix", Range(0, 1)) = 0
        _transitionDelta("stateTransition", Range(0, 120000000)) = 10000000

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
            uniform sampler2D _normal; uniform float4 _normal_ST;
            uniform float _dist;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform float _offset;
            uniform float _crackMovement;
            uniform float _distMultiplier;
            uniform float4 _col;
            uniform float _vignette;
            uniform float _vignetteDist;
            uniform sampler2D _vignetteDistorter; uniform float4 _vignetteDistorter_ST;
            uniform float4 _vignetteColor;
            uniform float _vignetteLevel;
            uniform float _randomOffset;
            uniform float _finalMix;
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
                float4 _normal_var = tex2D(_normal,TRANSFORM_TEX(i.uv0, _normal));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
                float node_562 = saturate(pow((i.uv0.g+_crackMovement),15.0));
                float node_3343 = 0.0;
                float2 node_8397 = (lerp(i.uv0,(i.uv0.r+float2(_normal_var.r,_normal_var.g)),((_dist*_mask_var.r*node_562)*_distMultiplier*_finalMix))+(lerp(float2(node_3343,node_3343),float2(_offset,node_3343),_mask_var.g)*_finalMix)); // Refract here
                float4 node_1672 = tex2D(_MainTex,TRANSFORM_TEX(node_8397, _MainTex));
                float2 node_778 = (i.uv0+(float2(3,7)*_randomOffset));
                float4 _vignetteDistorter_var = tex2D(_vignetteDistorter,TRANSFORM_TEX(node_778, _vignetteDistorter));
                float3 emissive = lerp(lerp(node_1672.rgb,_col.rgb,(_mask_var.b*_col.a*node_562*_dist*_finalMix)),_vignetteColor.rgb,((_vignetteColor.a*saturate(smoothstep( pow((1.0 - _vignette),0.4), 1.0, length((lerp(sceneUVs.rg,(sceneUVs.rg+_vignetteDistorter_var.r),_vignetteDist)*1.4+-0.7)) )))*_vignetteLevel*_finalMix));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
