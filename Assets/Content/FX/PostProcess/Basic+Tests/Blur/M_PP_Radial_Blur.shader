// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33176,y:32649,varname:node_2865,prsc:2|emission-3382-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:31466,y:33018,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:node_9933,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Code,id:3382,x:32096,y:32750,varname:node_3382,prsc:2,code:ZgBsAG8AYQB0ADIAIABjAGUAbgB0AGUAcgAgAD0AIAAoADAALgA1ACwAMAAuADUAKQA7AAoAZgBsAG8AYQB0ADQAIABiAGwAdQByAHIAZQBkACAAPQAgADAAOwAKAGYAbwByACgAaQBuAHQAIABpACAAPQAgADAAOwAgAGkAIAA8ACAAQgBsAHUAcgBTAGEAbQBwAGwAZQBzADsAIABpACsAKwApACAACgB7AA0ACgBmAGwAbwBhAHQAIABzAGMAYQBsAGUAIAA9ACAAQgBsAHUAcgBTAHQAYQByAHQAIAArACAAQgBsAHUAcgBXAGkAZAB0AGgAIAAqACAAKABpAC8AKABCAGwAdQByAFMAYQBtAHAAbABlAHMAIAAtACAAMQApACkAOwAJAAoAYgBsAHUAcgByAGUAZAAgACsAPQAgAHQAZQB4ADIARAAoAF8ATQBhAGkAbgBUAGUAeAAsAFQAUgBBAE4AUwBGAE8AUgBNAF8AVABFAFgAKAAoAFMAYwBlAG4AZQBVAFYAcwAuAHIAZwAtADAALgA1ACkAKgBzAGMAYQBsAGUAIAArACAAYwBlAG4AdABlAHIALAAgAF8ATQBhAGkAbgBUAGUAeAApACkAOwANAAoAfQANAAoAYgBsAHUAcgByAGUAZAAgAC8APQAgAEIAbAB1AHIAUwBhAG0AcABsAGUAcwA7AA0ACgByAGUAdAB1AHIAbgAgAGIAbAB1AHIAcgBlAGQAOwAKAA==,output:3,fname:Function_node_5830,width:843,height:336,input:1,input:0,input:0,input:0,input:12,input_1_label:SceneUVs,input_2_label:BlurStart,input_3_label:BlurWidth,input_4_label:BlurSamples,input_5_label:Scene|A-5932-UVOUT,B-3628-OUT,C-5380-OUT,D-1288-OUT,E-4430-TEX;n:type:ShaderForge.SFN_ScreenPos,id:5932,x:31453,y:32393,varname:node_5932,prsc:2,sctp:2;n:type:ShaderForge.SFN_Slider,id:3628,x:31375,y:32599,ptovrint:False,ptlb:blurStart,ptin:_blurStart,varname:_blurStart,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Slider,id:5380,x:31375,y:32732,ptovrint:False,ptlb:blurWidth,ptin:_blurWidth,varname:_blurWidth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.25,max:3;n:type:ShaderForge.SFN_Slider,id:1288,x:31375,y:32877,ptovrint:False,ptlb:blurSamples,ptin:_blurSamples,varname:_blurSamples,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5,max:24;proporder:4430-3628-5380-1288;pass:END;sub:END;*/

Shader "Colony_FX/PostProcess/S_PP_Radial_Blur" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _blurStart ("blurStart", Range(0, 2)) = 1
        _blurWidth ("blurWidth", Range(0, 3)) = 0.25
        _blurSamples ("blurSamples", Range(0, 24)) = 5
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
            float4 Function_node_5830( float2 SceneUVs , float BlurStart , float BlurWidth , float BlurSamples , sampler2D Scene ){
            float2 center = (0.5,0.5);
            float4 blurred = 0;
            for(int i = 0; i < BlurSamples; i++) 
            {
            float scale = BlurStart + BlurWidth * (i/(BlurSamples - 1));	
            blurred += tex2D(_MainTex,TRANSFORM_TEX((SceneUVs.rg-0.5)*scale + center, _MainTex));
            }
            blurred /= BlurSamples;
            return blurred;
            
            }
            
            uniform float _blurStart;
            uniform float _blurWidth;
            uniform float _blurSamples;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
////// Lighting:
////// Emissive:
                float3 emissive = Function_node_5830( sceneUVs.rg , _blurStart , _blurWidth , _blurSamples , _MainTex ).rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
