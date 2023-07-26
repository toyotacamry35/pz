// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33846,y:32783,varname:node_3138,prsc:2|emission-692-OUT;n:type:ShaderForge.SFN_Color,id:8117,x:31290,y:32699,ptovrint:False,ptlb:CoreCol,ptin:_CoreCol,varname:_CoreCol,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:4711,x:31290,y:32529,ptovrint:False,ptlb:BorderCol,ptin:_BorderCol,varname:_BorderCol,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:8155,x:31966,y:32666,varname:node_8155,prsc:2|A-4711-RGB,B-8117-RGB,T-7426-OUT;n:type:ShaderForge.SFN_Tex2d,id:8128,x:32053,y:33141,ptovrint:False,ptlb:bandTex,ptin:_bandTex,varname:_bandTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5298-OUT;n:type:ShaderForge.SFN_Tex2d,id:8831,x:31285,y:33362,ptovrint:False,ptlb:noiseTex,ptin:_noiseTex,varname:_noiseTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3563-OUT;n:type:ShaderForge.SFN_TexCoord,id:9744,x:30452,y:33183,varname:node_9744,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:884,x:30237,y:33372,ptovrint:False,ptlb:noiseTilingX,ptin:_noiseTilingX,varname:_noiseTilingX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Multiply,id:1853,x:30770,y:33261,varname:node_1853,prsc:2|A-9744-UVOUT,B-3712-OUT;n:type:ShaderForge.SFN_Slider,id:7595,x:30215,y:33477,ptovrint:False,ptlb:noiseTilingY,ptin:_noiseTilingY,varname:_noiseTilingY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Append,id:3712,x:30597,y:33437,varname:node_3712,prsc:2|A-884-OUT,B-7595-OUT;n:type:ShaderForge.SFN_Time,id:1632,x:30661,y:33693,varname:node_1632,prsc:2;n:type:ShaderForge.SFN_Add,id:3563,x:31079,y:33348,varname:node_3563,prsc:2|A-1853-OUT,B-4965-OUT;n:type:ShaderForge.SFN_Multiply,id:4965,x:30993,y:33755,varname:node_4965,prsc:2|A-1632-T,B-6202-OUT;n:type:ShaderForge.SFN_Slider,id:8914,x:30475,y:33878,ptovrint:False,ptlb:noisePanX,ptin:_noisePanX,varname:_noisePanX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-3,cur:0,max:3;n:type:ShaderForge.SFN_Slider,id:4848,x:30475,y:33976,ptovrint:False,ptlb:noisePanY,ptin:_noisePanY,varname:_noisePanY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-3,cur:0,max:3;n:type:ShaderForge.SFN_Append,id:6202,x:30802,y:33893,varname:node_6202,prsc:2|A-8914-OUT,B-4848-OUT;n:type:ShaderForge.SFN_TexCoord,id:5552,x:31400,y:33148,varname:node_5552,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:5298,x:31856,y:33141,varname:node_5298,prsc:2|A-5552-UVOUT,B-3521-OUT,T-7772-OUT;n:type:ShaderForge.SFN_Add,id:3521,x:31609,y:33269,varname:node_3521,prsc:2|A-5552-UVOUT,B-8831-R;n:type:ShaderForge.SFN_Slider,id:7772,x:31601,y:33435,ptovrint:False,ptlb:distStrength,ptin:_distStrength,varname:_distStrength,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:692,x:33521,y:32866,varname:node_692,prsc:2|A-133-OUT,B-9668-OUT;n:type:ShaderForge.SFN_Slider,id:9668,x:33277,y:33136,ptovrint:False,ptlb:finalEm,ptin:_finalEm,varname:_finalEm,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3,max:300;n:type:ShaderForge.SFN_Multiply,id:8760,x:32634,y:32862,varname:node_8760,prsc:2|A-8542-OUT,B-8128-R;n:type:ShaderForge.SFN_Multiply,id:7533,x:32899,y:32864,varname:node_7533,prsc:2|A-8760-OUT,B-1643-OUT;n:type:ShaderForge.SFN_TexCoord,id:6559,x:32410,y:33329,varname:node_6559,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector1,id:258,x:32455,y:33505,varname:node_258,prsc:2,v1:0.85;n:type:ShaderForge.SFN_Add,id:4236,x:32694,y:33361,varname:node_4236,prsc:2|A-6559-U,B-258-OUT;n:type:ShaderForge.SFN_Power,id:193,x:32891,y:33361,varname:node_193,prsc:2|VAL-4236-OUT,EXP-8599-OUT;n:type:ShaderForge.SFN_Vector1,id:8599,x:32691,y:33549,varname:node_8599,prsc:2,v1:50;n:type:ShaderForge.SFN_Clamp01,id:1643,x:33096,y:33361,varname:node_1643,prsc:2|IN-193-OUT;n:type:ShaderForge.SFN_VertexColor,id:1556,x:32931,y:32584,varname:node_1556,prsc:2;n:type:ShaderForge.SFN_Multiply,id:133,x:33159,y:32846,varname:node_133,prsc:2|A-1556-RGB,B-7533-OUT,C-1556-A;n:type:ShaderForge.SFN_Clamp01,id:7426,x:31814,y:32778,varname:node_7426,prsc:2|IN-2501-OUT;n:type:ShaderForge.SFN_Power,id:2501,x:31638,y:32801,varname:node_2501,prsc:2|VAL-8128-R,EXP-1193-OUT;n:type:ShaderForge.SFN_Slider,id:1193,x:31209,y:32927,ptovrint:False,ptlb:colMaskPow,ptin:_colMaskPow,varname:node_1193,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:6;n:type:ShaderForge.SFN_Lerp,id:8542,x:32258,y:32664,varname:node_8542,prsc:2|A-3871-OUT,B-8155-OUT,T-5285-OUT;n:type:ShaderForge.SFN_Slider,id:5285,x:32043,y:32875,ptovrint:False,ptlb:useMatColor,ptin:_useMatColor,varname:node_5285,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Vector1,id:3871,x:32029,y:32553,varname:node_3871,prsc:2,v1:1;proporder:8117-4711-8128-8831-884-7595-8914-4848-7772-9668-1193-5285;pass:END;sub:END;*/

Shader "Colony_FX/Unique/S_FieryTrails" {
    Properties {
        _CoreCol ("CoreCol", Color) = (0.5,0.5,0.5,1)
        _BorderCol ("BorderCol", Color) = (0.5,0.5,0.5,1)
        _bandTex ("bandTex", 2D) = "white" {}
        _noiseTex ("noiseTex", 2D) = "white" {}
        _noiseTilingX ("noiseTilingX", Range(0, 5)) = 0
        _noiseTilingY ("noiseTilingY", Range(0, 5)) = 0
        _noisePanX ("noisePanX", Range(-3, 3)) = 0
        _noisePanY ("noisePanY", Range(-3, 3)) = 0
        _distStrength ("distStrength", Range(0, 1)) = 0
        _finalEm ("finalEm", Range(0, 300)) = 3
        _colMaskPow ("colMaskPow", Range(0, 6)) = 0
        _useMatColor ("useMatColor", Range(0, 1)) = 0
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _CoreCol;
            uniform float4 _BorderCol;
            uniform sampler2D _bandTex; uniform float4 _bandTex_ST;
            uniform sampler2D _noiseTex; uniform float4 _noiseTex_ST;
            uniform float _noiseTilingX;
            uniform float _noiseTilingY;
            uniform float _noisePanX;
            uniform float _noisePanY;
            uniform float _distStrength;
            uniform float _finalEm;
            uniform float _colMaskPow;
            uniform float _useMatColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float node_3871 = 1.0;
                float4 node_1632 = _Time;
                float2 node_3563 = ((i.uv0*float2(_noiseTilingX,_noiseTilingY))+(node_1632.g*float2(_noisePanX,_noisePanY)));
                float4 _noiseTex_var = tex2D(_noiseTex,TRANSFORM_TEX(node_3563, _noiseTex));
                float2 node_5298 = lerp(i.uv0,(i.uv0+_noiseTex_var.r),_distStrength);
                float4 _bandTex_var = tex2D(_bandTex,TRANSFORM_TEX(node_5298, _bandTex));
                float3 emissive = ((i.vertexColor.rgb*((lerp(float3(node_3871,node_3871,node_3871),lerp(_BorderCol.rgb,_CoreCol.rgb,saturate(pow(_bandTex_var.r,_colMaskPow))),_useMatColor)*_bandTex_var.r)*saturate(pow((i.uv0.r+0.85),50.0)))*i.vertexColor.a)*_finalEm);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
