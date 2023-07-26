// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:34184,y:32885,varname:node_2865,prsc:2|diff-3320-OUT,spec-1196-OUT,gloss-8446-OUT,normal-2487-OUT,emission-1193-OUT,transm-2670-OUT,lwrap-2670-OUT,alpha-5125-OUT;n:type:ShaderForge.SFN_Tex2d,id:2296,x:29663,y:33199,ptovrint:False,ptlb:UVNoiseTex,ptin:_UVNoiseTex,varname:_UVNoiseTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-154-OUT;n:type:ShaderForge.SFN_Time,id:8980,x:28586,y:33137,varname:node_8980,prsc:2;n:type:ShaderForge.SFN_Slider,id:7514,x:28207,y:33281,ptovrint:False,ptlb:distPanX,ptin:_distPanX,varname:_distPanX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:4066,x:28207,y:33386,ptovrint:False,ptlb:distPanY,ptin:_distPanY,varname:_distPanY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Append,id:1806,x:28586,y:33325,varname:node_1806,prsc:2|A-7514-OUT,B-4066-OUT;n:type:ShaderForge.SFN_Multiply,id:8130,x:28807,y:33227,varname:node_8130,prsc:2|A-8980-T,B-1806-OUT;n:type:ShaderForge.SFN_Frac,id:6363,x:29006,y:33227,varname:node_6363,prsc:2|IN-8130-OUT;n:type:ShaderForge.SFN_TexCoord,id:1535,x:27711,y:32765,varname:node_1535,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:154,x:29437,y:33199,varname:node_154,prsc:2|A-641-OUT,B-1372-OUT;n:type:ShaderForge.SFN_Multiply,id:641,x:27983,y:32835,varname:node_641,prsc:2|A-1535-UVOUT,B-2689-OUT;n:type:ShaderForge.SFN_Slider,id:2689,x:27588,y:32980,ptovrint:False,ptlb:distTile,ptin:_distTile,varname:_distTile,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Multiply,id:8956,x:30040,y:33157,varname:node_8956,prsc:2|A-2296-R,B-6190-OUT;n:type:ShaderForge.SFN_Slider,id:6190,x:29694,y:33027,ptovrint:False,ptlb:distIntensityU,ptin:_distIntensityU,varname:_distIntensityU,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Slider,id:2549,x:29706,y:33437,ptovrint:False,ptlb:distIntensityV,ptin:_distIntensityV,varname:_distIntensityV,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Multiply,id:3012,x:30042,y:33303,varname:node_3012,prsc:2|A-2296-G,B-2549-OUT;n:type:ShaderForge.SFN_Append,id:9505,x:30197,y:33217,varname:node_9505,prsc:2|A-8956-OUT,B-3012-OUT;n:type:ShaderForge.SFN_Tex2d,id:3449,x:31200,y:33278,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4984-OUT;n:type:ShaderForge.SFN_Time,id:6663,x:29101,y:31630,varname:node_6663,prsc:2;n:type:ShaderForge.SFN_Slider,id:5422,x:28722,y:31774,ptovrint:False,ptlb:mainTexPanX,ptin:_mainTexPanX,varname:_mainTexPanX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:4679,x:28722,y:31878,ptovrint:False,ptlb:mainTexPanY,ptin:_mainTexPanY,varname:_mainTexPanY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Append,id:7681,x:29101,y:31818,varname:node_7681,prsc:2|A-5422-OUT,B-4679-OUT;n:type:ShaderForge.SFN_Multiply,id:8513,x:29321,y:31720,varname:node_8513,prsc:2|A-6663-T,B-7681-OUT;n:type:ShaderForge.SFN_Frac,id:3279,x:29521,y:31720,varname:node_3279,prsc:2|IN-8513-OUT;n:type:ShaderForge.SFN_Add,id:4984,x:31004,y:33228,varname:node_4984,prsc:2|A-7361-OUT,B-8152-OUT;n:type:ShaderForge.SFN_TexCoord,id:3604,x:29570,y:31979,varname:node_3604,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:2020,x:29864,y:32039,varname:node_2020,prsc:2|A-3604-UVOUT,B-837-OUT;n:type:ShaderForge.SFN_Slider,id:837,x:29491,y:32177,ptovrint:False,ptlb:mainTexTile,ptin:_mainTexTile,varname:_mainTexTile,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Add,id:7361,x:30183,y:32142,varname:node_7361,prsc:2|A-812-OUT,B-2020-OUT,C-3918-OUT;n:type:ShaderForge.SFN_Vector1,id:1196,x:33702,y:32809,varname:node_1196,prsc:2,v1:0;n:type:ShaderForge.SFN_Power,id:410,x:31805,y:33230,varname:node_410,prsc:2|VAL-9028-OUT,EXP-5272-OUT;n:type:ShaderForge.SFN_Slider,id:2648,x:31436,y:33471,ptovrint:False,ptlb:contrast,ptin:_contrast,varname:_contrast,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_SwitchProperty,id:5272,x:31780,y:33536,ptovrint:False,ptlb:useDynamicContrast(UV1x),ptin:_useDynamicContrastUV1x,varname:_useDynamicContrastUV1x,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2648-OUT,B-1660-U;n:type:ShaderForge.SFN_TexCoord,id:1660,x:31534,y:33605,varname:node_1660,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Add,id:8378,x:32182,y:33231,varname:node_8378,prsc:2|A-410-OUT,B-2366-OUT;n:type:ShaderForge.SFN_Slider,id:2366,x:32025,y:33392,ptovrint:False,ptlb:addValueToMainTex,ptin:_addValueToMainTex,varname:_addValueToMainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Subtract,id:4879,x:32514,y:33221,varname:node_4879,prsc:2|A-8378-OUT,B-6332-OUT;n:type:ShaderForge.SFN_Slider,id:2776,x:32077,y:33551,ptovrint:False,ptlb:alphaErosion,ptin:_alphaErosion,varname:_alphaErosion,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_TexCoord,id:4358,x:32223,y:33636,varname:node_4358,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_SwitchProperty,id:6332,x:32410,y:33582,ptovrint:False,ptlb:useDynamicAlphaErosion(UV1y),ptin:_useDynamicAlphaErosionUV1y,varname:_useDynamicAlphaErosionUV1y,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2776-OUT,B-4358-V;n:type:ShaderForge.SFN_SwitchProperty,id:9398,x:30640,y:33262,ptovrint:False,ptlb:useUVDist,ptin:_useUVDist,varname:_useUVDist,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5195-OUT,B-2501-OUT;n:type:ShaderForge.SFN_Vector1,id:5195,x:30291,y:33094,varname:node_5195,prsc:2,v1:0;n:type:ShaderForge.SFN_SwitchProperty,id:812,x:30022,y:31877,ptovrint:False,ptlb:useMainTexPan,ptin:_useMainTexPan,varname:_useMainTexPan,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-3245-OUT,B-3279-OUT;n:type:ShaderForge.SFN_Vector1,id:3245,x:29790,y:31692,varname:node_3245,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:8446,x:33369,y:32837,ptovrint:False,ptlb:gloss,ptin:_gloss,varname:_gloss,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Vector1,id:2670,x:33897,y:33036,varname:node_2670,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:5125,x:33567,y:33218,varname:node_5125,prsc:2|A-3242-OUT,B-7465-OUT;n:type:ShaderForge.SFN_VertexColor,id:1659,x:32019,y:32608,varname:node_1659,prsc:2;n:type:ShaderForge.SFN_DepthBlend,id:7465,x:33567,y:33387,varname:node_7465,prsc:2|DIST-4091-OUT;n:type:ShaderForge.SFN_Slider,id:4091,x:33444,y:33567,ptovrint:False,ptlb:distance,ptin:_distance,varname:_distance,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:100;n:type:ShaderForge.SFN_Multiply,id:3596,x:32757,y:33221,varname:node_3596,prsc:2|A-4879-OUT,B-2569-OUT;n:type:ShaderForge.SFN_Slider,id:2569,x:32710,y:33462,ptovrint:False,ptlb:brightness,ptin:_brightness,varname:_brightness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:12;n:type:ShaderForge.SFN_Clamp01,id:4824,x:32922,y:33240,varname:node_4824,prsc:2|IN-3596-OUT;n:type:ShaderForge.SFN_Multiply,id:3244,x:33155,y:33200,varname:node_3244,prsc:2|A-1659-A,B-4824-OUT;n:type:ShaderForge.SFN_Tex2d,id:2579,x:31125,y:33003,ptovrint:False,ptlb:MaskTex,ptin:_MaskTex,varname:_MaskTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5297-OUT;n:type:ShaderForge.SFN_Multiply,id:9028,x:31455,y:33228,varname:node_9028,prsc:2|A-5170-OUT,B-3449-R;n:type:ShaderForge.SFN_SwitchProperty,id:5170,x:31412,y:32994,ptovrint:False,ptlb:useMaskTex,ptin:_useMaskTex,varname:_useMaskTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-680-OUT,B-2579-R;n:type:ShaderForge.SFN_Vector1,id:680,x:31182,y:32895,varname:node_680,prsc:2,v1:1;n:type:ShaderForge.SFN_ViewPosition,id:7605,x:32189,y:34044,varname:node_7605,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:7006,x:32177,y:33869,varname:node_7006,prsc:2;n:type:ShaderForge.SFN_Distance,id:7904,x:32402,y:33986,varname:node_7904,prsc:2|A-7006-XYZ,B-7605-XYZ;n:type:ShaderForge.SFN_Divide,id:7377,x:32587,y:33986,varname:node_7377,prsc:2|A-7904-OUT,B-8242-OUT;n:type:ShaderForge.SFN_Power,id:2496,x:32777,y:33985,varname:node_2496,prsc:2|VAL-7377-OUT,EXP-3433-OUT;n:type:ShaderForge.SFN_Clamp01,id:6139,x:32962,y:33985,varname:node_6139,prsc:2|IN-2496-OUT;n:type:ShaderForge.SFN_Multiply,id:3242,x:33348,y:33218,varname:node_3242,prsc:2|A-3244-OUT,B-8617-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:8617,x:33149,y:33852,ptovrint:False,ptlb:useDistanceFade,ptin:_useDistanceFade,varname:_useDistanceFade,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-9166-OUT,B-6139-OUT;n:type:ShaderForge.SFN_Vector1,id:9166,x:32963,y:33824,varname:node_9166,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:8242,x:32305,y:34238,ptovrint:False,ptlb:distanceDivisor,ptin:_distanceDivisor,varname:_distanceDivisor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:200,max:5000;n:type:ShaderForge.SFN_Slider,id:3433,x:32634,y:34261,ptovrint:False,ptlb:distanceTransition,ptin:_distanceTransition,varname:_distanceTransition,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Vector2,id:8518,x:29570,y:32311,varname:node_8518,prsc:2,v1:3,v2:8;n:type:ShaderForge.SFN_TexCoord,id:6567,x:29585,y:32415,varname:node_6567,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Multiply,id:4046,x:29809,y:32356,varname:node_4046,prsc:2|A-8518-OUT,B-6567-Z;n:type:ShaderForge.SFN_SwitchProperty,id:3918,x:30018,y:32330,ptovrint:False,ptlb:useRandomMainTexOffset(FeedUV1z),ptin:_useRandomMainTexOffsetFeedUV1z,varname:_useRandomMainTexOffsetFeedUV1z,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8588-OUT,B-4046-OUT;n:type:ShaderForge.SFN_Vector1,id:8588,x:29805,y:32270,varname:node_8588,prsc:2,v1:0;n:type:ShaderForge.SFN_ComponentMask,id:9440,x:28396,y:32542,varname:node_9440,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-641-OUT;n:type:ShaderForge.SFN_ComponentMask,id:5328,x:28396,y:32717,varname:node_5328,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-641-OUT;n:type:ShaderForge.SFN_Add,id:7821,x:28796,y:32568,varname:node_7821,prsc:2|A-9440-OUT,B-8443-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8443,x:28543,y:32495,ptovrint:False,ptlb:_UVOffsetX,ptin:_UVOffsetX,varname:node_2409,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:5043,x:28549,y:32827,ptovrint:False,ptlb:_UVOffsetY,ptin:_UVOffsetY,varname:__UVOffsetX_copy,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:5908,x:28796,y:32740,varname:node_5908,prsc:2|A-5328-OUT,B-5043-OUT;n:type:ShaderForge.SFN_Append,id:8661,x:28980,y:32697,varname:node_8661,prsc:2|A-7821-OUT,B-5908-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:1372,x:29236,y:33268,ptovrint:False,ptlb:useScriptForUVDistTextureOffset,ptin:_useScriptForUVDistTextureOffset,varname:_useScriptForUVDistTextureOffset,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-6363-OUT,B-8661-OUT;n:type:ShaderForge.SFN_Time,id:920,x:30282,y:32544,varname:node_920,prsc:2;n:type:ShaderForge.SFN_Slider,id:9534,x:29903,y:32688,ptovrint:False,ptlb:maskTexPanX,ptin:_maskTexPanX,varname:_maskTexPanX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-15,cur:0,max:15;n:type:ShaderForge.SFN_Slider,id:4092,x:29903,y:32792,ptovrint:False,ptlb:maskTexPanY,ptin:_maskTexPanY,varname:_maskTexPanY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-15,cur:0,max:15;n:type:ShaderForge.SFN_Append,id:5903,x:30282,y:32732,varname:node_5903,prsc:2|A-9534-OUT,B-4092-OUT;n:type:ShaderForge.SFN_Multiply,id:2683,x:30502,y:32634,varname:node_2683,prsc:2|A-920-T,B-5903-OUT;n:type:ShaderForge.SFN_Frac,id:9647,x:30702,y:32634,varname:node_9647,prsc:2|IN-2683-OUT;n:type:ShaderForge.SFN_TexCoord,id:3716,x:30519,y:32883,varname:node_3716,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:5297,x:30770,y:32883,varname:node_5297,prsc:2|A-9647-OUT,B-3716-UVOUT;n:type:ShaderForge.SFN_RemapRange,id:7865,x:30353,y:33295,varname:node_7865,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-9505-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:2501,x:30489,y:33262,ptovrint:False,ptlb:isRemappedToNegative,ptin:_isRemappedToNegative,varname:_isRemappedToNegative,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-9505-OUT,B-7865-OUT;n:type:ShaderForge.SFN_Tex2d,id:9296,x:32549,y:32790,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:_Normal,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-4984-OUT;n:type:ShaderForge.SFN_Multiply,id:3092,x:32900,y:32924,varname:node_3092,prsc:2|A-9296-RGB,B-7986-OUT;n:type:ShaderForge.SFN_Vector1,id:2961,x:32142,y:33073,varname:node_2961,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:839,x:32063,y:32962,ptovrint:False,ptlb:normalIntensity,ptin:_normalIntensity,varname:_normalIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:111;n:type:ShaderForge.SFN_Append,id:7986,x:32402,y:32977,varname:node_7986,prsc:2|A-839-OUT,B-839-OUT,C-2961-OUT;n:type:ShaderForge.SFN_Lerp,id:2487,x:33127,y:32907,varname:node_2487,prsc:2|A-5685-OUT,B-3092-OUT,T-3242-OUT;n:type:ShaderForge.SFN_Vector3,id:5685,x:32900,y:32787,varname:node_5685,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Tex2d,id:1034,x:29567,y:33569,ptovrint:False,ptlb:flowMap,ptin:_flowMap,varname:_flowMap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:55574d839650a2543bac9890e754d1c1,ntxv:0,isnm:False;n:type:ShaderForge.SFN_SwitchProperty,id:4344,x:30749,y:33456,ptovrint:False,ptlb:useFlowMaps,ptin:_useFlowMaps,varname:_useFlowMaps,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-1833-OUT,B-4470-OUT;n:type:ShaderForge.SFN_Multiply,id:4470,x:30563,y:33657,varname:node_4470,prsc:2|A-5817-OUT,B-5334-W;n:type:ShaderForge.SFN_TexCoord,id:5334,x:30412,y:33791,varname:node_5334,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Add,id:8152,x:30828,y:33262,varname:node_8152,prsc:2|A-9398-OUT,B-4344-OUT;n:type:ShaderForge.SFN_Vector1,id:1833,x:30576,y:33460,varname:node_1833,prsc:2,v1:0;n:type:ShaderForge.SFN_TexCoord,id:2353,x:29999,y:33757,varname:node_2353,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Subtract,id:5817,x:30222,y:33653,varname:node_5817,prsc:2|A-6441-OUT,B-2353-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:6441,x:29968,y:33571,varname:node_6441,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-7118-OUT;n:type:ShaderForge.SFN_Clamp01,id:7118,x:29768,y:33571,varname:node_7118,prsc:2|IN-1034-RGB;n:type:ShaderForge.SFN_SwitchProperty,id:3320,x:32816,y:32610,ptovrint:False,ptlb:useDepthBasedColorGradient,ptin:_useDepthBasedColorGradient,varname:_useDepthBasedColorGradient,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-1659-RGB,B-9499-OUT;n:type:ShaderForge.SFN_Lerp,id:9499,x:32423,y:32331,varname:node_9499,prsc:2|A-1659-RGB,B-1302-RGB,T-4824-OUT;n:type:ShaderForge.SFN_Color,id:1302,x:32019,y:32350,ptovrint:False,ptlb:color2,ptin:_color2,varname:_color2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:1193,x:33681,y:33002,varname:node_1193,prsc:2|A-5691-OUT,B-5125-OUT,C-3320-OUT;n:type:ShaderForge.SFN_Slider,id:5691,x:33315,y:32971,ptovrint:False,ptlb:emis,ptin:_emis,varname:_emis,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:15;proporder:3449-837-3918-812-5422-4679-9398-2501-2296-2689-7514-4066-6190-2549-5272-2648-6332-2776-2366-2569-4091-8446-2579-5170-8617-8242-3433-1372-9534-4092-9296-839-1034-4344-3320-1302-5691;pass:END;sub:END;*/

Shader "Colony_FX/Basic/S_AlphaBlend_UVDist_Lit_NM" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _mainTexTile ("mainTexTile", Range(0, 15)) = 1
        [MaterialToggle] _useRandomMainTexOffsetFeedUV1z ("useRandomMainTexOffset(FeedUV1z)", Float ) = 0
        [MaterialToggle] _useMainTexPan ("useMainTexPan", Float ) = 0
        _mainTexPanX ("mainTexPanX", Range(-5, 5)) = 0
        _mainTexPanY ("mainTexPanY", Range(-5, 5)) = 0
        [MaterialToggle] _useUVDist ("useUVDist", Float ) = 0
        [MaterialToggle] _isRemappedToNegative ("isRemappedToNegative", Float ) = 0
        _UVNoiseTex ("UVNoiseTex", 2D) = "white" {}
        _distTile ("distTile", Range(0, 15)) = 1
        _distPanX ("distPanX", Range(-5, 5)) = 0
        _distPanY ("distPanY", Range(-5, 5)) = 0
        _distIntensityU ("distIntensityU", Range(0, 2)) = 0
        _distIntensityV ("distIntensityV", Range(0, 2)) = 0
        [MaterialToggle] _useDynamicContrastUV1x ("useDynamicContrast(UV1x)", Float ) = 1
        _contrast ("contrast", Range(0, 5)) = 1
        [MaterialToggle] _useDynamicAlphaErosionUV1y ("useDynamicAlphaErosion(UV1y)", Float ) = 0
        _alphaErosion ("alphaErosion", Range(0, 5)) = 0
        _addValueToMainTex ("addValueToMainTex", Range(0, 5)) = 0
        _brightness ("brightness", Range(0, 12)) = 1
        _distance ("distance", Range(0, 100)) = 0
        _gloss ("gloss", Range(0, 1)) = 0
        _MaskTex ("MaskTex", 2D) = "white" {}
        [MaterialToggle] _useMaskTex ("useMaskTex", Float ) = 1
        [MaterialToggle] _useDistanceFade ("useDistanceFade", Float ) = 1
        _distanceDivisor ("distanceDivisor", Range(0, 5000)) = 200
        _distanceTransition ("distanceTransition", Range(0, 5)) = 1
        [MaterialToggle] _useScriptForUVDistTextureOffset ("useScriptForUVDistTextureOffset", Float ) = 0
        _maskTexPanX ("maskTexPanX", Range(-15, 15)) = 0
        _maskTexPanY ("maskTexPanY", Range(-15, 15)) = 0
        _Normal ("Normal", 2D) = "bump" {}
        _normalIntensity ("normalIntensity", Range(0, 111)) = 1
        _flowMap ("flowMap", 2D) = "white" {}
        [MaterialToggle] _useFlowMaps ("useFlowMaps", Float ) = 0
        [MaterialToggle] _useDepthBasedColorGradient ("useDepthBasedColorGradient", Float ) = 0
        _color2 ("color2", Color) = (0.5,0.5,0.5,1)
        _emis ("emis", Range(0, 15)) = 0
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
			#define DS_HAZE_FULL
            #include "UnityCG.cginc"
			#include "Assets/ASkyLighting/DeepSky Haze/Resources/DS_TransparentLib.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _UVNoiseTex; uniform float4 _UVNoiseTex_ST;
            uniform float _distPanX;
            uniform float _distPanY;
            uniform float _distTile;
            uniform float _distIntensityU;
            uniform float _distIntensityV;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _mainTexPanX;
            uniform float _mainTexPanY;
            uniform float _mainTexTile;
            uniform float _contrast;
            uniform fixed _useDynamicContrastUV1x;
            uniform float _addValueToMainTex;
            uniform float _alphaErosion;
            uniform fixed _useDynamicAlphaErosionUV1y;
            uniform fixed _useUVDist;
            uniform fixed _useMainTexPan;
            uniform float _gloss;
            uniform float _distance;
            uniform float _brightness;
            uniform sampler2D _MaskTex; uniform float4 _MaskTex_ST;
            uniform fixed _useMaskTex;
            uniform fixed _useDistanceFade;
            uniform float _distanceDivisor;
            uniform float _distanceTransition;
            uniform fixed _useRandomMainTexOffsetFeedUV1z;
            uniform float _UVOffsetX;
            uniform float _UVOffsetY;
            uniform fixed _useScriptForUVDistTextureOffset;
            uniform float _maskTexPanX;
            uniform float _maskTexPanY;
            uniform fixed _isRemappedToNegative;
            uniform sampler2D _Normal; uniform float4 _Normal_ST;
            uniform float _normalIntensity;
            uniform sampler2D _flowMap; uniform float4 _flowMap_ST;
            uniform fixed _useFlowMaps;
            uniform fixed _useDepthBasedColorGradient;
            uniform float4 _color2;
            uniform float _emis;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float3 tangentDir : TEXCOORD4;
                float3 bitangentDir : TEXCOORD5;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD6;
                UNITY_FOG_COORDS(7)
				float3 air : TEXCOORD8;
				float3 hazeAndFog : TEXCOORD9;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
				DS_Haze_Per_Vertex(v.vertex, o.air, o.hazeAndFog);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_6663 = _Time;
                float2 node_641 = (i.uv0*_distTile);
                float4 node_8980 = _Time;
                float2 node_154 = (node_641+lerp( frac((node_8980.g*float2(_distPanX,_distPanY))), float2((node_641.r+_UVOffsetX),(node_641.g+_UVOffsetY)), _useScriptForUVDistTextureOffset ));
                float4 _UVNoiseTex_var = tex2D(_UVNoiseTex,TRANSFORM_TEX(node_154, _UVNoiseTex));
                float2 node_9505 = float2((_UVNoiseTex_var.r*_distIntensityU),(_UVNoiseTex_var.g*_distIntensityV));
                float4 _flowMap_var = tex2D(_flowMap,TRANSFORM_TEX(i.uv0, _flowMap));
                float2 node_4984 = ((lerp( 0.0, frac((node_6663.g*float2(_mainTexPanX,_mainTexPanY))), _useMainTexPan )+(i.uv0*_mainTexTile)+lerp( 0.0, (float2(3,8)*i.uv1.b), _useRandomMainTexOffsetFeedUV1z ))+(lerp( 0.0, lerp( node_9505, (node_9505*2.0+-1.0), _isRemappedToNegative ), _useUVDist )+lerp( 0.0, ((saturate(_flowMap_var.rgb).rg-i.uv0)*i.uv1.a), _useFlowMaps )));
                float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(node_4984, _Normal)));
                float4 node_920 = _Time;
                float2 node_5297 = (frac((node_920.g*float2(_maskTexPanX,_maskTexPanY)))+i.uv0);
                float4 _MaskTex_var = tex2D(_MaskTex,TRANSFORM_TEX(node_5297, _MaskTex));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4984, _MainTex));
                float node_4824 = saturate((((pow((lerp( 1.0, _MaskTex_var.r, _useMaskTex )*_MainTex_var.r),lerp( _contrast, i.uv1.r, _useDynamicContrastUV1x ))+_addValueToMainTex)-lerp( _alphaErosion, i.uv1.g, _useDynamicAlphaErosionUV1y ))*_brightness));
                float node_3242 = ((i.vertexColor.a*node_4824)*lerp( 1.0, saturate(pow((distance(i.posWorld.rgb,_WorldSpaceCameraPos)/_distanceDivisor),_distanceTransition)), _useDistanceFade ));
                float3 normalLocal = lerp(float3(0,0,1),(_Normal_var.rgb*float3(_normalIntensity,_normalIntensity,1.0)),node_3242);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _gloss;
                float perceptualRoughness = 1.0 - _gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = 0.0;
                float specularMonochrome;
                float3 _useDepthBasedColorGradient_var = lerp( i.vertexColor.rgb, lerp(i.vertexColor.rgb,_color2.rgb,node_4824), _useDepthBasedColorGradient );
                float3 diffuseColor = _useDepthBasedColorGradient_var; // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = dot( normalDirection, lightDirection );
                float node_2670 = 1.0;
                float3 w = float3(node_2670,node_2670,node_2670)*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 backLight = max(float3(0.0,0.0,0.0), -NdotLWrap + w ) * float3(node_2670,node_2670,node_2670);
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotLWrap);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((forwardLight+backLight) + ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL)) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float node_5125 = (node_3242*saturate((sceneZ-partZ)/_distance));
                float3 emissive = (_emis*node_5125*_useDepthBasedColorGradient_var);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,node_5125);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, unity_FogColor);
				DS_Haze_Apply(i.air, i.hazeAndFog, finalRGBA, finalRGBA.a);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
