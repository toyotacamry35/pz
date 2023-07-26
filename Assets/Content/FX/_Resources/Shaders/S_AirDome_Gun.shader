// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.2740957,fgcg:0.4279703,fgcb:0.5040076,fgca:1,fgde:0.005101918,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33685,y:32706,varname:node_9361,prsc:2|emission-4674-OUT,alpha-1691-OUT;n:type:ShaderForge.SFN_TexCoord,id:9422,x:31185,y:32933,varname:node_9422,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_OneMinus,id:7365,x:32217,y:32940,varname:node_7365,prsc:2|IN-4970-OUT;n:type:ShaderForge.SFN_Multiply,id:9027,x:31538,y:32938,varname:node_9027,prsc:2|A-9422-UVOUT,B-4080-OUT;n:type:ShaderForge.SFN_Vector1,id:4080,x:31328,y:33187,varname:node_4080,prsc:2,v1:2.3;n:type:ShaderForge.SFN_ComponentMask,id:4970,x:31996,y:32940,varname:node_4970,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-9027-OUT;n:type:ShaderForge.SFN_Color,id:8641,x:32617,y:32558,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_8641,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:4674,x:32864,y:32701,varname:node_4674,prsc:2|A-8641-RGB,B-75-OUT;n:type:ShaderForge.SFN_Slider,id:75,x:32460,y:32823,ptovrint:False,ptlb:emissiveMultiplier,ptin:_emissiveMultiplier,varname:node_75,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:10;n:type:ShaderForge.SFN_Multiply,id:3288,x:32496,y:32987,varname:node_3288,prsc:2|A-7365-OUT,B-1792-OUT;n:type:ShaderForge.SFN_Multiply,id:9190,x:32704,y:32987,varname:node_9190,prsc:2|A-3288-OUT,B-3989-G;n:type:ShaderForge.SFN_Tex2d,id:3989,x:32543,y:33308,ptovrint:False,ptlb:noiseTex,ptin:_noiseTex,varname:node_3989,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:04fca432d2d08bc45a501f481e49fbe1,ntxv:0,isnm:False|UVIN-2239-OUT;n:type:ShaderForge.SFN_Time,id:3984,x:31899,y:33590,varname:node_3984,prsc:2;n:type:ShaderForge.SFN_Vector2,id:3753,x:31936,y:33481,varname:node_3753,prsc:2,v1:-0.4,v2:0;n:type:ShaderForge.SFN_Multiply,id:7711,x:32131,y:33524,varname:node_7711,prsc:2|A-3753-OUT,B-3984-T;n:type:ShaderForge.SFN_TexCoord,id:1862,x:31881,y:33161,varname:node_1862,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:2239,x:32315,y:33422,varname:node_2239,prsc:2|A-3054-OUT,B-7711-OUT;n:type:ShaderForge.SFN_Vector1,id:1792,x:32309,y:33123,varname:node_1792,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Clamp01,id:4139,x:33092,y:32985,varname:node_4139,prsc:2|IN-3989-G;n:type:ShaderForge.SFN_TexCoord,id:5393,x:32926,y:33242,varname:node_5393,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:4993,x:33536,y:33539,varname:node_4993,prsc:2|A-5393-V,B-9123-OUT;n:type:ShaderForge.SFN_OneMinus,id:9123,x:33331,y:33617,varname:node_9123,prsc:2|IN-5393-V;n:type:ShaderForge.SFN_Vector1,id:3332,x:34005,y:33459,varname:node_3332,prsc:2,v1:5;n:type:ShaderForge.SFN_Multiply,id:1691,x:33395,y:32984,varname:node_1691,prsc:2|A-4139-OUT,B-6801-OUT;n:type:ShaderForge.SFN_Multiply,id:16,x:34017,y:33257,varname:node_16,prsc:2|A-4682-OUT,B-4993-OUT;n:type:ShaderForge.SFN_OneMinus,id:2589,x:33395,y:33220,varname:node_2589,prsc:2|IN-5393-U;n:type:ShaderForge.SFN_Multiply,id:6801,x:34211,y:33257,varname:node_6801,prsc:2|A-16-OUT,B-3332-OUT;n:type:ShaderForge.SFN_Multiply,id:3054,x:32077,y:33277,varname:node_3054,prsc:2|A-1862-UVOUT,B-3072-OUT;n:type:ShaderForge.SFN_Vector1,id:3072,x:31892,y:33340,varname:node_3072,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Power,id:4682,x:33732,y:33223,varname:node_4682,prsc:2|VAL-2589-OUT,EXP-7203-OUT;n:type:ShaderForge.SFN_Slider,id:2902,x:33379,y:33401,ptovrint:False,ptlb:linePow,ptin:_linePow,varname:node_2902,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:2,cur:2,max:50;n:type:ShaderForge.SFN_ValueProperty,id:7203,x:33536,y:33323,ptovrint:False,ptlb:_LinePow,ptin:_LinePow,varname:node_7203,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:8641-75-3989-2902;pass:END;sub:END;*/

Shader "Colony_FX/Unique/S_AirDome_Gun" {
    Properties {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _emissiveMultiplier ("emissiveMultiplier", Range(0, 10)) = 2
        _noiseTex ("noiseTex", 2D) = "white" {}
        _linePow ("linePow", Range(2, 50)) = 2
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
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _emissiveMultiplier;
            uniform sampler2D _noiseTex; uniform float4 _noiseTex_ST;
            uniform float _LinePow;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float3 emissive = (_Color.rgb*_emissiveMultiplier);
                float3 finalColor = emissive;
                float4 node_3984 = _Time;
                float2 node_2239 = ((i.uv0*0.2)+(float2(-0.4,0)*node_3984.g));
                float4 _noiseTex_var = tex2D(_noiseTex,TRANSFORM_TEX(node_2239, _noiseTex));
                fixed4 finalRGBA = fixed4(finalColor,(saturate(_noiseTex_var.g)*((pow((1.0 - i.uv0.r),_LinePow)*(i.uv0.g*(1.0 - i.uv0.g)))*5.0)));
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
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
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
