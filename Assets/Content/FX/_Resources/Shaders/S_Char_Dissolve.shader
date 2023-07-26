// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.2796003,fgcg:0.4513296,fgcb:0.5206271,fgca:1,fgde:0.005100179,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:34535,y:32820,varname:node_2865,prsc:2|diff-7942-OUT,emission-7942-OUT,clip-404-OUT;n:type:ShaderForge.SFN_Vector1,id:6967,x:32111,y:33378,varname:node_6967,prsc:2,v1:100;n:type:ShaderForge.SFN_Tex2d,id:1144,x:32305,y:33082,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_1144,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:9613c1063357fd0409821e64251298f9,ntxv:0,isnm:False|UVIN-1484-OUT;n:type:ShaderForge.SFN_Subtract,id:6040,x:32672,y:33102,varname:node_6040,prsc:2|A-1144-R,B-3001-OUT;n:type:ShaderForge.SFN_Divide,id:3001,x:32382,y:33291,varname:node_3001,prsc:2|A-9806-U,B-6967-OUT;n:type:ShaderForge.SFN_Subtract,id:275,x:32690,y:33361,varname:node_275,prsc:2|A-1144-R,B-8383-OUT;n:type:ShaderForge.SFN_Add,id:1981,x:32321,y:33472,varname:node_1981,prsc:2|A-9806-U,B-4012-OUT;n:type:ShaderForge.SFN_Vector1,id:5679,x:32078,y:33516,varname:node_5679,prsc:2,v1:20;n:type:ShaderForge.SFN_Divide,id:8383,x:32523,y:33438,varname:node_8383,prsc:2|A-1981-OUT,B-6967-OUT;n:type:ShaderForge.SFN_Subtract,id:4131,x:33323,y:33313,varname:node_4131,prsc:2|A-6786-OUT,B-2226-OUT;n:type:ShaderForge.SFN_Clamp01,id:2861,x:32862,y:33102,varname:node_2861,prsc:2|IN-6040-OUT;n:type:ShaderForge.SFN_Clamp01,id:2519,x:32860,y:33361,varname:node_2519,prsc:2|IN-275-OUT;n:type:ShaderForge.SFN_Slider,id:4012,x:31960,y:33598,ptovrint:False,ptlb:difference,ptin:_difference,varname:node_4012,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:10.04931,max:100;n:type:ShaderForge.SFN_Ceil,id:6786,x:33082,y:33102,varname:node_6786,prsc:2|IN-2861-OUT;n:type:ShaderForge.SFN_Ceil,id:2226,x:33068,y:33373,varname:node_2226,prsc:2|IN-2519-OUT;n:type:ShaderForge.SFN_Lerp,id:7942,x:33634,y:32895,varname:node_7942,prsc:2|A-7152-OUT,B-6647-OUT,T-3593-OUT;n:type:ShaderForge.SFN_Color,id:1105,x:33313,y:32456,ptovrint:False,ptlb:mainCol,ptin:_mainCol,varname:node_1105,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.00351371,c2:0.153358,c3:0.5019608,c4:1;n:type:ShaderForge.SFN_Multiply,id:4898,x:33589,y:33452,varname:node_4898,prsc:2|A-4131-OUT,B-7567-R;n:type:ShaderForge.SFN_Tex2d,id:7567,x:33366,y:33558,ptovrint:False,ptlb:diff_noise,ptin:_diff_noise,varname:node_7567,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5545-OUT;n:type:ShaderForge.SFN_Multiply,id:3593,x:33791,y:33452,varname:node_3593,prsc:2|A-4898-OUT,B-6682-OUT;n:type:ShaderForge.SFN_Slider,id:6682,x:33550,y:33643,ptovrint:False,ptlb:diff_noise_mask_mult,ptin:_diff_noise_mask_mult,varname:node_6682,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:15;n:type:ShaderForge.SFN_Lerp,id:404,x:34279,y:33096,varname:node_404,prsc:2|A-6786-OUT,B-3593-OUT,T-4131-OUT;n:type:ShaderForge.SFN_TexCoord,id:2175,x:32726,y:33547,varname:node_2175,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:8692,x:32919,y:33557,varname:node_8692,prsc:2|A-2175-UVOUT,B-119-OUT;n:type:ShaderForge.SFN_Slider,id:119,x:32597,y:33754,ptovrint:False,ptlb:diff_noise_tiling,ptin:_diff_noise_tiling,varname:node_119,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:15,max:25;n:type:ShaderForge.SFN_Add,id:5545,x:33155,y:33590,varname:node_5545,prsc:2|A-8692-OUT,B-5113-OUT;n:type:ShaderForge.SFN_Time,id:8460,x:32898,y:33814,varname:node_8460,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5113,x:33069,y:33814,varname:node_5113,prsc:2|A-8460-T,B-9674-OUT;n:type:ShaderForge.SFN_Slider,id:9674,x:32804,y:34013,ptovrint:False,ptlb:pan_speed,ptin:_pan_speed,varname:node_9674,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:3;n:type:ShaderForge.SFN_VertexColor,id:4227,x:32814,y:32759,varname:node_4227,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:9806,x:31609,y:33272,varname:node_9806,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Multiply,id:3740,x:33131,y:32816,varname:node_3740,prsc:2|A-4227-RGB,B-4227-A;n:type:ShaderForge.SFN_Multiply,id:6647,x:33329,y:32816,varname:node_6647,prsc:2|A-8758-OUT,B-3740-OUT;n:type:ShaderForge.SFN_Slider,id:8758,x:33052,y:32691,ptovrint:False,ptlb:emissive,ptin:_emissive,varname:node_8758,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_TexCoord,id:1853,x:31746,y:32954,varname:node_1853,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:1577,x:31958,y:33022,varname:node_1577,prsc:2|A-1853-UVOUT,B-5826-OUT;n:type:ShaderForge.SFN_Slider,id:5826,x:31620,y:33168,ptovrint:False,ptlb:mainTile,ptin:_mainTile,varname:node_5826,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:5;n:type:ShaderForge.SFN_Add,id:1484,x:32118,y:33007,varname:node_1484,prsc:2|A-1385-OUT,B-1577-OUT;n:type:ShaderForge.SFN_Vector2,id:2400,x:31802,y:32716,varname:node_2400,prsc:2,v1:-3,v2:7;n:type:ShaderForge.SFN_Multiply,id:1385,x:32032,y:32750,varname:node_1385,prsc:2|A-2400-OUT,B-9806-V;n:type:ShaderForge.SFN_Multiply,id:7152,x:33659,y:32668,varname:node_7152,prsc:2|A-5119-OUT,B-1105-RGB;n:type:ShaderForge.SFN_Slider,id:5119,x:33486,y:32534,ptovrint:False,ptlb:mainColEmissive,ptin:_mainColEmissive,varname:node_5119,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;proporder:1144-4012-1105-7567-6682-119-9674-8758-5826-5119;pass:END;sub:END;*/

Shader "Colony_FX/S_Char_Dissolve" {
    Properties {
        _mask ("mask", 2D) = "white" {}
        _difference ("difference", Range(0, 100)) = 10.04931
        _mainCol ("mainCol", Color) = (0.00351371,0.153358,0.5019608,1)
        _diff_noise ("diff_noise", 2D) = "white" {}
        _diff_noise_mask_mult ("diff_noise_mask_mult", Range(0, 15)) = 0
        _diff_noise_tiling ("diff_noise_tiling", Range(0, 25)) = 15
        _pan_speed ("pan_speed", Range(0, 3)) = 0
        _emissive ("emissive", Range(0, 15)) = 1
        _mainTile ("mainTile", Range(0, 5)) = 0.2
        _mainColEmissive ("mainColEmissive", Range(0, 15)) = 1
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
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform float _difference;
            uniform float4 _mainCol;
            uniform sampler2D _diff_noise; uniform float4 _diff_noise_ST;
            uniform float _diff_noise_mask_mult;
            uniform float _diff_noise_tiling;
            uniform float _pan_speed;
            uniform float _emissive;
            uniform float _mainTile;
            uniform float _mainColEmissive;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 node_1484 = ((float2(-3,7)*i.uv1.g)+(i.uv0*_mainTile));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(node_1484, _mask));
                float node_6967 = 100.0;
                float node_6786 = ceil(saturate((_mask_var.r-(i.uv1.r/node_6967))));
                float node_4131 = (node_6786-ceil(saturate((_mask_var.r-((i.uv1.r+_difference)/node_6967)))));
                float4 node_8460 = _Time;
                float2 node_5545 = ((i.uv0*_diff_noise_tiling)+(node_8460.g*_pan_speed));
                float4 _diff_noise_var = tex2D(_diff_noise,TRANSFORM_TEX(node_5545, _diff_noise));
                float node_3593 = ((node_4131*_diff_noise_var.r)*_diff_noise_mask_mult);
                clip(lerp(node_6786,node_3593,node_4131) - 0.5);
////// Lighting:
////// Emissive:
                float3 node_7942 = lerp((_mainColEmissive*_mainCol.rgb),(_emissive*(i.vertexColor.rgb*i.vertexColor.a)),node_3593);
                float3 emissive = node_7942;
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
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
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform float _difference;
            uniform sampler2D _diff_noise; uniform float4 _diff_noise_ST;
            uniform float _diff_noise_mask_mult;
            uniform float _diff_noise_tiling;
            uniform float _pan_speed;
            uniform float _mainTile;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 uv1 : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 node_1484 = ((float2(-3,7)*i.uv1.g)+(i.uv0*_mainTile));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(node_1484, _mask));
                float node_6967 = 100.0;
                float node_6786 = ceil(saturate((_mask_var.r-(i.uv1.r/node_6967))));
                float node_4131 = (node_6786-ceil(saturate((_mask_var.r-((i.uv1.r+_difference)/node_6967)))));
                float4 node_8460 = _Time;
                float2 node_5545 = ((i.uv0*_diff_noise_tiling)+(node_8460.g*_pan_speed));
                float4 _diff_noise_var = tex2D(_diff_noise,TRANSFORM_TEX(node_5545, _diff_noise));
                float node_3593 = ((node_4131*_diff_noise_var.r)*_diff_noise_mask_mult);
                clip(lerp(node_6786,node_3593,node_4131) - 0.5);
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
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform float _difference;
            uniform float4 _mainCol;
            uniform sampler2D _diff_noise; uniform float4 _diff_noise_ST;
            uniform float _diff_noise_mask_mult;
            uniform float _diff_noise_tiling;
            uniform float _pan_speed;
            uniform float _emissive;
            uniform float _mainTile;
            uniform float _mainColEmissive;
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
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float2 node_1484 = ((float2(-3,7)*i.uv1.g)+(i.uv0*_mainTile));
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(node_1484, _mask));
                float node_6967 = 100.0;
                float node_6786 = ceil(saturate((_mask_var.r-(i.uv1.r/node_6967))));
                float node_4131 = (node_6786-ceil(saturate((_mask_var.r-((i.uv1.r+_difference)/node_6967)))));
                float4 node_8460 = _Time;
                float2 node_5545 = ((i.uv0*_diff_noise_tiling)+(node_8460.g*_pan_speed));
                float4 _diff_noise_var = tex2D(_diff_noise,TRANSFORM_TEX(node_5545, _diff_noise));
                float node_3593 = ((node_4131*_diff_noise_var.r)*_diff_noise_mask_mult);
                float3 node_7942 = lerp((_mainColEmissive*_mainCol.rgb),(_emissive*(i.vertexColor.rgb*i.vertexColor.a)),node_3593);
                o.Emission = node_7942;
                
                float3 diffColor = float3(0,0,0);
                o.Albedo = diffColor;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
