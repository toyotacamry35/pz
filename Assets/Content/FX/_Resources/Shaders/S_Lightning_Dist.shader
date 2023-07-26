// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32729,y:32701,varname:node_3138,prsc:2|emission-8397-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:31878,y:32586,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_Tex2d,id:9036,x:31190,y:32793,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_9036,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6089-OUT;n:type:ShaderForge.SFN_TexCoord,id:4080,x:30177,y:32795,varname:node_4080,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:9813,x:28685,y:33619,varname:node_9813,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:1906,x:28745,y:33053,varname:node_1906,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:3370,x:29302,y:33219,varname:node_3370,prsc:2|A-1906-UVOUT,B-6782-OUT;n:type:ShaderForge.SFN_Slider,id:6121,x:28726,y:33306,ptovrint:False,ptlb:noiseTileX,ptin:_noiseTileX,varname:node_6121,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:3043,x:28726,y:33407,ptovrint:False,ptlb:noiseTileY,ptin:_noiseTileY,varname:_node_6121_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Append,id:6782,x:29075,y:33343,varname:node_6782,prsc:2|A-6121-OUT,B-3043-OUT;n:type:ShaderForge.SFN_Add,id:7474,x:29536,y:33243,varname:node_7474,prsc:2|A-3370-OUT,B-2670-OUT;n:type:ShaderForge.SFN_Tex2d,id:1164,x:30068,y:33227,ptovrint:False,ptlb:DistTex,ptin:_DistTex,varname:node_1164,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4613-OUT;n:type:ShaderForge.SFN_Slider,id:7520,x:28704,y:33806,ptovrint:False,ptlb:noisePanX,ptin:_noisePanX,varname:_noiseTileX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:8180,x:28704,y:33907,ptovrint:False,ptlb:noisePanY,ptin:_noisePanY,varname:_noiseTileY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Append,id:7834,x:29053,y:33847,varname:node_7834,prsc:2|A-7520-OUT,B-8180-OUT;n:type:ShaderForge.SFN_Multiply,id:2670,x:29257,y:33646,varname:node_2670,prsc:2|A-9813-T,B-7834-OUT;n:type:ShaderForge.SFN_Add,id:5516,x:30580,y:32794,varname:node_5516,prsc:2|A-4080-UVOUT,B-4887-OUT;n:type:ShaderForge.SFN_Multiply,id:4887,x:30345,y:33149,varname:node_4887,prsc:2|A-1872-OUT,B-1164-R;n:type:ShaderForge.SFN_Slider,id:1872,x:29989,y:33106,ptovrint:False,ptlb:distAmount,ptin:_distAmount,varname:node_1872,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:1281,x:32076,y:32791,varname:node_1281,prsc:2|A-7241-RGB,B-5915-OUT;n:type:ShaderForge.SFN_Multiply,id:8397,x:32380,y:32799,varname:node_8397,prsc:2|A-1281-OUT,B-9853-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9853,x:32151,y:32970,ptovrint:False,ptlb:emission,ptin:_emission,varname:node_9853,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:150;n:type:ShaderForge.SFN_Multiply,id:5915,x:31796,y:32795,varname:node_5915,prsc:2|A-2673-OUT,B-3817-OUT;n:type:ShaderForge.SFN_TexCoord,id:5028,x:30648,y:32309,varname:node_5028,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_OneMinus,id:3373,x:30923,y:32277,varname:node_3373,prsc:2|IN-5028-U;n:type:ShaderForge.SFN_TexCoord,id:4091,x:30557,y:32492,varname:node_4091,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Clamp01,id:2673,x:31559,y:32330,varname:node_2673,prsc:2|IN-2351-OUT;n:type:ShaderForge.SFN_Subtract,id:2351,x:31340,y:32330,varname:node_2351,prsc:2|A-3373-OUT,B-9160-OUT;n:type:ShaderForge.SFN_Multiply,id:8234,x:31079,y:32088,varname:node_8234,prsc:2|A-9018-OUT,B-5028-U;n:type:ShaderForge.SFN_Vector1,id:9018,x:30757,y:32092,varname:node_9018,prsc:2,v1:1;n:type:ShaderForge.SFN_OneMinus,id:9160,x:31076,y:32521,varname:node_9160,prsc:2|IN-4091-U;n:type:ShaderForge.SFN_Add,id:4613,x:29791,y:33243,varname:node_4613,prsc:2|A-7474-OUT,B-3899-OUT;n:type:ShaderForge.SFN_Vector2,id:6140,x:29501,y:33414,varname:node_6140,prsc:2,v1:3,v2:7;n:type:ShaderForge.SFN_Multiply,id:3899,x:29711,y:33483,varname:node_3899,prsc:2|A-6140-OUT,B-5679-Z;n:type:ShaderForge.SFN_TexCoord,id:5679,x:29501,y:33541,varname:node_5679,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Subtract,id:7453,x:31432,y:32971,varname:node_7453,prsc:2|A-9036-R,B-4091-V;n:type:ShaderForge.SFN_Clamp01,id:3817,x:31618,y:32971,varname:node_3817,prsc:2|IN-7453-OUT;n:type:ShaderForge.SFN_Add,id:6089,x:30863,y:32793,varname:node_6089,prsc:2|A-5516-OUT,B-7792-OUT;n:type:ShaderForge.SFN_Tex2d,id:9710,x:30422,y:33449,ptovrint:False,ptlb:radialGradDistTex,ptin:_radialGradDistTex,varname:node_9710,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Append,id:8686,x:30626,y:33406,varname:node_8686,prsc:2|A-6404-OUT,B-9710-R;n:type:ShaderForge.SFN_Vector1,id:6404,x:30422,y:33349,varname:node_6404,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:7792,x:30867,y:33388,varname:node_7792,prsc:2|A-4091-W,B-8686-OUT;n:type:ShaderForge.SFN_Slider,id:9447,x:30566,y:33251,ptovrint:False,ptlb:radialDistAmount,ptin:_radialDistAmount,varname:node_9447,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;proporder:7241-9036-6121-3043-1164-7520-8180-1872-9853-9710-9447;pass:END;sub:END;*/

Shader "Shader Forge/S_Lightning_Dist" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _noiseTileX ("noiseTileX", Range(0, 5)) = 0
        _noiseTileY ("noiseTileY", Range(0, 5)) = 0
        _DistTex ("DistTex", 2D) = "white" {}
        _noisePanX ("noisePanX", Range(-5, 5)) = 0
        _noisePanY ("noisePanY", Range(-5, 5)) = 0
        _distAmount ("distAmount", Range(0, 1)) = 0
        _emission ("emission", Float ) = 150
        _radialGradDistTex ("radialGradDistTex", 2D) = "white" {}
        _radialDistAmount ("radialDistAmount", Range(-1, 1)) = 0
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
            uniform float4 _Color;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _noiseTileX;
            uniform float _noiseTileY;
            uniform sampler2D _DistTex; uniform float4 _DistTex_ST;
            uniform float _noisePanX;
            uniform float _noisePanY;
            uniform float _distAmount;
            uniform float _emission;
            uniform sampler2D _radialGradDistTex; uniform float4 _radialGradDistTex_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float node_3373 = (1.0 - i.uv0.r);
                float4 node_9813 = _Time;
                float2 node_4613 = (((i.uv0*float2(_noiseTileX,_noiseTileY))+(node_9813.g*float2(_noisePanX,_noisePanY)))+(float2(3,7)*i.uv1.b));
                float4 _DistTex_var = tex2D(_DistTex,TRANSFORM_TEX(node_4613, _DistTex));
                float4 _radialGradDistTex_var = tex2D(_radialGradDistTex,TRANSFORM_TEX(i.uv0, _radialGradDistTex));
                float2 node_6089 = ((i.uv0+(_distAmount*_DistTex_var.r))+(i.uv1.a*float2(0.0,_radialGradDistTex_var.r)));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_6089, _MainTex));
                float3 emissive = ((_Color.rgb*(saturate((node_3373-(1.0 - i.uv1.r)))*saturate((_MainTex_var.r-i.uv1.g))))*_emission);
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
