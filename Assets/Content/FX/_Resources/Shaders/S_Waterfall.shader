// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:35929,y:32788,varname:node_2865,prsc:2|diff-5421-OUT,spec-1196-OUT,gloss-876-OUT,normal-8277-OUT,transm-2670-OUT,lwrap-2670-OUT,alpha-6285-OUT,clip-713-OUT,refract-120-OUT,voffset-1381-OUT;n:type:ShaderForge.SFN_Tex2d,id:2296,x:29663,y:33199,ptovrint:False,ptlb:UVNoiseTex,ptin:_UVNoiseTex,varname:node_2296,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-154-OUT;n:type:ShaderForge.SFN_Time,id:8980,x:28586,y:33137,varname:node_8980,prsc:2;n:type:ShaderForge.SFN_Slider,id:7514,x:28207,y:33281,ptovrint:False,ptlb:distPanX,ptin:_distPanX,varname:node_7514,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:4066,x:28207,y:33386,ptovrint:False,ptlb:distPanY,ptin:_distPanY,varname:node_4066,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Append,id:1806,x:28586,y:33325,varname:node_1806,prsc:2|A-7514-OUT,B-4066-OUT;n:type:ShaderForge.SFN_Multiply,id:8130,x:28807,y:33227,varname:node_8130,prsc:2|A-8980-T,B-1806-OUT;n:type:ShaderForge.SFN_Frac,id:6363,x:29006,y:33227,varname:node_6363,prsc:2|IN-8130-OUT;n:type:ShaderForge.SFN_TexCoord,id:1535,x:27711,y:32765,varname:node_1535,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:154,x:29437,y:33199,varname:node_154,prsc:2|A-641-OUT,B-1372-OUT;n:type:ShaderForge.SFN_Multiply,id:641,x:27983,y:32835,varname:node_641,prsc:2|A-1535-UVOUT,B-2689-OUT;n:type:ShaderForge.SFN_Slider,id:2689,x:27588,y:32980,ptovrint:False,ptlb:distTile,ptin:_distTile,varname:node_2689,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Multiply,id:8956,x:30040,y:33157,varname:node_8956,prsc:2|A-2296-R,B-6190-OUT;n:type:ShaderForge.SFN_Slider,id:6190,x:29694,y:33027,ptovrint:False,ptlb:distIntensityU,ptin:_distIntensityU,varname:node_6190,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Slider,id:2549,x:29706,y:33437,ptovrint:False,ptlb:distIntensityV,ptin:_distIntensityV,varname:node_2549,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Multiply,id:3012,x:30042,y:33303,varname:node_3012,prsc:2|A-2296-G,B-2549-OUT;n:type:ShaderForge.SFN_Append,id:9505,x:30267,y:33227,varname:node_9505,prsc:2|A-8956-OUT,B-3012-OUT;n:type:ShaderForge.SFN_Tex2d,id:3449,x:31125,y:33240,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_3449,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4984-OUT;n:type:ShaderForge.SFN_Time,id:6663,x:29101,y:31630,varname:node_6663,prsc:2;n:type:ShaderForge.SFN_Slider,id:5422,x:28722,y:31774,ptovrint:False,ptlb:mainTexPanX,ptin:_mainTexPanX,varname:_distPanX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:4679,x:28722,y:31878,ptovrint:False,ptlb:mainTexPanY,ptin:_mainTexPanY,varname:_distPanY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Append,id:7681,x:29101,y:31818,varname:node_7681,prsc:2|A-5422-OUT,B-4679-OUT;n:type:ShaderForge.SFN_Multiply,id:8513,x:29321,y:31720,varname:node_8513,prsc:2|A-6663-T,B-7681-OUT;n:type:ShaderForge.SFN_Frac,id:3279,x:29521,y:31720,varname:node_3279,prsc:2|IN-8513-OUT;n:type:ShaderForge.SFN_Add,id:4984,x:30866,y:33169,varname:node_4984,prsc:2|A-7361-OUT,B-9398-OUT;n:type:ShaderForge.SFN_TexCoord,id:3604,x:29570,y:31979,varname:node_3604,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:2020,x:29864,y:32039,varname:node_2020,prsc:2|A-3604-UVOUT,B-837-OUT;n:type:ShaderForge.SFN_Slider,id:837,x:29491,y:32177,ptovrint:False,ptlb:mainTexTile,ptin:_mainTexTile,varname:node_837,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Add,id:7361,x:30183,y:32142,varname:node_7361,prsc:2|A-812-OUT,B-2020-OUT,C-3918-OUT;n:type:ShaderForge.SFN_Vector1,id:1196,x:33702,y:32809,varname:node_1196,prsc:2,v1:0;n:type:ShaderForge.SFN_Power,id:410,x:31805,y:33230,varname:node_410,prsc:2|VAL-9028-OUT,EXP-5272-OUT;n:type:ShaderForge.SFN_Slider,id:2648,x:31436,y:33471,ptovrint:False,ptlb:contrast,ptin:_contrast,varname:node_2648,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_SwitchProperty,id:5272,x:31780,y:33536,ptovrint:False,ptlb:useDynamicContrast(UV1x),ptin:_useDynamicContrastUV1x,varname:node_5272,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2648-OUT,B-1660-U;n:type:ShaderForge.SFN_TexCoord,id:1660,x:31534,y:33605,varname:node_1660,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Add,id:8378,x:32182,y:33231,varname:node_8378,prsc:2|A-410-OUT,B-2366-OUT;n:type:ShaderForge.SFN_Slider,id:2366,x:32025,y:33392,ptovrint:False,ptlb:addValueToMainTex,ptin:_addValueToMainTex,varname:node_2366,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Subtract,id:4879,x:32514,y:33221,varname:node_4879,prsc:2|A-8378-OUT,B-6332-OUT;n:type:ShaderForge.SFN_Slider,id:2776,x:32077,y:33551,ptovrint:False,ptlb:alphaErosion,ptin:_alphaErosion,varname:node_2776,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_TexCoord,id:4358,x:32223,y:33636,varname:node_4358,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_SwitchProperty,id:6332,x:32410,y:33582,ptovrint:False,ptlb:useDynamicAlphaErosion(UV1y),ptin:_useDynamicAlphaErosionUV1y,varname:node_6332,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2776-OUT,B-4358-V;n:type:ShaderForge.SFN_SwitchProperty,id:9398,x:30575,y:33205,ptovrint:False,ptlb:useUVDist,ptin:_useUVDist,varname:node_9398,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5195-OUT,B-9505-OUT;n:type:ShaderForge.SFN_Vector1,id:5195,x:30374,y:33112,varname:node_5195,prsc:2,v1:0;n:type:ShaderForge.SFN_SwitchProperty,id:812,x:30022,y:31877,ptovrint:False,ptlb:useMainTexPan,ptin:_useMainTexPan,varname:node_812,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-3245-OUT,B-3279-OUT;n:type:ShaderForge.SFN_Vector1,id:3245,x:29790,y:31692,varname:node_3245,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:8446,x:33524,y:32873,ptovrint:False,ptlb:gloss,ptin:_gloss,varname:node_8446,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Vector1,id:2670,x:33702,y:32950,varname:node_2670,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:3596,x:32757,y:33221,varname:node_3596,prsc:2|A-4879-OUT,B-2569-OUT;n:type:ShaderForge.SFN_Slider,id:2569,x:32710,y:33462,ptovrint:False,ptlb:brightness,ptin:_brightness,varname:node_2569,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Clamp01,id:4824,x:32957,y:33233,varname:node_4824,prsc:2|IN-3596-OUT;n:type:ShaderForge.SFN_Tex2d,id:2579,x:31125,y:33003,ptovrint:False,ptlb:MaskTex,ptin:_MaskTex,varname:node_2579,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9028,x:31455,y:33228,varname:node_9028,prsc:2|A-5170-OUT,B-3449-R;n:type:ShaderForge.SFN_SwitchProperty,id:5170,x:31412,y:32994,ptovrint:False,ptlb:useMaskTex,ptin:_useMaskTex,varname:node_5170,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-680-OUT,B-2579-R;n:type:ShaderForge.SFN_Vector1,id:680,x:31182,y:32895,varname:node_680,prsc:2,v1:1;n:type:ShaderForge.SFN_ViewPosition,id:7605,x:32189,y:34044,varname:node_7605,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:7006,x:32177,y:33869,varname:node_7006,prsc:2;n:type:ShaderForge.SFN_Distance,id:7904,x:32402,y:33986,varname:node_7904,prsc:2|A-7006-XYZ,B-7605-XYZ;n:type:ShaderForge.SFN_Divide,id:7377,x:32587,y:33986,varname:node_7377,prsc:2|A-7904-OUT,B-8242-OUT;n:type:ShaderForge.SFN_Power,id:2496,x:32777,y:33985,varname:node_2496,prsc:2|VAL-7377-OUT,EXP-3433-OUT;n:type:ShaderForge.SFN_Clamp01,id:6139,x:32962,y:33985,varname:node_6139,prsc:2|IN-2496-OUT;n:type:ShaderForge.SFN_Multiply,id:3242,x:33300,y:33218,varname:node_3242,prsc:2|A-1263-OUT,B-8617-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:8617,x:33149,y:33852,ptovrint:False,ptlb:useDistanceFade,ptin:_useDistanceFade,varname:node_8617,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-9166-OUT,B-6139-OUT;n:type:ShaderForge.SFN_Vector1,id:9166,x:32963,y:33824,varname:node_9166,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:8242,x:32305,y:34238,ptovrint:False,ptlb:distanceDivisor,ptin:_distanceDivisor,varname:node_8242,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:200,max:5000;n:type:ShaderForge.SFN_Slider,id:3433,x:32634,y:34261,ptovrint:False,ptlb:distanceTransition,ptin:_distanceTransition,varname:node_3433,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Vector2,id:8518,x:29570,y:32311,varname:node_8518,prsc:2,v1:3,v2:8;n:type:ShaderForge.SFN_TexCoord,id:6567,x:29585,y:32415,varname:node_6567,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Multiply,id:4046,x:29809,y:32356,varname:node_4046,prsc:2|A-8518-OUT,B-6567-Z;n:type:ShaderForge.SFN_SwitchProperty,id:3918,x:30018,y:32330,ptovrint:False,ptlb:useRandomMainTexOffset(FeedUV1z),ptin:_useRandomMainTexOffsetFeedUV1z,varname:node_3918,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8588-OUT,B-4046-OUT;n:type:ShaderForge.SFN_Vector1,id:8588,x:29805,y:32270,varname:node_8588,prsc:2,v1:0;n:type:ShaderForge.SFN_ComponentMask,id:9440,x:28396,y:32542,varname:node_9440,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-641-OUT;n:type:ShaderForge.SFN_ComponentMask,id:5328,x:28396,y:32717,varname:node_5328,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-641-OUT;n:type:ShaderForge.SFN_Add,id:7821,x:28796,y:32568,varname:node_7821,prsc:2|A-9440-OUT,B-8443-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8443,x:28543,y:32495,ptovrint:False,ptlb:_UVOffsetX,ptin:_UVOffsetX,varname:node_2409,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:5043,x:28549,y:32827,ptovrint:False,ptlb:_UVOffsetY,ptin:_UVOffsetY,varname:__UVOffsetX_copy,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:5908,x:28796,y:32740,varname:node_5908,prsc:2|A-5328-OUT,B-5043-OUT;n:type:ShaderForge.SFN_Append,id:8661,x:28980,y:32697,varname:node_8661,prsc:2|A-7821-OUT,B-5908-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:1372,x:29236,y:33268,ptovrint:False,ptlb:useScriptForUVDistTextureOffset,ptin:_useScriptForUVDistTextureOffset,varname:node_1372,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-6363-OUT,B-8661-OUT;n:type:ShaderForge.SFN_Time,id:920,x:32164,y:32502,varname:node_920,prsc:2;n:type:ShaderForge.SFN_Slider,id:9534,x:31785,y:32646,ptovrint:False,ptlb:maskTexPanX,ptin:_maskTexPanX,varname:_mainTexPanX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-15,cur:0,max:15;n:type:ShaderForge.SFN_Slider,id:4092,x:31785,y:32750,ptovrint:False,ptlb:maskTexPanY,ptin:_maskTexPanY,varname:_mainTexPanY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-15,cur:0,max:15;n:type:ShaderForge.SFN_Append,id:5903,x:32164,y:32690,varname:node_5903,prsc:2|A-9534-OUT,B-4092-OUT;n:type:ShaderForge.SFN_Multiply,id:2683,x:32384,y:32592,varname:node_2683,prsc:2|A-920-T,B-5903-OUT;n:type:ShaderForge.SFN_Frac,id:9647,x:32584,y:32592,varname:node_9647,prsc:2|IN-2683-OUT;n:type:ShaderForge.SFN_TexCoord,id:3716,x:32114,y:32847,varname:node_3716,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:5297,x:32616,y:32830,varname:node_5297,prsc:2|A-9647-OUT,B-4626-OUT;n:type:ShaderForge.SFN_Tex2d,id:1798,x:33245,y:32905,ptovrint:False,ptlb:NormalTex,ptin:_NormalTex,varname:node_1798,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True|UVIN-4984-OUT;n:type:ShaderForge.SFN_Vector3,id:1803,x:33262,y:33078,varname:node_1803,prsc:2,v1:3,v2:3,v3:1;n:type:ShaderForge.SFN_Multiply,id:9294,x:33452,y:33038,varname:node_9294,prsc:2|A-1798-RGB,B-1803-OUT;n:type:ShaderForge.SFN_Color,id:3209,x:32724,y:32299,ptovrint:False,ptlb:baseCol,ptin:_baseCol,varname:node_3209,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:1263,x:33146,y:33218,varname:node_1263,prsc:2|A-8297-OUT,B-4824-OUT;n:type:ShaderForge.SFN_Tex2d,id:1329,x:32815,y:32951,ptovrint:False,ptlb:multMask,ptin:_multMask,varname:node_1329,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5297-OUT;n:type:ShaderForge.SFN_Power,id:8297,x:33058,y:33015,varname:node_8297,prsc:2|VAL-1329-R,EXP-1896-OUT;n:type:ShaderForge.SFN_Slider,id:1896,x:32736,y:33129,ptovrint:False,ptlb:maskPow,ptin:_maskPow,varname:node_1896,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:11;n:type:ShaderForge.SFN_Multiply,id:4626,x:32343,y:32875,varname:node_4626,prsc:2|A-3716-UVOUT,B-5356-OUT;n:type:ShaderForge.SFN_Slider,id:5356,x:32020,y:33100,ptovrint:False,ptlb:multMaskTile,ptin:_multMaskTile,varname:node_5356,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_VertexColor,id:9570,x:33786,y:33387,varname:node_9570,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3365,x:33894,y:33215,varname:node_3365,prsc:2|A-3242-OUT,B-9570-R;n:type:ShaderForge.SFN_Smoothstep,id:3839,x:33553,y:31895,varname:node_3839,prsc:2|A-5273-OUT,B-8187-OUT,V-713-OUT;n:type:ShaderForge.SFN_Vector1,id:3094,x:33056,y:31874,varname:node_3094,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Subtract,id:5273,x:33256,y:31874,varname:node_5273,prsc:2|A-3094-OUT,B-494-OUT;n:type:ShaderForge.SFN_Slider,id:494,x:32899,y:31988,ptovrint:False,ptlb:colorRange,ptin:_colorRange,varname:node_6935,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:2;n:type:ShaderForge.SFN_Add,id:8187,x:33256,y:32045,varname:node_8187,prsc:2|A-494-OUT,B-3094-OUT;n:type:ShaderForge.SFN_Multiply,id:6349,x:33782,y:31919,varname:node_6349,prsc:2|A-3839-OUT,B-3209-RGB,C-3839-OUT;n:type:ShaderForge.SFN_Tex2d,id:4899,x:33989,y:33568,ptovrint:False,ptlb:node_4899,ptin:_node_4899,varname:node_4899,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5297-OUT;n:type:ShaderForge.SFN_Multiply,id:9290,x:34211,y:33684,varname:node_9290,prsc:2|A-4899-R,B-2798-OUT;n:type:ShaderForge.SFN_OneMinus,id:2798,x:33973,y:33755,varname:node_2798,prsc:2|IN-9570-R;n:type:ShaderForge.SFN_Multiply,id:8105,x:34474,y:33644,varname:node_8105,prsc:2|A-9290-OUT,B-3160-OUT;n:type:ShaderForge.SFN_Vector1,id:2495,x:34006,y:34126,varname:node_2495,prsc:2,v1:0.01;n:type:ShaderForge.SFN_Multiply,id:1381,x:34789,y:33657,varname:node_1381,prsc:2|A-8105-OUT,B-891-OUT;n:type:ShaderForge.SFN_NormalVector,id:891,x:34608,y:33774,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:3160,x:34308,y:33865,varname:node_3160,prsc:2|A-5314-OUT,B-2495-OUT;n:type:ShaderForge.SFN_Slider,id:5314,x:33920,y:33998,ptovrint:False,ptlb:node_5314,ptin:_node_5314,varname:node_5314,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:8277,x:34143,y:33027,varname:node_8277,prsc:2|A-2697-OUT,B-9294-OUT,T-3242-OUT;n:type:ShaderForge.SFN_Vector3,id:2697,x:33922,y:32969,varname:node_2697,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_TexCoord,id:7122,x:34023,y:33316,varname:node_7122,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:7207,x:34242,y:33338,varname:node_7207,prsc:2|A-7122-V,B-8734-OUT;n:type:ShaderForge.SFN_Vector1,id:4221,x:34198,y:33506,varname:node_4221,prsc:2,v1:0.85;n:type:ShaderForge.SFN_Clamp01,id:6437,x:34434,y:33323,varname:node_6437,prsc:2|IN-7207-OUT;n:type:ShaderForge.SFN_Multiply,id:713,x:34503,y:33174,varname:node_713,prsc:2|A-3365-OUT,B-6437-OUT;n:type:ShaderForge.SFN_Add,id:1729,x:34297,y:32477,varname:node_1729,prsc:2|A-6349-OUT,B-7189-OUT;n:type:ShaderForge.SFN_Color,id:1679,x:33365,y:32254,ptovrint:False,ptlb:foam,ptin:_foam,varname:node_1679,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:7189,x:34005,y:32446,varname:node_7189,prsc:2|A-1679-RGB,B-866-OUT;n:type:ShaderForge.SFN_Power,id:866,x:33678,y:32442,varname:node_866,prsc:2|VAL-3242-OUT,EXP-8983-OUT;n:type:ShaderForge.SFN_Vector1,id:8983,x:33481,y:32505,varname:node_8983,prsc:2,v1:16;n:type:ShaderForge.SFN_Lerp,id:1536,x:34089,y:32206,varname:node_1536,prsc:2|A-6349-OUT,B-1679-RGB,T-866-OUT;n:type:ShaderForge.SFN_Slider,id:8734,x:34274,y:33499,ptovrint:False,ptlb:bottomPow,ptin:_bottomPow,varname:node_8734,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Multiply,id:876,x:34143,y:32850,varname:node_876,prsc:2|A-8446-OUT,B-3242-OUT;n:type:ShaderForge.SFN_Clamp01,id:5421,x:34574,y:32645,varname:node_5421,prsc:2|IN-1729-OUT;n:type:ShaderForge.SFN_Multiply,id:8095,x:34718,y:33039,varname:node_8095,prsc:2|A-6299-OUT,B-713-OUT;n:type:ShaderForge.SFN_Vector1,id:6299,x:34433,y:33073,varname:node_6299,prsc:2,v1:0.45;n:type:ShaderForge.SFN_Multiply,id:6285,x:35221,y:33039,varname:node_6285,prsc:2|A-8095-OUT,B-7332-OUT;n:type:ShaderForge.SFN_DepthBlend,id:8781,x:34863,y:33196,varname:node_8781,prsc:2|DIST-5600-OUT;n:type:ShaderForge.SFN_Slider,id:5600,x:34740,y:33376,ptovrint:False,ptlb:distance,ptin:_distance,varname:node_4091,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:100;n:type:ShaderForge.SFN_Multiply,id:455,x:35100,y:33196,varname:node_455,prsc:2|A-8781-OUT,B-8040-OUT;n:type:ShaderForge.SFN_Vector1,id:8040,x:35003,y:33352,varname:node_8040,prsc:2,v1:15;n:type:ShaderForge.SFN_Clamp01,id:7332,x:35276,y:33207,varname:node_7332,prsc:2|IN-455-OUT;n:type:ShaderForge.SFN_ComponentMask,id:8065,x:35345,y:33422,varname:node_8065,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-8277-OUT;n:type:ShaderForge.SFN_Multiply,id:120,x:35868,y:33360,varname:node_120,prsc:2|A-8065-OUT,B-7609-OUT;n:type:ShaderForge.SFN_Vector1,id:7609,x:35723,y:33494,varname:node_7609,prsc:2,v1:0.01;proporder:3449-837-3918-812-5422-4679-9398-2296-2689-7514-4066-6190-2549-5272-2648-6332-2776-2366-2569-8446-2579-5170-8617-8242-3433-1372-9534-4092-1798-3209-1329-1896-5356-494-4899-5314-1679-8734-5600;pass:END;sub:END;*/

Shader "Colony_FX/Unique/S_Waterfall" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _mainTexTile ("mainTexTile", Range(0, 15)) = 1
        [MaterialToggle] _useRandomMainTexOffsetFeedUV1z ("useRandomMainTexOffset(FeedUV1z)", Float ) = 0
        [MaterialToggle] _useMainTexPan ("useMainTexPan", Float ) = 0
        _mainTexPanX ("mainTexPanX", Range(-5, 5)) = 0
        _mainTexPanY ("mainTexPanY", Range(-5, 5)) = 0
        [MaterialToggle] _useUVDist ("useUVDist", Float ) = 0
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
        _brightness ("brightness", Range(0, 5)) = 1
        _gloss ("gloss", Range(0, 1)) = 1
        _MaskTex ("MaskTex", 2D) = "white" {}
        [MaterialToggle] _useMaskTex ("useMaskTex", Float ) = 1
        [MaterialToggle] _useDistanceFade ("useDistanceFade", Float ) = 1
        _distanceDivisor ("distanceDivisor", Range(0, 5000)) = 200
        _distanceTransition ("distanceTransition", Range(0, 5)) = 1
        [MaterialToggle] _useScriptForUVDistTextureOffset ("useScriptForUVDistTextureOffset", Float ) = 0
        _maskTexPanX ("maskTexPanX", Range(-15, 15)) = 0
        _maskTexPanY ("maskTexPanY", Range(-15, 15)) = 0
        _NormalTex ("NormalTex", 2D) = "bump" {}
        _baseCol ("baseCol", Color) = (0.5,0.5,0.5,1)
        _multMask ("multMask", 2D) = "white" {}
        _maskPow ("maskPow", Range(0, 11)) = 0
        _multMaskTile ("multMaskTile", Range(0, 5)) = 1
        _colorRange ("colorRange", Range(0, 2)) = 2
        _node_4899 ("node_4899", 2D) = "white" {}
        _node_5314 ("node_5314", Range(0, 1)) = 0
        _foam ("foam", Color) = (0.5,0.5,0.5,1)
        _bottomPow ("bottomPow", Range(0, 5)) = 0
        _distance ("distance", Range(0, 100)) = 0
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
			#define DS_HAZE_FULL
            #include "UnityCG.cginc"
			#include "Assets/ASkyLighting/DeepSky Haze/Resources/DS_TransparentLib.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
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
            uniform sampler2D _NormalTex; uniform float4 _NormalTex_ST;
            uniform float4 _baseCol;
            uniform sampler2D _multMask; uniform float4 _multMask_ST;
            uniform float _maskPow;
            uniform float _multMaskTile;
            uniform float _colorRange;
            uniform sampler2D _node_4899; uniform float4 _node_4899_ST;
            uniform float _node_5314;
            uniform float4 _foam;
            uniform float _bottomPow;
            uniform float _distance;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD7;
                UNITY_FOG_COORDS(8)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD9;
                #endif
				float3 air : TEXCOORD10;
				float3 hazeAndFog : TEXCOORD11;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                float4 node_920 = _Time;
                float2 node_5297 = (frac((node_920.g*float2(_maskTexPanX,_maskTexPanY)))+(o.uv0*_multMaskTile));
                float4 _node_4899_var = tex2Dlod(_node_4899,float4(TRANSFORM_TEX(node_5297, _node_4899),0.0,0));
                v.vertex.xyz += (((_node_4899_var.r*(1.0 - o.vertexColor.r))*(_node_5314*0.01))*v.normal);
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
                float2 node_4984 = ((lerp( 0.0, frac((node_6663.g*float2(_mainTexPanX,_mainTexPanY))), _useMainTexPan )+(i.uv0*_mainTexTile)+lerp( 0.0, (float2(3,8)*i.uv1.b), _useRandomMainTexOffsetFeedUV1z ))+lerp( 0.0, float2((_UVNoiseTex_var.r*_distIntensityU),(_UVNoiseTex_var.g*_distIntensityV)), _useUVDist ));
                float3 _NormalTex_var = UnpackNormal(tex2D(_NormalTex,TRANSFORM_TEX(node_4984, _NormalTex)));
                float4 node_920 = _Time;
                float2 node_5297 = (frac((node_920.g*float2(_maskTexPanX,_maskTexPanY)))+(i.uv0*_multMaskTile));
                float4 _multMask_var = tex2D(_multMask,TRANSFORM_TEX(node_5297, _multMask));
                float4 _MaskTex_var = tex2D(_MaskTex,TRANSFORM_TEX(i.uv0, _MaskTex));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4984, _MainTex));
                float node_3242 = ((pow(_multMask_var.r,_maskPow)*saturate((((pow((lerp( 1.0, _MaskTex_var.r, _useMaskTex )*_MainTex_var.r),lerp( _contrast, i.uv1.r, _useDynamicContrastUV1x ))+_addValueToMainTex)-lerp( _alphaErosion, i.uv1.g, _useDynamicAlphaErosionUV1y ))*_brightness)))*lerp( 1.0, saturate(pow((distance(i.posWorld.rgb,_WorldSpaceCameraPos)/_distanceDivisor),_distanceTransition)), _useDistanceFade ));
                float3 node_8277 = lerp(float3(0,0,1),(_NormalTex_var.rgb*float3(3,3,1)),node_3242);
                float3 normalLocal = node_8277;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float2 node_8065 = node_8277.rg;
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (node_8065*0.01);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float node_713 = ((node_3242*i.vertexColor.r)*saturate((i.uv0.g*_bottomPow)));
                clip(node_713 - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = (_gloss*node_3242);
                float perceptualRoughness = 1.0 - (_gloss*node_3242);
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
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
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
                float node_3094 = 0.5;
                float node_3839 = smoothstep( (node_3094-_colorRange), (_colorRange+node_3094), node_713 );
                float3 node_6349 = (node_3839*_baseCol.rgb*node_3839);
                float node_866 = pow(node_3242,16.0);
                float3 diffuseColor = saturate((node_6349+(_foam.rgb*node_866))); // Need this for specular when using metallic
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
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
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
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,((0.45*node_713)*saturate((saturate((sceneZ-partZ)/_distance)*15.0)))),1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, unity_FogColor);
				DS_Haze_Apply(i.air, i.hazeAndFog, finalRGBA, finalRGBA.a);
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
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
            uniform sampler2D _multMask; uniform float4 _multMask_ST;
            uniform float _maskPow;
            uniform float _multMaskTile;
            uniform sampler2D _node_4899; uniform float4 _node_4899_ST;
            uniform float _node_5314;
            uniform float _bottomPow;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 uv1 : TEXCOORD2;
                float2 uv2 : TEXCOORD3;
                float4 posWorld : TEXCOORD4;
                float3 normalDir : TEXCOORD5;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_920 = _Time;
                float2 node_5297 = (frac((node_920.g*float2(_maskTexPanX,_maskTexPanY)))+(o.uv0*_multMaskTile));
                float4 _node_4899_var = tex2Dlod(_node_4899,float4(TRANSFORM_TEX(node_5297, _node_4899),0.0,0));
                v.vertex.xyz += (((_node_4899_var.r*(1.0 - o.vertexColor.r))*(_node_5314*0.01))*v.normal);
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
                float4 node_920 = _Time;
                float2 node_5297 = (frac((node_920.g*float2(_maskTexPanX,_maskTexPanY)))+(i.uv0*_multMaskTile));
                float4 _multMask_var = tex2D(_multMask,TRANSFORM_TEX(node_5297, _multMask));
                float4 _MaskTex_var = tex2D(_MaskTex,TRANSFORM_TEX(i.uv0, _MaskTex));
                float4 node_6663 = _Time;
                float2 node_641 = (i.uv0*_distTile);
                float4 node_8980 = _Time;
                float2 node_154 = (node_641+lerp( frac((node_8980.g*float2(_distPanX,_distPanY))), float2((node_641.r+_UVOffsetX),(node_641.g+_UVOffsetY)), _useScriptForUVDistTextureOffset ));
                float4 _UVNoiseTex_var = tex2D(_UVNoiseTex,TRANSFORM_TEX(node_154, _UVNoiseTex));
                float2 node_4984 = ((lerp( 0.0, frac((node_6663.g*float2(_mainTexPanX,_mainTexPanY))), _useMainTexPan )+(i.uv0*_mainTexTile)+lerp( 0.0, (float2(3,8)*i.uv1.b), _useRandomMainTexOffsetFeedUV1z ))+lerp( 0.0, float2((_UVNoiseTex_var.r*_distIntensityU),(_UVNoiseTex_var.g*_distIntensityV)), _useUVDist ));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4984, _MainTex));
                float node_3242 = ((pow(_multMask_var.r,_maskPow)*saturate((((pow((lerp( 1.0, _MaskTex_var.r, _useMaskTex )*_MainTex_var.r),lerp( _contrast, i.uv1.r, _useDynamicContrastUV1x ))+_addValueToMainTex)-lerp( _alphaErosion, i.uv1.g, _useDynamicAlphaErosionUV1y ))*_brightness)))*lerp( 1.0, saturate(pow((distance(i.posWorld.rgb,_WorldSpaceCameraPos)/_distanceDivisor),_distanceTransition)), _useDistanceFade ));
                float node_713 = ((node_3242*i.vertexColor.r)*saturate((i.uv0.g*_bottomPow)));
                clip(node_713 - 0.5);
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
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
            uniform float4 _baseCol;
            uniform sampler2D _multMask; uniform float4 _multMask_ST;
            uniform float _maskPow;
            uniform float _multMaskTile;
            uniform float _colorRange;
            uniform sampler2D _node_4899; uniform float4 _node_4899_ST;
            uniform float _node_5314;
            uniform float4 _foam;
            uniform float _bottomPow;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_920 = _Time;
                float2 node_5297 = (frac((node_920.g*float2(_maskTexPanX,_maskTexPanY)))+(o.uv0*_multMaskTile));
                float4 _node_4899_var = tex2Dlod(_node_4899,float4(TRANSFORM_TEX(node_5297, _node_4899),0.0,0));
                v.vertex.xyz += (((_node_4899_var.r*(1.0 - o.vertexColor.r))*(_node_5314*0.01))*v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : SV_Target {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float node_3094 = 0.5;
                float4 node_920 = _Time;
                float2 node_5297 = (frac((node_920.g*float2(_maskTexPanX,_maskTexPanY)))+(i.uv0*_multMaskTile));
                float4 _multMask_var = tex2D(_multMask,TRANSFORM_TEX(node_5297, _multMask));
                float4 _MaskTex_var = tex2D(_MaskTex,TRANSFORM_TEX(i.uv0, _MaskTex));
                float4 node_6663 = _Time;
                float2 node_641 = (i.uv0*_distTile);
                float4 node_8980 = _Time;
                float2 node_154 = (node_641+lerp( frac((node_8980.g*float2(_distPanX,_distPanY))), float2((node_641.r+_UVOffsetX),(node_641.g+_UVOffsetY)), _useScriptForUVDistTextureOffset ));
                float4 _UVNoiseTex_var = tex2D(_UVNoiseTex,TRANSFORM_TEX(node_154, _UVNoiseTex));
                float2 node_4984 = ((lerp( 0.0, frac((node_6663.g*float2(_mainTexPanX,_mainTexPanY))), _useMainTexPan )+(i.uv0*_mainTexTile)+lerp( 0.0, (float2(3,8)*i.uv1.b), _useRandomMainTexOffsetFeedUV1z ))+lerp( 0.0, float2((_UVNoiseTex_var.r*_distIntensityU),(_UVNoiseTex_var.g*_distIntensityV)), _useUVDist ));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4984, _MainTex));
                float node_3242 = ((pow(_multMask_var.r,_maskPow)*saturate((((pow((lerp( 1.0, _MaskTex_var.r, _useMaskTex )*_MainTex_var.r),lerp( _contrast, i.uv1.r, _useDynamicContrastUV1x ))+_addValueToMainTex)-lerp( _alphaErosion, i.uv1.g, _useDynamicAlphaErosionUV1y ))*_brightness)))*lerp( 1.0, saturate(pow((distance(i.posWorld.rgb,_WorldSpaceCameraPos)/_distanceDivisor),_distanceTransition)), _useDistanceFade ));
                float node_713 = ((node_3242*i.vertexColor.r)*saturate((i.uv0.g*_bottomPow)));
                float node_3839 = smoothstep( (node_3094-_colorRange), (_colorRange+node_3094), node_713 );
                float3 node_6349 = (node_3839*_baseCol.rgb*node_3839);
                float node_866 = pow(node_3242,16.0);
                float3 diffColor = saturate((node_6349+(_foam.rgb*node_866)));
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, 0.0, specColor, specularMonochrome );
                float roughness = 1.0 - (_gloss*node_3242);
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
