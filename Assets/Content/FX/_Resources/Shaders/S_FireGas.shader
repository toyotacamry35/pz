// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.2601588,fgcg:0.3732891,fgcb:0.4629476,fgca:1,fgde:0.005088947,fgrn:0,fgrf:600,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:37133,y:32749,varname:node_3138,prsc:2|emission-3692-OUT,alpha-3670-OUT,clip-9078-OUT;n:type:ShaderForge.SFN_Tex2d,id:7396,x:33684,y:34841,ptovrint:False,ptlb:noiseTex2,ptin:_noiseTex2,varname:node_7396,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1363-OUT;n:type:ShaderForge.SFN_Append,id:9460,x:32832,y:34913,varname:node_9460,prsc:2|A-2045-OUT,B-9843-OUT;n:type:ShaderForge.SFN_Multiply,id:3808,x:33058,y:34878,varname:node_3808,prsc:2|A-2020-T,B-9460-OUT;n:type:ShaderForge.SFN_TexCoord,id:9250,x:32925,y:34682,cmnt:DYNAMIC PARAMS - RANDOM UV OFFSET,varname:node_9250,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Add,id:1363,x:33330,y:34877,varname:node_1363,prsc:2|A-3808-OUT,B-7583-OUT,C-2592-OUT;n:type:ShaderForge.SFN_Vector2,id:5381,x:33002,y:34572,varname:node_5381,prsc:2,v1:3,v2:8;n:type:ShaderForge.SFN_Multiply,id:7583,x:33263,y:34674,varname:node_7583,prsc:2|A-5381-OUT,B-9250-V;n:type:ShaderForge.SFN_Multiply,id:8706,x:33979,y:34845,varname:node_8706,prsc:2|A-7396-R,B-3621-OUT;n:type:ShaderForge.SFN_Slider,id:3621,x:33527,y:35118,ptovrint:False,ptlb:noise2Mix,ptin:_noise2Mix,varname:node_3621,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:15;n:type:ShaderForge.SFN_Tex2dAsset,id:2215,x:32650,y:32947,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_2215,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:6949,x:32879,y:32905,varname:node_6949,prsc:2,ntxv:0,isnm:False|TEX-2215-TEX;n:type:ShaderForge.SFN_VertexColor,id:7937,x:33008,y:32504,varname:node_7937,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5347,x:33363,y:33057,varname:node_5347,prsc:2|A-7937-A,B-6036-OUT;n:type:ShaderForge.SFN_Multiply,id:578,x:33883,y:33054,varname:node_578,prsc:2|A-7181-OUT,B-7891-OUT;n:type:ShaderForge.SFN_DepthBlend,id:7891,x:33669,y:33436,varname:node_7891,prsc:2|DIST-6349-OUT;n:type:ShaderForge.SFN_Slider,id:6349,x:33333,y:33436,ptovrint:False,ptlb:distance,ptin:_distance,varname:node_6349,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:3883,x:34077,y:33054,varname:node_3883,prsc:2|A-578-OUT,B-7813-OUT;n:type:ShaderForge.SFN_Slider,id:7813,x:33890,y:33443,ptovrint:False,ptlb:finalOpacity,ptin:_finalOpacity,varname:node_7813,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:2;n:type:ShaderForge.SFN_Multiply,id:15,x:33635,y:32523,varname:node_15,prsc:2|A-7937-RGB,B-6949-B;n:type:ShaderForge.SFN_Multiply,id:3692,x:34136,y:32539,varname:node_3692,prsc:2|A-6372-OUT,B-6504-OUT;n:type:ShaderForge.SFN_Slider,id:6504,x:33669,y:32796,ptovrint:False,ptlb:finalEmissive,ptin:_finalEmissive,varname:node_6504,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:20;n:type:ShaderForge.SFN_TexCoord,id:2979,x:32726,y:35263,varname:node_2979,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:2592,x:33002,y:35215,varname:node_2592,prsc:2|A-9049-OUT,B-2979-UVOUT;n:type:ShaderForge.SFN_Slider,id:9049,x:32648,y:35186,ptovrint:False,ptlb:noise2Tiling,ptin:_noise2Tiling,varname:node_9049,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Time,id:7905,x:30238,y:34117,varname:node_7905,prsc:2;n:type:ShaderForge.SFN_Slider,id:2960,x:30261,y:33774,ptovrint:False,ptlb:noise1PanX,ptin:_noise1PanX,varname:_noise1PanX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:0,max:2;n:type:ShaderForge.SFN_Slider,id:9922,x:30261,y:33875,ptovrint:False,ptlb:noise1PanY,ptin:_noise1PanY,varname:_noise1PanY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:0,max:2;n:type:ShaderForge.SFN_Append,id:3701,x:30631,y:33805,varname:node_3701,prsc:2|A-2960-OUT,B-9922-OUT;n:type:ShaderForge.SFN_Multiply,id:1556,x:30841,y:33674,varname:node_1556,prsc:2|A-7905-T,B-3701-OUT;n:type:ShaderForge.SFN_TexCoord,id:7859,x:30884,y:33923,cmnt:DYNAMIC PARAMS - RANDOM UV OFFSET,varname:node_7859,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Vector2,id:47,x:30961,y:33813,varname:node_47,prsc:2,v1:3,v2:8;n:type:ShaderForge.SFN_Multiply,id:1155,x:31222,y:33915,varname:node_1155,prsc:2|A-47-OUT,B-7859-V;n:type:ShaderForge.SFN_Add,id:2037,x:31278,y:33683,varname:node_2037,prsc:2|A-1556-OUT,B-6453-OUT,C-1155-OUT;n:type:ShaderForge.SFN_Multiply,id:1818,x:32416,y:33707,varname:node_1818,prsc:2|A-7707-OUT,B-5618-OUT;n:type:ShaderForge.SFN_Slider,id:5618,x:32184,y:34051,ptovrint:False,ptlb:noise1Mix,ptin:_noise1Mix,varname:_noise1Mix_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_TexCoord,id:5724,x:30693,y:33437,varname:node_5724,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:6453,x:30969,y:33389,varname:node_6453,prsc:2|A-7565-OUT,B-5724-UVOUT;n:type:ShaderForge.SFN_Slider,id:7565,x:30573,y:33333,ptovrint:False,ptlb:noise1Tiling,ptin:_noise1Tiling,varname:_noise1Tiling_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Time,id:2020,x:32564,y:34646,varname:node_2020,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7181,x:33653,y:33051,varname:node_7181,prsc:2|A-5347-OUT,B-9341-OUT;n:type:ShaderForge.SFN_Multiply,id:6372,x:33882,y:32553,varname:node_6372,prsc:2|A-15-OUT,B-9341-OUT;n:type:ShaderForge.SFN_Tex2d,id:8139,x:31672,y:34482,varname:_noiseTex_copy,prsc:2,ntxv:0,isnm:False|UVIN-6250-OUT,TEX-3258-TEX;n:type:ShaderForge.SFN_Append,id:6890,x:30753,y:34617,varname:node_6890,prsc:2|A-3012-OUT,B-4374-OUT;n:type:ShaderForge.SFN_Multiply,id:876,x:30962,y:34487,varname:node_876,prsc:2|A-7905-T,B-6890-OUT;n:type:ShaderForge.SFN_TexCoord,id:6788,x:31006,y:34735,cmnt:DYNAMIC PARAMS - RANDOM UV OFFSET,varname:node_6788,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Vector2,id:1230,x:31083,y:34625,varname:node_1230,prsc:2,v1:3,v2:8;n:type:ShaderForge.SFN_Multiply,id:8931,x:31344,y:34727,varname:node_8931,prsc:2|A-1230-OUT,B-6788-V;n:type:ShaderForge.SFN_Add,id:6250,x:31400,y:34495,varname:node_6250,prsc:2|A-876-OUT,B-7697-OUT,C-8931-OUT;n:type:ShaderForge.SFN_Multiply,id:8336,x:31985,y:34503,varname:node_8336,prsc:2|A-8139-R,B-8168-OUT;n:type:ShaderForge.SFN_TexCoord,id:938,x:30815,y:34249,varname:node_938,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:7697,x:31091,y:34201,varname:node_7697,prsc:2|A-8367-OUT,B-938-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:5659,x:31522,y:33683,varname:node_5659,prsc:2,ntxv:0,isnm:False|UVIN-2037-OUT,TEX-3258-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:3258,x:31270,y:33410,ptovrint:False,ptlb:noiseTex,ptin:_noiseTex,varname:node_3258,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:7707,x:32084,y:33721,varname:node_7707,prsc:2|A-5659-R,B-8336-OUT;n:type:ShaderForge.SFN_Slider,id:3012,x:30347,y:34591,ptovrint:False,ptlb:noise1dPanX,ptin:_noise1dPanX,varname:node_3012,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-3,cur:0,max:3;n:type:ShaderForge.SFN_Slider,id:4374,x:30347,y:34701,ptovrint:False,ptlb:noise1dPanY,ptin:_noise1dPanY,varname:_noise1dPanX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-3,cur:0,max:3;n:type:ShaderForge.SFN_Slider,id:8367,x:30707,y:34119,ptovrint:False,ptlb:noise1dTiling,ptin:_noise1dTiling,varname:node_8367,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:15;n:type:ShaderForge.SFN_Slider,id:8168,x:31628,y:34684,ptovrint:False,ptlb:noise1dMix,ptin:_noise1dMix,varname:node_8168,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Clamp01,id:1036,x:34364,y:33073,varname:node_1036,prsc:2|IN-3883-OUT;n:type:ShaderForge.SFN_Multiply,id:9341,x:32953,y:33657,varname:node_9341,prsc:2|A-1818-OUT,B-8706-OUT;n:type:ShaderForge.SFN_Slider,id:2045,x:32483,y:34894,ptovrint:False,ptlb:noise2PanX,ptin:_noise2PanX,varname:node_2045,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-3,cur:0,max:3;n:type:ShaderForge.SFN_Slider,id:9843,x:32483,y:34989,ptovrint:False,ptlb:noise2PanY,ptin:_noise2PanY,varname:_noise2PanX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-3,cur:0,max:3;n:type:ShaderForge.SFN_Subtract,id:8169,x:34698,y:33210,varname:node_8169,prsc:2|A-1036-OUT,B-7130-U;n:type:ShaderForge.SFN_TexCoord,id:7130,x:34440,y:33323,varname:node_7130,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Clamp01,id:2362,x:35249,y:33107,varname:node_2362,prsc:2|IN-8169-OUT;n:type:ShaderForge.SFN_TexCoord,id:8791,x:36273,y:34827,varname:node_8791,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:5407,x:36616,y:34764,varname:node_5407,prsc:2|A-8791-V,B-7309-OUT;n:type:ShaderForge.SFN_Vector1,id:7309,x:36407,y:34720,varname:node_7309,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Add,id:8518,x:36616,y:34996,varname:node_8518,prsc:2|A-8791-V,B-6407-OUT;n:type:ShaderForge.SFN_Vector1,id:6407,x:36401,y:35030,varname:node_6407,prsc:2,v1:-0.8;n:type:ShaderForge.SFN_Power,id:6391,x:36935,y:34764,varname:node_6391,prsc:2|VAL-5407-OUT,EXP-8239-OUT;n:type:ShaderForge.SFN_Power,id:1539,x:36972,y:35007,varname:node_1539,prsc:2|VAL-4044-OUT,EXP-8239-OUT;n:type:ShaderForge.SFN_Slider,id:8239,x:36616,y:34914,ptovrint:False,ptlb:meshEdgeCutoff,ptin:_meshEdgeCutoff,varname:node_8239,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:59.45388,max:240;n:type:ShaderForge.SFN_OneMinus,id:4044,x:36799,y:35007,varname:node_4044,prsc:2|IN-8518-OUT;n:type:ShaderForge.SFN_Clamp01,id:2963,x:37147,y:35007,varname:node_2963,prsc:2|IN-1539-OUT;n:type:ShaderForge.SFN_Multiply,id:9232,x:37359,y:34868,varname:node_9232,prsc:2|A-198-OUT,B-2963-OUT;n:type:ShaderForge.SFN_Clamp01,id:198,x:37130,y:34764,varname:node_198,prsc:2|IN-6391-OUT;n:type:ShaderForge.SFN_Tex2d,id:4127,x:37367,y:34349,ptovrint:False,ptlb:bandFilter,ptin:_bandFilter,varname:node_4127,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7624-OUT;n:type:ShaderForge.SFN_TexCoord,id:6213,x:36125,y:34276,varname:node_6213,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector1,id:6151,x:36158,y:34475,varname:node_6151,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:2264,x:36382,y:34475,varname:node_2264,prsc:2|A-6151-OUT,B-3902-U;n:type:ShaderForge.SFN_Add,id:7061,x:36565,y:34316,varname:node_7061,prsc:2|A-6213-UVOUT,B-2264-OUT;n:type:ShaderForge.SFN_Multiply,id:8997,x:37603,y:34481,varname:node_8997,prsc:2|A-4127-R,B-9232-OUT;n:type:ShaderForge.SFN_Vector1,id:3010,x:32848,y:33102,varname:node_3010,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:3670,x:36258,y:33093,varname:node_3670,prsc:2|A-2362-OUT,B-5384-OUT;n:type:ShaderForge.SFN_ToggleProperty,id:8117,x:32889,y:33256,ptovrint:False,ptlb:useMesh,ptin:_useMesh,varname:node_8117,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False;n:type:ShaderForge.SFN_Lerp,id:6036,x:33134,y:33071,varname:node_6036,prsc:2|A-6949-B,B-3010-OUT,T-8117-OUT;n:type:ShaderForge.SFN_Lerp,id:5384,x:37853,y:34481,varname:node_5384,prsc:2|A-3302-OUT,B-8997-OUT,T-8117-OUT;n:type:ShaderForge.SFN_Vector1,id:3302,x:37579,y:34393,varname:node_3302,prsc:2,v1:1;n:type:ShaderForge.SFN_TexCoord,id:3902,x:36123,y:34533,varname:node_3902,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Lerp,id:7624,x:37155,y:34349,varname:node_7624,prsc:2|A-7061-OUT,B-670-OUT,T-7142-OUT;n:type:ShaderForge.SFN_Slider,id:7142,x:36687,y:34581,ptovrint:False,ptlb:bandFilterDist,ptin:_bandFilterDist,varname:node_7142,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_TexCoord,id:6350,x:35703,y:33920,varname:node_6350,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:6114,x:35954,y:33940,varname:node_6114,prsc:2|A-6350-UVOUT,B-1336-OUT;n:type:ShaderForge.SFN_Slider,id:1336,x:35666,y:34098,ptovrint:False,ptlb:bandDistTiling,ptin:_bandDistTiling,varname:node_1336,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Tex2d,id:9408,x:36368,y:33961,ptovrint:False,ptlb:bandDistTex,ptin:_bandDistTex,varname:node_9408,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7328-OUT;n:type:ShaderForge.SFN_Append,id:2233,x:36587,y:33978,varname:node_2233,prsc:2|A-9408-R,B-9408-R;n:type:ShaderForge.SFN_Add,id:670,x:36844,y:34394,varname:node_670,prsc:2|A-7061-OUT,B-2233-OUT;n:type:ShaderForge.SFN_Power,id:5667,x:36649,y:33182,varname:node_5667,prsc:2|VAL-3670-OUT,EXP-9895-OUT;n:type:ShaderForge.SFN_Slider,id:9895,x:36287,y:33286,ptovrint:False,ptlb:opacityClipContrast,ptin:_opacityClipContrast,varname:node_9895,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.99,max:5;n:type:ShaderForge.SFN_Add,id:7328,x:36167,y:33960,varname:node_7328,prsc:2|A-6114-OUT,B-7583-OUT;n:type:ShaderForge.SFN_Lerp,id:9078,x:36918,y:33132,varname:node_9078,prsc:2|A-3456-OUT,B-5667-OUT,T-3623-OUT;n:type:ShaderForge.SFN_Vector1,id:3456,x:36670,y:33093,varname:node_3456,prsc:2,v1:1;n:type:ShaderForge.SFN_ToggleProperty,id:3623,x:36793,y:33337,ptovrint:False,ptlb:useOpacityClip,ptin:_useOpacityClip,varname:node_3623,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True;proporder:2215-6349-7813-6504-3258-2960-9922-5618-7565-3012-4374-8367-8168-7396-9049-2045-9843-3621-8239-4127-8117-7142-1336-9408-9895-3623;pass:END;sub:END;*/

Shader "Colony_FX/S_MultipliedTextures_Unlit" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _distance ("distance", Range(0, 1)) = 1
        _finalOpacity ("finalOpacity", Range(0, 2)) = 1
        _finalEmissive ("finalEmissive", Range(0, 20)) = 1
        _noiseTex ("noiseTex", 2D) = "white" {}
        _noise1PanX ("noise1PanX", Range(-2, 2)) = 0
        _noise1PanY ("noise1PanY", Range(-2, 2)) = 0
        _noise1Mix ("noise1Mix", Range(0, 5)) = 1
        _noise1Tiling ("noise1Tiling", Range(0, 15)) = 1
        _noise1dPanX ("noise1dPanX", Range(-3, 3)) = 0
        _noise1dPanY ("noise1dPanY", Range(-3, 3)) = 0
        _noise1dTiling ("noise1dTiling", Range(0, 15)) = 0
        _noise1dMix ("noise1dMix", Range(0, 5)) = 1
        _noiseTex2 ("noiseTex2", 2D) = "white" {}
        _noise2Tiling ("noise2Tiling", Range(0, 15)) = 1
        _noise2PanX ("noise2PanX", Range(-3, 3)) = 0
        _noise2PanY ("noise2PanY", Range(-3, 3)) = 0
        _noise2Mix ("noise2Mix", Range(0, 15)) = 0
        _meshEdgeCutoff ("meshEdgeCutoff", Range(0, 240)) = 59.45388
        _bandFilter ("bandFilter", 2D) = "white" {}
        [MaterialToggle] _useMesh ("useMesh", Float ) = 0
        _bandFilterDist ("bandFilterDist", Range(0, 1)) = 0
        _bandDistTiling ("bandDistTiling", Range(0, 3)) = 1
        _bandDistTex ("bandDistTex", 2D) = "white" {}
        _opacityClipContrast ("opacityClipContrast", Range(0, 5)) = 0.99
        [MaterialToggle] _useOpacityClip ("useOpacityClip", Float ) = 1
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
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _noiseTex2; uniform float4 _noiseTex2_ST;
            uniform float _noise2Mix;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _distance;
            uniform float _finalOpacity;
            uniform float _finalEmissive;
            uniform float _noise2Tiling;
            uniform float _noise1PanX;
            uniform float _noise1PanY;
            uniform float _noise1Mix;
            uniform float _noise1Tiling;
            uniform sampler2D _noiseTex; uniform float4 _noiseTex_ST;
            uniform float _noise1dPanX;
            uniform float _noise1dPanY;
            uniform float _noise1dTiling;
            uniform float _noise1dMix;
            uniform float _noise2PanX;
            uniform float _noise2PanY;
            uniform float _meshEdgeCutoff;
            uniform sampler2D _bandFilter; uniform float4 _bandFilter_ST;
            uniform fixed _useMesh;
            uniform float _bandFilterDist;
            uniform float _bandDistTiling;
            uniform sampler2D _bandDistTex; uniform float4 _bandDistTex_ST;
            uniform float _opacityClipContrast;
            uniform fixed _useOpacityClip;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 node_6949 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_7905 = _Time;
                float2 node_2037 = ((node_7905.g*float2(_noise1PanX,_noise1PanY))+(_noise1Tiling*i.uv0)+(float2(3,8)*i.uv1.g));
                float4 node_5659 = tex2D(_noiseTex,TRANSFORM_TEX(node_2037, _noiseTex));
                float2 node_6250 = ((node_7905.g*float2(_noise1dPanX,_noise1dPanY))+(_noise1dTiling*i.uv0)+(float2(3,8)*i.uv1.g));
                float4 _noiseTex_copy = tex2D(_noiseTex,TRANSFORM_TEX(node_6250, _noiseTex));
                float4 node_2020 = _Time;
                float2 node_7583 = (float2(3,8)*i.uv1.g);
                float2 node_1363 = ((node_2020.g*float2(_noise2PanX,_noise2PanY))+node_7583+(_noise2Tiling*i.uv0));
                float4 _noiseTex2_var = tex2D(_noiseTex2,TRANSFORM_TEX(node_1363, _noiseTex2));
                float node_9341 = (((node_5659.r*(_noiseTex_copy.r*_noise1dMix))*_noise1Mix)*(_noiseTex2_var.r*_noise2Mix));
                float node_2362 = saturate((saturate(((((i.vertexColor.a*lerp(node_6949.b,1.0,_useMesh))*node_9341)*saturate((sceneZ-partZ)/_distance))*_finalOpacity))-i.uv1.r));
                float2 node_7061 = (i.uv0+float2(0.0,i.uv1.r));
                float2 node_7328 = ((i.uv0*_bandDistTiling)+node_7583);
                float4 _bandDistTex_var = tex2D(_bandDistTex,TRANSFORM_TEX(node_7328, _bandDistTex));
                float2 node_7624 = lerp(node_7061,(node_7061+float2(_bandDistTex_var.r,_bandDistTex_var.r)),_bandFilterDist);
                float4 _bandFilter_var = tex2D(_bandFilter,TRANSFORM_TEX(node_7624, _bandFilter));
                float node_3670 = (node_2362*lerp(1.0,(_bandFilter_var.r*(saturate(pow((i.uv0.g+0.8),_meshEdgeCutoff))*saturate(pow((1.0 - (i.uv0.g+(-0.8))),_meshEdgeCutoff)))),_useMesh));
                clip(lerp(1.0,pow(node_3670,_opacityClipContrast),_useOpacityClip) - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = (((i.vertexColor.rgb*node_6949.b)*node_9341)*_finalEmissive);
                float3 finalColor = emissive;
                return fixed4(finalColor,node_3670);
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
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _noiseTex2; uniform float4 _noiseTex2_ST;
            uniform float _noise2Mix;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _distance;
            uniform float _finalOpacity;
            uniform float _noise2Tiling;
            uniform float _noise1PanX;
            uniform float _noise1PanY;
            uniform float _noise1Mix;
            uniform float _noise1Tiling;
            uniform sampler2D _noiseTex; uniform float4 _noiseTex_ST;
            uniform float _noise1dPanX;
            uniform float _noise1dPanY;
            uniform float _noise1dTiling;
            uniform float _noise1dMix;
            uniform float _noise2PanX;
            uniform float _noise2PanY;
            uniform float _meshEdgeCutoff;
            uniform sampler2D _bandFilter; uniform float4 _bandFilter_ST;
            uniform fixed _useMesh;
            uniform float _bandFilterDist;
            uniform float _bandDistTiling;
            uniform sampler2D _bandDistTex; uniform float4 _bandDistTex_ST;
            uniform float _opacityClipContrast;
            uniform fixed _useOpacityClip;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 uv1 : TEXCOORD2;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 node_6949 = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float4 node_7905 = _Time;
                float2 node_2037 = ((node_7905.g*float2(_noise1PanX,_noise1PanY))+(_noise1Tiling*i.uv0)+(float2(3,8)*i.uv1.g));
                float4 node_5659 = tex2D(_noiseTex,TRANSFORM_TEX(node_2037, _noiseTex));
                float2 node_6250 = ((node_7905.g*float2(_noise1dPanX,_noise1dPanY))+(_noise1dTiling*i.uv0)+(float2(3,8)*i.uv1.g));
                float4 _noiseTex_copy = tex2D(_noiseTex,TRANSFORM_TEX(node_6250, _noiseTex));
                float4 node_2020 = _Time;
                float2 node_7583 = (float2(3,8)*i.uv1.g);
                float2 node_1363 = ((node_2020.g*float2(_noise2PanX,_noise2PanY))+node_7583+(_noise2Tiling*i.uv0));
                float4 _noiseTex2_var = tex2D(_noiseTex2,TRANSFORM_TEX(node_1363, _noiseTex2));
                float node_9341 = (((node_5659.r*(_noiseTex_copy.r*_noise1dMix))*_noise1Mix)*(_noiseTex2_var.r*_noise2Mix));
                float node_2362 = saturate((saturate(((((i.vertexColor.a*lerp(node_6949.b,1.0,_useMesh))*node_9341)*saturate((sceneZ-partZ)/_distance))*_finalOpacity))-i.uv1.r));
                float2 node_7061 = (i.uv0+float2(0.0,i.uv1.r));
                float2 node_7328 = ((i.uv0*_bandDistTiling)+node_7583);
                float4 _bandDistTex_var = tex2D(_bandDistTex,TRANSFORM_TEX(node_7328, _bandDistTex));
                float2 node_7624 = lerp(node_7061,(node_7061+float2(_bandDistTex_var.r,_bandDistTex_var.r)),_bandFilterDist);
                float4 _bandFilter_var = tex2D(_bandFilter,TRANSFORM_TEX(node_7624, _bandFilter));
                float node_3670 = (node_2362*lerp(1.0,(_bandFilter_var.r*(saturate(pow((i.uv0.g+0.8),_meshEdgeCutoff))*saturate(pow((1.0 - (i.uv0.g+(-0.8))),_meshEdgeCutoff)))),_useMesh));
                clip(lerp(1.0,pow(node_3670,_opacityClipContrast),_useOpacityClip) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
