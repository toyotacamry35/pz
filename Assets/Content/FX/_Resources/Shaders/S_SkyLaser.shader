// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.004761319,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:34550,y:33008,varname:node_9361,prsc:2|emission-358-OUT,voffset-7367-OUT;n:type:ShaderForge.SFN_TexCoord,id:9422,x:31185,y:32933,varname:node_9422,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_OneMinus,id:7365,x:31979,y:32947,varname:node_7365,prsc:2|IN-4970-OUT;n:type:ShaderForge.SFN_Multiply,id:9027,x:31538,y:32938,varname:node_9027,prsc:2|A-9422-UVOUT,B-4080-OUT;n:type:ShaderForge.SFN_Vector1,id:4080,x:31328,y:33187,varname:node_4080,prsc:2,v1:3;n:type:ShaderForge.SFN_ComponentMask,id:4970,x:31770,y:32947,varname:node_4970,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-9027-OUT;n:type:ShaderForge.SFN_Color,id:8641,x:32617,y:32558,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_8641,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:4674,x:32864,y:32701,varname:node_4674,prsc:2|A-8641-RGB,B-75-OUT;n:type:ShaderForge.SFN_Slider,id:75,x:32460,y:32823,ptovrint:False,ptlb:emissiveMultiplier,ptin:_emissiveMultiplier,varname:node_75,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:10;n:type:ShaderForge.SFN_Multiply,id:3288,x:32496,y:32987,varname:node_3288,prsc:2|A-3263-OUT,B-1792-OUT;n:type:ShaderForge.SFN_Multiply,id:9190,x:32759,y:32986,varname:node_9190,prsc:2|A-3288-OUT,B-3989-G;n:type:ShaderForge.SFN_Tex2d,id:3989,x:32543,y:33308,ptovrint:False,ptlb:node_3989,ptin:_node_3989,varname:node_3989,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:04fca432d2d08bc45a501f481e49fbe1,ntxv:0,isnm:False|UVIN-2239-OUT;n:type:ShaderForge.SFN_Time,id:3984,x:31899,y:33590,varname:node_3984,prsc:2;n:type:ShaderForge.SFN_Vector2,id:3753,x:31936,y:33481,varname:node_3753,prsc:2,v1:0.02,v2:-0.05;n:type:ShaderForge.SFN_Multiply,id:7711,x:32131,y:33524,varname:node_7711,prsc:2|A-3753-OUT,B-3984-T;n:type:ShaderForge.SFN_TexCoord,id:1862,x:32104,y:33357,varname:node_1862,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:2239,x:32315,y:33422,varname:node_2239,prsc:2|A-1862-UVOUT,B-7711-OUT;n:type:ShaderForge.SFN_Vector1,id:1792,x:32309,y:33123,varname:node_1792,prsc:2,v1:1.3;n:type:ShaderForge.SFN_Clamp01,id:4139,x:33018,y:32985,varname:node_4139,prsc:2|IN-9190-OUT;n:type:ShaderForge.SFN_Fresnel,id:6729,x:33199,y:33189,varname:node_6729,prsc:2|EXP-6060-OUT;n:type:ShaderForge.SFN_Slider,id:6060,x:32863,y:33225,ptovrint:False,ptlb:fresnelPow,ptin:_fresnelPow,varname:node_6060,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Multiply,id:4750,x:33408,y:32975,varname:node_4750,prsc:2|A-4139-OUT,B-2969-OUT;n:type:ShaderForge.SFN_OneMinus,id:2969,x:33359,y:33189,varname:node_2969,prsc:2|IN-6729-OUT;n:type:ShaderForge.SFN_Power,id:3263,x:32218,y:32975,varname:node_3263,prsc:2|VAL-7365-OUT,EXP-9409-OUT;n:type:ShaderForge.SFN_Vector1,id:9409,x:32037,y:33098,varname:node_9409,prsc:2,v1:1.8;n:type:ShaderForge.SFN_Multiply,id:358,x:33468,y:32810,varname:node_358,prsc:2|A-4674-OUT,B-4750-OUT;n:type:ShaderForge.SFN_ViewPosition,id:9313,x:33474,y:33561,varname:node_9313,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:3083,x:33474,y:33393,varname:node_3083,prsc:2;n:type:ShaderForge.SFN_Distance,id:1531,x:33686,y:33498,varname:node_1531,prsc:2|A-3083-XYZ,B-9313-XYZ;n:type:ShaderForge.SFN_Divide,id:4020,x:33904,y:33498,varname:node_4020,prsc:2|A-1531-OUT,B-8045-OUT;n:type:ShaderForge.SFN_Vector1,id:8045,x:33728,y:33681,varname:node_8045,prsc:2,v1:50;n:type:ShaderForge.SFN_NormalVector,id:4325,x:33949,y:33659,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:8621,x:34148,y:33524,varname:node_8621,prsc:2|A-4020-OUT,B-4325-OUT;n:type:ShaderForge.SFN_Vector3,id:2199,x:34211,y:33662,varname:node_2199,prsc:2,v1:1,v2:0,v3:1;n:type:ShaderForge.SFN_Multiply,id:7367,x:34353,y:33524,varname:node_7367,prsc:2|A-8621-OUT,B-2199-OUT;proporder:8641-75-3989-6060;pass:END;sub:END;*/

Shader "Colony_FX/Unique/S_SkyLaser" {
    Properties {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _emissiveMultiplier ("emissiveMultiplier", Range(0, 10)) = 2
        _node_3989 ("node_3989", 2D) = "white" {}
        _fresnelPow ("fresnelPow", Range(0, 5)) = 1
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
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _emissiveMultiplier;
            uniform sampler2D _node_3989; uniform float4 _node_3989_ST;
            uniform float _fresnelPow;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                v.vertex.xyz += (((distance(mul(unity_ObjectToWorld, v.vertex).rgb,_WorldSpaceCameraPos)/50.0)*v.normal)*float3(1,0,1));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
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
                float4 node_3984 = _Time;
                float2 node_2239 = (i.uv0+(float2(0.02,-0.05)*node_3984.g));
                float4 _node_3989_var = tex2D(_node_3989,TRANSFORM_TEX(node_2239, _node_3989));
                float3 emissive = ((_Color.rgb*_emissiveMultiplier)*(saturate(((pow((1.0 - (i.uv0*3.0).g),1.8)*1.3)*_node_3989_var.g))*(1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_fresnelPow))));
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
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                v.vertex.xyz += (((distance(mul(unity_ObjectToWorld, v.vertex).rgb,_WorldSpaceCameraPos)/50.0)*v.normal)*float3(1,0,1));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
