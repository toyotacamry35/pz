// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:6,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1292886,fgcg:0.6848478,fgcb:0.7322265,fgca:1,fgde:0.004151576,fgrn:0,fgrf:600,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:38512,y:33527,varname:node_2865,prsc:2|emission-5288-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:34309,y:33459,ptovrint:False,ptlb:MainTex,ptin:_MainTex,cmnt:MainTex contains the color of the scene,varname:_MainTex,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:3687,x:34515,y:33340,varname:node_3687,prsc:2,ntxv:0,isnm:False|UVIN-8665-OUT,TEX-4430-TEX;n:type:ShaderForge.SFN_Slider,id:9197,x:34123,y:33966,ptovrint:False,ptlb:finalMix,ptin:_finalMix,varname:_finalMix,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Smoothstep,id:5092,x:33994,y:34285,varname:node_5092,prsc:2|A-5289-OUT,B-6529-OUT,V-543-OUT;n:type:ShaderForge.SFN_RemapRange,id:5935,x:33544,y:34505,varname:node_5935,prsc:2,frmn:0,frmx:1,tomn:-0.5,tomx:0.5|IN-6890-OUT;n:type:ShaderForge.SFN_ScreenPos,id:1510,x:32934,y:34504,varname:node_1510,prsc:2,sctp:2;n:type:ShaderForge.SFN_Vector1,id:6529,x:33690,y:34319,varname:node_6529,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:6447,x:32506,y:34084,ptovrint:False,ptlb:mainVignette,ptin:_mainVignette,varname:_vignette,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.15,max:2;n:type:ShaderForge.SFN_Power,id:3424,x:33507,y:34163,varname:node_3424,prsc:2|VAL-1119-OUT,EXP-1444-OUT;n:type:ShaderForge.SFN_Vector1,id:1444,x:33343,y:34195,varname:node_1444,prsc:2,v1:0.4;n:type:ShaderForge.SFN_OneMinus,id:5289,x:33675,y:34163,varname:node_5289,prsc:2|IN-3424-OUT;n:type:ShaderForge.SFN_Length,id:543,x:33726,y:34491,varname:node_543,prsc:2|IN-5935-OUT;n:type:ShaderForge.SFN_Lerp,id:4413,x:36144,y:33401,varname:node_4413,prsc:2|A-3687-RGB,B-8597-OUT,T-6426-OUT;n:type:ShaderForge.SFN_Multiply,id:7716,x:34238,y:34220,varname:node_7716,prsc:2|A-1939-OUT,B-5092-OUT;n:type:ShaderForge.SFN_Vector1,id:1939,x:33955,y:34149,varname:node_1939,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Slider,id:2799,x:35416,y:33664,ptovrint:False,ptlb:vignetteDarken,ptin:_vignetteDarken,varname:_vignetteDarken,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_OneMinus,id:5658,x:35704,y:33648,varname:node_5658,prsc:2|IN-2799-OUT;n:type:ShaderForge.SFN_Color,id:8991,x:37014,y:33570,ptovrint:False,ptlb:stainsColor,ptin:_stainsColor,varname:_stainsColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:3221,x:37043,y:34197,varname:node_3221,prsc:2|A-8454-OUT,B-157-OUT;n:type:ShaderForge.SFN_Slider,id:157,x:36660,y:34358,ptovrint:False,ptlb:stainsMult,ptin:_stainsMult,varname:_stainsMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:5;n:type:ShaderForge.SFN_Time,id:8969,x:31831,y:35479,varname:node_8969,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:6459,x:33077,y:35791,ptovrint:False,ptlb:stainsDistorter,ptin:_stainsDistorter,varname:_stainsDistorter,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:edecc509e768569459e378283355ae30,ntxv:0,isnm:False|UVIN-9101-OUT;n:type:ShaderForge.SFN_Multiply,id:2816,x:32517,y:35704,varname:node_2816,prsc:2|A-8969-T,B-2069-OUT;n:type:ShaderForge.SFN_Append,id:2069,x:32235,y:35736,varname:node_2069,prsc:2|A-1164-OUT,B-2122-OUT;n:type:ShaderForge.SFN_Frac,id:1868,x:32693,y:35704,varname:node_1868,prsc:2|IN-2816-OUT;n:type:ShaderForge.SFN_TexCoord,id:1003,x:32422,y:35958,varname:node_1003,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:9101,x:32900,y:35842,varname:node_9101,prsc:2|A-1868-OUT,B-4080-OUT;n:type:ShaderForge.SFN_Multiply,id:4080,x:32668,y:35958,varname:node_4080,prsc:2|A-1003-UVOUT,B-7520-OUT;n:type:ShaderForge.SFN_TexCoord,id:2808,x:33410,y:35454,varname:node_2808,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Lerp,id:3043,x:34519,y:35666,varname:node_3043,prsc:2|A-8694-OUT,B-6782-OUT,T-7301-OUT;n:type:ShaderForge.SFN_Add,id:6782,x:34168,y:35751,varname:node_6782,prsc:2|A-8694-OUT,B-6459-R;n:type:ShaderForge.SFN_Slider,id:7301,x:34150,y:35988,ptovrint:False,ptlb:stainsDistortion,ptin:_stainsDistortion,varname:_stainsDistortion,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3,max:1;n:type:ShaderForge.SFN_Slider,id:1164,x:31772,y:35722,ptovrint:False,ptlb:stainsDistorterPanX,ptin:_stainsDistorterPanX,varname:_stainsDistorterPanX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:2122,x:31727,y:35886,ptovrint:False,ptlb:stainsDistorterPanY,ptin:_stainsDistorterPanY,varname:_stainsDistorterPanY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:7520,x:32339,y:36225,ptovrint:False,ptlb:stainsDistorterTile,ptin:_stainsDistorterTile,varname:_stainsDistorterTile,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:4;n:type:ShaderForge.SFN_Lerp,id:4179,x:37443,y:33651,varname:node_4179,prsc:2|A-8991-RGB,B-2245-RGB,T-5317-OUT;n:type:ShaderForge.SFN_Color,id:2245,x:37014,y:33747,ptovrint:False,ptlb:stainsColor_copy,ptin:_stainsColor_copy,varname:_stainsColor_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:9867,x:37383,y:34170,varname:node_9867,prsc:2|A-9197-OUT,B-3221-OUT;n:type:ShaderForge.SFN_Multiply,id:6426,x:35926,y:33890,varname:node_6426,prsc:2|A-9197-OUT,B-7716-OUT;n:type:ShaderForge.SFN_Multiply,id:579,x:36963,y:33964,varname:node_579,prsc:2|A-7979-OUT,B-3221-OUT;n:type:ShaderForge.SFN_Vector1,id:7979,x:36758,y:33964,varname:node_7979,prsc:2,v1:3;n:type:ShaderForge.SFN_Clamp01,id:5317,x:37117,y:33964,varname:node_5317,prsc:2|IN-579-OUT;n:type:ShaderForge.SFN_Lerp,id:3072,x:37181,y:35834,varname:node_3072,prsc:2|A-8504-OUT,B-2720-OUT,T-2157-OUT;n:type:ShaderForge.SFN_Power,id:7125,x:36541,y:35532,varname:node_7125,prsc:2|VAL-8606-OUT,EXP-9509-OUT;n:type:ShaderForge.SFN_Slider,id:9509,x:36289,y:35774,ptovrint:False,ptlb:stainsFalloff,ptin:_stainsFalloff,varname:_stainsFalloff,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3,max:5;n:type:ShaderForge.SFN_Multiply,id:8504,x:36836,y:35531,varname:node_8504,prsc:2|A-7125-OUT,B-3297-OUT;n:type:ShaderForge.SFN_Slider,id:3297,x:36651,y:35793,ptovrint:False,ptlb:stainsFalloffBrightness,ptin:_stainsFalloffBrightness,varname:_stainsFalloffBrightness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:4;n:type:ShaderForge.SFN_Multiply,id:8454,x:37370,y:35059,varname:node_8454,prsc:2|A-7716-OUT,B-3072-OUT;n:type:ShaderForge.SFN_OneMinus,id:8606,x:36275,y:35534,varname:node_8606,prsc:2|IN-2720-OUT;n:type:ShaderForge.SFN_Lerp,id:5288,x:37900,y:33635,varname:node_5288,prsc:2|A-4413-OUT,B-4179-OUT,T-3382-OUT;n:type:ShaderForge.SFN_Multiply,id:8597,x:35858,y:33481,varname:node_8597,prsc:2|A-3687-RGB,B-5658-OUT;n:type:ShaderForge.SFN_Power,id:2157,x:36730,y:36224,varname:node_2157,prsc:2|VAL-9875-G,EXP-8896-OUT;n:type:ShaderForge.SFN_Slider,id:8896,x:36399,y:36391,ptovrint:False,ptlb:edgeMaskPow,ptin:_edgeMaskPow,varname:node_8896,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Sin,id:1322,x:34449,y:37261,varname:node_1322,prsc:2|IN-6668-OUT;n:type:ShaderForge.SFN_RemapRange,id:7056,x:34637,y:37261,varname:node_7056,prsc:2,frmn:-1,frmx:1,tomn:0,tomx:1|IN-1322-OUT;n:type:ShaderForge.SFN_Trunc,id:3321,x:34374,y:36894,varname:node_3321,prsc:2|IN-6389-OUT;n:type:ShaderForge.SFN_Pi,id:3523,x:33730,y:37335,varname:node_3523,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4340,x:33879,y:37380,varname:node_4340,prsc:2|A-3523-OUT,B-4624-OUT;n:type:ShaderForge.SFN_Vector1,id:4624,x:33697,y:37470,varname:node_4624,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:7294,x:33575,y:36884,varname:node_7294,prsc:2|A-7987-OUT,B-3712-T;n:type:ShaderForge.SFN_Trunc,id:9157,x:34374,y:36736,varname:node_9157,prsc:2|IN-2082-OUT;n:type:ShaderForge.SFN_Time,id:3712,x:32948,y:36738,varname:node_3712,prsc:2;n:type:ShaderForge.SFN_Tex2dAsset,id:2236,x:34563,y:35899,ptovrint:False,ptlb:stainsPattern,ptin:_stainsPattern,varname:node_2236,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8258,x:35074,y:35909,varname:node_8258,prsc:2,ntxv:0,isnm:False|UVIN-3975-OUT,TEX-2236-TEX;n:type:ShaderForge.SFN_Add,id:1597,x:34835,y:35681,varname:node_1597,prsc:2|A-3043-OUT,B-652-OUT;n:type:ShaderForge.SFN_Add,id:3975,x:34842,y:35989,varname:node_3975,prsc:2|A-3043-OUT,B-556-OUT;n:type:ShaderForge.SFN_Lerp,id:2720,x:35480,y:35850,varname:node_2720,prsc:2|A-4321-R,B-8258-R,T-7056-OUT;n:type:ShaderForge.SFN_Tex2d,id:4321,x:35074,y:35681,varname:node_4321,prsc:2,ntxv:0,isnm:False|UVIN-1597-OUT,TEX-2236-TEX;n:type:ShaderForge.SFN_Slider,id:7987,x:33212,y:36669,ptovrint:False,ptlb:swapFrequency,ptin:_swapFrequency,varname:node_7987,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:8;n:type:ShaderForge.SFN_Multiply,id:2082,x:33904,y:36732,varname:node_2082,prsc:2|A-8144-OUT,B-7294-OUT;n:type:ShaderForge.SFN_Vector1,id:8144,x:33698,y:36603,varname:node_8144,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Add,id:6389,x:34173,y:36894,varname:node_6389,prsc:2|A-2082-OUT,B-8412-OUT;n:type:ShaderForge.SFN_Vector1,id:8412,x:33972,y:36946,varname:node_8412,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:5781,x:34060,y:37261,varname:node_5781,prsc:2|A-7294-OUT,B-4340-OUT;n:type:ShaderForge.SFN_Add,id:6668,x:34254,y:37261,varname:node_6668,prsc:2|A-5781-OUT,B-4340-OUT;n:type:ShaderForge.SFN_Multiply,id:652,x:34611,y:36736,varname:node_652,prsc:2|A-9157-OUT,B-2968-OUT;n:type:ShaderForge.SFN_Multiply,id:556,x:34601,y:36907,varname:node_556,prsc:2|A-3321-OUT,B-2968-OUT;n:type:ShaderForge.SFN_Vector1,id:2968,x:34493,y:37087,varname:node_2968,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Time,id:9237,x:31963,y:34209,varname:node_9237,prsc:2;n:type:ShaderForge.SFN_Multiply,id:198,x:32170,y:34312,varname:node_198,prsc:2|A-9237-T,B-9553-OUT;n:type:ShaderForge.SFN_Slider,id:9553,x:31810,y:34418,ptovrint:False,ptlb:vignetteFreq,ptin:_vignetteFreq,varname:node_9553,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.3576384,max:3;n:type:ShaderForge.SFN_Sin,id:7839,x:32358,y:34312,varname:node_7839,prsc:2|IN-198-OUT;n:type:ShaderForge.SFN_Multiply,id:2444,x:32569,y:34337,varname:node_2444,prsc:2|A-7839-OUT,B-8848-OUT;n:type:ShaderForge.SFN_Vector1,id:8848,x:32345,y:34487,varname:node_8848,prsc:2,v1:0.01;n:type:ShaderForge.SFN_Add,id:3154,x:32873,y:34106,varname:node_3154,prsc:2|A-6447-OUT,B-2444-OUT;n:type:ShaderForge.SFN_Power,id:8705,x:37733,y:34177,varname:node_8705,prsc:2|VAL-9867-OUT,EXP-8734-OUT;n:type:ShaderForge.SFN_Slider,id:8734,x:37470,y:34404,ptovrint:False,ptlb:stainsPow,ptin:_stainsPow,varname:node_8734,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10;n:type:ShaderForge.SFN_Add,id:2963,x:33161,y:34539,varname:node_2963,prsc:2|A-1510-UVOUT,B-3473-R;n:type:ShaderForge.SFN_Lerp,id:6890,x:33367,y:34505,varname:node_6890,prsc:2|A-1510-UVOUT,B-2963-OUT,T-6842-OUT;n:type:ShaderForge.SFN_Slider,id:6842,x:33123,y:34736,ptovrint:False,ptlb:distVign,ptin:_distVign,varname:node_6842,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.3;n:type:ShaderForge.SFN_Clamp01,id:3382,x:37992,y:34167,varname:node_3382,prsc:2|IN-8705-OUT;n:type:ShaderForge.SFN_Tex2d,id:9875,x:36399,y:36188,varname:node_9875,prsc:2,ntxv:0,isnm:False|TEX-2236-TEX;n:type:ShaderForge.SFN_Multiply,id:8694,x:33937,y:35502,varname:node_8694,prsc:2|A-5395-OUT,B-8721-OUT;n:type:ShaderForge.SFN_Vector2,id:8721,x:33926,y:35681,varname:node_8721,prsc:2,v1:1,v2:0.5;n:type:ShaderForge.SFN_Multiply,id:5395,x:33664,y:35501,varname:node_5395,prsc:2|A-2808-UVOUT,B-6252-OUT;n:type:ShaderForge.SFN_Slider,id:6252,x:33367,y:35667,ptovrint:False,ptlb:stainsPatternTiling,ptin:_stainsPatternTiling,varname:node_6252,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:4;n:type:ShaderForge.SFN_Multiply,id:1119,x:33091,y:34125,varname:node_1119,prsc:2|A-3154-OUT,B-260-OUT;n:type:ShaderForge.SFN_Slider,id:5139,x:32506,y:33946,ptovrint:False,ptlb:statVignetteScale,ptin:_statVignetteScale,varname:node_5139,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.5,cur:0.5,max:1;n:type:ShaderForge.SFN_Multiply,id:260,x:32908,y:33892,varname:node_260,prsc:2|A-1236-OUT,B-5139-OUT;n:type:ShaderForge.SFN_Vector1,id:1236,x:32605,y:33801,varname:node_1236,prsc:2,v1:2;n:type:ShaderForge.SFN_Tex2d,id:3473,x:32776,y:34703,ptovrint:False,ptlb:vignetteDistortionTexture,ptin:_vignetteDistortionTexture,varname:node_3473,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ScreenPos,id:9479,x:33679,y:33276,varname:node_9479,prsc:2,sctp:2;n:type:ShaderForge.SFN_Add,id:7864,x:34015,y:33412,varname:node_7864,prsc:2|A-9479-UVOUT,B-8454-OUT;n:type:ShaderForge.SFN_Lerp,id:8665,x:34272,y:33290,varname:node_8665,prsc:2|A-9479-UVOUT,B-7864-OUT,T-3554-OUT;n:type:ShaderForge.SFN_Slider,id:3554,x:33674,y:33576,ptovrint:False,ptlb:screenUVDist,ptin:_screenUVDist,varname:node_3554,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:1;proporder:4430-8991-2245-6447-2799-9553-2236-6252-9509-3297-157-7987-8896-6459-7520-1164-2122-7301-9197-8734-6842-5139-3473-3554;pass:END;sub:END;*/

Shader "Colony_FX/PostProcess/S_PP_Poison" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _stainsColor ("stainsColor", Color) = (0.5,0.5,0.5,1)
        _stainsColor_copy ("stainsColor_copy", Color) = (0.5,0.5,0.5,1)
        _mainVignette ("mainVignette", Range(0, 2)) = 0.15
        _vignetteDarken ("vignetteDarken", Range(0, 1)) = 0
        _vignetteFreq ("vignetteFreq", Range(0, 3)) = 0.3576384
        _stainsPattern ("stainsPattern", 2D) = "white" {}
        _stainsPatternTiling ("stainsPatternTiling", Range(0, 4)) = 2
        _stainsFalloff ("stainsFalloff", Range(0, 5)) = 3
        _stainsFalloffBrightness ("stainsFalloffBrightness", Range(0, 4)) = 2
        _stainsMult ("stainsMult", Range(0, 5)) = 2
        _swapFrequency ("swapFrequency", Range(0, 8)) = 1
        _edgeMaskPow ("edgeMaskPow", Range(0, 3)) = 1
        _stainsDistorter ("stainsDistorter", 2D) = "white" {}
        _stainsDistorterTile ("stainsDistorterTile", Range(0, 4)) = 1
        _stainsDistorterPanX ("stainsDistorterPanX", Range(-1, 1)) = 0
        _stainsDistorterPanY ("stainsDistorterPanY", Range(-1, 1)) = 0
        _stainsDistortion ("stainsDistortion", Range(0, 1)) = 0.3
        _finalMix ("finalMix", Range(0, 1)) = 0
        _stainsPow ("stainsPow", Range(0, 10)) = 1
        _distVign ("distVign", Range(0, 0.3)) = 0
        _statVignetteScale ("statVignetteScale", Range(0.5, 1)) = 0.5
        _vignetteDistortionTexture ("vignetteDistortionTexture", 2D) = "white" {}
        _screenUVDist ("screenUVDist", Range(0, 1)) = 0.1
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
            uniform float _finalMix;
            uniform float _mainVignette;
            uniform float _vignetteDarken;
            uniform float4 _stainsColor;
            uniform float _stainsMult;
            uniform sampler2D _stainsDistorter; uniform float4 _stainsDistorter_ST;
            uniform float _stainsDistortion;
            uniform float _stainsDistorterPanX;
            uniform float _stainsDistorterPanY;
            uniform float _stainsDistorterTile;
            uniform float4 _stainsColor_copy;
            uniform float _stainsFalloff;
            uniform float _stainsFalloffBrightness;
            uniform float _edgeMaskPow;
            uniform sampler2D _stainsPattern; uniform float4 _stainsPattern_ST;
            uniform float _swapFrequency;
            uniform float _vignetteFreq;
            uniform float _stainsPow;
            uniform float _distVign;
            uniform float _stainsPatternTiling;
            uniform float _statVignetteScale;
            uniform sampler2D _vignetteDistortionTexture; uniform float4 _vignetteDistortionTexture_ST;
            uniform float _screenUVDist;
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
                float4 node_9237 = _Time;
                float4 _vignetteDistortionTexture_var = tex2D(_vignetteDistortionTexture,TRANSFORM_TEX(i.uv0, _vignetteDistortionTexture));
                float node_7716 = (1.5*smoothstep( (1.0 - pow(((_mainVignette+(sin((node_9237.g*_vignetteFreq))*0.01))*(2.0*_statVignetteScale)),0.4)), 1.0, length((lerp(sceneUVs.rg,(sceneUVs.rg+_vignetteDistortionTexture_var.r),_distVign)*1.0+-0.5)) ));
                float2 node_8694 = ((i.uv0*_stainsPatternTiling)*float2(1,0.5));
                float4 node_8969 = _Time;
                float2 node_9101 = (frac((node_8969.g*float2(_stainsDistorterPanX,_stainsDistorterPanY)))+(i.uv0*_stainsDistorterTile));
                float4 _stainsDistorter_var = tex2D(_stainsDistorter,TRANSFORM_TEX(node_9101, _stainsDistorter));
                float2 node_3043 = lerp(node_8694,(node_8694+_stainsDistorter_var.r),_stainsDistortion);
                float4 node_3712 = _Time;
                float node_7294 = (_swapFrequency*node_3712.g);
                float node_2082 = (0.25*node_7294);
                float node_2968 = 0.2;
                float2 node_1597 = (node_3043+(trunc(node_2082)*node_2968));
                float4 node_4321 = tex2D(_stainsPattern,TRANSFORM_TEX(node_1597, _stainsPattern));
                float2 node_3975 = (node_3043+(trunc((node_2082+0.5))*node_2968));
                float4 node_8258 = tex2D(_stainsPattern,TRANSFORM_TEX(node_3975, _stainsPattern));
                float node_4340 = (3.141592654*0.5);
                float node_2720 = lerp(node_4321.r,node_8258.r,(sin(((node_7294*node_4340)+node_4340))*0.5+0.5));
                float4 node_9875 = tex2D(_stainsPattern,TRANSFORM_TEX(i.uv0, _stainsPattern));
                float node_8454 = (node_7716*lerp((pow((1.0 - node_2720),_stainsFalloff)*_stainsFalloffBrightness),node_2720,pow(node_9875.g,_edgeMaskPow)));
                float2 node_8665 = lerp(sceneUVs.rg,(sceneUVs.rg+node_8454),_screenUVDist);
                float4 node_3687 = tex2D(_MainTex,TRANSFORM_TEX(node_8665, _MainTex));
                float node_3221 = (node_8454*_stainsMult);
                float3 emissive = lerp(lerp(node_3687.rgb,(node_3687.rgb*(1.0 - _vignetteDarken)),(_finalMix*node_7716)),lerp(_stainsColor.rgb,_stainsColor_copy.rgb,saturate((3.0*node_3221))),saturate(pow((_finalMix*node_3221),_stainsPow)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
