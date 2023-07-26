// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.009757008,fgcg:0.6048318,fgcb:0.6488286,fgca:1,fgde:0.004197305,fgrn:0,fgrf:6000,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:34262,y:32853,varname:node_3138,prsc:2|emission-7463-OUT,alpha-5713-OUT;n:type:ShaderForge.SFN_Tex2d,id:6308,x:30635,y:33018,ptovrint:False,ptlb:radial,ptin:_radial,varname:node_6308,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:e1d7bb6cd67a71a4fa714262ce9ee63b,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3500,x:32416,y:33516,ptovrint:False,ptlb:band,ptin:_band,varname:node_3500,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:784f7c5a2fef0074b80c4bf95d67b13d,ntxv:0,isnm:False|UVIN-7634-OUT;n:type:ShaderForge.SFN_Multiply,id:1860,x:32972,y:33048,varname:node_1860,prsc:2|A-6308-R,B-3308-OUT;n:type:ShaderForge.SFN_Multiply,id:3308,x:32908,y:33540,varname:node_3308,prsc:2|A-2218-OUT,B-1042-OUT;n:type:ShaderForge.SFN_Slider,id:1042,x:32606,y:33955,ptovrint:False,ptlb:brightness,ptin:_brightness,varname:node_1042,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:111;n:type:ShaderForge.SFN_Multiply,id:8745,x:33516,y:32886,varname:node_8745,prsc:2|A-3431-OUT,B-1860-OUT;n:type:ShaderForge.SFN_Slider,id:3431,x:33390,y:32695,ptovrint:False,ptlb:em,ptin:_em,varname:node_3431,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:155;n:type:ShaderForge.SFN_Clamp01,id:5713,x:33595,y:33207,varname:node_5713,prsc:2|IN-1860-OUT;n:type:ShaderForge.SFN_Slider,id:5981,x:31415,y:33785,ptovrint:False,ptlb:dist,ptin:_dist,varname:node_5981,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Time,id:982,x:30580,y:33468,varname:node_982,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2535,x:31063,y:33490,varname:node_2535,prsc:2|A-982-T,B-1-OUT;n:type:ShaderForge.SFN_Slider,id:1848,x:30466,y:33667,ptovrint:False,ptlb:timeNoisePanX,ptin:_timeNoisePanX,varname:node_1848,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:3681,x:30477,y:33811,ptovrint:False,ptlb:timeNoisePanY,ptin:_timeNoisePanY,varname:node_3681,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:0,max:5;n:type:ShaderForge.SFN_Append,id:1,x:30859,y:33711,varname:node_1,prsc:2|A-1848-OUT,B-3681-OUT;n:type:ShaderForge.SFN_Add,id:8834,x:31601,y:33475,varname:node_8834,prsc:2|A-1486-OUT,B-2535-OUT;n:type:ShaderForge.SFN_TexCoord,id:106,x:31829,y:33239,varname:node_106,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:7634,x:32099,y:33476,varname:node_7634,prsc:2|A-106-UVOUT,B-8834-OUT,T-5981-OUT;n:type:ShaderForge.SFN_Append,id:1486,x:31395,y:33262,varname:node_1486,prsc:2|A-5642-OUT,B-5642-OUT;n:type:ShaderForge.SFN_Multiply,id:7463,x:33960,y:32900,varname:node_7463,prsc:2|A-8745-OUT,B-6191-RGB,C-2574-V;n:type:ShaderForge.SFN_Color,id:6191,x:33746,y:32983,ptovrint:False,ptlb:col,ptin:_col,varname:node_6191,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_OneMinus,id:5642,x:31112,y:33158,varname:node_5642,prsc:2|IN-6308-R;n:type:ShaderForge.SFN_Subtract,id:2218,x:32723,y:33552,varname:node_2218,prsc:2|A-3500-R,B-2574-U;n:type:ShaderForge.SFN_Slider,id:8468,x:31981,y:33808,ptovrint:False,ptlb:erosion,ptin:_erosion,varname:node_8468,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_TexCoord,id:2574,x:32266,y:33847,varname:node_2574,prsc:2,uv:1,uaff:True;proporder:6308-3500-1042-3431-5981-1848-3681-6191-8468;pass:END;sub:END;*/

Shader "Shader Forge/S_Radial_Distortion" {
    Properties {
        _radial ("radial", 2D) = "white" {}
        _band ("band", 2D) = "white" {}
        _brightness ("brightness", Range(0, 111)) = 1
        _em ("em", Range(0, 155)) = 0
        _dist ("dist", Range(0, 1)) = 0
        _timeNoisePanX ("timeNoisePanX", Range(-2, 5)) = 0
        _timeNoisePanY ("timeNoisePanY", Range(-2, 5)) = 0
        _col ("col", Color) = (0.5,0.5,0.5,1)
        _erosion ("erosion", Range(0, 1)) = 0
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
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _radial; uniform float4 _radial_ST;
            uniform sampler2D _band; uniform float4 _band_ST;
            uniform float _brightness;
            uniform float _em;
            uniform float _dist;
            uniform float _timeNoisePanX;
            uniform float _timeNoisePanY;
            uniform float4 _col;
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
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 _radial_var = tex2D(_radial,TRANSFORM_TEX(i.uv0, _radial));
                float node_5642 = (1.0 - _radial_var.r);
                float4 node_982 = _Time;
                float2 node_7634 = lerp(i.uv0,(float2(node_5642,node_5642)+(node_982.g*float2(_timeNoisePanX,_timeNoisePanY))),_dist);
                float4 _band_var = tex2D(_band,TRANSFORM_TEX(node_7634, _band));
                float node_1860 = (_radial_var.r*((_band_var.r-i.uv1.r)*_brightness));
                float3 emissive = ((_em*node_1860)*_col.rgb*i.uv1.g);
                float3 finalColor = emissive;
                return fixed4(finalColor,saturate(node_1860));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
