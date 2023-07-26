// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:37323,y:35416,varname:node_2865,prsc:2|emission-3997-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:28612,y:34774,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4890,x:32830,y:36550,ptovrint:False,ptlb:distortionTex,ptin:_distortionTex,varname:_blackHolesNoise,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:edecc509e768569459e378283355ae30,ntxv:0,isnm:False|UVIN-7436-OUT;n:type:ShaderForge.SFN_Multiply,id:7976,x:32135,y:36434,varname:node_7976,prsc:2|A-9086-T,B-1353-OUT;n:type:ShaderForge.SFN_Slider,id:7684,x:31544,y:36424,ptovrint:False,ptlb:distortionPanX,ptin:_distortionPanX,varname:_blackHolesNoisePanX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:-0.05,max:2;n:type:ShaderForge.SFN_Slider,id:5335,x:31544,y:36538,ptovrint:False,ptlb:distortionPanY,ptin:_distortionPanY,varname:_blackHolesNoisePanY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:-0.02,max:5;n:type:ShaderForge.SFN_Append,id:1353,x:31902,y:36455,varname:node_1353,prsc:2|A-7684-OUT,B-5335-OUT;n:type:ShaderForge.SFN_Frac,id:6033,x:32371,y:36434,varname:node_6033,prsc:2|IN-7976-OUT;n:type:ShaderForge.SFN_TexCoord,id:8645,x:32139,y:36633,varname:node_8645,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:7436,x:32599,y:36550,varname:node_7436,prsc:2|A-6033-OUT,B-8890-OUT;n:type:ShaderForge.SFN_Multiply,id:8890,x:32371,y:36633,varname:node_8890,prsc:2|A-8645-UVOUT,B-3369-OUT;n:type:ShaderForge.SFN_Slider,id:3369,x:31982,y:36811,ptovrint:False,ptlb:distortionTile,ptin:_distortionTile,varname:_blackHolesNoiseTile,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:6,max:10;n:type:ShaderForge.SFN_Append,id:8267,x:33124,y:36563,varname:node_8267,prsc:2|A-4890-R,B-4890-R;n:type:ShaderForge.SFN_Multiply,id:7275,x:33384,y:36567,varname:node_7275,prsc:2|A-8267-OUT,B-3073-OUT;n:type:ShaderForge.SFN_Slider,id:3073,x:33057,y:36754,ptovrint:False,ptlb:distortionStrength,ptin:_distortionStrength,varname:_blackHolesDistortion,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.01,max:3;n:type:ShaderForge.SFN_Time,id:9086,x:31012,y:35510,varname:node_9086,prsc:2;n:type:ShaderForge.SFN_Lerp,id:3997,x:36864,y:35514,varname:node_3997,prsc:2|A-6003-RGB,B-1579-OUT,T-9197-OUT;n:type:ShaderForge.SFN_Slider,id:9197,x:36757,y:35717,ptovrint:False,ptlb:finalMix,ptin:_finalMix,varname:_finalMix,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Time,id:418,x:29247,y:32018,varname:node_418,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3291,x:29478,y:32080,varname:node_3291,prsc:2|A-418-T,B-7785-OUT;n:type:ShaderForge.SFN_Slider,id:7785,x:29134,y:32219,ptovrint:False,ptlb:blurPulseFrequency,ptin:_blurPulseFrequency,varname:_frequency,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:7;n:type:ShaderForge.SFN_Sin,id:1224,x:29670,y:32080,varname:node_1224,prsc:2|IN-3291-OUT;n:type:ShaderForge.SFN_Cos,id:9260,x:30064,y:32160,varname:node_9260,prsc:2|IN-9368-OUT;n:type:ShaderForge.SFN_Multiply,id:9368,x:29870,y:32160,varname:node_9368,prsc:2|A-1224-OUT,B-7864-OUT;n:type:ShaderForge.SFN_Pi,id:8247,x:29686,y:32242,varname:node_8247,prsc:2;n:type:ShaderForge.SFN_Distance,id:4997,x:30283,y:32083,varname:node_4997,prsc:2|A-1224-OUT,B-9260-OUT;n:type:ShaderForge.SFN_Multiply,id:7864,x:29870,y:32289,varname:node_7864,prsc:2|A-8247-OUT,B-8021-OUT;n:type:ShaderForge.SFN_Vector1,id:8021,x:29670,y:32355,varname:node_8021,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Lerp,id:1579,x:36474,y:35469,varname:node_1579,prsc:2|A-8752-OUT,B-8189-RGB,T-1012-OUT;n:type:ShaderForge.SFN_Tex2d,id:2519,x:34528,y:35622,varname:node_2519,prsc:2,ntxv:0,isnm:False|UVIN-6710-OUT,TEX-931-TEX;n:type:ShaderForge.SFN_TexCoord,id:3460,x:33582,y:35575,varname:node_3460,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:6040,x:33770,y:35642,varname:node_6040,prsc:2|A-3460-UVOUT,B-585-OUT;n:type:ShaderForge.SFN_RemapRange,id:585,x:33614,y:35824,varname:node_585,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-7275-OUT;n:type:ShaderForge.SFN_Add,id:6710,x:34273,y:35648,varname:node_6710,prsc:2|A-3681-OUT,B-6040-OUT;n:type:ShaderForge.SFN_Add,id:6879,x:34273,y:35839,varname:node_6879,prsc:2|A-1398-OUT,B-1637-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:931,x:34283,y:35397,ptovrint:False,ptlb:DizzyMask,ptin:_DizzyMask,varname:_DizzyMask,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:9118,x:34926,y:35759,varname:node_9118,prsc:2|A-3466-OUT,B-4313-OUT,T-9163-OUT;n:type:ShaderForge.SFN_Multiply,id:5986,x:35742,y:35765,varname:node_5986,prsc:2|A-5817-OUT,B-5232-OUT;n:type:ShaderForge.SFN_Slider,id:5232,x:35309,y:35992,ptovrint:False,ptlb:dizzyMaskMult,ptin:_dizzyMaskMult,varname:_dizzyMaskMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:25;n:type:ShaderForge.SFN_Tex2d,id:3298,x:34528,y:35820,varname:node_25111,prsc:2,ntxv:0,isnm:False|UVIN-6879-OUT,TEX-931-TEX;n:type:ShaderForge.SFN_Vector2,id:3382,x:33895,y:35942,varname:node_3382,prsc:2,v1:0.6,v2:0.6;n:type:ShaderForge.SFN_Add,id:1637,x:34080,y:35901,varname:node_1637,prsc:2|A-6040-OUT,B-3382-OUT;n:type:ShaderForge.SFN_Code,id:5830,x:31423,y:32556,varname:node_5830,prsc:2,code:ZgBsAG8AYQB0ADIAIABjAGUAbgB0AGUAcgAgAD0AIAAoADAALgA1ACwAMAAuADUAKQA7AAoAZgBsAG8AYQB0ADQAIABiAGwAdQByAHIAZQBkACAAPQAgADAAOwAKAGYAbwByACgAaQBuAHQAIABpACAAPQAgADAAOwAgAGkAIAA8ACAAQgBsAHUAcgBTAGEAbQBwAGwAZQBzADsAIABpACsAKwApACAACgB7AA0ACgBmAGwAbwBhAHQAIABzAGMAYQBsAGUAIAA9ACAAQgBsAHUAcgBTAHQAYQByAHQAIAArACAAQgBsAHUAcgBXAGkAZAB0AGgAIAAqACAAKABpAC8AKABCAGwAdQByAFMAYQBtAHAAbABlAHMAIAAtACAAMQApACkAOwAJAAoAYgBsAHUAcgByAGUAZAAgACsAPQAgAHQAZQB4ADIARAAoAF8ATQBhAGkAbgBUAGUAeAAsAFQAUgBBAE4AUwBGAE8AUgBNAF8AVABFAFgAKAAoAFMAYwBlAG4AZQBVAFYAcwAuAHIAZwAtADAALgA1ACkAKgBzAGMAYQBsAGUAIAArACAAYwBlAG4AdABlAHIALAAgAF8ATQBhAGkAbgBUAGUAeAApACkAOwANAAoAfQANAAoAYgBsAHUAcgByAGUAZAAgAC8APQAgAEIAbAB1AHIAUwBhAG0AcABsAGUAcwA7AA0ACgByAGUAdAB1AHIAbgAgAGIAbAB1AHIAcgBlAGQAOwAKAA==,output:3,fname:Function_node_5830,width:847,height:336,input:1,input:0,input:0,input:0,input:12,input_1_label:SceneUVs,input_2_label:BlurStart,input_3_label:BlurWidth,input_4_label:BlurSamples,input_5_label:Scene|A-219-UVOUT,B-4510-OUT,C-7056-OUT,D-8834-OUT,E-4430-TEX;n:type:ShaderForge.SFN_ScreenPos,id:219,x:30893,y:32336,varname:node_219,prsc:2,sctp:2;n:type:ShaderForge.SFN_Slider,id:4510,x:30808,y:32522,ptovrint:False,ptlb:blurStart,ptin:_blurStart,varname:_blurStart,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Slider,id:1349,x:30394,y:32740,ptovrint:False,ptlb:blurWidth,ptin:_blurWidth,varname:_blurWidth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.25,max:3;n:type:ShaderForge.SFN_Slider,id:8834,x:30808,y:32691,ptovrint:False,ptlb:blurSamples,ptin:_blurSamples,varname:_blurSamples,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5,max:24;n:type:ShaderForge.SFN_Tex2d,id:6003,x:35659,y:34786,varname:node_6003,prsc:2,ntxv:0,isnm:False|TEX-4430-TEX;n:type:ShaderForge.SFN_Multiply,id:7056,x:30606,y:32582,varname:node_7056,prsc:2|A-4997-OUT,B-1349-OUT;n:type:ShaderForge.SFN_Multiply,id:5817,x:35280,y:35737,varname:node_5817,prsc:2|A-9118-OUT,B-5973-OUT;n:type:ShaderForge.SFN_Clamp01,id:5973,x:35084,y:35839,varname:node_5973,prsc:2|IN-1013-OUT;n:type:ShaderForge.SFN_Clamp01,id:1012,x:36235,y:35625,varname:node_1012,prsc:2|IN-5092-OUT;n:type:ShaderForge.SFN_Smoothstep,id:8619,x:33912,y:34113,varname:node_8619,prsc:2|A-5709-OUT,B-631-OUT,V-4435-OUT;n:type:ShaderForge.SFN_Vector1,id:631,x:33622,y:34138,varname:node_631,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:8584,x:32474,y:34113,ptovrint:False,ptlb:mainVignette,ptin:_mainVignette,varname:_vignette_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.11,max:2;n:type:ShaderForge.SFN_Power,id:1170,x:33426,y:33973,varname:node_1170,prsc:2|VAL-8690-OUT,EXP-6407-OUT;n:type:ShaderForge.SFN_Vector1,id:6407,x:33276,y:34106,varname:node_6407,prsc:2,v1:0.4;n:type:ShaderForge.SFN_OneMinus,id:5709,x:33607,y:33982,varname:node_5709,prsc:2|IN-1170-OUT;n:type:ShaderForge.SFN_Multiply,id:1013,x:34096,y:34018,varname:node_1013,prsc:2|A-9572-OUT,B-8619-OUT;n:type:ShaderForge.SFN_Vector1,id:9572,x:33887,y:33998,varname:node_9572,prsc:2,v1:1.25;n:type:ShaderForge.SFN_Sin,id:1071,x:32928,y:35644,varname:node_1071,prsc:2|IN-2535-OUT;n:type:ShaderForge.SFN_RemapRange,id:9163,x:33116,y:35644,varname:node_9163,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-1071-OUT;n:type:ShaderForge.SFN_Trunc,id:9346,x:32853,y:35277,varname:node_9346,prsc:2|IN-9181-OUT;n:type:ShaderForge.SFN_Pi,id:4301,x:32209,y:35718,varname:node_4301,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3616,x:32358,y:35763,varname:node_3616,prsc:2|A-4301-OUT,B-87-OUT;n:type:ShaderForge.SFN_Vector1,id:87,x:32176,y:35853,varname:node_87,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:2066,x:32054,y:35267,varname:node_2066,prsc:2|A-9821-OUT,B-9086-T;n:type:ShaderForge.SFN_Trunc,id:4622,x:32853,y:35119,varname:node_4622,prsc:2|IN-9874-OUT;n:type:ShaderForge.SFN_Slider,id:9821,x:31691,y:35052,ptovrint:False,ptlb:swapFrequency,ptin:_swapFrequency,varname:node_7987,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:8;n:type:ShaderForge.SFN_Multiply,id:9874,x:32383,y:35115,varname:node_9874,prsc:2|A-3919-OUT,B-2066-OUT;n:type:ShaderForge.SFN_Vector1,id:3919,x:32177,y:34986,varname:node_3919,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Add,id:9181,x:32652,y:35277,varname:node_9181,prsc:2|A-9874-OUT,B-6364-OUT;n:type:ShaderForge.SFN_Vector1,id:6364,x:32451,y:35329,varname:node_6364,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:76,x:32539,y:35644,varname:node_76,prsc:2|A-2066-OUT,B-3616-OUT;n:type:ShaderForge.SFN_Add,id:2535,x:32733,y:35644,varname:node_2535,prsc:2|A-76-OUT,B-3616-OUT;n:type:ShaderForge.SFN_Multiply,id:3681,x:33090,y:35119,varname:node_3681,prsc:2|A-4622-OUT,B-5804-OUT;n:type:ShaderForge.SFN_Multiply,id:1398,x:33080,y:35290,varname:node_1398,prsc:2|A-9346-OUT,B-5804-OUT;n:type:ShaderForge.SFN_Vector1,id:5804,x:32972,y:35470,varname:node_5804,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Lerp,id:7655,x:32507,y:32631,varname:node_7655,prsc:2|A-3394-RGB,B-5830-OUT,T-9065-OUT;n:type:ShaderForge.SFN_Tex2d,id:3394,x:32225,y:33007,varname:node_3394,prsc:2,ntxv:0,isnm:False|TEX-4430-TEX;n:type:ShaderForge.SFN_Lerp,id:8752,x:35124,y:33173,varname:node_8752,prsc:2|A-7655-OUT,B-8646-RGB,T-6765-OUT;n:type:ShaderForge.SFN_Color,id:8646,x:34637,y:33336,ptovrint:False,ptlb:vignetteColor,ptin:_vignetteColor,varname:node_8646,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.990566,c2:0.9300119,c3:0.6961997,c4:0.9372549;n:type:ShaderForge.SFN_Power,id:3769,x:34502,y:33826,varname:node_3769,prsc:2|VAL-1013-OUT,EXP-2214-OUT;n:type:ShaderForge.SFN_Slider,id:2214,x:34439,y:34035,ptovrint:False,ptlb:colorVignettePow,ptin:_colorVignettePow,varname:node_2214,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:4;n:type:ShaderForge.SFN_Multiply,id:6765,x:34813,y:33727,varname:node_6765,prsc:2|A-8646-A,B-3769-OUT;n:type:ShaderForge.SFN_Tex2d,id:8189,x:36525,y:35661,ptovrint:False,ptlb:filmColorGradent,ptin:_filmColorGradent,varname:node_8189,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6624-OUT;n:type:ShaderForge.SFN_Append,id:6624,x:35978,y:36065,varname:node_6624,prsc:2|A-2976-OUT,B-2976-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:2976,x:35770,y:36055,varname:node_2976,prsc:2,min:0,max:0.9|IN-5986-OUT;n:type:ShaderForge.SFN_Multiply,id:5092,x:36061,y:35625,varname:node_5092,prsc:2|A-5986-OUT,B-821-OUT;n:type:ShaderForge.SFN_Vector1,id:821,x:36023,y:35789,varname:node_821,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Subtract,id:4313,x:34729,y:35897,varname:node_4313,prsc:2|A-3298-R,B-7716-OUT;n:type:ShaderForge.SFN_Vector1,id:3308,x:34222,y:36325,varname:node_3308,prsc:2,v1:0.75;n:type:ShaderForge.SFN_Multiply,id:7716,x:34428,y:36162,varname:node_7716,prsc:2|A-7233-OUT,B-3308-OUT;n:type:ShaderForge.SFN_OneMinus,id:1939,x:33922,y:36163,varname:node_1939,prsc:2|IN-9163-OUT;n:type:ShaderForge.SFN_Power,id:7233,x:34139,y:36163,varname:node_7233,prsc:2|VAL-1939-OUT,EXP-2862-OUT;n:type:ShaderForge.SFN_Vector1,id:2862,x:33973,y:36294,varname:node_2862,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Sin,id:9030,x:32931,y:35937,varname:node_9030,prsc:2|IN-1106-OUT;n:type:ShaderForge.SFN_RemapRange,id:3074,x:33119,y:35937,varname:node_3074,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-9030-OUT;n:type:ShaderForge.SFN_Multiply,id:5213,x:32542,y:35937,varname:node_5213,prsc:2|A-3616-OUT,B-2066-OUT;n:type:ShaderForge.SFN_Add,id:1106,x:32736,y:35937,varname:node_1106,prsc:2|A-5213-OUT,B-311-OUT;n:type:ShaderForge.SFN_Multiply,id:311,x:32618,y:36067,varname:node_311,prsc:2|A-4301-OUT,B-8500-OUT;n:type:ShaderForge.SFN_Vector1,id:8500,x:32411,y:36086,varname:node_8500,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Vector1,id:8314,x:34273,y:36571,varname:node_8314,prsc:2,v1:0.75;n:type:ShaderForge.SFN_Multiply,id:7430,x:34479,y:36408,varname:node_7430,prsc:2|A-8785-OUT,B-8314-OUT;n:type:ShaderForge.SFN_OneMinus,id:2742,x:33914,y:36448,varname:node_2742,prsc:2|IN-3074-OUT;n:type:ShaderForge.SFN_Power,id:8785,x:34190,y:36409,varname:node_8785,prsc:2|VAL-9163-OUT,EXP-7081-OUT;n:type:ShaderForge.SFN_Vector1,id:7081,x:34024,y:36540,varname:node_7081,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Subtract,id:3466,x:34729,y:35683,varname:node_3466,prsc:2|A-2519-R,B-7430-OUT;n:type:ShaderForge.SFN_Smoothstep,id:1917,x:31712,y:33366,varname:node_1917,prsc:2|A-413-OUT,B-3596-OUT,V-2072-OUT;n:type:ShaderForge.SFN_RemapRange,id:4499,x:31185,y:33465,varname:node_4499,prsc:2,frmn:0,frmx:1,tomn:-0.5,tomx:0.5|IN-3660-UVOUT;n:type:ShaderForge.SFN_ScreenPos,id:3660,x:31010,y:33473,varname:node_3660,prsc:2,sctp:2;n:type:ShaderForge.SFN_Vector1,id:3596,x:31422,y:33391,varname:node_3596,prsc:2,v1:1;n:type:ShaderForge.SFN_Power,id:7357,x:31239,y:33235,varname:node_7357,prsc:2|VAL-2560-OUT,EXP-1860-OUT;n:type:ShaderForge.SFN_Vector1,id:1860,x:31075,y:33267,varname:node_1860,prsc:2,v1:0.4;n:type:ShaderForge.SFN_OneMinus,id:413,x:31407,y:33235,varname:node_413,prsc:2|IN-7357-OUT;n:type:ShaderForge.SFN_Length,id:2072,x:31407,y:33465,varname:node_2072,prsc:2|IN-4499-OUT;n:type:ShaderForge.SFN_Multiply,id:9065,x:31896,y:33268,varname:node_9065,prsc:2|A-1381-OUT,B-1917-OUT;n:type:ShaderForge.SFN_Vector1,id:1381,x:31687,y:33251,varname:node_1381,prsc:2,v1:1.25;n:type:ShaderForge.SFN_Slider,id:2560,x:30909,y:33158,ptovrint:False,ptlb:blurVignette,ptin:_blurVignette,varname:node_2560,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4,max:2;n:type:ShaderForge.SFN_Time,id:5393,x:31864,y:33713,varname:node_5393,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7792,x:32103,y:33806,varname:node_7792,prsc:2|A-5393-T,B-7107-OUT;n:type:ShaderForge.SFN_Slider,id:7107,x:31772,y:33931,ptovrint:False,ptlb:vignettePulse,ptin:_vignettePulse,varname:node_7107,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Sin,id:9447,x:32297,y:33806,varname:node_9447,prsc:2|IN-7792-OUT;n:type:ShaderForge.SFN_Multiply,id:7747,x:32666,y:33848,varname:node_7747,prsc:2|A-7130-OUT,B-9646-OUT;n:type:ShaderForge.SFN_Slider,id:7130,x:32218,y:33968,ptovrint:False,ptlb:vignettePulseAmp,ptin:_vignettePulseAmp,varname:node_7130,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.1;n:type:ShaderForge.SFN_Add,id:3096,x:32901,y:34010,varname:node_3096,prsc:2|A-8584-OUT,B-7747-OUT;n:type:ShaderForge.SFN_RemapRange,id:9646,x:32474,y:33768,varname:node_9646,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-9447-OUT;n:type:ShaderForge.SFN_Tex2d,id:4752,x:31637,y:34332,ptovrint:False,ptlb:vignetteDistorter,ptin:_vignetteDistorter,varname:node_7169,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Append,id:7734,x:31850,y:34392,varname:node_7734,prsc:2|A-4752-R,B-4752-G;n:type:ShaderForge.SFN_TexCoord,id:4482,x:29023,y:34577,varname:node_4482,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:8548,x:29231,y:34577,varname:node_8548,prsc:2|A-4482-UVOUT,B-5437-OUT;n:type:ShaderForge.SFN_Slider,id:5437,x:28893,y:34791,ptovrint:False,ptlb:vignetteDistorterTiling,ptin:_vignetteDistorterTiling,varname:node_3519,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:3;n:type:ShaderForge.SFN_ScreenPos,id:9808,x:31887,y:34146,varname:node_9808,prsc:2,sctp:2;n:type:ShaderForge.SFN_Slider,id:5859,x:31850,y:34633,ptovrint:False,ptlb:vignetteDistorterIntensity,ptin:_vignetteDistorterIntensity,varname:node_201,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:0.5;n:type:ShaderForge.SFN_RemapRange,id:4718,x:32613,y:34384,varname:node_4718,prsc:2,frmn:0,frmx:1,tomn:-0.5,tomx:0.5|IN-1154-OUT;n:type:ShaderForge.SFN_Lerp,id:1154,x:32461,y:34343,varname:node_1154,prsc:2|A-9808-UVOUT,B-3729-OUT,T-5859-OUT;n:type:ShaderForge.SFN_Add,id:3729,x:32133,y:34370,varname:node_3729,prsc:2|A-9808-UVOUT,B-7734-OUT;n:type:ShaderForge.SFN_Length,id:4435,x:32970,y:34329,varname:node_4435,prsc:2|IN-4718-OUT;n:type:ShaderForge.SFN_Multiply,id:8690,x:33201,y:33973,varname:node_8690,prsc:2|A-5373-OUT,B-3096-OUT;n:type:ShaderForge.SFN_Multiply,id:5373,x:33115,y:33744,varname:node_5373,prsc:2|A-6850-OUT,B-4216-OUT;n:type:ShaderForge.SFN_Slider,id:6850,x:32763,y:33684,ptovrint:False,ptlb:statVignetteScale,ptin:_statVignetteScale,varname:node_6850,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.4,cur:0.5,max:1;n:type:ShaderForge.SFN_Vector1,id:4216,x:32827,y:33776,varname:node_4216,prsc:2,v1:2.4;proporder:4430-931-5232-9821-4890-3369-7684-5335-3073-2560-7785-4510-1349-8834-8584-8646-2214-7107-7130-8189-9197-4752-5859-6850;pass:END;sub:END;*/

Shader "Colony_FX/PostProcess/S_PP_Overheat" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _DizzyMask ("DizzyMask", 2D) = "white" {}
        _dizzyMaskMult ("dizzyMaskMult", Range(0, 25)) = 0
        _swapFrequency ("swapFrequency", Range(0, 8)) = 1
        _distortionTex ("distortionTex", 2D) = "white" {}
        _distortionTile ("distortionTile", Range(0, 10)) = 6
        _distortionPanX ("distortionPanX", Range(-2, 2)) = -0.05
        _distortionPanY ("distortionPanY", Range(-5, 5)) = -0.02
        _distortionStrength ("distortionStrength", Range(0, 3)) = 0.01
        _blurVignette ("blurVignette", Range(0, 2)) = 0.4
        _blurPulseFrequency ("blurPulseFrequency", Range(0, 7)) = 1
        _blurStart ("blurStart", Range(0, 2)) = 1
        _blurWidth ("blurWidth", Range(0, 3)) = 0.25
        _blurSamples ("blurSamples", Range(0, 24)) = 5
        _mainVignette ("mainVignette", Range(0, 2)) = 0.11
        _vignetteColor ("vignetteColor", Color) = (0.990566,0.9300119,0.6961997,0.9372549)
        _colorVignettePow ("colorVignettePow", Range(0, 4)) = 0
        _vignettePulse ("vignettePulse", Range(0, 5)) = 0
        _vignettePulseAmp ("vignettePulseAmp", Range(0, 0.1)) = 0
        _filmColorGradent ("filmColorGradent", 2D) = "white" {}
        _finalMix ("finalMix", Range(0, 1)) = 0
        _vignetteDistorter ("vignetteDistorter", 2D) = "white" {}
        _vignetteDistorterIntensity ("vignetteDistorterIntensity", Range(0, 0.5)) = 0.2
        _statVignetteScale ("statVignetteScale", Range(0.4, 1)) = 0.5
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
            uniform sampler2D _distortionTex; uniform float4 _distortionTex_ST;
            uniform float _distortionPanX;
            uniform float _distortionPanY;
            uniform float _distortionTile;
            uniform float _distortionStrength;
            uniform float _finalMix;
            uniform float _blurPulseFrequency;
            uniform sampler2D _DizzyMask; uniform float4 _DizzyMask_ST;
            uniform float _dizzyMaskMult;
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
            uniform float _mainVignette;
            uniform float _swapFrequency;
            uniform float4 _vignetteColor;
            uniform float _colorVignettePow;
            uniform sampler2D _filmColorGradent; uniform float4 _filmColorGradent_ST;
            uniform float _blurVignette;
            uniform float _vignettePulse;
            uniform float _vignettePulseAmp;
            uniform sampler2D _vignetteDistorter; uniform float4 _vignetteDistorter_ST;
            uniform float _vignetteDistorterIntensity;
            uniform float _statVignetteScale;
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
                float4 node_418 = _Time;
                float node_1224 = sin((node_418.g*_blurPulseFrequency));
                float4 node_5393 = _Time;
                float4 _vignetteDistorter_var = tex2D(_vignetteDistorter,TRANSFORM_TEX(i.uv0, _vignetteDistorter));
                float node_1013 = (1.25*smoothstep( (1.0 - pow(((_statVignetteScale*2.4)*(_mainVignette+(_vignettePulseAmp*(sin((node_5393.g*_vignettePulse))*0.5+0.5)))),0.4)), 1.0, length((lerp(sceneUVs.rg,(sceneUVs.rg+float2(_vignetteDistorter_var.r,_vignetteDistorter_var.g)),_vignetteDistorterIntensity)*1.0+-0.5)) ));
                float4 node_9086 = _Time;
                float node_2066 = (_swapFrequency*node_9086.g);
                float node_9874 = (0.25*node_2066);
                float node_5804 = 0.2;
                float2 node_7436 = (frac((node_9086.g*float2(_distortionPanX,_distortionPanY)))+(i.uv0*_distortionTile));
                float4 _distortionTex_var = tex2D(_distortionTex,TRANSFORM_TEX(node_7436, _distortionTex));
                float2 node_6040 = (i.uv0+((float2(_distortionTex_var.r,_distortionTex_var.r)*_distortionStrength)*2.0+-1.0));
                float2 node_6710 = ((trunc(node_9874)*node_5804)+node_6040);
                float4 node_2519 = tex2D(_DizzyMask,TRANSFORM_TEX(node_6710, _DizzyMask));
                float node_4301 = 3.141592654;
                float node_3616 = (node_4301*0.5);
                float node_9163 = (sin(((node_2066*node_3616)+node_3616))*0.5+0.5);
                float2 node_6879 = ((trunc((node_9874+0.5))*node_5804)+(node_6040+float2(0.6,0.6)));
                float4 node_25111 = tex2D(_DizzyMask,TRANSFORM_TEX(node_6879, _DizzyMask));
                float node_5986 = ((lerp((node_2519.r-(pow(node_9163,0.5)*0.75)),(node_25111.r-(pow((1.0 - node_9163),0.5)*0.75)),node_9163)*saturate(node_1013))*_dizzyMaskMult);
                float node_2976 = clamp(node_5986,0,0.9);
                float2 node_6624 = float2(node_2976,node_2976);
                float4 _filmColorGradent_var = tex2D(_filmColorGradent,TRANSFORM_TEX(node_6624, _filmColorGradent));
                float3 emissive = lerp(float4(node_6003.rgb,0.0),lerp(lerp(lerp(float4(node_3394.rgb,0.0),Function_node_5830( sceneUVs.rg , _blurStart , (distance(node_1224,cos((node_1224*(3.141592654*0.7))))*_blurWidth) , _blurSamples , _MainTex ),(1.25*smoothstep( (1.0 - pow(_blurVignette,0.4)), 1.0, length((sceneUVs.rg*1.0+-0.5)) ))),float4(_vignetteColor.rgb,0.0),(_vignetteColor.a*pow(node_1013,_colorVignettePow))),float4(_filmColorGradent_var.rgb,0.0),saturate((node_5986*1.5))),_finalMix).rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
