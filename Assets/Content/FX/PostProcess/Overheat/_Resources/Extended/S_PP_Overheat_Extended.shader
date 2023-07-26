// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:VwBvAHIAbABkAFUAVgA=,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:36793,y:35406,varname:node_2865,prsc:2|emission-3997-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:30049,y:34134,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4890,x:32828,y:36286,ptovrint:False,ptlb:blackHolesNoise,ptin:_blackHolesNoise,varname:_blackHolesNoise,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:edecc509e768569459e378283355ae30,ntxv:0,isnm:False|UVIN-7436-OUT;n:type:ShaderForge.SFN_Multiply,id:7976,x:32133,y:36170,varname:node_7976,prsc:2|A-9086-T,B-1353-OUT;n:type:ShaderForge.SFN_Slider,id:7684,x:31542,y:36160,ptovrint:False,ptlb:blackHolesNoisePanX,ptin:_blackHolesNoisePanX,varname:_blackHolesNoisePanX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:-0.0232017,max:2;n:type:ShaderForge.SFN_Slider,id:5335,x:31542,y:36274,ptovrint:False,ptlb:blackHolesNoisePanY,ptin:_blackHolesNoisePanY,varname:_blackHolesNoisePanY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0.2661707,max:5;n:type:ShaderForge.SFN_Append,id:1353,x:31900,y:36191,varname:node_1353,prsc:2|A-7684-OUT,B-5335-OUT;n:type:ShaderForge.SFN_Frac,id:6033,x:32369,y:36170,varname:node_6033,prsc:2|IN-7976-OUT;n:type:ShaderForge.SFN_TexCoord,id:8645,x:32137,y:36369,varname:node_8645,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:7436,x:32597,y:36286,varname:node_7436,prsc:2|A-6033-OUT,B-8890-OUT;n:type:ShaderForge.SFN_Multiply,id:8890,x:32369,y:36369,varname:node_8890,prsc:2|A-8645-UVOUT,B-3369-OUT;n:type:ShaderForge.SFN_Slider,id:3369,x:31980,y:36547,ptovrint:False,ptlb:blackHolesNoiseTile,ptin:_blackHolesNoiseTile,varname:_blackHolesNoiseTile,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Append,id:8267,x:33122,y:36299,varname:node_8267,prsc:2|A-4890-R,B-4890-R;n:type:ShaderForge.SFN_Multiply,id:7275,x:33382,y:36303,varname:node_7275,prsc:2|A-8267-OUT,B-3073-OUT;n:type:ShaderForge.SFN_Slider,id:3073,x:33055,y:36490,ptovrint:False,ptlb:blackHolesDistortion,ptin:_blackHolesDistortion,varname:_blackHolesDistortion,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8281516,max:3;n:type:ShaderForge.SFN_Time,id:9086,x:28416,y:34151,varname:node_9086,prsc:2;n:type:ShaderForge.SFN_Desaturate,id:5119,x:33178,y:33054,varname:node_5119,prsc:2|COL-6608-OUT,DES-1175-OUT;n:type:ShaderForge.SFN_Slider,id:1175,x:33037,y:33214,ptovrint:False,ptlb:sceneDesaturation,ptin:_sceneDesaturation,varname:_sceneDesaturation,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:3997,x:36334,y:35504,varname:node_3997,prsc:2|A-6003-RGB,B-1579-OUT,T-9197-OUT;n:type:ShaderForge.SFN_Slider,id:9197,x:36276,y:35751,ptovrint:False,ptlb:finalMix,ptin:_finalMix,varname:_finalMix,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:2445,x:34662,y:33595,varname:node_2445,prsc:2|A-110-OUT,B-1836-OUT;n:type:ShaderForge.SFN_Sin,id:8965,x:34556,y:36153,varname:node_8965,prsc:2|IN-2709-OUT;n:type:ShaderForge.SFN_Multiply,id:2709,x:34387,y:36153,varname:node_2709,prsc:2|A-9086-T,B-7107-OUT;n:type:ShaderForge.SFN_Vector1,id:7107,x:34224,y:36231,varname:node_7107,prsc:2,v1:0.5;n:type:ShaderForge.SFN_RemapRange,id:1006,x:34729,y:36153,varname:node_1006,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-8965-OUT;n:type:ShaderForge.SFN_Slider,id:9403,x:32475,y:32493,ptovrint:False,ptlb:sceneMultiplicationR,ptin:_sceneMultiplicationR,varname:_sceneMultiplicationR,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Append,id:6123,x:33135,y:32517,varname:node_6123,prsc:2|A-5346-OUT,B-941-OUT,C-7704-OUT;n:type:ShaderForge.SFN_Slider,id:9761,x:32475,y:32594,ptovrint:False,ptlb:sceneMultiplicationG,ptin:_sceneMultiplicationG,varname:_sceneMultiplicationG,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Slider,id:8697,x:32475,y:32698,ptovrint:False,ptlb:sceneMultiplicationB,ptin:_sceneMultiplicationB,varname:_sceneMultiplicationB,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Power,id:110,x:33913,y:33050,varname:node_110,prsc:2|VAL-5342-OUT,EXP-4759-OUT;n:type:ShaderForge.SFN_Slider,id:4759,x:33600,y:32961,ptovrint:False,ptlb:scenePow,ptin:_scenePow,varname:_scenePow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:11;n:type:ShaderForge.SFN_Multiply,id:5346,x:32846,y:32437,varname:node_5346,prsc:2|A-9403-OUT,B-323-OUT;n:type:ShaderForge.SFN_Multiply,id:941,x:32846,y:32555,varname:node_941,prsc:2|A-9761-OUT,B-323-OUT;n:type:ShaderForge.SFN_Multiply,id:7704,x:32846,y:32680,varname:node_7704,prsc:2|A-8697-OUT,B-323-OUT;n:type:ShaderForge.SFN_Slider,id:323,x:32464,y:32399,ptovrint:False,ptlb:sceneMultiplicationScalar,ptin:_sceneMultiplicationScalar,varname:_sceneMultiplicationScalar,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Sin,id:932,x:33335,y:34569,varname:node_932,prsc:2|IN-9086-TDB;n:type:ShaderForge.SFN_Vector1,id:5421,x:33335,y:34743,varname:node_5421,prsc:2,v1:0.09;n:type:ShaderForge.SFN_Multiply,id:3970,x:33514,y:34569,varname:node_3970,prsc:2|A-932-OUT,B-5421-OUT;n:type:ShaderForge.SFN_Add,id:2820,x:32601,y:35368,varname:node_2820,prsc:2|A-6831-OUT,B-7927-OUT;n:type:ShaderForge.SFN_Trunc,id:6831,x:32433,y:35368,varname:node_6831,prsc:2|IN-6996-OUT;n:type:ShaderForge.SFN_Vector1,id:7927,x:32519,y:35540,varname:node_7927,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Pi,id:7690,x:31831,y:35516,varname:node_7690,prsc:2;n:type:ShaderForge.SFN_Add,id:9352,x:32018,y:35368,varname:node_9352,prsc:2|A-9086-T,B-7690-OUT;n:type:ShaderForge.SFN_Multiply,id:4643,x:32157,y:35525,varname:node_4643,prsc:2|A-7690-OUT,B-54-OUT;n:type:ShaderForge.SFN_Vector1,id:54,x:31970,y:35628,varname:node_54,prsc:2,v1:4;n:type:ShaderForge.SFN_Divide,id:6996,x:32250,y:35368,varname:node_6996,prsc:2|A-9352-OUT,B-4643-OUT;n:type:ShaderForge.SFN_Add,id:117,x:32250,y:35200,varname:node_117,prsc:2|A-3251-OUT,B-9086-T;n:type:ShaderForge.SFN_Vector1,id:2929,x:31908,y:35081,varname:node_2929,prsc:2,v1:3;n:type:ShaderForge.SFN_Multiply,id:3251,x:32018,y:35208,varname:node_3251,prsc:2|A-2929-OUT,B-7690-OUT;n:type:ShaderForge.SFN_Divide,id:8713,x:32433,y:35200,varname:node_8713,prsc:2|A-117-OUT,B-4643-OUT;n:type:ShaderForge.SFN_Trunc,id:7733,x:32629,y:35200,varname:node_7733,prsc:2|IN-8713-OUT;n:type:ShaderForge.SFN_Add,id:5598,x:32831,y:35200,varname:node_5598,prsc:2|A-7733-OUT,B-7927-OUT;n:type:ShaderForge.SFN_Multiply,id:5342,x:33459,y:33047,varname:node_5342,prsc:2|A-3793-OUT,B-5119-OUT;n:type:ShaderForge.SFN_Time,id:418,x:29564,y:32505,varname:node_418,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3291,x:29795,y:32567,varname:node_3291,prsc:2|A-418-T,B-7785-OUT;n:type:ShaderForge.SFN_Slider,id:7785,x:29451,y:32706,ptovrint:False,ptlb:frequency,ptin:_frequency,varname:_frequency,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8914911,max:7;n:type:ShaderForge.SFN_Sin,id:1224,x:29987,y:32567,varname:node_1224,prsc:2|IN-3291-OUT;n:type:ShaderForge.SFN_Cos,id:9260,x:30381,y:32647,varname:node_9260,prsc:2|IN-9368-OUT;n:type:ShaderForge.SFN_Multiply,id:9368,x:30187,y:32647,varname:node_9368,prsc:2|A-1224-OUT,B-7864-OUT;n:type:ShaderForge.SFN_Pi,id:8247,x:30003,y:32729,varname:node_8247,prsc:2;n:type:ShaderForge.SFN_Distance,id:4997,x:30600,y:32570,varname:node_4997,prsc:2|A-1224-OUT,B-9260-OUT;n:type:ShaderForge.SFN_Multiply,id:7864,x:30187,y:32776,varname:node_7864,prsc:2|A-8247-OUT,B-8021-OUT;n:type:ShaderForge.SFN_Vector1,id:8021,x:29987,y:32842,varname:node_8021,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Lerp,id:3793,x:33394,y:32707,varname:node_3793,prsc:2|A-3048-OUT,B-6123-OUT,T-2632-OUT;n:type:ShaderForge.SFN_Vector1,id:3048,x:33111,y:32725,varname:node_3048,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:2632,x:33053,y:32835,ptovrint:False,ptlb:sceneColorization,ptin:_sceneColorization,varname:_sceneColorization,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:1579,x:35944,y:35459,varname:node_1579,prsc:2|A-2445-OUT,B-3487-OUT,T-1012-OUT;n:type:ShaderForge.SFN_Tex2d,id:2519,x:34528,y:35622,varname:node_2519,prsc:2,ntxv:0,isnm:False|UVIN-6710-OUT,TEX-931-TEX;n:type:ShaderForge.SFN_Color,id:3274,x:35019,y:35415,ptovrint:False,ptlb:DizzyColor,ptin:_DizzyColor,varname:_DizzyColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_TexCoord,id:3460,x:33582,y:35575,varname:node_3460,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:6040,x:33770,y:35642,varname:node_6040,prsc:2|A-3460-UVOUT,B-585-OUT;n:type:ShaderForge.SFN_RemapRange,id:585,x:33614,y:35824,varname:node_585,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-7275-OUT;n:type:ShaderForge.SFN_Add,id:6710,x:34273,y:35648,varname:node_6710,prsc:2|A-5598-OUT,B-6040-OUT;n:type:ShaderForge.SFN_Add,id:6879,x:34273,y:35839,varname:node_6879,prsc:2|A-2820-OUT,B-1637-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:931,x:34283,y:35397,ptovrint:False,ptlb:DizzyMask,ptin:_DizzyMask,varname:_DizzyMask,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:9118,x:34854,y:35722,varname:node_9118,prsc:2|A-2519-R,B-3298-R,T-1006-OUT;n:type:ShaderForge.SFN_Multiply,id:5986,x:35563,y:35773,varname:node_5986,prsc:2|A-5817-OUT,B-5232-OUT;n:type:ShaderForge.SFN_Slider,id:5232,x:35237,y:36077,ptovrint:False,ptlb:dizzyMaskMult,ptin:_dizzyMaskMult,varname:_dizzyMaskMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:11;n:type:ShaderForge.SFN_Tex2d,id:3298,x:34528,y:35820,varname:node_25111,prsc:2,ntxv:0,isnm:False|UVIN-6879-OUT,TEX-931-TEX;n:type:ShaderForge.SFN_Vector2,id:3382,x:33895,y:35942,varname:node_3382,prsc:2,v1:0.6,v2:0.6;n:type:ShaderForge.SFN_Add,id:1637,x:34080,y:35901,varname:node_1637,prsc:2|A-6040-OUT,B-3382-OUT;n:type:ShaderForge.SFN_Code,id:5830,x:31740,y:33043,varname:node_5830,prsc:2,code:ZgBsAG8AYQB0ADIAIABjAGUAbgB0AGUAcgAgAD0AIAAoADAALgA1ACwAMAAuADUAKQA7AAoAZgBsAG8AYQB0ADQAIABiAGwAdQByAHIAZQBkACAAPQAgADAAOwAKAGYAbwByACgAaQBuAHQAIABpACAAPQAgADAAOwAgAGkAIAA8ACAAQgBsAHUAcgBTAGEAbQBwAGwAZQBzADsAIABpACsAKwApACAACgB7AA0ACgBmAGwAbwBhAHQAIABzAGMAYQBsAGUAIAA9ACAAQgBsAHUAcgBTAHQAYQByAHQAIAArACAAQgBsAHUAcgBXAGkAZAB0AGgAIAAqACAAKABpAC8AKABCAGwAdQByAFMAYQBtAHAAbABlAHMAIAAtACAAMQApACkAOwAJAAoAYgBsAHUAcgByAGUAZAAgACsAPQAgAHQAZQB4ADIARAAoAF8ATQBhAGkAbgBUAGUAeAAsAFQAUgBBAE4AUwBGAE8AUgBNAF8AVABFAFgAKAAoAFMAYwBlAG4AZQBVAFYAcwAuAHIAZwAtADAALgA1ACkAKgBzAGMAYQBsAGUAIAArACAAYwBlAG4AdABlAHIALAAgAF8ATQBhAGkAbgBUAGUAeAApACkAOwANAAoAfQANAAoAYgBsAHUAcgByAGUAZAAgAC8APQAgAEIAbAB1AHIAUwBhAG0AcABsAGUAcwA7AA0ACgByAGUAdAB1AHIAbgAgAGIAbAB1AHIAcgBlAGQAOwAKAA==,output:3,fname:Function_node_5830,width:847,height:336,input:1,input:0,input:0,input:0,input:12,input_1_label:SceneUVs,input_2_label:BlurStart,input_3_label:BlurWidth,input_4_label:BlurSamples,input_5_label:Scene|A-219-UVOUT,B-4510-OUT,C-7056-OUT,D-9854-OUT,E-4430-TEX;n:type:ShaderForge.SFN_ScreenPos,id:219,x:31210,y:32823,varname:node_219,prsc:2,sctp:2;n:type:ShaderForge.SFN_Slider,id:4510,x:31125,y:33009,ptovrint:False,ptlb:blurStart,ptin:_blurStart,varname:_blurStart,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Slider,id:1349,x:30711,y:33227,ptovrint:False,ptlb:blurWidth,ptin:_blurWidth,varname:_blurWidth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.25,max:3;n:type:ShaderForge.SFN_Slider,id:8834,x:31028,y:33277,ptovrint:False,ptlb:blurSamples,ptin:_blurSamples,varname:_blurSamples,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5,max:24;n:type:ShaderForge.SFN_Tex2d,id:6003,x:35659,y:34786,varname:node_6003,prsc:2,ntxv:0,isnm:False|TEX-4430-TEX;n:type:ShaderForge.SFN_SwitchProperty,id:3487,x:35636,y:35471,ptovrint:False,ptlb:useDizzyColor,ptin:_useDizzyColor,varname:_useDizzyColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-2445-OUT,B-3274-RGB;n:type:ShaderForge.SFN_Vector1,id:8705,x:34179,y:33879,varname:node_8705,prsc:2,v1:1;n:type:ShaderForge.SFN_SwitchProperty,id:1836,x:34419,y:33879,ptovrint:False,ptlb:useVignette,ptin:_useVignette,varname:_useVignette,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8705-OUT,B-1013-OUT;n:type:ShaderForge.SFN_Multiply,id:7056,x:30923,y:33069,varname:node_7056,prsc:2|A-4997-OUT,B-1349-OUT;n:type:ShaderForge.SFN_Multiply,id:672,x:31270,y:33122,varname:node_672,prsc:2|A-4997-OUT,B-8834-OUT;n:type:ShaderForge.SFN_Multiply,id:4760,x:25625,y:38753,varname:node_4760,prsc:2|A-9086-T,B-7597-OUT;n:type:ShaderForge.SFN_Slider,id:5966,x:25034,y:38743,ptovrint:False,ptlb:blackHolesNoisePanX_copy,ptin:_blackHolesNoisePanX_copy,varname:_blackHolesNoisePanX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:-0.0232017,max:2;n:type:ShaderForge.SFN_Slider,id:9181,x:25034,y:38857,ptovrint:False,ptlb:blackHolesNoisePanY_copy,ptin:_blackHolesNoisePanY_copy,varname:_blackHolesNoisePanY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0.2661707,max:5;n:type:ShaderForge.SFN_Append,id:7597,x:25379,y:38775,varname:node_7597,prsc:2|A-5966-OUT,B-9181-OUT;n:type:ShaderForge.SFN_Color,id:4046,x:32444,y:34500,ptovrint:False,ptlb:blackHolesColor,ptin:_blackHolesColor,varname:_blackHolesColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:3703,x:28815,y:35003,ptovrint:False,ptlb:blackHolesMaskMultiplication,ptin:_blackHolesMaskMultiplication,varname:_blackHolesMaskMultiplication,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:111;n:type:ShaderForge.SFN_Slider,id:6342,x:28931,y:35598,ptovrint:False,ptlb:blackHolesMaskErosion,ptin:_blackHolesMaskErosion,varname:_blackHolesMaskErosion,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Tex2dAsset,id:7180,x:27402,y:37405,ptovrint:False,ptlb:BlackHolesMask,ptin:_BlackHolesMask,varname:_BlackHolesMask,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:7435,x:29219,y:35069,varname:node_7435,prsc:2|A-3703-OUT,B-7795-OUT;n:type:ShaderForge.SFN_Subtract,id:201,x:29690,y:35154,varname:node_201,prsc:2|A-1513-OUT,B-5184-OUT;n:type:ShaderForge.SFN_Cos,id:3117,x:28771,y:35417,varname:node_3117,prsc:2|IN-9086-T;n:type:ShaderForge.SFN_RemapRange,id:6081,x:29060,y:35396,varname:node_6081,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-3117-OUT;n:type:ShaderForge.SFN_Multiply,id:1428,x:29296,y:35384,varname:node_1428,prsc:2|A-6342-OUT,B-6081-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:5184,x:29492,y:35384,varname:node_5184,prsc:2,min:0,max:1|IN-1428-OUT;n:type:ShaderForge.SFN_Code,id:2770,x:25319,y:36641,varname:node_2770,prsc:2,code:ZgBsAG8AYQB0ADQAIAB3AG8AcgBsAGQAUABvAHMARgByAG8AbQBTAGMAcgBlAGUAbgA7AAoAZgBsAG8AYQB0ADQAIAB3AG8AcgBsAGQATgBvAHIAbQBhAGwARgByAG8AbQBTAGMAcgBlAGUAbgA7AAoACgBXAG8AcgBsAGQAVQBWAEMAYQBsAGMAdQBsAGEAdABlACgAdwBvAHIAbABkAFAAbwBzAEYAcgBvAG0AUwBjAHIAZQBlAG4ALAAgAHcAbwByAGwAZABOAG8AcgBtAGEAbABGAHIAbwBtAFMAYwByAGUAZQBuACwAdwBvAHIAbABkAFAAbwBzACwAIAB1AHYAKQA7AAoACgByAGUAdAB1AHIAbgAgAHcAbwByAGwAZABOAG8AcgBtAGEAbABGAHIAbwBtAFMAYwByAGUAZQBuADsA,output:3,fname:getWorldNormalFromScreen,width:842,height:249,input:2,input:1,input_1_label:worldPos,input_2_label:uv|A-9589-XYZ,B-579-UVOUT;n:type:ShaderForge.SFN_FragmentPosition,id:9589,x:24701,y:36670,varname:node_9589,prsc:2;n:type:ShaderForge.SFN_Code,id:9496,x:25304,y:37002,varname:node_9496,prsc:2,code:ZgBsAG8AYQB0ADQAIAB3AG8AcgBsAGQAUABvAHMARgByAG8AbQBTAGMAcgBlAGUAbgA7AAoAZgBsAG8AYQB0ADQAIAB3AG8AcgBsAGQATgBvAHIAbQBhAGwARgByAG8AbQBTAGMAcgBlAGUAbgA7AAoACgBXAG8AcgBsAGQAVQBWAEMAYQBsAGMAdQBsAGEAdABlACgAdwBvAHIAbABkAFAAbwBzAEYAcgBvAG0AUwBjAHIAZQBlAG4ALAAgAHcAbwByAGwAZABOAG8AcgBtAGEAbABGAHIAbwBtAFMAYwByAGUAZQBuACwAdwBvAHIAbABkAFAAbwBzACwAIAB1AHYAKQA7AAoACgByAGUAdAB1AHIAbgAgAHcAbwByAGwAZABQAG8AcwBGAHIAbwBtAFMAYwByAGUAZQBuADsA,output:3,fname:getWorldPosFromScreen,width:834,height:249,input:2,input:1,input_1_label:worldPos,input_2_label:uv|A-9589-XYZ,B-579-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:1818,x:27602,y:36627,varname:_node_9932,prsc:2,ntxv:0,isnm:False|UVIN-734-OUT,TEX-7180-TEX;n:type:ShaderForge.SFN_Tex2d,id:9072,x:27640,y:36908,varname:_node_7496,prsc:2,ntxv:0,isnm:False|UVIN-2078-OUT,TEX-7180-TEX;n:type:ShaderForge.SFN_Tex2d,id:3724,x:27619,y:37175,varname:_node_1597,prsc:2,ntxv:0,isnm:False|UVIN-968-OUT,TEX-7180-TEX;n:type:ShaderForge.SFN_Divide,id:4179,x:26441,y:36703,varname:node_4179,prsc:2|A-9496-OUT,B-2340-OUT;n:type:ShaderForge.SFN_ComponentMask,id:2245,x:26784,y:36691,varname:node_2245,prsc:2,cc1:0,cc2:2,cc3:-1,cc4:-1|IN-4179-OUT;n:type:ShaderForge.SFN_ComponentMask,id:9038,x:26291,y:36983,varname:node_9038,prsc:2,cc1:2,cc2:-1,cc3:-1,cc4:-1|IN-2770-OUT;n:type:ShaderForge.SFN_Lerp,id:6925,x:27971,y:36814,varname:node_6925,prsc:2|A-1818-RGB,B-9072-RGB,T-743-OUT;n:type:ShaderForge.SFN_ComponentMask,id:6426,x:26784,y:36915,varname:node_6426,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-4179-OUT;n:type:ShaderForge.SFN_Lerp,id:7795,x:28192,y:36958,varname:node_7795,prsc:2|A-6925-OUT,B-3724-RGB,T-3714-OUT;n:type:ShaderForge.SFN_ComponentMask,id:6417,x:26771,y:37132,varname:node_6417,prsc:2,cc1:1,cc2:2,cc3:-1,cc4:-1|IN-4179-OUT;n:type:ShaderForge.SFN_ComponentMask,id:1160,x:26291,y:37176,varname:node_1160,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-2770-OUT;n:type:ShaderForge.SFN_Abs,id:743,x:26497,y:36983,varname:node_743,prsc:2|IN-9038-OUT;n:type:ShaderForge.SFN_Abs,id:3714,x:26497,y:37176,varname:node_3714,prsc:2|IN-1160-OUT;n:type:ShaderForge.SFN_Slider,id:2340,x:26314,y:36463,ptovrint:False,ptlb:worldTiler,ptin:_worldTiler,varname:_worldTiler,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5000,cur:1,max:5000;n:type:ShaderForge.SFN_ScreenPos,id:579,x:24714,y:36946,varname:node_579,prsc:2,sctp:2;n:type:ShaderForge.SFN_Clamp01,id:2622,x:31617,y:34848,varname:node_2622,prsc:2|IN-201-OUT;n:type:ShaderForge.SFN_Tex2d,id:9509,x:26178,y:37916,varname:_node_5913,prsc:2,ntxv:0,isnm:False|UVIN-9171-OUT,TEX-2611-TEX;n:type:ShaderForge.SFN_Tex2d,id:3297,x:26201,y:38160,varname:_node_7043,prsc:2,ntxv:0,isnm:False|UVIN-503-OUT,TEX-2611-TEX;n:type:ShaderForge.SFN_Tex2d,id:3625,x:26208,y:38414,varname:_node_7097,prsc:2,ntxv:0,isnm:False|UVIN-2839-OUT,TEX-2611-TEX;n:type:ShaderForge.SFN_ComponentMask,id:7280,x:25665,y:37882,varname:node_7280,prsc:2,cc1:0,cc2:2,cc3:-1,cc4:-1|IN-1797-OUT;n:type:ShaderForge.SFN_ComponentMask,id:1537,x:25679,y:38150,varname:node_1537,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-1797-OUT;n:type:ShaderForge.SFN_ComponentMask,id:3186,x:25674,y:38419,varname:node_3186,prsc:2,cc1:1,cc2:2,cc3:-1,cc4:-1|IN-1797-OUT;n:type:ShaderForge.SFN_Append,id:9073,x:26407,y:37932,varname:node_9073,prsc:2|A-9509-R,B-9509-R;n:type:ShaderForge.SFN_Append,id:8454,x:26429,y:38204,varname:node_8454,prsc:2|A-3297-R,B-3297-R;n:type:ShaderForge.SFN_Append,id:4906,x:26429,y:38414,varname:node_4906,prsc:2|A-3625-R,B-3625-R;n:type:ShaderForge.SFN_Add,id:734,x:27364,y:36700,varname:node_734,prsc:2|A-1498-OUT,B-1599-OUT;n:type:ShaderForge.SFN_Add,id:2078,x:27364,y:36924,varname:node_2078,prsc:2|A-4698-OUT,B-7313-OUT;n:type:ShaderForge.SFN_Add,id:968,x:27390,y:37175,varname:node_968,prsc:2|A-2708-OUT,B-81-OUT;n:type:ShaderForge.SFN_Divide,id:1797,x:25400,y:37635,varname:node_1797,prsc:2|A-9496-OUT,B-6443-OUT;n:type:ShaderForge.SFN_Slider,id:6443,x:25026,y:37707,ptovrint:False,ptlb:distWorldTiler,ptin:_distWorldTiler,varname:_distWorldTiler,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-50,cur:1,max:50;n:type:ShaderForge.SFN_Tex2dAsset,id:2611,x:25112,y:38095,ptovrint:False,ptlb:allBlacks,ptin:_allBlacks,varname:_allBlacks,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:9171,x:25919,y:37882,varname:node_9171,prsc:2|A-4760-OUT,B-7280-OUT;n:type:ShaderForge.SFN_Add,id:503,x:25923,y:38150,varname:node_503,prsc:2|A-4760-OUT,B-1537-OUT;n:type:ShaderForge.SFN_Add,id:2839,x:25923,y:38392,varname:node_2839,prsc:2|A-4760-OUT,B-3186-OUT;n:type:ShaderForge.SFN_Multiply,id:1599,x:26657,y:37932,varname:node_1599,prsc:2|A-9073-OUT,B-5353-OUT;n:type:ShaderForge.SFN_Multiply,id:7313,x:26657,y:38181,varname:node_7313,prsc:2|A-8454-OUT,B-5353-OUT;n:type:ShaderForge.SFN_Multiply,id:81,x:26657,y:38402,varname:node_81,prsc:2|A-4906-OUT,B-5353-OUT;n:type:ShaderForge.SFN_Slider,id:5353,x:26279,y:38726,ptovrint:False,ptlb:distortionIntensity,ptin:_distortionIntensity,varname:_distortionIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:6608,x:32707,y:34603,varname:node_6608,prsc:2|A-5830-OUT,B-4046-RGB,T-2622-OUT;n:type:ShaderForge.SFN_Clamp01,id:1513,x:29423,y:35076,varname:node_1513,prsc:2|IN-7435-OUT;n:type:ShaderForge.SFN_Add,id:1498,x:26981,y:36661,varname:node_1498,prsc:2|A-2245-OUT,B-7714-OUT;n:type:ShaderForge.SFN_Add,id:4698,x:27041,y:36908,varname:node_4698,prsc:2|A-6426-OUT,B-7714-OUT;n:type:ShaderForge.SFN_Add,id:2708,x:27041,y:37135,varname:node_2708,prsc:2|A-6417-OUT,B-7714-OUT;n:type:ShaderForge.SFN_Slider,id:8597,x:26399,y:37672,ptovrint:False,ptlb:blackHoleSpeed,ptin:_blackHoleSpeed,varname:_blackHoleSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Time,id:285,x:26510,y:37464,varname:node_285,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7714,x:26795,y:37544,varname:node_7714,prsc:2|A-285-T,B-8597-OUT;n:type:ShaderForge.SFN_Slider,id:1547,x:30334,y:33020,ptovrint:False,ptlb:blurCtrl,ptin:_blurCtrl,varname:node_1547,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_ConstantClamp,id:9854,x:31511,y:33206,varname:node_9854,prsc:2,min:2,max:8|IN-672-OUT;n:type:ShaderForge.SFN_Vector1,id:9479,x:25611,y:38648,varname:node_9479,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:5817,x:35309,y:35751,varname:node_5817,prsc:2|A-9118-OUT,B-5973-OUT;n:type:ShaderForge.SFN_Clamp01,id:5973,x:34690,y:34977,varname:node_5973,prsc:2|IN-1013-OUT;n:type:ShaderForge.SFN_Clamp01,id:1012,x:35780,y:35753,varname:node_1012,prsc:2|IN-5986-OUT;n:type:ShaderForge.SFN_Smoothstep,id:8619,x:33912,y:34113,varname:node_8619,prsc:2|A-5709-OUT,B-631-OUT,V-9709-OUT;n:type:ShaderForge.SFN_RemapRange,id:8317,x:33385,y:34212,varname:node_8317,prsc:2,frmn:0,frmx:1,tomn:-0.5,tomx:0.5|IN-5590-UVOUT;n:type:ShaderForge.SFN_ScreenPos,id:5590,x:33210,y:34220,varname:node_5590,prsc:2,sctp:2;n:type:ShaderForge.SFN_Vector1,id:631,x:33622,y:34138,varname:node_631,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:8584,x:33138,y:33923,ptovrint:False,ptlb:vignette_copy,ptin:_vignette_copy,varname:_vignette_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Power,id:1170,x:33439,y:33982,varname:node_1170,prsc:2|VAL-8584-OUT,EXP-6407-OUT;n:type:ShaderForge.SFN_Vector1,id:6407,x:33275,y:34014,varname:node_6407,prsc:2,v1:0.4;n:type:ShaderForge.SFN_OneMinus,id:5709,x:33607,y:33982,varname:node_5709,prsc:2|IN-1170-OUT;n:type:ShaderForge.SFN_Length,id:9709,x:33607,y:34212,varname:node_9709,prsc:2|IN-8317-OUT;n:type:ShaderForge.SFN_Multiply,id:1013,x:34065,y:33990,varname:node_1013,prsc:2|A-9572-OUT,B-8619-OUT;n:type:ShaderForge.SFN_Vector1,id:9572,x:33887,y:33968,varname:node_9572,prsc:2,v1:1.25;n:type:ShaderForge.SFN_Lerp,id:5309,x:35381,y:35496,varname:node_5309,prsc:2|A-4413-OUT,B-3308-OUT,T-1012-OUT;n:type:ShaderForge.SFN_Vector1,id:4413,x:35112,y:35370,varname:node_4413,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:3308,x:35114,y:35520,varname:node_3308,prsc:2,v1:0;proporder:4430-4890-3369-7684-5335-3073-9403-9761-8697-4759-1175-323-2632-3487-931-3274-5232-7785-4510-1349-8834-9197-1836-4046-3703-6342-7180-5966-9181-2340-6443-2611-5353-8597-8584;pass:END;sub:END;*/

Shader "Colony_FX/PostProcess/S_PP_Overheat_Extended" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _blackHolesNoise ("blackHolesNoise", 2D) = "white" {}
        _blackHolesNoiseTile ("blackHolesNoiseTile", Range(0, 15)) = 1
        _blackHolesNoisePanX ("blackHolesNoisePanX", Range(-2, 2)) = -0.0232017
        _blackHolesNoisePanY ("blackHolesNoisePanY", Range(-5, 5)) = 0.2661707
        _blackHolesDistortion ("blackHolesDistortion", Range(0, 3)) = 0.8281516
        _sceneMultiplicationR ("sceneMultiplicationR", Range(0, 15)) = 1
        _sceneMultiplicationG ("sceneMultiplicationG", Range(0, 15)) = 1
        _sceneMultiplicationB ("sceneMultiplicationB", Range(0, 15)) = 1
        _scenePow ("scenePow", Range(0, 11)) = 1
        _sceneDesaturation ("sceneDesaturation", Range(0, 1)) = 0
        _sceneMultiplicationScalar ("sceneMultiplicationScalar", Range(0, 1)) = 0
        _sceneColorization ("sceneColorization", Range(0, 1)) = 0
        [MaterialToggle] _useDizzyColor ("useDizzyColor", Float ) = 0.5
        _DizzyMask ("DizzyMask", 2D) = "white" {}
        _DizzyColor ("DizzyColor", Color) = (0.5,0.5,0.5,1)
        _dizzyMaskMult ("dizzyMaskMult", Range(0, 11)) = 0
        _frequency ("frequency", Range(0, 7)) = 0.8914911
        _blurStart ("blurStart", Range(0, 2)) = 1
        _blurWidth ("blurWidth", Range(0, 3)) = 0.25
        _blurSamples ("blurSamples", Range(0, 24)) = 5
        _finalMix ("finalMix", Range(0, 1)) = 0
        [MaterialToggle] _useVignette ("useVignette", Float ) = 1
        _blackHolesColor ("blackHolesColor", Color) = (0.5,0.5,0.5,1)
        _blackHolesMaskMultiplication ("blackHolesMaskMultiplication", Range(0, 111)) = 0
        _blackHolesMaskErosion ("blackHolesMaskErosion", Range(0, 1)) = 0
        _BlackHolesMask ("BlackHolesMask", 2D) = "white" {}
        _blackHolesNoisePanX_copy ("blackHolesNoisePanX_copy", Range(-2, 2)) = -0.0232017
        _blackHolesNoisePanY_copy ("blackHolesNoisePanY_copy", Range(-5, 5)) = 0.2661707
        _worldTiler ("worldTiler", Range(-5000, 5000)) = 1
        _distWorldTiler ("distWorldTiler", Range(-50, 50)) = 1
        _allBlacks ("allBlacks", 2D) = "white" {}
        _distortionIntensity ("distortionIntensity", Range(0, 1)) = 0
        _blackHoleSpeed ("blackHoleSpeed", Range(0, 15)) = 1
        _vignette_copy ("vignette_copy", Range(0, 2)) = 0
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
            #include "WorldUV.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _blackHolesNoise; uniform float4 _blackHolesNoise_ST;
            uniform float _blackHolesNoisePanX;
            uniform float _blackHolesNoisePanY;
            uniform float _blackHolesNoiseTile;
            uniform float _blackHolesDistortion;
            uniform float _sceneDesaturation;
            uniform float _finalMix;
            uniform float _sceneMultiplicationR;
            uniform float _sceneMultiplicationG;
            uniform float _sceneMultiplicationB;
            uniform float _scenePow;
            uniform float _sceneMultiplicationScalar;
            uniform float _frequency;
            uniform float _sceneColorization;
            uniform float4 _DizzyColor;
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
            uniform fixed _useDizzyColor;
            uniform fixed _useVignette;
            uniform float _blackHolesNoisePanX_copy;
            uniform float _blackHolesNoisePanY_copy;
            uniform float4 _blackHolesColor;
            uniform float _blackHolesMaskMultiplication;
            uniform float _blackHolesMaskErosion;
            uniform sampler2D _BlackHolesMask; uniform float4 _BlackHolesMask_ST;
            float4 getWorldNormalFromScreen( float3 worldPos , float2 uv ){
            float4 worldPosFromScreen;
            float4 worldNormalFromScreen;
            
            WorldUVCalculate(worldPosFromScreen, worldNormalFromScreen,worldPos, uv);
            
            return worldNormalFromScreen;
            }
            
            float4 getWorldPosFromScreen( float3 worldPos , float2 uv ){
            float4 worldPosFromScreen;
            float4 worldNormalFromScreen;
            
            WorldUVCalculate(worldPosFromScreen, worldNormalFromScreen,worldPos, uv);
            
            return worldPosFromScreen;
            }
            
            uniform float _worldTiler;
            uniform float _distWorldTiler;
            uniform sampler2D _allBlacks; uniform float4 _allBlacks_ST;
            uniform float _distortionIntensity;
            uniform float _blackHoleSpeed;
            uniform float _vignette_copy;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 projPos : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
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
                float node_3048 = 1.0;
                float4 node_418 = _Time;
                float node_1224 = sin((node_418.g*_frequency));
                float node_4997 = distance(node_1224,cos((node_1224*(3.141592654*0.7))));
                float4 node_9496 = getWorldPosFromScreen( i.posWorld.rgb , sceneUVs.rg );
                float4 node_4179 = (node_9496/_worldTiler);
                float4 node_285 = _Time;
                float node_7714 = (node_285.g*_blackHoleSpeed);
                float4 node_9086 = _Time;
                float2 node_4760 = (node_9086.g*float2(_blackHolesNoisePanX_copy,_blackHolesNoisePanY_copy));
                float4 node_1797 = (node_9496/_distWorldTiler);
                float2 node_9171 = (node_4760+node_1797.rb);
                float4 _node_5913 = tex2D(_allBlacks,TRANSFORM_TEX(node_9171, _allBlacks));
                float2 node_734 = ((node_4179.rb+node_7714)+(float2(_node_5913.r,_node_5913.r)*_distortionIntensity));
                float4 _node_9932 = tex2D(_BlackHolesMask,TRANSFORM_TEX(node_734, _BlackHolesMask));
                float2 node_503 = (node_4760+node_1797.rg);
                float4 _node_7043 = tex2D(_allBlacks,TRANSFORM_TEX(node_503, _allBlacks));
                float2 node_2078 = ((node_4179.rg+node_7714)+(float2(_node_7043.r,_node_7043.r)*_distortionIntensity));
                float4 _node_7496 = tex2D(_BlackHolesMask,TRANSFORM_TEX(node_2078, _BlackHolesMask));
                float4 node_2770 = getWorldNormalFromScreen( i.posWorld.rgb , sceneUVs.rg );
                float2 node_2839 = (node_4760+node_1797.gb);
                float4 _node_7097 = tex2D(_allBlacks,TRANSFORM_TEX(node_2839, _allBlacks));
                float2 node_968 = ((node_4179.gb+node_7714)+(float2(_node_7097.r,_node_7097.r)*_distortionIntensity));
                float4 _node_1597 = tex2D(_BlackHolesMask,TRANSFORM_TEX(node_968, _BlackHolesMask));
                float node_1013 = (1.25*smoothstep( (1.0 - pow(_vignette_copy,0.4)), 1.0, length((sceneUVs.rg*1.0+-0.5)) ));
                float3 node_2445 = (pow((lerp(float3(node_3048,node_3048,node_3048),float3((_sceneMultiplicationR*_sceneMultiplicationScalar),(_sceneMultiplicationG*_sceneMultiplicationScalar),(_sceneMultiplicationB*_sceneMultiplicationScalar)),_sceneColorization)*lerp(lerp(Function_node_5830( sceneUVs.rg , _blurStart , (node_4997*_blurWidth) , clamp((node_4997*_blurSamples),2,8) , _MainTex ),float4(_blackHolesColor.rgb,0.0),saturate((saturate((_blackHolesMaskMultiplication*lerp(lerp(_node_9932.rgb,_node_7496.rgb,abs(node_2770.b)),_node_1597.rgb,abs(node_2770.r))))-clamp((_blackHolesMaskErosion*(cos(node_9086.g)*0.5+0.5)),0,1)))),dot(lerp(Function_node_5830( sceneUVs.rg , _blurStart , (node_4997*_blurWidth) , clamp((node_4997*_blurSamples),2,8) , _MainTex ),float4(_blackHolesColor.rgb,0.0),saturate((saturate((_blackHolesMaskMultiplication*lerp(lerp(_node_9932.rgb,_node_7496.rgb,abs(node_2770.b)),_node_1597.rgb,abs(node_2770.r))))-clamp((_blackHolesMaskErosion*(cos(node_9086.g)*0.5+0.5)),0,1)))),float3(0.3,0.59,0.11)),_sceneDesaturation)),_scenePow)*lerp( 1.0, node_1013, _useVignette ));
                float node_7690 = 3.141592654;
                float node_4643 = (node_7690*4.0);
                float node_7927 = 0.5;
                float2 node_7436 = (frac((node_9086.g*float2(_blackHolesNoisePanX,_blackHolesNoisePanY)))+(i.uv0*_blackHolesNoiseTile));
                float4 _blackHolesNoise_var = tex2D(_blackHolesNoise,TRANSFORM_TEX(node_7436, _blackHolesNoise));
                float2 node_6040 = (i.uv0+((float2(_blackHolesNoise_var.r,_blackHolesNoise_var.r)*_blackHolesDistortion)*2.0+-1.0));
                float2 node_6710 = ((trunc((((3.0*node_7690)+node_9086.g)/node_4643))+node_7927)+node_6040);
                float4 node_2519 = tex2D(_DizzyMask,TRANSFORM_TEX(node_6710, _DizzyMask));
                float2 node_6879 = ((trunc(((node_9086.g+node_7690)/node_4643))+node_7927)+(node_6040+float2(0.6,0.6)));
                float4 node_25111 = tex2D(_DizzyMask,TRANSFORM_TEX(node_6879, _DizzyMask));
                float node_1012 = saturate(((lerp(node_2519.r,node_25111.r,(sin((node_9086.g*0.5))*0.5+0.5))*saturate(node_1013))*_dizzyMaskMult));
                float3 emissive = lerp(node_6003.rgb,lerp(node_2445,lerp( node_2445, _DizzyColor.rgb, _useDizzyColor ),node_1012),_finalMix);
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
