// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:False,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.7206621,fgcg:0.6951215,fgcb:0.6087191,fgca:1,fgde:0.004199915,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33985,y:32773,varname:node_2865,prsc:2|diff-1659-RGB,spec-1196-OUT,gloss-8446-OUT,transm-2670-OUT,lwrap-2670-OUT,alpha-5125-OUT;n:type:ShaderForge.SFN_Tex2d,id:2296,x:29150,y:33340,ptovrint:False,ptlb:UVNoiseTex,ptin:_UVNoiseTex,varname:node_2296,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-154-OUT;n:type:ShaderForge.SFN_Time,id:8980,x:28073,y:33278,varname:node_8980,prsc:2;n:type:ShaderForge.SFN_Slider,id:7514,x:27694,y:33422,ptovrint:False,ptlb:distPanX,ptin:_distPanX,varname:node_7514,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:4066,x:27694,y:33527,ptovrint:False,ptlb:distPanY,ptin:_distPanY,varname:node_4066,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Append,id:1806,x:28073,y:33466,varname:node_1806,prsc:2|A-7514-OUT,B-4066-OUT;n:type:ShaderForge.SFN_Multiply,id:8130,x:28294,y:33368,varname:node_8130,prsc:2|A-8980-T,B-1806-OUT;n:type:ShaderForge.SFN_Frac,id:6363,x:28493,y:33368,varname:node_6363,prsc:2|IN-8130-OUT;n:type:ShaderForge.SFN_TexCoord,id:1535,x:27198,y:32906,varname:node_1535,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:154,x:28924,y:33340,varname:node_154,prsc:2|A-641-OUT,B-1372-OUT;n:type:ShaderForge.SFN_Multiply,id:641,x:27470,y:32976,varname:node_641,prsc:2|A-1535-UVOUT,B-2689-OUT;n:type:ShaderForge.SFN_Slider,id:2689,x:27075,y:33121,ptovrint:False,ptlb:distTile,ptin:_distTile,varname:node_2689,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Multiply,id:8956,x:29527,y:33298,varname:node_8956,prsc:2|A-2296-R,B-6190-OUT;n:type:ShaderForge.SFN_Slider,id:6190,x:29181,y:33168,ptovrint:False,ptlb:distIntensityU,ptin:_distIntensityU,varname:node_6190,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Slider,id:2549,x:29193,y:33578,ptovrint:False,ptlb:distIntensityV,ptin:_distIntensityV,varname:node_2549,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:2;n:type:ShaderForge.SFN_Multiply,id:3012,x:29529,y:33444,varname:node_3012,prsc:2|A-2296-G,B-2549-OUT;n:type:ShaderForge.SFN_Append,id:9505,x:29754,y:33368,varname:node_9505,prsc:2|A-8956-OUT,B-3012-OUT;n:type:ShaderForge.SFN_Tex2d,id:3449,x:31125,y:33202,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_3449,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4984-OUT;n:type:ShaderForge.SFN_Time,id:6663,x:29101,y:31630,varname:node_6663,prsc:2;n:type:ShaderForge.SFN_Slider,id:5422,x:28722,y:31774,ptovrint:False,ptlb:mainTexPanX,ptin:_mainTexPanX,varname:_distPanX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:4679,x:28722,y:31878,ptovrint:False,ptlb:mainTexPanY,ptin:_mainTexPanY,varname:_distPanY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0,max:5;n:type:ShaderForge.SFN_Append,id:7681,x:29101,y:31818,varname:node_7681,prsc:2|A-5422-OUT,B-4679-OUT;n:type:ShaderForge.SFN_Multiply,id:8513,x:29321,y:31720,varname:node_8513,prsc:2|A-6663-T,B-7681-OUT;n:type:ShaderForge.SFN_Frac,id:3279,x:29521,y:31720,varname:node_3279,prsc:2|IN-8513-OUT;n:type:ShaderForge.SFN_Add,id:4984,x:30927,y:33202,varname:node_4984,prsc:2|A-7361-OUT,B-9398-OUT;n:type:ShaderForge.SFN_TexCoord,id:3604,x:29570,y:31979,varname:node_3604,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:2020,x:29864,y:32039,varname:node_2020,prsc:2|A-3604-UVOUT,B-837-OUT;n:type:ShaderForge.SFN_Slider,id:837,x:29491,y:32177,ptovrint:False,ptlb:mainTexTile,ptin:_mainTexTile,varname:node_837,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Add,id:7361,x:30183,y:32142,varname:node_7361,prsc:2|A-812-OUT,B-2020-OUT,C-3918-OUT;n:type:ShaderForge.SFN_Vector1,id:1196,x:33702,y:32809,varname:node_1196,prsc:2,v1:0;n:type:ShaderForge.SFN_Power,id:410,x:31804,y:33225,varname:node_410,prsc:2|VAL-9028-OUT,EXP-5272-OUT;n:type:ShaderForge.SFN_Slider,id:2648,x:31436,y:33471,ptovrint:False,ptlb:contrast,ptin:_contrast,varname:node_2648,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_SwitchProperty,id:5272,x:31780,y:33536,ptovrint:False,ptlb:useDynamicContrast(UV1x),ptin:_useDynamicContrastUV1x,varname:node_5272,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2648-OUT,B-1660-U;n:type:ShaderForge.SFN_TexCoord,id:1660,x:31534,y:33605,varname:node_1660,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Add,id:8378,x:32182,y:33231,varname:node_8378,prsc:2|A-410-OUT,B-2366-OUT;n:type:ShaderForge.SFN_Slider,id:2366,x:32025,y:33392,ptovrint:False,ptlb:addValueToMainTex,ptin:_addValueToMainTex,varname:node_2366,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Subtract,id:4879,x:32514,y:33221,varname:node_4879,prsc:2|A-8378-OUT,B-6332-OUT;n:type:ShaderForge.SFN_Slider,id:2776,x:32077,y:33551,ptovrint:False,ptlb:alphaErosion,ptin:_alphaErosion,varname:node_2776,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_TexCoord,id:4358,x:32223,y:33636,varname:node_4358,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_SwitchProperty,id:6332,x:32410,y:33582,ptovrint:False,ptlb:useDynamicAlphaErosion(UV1y),ptin:_useDynamicAlphaErosionUV1y,varname:node_6332,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-2776-OUT,B-4358-V;n:type:ShaderForge.SFN_SwitchProperty,id:9398,x:30730,y:33202,ptovrint:False,ptlb:useUVDist,ptin:_useUVDist,varname:node_9398,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5195-OUT,B-9885-OUT;n:type:ShaderForge.SFN_Vector1,id:5195,x:30291,y:33094,varname:node_5195,prsc:2,v1:0;n:type:ShaderForge.SFN_SwitchProperty,id:812,x:30022,y:31877,ptovrint:False,ptlb:useMainTexPan,ptin:_useMainTexPan,varname:node_812,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-3245-OUT,B-3279-OUT;n:type:ShaderForge.SFN_Vector1,id:3245,x:29790,y:31692,varname:node_3245,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:8446,x:33307,y:32878,ptovrint:False,ptlb:gloss,ptin:_gloss,varname:node_8446,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Vector1,id:2670,x:33717,y:32975,varname:node_2670,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:5125,x:33777,y:33219,varname:node_5125,prsc:2|A-3242-OUT,B-7465-OUT;n:type:ShaderForge.SFN_VertexColor,id:1659,x:32936,y:32766,varname:node_1659,prsc:2;n:type:ShaderForge.SFN_DepthBlend,id:7465,x:33777,y:33388,varname:node_7465,prsc:2|DIST-4091-OUT;n:type:ShaderForge.SFN_Slider,id:4091,x:33654,y:33568,ptovrint:False,ptlb:distance,ptin:_distance,varname:node_4091,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:100;n:type:ShaderForge.SFN_Multiply,id:3596,x:32757,y:33221,varname:node_3596,prsc:2|A-4879-OUT,B-2569-OUT;n:type:ShaderForge.SFN_Slider,id:2569,x:32710,y:33462,ptovrint:False,ptlb:brightness,ptin:_brightness,varname:node_2569,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Clamp01,id:4824,x:32957,y:33233,varname:node_4824,prsc:2|IN-3596-OUT;n:type:ShaderForge.SFN_Multiply,id:3244,x:33153,y:33218,varname:node_3244,prsc:2|A-1659-A,B-4824-OUT;n:type:ShaderForge.SFN_Tex2d,id:2579,x:31125,y:33003,ptovrint:False,ptlb:MaskTex,ptin:_MaskTex,varname:node_2579,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5297-OUT;n:type:ShaderForge.SFN_Multiply,id:9028,x:31455,y:33228,varname:node_9028,prsc:2|A-5170-OUT,B-3449-R;n:type:ShaderForge.SFN_SwitchProperty,id:5170,x:31412,y:32994,ptovrint:False,ptlb:useMaskTex,ptin:_useMaskTex,varname:node_5170,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-680-OUT,B-2579-R;n:type:ShaderForge.SFN_Vector1,id:680,x:31182,y:32895,varname:node_680,prsc:2,v1:1;n:type:ShaderForge.SFN_ViewPosition,id:7605,x:32008,y:34029,varname:node_7605,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:7006,x:32025,y:33889,varname:node_7006,prsc:2;n:type:ShaderForge.SFN_Distance,id:7904,x:32305,y:33973,varname:node_7904,prsc:2|A-7006-XYZ,B-7605-XYZ;n:type:ShaderForge.SFN_Divide,id:7377,x:32604,y:33985,varname:node_7377,prsc:2|A-7904-OUT,B-8242-OUT;n:type:ShaderForge.SFN_Power,id:2496,x:32800,y:33985,varname:node_2496,prsc:2|VAL-7377-OUT,EXP-2404-OUT;n:type:ShaderForge.SFN_Clamp01,id:6139,x:32962,y:33985,varname:node_6139,prsc:2|IN-2496-OUT;n:type:ShaderForge.SFN_Multiply,id:3242,x:33348,y:33218,varname:node_3242,prsc:2|A-3244-OUT,B-8617-OUT,C-1359-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:8617,x:33149,y:33852,ptovrint:False,ptlb:useDistanceFade,ptin:_useDistanceFade,varname:node_8617,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-9166-OUT,B-6139-OUT;n:type:ShaderForge.SFN_Vector1,id:9166,x:32963,y:33824,varname:node_9166,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:8242,x:32258,y:34214,ptovrint:False,ptlb:distanceDivisor,ptin:_distanceDivisor,varname:node_8242,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:200,max:5000;n:type:ShaderForge.SFN_Slider,id:3433,x:32318,y:34363,ptovrint:False,ptlb:distanceTransition,ptin:_distanceTransition,varname:node_3433,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Vector2,id:8518,x:29570,y:32311,varname:node_8518,prsc:2,v1:3,v2:8;n:type:ShaderForge.SFN_TexCoord,id:6567,x:29585,y:32415,varname:node_6567,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Multiply,id:4046,x:29809,y:32356,varname:node_4046,prsc:2|A-8518-OUT,B-6567-Z;n:type:ShaderForge.SFN_SwitchProperty,id:3918,x:30018,y:32330,ptovrint:False,ptlb:useRandomMainTexOffset(FeedUV1z),ptin:_useRandomMainTexOffsetFeedUV1z,varname:node_3918,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8588-OUT,B-4046-OUT;n:type:ShaderForge.SFN_Vector1,id:8588,x:29805,y:32270,varname:node_8588,prsc:2,v1:0;n:type:ShaderForge.SFN_ComponentMask,id:9440,x:27883,y:32683,varname:node_9440,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-641-OUT;n:type:ShaderForge.SFN_ComponentMask,id:5328,x:27883,y:32858,varname:node_5328,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-641-OUT;n:type:ShaderForge.SFN_Add,id:7821,x:28283,y:32709,varname:node_7821,prsc:2|A-9440-OUT,B-8443-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8443,x:28030,y:32636,ptovrint:False,ptlb:_UVOffsetX,ptin:_UVOffsetX,varname:node_2409,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:5043,x:28036,y:32968,ptovrint:False,ptlb:_UVOffsetY,ptin:_UVOffsetY,varname:__UVOffsetX_copy,prsc:2,glob:True,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:5908,x:28283,y:32881,varname:node_5908,prsc:2|A-5328-OUT,B-5043-OUT;n:type:ShaderForge.SFN_Append,id:8661,x:28467,y:32838,varname:node_8661,prsc:2|A-7821-OUT,B-5908-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:1372,x:28723,y:33409,ptovrint:False,ptlb:useScriptForUVDistTextureOffset,ptin:_useScriptForUVDistTextureOffset,varname:node_1372,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-6363-OUT,B-8661-OUT;n:type:ShaderForge.SFN_Time,id:920,x:30282,y:32544,varname:node_920,prsc:2;n:type:ShaderForge.SFN_Slider,id:9534,x:29903,y:32688,ptovrint:False,ptlb:maskTexPanX,ptin:_maskTexPanX,varname:_mainTexPanX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-15,cur:0,max:15;n:type:ShaderForge.SFN_Slider,id:4092,x:29903,y:32792,ptovrint:False,ptlb:maskTexPanY,ptin:_maskTexPanY,varname:_mainTexPanY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-15,cur:0,max:15;n:type:ShaderForge.SFN_Append,id:5903,x:30282,y:32732,varname:node_5903,prsc:2|A-9534-OUT,B-4092-OUT;n:type:ShaderForge.SFN_Multiply,id:2683,x:30502,y:32634,varname:node_2683,prsc:2|A-920-T,B-5903-OUT;n:type:ShaderForge.SFN_Frac,id:9647,x:30702,y:32634,varname:node_9647,prsc:2|IN-2683-OUT;n:type:ShaderForge.SFN_TexCoord,id:3716,x:30519,y:32883,varname:node_3716,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:5297,x:30770,y:32883,varname:node_5297,prsc:2|A-9647-OUT,B-3716-UVOUT;n:type:ShaderForge.SFN_RemapRange,id:7865,x:29954,y:33488,varname:node_7865,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-9505-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:2501,x:30128,y:33364,ptovrint:False,ptlb:isRemappedToNegative,ptin:_isRemappedToNegative,varname:node_2501,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-9505-OUT,B-7865-OUT;n:type:ShaderForge.SFN_Multiply,id:2404,x:32893,y:34294,varname:node_2404,prsc:2|A-3433-OUT,B-2014-OUT;n:type:ShaderForge.SFN_Multiply,id:2014,x:32767,y:34485,varname:node_2014,prsc:2|A-7904-OUT,B-2621-OUT;n:type:ShaderForge.SFN_Slider,id:2621,x:32130,y:34551,ptovrint:False,ptlb:distanceTransitionSensitivity,ptin:_distanceTransitionSensitivity,varname:node_2621,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Divide,id:8287,x:32546,y:34624,varname:node_8287,prsc:2|A-2621-OUT,B-4504-OUT;n:type:ShaderForge.SFN_Vector1,id:4504,x:32250,y:34716,varname:node_4504,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:585,x:32492,y:33818,varname:node_585,prsc:2,v1:1;n:type:ShaderForge.SFN_FragmentPosition,id:8056,x:33596,y:34227,varname:node_8056,prsc:2;n:type:ShaderForge.SFN_ViewPosition,id:1292,x:33596,y:34380,varname:node_1292,prsc:2;n:type:ShaderForge.SFN_Distance,id:5607,x:33793,y:34308,varname:node_5607,prsc:2|A-8056-XYZ,B-1292-XYZ;n:type:ShaderForge.SFN_Subtract,id:6710,x:34042,y:34308,varname:node_6710,prsc:2|A-5607-OUT,B-6879-OUT;n:type:ShaderForge.SFN_Slider,id:6879,x:33793,y:34492,ptovrint:False,ptlb:nearFieldDistanceFade,ptin:_nearFieldDistanceFade,varname:node_6879,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:150;n:type:ShaderForge.SFN_Power,id:931,x:34243,y:34308,varname:node_931,prsc:2|VAL-6710-OUT,EXP-9118-OUT;n:type:ShaderForge.SFN_Vector1,id:9118,x:34057,y:34589,varname:node_9118,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:9983,x:34480,y:34308,varname:node_9983,prsc:2|A-931-OUT,B-2977-OUT;n:type:ShaderForge.SFN_Divide,id:2977,x:34480,y:34522,varname:node_2977,prsc:2|A-8771-OUT,B-5232-OUT;n:type:ShaderForge.SFN_Slider,id:5232,x:34120,y:34723,ptovrint:False,ptlb:overallComplexDistanceFadeScale,ptin:_overallComplexDistanceFadeScale,varname:node_5232,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5,max:105000;n:type:ShaderForge.SFN_Vector1,id:9126,x:34559,y:34167,varname:node_9126,prsc:2,v1:1;n:type:ShaderForge.SFN_Subtract,id:6014,x:34718,y:34280,varname:node_6014,prsc:2|A-9126-OUT,B-9983-OUT;n:type:ShaderForge.SFN_Vector1,id:8771,x:34256,y:34517,varname:node_8771,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:3298,x:34790,y:33994,varname:node_3298,prsc:2,v1:1;n:type:ShaderForge.SFN_Clamp01,id:4866,x:34930,y:34280,varname:node_4866,prsc:2|IN-6014-OUT;n:type:ShaderForge.SFN_Lerp,id:1359,x:35146,y:33965,varname:node_1359,prsc:2|A-3298-OUT,B-4866-OUT,T-6440-OUT;n:type:ShaderForge.SFN_Slider,id:6440,x:35004,y:34176,ptovrint:False,ptlb:useComplexDistanceFade,ptin:_useComplexDistanceFade,varname:node_6440,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:9885,x:30561,y:33229,varname:node_9885,prsc:2|A-2501-OUT,B-1019-OUT,T-5842-OUT;n:type:ShaderForge.SFN_Multiply,id:1019,x:30336,y:33572,varname:node_1019,prsc:2|A-2501-OUT,B-2666-W;n:type:ShaderForge.SFN_Slider,id:5842,x:30557,y:33420,ptovrint:False,ptlb:useDynamicUVDist,ptin:_useDynamicUVDist,varname:node_5842,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_TexCoord,id:2666,x:30139,y:33768,varname:node_2666,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Vector1,id:4982,x:33758,y:32909,varname:node_4982,prsc:2,v1:0;proporder:3449-837-3918-812-5422-4679-9398-2501-2296-2689-7514-4066-6190-2549-5272-2648-6332-2776-2366-2569-4091-8446-2579-5170-8617-8242-3433-1372-9534-4092-2621-6879-5232-6440-5842;pass:END;sub:END;*/

Shader "Colony_FX/Basic/S_AlphaBlend_UVDist_Lit" {
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
        _brightness ("brightness", Range(0, 5)) = 1
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
        _distanceTransitionSensitivity ("distanceTransitionSensitivity", Range(0, 1)) = 0
        _nearFieldDistanceFade ("nearFieldDistanceFade", Range(0, 150)) = 1
        _overallComplexDistanceFadeScale ("overallComplexDistanceFadeScale", Range(0, 105000)) = 5
        _useComplexDistanceFade ("useComplexDistanceFade", Range(0, 1)) = 0
        _useDynamicUVDist ("useDynamicUVDist", Range(0, 1)) = 0
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
            #pragma target 5.0
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _UVNoiseTex; uniform float4 _UVNoiseTex_ST;
			//uniform float4 _DS_LightColour;
			
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
            uniform float _distanceTransitionSensitivity;
            uniform float _nearFieldDistanceFade;
            uniform float _overallComplexDistanceFadeScale;
            uniform float _useComplexDistanceFade;
            uniform float _useDynamicUVDist;
			
			
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
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
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD4;
				UNITY_FOG_COORDS(5)
				float3 air : TEXCOORD6;
				float3 hazeAndFog : TEXCOORD7;
				
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;//_DS_LightColour;
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
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);//_DS_LightDirection;
                float3 lightColor = _LightColor0.rgb;//_DS_LightColour;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;// * _DS_LightColour;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _gloss;
                float perceptualRoughness = 1.0 - _gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
				/*
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
				*/
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = 0.0;
                float specularMonochrome;
                float3 diffuseColor = i.vertexColor.rgb; // Need this for specular when using metallic
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
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                float4 node_920 = _Time;
                float2 node_5297 = (frac((node_920.g*float2(_maskTexPanX,_maskTexPanY)))+i.uv0);
                float4 _MaskTex_var = tex2D(_MaskTex,TRANSFORM_TEX(node_5297, _MaskTex));
                float4 node_6663 = _Time;
                float2 node_641 = (i.uv0*_distTile);
                float4 node_8980 = _Time;
                float2 node_154 = (node_641+lerp( frac((node_8980.g*float2(_distPanX,_distPanY))), float2((node_641.r+_UVOffsetX),(node_641.g+_UVOffsetY)), _useScriptForUVDistTextureOffset ));
                float4 _UVNoiseTex_var = tex2D(_UVNoiseTex,TRANSFORM_TEX(node_154, _UVNoiseTex));
                float2 node_9505 = float2((_UVNoiseTex_var.r*_distIntensityU),(_UVNoiseTex_var.g*_distIntensityV));
                float2 _isRemappedToNegative_var = lerp( node_9505, (node_9505*2.0+-1.0), _isRemappedToNegative );
                float2 node_4984 = ((lerp( 0.0, frac((node_6663.g*float2(_mainTexPanX,_mainTexPanY))), _useMainTexPan )+(i.uv0*_mainTexTile)+lerp( 0.0, (float2(3,8)*i.uv1.b), _useRandomMainTexOffsetFeedUV1z ))+lerp( 0.0, lerp(_isRemappedToNegative_var,(_isRemappedToNegative_var*i.uv1.a),_useDynamicUVDist), _useUVDist ));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_4984, _MainTex));
                float node_7904 = distance(i.posWorld.rgb,_WorldSpaceCameraPos);
                fixed4 finalRGBA = fixed4(finalColor,(((i.vertexColor.a*saturate((((pow((lerp( 1.0, _MaskTex_var.r, _useMaskTex )*_MainTex_var.r),lerp( _contrast, i.uv1.r, _useDynamicContrastUV1x ))+_addValueToMainTex)-lerp( _alphaErosion, i.uv1.g, _useDynamicAlphaErosionUV1y ))*_brightness)))*lerp( 1.0, saturate(pow((node_7904/_distanceDivisor),(_distanceTransition*(node_7904*_distanceTransitionSensitivity)))), _useDistanceFade )*lerp(1.0,saturate((1.0-(pow((distance(i.posWorld.rgb,_WorldSpaceCameraPos)-_nearFieldDistanceFade),2.0)*(1.0/_overallComplexDistanceFadeScale)))),_useComplexDistanceFade))*saturate((sceneZ-partZ)/_distance)));
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
