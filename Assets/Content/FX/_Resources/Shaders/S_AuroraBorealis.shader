// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:1,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.2096435,fgcg:0.1750923,fgcb:0.3141219,fgca:1,fgde:0.004917444,fgrn:0,fgrf:6000,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:41611,y:32543,varname:node_3138,prsc:2|emission-1160-OUT,voffset-1632-OUT,tess-3308-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32552,y:32290,ptovrint:False,ptlb:ColorBottom,ptin:_ColorBottom,varname:_ColorBottom,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.3726415,c2:1,c3:0.9178863,c4:1;n:type:ShaderForge.SFN_ObjectPosition,id:1319,x:31694,y:32400,varname:node_1319,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:1125,x:31694,y:32551,varname:node_1125,prsc:2;n:type:ShaderForge.SFN_Subtract,id:3242,x:31938,y:32441,varname:node_3242,prsc:2|A-1319-Y,B-1125-Y;n:type:ShaderForge.SFN_Add,id:9609,x:32208,y:32444,varname:node_9609,prsc:2|A-3242-OUT,B-1143-OUT;n:type:ShaderForge.SFN_Slider,id:1143,x:31859,y:32715,ptovrint:False,ptlb:meshPositionY,ptin:_meshPositionY,varname:_meshPositionY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:500;n:type:ShaderForge.SFN_Lerp,id:4573,x:33140,y:32449,varname:node_4573,prsc:2|A-8785-OUT,B-1221-OUT,T-5456-OUT;n:type:ShaderForge.SFN_Color,id:6395,x:32553,y:31859,ptovrint:False,ptlb:ColorTop,ptin:_ColorTop,varname:_ColorTop,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9622642,c2:0.4667079,c3:0.3948915,c4:1;n:type:ShaderForge.SFN_Slider,id:3547,x:32254,y:32656,ptovrint:False,ptlb:meshScaleDivisor,ptin:_meshScaleDivisor,varname:_meshScaleDivisor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:100,max:1000;n:type:ShaderForge.SFN_Divide,id:5456,x:32572,y:32461,varname:node_5456,prsc:2|A-9609-OUT,B-3547-OUT;n:type:ShaderForge.SFN_Multiply,id:5842,x:32546,y:33618,varname:node_5842,prsc:2|A-3296-OUT,B-496-OUT;n:type:ShaderForge.SFN_Tex2d,id:5289,x:31992,y:33626,varname:node_5289,prsc:2,ntxv:0,isnm:False|UVIN-2790-OUT,TEX-814-TEX;n:type:ShaderForge.SFN_Multiply,id:2666,x:33573,y:32564,varname:node_2666,prsc:2|A-4573-OUT,B-4291-OUT;n:type:ShaderForge.SFN_Slider,id:496,x:32325,y:33870,ptovrint:False,ptlb:basicNoiseMult,ptin:_basicNoiseMult,varname:_basicNoiseMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Power,id:3296,x:32269,y:33617,varname:node_3296,prsc:2|VAL-5289-RGB,EXP-7343-OUT;n:type:ShaderForge.SFN_Slider,id:7343,x:31947,y:33867,ptovrint:False,ptlb:basicNoisePow,ptin:_basicNoisePow,varname:_basicNoisePow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_TexCoord,id:7155,x:30845,y:33508,varname:node_7155,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:6179,x:31220,y:33616,varname:node_6179,prsc:2|A-7155-UVOUT,B-3264-OUT;n:type:ShaderForge.SFN_Slider,id:4029,x:30708,y:33717,ptovrint:False,ptlb:basicNoiseTileX,ptin:_basicNoiseTileX,varname:_basicNoiseTileX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Time,id:2482,x:30822,y:33993,varname:node_2482,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7217,x:31236,y:34091,varname:node_7217,prsc:2|A-2482-T,B-6140-OUT;n:type:ShaderForge.SFN_Slider,id:3292,x:30573,y:34187,ptovrint:False,ptlb:basicNoisePanX,ptin:_basicNoisePanX,varname:_basicNoisePanX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Slider,id:8849,x:30573,y:34288,ptovrint:False,ptlb:basicNoisePanY,ptin:_basicNoisePanY,varname:_basicNoisePanY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Append,id:6140,x:30970,y:34209,varname:node_6140,prsc:2|A-3292-OUT,B-8849-OUT;n:type:ShaderForge.SFN_Add,id:2790,x:31437,y:33616,varname:node_2790,prsc:2|A-6179-OUT,B-7217-OUT;n:type:ShaderForge.SFN_Slider,id:6373,x:30708,y:33855,ptovrint:False,ptlb:basicNoiseTileY,ptin:_basicNoiseTileY,varname:_basicNoiseTileY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Append,id:3264,x:31043,y:33761,varname:node_3264,prsc:2|A-4029-OUT,B-6373-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:814,x:31514,y:33311,ptovrint:False,ptlb:basicNoise,ptin:_basicNoise,varname:_basicNoise,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:196,x:32643,y:34573,varname:node_196,prsc:2|A-2262-OUT,B-8872-OUT;n:type:ShaderForge.SFN_Slider,id:8872,x:32422,y:34825,ptovrint:False,ptlb:basicNoiseMult_copy,ptin:_basicNoiseMult_copy,varname:_basicNoiseMult_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Power,id:2262,x:32366,y:34572,varname:node_2262,prsc:2|VAL-2773-RGB,EXP-7795-OUT;n:type:ShaderForge.SFN_Slider,id:7795,x:32044,y:34822,ptovrint:False,ptlb:basicNoisePow_copy,ptin:_basicNoisePow_copy,varname:_basicNoisePow_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_TexCoord,id:5785,x:30942,y:34463,varname:node_5785,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:9073,x:31317,y:34571,varname:node_9073,prsc:2|A-5785-UVOUT,B-2254-OUT;n:type:ShaderForge.SFN_Slider,id:3848,x:30805,y:34672,ptovrint:False,ptlb:basicNoiseTileX_copy,ptin:_basicNoiseTileX_copy,varname:_basicNoiseTileX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Time,id:2867,x:30919,y:34948,varname:node_2867,prsc:2;n:type:ShaderForge.SFN_Multiply,id:108,x:31333,y:35046,varname:node_108,prsc:2|A-2867-T,B-2963-OUT;n:type:ShaderForge.SFN_Slider,id:2371,x:30670,y:35142,ptovrint:False,ptlb:basicNoisePanX_copy,ptin:_basicNoisePanX_copy,varname:_basicNoisePanX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Slider,id:5795,x:30670,y:35243,ptovrint:False,ptlb:basicNoisePanY_copy,ptin:_basicNoisePanY_copy,varname:_basicNoisePanY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Append,id:2963,x:31067,y:35164,varname:node_2963,prsc:2|A-2371-OUT,B-5795-OUT;n:type:ShaderForge.SFN_Add,id:8026,x:31534,y:34571,varname:node_8026,prsc:2|A-9073-OUT,B-108-OUT;n:type:ShaderForge.SFN_Slider,id:6890,x:30805,y:34810,ptovrint:False,ptlb:basicNoiseTileY_copy,ptin:_basicNoiseTileY_copy,varname:_basicNoiseTileY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Append,id:2254,x:31140,y:34716,varname:node_2254,prsc:2|A-3848-OUT,B-6890-OUT;n:type:ShaderForge.SFN_Multiply,id:6842,x:32985,y:34039,varname:node_6842,prsc:2|A-5842-OUT,B-196-OUT;n:type:ShaderForge.SFN_Tex2d,id:2773,x:32069,y:34574,varname:node_2773,prsc:2,ntxv:0,isnm:False|UVIN-8026-OUT,TEX-814-TEX;n:type:ShaderForge.SFN_TexCoord,id:3525,x:37253,y:31618,varname:node_3525,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:4187,x:38637,y:31871,varname:node_4187,prsc:2|A-6865-OUT,B-6055-OUT;n:type:ShaderForge.SFN_Negate,id:6055,x:38481,y:32077,varname:node_6055,prsc:2|IN-9723-OUT;n:type:ShaderForge.SFN_Power,id:6532,x:39239,y:31608,varname:node_6532,prsc:2|VAL-3394-OUT,EXP-5276-OUT;n:type:ShaderForge.SFN_Slider,id:5276,x:38851,y:31756,ptovrint:False,ptlb:edgeMaskPower,ptin:_edgeMaskPower,varname:_edgeMaskPower,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:32.14378,max:155;n:type:ShaderForge.SFN_Power,id:9471,x:39255,y:31854,varname:node_9471,prsc:2|VAL-5753-OUT,EXP-5276-OUT;n:type:ShaderForge.SFN_Multiply,id:4016,x:39763,y:31746,varname:node_4016,prsc:2|A-9009-OUT,B-9471-OUT;n:type:ShaderForge.SFN_Clamp01,id:9009,x:39466,y:31608,varname:node_9009,prsc:2|IN-6532-OUT;n:type:ShaderForge.SFN_Clamp01,id:6094,x:38826,y:31858,varname:node_6094,prsc:2|IN-4187-OUT;n:type:ShaderForge.SFN_OneMinus,id:5753,x:39008,y:31858,varname:node_5753,prsc:2|IN-6094-OUT;n:type:ShaderForge.SFN_Slider,id:7992,x:37905,y:32057,ptovrint:False,ptlb:edgeMaskWidth,ptin:_edgeMaskWidth,varname:_edgeMaskWidth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8641879,max:1;n:type:ShaderForge.SFN_OneMinus,id:9723,x:38302,y:32077,varname:node_9723,prsc:2|IN-7992-OUT;n:type:ShaderForge.SFN_NormalVector,id:183,x:40323,y:33291,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:5285,x:40523,y:33208,varname:node_5285,prsc:2|A-6190-OUT,B-183-OUT;n:type:ShaderForge.SFN_Slider,id:6190,x:40166,y:33178,ptovrint:False,ptlb:geometryOffset,ptin:_geometryOffset,varname:_geometryOffset,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-20,cur:0,max:20;n:type:ShaderForge.SFN_Multiply,id:3405,x:40461,y:33966,varname:node_3405,prsc:2|A-462-OUT,B-7997-OUT;n:type:ShaderForge.SFN_Power,id:462,x:40184,y:33965,varname:node_462,prsc:2|VAL-4465-RGB,EXP-3424-OUT;n:type:ShaderForge.SFN_TexCoord,id:4387,x:39076,y:33866,varname:node_4387,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:5345,x:39451,y:33974,varname:node_5345,prsc:2|A-4387-UVOUT,B-9154-OUT;n:type:ShaderForge.SFN_Time,id:2851,x:39218,y:34351,varname:node_2851,prsc:2;n:type:ShaderForge.SFN_Multiply,id:189,x:39467,y:34449,varname:node_189,prsc:2|A-2851-T,B-7075-OUT;n:type:ShaderForge.SFN_Append,id:7075,x:39293,y:34576,varname:node_7075,prsc:2|A-5309-OUT,B-4413-OUT;n:type:ShaderForge.SFN_Add,id:5751,x:39668,y:33974,varname:node_5751,prsc:2|A-5345-OUT,B-189-OUT;n:type:ShaderForge.SFN_Append,id:9154,x:39274,y:34119,varname:node_9154,prsc:2|A-1444-OUT,B-543-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:7454,x:39550,y:34720,ptovrint:False,ptlb:geometryOffsetNoise,ptin:_geometryOffsetNoise,varname:_geometryOffsetNoise,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1632,x:40935,y:33208,varname:node_1632,prsc:2|A-5285-OUT,B-3405-OUT;n:type:ShaderForge.SFN_Tex2d,id:4465,x:39921,y:33974,varname:node_4465,prsc:2,ntxv:0,isnm:False|UVIN-5751-OUT,TEX-7454-TEX;n:type:ShaderForge.SFN_Slider,id:7997,x:40290,y:34191,ptovrint:False,ptlb:offsetNoiseMult,ptin:_offsetNoiseMult,varname:_offsetNoiseMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Slider,id:3424,x:39886,y:34184,ptovrint:False,ptlb:offsetNoisePow,ptin:_offsetNoisePow,varname:_offsetNoisePow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Slider,id:1444,x:38852,y:34077,ptovrint:False,ptlb:offsetNoiseTileX,ptin:_offsetNoiseTileX,varname:_offsetNoiseTileX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:543,x:38852,y:34218,ptovrint:False,ptlb:offsetNoiseTileY,ptin:_offsetNoiseTileY,varname:_offsetNoiseTileY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Slider,id:5309,x:39007,y:34562,ptovrint:False,ptlb:offsetNoisePanX,ptin:_offsetNoisePanX,varname:_offsetNoisePanX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Slider,id:4413,x:39018,y:34641,ptovrint:False,ptlb:offsetNoisePanY,ptin:_offsetNoisePanY,varname:_offsetNoisePanY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Vector1,id:3308,x:40226,y:32980,varname:node_3308,prsc:2,v1:24;n:type:ShaderForge.SFN_Vector1,id:7233,x:35184,y:31980,varname:node_7233,prsc:2,v1:1;n:type:ShaderForge.SFN_ObjectPosition,id:2862,x:34540,y:31608,varname:node_2862,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:5383,x:34552,y:31837,varname:node_5383,prsc:2;n:type:ShaderForge.SFN_Distance,id:9030,x:34880,y:31700,varname:node_9030,prsc:2|A-1106-OUT,B-2647-OUT;n:type:ShaderForge.SFN_Divide,id:2149,x:35337,y:31729,varname:node_2149,prsc:2|A-1793-OUT,B-9615-OUT;n:type:ShaderForge.SFN_Slider,id:9615,x:35063,y:31894,ptovrint:False,ptlb:radialMaskDistanceScaler,ptin:_radialMaskDistanceScaler,varname:_radialMaskDistanceScaler,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:52.09377,max:10000;n:type:ShaderForge.SFN_OneMinus,id:5213,x:35742,y:31794,varname:node_5213,prsc:2|IN-4337-OUT;n:type:ShaderForge.SFN_Abs,id:1793,x:35089,y:31700,varname:node_1793,prsc:2|IN-9030-OUT;n:type:ShaderForge.SFN_Append,id:1106,x:34708,y:31666,varname:node_1106,prsc:2|A-2862-X,B-2862-Z;n:type:ShaderForge.SFN_Append,id:2647,x:34750,y:31837,varname:node_2647,prsc:2|A-5383-X,B-5383-Z;n:type:ShaderForge.SFN_Clamp01,id:4337,x:35563,y:31816,varname:node_4337,prsc:2|IN-2149-OUT;n:type:ShaderForge.SFN_Power,id:5441,x:35928,y:31778,varname:node_5441,prsc:2|VAL-5213-OUT,EXP-1516-OUT;n:type:ShaderForge.SFN_Slider,id:1516,x:35450,y:32036,ptovrint:False,ptlb:radialMaskPow,ptin:_radialMaskPow,varname:_radialMaskPow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:50;n:type:ShaderForge.SFN_Multiply,id:3020,x:36101,y:32517,varname:node_3020,prsc:2|A-5441-OUT,B-2666-OUT;n:type:ShaderForge.SFN_Fresnel,id:4376,x:36901,y:32340,varname:node_4376,prsc:2|EXP-2786-OUT;n:type:ShaderForge.SFN_Slider,id:2786,x:36550,y:32353,ptovrint:False,ptlb:fresnelPow,ptin:_fresnelPow,varname:_fresnelPow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7682292,max:15;n:type:ShaderForge.SFN_OneMinus,id:3221,x:37095,y:32340,varname:node_3221,prsc:2|IN-4376-OUT;n:type:ShaderForge.SFN_Multiply,id:6720,x:37015,y:32507,varname:node_6720,prsc:2|A-3221-OUT,B-3020-OUT;n:type:ShaderForge.SFN_Multiply,id:1060,x:37067,y:33423,varname:node_1060,prsc:2|A-7068-OUT,B-7424-OUT;n:type:ShaderForge.SFN_Power,id:7068,x:36790,y:33422,varname:node_7068,prsc:2|VAL-931-R,EXP-8056-OUT;n:type:ShaderForge.SFN_TexCoord,id:6317,x:35366,y:33313,varname:node_6317,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Multiply,id:7090,x:35741,y:33421,varname:node_7090,prsc:2|A-6317-UVOUT,B-2404-OUT;n:type:ShaderForge.SFN_Slider,id:7051,x:34903,y:33507,ptovrint:True,ptlb:largeDetailNoiseTileX,ptin:_largeDetailNoiseTileX,varname:_largeDetailNoiseTileX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Time,id:2325,x:35343,y:33798,varname:node_2325,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8612,x:35757,y:33896,varname:node_8612,prsc:2|A-2325-T,B-7243-OUT;n:type:ShaderForge.SFN_Slider,id:4320,x:35045,y:33945,ptovrint:True,ptlb:largeDetailNoisePanX,ptin:_largeDetailNoisePanX,varname:_largeDetailNoisePanX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Slider,id:9115,x:35094,y:34093,ptovrint:True,ptlb:largeDetailNoisePanY,ptin:_largeDetailNoisePanY,varname:_largeDetailNoisePanY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Append,id:7243,x:35491,y:34014,varname:node_7243,prsc:2|A-4320-OUT,B-9115-OUT;n:type:ShaderForge.SFN_Add,id:648,x:35958,y:33421,varname:node_648,prsc:2|A-7090-OUT,B-8612-OUT;n:type:ShaderForge.SFN_Slider,id:5034,x:35229,y:33660,ptovrint:True,ptlb:largeDetailNoiseTileY,ptin:_largeDetailNoiseTileY,varname:_largeDetailNoiseTileY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Append,id:2404,x:35564,y:33566,varname:node_2404,prsc:2|A-7051-OUT,B-5034-OUT;n:type:ShaderForge.SFN_Slider,id:7424,x:36855,y:33710,ptovrint:False,ptlb:largeDetailNoiseMult,ptin:_largeDetailNoiseMult,varname:_largeDetailNoiseMult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Slider,id:8056,x:36412,y:33653,ptovrint:False,ptlb:largeDetailNoisePow,ptin:_largeDetailNoisePow,varname:_largeDetailNoisePow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Tex2d,id:931,x:36462,y:33415,varname:node_931,prsc:2,ntxv:0,isnm:False|UVIN-648-OUT,TEX-9983-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:9983,x:36018,y:33147,ptovrint:False,ptlb:largeDetailNoise,ptin:_largeDetailNoise,varname:_largeDetailNoise,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3616,x:37787,y:32547,varname:node_3616,prsc:2|A-6720-OUT,B-1060-OUT,C-9676-OUT;n:type:ShaderForge.SFN_Color,id:7430,x:32529,y:32047,ptovrint:False,ptlb:centralTint,ptin:_centralTint,varname:_centralTint,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:8785,x:32906,y:32037,varname:node_8785,prsc:2|A-6395-RGB,B-7430-RGB,T-3466-OUT;n:type:ShaderForge.SFN_Lerp,id:1221,x:32849,y:32280,varname:node_1221,prsc:2|A-7241-RGB,B-7430-RGB,T-3466-OUT;n:type:ShaderForge.SFN_Set,id:7081,x:36157,y:31858,varname:radialMeshGrad,prsc:2|IN-5441-OUT;n:type:ShaderForge.SFN_Get,id:3466,x:32508,y:32197,varname:node_3466,prsc:2|IN-7081-OUT;n:type:ShaderForge.SFN_TexCoord,id:3381,x:35329,y:35607,varname:node_3381,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:689,x:35723,y:35535,varname:node_689,prsc:2|A-3381-V,B-2816-OUT;n:type:ShaderForge.SFN_Add,id:5354,x:35775,y:35755,varname:node_5354,prsc:2|A-3381-V,B-4874-OUT;n:type:ShaderForge.SFN_Negate,id:4874,x:35600,y:35789,varname:node_4874,prsc:2|IN-2816-OUT;n:type:ShaderForge.SFN_Power,id:6483,x:36388,y:35545,varname:node_6483,prsc:2|VAL-689-OUT,EXP-3623-OUT;n:type:ShaderForge.SFN_Slider,id:3623,x:35984,y:35653,ptovrint:False,ptlb:edgeMaskPower_copy,ptin:_edgeMaskPower_copy,varname:_edgeMaskPower_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:32.14378,max:155;n:type:ShaderForge.SFN_Power,id:6951,x:36388,y:35751,varname:node_6951,prsc:2|VAL-8969-OUT,EXP-3623-OUT;n:type:ShaderForge.SFN_Clamp01,id:7601,x:36579,y:35535,varname:node_7601,prsc:2|IN-6483-OUT;n:type:ShaderForge.SFN_Clamp01,id:9775,x:35959,y:35755,varname:node_9775,prsc:2|IN-5354-OUT;n:type:ShaderForge.SFN_OneMinus,id:8969,x:36141,y:35755,varname:node_8969,prsc:2|IN-9775-OUT;n:type:ShaderForge.SFN_Slider,id:6459,x:35225,y:36039,ptovrint:False,ptlb:edgeMaskWidth_copy,ptin:_edgeMaskWidth_copy,varname:_edgeMaskWidth_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8641879,max:1;n:type:ShaderForge.SFN_OneMinus,id:2816,x:35417,y:35772,varname:node_2816,prsc:2|IN-6459-OUT;n:type:ShaderForge.SFN_Multiply,id:5966,x:37311,y:34472,varname:node_5966,prsc:2|A-9181-OUT,B-4751-OUT;n:type:ShaderForge.SFN_Power,id:9181,x:36913,y:34473,varname:node_9181,prsc:2|VAL-7435-R,EXP-2025-OUT;n:type:ShaderForge.SFN_TexCoord,id:7597,x:35489,y:34364,varname:node_7597,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Multiply,id:4046,x:35864,y:34472,varname:node_4046,prsc:2|A-7597-UVOUT,B-4381-OUT;n:type:ShaderForge.SFN_Slider,id:4730,x:35026,y:34558,ptovrint:True,ptlb:largeDetailNoiseTileX_copy,ptin:_largeDetailNoiseTileX_copy,varname:_largeDetailNoiseTileX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Time,id:3703,x:35466,y:34849,varname:node_3703,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9812,x:35880,y:34947,varname:node_9812,prsc:2|A-3703-T,B-3411-OUT;n:type:ShaderForge.SFN_Slider,id:1689,x:35168,y:34996,ptovrint:True,ptlb:largeDetailNoisePanX_copy,ptin:_largeDetailNoisePanX_copy,varname:_largeDetailNoisePanX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Slider,id:6342,x:35188,y:35155,ptovrint:True,ptlb:largeDetailNoisePanY_copy,ptin:_largeDetailNoisePanY_copy,varname:_largeDetailNoisePanY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Append,id:3411,x:35614,y:35065,varname:node_3411,prsc:2|A-1689-OUT,B-6342-OUT;n:type:ShaderForge.SFN_Add,id:9350,x:36081,y:34472,varname:node_9350,prsc:2|A-4046-OUT,B-9812-OUT;n:type:ShaderForge.SFN_Slider,id:7428,x:35352,y:34711,ptovrint:True,ptlb:largeDetailNoiseTileY_copy,ptin:_largeDetailNoiseTileY_copy,varname:_largeDetailNoiseTileY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Append,id:4381,x:35687,y:34617,varname:node_4381,prsc:2|A-4730-OUT,B-7428-OUT;n:type:ShaderForge.SFN_Slider,id:4751,x:36978,y:34761,ptovrint:False,ptlb:largeDetailNoiseMult_copy,ptin:_largeDetailNoiseMult_copy,varname:_largeDetailNoiseMult_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Slider,id:2025,x:36535,y:34704,ptovrint:False,ptlb:largeDetailNoisePow_copy,ptin:_largeDetailNoisePow_copy,varname:_largeDetailNoisePow_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Tex2d,id:7435,x:36535,y:34467,varname:node_7435,prsc:2,ntxv:0,isnm:False|UVIN-9350-OUT,TEX-9983-TEX;n:type:ShaderForge.SFN_Add,id:3117,x:36917,y:35651,varname:node_3117,prsc:2|A-7601-OUT,B-6951-OUT;n:type:ShaderForge.SFN_Multiply,id:6081,x:39603,y:32536,varname:node_6081,prsc:2|A-6827-OUT,B-3350-OUT;n:type:ShaderForge.SFN_Lerp,id:6827,x:39020,y:32696,varname:node_6827,prsc:2|A-3616-OUT,B-3714-OUT,T-3117-OUT;n:type:ShaderForge.SFN_Multiply,id:1160,x:41156,y:32625,varname:node_1160,prsc:2|A-6389-OUT,B-743-OUT;n:type:ShaderForge.SFN_Slider,id:743,x:40729,y:32837,ptovrint:False,ptlb:emissive,ptin:_emissive,varname:_emissive,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1000;n:type:ShaderForge.SFN_Subtract,id:1329,x:38244,y:32631,varname:node_1329,prsc:2|A-3616-OUT,B-5966-OUT;n:type:ShaderForge.SFN_Clamp01,id:3714,x:38497,y:32631,varname:node_3714,prsc:2|IN-1329-OUT;n:type:ShaderForge.SFN_Multiply,id:5151,x:40249,y:32549,varname:node_5151,prsc:2|A-6081-OUT,B-5373-R;n:type:ShaderForge.SFN_VertexColor,id:5373,x:39877,y:32595,varname:node_5373,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:3427,x:37348,y:31894,varname:node_3427,prsc:2,ntxv:0,isnm:False|UVIN-1046-OUT,TEX-7454-TEX;n:type:ShaderForge.SFN_Lerp,id:4546,x:38032,y:31619,varname:node_4546,prsc:2|A-3525-UVOUT,B-9910-OUT,T-6144-OUT;n:type:ShaderForge.SFN_Add,id:9910,x:37550,y:31777,varname:node_9910,prsc:2|A-3525-UVOUT,B-3427-R;n:type:ShaderForge.SFN_Slider,id:252,x:36717,y:31881,ptovrint:False,ptlb:edgeMaskDistortionTexTile,ptin:_edgeMaskDistortionTexTile,varname:_edgeMaskDistortionTexTile,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:5;n:type:ShaderForge.SFN_TexCoord,id:2960,x:36853,y:31664,varname:node_2960,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:1046,x:37055,y:31813,varname:node_1046,prsc:2|A-2960-UVOUT,B-252-OUT;n:type:ShaderForge.SFN_Slider,id:6144,x:37721,y:31872,ptovrint:False,ptlb:edgeMaskDistortion,ptin:_edgeMaskDistortion,varname:_edgeMaskDistortion,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_ComponentMask,id:6865,x:38239,y:31528,varname:node_6865,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-4546-OUT;n:type:ShaderForge.SFN_Multiply,id:5727,x:32338,y:35874,varname:node_5727,prsc:2|A-5951-OUT,B-6995-OUT;n:type:ShaderForge.SFN_Slider,id:6995,x:32117,y:36126,ptovrint:False,ptlb:basicNoise2Mult,ptin:_basicNoise2Mult,varname:_basicNoise2Mult,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Power,id:5951,x:32061,y:35873,varname:node_5951,prsc:2|VAL-6879-RGB,EXP-9015-OUT;n:type:ShaderForge.SFN_Slider,id:4795,x:31565,y:36416,ptovrint:False,ptlb:basicNoise2Pow,ptin:_basicNoise2Pow,varname:_basicNoise2Pow,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_TexCoord,id:9043,x:30637,y:35764,varname:node_9043,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:8191,x:31012,y:35872,varname:node_8191,prsc:2|A-9043-UVOUT,B-7780-OUT;n:type:ShaderForge.SFN_Slider,id:9318,x:30500,y:35973,ptovrint:False,ptlb:basicNoise2TileX,ptin:_basicNoise2TileX,varname:_basicNoise2TileX,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Time,id:3263,x:30614,y:36249,varname:node_3263,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8146,x:31028,y:36347,varname:node_8146,prsc:2|A-3263-T,B-7278-OUT;n:type:ShaderForge.SFN_Slider,id:9629,x:30365,y:36443,ptovrint:False,ptlb:basicNoise2PanX,ptin:_basicNoise2PanX,varname:_basicNoise2PanX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Slider,id:2466,x:30365,y:36544,ptovrint:False,ptlb:basicNoise2PanY,ptin:_basicNoise2PanY,varname:_basicNoise2PanY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:0,max:10;n:type:ShaderForge.SFN_Append,id:7278,x:30762,y:36465,varname:node_7278,prsc:2|A-9629-OUT,B-2466-OUT;n:type:ShaderForge.SFN_Add,id:1864,x:31229,y:35872,varname:node_1864,prsc:2|A-8191-OUT,B-8146-OUT;n:type:ShaderForge.SFN_Slider,id:5683,x:30500,y:36111,ptovrint:False,ptlb:basicNoise2TileY,ptin:_basicNoise2TileY,varname:_basicNoise2TileY,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Append,id:7780,x:30835,y:36017,varname:node_7780,prsc:2|A-9318-OUT,B-5683-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:3842,x:31306,y:35567,ptovrint:False,ptlb:basicNoise2,ptin:_basicNoise2,varname:_basicNoise2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:5556,x:32435,y:36829,varname:node_5556,prsc:2|A-1359-OUT,B-9789-OUT;n:type:ShaderForge.SFN_Slider,id:9789,x:32435,y:37109,ptovrint:False,ptlb:basicNoise2Mult_copy,ptin:_basicNoise2Mult_copy,varname:_basicNoise2Mult_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Power,id:1359,x:32158,y:36828,varname:node_1359,prsc:2|VAL-9118-RGB,EXP-1124-OUT;n:type:ShaderForge.SFN_Slider,id:1124,x:31579,y:37245,ptovrint:False,ptlb:basicNoise2Pow_copy,ptin:_basicNoise2Pow_copy,varname:_basicNoise2Pow_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_TexCoord,id:8251,x:30734,y:36719,varname:node_8251,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:7213,x:31109,y:36827,varname:node_7213,prsc:2|A-8251-UVOUT,B-3460-OUT;n:type:ShaderForge.SFN_Slider,id:2160,x:30597,y:36928,ptovrint:False,ptlb:basicNoise2TileX_copy,ptin:_basicNoise2TileX_copy,varname:_basicNoise2TileX_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Time,id:7286,x:30711,y:37204,varname:node_7286,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5229,x:31125,y:37302,varname:node_5229,prsc:2|A-7286-T,B-678-OUT;n:type:ShaderForge.SFN_Append,id:678,x:30859,y:37420,varname:node_678,prsc:2|A-2977-OUT,B-4622-OUT;n:type:ShaderForge.SFN_Add,id:2291,x:31326,y:36827,varname:node_2291,prsc:2|A-7213-OUT,B-5229-OUT;n:type:ShaderForge.SFN_Slider,id:2621,x:30597,y:37066,ptovrint:False,ptlb:basicNoise2TileY_copy,ptin:_basicNoise2TileY_copy,varname:_basicNoise2TileY_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:5;n:type:ShaderForge.SFN_Append,id:3460,x:30932,y:36972,varname:node_3460,prsc:2|A-2160-OUT,B-2621-OUT;n:type:ShaderForge.SFN_Multiply,id:1500,x:32777,y:36295,varname:node_1500,prsc:2|A-5727-OUT,B-5556-OUT;n:type:ShaderForge.SFN_Tex2d,id:6879,x:31678,y:35890,varname:node_6879,prsc:2,ntxv:0,isnm:False|UVIN-1864-OUT,TEX-3842-TEX;n:type:ShaderForge.SFN_Tex2d,id:9118,x:31699,y:36828,varname:node_9118,prsc:2,ntxv:0,isnm:False|UVIN-2291-OUT,TEX-3842-TEX;n:type:ShaderForge.SFN_Slider,id:2977,x:30387,y:37353,ptovrint:False,ptlb:basicNoise2PanX_copy,ptin:_basicNoise2PanX_copy,varname:_node_2977,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:1,max:10;n:type:ShaderForge.SFN_Slider,id:4622,x:30387,y:37471,ptovrint:False,ptlb:basicNoise2PanY_copy,ptin:_basicNoise2PanY_copy,varname:_basicNoise2PanX_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-10,cur:1,max:10;n:type:ShaderForge.SFN_Multiply,id:4291,x:33504,y:34931,varname:node_4291,prsc:2|A-9200-OUT,B-1500-OUT;n:type:ShaderForge.SFN_Power,id:6389,x:40609,y:32578,varname:node_6389,prsc:2|VAL-5151-OUT,EXP-4504-OUT;n:type:ShaderForge.SFN_Slider,id:4504,x:40264,y:32777,ptovrint:False,ptlb:emissivePow,ptin:_emissivePow,varname:_emissive_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Clamp01,id:9200,x:33282,y:34240,varname:node_9200,prsc:2|IN-6842-OUT;n:type:ShaderForge.SFN_Add,id:3394,x:38474,y:31554,varname:node_3394,prsc:2|A-6865-OUT,B-7992-OUT;n:type:ShaderForge.SFN_TexCoord,id:2583,x:35099,y:30194,varname:node_2583,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:2048,x:35793,y:30200,varname:node_2048,prsc:2|A-5920-OUT,B-2485-OUT;n:type:ShaderForge.SFN_Add,id:4356,x:35793,y:30548,varname:node_4356,prsc:2|A-5920-OUT,B-658-OUT;n:type:ShaderForge.SFN_Power,id:9433,x:36458,y:30210,varname:node_9433,prsc:2|VAL-5640-OUT,EXP-1969-OUT;n:type:ShaderForge.SFN_Slider,id:1969,x:36049,y:30448,ptovrint:False,ptlb:bottomPower,ptin:_bottomPower,varname:_edgeMaskPower,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5,max:55;n:type:ShaderForge.SFN_Power,id:6128,x:36677,y:30587,varname:node_6128,prsc:2|VAL-7872-OUT,EXP-4294-OUT;n:type:ShaderForge.SFN_Multiply,id:9676,x:36993,y:30438,varname:node_9676,prsc:2|A-1932-OUT,B-6128-OUT;n:type:ShaderForge.SFN_Clamp01,id:1932,x:36726,y:30216,varname:node_1932,prsc:2|IN-9433-OUT;n:type:ShaderForge.SFN_Clamp01,id:6933,x:36049,y:30608,varname:node_6933,prsc:2|IN-4356-OUT;n:type:ShaderForge.SFN_OneMinus,id:7872,x:36336,y:30609,varname:node_7872,prsc:2|IN-6933-OUT;n:type:ShaderForge.SFN_Slider,id:6488,x:35111,y:30664,ptovrint:False,ptlb:topOffset,ptin:_topOffset,varname:_edgeMaskWidth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.18,max:1;n:type:ShaderForge.SFN_ComponentMask,id:5920,x:35353,y:30194,varname:node_5920,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-4546-OUT;n:type:ShaderForge.SFN_Slider,id:2485,x:35255,y:30008,ptovrint:False,ptlb:bottomOffset,ptin:_bottomOffset,varname:node_2538,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:4294,x:36097,y:30861,ptovrint:False,ptlb:topPower,ptin:_topPower,varname:node_7264,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:55;n:type:ShaderForge.SFN_OneMinus,id:3447,x:35461,y:30682,varname:node_3447,prsc:2|IN-6488-OUT;n:type:ShaderForge.SFN_Negate,id:658,x:35618,y:30682,varname:node_658,prsc:2|IN-3447-OUT;n:type:ShaderForge.SFN_Vector1,id:3350,x:39390,y:32654,varname:node_3350,prsc:2,v1:1;n:type:ShaderForge.SFN_Clamp01,id:5640,x:36127,y:30230,varname:node_5640,prsc:2|IN-2048-OUT;n:type:ShaderForge.SFN_Time,id:2526,x:32168,y:37798,varname:node_2526,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:633,x:32226,y:38250,varname:node_8053,prsc:2,ntxv:0,isnm:False|UVIN-6831-OUT,TEX-814-TEX;n:type:ShaderForge.SFN_Add,id:407,x:32862,y:37966,varname:node_407,prsc:2|A-2550-OUT,B-1687-OUT;n:type:ShaderForge.SFN_Multiply,id:1687,x:32624,y:38265,varname:node_1687,prsc:2|A-3965-OUT,B-8426-OUT;n:type:ShaderForge.SFN_Vector1,id:8426,x:32496,y:38473,varname:node_8426,prsc:2,v1:5;n:type:ShaderForge.SFN_TexCoord,id:9373,x:31791,y:38212,varname:node_9373,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Sin,id:1460,x:33146,y:37969,varname:node_1460,prsc:2|IN-407-OUT;n:type:ShaderForge.SFN_Multiply,id:6831,x:32034,y:38250,varname:node_6831,prsc:2|A-9373-UVOUT,B-5548-OUT;n:type:ShaderForge.SFN_Vector2,id:5548,x:31791,y:38467,varname:node_5548,prsc:2,v1:0.1,v2:0.5;n:type:ShaderForge.SFN_Multiply,id:4729,x:34257,y:37974,varname:node_4729,prsc:2|A-2595-OUT,B-1487-OUT;n:type:ShaderForge.SFN_Slider,id:1487,x:33650,y:38268,ptovrint:False,ptlb:modulateTextureAmount,ptin:_modulateTextureAmount,varname:node_1487,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Add,id:9015,x:31979,y:36289,varname:node_9015,prsc:2|A-4729-OUT,B-4795-OUT;n:type:ShaderForge.SFN_Power,id:3965,x:32440,y:38265,varname:node_3965,prsc:2|VAL-633-R,EXP-4206-OUT;n:type:ShaderForge.SFN_Vector1,id:4206,x:32259,y:38452,varname:node_4206,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Clamp01,id:2595,x:33434,y:37962,varname:node_2595,prsc:2|IN-1460-OUT;n:type:ShaderForge.SFN_Divide,id:2550,x:32595,y:37914,varname:node_2550,prsc:2|A-2526-T,B-3722-OUT;n:type:ShaderForge.SFN_Vector1,id:5875,x:32122,y:37962,varname:node_5875,prsc:2,v1:1;n:type:ShaderForge.SFN_Divide,id:3722,x:32404,y:38023,varname:node_3722,prsc:2|A-5875-OUT,B-8948-OUT;n:type:ShaderForge.SFN_Slider,id:8948,x:31773,y:38055,ptovrint:False,ptlb:modulationFreq,ptin:_modulationFreq,varname:node_8948,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2125662,max:0.99;proporder:6395-7241-1143-3547-814-496-7343-4029-6373-3292-8849-8872-7795-3848-6890-2371-5795-5276-7992-252-6144-6190-7454-7997-3424-1444-543-5309-4413-9615-1516-9983-2786-7424-8056-7051-5034-4320-9115-7430-3623-6459-4730-1689-6342-7428-4751-2025-743-6995-4795-3842-9789-1124-2160-2621-9318-9629-2466-5683-2977-4622-4504-1969-2485-6488-4294-1487-8948;pass:END;sub:END;*/

Shader "Colony_FX/S_AuroraBorealis" {
Properties {
		[Header(Colors)]
		[Space]
        _ColorTop ("ColorTop", Color) = (0.9622642,0.4667079,0.3948915,1)
        _ColorBottom ("ColorBottom", Color) = (0.3726415,1,0.9178863,1)
		_centralTint ("centralTint", Color) = (0.5,0.5,0.5,1)
		
		[Space]
		[Header(WP based color gradient controls)]
		[Space]
        _meshPositionY ("meshPositionY", Range(0, 500)) = 0
        _meshScaleDivisor ("meshScaleDivisor", Range(0, 1000)) = 100
		
		[Space]
		[Header(Radial color masking controls)]
		[Space]		
		_radialMaskDistanceScaler ("radialMaskDistanceScaler", Range(0, 10000)) = 52.09377
        _radialMaskPow ("radialMaskPow", Range(0, 50)) = 0		
		
		[Space]
		[Header(Noisemap 1)]
		[Space]
        _basicNoise ("basicNoise", 2D) = "white" {}
        _basicNoiseMult ("basicNoiseMult", Range(0, 5)) = 1
        _basicNoisePow ("basicNoisePow", Range(0, 5)) = 1
        _basicNoiseTileX ("basicNoiseTileX", Range(0, 5)) = 1
        _basicNoiseTileY ("basicNoiseTileY", Range(0, 5)) = 1
        _basicNoisePanX ("basicNoisePanX", Range(-10, 10)) = 0
        _basicNoisePanY ("basicNoisePanY", Range(-10, 10)) = 0
        _basicNoiseMult_copy ("basicNoiseMult_copy", Range(0, 5)) = 1
        _basicNoisePow_copy ("basicNoisePow_copy", Range(0, 5)) = 1
        _basicNoiseTileX_copy ("basicNoiseTileX_copy", Range(0, 5)) = 1
        _basicNoiseTileY_copy ("basicNoiseTileY_copy", Range(0, 5)) = 1
        _basicNoisePanX_copy ("basicNoisePanX_copy", Range(-10, 10)) = 0
        _basicNoisePanY_copy ("basicNoisePanY_copy", Range(-10, 10)) = 0
		
		[Space]
		[Header(Noisemap 2)]
		[Space]
		_basicNoise2 ("basicNoise2", 2D) = "white" {}
		_basicNoise2Mult ("basicNoise2Mult", Range(0, 5)) = 1
        _basicNoise2Pow ("basicNoise2Pow", Range(0, 5)) = 1               		
        _basicNoise2TileX ("basicNoise2TileX", Range(0, 5)) = 1
		_basicNoise2TileY ("basicNoise2TileY", Range(0, 5)) = 1
        _basicNoise2PanX ("basicNoise2PanX", Range(-10, 10)) = 0
        _basicNoise2PanY ("basicNoise2PanY", Range(-10, 10)) = 0        
		_basicNoise2Mult_copy ("basicNoise2Mult_copy", Range(0, 5)) = 1
        _basicNoise2Pow_copy ("basicNoise2Pow_copy", Range(0, 5)) = 1
		_basicNoise2TileX_copy ("basicNoise2TileX_copy", Range(0, 5)) = 1
        _basicNoise2TileY_copy ("basicNoise2TileY_copy", Range(0, 5)) = 1
        _basicNoise2PanX_copy ("basicNoise2PanX_copy", Range(-10, 10)) = 1
        _basicNoise2PanY_copy ("basicNoise2PanY_copy", Range(-10, 10)) = 1
		
		[Space]
		[Header(Horizontal edges masking controls)]
		[Space]
		_edgeMaskPower ("edgeMaskPower(unused)", Range(0, 155)) = 32.14378
        _edgeMaskWidth ("edgeMaskWidth(unused)", Range(0, 1)) = 0.8641879
        _edgeMaskDistortionTexTile ("edgeMaskDistortionTexTile", Range(0, 5)) = 2
        _edgeMaskDistortion ("edgeMaskDistortion", Range(0, 1)) = 0
		 _bottomPower ("bottomPower", Range(0, 55)) = 5
        _bottomOffset ("bottomOffset", Range(-1, 1)) = 0
        _topOffset ("topOffset", Range(0, 1)) = 0.18
        _topPower ("topPower", Range(0, 55)) = 0
		
		
		[Space]
		[Header(Fresnel)]
		[Space]
		
		_fresnelPow ("fresnelPow", Range(0, 15)) = 0.7682292		
		
		[Space]
		[Header(Geometry offset)]
		[Space]
		
        _geometryOffset ("geometryOffset", Range(-20, 20)) = 0
        _geometryOffsetNoise ("geometryOffsetNoise", 2D) = "white" {}
        _offsetNoiseMult ("offsetNoiseMult", Range(0, 5)) = 1
        _offsetNoisePow ("offsetNoisePow", Range(0, 5)) = 1
        _offsetNoiseTileX ("offsetNoiseTileX", Range(0, 5)) = 0
        _offsetNoiseTileY ("offsetNoiseTileY", Range(0, 5)) = 0
        _offsetNoisePanX ("offsetNoisePanX", Range(-10, 10)) = 0
        _offsetNoisePanY ("offsetNoisePanY", Range(-10, 10)) = 0		
		
		[Space]
		[Header(Large detail noisemap)]
		[Space]        
		
        _largeDetailNoise ("largeDetailNoise", 2D) = "white" {}        
        _largeDetailNoiseMult ("largeDetailNoiseMult", Range(0, 5)) = 1
        _largeDetailNoisePow ("largeDetailNoisePow", Range(0, 5)) = 1
        _largeDetailNoiseTileX ("largeDetailNoiseTileX", Range(0, 5)) = 1
        _largeDetailNoiseTileY ("largeDetailNoiseTileY", Range(0, 5)) = 1
        _largeDetailNoisePanX ("largeDetailNoisePanX", Range(-10, 10)) = 0
        _largeDetailNoisePanY ("largeDetailNoisePanY", Range(-10, 10)) = 0
		
		
		[Space]
		[Header(Horizontal edges subtractive noise)]
		[Space]
        
        _edgeMaskPower_copy ("edgeMaskPower_copy", Range(0, 155)) = 32.14378
        _edgeMaskWidth_copy ("edgeMaskWidth_copy", Range(0, 1)) = 0.8641879
        _largeDetailNoiseTileX_copy ("largeDetailNoiseTileX_copy", Range(0, 5)) = 1
        _largeDetailNoisePanX_copy ("largeDetailNoisePanX_copy", Range(-10, 10)) = 0
        _largeDetailNoisePanY_copy ("largeDetailNoisePanY_copy", Range(-10, 10)) = 0
        _largeDetailNoiseTileY_copy ("largeDetailNoiseTileY_copy", Range(0, 5)) = 1
        _largeDetailNoiseMult_copy ("largeDetailNoiseMult_copy", Range(0, 5)) = 1
        _largeDetailNoisePow_copy ("largeDetailNoisePow_copy", Range(0, 15)) = 1		
		
		[Space]
		[Header(Noise modulation control (vertical lines))]
		[Space]
		
		_modulateTextureAmount ("modulateTextureAmount", Range(0, 1)) = 0
        _modulationFreq ("modulationFreq", Range(0, 0.99)) = 0.2125662
		
		[Space]
		[Header(Final emissive)]
		[Space]
		
        _emissive ("emissive", Range(0, 1000)) = 1        
        _emissivePow ("emissivePow", Range(0, 15)) = 1
        
        
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
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            //#pragma hull hull
            //#pragma domain domain
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "Tessellation.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform float4 _ColorBottom;
            uniform float _meshPositionY;
            uniform float4 _ColorTop;
            uniform float _meshScaleDivisor;
            uniform float _basicNoiseMult;
            uniform float _basicNoisePow;
            uniform float _basicNoiseTileX;
            uniform float _basicNoisePanX;
            uniform float _basicNoisePanY;
            uniform float _basicNoiseTileY;
            uniform sampler2D _basicNoise; uniform float4 _basicNoise_ST;
            uniform float _basicNoiseMult_copy;
            uniform float _basicNoisePow_copy;
            uniform float _basicNoiseTileX_copy;
            uniform float _basicNoisePanX_copy;
            uniform float _basicNoisePanY_copy;
            uniform float _basicNoiseTileY_copy;
            uniform float _geometryOffset;
            uniform sampler2D _geometryOffsetNoise; uniform float4 _geometryOffsetNoise_ST;
            uniform float _offsetNoiseMult;
            uniform float _offsetNoisePow;
            uniform float _offsetNoiseTileX;
            uniform float _offsetNoiseTileY;
            uniform float _offsetNoisePanX;
            uniform float _offsetNoisePanY;
            uniform float _radialMaskDistanceScaler;
            uniform float _radialMaskPow;
            uniform float _fresnelPow;
            uniform float _largeDetailNoiseTileX;
            uniform float _largeDetailNoisePanX;
            uniform float _largeDetailNoisePanY;
            uniform float _largeDetailNoiseTileY;
            uniform float _largeDetailNoiseMult;
            uniform float _largeDetailNoisePow;
            uniform sampler2D _largeDetailNoise; uniform float4 _largeDetailNoise_ST;
            uniform float4 _centralTint;
            uniform float _edgeMaskPower_copy;
            uniform float _edgeMaskWidth_copy;
            uniform float _largeDetailNoiseTileX_copy;
            uniform float _largeDetailNoisePanX_copy;
            uniform float _largeDetailNoisePanY_copy;
            uniform float _largeDetailNoiseTileY_copy;
            uniform float _largeDetailNoiseMult_copy;
            uniform float _largeDetailNoisePow_copy;
            uniform float _emissive;
            uniform float _edgeMaskDistortionTexTile;
            uniform float _edgeMaskDistortion;
            uniform float _basicNoise2Mult;
            uniform float _basicNoise2Pow;
            uniform float _basicNoise2TileX;
            uniform float _basicNoise2PanX;
            uniform float _basicNoise2PanY;
            uniform float _basicNoise2TileY;
            uniform sampler2D _basicNoise2; uniform float4 _basicNoise2_ST;
            uniform float _basicNoise2Mult_copy;
            uniform float _basicNoise2Pow_copy;
            uniform float _basicNoise2TileX_copy;
            uniform float _basicNoise2TileY_copy;
            uniform float _basicNoise2PanX_copy;
            uniform float _basicNoise2PanY_copy;
            uniform float _emissivePow;
            uniform float _bottomPower;
            uniform float _topOffset;
            uniform float _bottomOffset;
            uniform float _topPower;
            uniform float _modulateTextureAmount;
            uniform float _modulationFreq;
			uniform float _eclipsePower;
			
			
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                float4 node_2851 = _Time;
                float2 node_5751 = ((o.uv0*float2(_offsetNoiseTileX,_offsetNoiseTileY))+(node_2851.g*float2(_offsetNoisePanX,_offsetNoisePanY)));
                float4 node_4465 = tex2Dlod(_geometryOffsetNoise,float4(TRANSFORM_TEX(node_5751, _geometryOffsetNoise),0.0,0));
                v.vertex.xyz += ((_geometryOffset*v.normal)*(pow(node_4465.rgb,_offsetNoisePow)*_offsetNoiseMult));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
			
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 objPos = mul ( unity_ObjectToWorld, float4(0,0,0,1) );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                float node_5441 = pow((1.0 - saturate((abs(distance(float2(objPos.r,objPos.b),float2(i.posWorld.r,i.posWorld.b)))/_radialMaskDistanceScaler))),_radialMaskPow);
                float radialMeshGrad = node_5441;
                float node_3466 = radialMeshGrad;
                float4 node_2482 = _Time;
                float2 node_2790 = ((i.uv0*float2(_basicNoiseTileX,_basicNoiseTileY))+(node_2482.g*float2(_basicNoisePanX,_basicNoisePanY)));
                float4 node_5289 = tex2D(_basicNoise,TRANSFORM_TEX(node_2790, _basicNoise));
                float4 node_2867 = _Time;
                float2 node_8026 = ((i.uv0*float2(_basicNoiseTileX_copy,_basicNoiseTileY_copy))+(node_2867.g*float2(_basicNoisePanX_copy,_basicNoisePanY_copy)));
                float4 node_2773 = tex2D(_basicNoise,TRANSFORM_TEX(node_8026, _basicNoise));
                float4 node_3263 = _Time;
                float2 node_1864 = ((i.uv0*float2(_basicNoise2TileX,_basicNoise2TileY))+(node_3263.g*float2(_basicNoise2PanX,_basicNoise2PanY)));
                float4 node_6879 = tex2D(_basicNoise2,TRANSFORM_TEX(node_1864, _basicNoise2));
                float4 node_2526 = _Time;
                float2 node_6831 = (i.uv0*float2(0.1,0.5));
                float4 node_8053 = tex2D(_basicNoise,TRANSFORM_TEX(node_6831, _basicNoise));
                float4 node_7286 = _Time;
                float2 node_2291 = ((i.uv0*float2(_basicNoise2TileX_copy,_basicNoise2TileY_copy))+(node_7286.g*float2(_basicNoise2PanX_copy,_basicNoise2PanY_copy)));
                float4 node_9118 = tex2D(_basicNoise2,TRANSFORM_TEX(node_2291, _basicNoise2));
                float4 node_2325 = _Time;
                float2 node_648 = ((i.uv1*float2(_largeDetailNoiseTileX,_largeDetailNoiseTileY))+(node_2325.g*float2(_largeDetailNoisePanX,_largeDetailNoisePanY)));
                float4 node_931 = tex2D(_largeDetailNoise,TRANSFORM_TEX(node_648, _largeDetailNoise));
                float2 node_1046 = (i.uv0*_edgeMaskDistortionTexTile);
                float4 node_3427 = tex2D(_geometryOffsetNoise,TRANSFORM_TEX(node_1046, _geometryOffsetNoise));
                float2 node_4546 = lerp(i.uv0,(i.uv0+node_3427.r),_edgeMaskDistortion);
                float node_5920 = node_4546.g;
                float3 node_3616 = (((1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_fresnelPow))*(node_5441*(lerp(lerp(_ColorTop.rgb,_centralTint.rgb,node_3466),lerp(_ColorBottom.rgb,_centralTint.rgb,node_3466),(((objPos.g-i.posWorld.g)+_meshPositionY)/_meshScaleDivisor))*(saturate(((pow(node_5289.rgb,_basicNoisePow)*_basicNoiseMult)*(pow(node_2773.rgb,_basicNoisePow_copy)*_basicNoiseMult_copy)))*((pow(node_6879.rgb,((saturate(sin(((node_2526.g/(1.0/_modulationFreq))+(pow(node_8053.r,0.4)*5.0))))*_modulateTextureAmount)+_basicNoise2Pow))*_basicNoise2Mult)*(pow(node_9118.rgb,_basicNoise2Pow_copy)*_basicNoise2Mult_copy))))))*(pow(node_931.r,_largeDetailNoisePow)*_largeDetailNoiseMult)*(saturate(pow(saturate((node_5920+_bottomOffset)),_bottomPower))*pow((1.0 - saturate((node_5920+(-1*(1.0 - _topOffset))))),_topPower)));
                float4 node_3703 = _Time;
                float2 node_9350 = ((i.uv1*float2(_largeDetailNoiseTileX_copy,_largeDetailNoiseTileY_copy))+(node_3703.g*float2(_largeDetailNoisePanX_copy,_largeDetailNoisePanY_copy)));
                float4 node_7435 = tex2D(_largeDetailNoise,TRANSFORM_TEX(node_9350, _largeDetailNoise));
                float node_2816 = (1.0 - _edgeMaskWidth_copy);
                float3 emissive = (pow(((lerp(node_3616,saturate((node_3616-(pow(node_7435.r,_largeDetailNoisePow_copy)*_largeDetailNoiseMult_copy))),(saturate(pow((i.uv0.g+node_2816),_edgeMaskPower_copy))+pow((1.0 - saturate((i.uv0.g+(-1*node_2816)))),_edgeMaskPower_copy)))*1.0)*i.vertexColor.r),_emissivePow)*_emissive);
                float3 finalColor = emissive * (1-_eclipsePower);
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
