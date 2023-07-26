// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:34937,y:32440,varname:node_3138,prsc:2|emission-7692-OUT,alpha-6249-OUT,voffset-8705-OUT;n:type:ShaderForge.SFN_Add,id:1778,x:31898,y:33193,varname:node_1778,prsc:2|A-9126-OUT,B-3487-OUT;n:type:ShaderForge.SFN_Slider,id:3487,x:31544,y:33421,ptovrint:False,ptlb:offsetMaskWidth,ptin:_offsetMaskWidth,varname:node_3487,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.244,max:1;n:type:ShaderForge.SFN_Power,id:5800,x:32180,y:33192,varname:node_5800,prsc:2|VAL-1778-OUT,EXP-5009-OUT;n:type:ShaderForge.SFN_Slider,id:5009,x:31859,y:33419,ptovrint:False,ptlb:offsetMaskPow,ptin:_offsetMaskPow,varname:node_5009,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3.2,max:15;n:type:ShaderForge.SFN_OneMinus,id:6923,x:31687,y:33602,varname:node_6923,prsc:2|IN-9126-OUT;n:type:ShaderForge.SFN_Add,id:9853,x:31927,y:33628,varname:node_9853,prsc:2|A-3487-OUT,B-6923-OUT;n:type:ShaderForge.SFN_Power,id:6476,x:32186,y:33631,varname:node_6476,prsc:2|VAL-9853-OUT,EXP-5009-OUT;n:type:ShaderForge.SFN_Clamp01,id:1723,x:32401,y:33192,varname:node_1723,prsc:2|IN-5800-OUT;n:type:ShaderForge.SFN_Clamp01,id:6984,x:32403,y:33631,varname:node_6984,prsc:2|IN-6476-OUT;n:type:ShaderForge.SFN_Multiply,id:3360,x:32649,y:33388,varname:node_3360,prsc:2|A-1723-OUT,B-6984-OUT,C-3382-OUT;n:type:ShaderForge.SFN_Color,id:7878,x:30732,y:32071,ptovrint:False,ptlb:wingCol1,ptin:_wingCol1,varname:node_7878,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:652,x:30657,y:32365,ptovrint:False,ptlb:wingPattern,ptin:_wingPattern,varname:node_652,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3638-OUT;n:type:ShaderForge.SFN_Lerp,id:556,x:31619,y:32174,varname:node_556,prsc:2|A-4407-OUT,B-8232-OUT,T-664-OUT;n:type:ShaderForge.SFN_Color,id:8805,x:30765,y:31723,ptovrint:False,ptlb:wingCol2,ptin:_wingCol2,varname:_wingCol2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:2580,x:30936,y:31613,ptovrint:False,ptlb:wingEmissive2,ptin:_wingEmissive2,varname:node_2580,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:7,max:10;n:type:ShaderForge.SFN_Power,id:664,x:31056,y:32472,varname:node_664,prsc:2|VAL-652-R,EXP-2398-OUT;n:type:ShaderForge.SFN_Slider,id:2398,x:30599,y:32595,ptovrint:False,ptlb:wingPatternPow,ptin:_wingPatternPow,varname:node_2398,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_Multiply,id:4407,x:31186,y:31719,varname:node_4407,prsc:2|A-2580-OUT,B-8805-RGB;n:type:ShaderForge.SFN_Multiply,id:8232,x:31144,y:32067,varname:node_8232,prsc:2|A-980-OUT,B-7878-RGB;n:type:ShaderForge.SFN_Slider,id:980,x:30575,y:31947,ptovrint:False,ptlb:wingEmissive1,ptin:_wingEmissive1,varname:node_980,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3,max:10;n:type:ShaderForge.SFN_TexCoord,id:2729,x:29910,y:33296,varname:node_2729,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:4051,x:31126,y:33409,varname:node_4051,prsc:2|A-6055-OUT,B-948-OUT;n:type:ShaderForge.SFN_Time,id:254,x:30602,y:33489,varname:node_254,prsc:2;n:type:ShaderForge.SFN_Frac,id:5496,x:31311,y:33409,varname:node_5496,prsc:2|IN-4051-OUT;n:type:ShaderForge.SFN_Multiply,id:1456,x:32989,y:33387,varname:node_1456,prsc:2|A-3360-OUT,B-2174-OUT;n:type:ShaderForge.SFN_Vector3,id:2174,x:32713,y:33609,varname:node_2174,prsc:2,v1:0,v2:1,v3:0;n:type:ShaderForge.SFN_Multiply,id:8705,x:33482,y:33384,varname:node_8705,prsc:2|A-1456-OUT,B-4335-OUT;n:type:ShaderForge.SFN_Slider,id:4335,x:33255,y:33646,ptovrint:False,ptlb:offset,ptin:_offset,varname:node_4335,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Multiply,id:7882,x:30857,y:33511,varname:node_7882,prsc:2|A-254-T,B-1373-OUT;n:type:ShaderForge.SFN_Slider,id:1373,x:30212,y:33721,ptovrint:False,ptlb:offsetFreq,ptin:_offsetFreq,varname:node_1373,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.75,max:2;n:type:ShaderForge.SFN_Slider,id:2368,x:32501,y:33027,ptovrint:False,ptlb:opacity,ptin:_opacity,varname:node_2368,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4012284,max:1;n:type:ShaderForge.SFN_Multiply,id:9015,x:32739,y:32819,varname:node_9015,prsc:2|A-2368-OUT,B-7795-OUT;n:type:ShaderForge.SFN_TexCoord,id:814,x:31895,y:32809,varname:node_814,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Power,id:7753,x:32311,y:32819,varname:node_7753,prsc:2|VAL-2388-OUT,EXP-196-OUT;n:type:ShaderForge.SFN_Slider,id:196,x:32123,y:33023,ptovrint:False,ptlb:edgeMskPow,ptin:_edgeMskPow,varname:node_196,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:12,max:15;n:type:ShaderForge.SFN_Add,id:2388,x:32123,y:32819,varname:node_2388,prsc:2|A-814-V,B-5780-OUT;n:type:ShaderForge.SFN_Vector1,id:5780,x:31846,y:33012,varname:node_5780,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Clamp01,id:7795,x:32501,y:32819,varname:node_7795,prsc:2|IN-7753-OUT;n:type:ShaderForge.SFN_Tex2d,id:4187,x:29889,y:33488,ptovrint:False,ptlb:noise,ptin:_noise,varname:node_4187,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5285-OUT;n:type:ShaderForge.SFN_Add,id:832,x:30115,y:33444,varname:node_832,prsc:2|A-4187-R,B-2729-V;n:type:ShaderForge.SFN_Lerp,id:6055,x:30424,y:33428,varname:node_6055,prsc:2|A-2729-UVOUT,B-832-OUT,T-6532-OUT;n:type:ShaderForge.SFN_Slider,id:6532,x:29810,y:33707,ptovrint:False,ptlb:offsetNoiseDist,ptin:_offsetNoiseDist,varname:node_6532,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6,max:1;n:type:ShaderForge.SFN_Time,id:2918,x:29204,y:33457,varname:node_2918,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6094,x:29493,y:33533,varname:node_6094,prsc:2|A-2918-T,B-9723-OUT;n:type:ShaderForge.SFN_Slider,id:5753,x:28850,y:33748,ptovrint:False,ptlb:offsetNoisePanY,ptin:_offsetNoisePanY,varname:node_5753,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:-0.2,max:1;n:type:ShaderForge.SFN_Vector1,id:7992,x:28939,y:33604,varname:node_7992,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:9723,x:29230,y:33652,varname:node_9723,prsc:2|A-7992-OUT,B-5753-OUT;n:type:ShaderForge.SFN_TexCoord,id:183,x:29271,y:33285,varname:node_183,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:5285,x:29690,y:33488,varname:node_5285,prsc:2|A-183-UVOUT,B-6094-OUT;n:type:ShaderForge.SFN_Multiply,id:7286,x:33233,y:32818,varname:node_7286,prsc:2|A-9015-OUT,B-6317-A,C-3154-OUT;n:type:ShaderForge.SFN_VertexColor,id:6317,x:32946,y:32894,varname:node_6317,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:9115,x:29935,y:32269,varname:node_9115,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:3638,x:30276,y:32353,varname:node_3638,prsc:2|A-9115-UVOUT,B-648-OUT;n:type:ShaderForge.SFN_Time,id:678,x:29700,y:32636,varname:node_678,prsc:2;n:type:ShaderForge.SFN_Multiply,id:648,x:29977,y:32524,varname:node_648,prsc:2|A-2291-OUT,B-678-T;n:type:ShaderForge.SFN_Slider,id:2441,x:29422,y:32516,ptovrint:False,ptlb:wingPatternPanY,ptin:_wingPatternPanY,varname:node_2441,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:0,max:2;n:type:ShaderForge.SFN_Vector1,id:5034,x:29579,y:32438,varname:node_5034,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:2291,x:29756,y:32465,varname:node_2291,prsc:2|A-5034-OUT,B-2441-OUT;n:type:ShaderForge.SFN_Add,id:2014,x:31966,y:33789,varname:node_2014,prsc:2|A-3298-OUT,B-5486-OUT;n:type:ShaderForge.SFN_Slider,id:5486,x:31612,y:34017,ptovrint:False,ptlb:innerRingMaskWidth,ptin:_innerRingMaskWidth,varname:_edgeWidth_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7874881,max:1;n:type:ShaderForge.SFN_Power,id:6040,x:32248,y:33788,varname:node_6040,prsc:2|VAL-2014-OUT,EXP-7130-OUT;n:type:ShaderForge.SFN_Slider,id:7130,x:31927,y:34015,ptovrint:False,ptlb:innerRingMaskPow,ptin:_innerRingMaskPow,varname:_edgePow_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.31,max:15;n:type:ShaderForge.SFN_OneMinus,id:1257,x:31709,y:34243,varname:node_1257,prsc:2|IN-3298-OUT;n:type:ShaderForge.SFN_Add,id:5323,x:31995,y:34224,varname:node_5323,prsc:2|A-5486-OUT,B-1257-OUT;n:type:ShaderForge.SFN_Power,id:8056,x:32254,y:34227,varname:node_8056,prsc:2|VAL-5323-OUT,EXP-7130-OUT;n:type:ShaderForge.SFN_Clamp01,id:5607,x:32469,y:33788,varname:node_5607,prsc:2|IN-6040-OUT;n:type:ShaderForge.SFN_Clamp01,id:6879,x:32471,y:34227,varname:node_6879,prsc:2|IN-8056-OUT;n:type:ShaderForge.SFN_Multiply,id:9118,x:32685,y:33984,varname:node_9118,prsc:2|A-5607-OUT,B-6879-OUT;n:type:ShaderForge.SFN_ComponentMask,id:9126,x:31500,y:33570,varname:node_9126,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-5496-OUT;n:type:ShaderForge.SFN_TexCoord,id:6014,x:30825,y:33906,varname:node_6014,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:3714,x:31040,y:33906,varname:node_3714,prsc:2|A-6014-UVOUT,B-4866-OUT;n:type:ShaderForge.SFN_ComponentMask,id:3298,x:31336,y:33910,varname:node_3298,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-3714-OUT;n:type:ShaderForge.SFN_Vector1,id:4866,x:30856,y:34107,varname:node_4866,prsc:2,v1:-0.7;n:type:ShaderForge.SFN_OneMinus,id:3382,x:32927,y:33919,varname:node_3382,prsc:2|IN-9118-OUT;n:type:ShaderForge.SFN_Add,id:1637,x:33481,y:32446,varname:node_1637,prsc:2|A-556-OUT,B-5932-OUT;n:type:ShaderForge.SFN_Multiply,id:5932,x:33188,y:32456,varname:node_5932,prsc:2|A-2898-OUT,B-5094-OUT,C-3628-RGB;n:type:ShaderForge.SFN_Slider,id:2898,x:32645,y:32449,ptovrint:False,ptlb:innerRingEmissive,ptin:_innerRingEmissive,varname:node_2898,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:14,max:222;n:type:ShaderForge.SFN_Color,id:3628,x:32752,y:32204,ptovrint:False,ptlb:innerRingColor,ptin:_innerRingColor,varname:node_3628,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Power,id:5094,x:32971,y:32569,varname:node_5094,prsc:2|VAL-9118-OUT,EXP-5380-OUT;n:type:ShaderForge.SFN_Slider,id:5380,x:32645,y:32630,ptovrint:False,ptlb:innerRingColorMaskPow,ptin:_innerRingColorMaskPow,varname:node_5380,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:55,max:55;n:type:ShaderForge.SFN_TexCoord,id:8143,x:30867,y:33692,varname:node_8143,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Add,id:948,x:31028,y:33536,varname:node_948,prsc:2|A-7882-OUT,B-8143-U;n:type:ShaderForge.SFN_TexCoord,id:567,x:33286,y:33018,varname:node_567,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Power,id:2777,x:33475,y:33018,varname:node_2777,prsc:2|VAL-567-V,EXP-291-OUT;n:type:ShaderForge.SFN_Vector1,id:291,x:33274,y:33186,varname:node_291,prsc:2,v1:3;n:type:ShaderForge.SFN_OneMinus,id:6360,x:33638,y:33018,varname:node_6360,prsc:2|IN-2777-OUT;n:type:ShaderForge.SFN_Multiply,id:5261,x:33682,y:32825,varname:node_5261,prsc:2|A-7286-OUT,B-6360-OUT;n:type:ShaderForge.SFN_Power,id:2444,x:31599,y:32530,varname:node_2444,prsc:2|VAL-652-R,EXP-8848-OUT;n:type:ShaderForge.SFN_Slider,id:8848,x:31220,y:32599,ptovrint:False,ptlb:opacityPow,ptin:_opacityPow,varname:node_8848,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:12;n:type:ShaderForge.SFN_Lerp,id:3154,x:32217,y:32471,varname:node_3154,prsc:2|A-2444-OUT,B-652-R,T-3473-OUT;n:type:ShaderForge.SFN_Power,id:3473,x:32024,y:32553,varname:node_3473,prsc:2|VAL-5094-OUT,EXP-4982-OUT;n:type:ShaderForge.SFN_Slider,id:4982,x:31675,y:32744,ptovrint:False,ptlb:lowerEdgeOpacityMaskPow,ptin:_lowerEdgeOpacityMaskPow,varname:node_4982,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:25;n:type:ShaderForge.SFN_Power,id:4465,x:33845,y:32480,varname:node_4465,prsc:2|VAL-1637-OUT,EXP-6218-OUT;n:type:ShaderForge.SFN_Multiply,id:7692,x:34131,y:32507,varname:node_7692,prsc:2|A-4465-OUT,B-9920-OUT;n:type:ShaderForge.SFN_Slider,id:6218,x:33501,y:32662,ptovrint:False,ptlb:finalEmPower,ptin:_finalEmPower,varname:node_6218,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Slider,id:9920,x:33845,y:32657,ptovrint:False,ptlb:finalEm,ptin:_finalEm,varname:_finalEmPower_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:15;n:type:ShaderForge.SFN_Slider,id:3641,x:33802,y:33056,ptovrint:False,ptlb:depthFadeDistance,ptin:_depthFadeDistance,varname:node_3641,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_DepthBlend,id:9013,x:34118,y:32960,varname:node_9013,prsc:2|DIST-3641-OUT;n:type:ShaderForge.SFN_Multiply,id:9062,x:34271,y:32813,varname:node_9062,prsc:2|A-5261-OUT,B-9013-OUT;n:type:ShaderForge.SFN_Multiply,id:6249,x:34607,y:32807,varname:node_6249,prsc:2|A-9062-OUT,B-2351-OUT;n:type:ShaderForge.SFN_ViewPosition,id:7171,x:34343,y:33142,varname:node_7171,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:5915,x:34343,y:33275,varname:node_5915,prsc:2;n:type:ShaderForge.SFN_Distance,id:5028,x:34567,y:33207,varname:node_5028,prsc:2|A-7171-XYZ,B-5915-XYZ;n:type:ShaderForge.SFN_Divide,id:3373,x:34792,y:33207,varname:node_3373,prsc:2|A-5028-OUT,B-4091-OUT;n:type:ShaderForge.SFN_Slider,id:4091,x:34450,y:33412,ptovrint:False,ptlb:distanceFadeDivisor,ptin:_distanceFadeDivisor,varname:node_4091,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:10000;n:type:ShaderForge.SFN_Power,id:8221,x:35057,y:33208,varname:node_8221,prsc:2|VAL-3373-OUT,EXP-2673-OUT;n:type:ShaderForge.SFN_Slider,id:2673,x:34803,y:33391,ptovrint:False,ptlb:distanceFadePow,ptin:_distanceFadePow,varname:node_2673,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:25;n:type:ShaderForge.SFN_Clamp01,id:2351,x:35263,y:33220,varname:node_2351,prsc:2|IN-8221-OUT;proporder:3487-5009-1373-4335-4187-6532-5753-652-2441-2398-7878-8805-980-2580-196-5486-7130-3628-2898-5380-4982-8848-2368-6218-9920-3641-4091-2673;pass:END;sub:END;*/

Shader "Colony_FX/S_JellyFlies" {
    Properties {
        _offsetMaskWidth ("offsetMaskWidth", Range(0, 1)) = 0.244
        _offsetMaskPow ("offsetMaskPow", Range(0, 15)) = 3.2
        _offsetFreq ("offsetFreq", Range(0, 2)) = 0.75
        _offset ("offset", Range(0, 10)) = 0
        _noise ("noise", 2D) = "white" {}
        _offsetNoiseDist ("offsetNoiseDist", Range(0, 1)) = 0.6
        _offsetNoisePanY ("offsetNoisePanY", Range(-1, 1)) = -0.2
        _wingPattern ("wingPattern", 2D) = "white" {}
        _wingPatternPanY ("wingPatternPanY", Range(-2, 2)) = 0
        _wingPatternPow ("wingPatternPow", Range(0, 5)) = 0
        _wingCol1 ("wingCol1", Color) = (0.5,0.5,0.5,1)
        _wingCol2 ("wingCol2", Color) = (0.5,0.5,0.5,1)
        _wingEmissive1 ("wingEmissive1", Range(0, 10)) = 3
        _wingEmissive2 ("wingEmissive2", Range(0, 10)) = 7
        _edgeMskPow ("edgeMskPow", Range(0, 15)) = 12
        _innerRingMaskWidth ("innerRingMaskWidth", Range(0, 1)) = 0.7874881
        _innerRingMaskPow ("innerRingMaskPow", Range(0, 15)) = 0.31
        _innerRingColor ("innerRingColor", Color) = (0.5,0.5,0.5,1)
        _innerRingEmissive ("innerRingEmissive", Range(0, 222)) = 14
        _innerRingColorMaskPow ("innerRingColorMaskPow", Range(0, 55)) = 55
        _lowerEdgeOpacityMaskPow ("lowerEdgeOpacityMaskPow", Range(0, 25)) = 0.1
        _opacityPow ("opacityPow", Range(0, 12)) = 0
        _opacity ("opacity", Range(0, 1)) = 0.4012284
        _finalEmPower ("finalEmPower", Range(0, 15)) = 1
        _finalEm ("finalEm", Range(0, 15)) = 1
        _depthFadeDistance ("depthFadeDistance", Range(0, 1)) = 0
        _distanceFadeDivisor ("distanceFadeDivisor", Range(0, 10000)) = 1
        _distanceFadePow ("distanceFadePow", Range(0, 25)) = 1
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
            uniform float _offsetMaskWidth;
            uniform float _offsetMaskPow;
            uniform float4 _wingCol1;
            uniform sampler2D _wingPattern; uniform float4 _wingPattern_ST;
            uniform float4 _wingCol2;
            uniform float _wingEmissive2;
            uniform float _wingPatternPow;
            uniform float _wingEmissive1;
            uniform float _offset;
            uniform float _offsetFreq;
            uniform float _opacity;
            uniform float _edgeMskPow;
            uniform sampler2D _noise; uniform float4 _noise_ST;
            uniform float _offsetNoiseDist;
            uniform float _offsetNoisePanY;
            uniform float _wingPatternPanY;
            uniform float _innerRingMaskWidth;
            uniform float _innerRingMaskPow;
            uniform float _innerRingEmissive;
            uniform float4 _innerRingColor;
            uniform float _innerRingColorMaskPow;
            uniform float _opacityPow;
            uniform float _lowerEdgeOpacityMaskPow;
            uniform float _finalEmPower;
            uniform float _finalEm;
            uniform float _depthFadeDistance;
            uniform float _distanceFadeDivisor;
            uniform float _distanceFadePow;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                float4 node_2918 = _Time;
                float2 node_5285 = (o.uv0+(node_2918.g*float2(0.0,_offsetNoisePanY)));
                float4 _noise_var = tex2Dlod(_noise,float4(TRANSFORM_TEX(node_5285, _noise),0.0,0));
                float node_832 = (_noise_var.r+o.uv0.g);
                float4 node_254 = _Time;
                float node_9126 = frac((lerp(o.uv0,float2(node_832,node_832),_offsetNoiseDist)+((node_254.g*_offsetFreq)+o.uv1.r))).g;
                float node_3298 = (o.uv0+(-0.7)).g;
                float node_9118 = (saturate(pow((node_3298+_innerRingMaskWidth),_innerRingMaskPow))*saturate(pow((_innerRingMaskWidth+(1.0 - node_3298)),_innerRingMaskPow)));
                v.vertex.xyz += (((saturate(pow((node_9126+_offsetMaskWidth),_offsetMaskPow))*saturate(pow((_offsetMaskWidth+(1.0 - node_9126)),_offsetMaskPow))*(1.0 - node_9118))*float3(0,1,0))*_offset);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
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
////// Lighting:
////// Emissive:
                float4 node_678 = _Time;
                float2 node_3638 = (i.uv0+(float2(0.0,_wingPatternPanY)*node_678.g));
                float4 _wingPattern_var = tex2D(_wingPattern,TRANSFORM_TEX(node_3638, _wingPattern));
                float node_3298 = (i.uv0+(-0.7)).g;
                float node_9118 = (saturate(pow((node_3298+_innerRingMaskWidth),_innerRingMaskPow))*saturate(pow((_innerRingMaskWidth+(1.0 - node_3298)),_innerRingMaskPow)));
                float node_5094 = pow(node_9118,_innerRingColorMaskPow);
                float3 emissive = (pow((lerp((_wingEmissive2*_wingCol2.rgb),(_wingEmissive1*_wingCol1.rgb),pow(_wingPattern_var.r,_wingPatternPow))+(_innerRingEmissive*node_5094*_innerRingColor.rgb)),_finalEmPower)*_finalEm);
                float3 finalColor = emissive;
                return fixed4(finalColor,(((((_opacity*saturate(pow((i.uv0.g+0.2),_edgeMskPow)))*i.vertexColor.a*lerp(pow(_wingPattern_var.r,_opacityPow),_wingPattern_var.r,pow(node_5094,_lowerEdgeOpacityMaskPow)))*(1.0 - pow(i.uv0.g,3.0)))*saturate((sceneZ-partZ)/_depthFadeDistance))*saturate(pow((distance(_WorldSpaceCameraPos,i.posWorld.rgb)/_distanceFadeDivisor),_distanceFadePow))));
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
            uniform float _offsetMaskWidth;
            uniform float _offsetMaskPow;
            uniform float _offset;
            uniform float _offsetFreq;
            uniform sampler2D _noise; uniform float4 _noise_ST;
            uniform float _offsetNoiseDist;
            uniform float _offsetNoisePanY;
            uniform float _innerRingMaskWidth;
            uniform float _innerRingMaskPow;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                float4 node_2918 = _Time;
                float2 node_5285 = (o.uv0+(node_2918.g*float2(0.0,_offsetNoisePanY)));
                float4 _noise_var = tex2Dlod(_noise,float4(TRANSFORM_TEX(node_5285, _noise),0.0,0));
                float node_832 = (_noise_var.r+o.uv0.g);
                float4 node_254 = _Time;
                float node_9126 = frac((lerp(o.uv0,float2(node_832,node_832),_offsetNoiseDist)+((node_254.g*_offsetFreq)+o.uv1.r))).g;
                float node_3298 = (o.uv0+(-0.7)).g;
                float node_9118 = (saturate(pow((node_3298+_innerRingMaskWidth),_innerRingMaskPow))*saturate(pow((_innerRingMaskWidth+(1.0 - node_3298)),_innerRingMaskPow)));
                v.vertex.xyz += (((saturate(pow((node_9126+_offsetMaskWidth),_offsetMaskPow))*saturate(pow((_offsetMaskWidth+(1.0 - node_9126)),_offsetMaskPow))*(1.0 - node_9118))*float3(0,1,0))*_offset);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
