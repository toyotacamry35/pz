// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33785,y:32638,varname:node_3138,prsc:2|emission-3423-OUT,alpha-1409-OUT,refract-726-OUT;n:type:ShaderForge.SFN_Tex2d,id:665,x:31315,y:32732,ptovrint:False,ptlb:mainTex,ptin:_mainTex,varname:node_665,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6171-OUT;n:type:ShaderForge.SFN_Multiply,id:3423,x:32632,y:32731,varname:node_3423,prsc:2|A-6717-OUT,B-9980-OUT;n:type:ShaderForge.SFN_Slider,id:6717,x:32258,y:32612,ptovrint:False,ptlb:emissiveIntensity,ptin:_emissiveIntensity,varname:node_6717,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:33;n:type:ShaderForge.SFN_TexCoord,id:4058,x:30673,y:32743,varname:node_4058,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:8794,x:30882,y:32820,varname:node_8794,prsc:2|A-4556-OUT,B-4058-V;n:type:ShaderForge.SFN_Append,id:6171,x:31081,y:32766,varname:node_6171,prsc:2|A-4058-U,B-8794-OUT;n:type:ShaderForge.SFN_Slider,id:4556,x:30544,y:32967,ptovrint:False,ptlb:amp,ptin:_amp,varname:node_4556,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:15;n:type:ShaderForge.SFN_Time,id:1628,x:29671,y:31886,varname:node_1628,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5061,x:29992,y:31910,varname:node_5061,prsc:2|A-1628-T,B-7610-OUT;n:type:ShaderForge.SFN_Multiply,id:3068,x:29992,y:32051,varname:node_3068,prsc:2|A-1628-T,B-494-OUT;n:type:ShaderForge.SFN_Slider,id:7610,x:29636,y:32061,ptovrint:False,ptlb:panX,ptin:_panX,varname:node_7610,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:494,x:29636,y:32146,ptovrint:False,ptlb:panY,ptin:_panY,varname:_panX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_TexCoord,id:134,x:29992,y:31749,varname:node_134,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:2454,x:30193,y:31728,varname:node_2454,prsc:2|A-9316-OUT,B-134-UVOUT;n:type:ShaderForge.SFN_Slider,id:9316,x:29835,y:31594,ptovrint:False,ptlb:noiseTile,ptin:_noiseTile,varname:node_9316,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Append,id:8787,x:30193,y:31976,varname:node_8787,prsc:2|A-5061-OUT,B-3068-OUT;n:type:ShaderForge.SFN_Add,id:1451,x:30397,y:31855,varname:node_1451,prsc:2|A-2454-OUT,B-8787-OUT;n:type:ShaderForge.SFN_Tex2d,id:6326,x:30740,y:31921,ptovrint:False,ptlb:noiseTex,ptin:_noiseTex,varname:node_6326,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1451-OUT;n:type:ShaderForge.SFN_Multiply,id:9980,x:32337,y:32750,varname:node_9980,prsc:2|A-5096-OUT,B-665-R,C-6615-OUT,D-7286-A;n:type:ShaderForge.SFN_Multiply,id:7606,x:30995,y:31888,varname:node_7606,prsc:2|A-9552-OUT,B-6326-R;n:type:ShaderForge.SFN_Slider,id:9552,x:30719,y:31738,ptovrint:False,ptlb:noiseIntensity,ptin:_noiseIntensity,varname:node_9552,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Color,id:3286,x:31631,y:32991,ptovrint:False,ptlb:col,ptin:_col,varname:node_3286,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:6272,x:32532,y:33098,ptovrint:False,ptlb:normalTex,ptin:_normalTex,varname:node_6272,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-1451-OUT;n:type:ShaderForge.SFN_ComponentMask,id:207,x:32764,y:33105,varname:node_207,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6272-RGB;n:type:ShaderForge.SFN_Slider,id:1544,x:33059,y:33313,ptovrint:False,ptlb:refIntensity,ptin:_refIntensity,varname:node_1544,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Multiply,id:726,x:33449,y:33097,varname:node_726,prsc:2|A-9561-OUT,B-1544-OUT;n:type:ShaderForge.SFN_ComponentMask,id:1409,x:32916,y:32899,varname:node_1409,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-3423-OUT;n:type:ShaderForge.SFN_Lerp,id:9561,x:33232,y:33097,varname:node_9561,prsc:2|A-6062-OUT,B-207-OUT,T-6326-R;n:type:ShaderForge.SFN_Vector1,id:6062,x:33002,y:33044,varname:node_6062,prsc:2,v1:0;n:type:ShaderForge.SFN_VertexColor,id:7286,x:32289,y:32897,varname:node_7286,prsc:2;n:type:ShaderForge.SFN_Power,id:5096,x:31234,y:31888,varname:node_5096,prsc:2|VAL-7606-OUT,EXP-9662-OUT;n:type:ShaderForge.SFN_Slider,id:9662,x:31077,y:31740,ptovrint:False,ptlb:noisePow,ptin:_noisePow,varname:node_9662,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Lerp,id:4718,x:31963,y:33153,varname:node_4718,prsc:2|A-3286-RGB,B-1749-RGB,T-9198-OUT;n:type:ShaderForge.SFN_ScreenPos,id:1558,x:33062,y:34632,varname:node_1558,prsc:2,sctp:2;n:type:ShaderForge.SFN_Color,id:1749,x:31631,y:33170,ptovrint:False,ptlb:col2,ptin:_col2,varname:_col_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Subtract,id:8566,x:31540,y:33332,varname:node_8566,prsc:2|A-5096-OUT,B-3619-OUT;n:type:ShaderForge.SFN_Slider,id:3619,x:31180,y:33350,ptovrint:False,ptlb:colorMix,ptin:_colorMix,varname:node_3619,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Clamp01,id:9198,x:31721,y:33332,varname:node_9198,prsc:2|IN-8566-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:6615,x:32133,y:33083,ptovrint:False,ptlb:useColorMix,ptin:_useColorMix,varname:node_6615,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-3286-RGB,B-4718-OUT;proporder:665-6717-4556-7610-494-9316-6326-9552-3286-6272-1544-9662-1749-3619-6615;pass:END;sub:END;*/

Shader "Colony_FX/S_Trail_Plugin" {
    Properties {
        _mainTex ("mainTex", 2D) = "white" {}
        _emissiveIntensity ("emissiveIntensity", Range(0, 33)) = 0
        _amp ("amp", Range(0, 15)) = 0
        _panX ("panX", Range(-5, 5)) = 0
        _panY ("panY", Range(-5, 5)) = 0
        _noiseTile ("noiseTile", Range(0, 10)) = 1
        _noiseTex ("noiseTex", 2D) = "white" {}
        _noiseIntensity ("noiseIntensity", Range(0, 5)) = 1
        _col ("col", Color) = (0.5,0.5,0.5,1)
        _normalTex ("normalTex", 2D) = "bump" {}
        _refIntensity ("refIntensity", Range(0, 10)) = 1
        _noisePow ("noisePow", Range(0, 15)) = 1
        _col2 ("col2", Color) = (0.5,0.5,0.5,1)
        _colorMix ("colorMix", Range(0, 3)) = 1
        [MaterialToggle] _useColorMix ("useColorMix", Float ) = 0.5
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _mainTex; uniform float4 _mainTex_ST;
            uniform float _emissiveIntensity;
            uniform float _amp;
            uniform float _panX;
            uniform float _panY;
            uniform float _noiseTile;
            uniform sampler2D _noiseTex; uniform float4 _noiseTex_ST;
            uniform float _noiseIntensity;
            uniform float4 _col;
            uniform sampler2D _normalTex; uniform float4 _normalTex_ST;
            uniform float _refIntensity;
            uniform float _noisePow;
            uniform float4 _col2;
            uniform float _colorMix;
            uniform fixed _useColorMix;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float node_6062 = 0.0;
                float4 node_1628 = _Time;
                float2 node_1451 = ((_noiseTile*i.uv0)+float2((node_1628.g*_panX),(node_1628.g*_panY)));
                float3 _normalTex_var = UnpackNormal(tex2D(_normalTex,TRANSFORM_TEX(node_1451, _normalTex)));
                float4 _noiseTex_var = tex2D(_noiseTex,TRANSFORM_TEX(node_1451, _noiseTex));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (lerp(float2(node_6062,node_6062),_normalTex_var.rgb.rg,_noiseTex_var.r)*_refIntensity);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float node_5096 = pow((_noiseIntensity*_noiseTex_var.r),_noisePow);
                float2 node_6171 = float2(i.uv0.r,(_amp*i.uv0.g));
                float4 _mainTex_var = tex2D(_mainTex,TRANSFORM_TEX(node_6171, _mainTex));
                float3 node_3423 = (_emissiveIntensity*(node_5096*_mainTex_var.r*lerp( _col.rgb, lerp(_col.rgb,_col2.rgb,saturate((node_5096-_colorMix))), _useColorMix )*i.vertexColor.a));
                float3 emissive = node_3423;
                float3 finalColor = emissive;
                return fixed4(lerp(sceneColor.rgb, finalColor,node_3423.r),1);
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
