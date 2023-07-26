// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:3,spmd:1,trmd:0,grmd:1,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.09543246,fgcg:0.6686392,fgcb:0.7158822,fgca:1,fgde:0.004160235,fgrn:0,fgrf:600,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:36292,y:33033,varname:node_2865,prsc:2|diff-2079-RGB,spec-556-OUT,gloss-2580-OUT,emission-2981-OUT,clip-201-OUT,disp-6638-OUT,tess-9644-OUT;n:type:ShaderForge.SFN_Tex2d,id:4229,x:30679,y:32929,ptovrint:False,ptlb:EmissiveTex,ptin:_EmissiveTex,varname:node_4229,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2968-OUT;n:type:ShaderForge.SFN_Slider,id:556,x:35357,y:32712,ptovrint:False,ptlb:metallic,ptin:_metallic,varname:node_556,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:2580,x:35357,y:32825,ptovrint:False,ptlb:roughness,ptin:_roughness,varname:node_2580,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Color,id:2079,x:35454,y:32528,ptovrint:False,ptlb:BaseCol,ptin:_BaseCol,varname:node_2079,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:5950,x:34975,y:32873,varname:node_5950,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:9227,x:31949,y:32950,varname:node_9227,prsc:2|A-4229-RGB,B-5009-OUT;n:type:ShaderForge.SFN_Slider,id:5009,x:31599,y:33031,ptovrint:False,ptlb:emissiveIntensity,ptin:_emissiveIntensity,varname:node_5009,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:100;n:type:ShaderForge.SFN_Fresnel,id:5705,x:31313,y:33392,varname:node_5705,prsc:2|EXP-3173-OUT;n:type:ShaderForge.SFN_Slider,id:3173,x:30942,y:33413,ptovrint:False,ptlb:emissiveFresnelExp,ptin:_emissiveFresnelExp,varname:node_3173,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:20;n:type:ShaderForge.SFN_Add,id:2596,x:32317,y:32948,varname:node_2596,prsc:2|A-9227-OUT,B-623-OUT;n:type:ShaderForge.SFN_Multiply,id:664,x:31506,y:33358,varname:node_664,prsc:2|A-4229-RGB,B-5705-OUT;n:type:ShaderForge.SFN_Multiply,id:623,x:31705,y:33358,varname:node_623,prsc:2|A-664-OUT,B-1042-OUT;n:type:ShaderForge.SFN_Slider,id:1042,x:31521,y:33556,ptovrint:False,ptlb:emissiveFresnelIntensity,ptin:_emissiveFresnelIntensity,varname:node_1042,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:100;n:type:ShaderForge.SFN_Add,id:3154,x:32992,y:32954,varname:node_3154,prsc:2|A-2596-OUT,B-3473-RGB;n:type:ShaderForge.SFN_Tex2d,id:3473,x:32585,y:33639,ptovrint:False,ptlb:subColorTex,ptin:_subColorTex,varname:node_3473,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-4465-UVOUT;n:type:ShaderForge.SFN_Parallax,id:4465,x:32408,y:33639,varname:node_4465,prsc:2|UVIN-2202-OUT,HEI-4229-G,DEP-7107-OUT,REF-9062-OUT;n:type:ShaderForge.SFN_Slider,id:9062,x:32076,y:33807,ptovrint:False,ptlb:ParallaxRatio,ptin:_ParallaxRatio,varname:node_9062,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.76,max:1;n:type:ShaderForge.SFN_Slider,id:7107,x:32076,y:33703,ptovrint:False,ptlb:Depth,ptin:_Depth,varname:node_7107,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_TexCoord,id:5845,x:29396,y:33612,varname:node_5845,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:2202,x:30706,y:33622,varname:node_2202,prsc:2|A-5845-UVOUT,B-4079-OUT;n:type:ShaderForge.SFN_Panner,id:5618,x:29670,y:33745,varname:node_5618,prsc:2,spu:0.1,spv:0.1|UVIN-5845-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:5763,x:29855,y:33745,ptovrint:False,ptlb:uvDistTex,ptin:_uvDistTex,varname:node_5763,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-5618-UVOUT;n:type:ShaderForge.SFN_Multiply,id:4079,x:30264,y:33770,varname:node_4079,prsc:2|A-5763-R,B-6517-OUT;n:type:ShaderForge.SFN_Slider,id:6517,x:29887,y:33984,ptovrint:False,ptlb:uvDist,ptin:_uvDist,varname:node_6517,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_TexCoord,id:652,x:30240,y:32929,varname:node_652,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:2968,x:30452,y:32929,varname:node_2968,prsc:2|A-652-UVOUT,B-9079-OUT;n:type:ShaderForge.SFN_Slider,id:8805,x:29797,y:33115,ptovrint:False,ptlb:uvDistMaint,ptin:_uvDistMaint,varname:node_8805,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:9079,x:30240,y:33117,varname:node_9079,prsc:2|A-8805-OUT,B-5763-R;n:type:ShaderForge.SFN_Tex2d,id:1778,x:32249,y:34166,ptovrint:False,ptlb:opacityMask,ptin:_opacityMask,varname:node_1778,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7962-OUT;n:type:ShaderForge.SFN_Vector1,id:3487,x:34017,y:33441,varname:node_3487,prsc:2,v1:1;n:type:ShaderForge.SFN_Subtract,id:5800,x:34096,y:33521,varname:node_5800,prsc:2|A-3487-OUT,B-9853-OUT;n:type:ShaderForge.SFN_Slider,id:6923,x:31642,y:34525,ptovrint:False,ptlb:opacityMaskExp,ptin:_opacityMaskExp,varname:node_6923,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:1,max:1;n:type:ShaderForge.SFN_Clamp01,id:9853,x:32663,y:34209,varname:node_9853,prsc:2|IN-563-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:1166,x:34355,y:33499,ptovrint:False,ptlb:useOpacity,ptin:_useOpacity,varname:node_1166,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-7734-OUT,B-5800-OUT;n:type:ShaderForge.SFN_Vector1,id:7734,x:34300,y:33385,varname:node_7734,prsc:2,v1:1;n:type:ShaderForge.SFN_Subtract,id:563,x:32453,y:34209,varname:node_563,prsc:2|A-1778-R,B-3070-OUT;n:type:ShaderForge.SFN_Slider,id:9553,x:31968,y:34565,ptovrint:False,ptlb:diff,ptin:_diff,varname:node_9553,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:1;n:type:ShaderForge.SFN_Ceil,id:1169,x:32923,y:34406,varname:node_1169,prsc:2|IN-9478-OUT;n:type:ShaderForge.SFN_Subtract,id:636,x:33257,y:34400,varname:node_636,prsc:2|A-3096-OUT,B-1169-OUT;n:type:ShaderForge.SFN_Clamp01,id:9478,x:32728,y:34406,varname:node_9478,prsc:2|IN-4145-OUT;n:type:ShaderForge.SFN_Lerp,id:6126,x:33643,y:32954,varname:node_6126,prsc:2|A-3154-OUT,B-240-OUT,T-4103-OUT;n:type:ShaderForge.SFN_Ceil,id:3096,x:33009,y:34216,varname:node_3096,prsc:2|IN-9853-OUT;n:type:ShaderForge.SFN_Subtract,id:4145,x:32544,y:34406,varname:node_4145,prsc:2|A-1778-R,B-821-OUT;n:type:ShaderForge.SFN_Multiply,id:240,x:33187,y:33047,varname:node_240,prsc:2|A-3154-OUT,B-5302-OUT;n:type:ShaderForge.SFN_Vector1,id:9644,x:36065,y:33482,varname:node_9644,prsc:2,v1:15;n:type:ShaderForge.SFN_Multiply,id:1759,x:31972,y:35717,varname:node_1759,prsc:2|A-9015-OUT,B-4407-R,C-4425-OUT;n:type:ShaderForge.SFN_NormalVector,id:4425,x:31760,y:35901,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:6265,x:31157,y:35364,ptovrint:False,ptlb:displace,ptin:_displace,varname:node_6265,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2,max:15;n:type:ShaderForge.SFN_Slider,id:5302,x:32944,y:33268,ptovrint:False,ptlb:edgeHighlight,ptin:_edgeHighlight,varname:node_5302,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:15,max:25;n:type:ShaderForge.SFN_Vector1,id:9317,x:32793,y:35592,varname:node_9317,prsc:2,v1:0;n:type:ShaderForge.SFN_SwitchProperty,id:6638,x:33139,y:35702,ptovrint:False,ptlb:useDisplacement,ptin:_useDisplacement,varname:node_6638,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-9317-OUT,B-9710-OUT;n:type:ShaderForge.SFN_Tex2d,id:4407,x:31544,y:35736,ptovrint:False,ptlb:displacementNoise,ptin:_displacementNoise,varname:node_4407,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6043-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:8731,x:31195,y:35736,varname:node_8731,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:6043,x:31370,y:35736,varname:node_6043,prsc:2,spu:0,spv:0.05|UVIN-8731-UVOUT;n:type:ShaderForge.SFN_Color,id:1023,x:32028,y:33245,ptovrint:False,ptlb:col_copy,ptin:_col_copy,varname:_col_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Color,id:6660,x:33440,y:33107,ptovrint:False,ptlb:col_copy_copy,ptin:_col_copy_copy,varname:_col_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Fresnel,id:5634,x:32660,y:32417,varname:node_5634,prsc:2|EXP-8232-OUT;n:type:ShaderForge.SFN_Vector1,id:8232,x:32428,y:32504,varname:node_8232,prsc:2,v1:0.1;n:type:ShaderForge.SFN_OneMinus,id:980,x:32852,y:32405,varname:node_980,prsc:2|IN-5634-OUT;n:type:ShaderForge.SFN_Power,id:9013,x:33066,y:32405,varname:node_9013,prsc:2|VAL-980-OUT,EXP-8234-OUT;n:type:ShaderForge.SFN_Multiply,id:7171,x:33295,y:32391,varname:node_7171,prsc:2|A-9013-OUT,B-871-OUT,C-9877-R;n:type:ShaderForge.SFN_ValueProperty,id:8234,x:32884,y:32576,ptovrint:False,ptlb:e,ptin:_e,varname:node_8234,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:9160,x:33038,y:32616,ptovrint:False,ptlb:m,ptin:_m,varname:node_9160,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Lerp,id:7792,x:33990,y:32955,varname:node_7792,prsc:2|A-6126-OUT,B-6660-RGB,T-5817-OUT;n:type:ShaderForge.SFN_Clamp01,id:5817,x:33513,y:32378,varname:node_5817,prsc:2|IN-7171-OUT;n:type:ShaderForge.SFN_TexCoord,id:1236,x:31672,y:34339,varname:node_1236,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_SwitchProperty,id:3070,x:31968,y:34407,ptovrint:False,ptlb:dynamicDissolve,ptin:_dynamicDissolve,varname:node_3070,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-6923-OUT,B-1236-U;n:type:ShaderForge.SFN_TexCoord,id:9014,x:33018,y:32690,varname:node_9014,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Multiply,id:871,x:33227,y:32658,varname:node_871,prsc:2|A-9160-OUT,B-9014-V;n:type:ShaderForge.SFN_Desaturate,id:5131,x:34508,y:32964,varname:node_5131,prsc:2|COL-7792-OUT,DES-7453-Z;n:type:ShaderForge.SFN_TexCoord,id:7453,x:34250,y:32998,varname:node_7453,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_SwitchProperty,id:2981,x:35514,y:32961,ptovrint:False,ptlb:use_emissive,ptin:_use_emissive,varname:node_2981,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5950-OUT,B-5220-OUT;n:type:ShaderForge.SFN_Multiply,id:5220,x:35139,y:32983,varname:node_5220,prsc:2|A-5131-OUT,B-6252-OUT,C-7274-OUT;n:type:ShaderForge.SFN_Color,id:9852,x:34600,y:33256,ptovrint:False,ptlb:colorMod,ptin:_colorMod,varname:node_9852,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Lerp,id:6252,x:34786,y:33085,varname:node_6252,prsc:2|A-3348-OUT,B-9852-RGB,T-1793-OUT;n:type:ShaderForge.SFN_Vector1,id:3348,x:34600,y:33105,varname:node_3348,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:9015,x:31491,y:35453,varname:node_9015,prsc:2|A-6265-OUT,B-814-Z,C-567-OUT;n:type:ShaderForge.SFN_TexCoord,id:814,x:31236,y:35453,varname:node_814,prsc:2,uv:0,uaff:True;n:type:ShaderForge.SFN_Vector3,id:567,x:31419,y:35594,varname:node_567,prsc:2,v1:2,v2:1,v3:2;n:type:ShaderForge.SFN_Tex2d,id:9877,x:32993,y:32208,ptovrint:False,ptlb:fresnelNoise,ptin:_fresnelNoise,varname:node_9877,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-759-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:87,x:32606,y:32208,varname:node_87,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:759,x:32794,y:32208,varname:node_759,prsc:2,spu:0.01,spv:0.03|UVIN-87-UVOUT;n:type:ShaderForge.SFN_Add,id:821,x:32336,y:34428,varname:node_821,prsc:2|A-3070-OUT,B-9553-OUT;n:type:ShaderForge.SFN_TexCoord,id:3308,x:31662,y:34036,varname:node_3308,prsc:2,uv:0,uaff:True;n:type:ShaderForge.SFN_Add,id:1167,x:31863,y:34119,varname:node_1167,prsc:2|A-3308-V,B-3308-W;n:type:ShaderForge.SFN_Append,id:7962,x:32030,y:34076,varname:node_7962,prsc:2|A-3308-U,B-1167-OUT;n:type:ShaderForge.SFN_Multiply,id:1793,x:34470,y:33148,varname:node_1793,prsc:2|A-7453-Z,B-1106-OUT;n:type:ShaderForge.SFN_Vector1,id:1106,x:34300,y:33182,varname:node_1106,prsc:2,v1:2;n:type:ShaderForge.SFN_ValueProperty,id:1917,x:34643,y:33750,ptovrint:False,ptlb:verticalMaskIntensity,ptin:_verticalMaskIntensity,varname:node_1917,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_TexCoord,id:1181,x:34643,y:33558,varname:node_1181,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:3058,x:34861,y:33642,varname:node_3058,prsc:2|A-1181-V,B-1917-OUT;n:type:ShaderForge.SFN_Clamp01,id:7274,x:35041,y:33642,varname:node_7274,prsc:2|IN-3058-OUT;n:type:ShaderForge.SFN_Add,id:9710,x:32667,y:35707,varname:node_9710,prsc:2|A-1759-OUT,B-8445-OUT;n:type:ShaderForge.SFN_VertexColor,id:6404,x:32640,y:36264,varname:node_6404,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:2636,x:32335,y:36096,ptovrint:False,ptlb:lowScaleDisplacementNoise,ptin:_lowScaleDisplacementNoise,varname:node_2636,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7747-UVOUT;n:type:ShaderForge.SFN_Multiply,id:133,x:32956,y:36109,varname:node_133,prsc:2|A-8935-OUT,B-6404-A;n:type:ShaderForge.SFN_TexCoord,id:9447,x:31893,y:36081,varname:node_9447,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:7747,x:32166,y:36082,varname:node_7747,prsc:2,spu:0.05,spv:0.05|UVIN-9447-UVOUT;n:type:ShaderForge.SFN_SwitchProperty,id:1473,x:32166,y:34993,ptovrint:False,ptlb:useVerticalGradientDissolve,ptin:_useVerticalGradientDissolve,varname:node_1473,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-6179-OUT,B-1236-U;n:type:ShaderForge.SFN_Vector1,id:6179,x:31944,y:34969,varname:node_6179,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:201,x:33739,y:34237,varname:node_201,prsc:2|A-3096-OUT,B-3483-R;n:type:ShaderForge.SFN_Tex2d,id:3483,x:33067,y:34803,ptovrint:False,ptlb:growthMask,ptin:_growthMask,varname:node_3483,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3690-OUT;n:type:ShaderForge.SFN_TexCoord,id:4392,x:32320,y:34841,varname:node_4392,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:5098,x:32571,y:34982,varname:node_5098,prsc:2|A-4392-V,B-1473-OUT;n:type:ShaderForge.SFN_Append,id:3690,x:32807,y:34806,varname:node_3690,prsc:2|A-1902-OUT,B-5098-OUT;n:type:ShaderForge.SFN_RemapRange,id:408,x:32543,y:36096,varname:node_408,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-2636-R;n:type:ShaderForge.SFN_Vector1,id:891,x:32561,y:35995,varname:node_891,prsc:2,v1:0;n:type:ShaderForge.SFN_Append,id:8935,x:32754,y:36096,varname:node_8935,prsc:2|A-891-OUT,B-408-OUT,C-891-OUT;n:type:ShaderForge.SFN_Add,id:1902,x:32571,y:34771,varname:node_1902,prsc:2|A-4392-U,B-1300-OUT;n:type:ShaderForge.SFN_Time,id:734,x:31576,y:34851,varname:node_734,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1300,x:31829,y:34790,varname:node_1300,prsc:2|A-7720-OUT,B-734-T;n:type:ShaderForge.SFN_Slider,id:7720,x:31456,y:34730,ptovrint:False,ptlb:growthPanX,ptin:_growthPanX,varname:node_7720,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:8445,x:33173,y:36122,varname:node_8445,prsc:2|A-133-OUT,B-2993-U;n:type:ShaderForge.SFN_TexCoord,id:2993,x:32963,y:36270,varname:node_2993,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Subtract,id:1437,x:33521,y:34764,varname:node_1437,prsc:2|A-3483-R,B-5376-OUT;n:type:ShaderForge.SFN_Subtract,id:8179,x:33246,y:35025,varname:node_8179,prsc:2|A-3483-R,B-9553-OUT;n:type:ShaderForge.SFN_Ceil,id:7087,x:33759,y:34816,varname:node_7087,prsc:2|IN-1437-OUT;n:type:ShaderForge.SFN_Add,id:4103,x:33624,y:33908,varname:node_4103,prsc:2|A-636-OUT,B-7087-OUT;n:type:ShaderForge.SFN_Ceil,id:5376,x:33474,y:35053,varname:node_5376,prsc:2|IN-8179-OUT;proporder:4229-556-2580-2079-5009-3173-1042-3473-9062-7107-5763-6517-8805-1778-6923-1166-9553-6265-5302-6638-4407-8234-9160-6660-3070-2981-9852-9877-1917-2636-1473-3483-7720;pass:END;sub:END;*/

Shader "Shader Forge/S_Cocoon" {
    Properties {
        _EmissiveTex ("EmissiveTex", 2D) = "white" {}
        _metallic ("metallic", Range(0, 1)) = 0
        _roughness ("roughness", Range(0, 1)) = 0
        _BaseCol ("BaseCol", Color) = (0.5,0.5,0.5,1)
        _emissiveIntensity ("emissiveIntensity", Range(0, 100)) = 1
        _emissiveFresnelExp ("emissiveFresnelExp", Range(0, 20)) = 1
        _emissiveFresnelIntensity ("emissiveFresnelIntensity", Range(0, 100)) = 0
        _subColorTex ("subColorTex", 2D) = "white" {}
        _ParallaxRatio ("ParallaxRatio", Range(0, 1)) = 0.76
        _Depth ("Depth", Range(0, 1)) = 1
        _uvDistTex ("uvDistTex", 2D) = "white" {}
        _uvDist ("uvDist", Range(0, 1)) = 0
        _uvDistMaint ("uvDistMaint", Range(0, 1)) = 0
        _opacityMask ("opacityMask", 2D) = "white" {}
        _opacityMaskExp ("opacityMaskExp", Range(-2, 1)) = 1
        [MaterialToggle] _useOpacity ("useOpacity", Float ) = 1
        _diff ("diff", Range(0, 1)) = 0.1
        _displace ("displace", Range(0, 15)) = 2
        _edgeHighlight ("edgeHighlight", Range(0, 25)) = 15
        [MaterialToggle] _useDisplacement ("useDisplacement", Float ) = 0
        _displacementNoise ("displacementNoise", 2D) = "white" {}
        _e ("e", Float ) = 0
        _m ("m", Float ) = 0
        _col_copy_copy ("col_copy_copy", Color) = (0,0.5,0.5,1)
        [MaterialToggle] _dynamicDissolve ("dynamicDissolve", Float ) = 1
        [MaterialToggle] _use_emissive ("use_emissive", Float ) = 0
        _colorMod ("colorMod", Color) = (0.5,0.5,0.5,1)
        _fresnelNoise ("fresnelNoise", 2D) = "white" {}
        _verticalMaskIntensity ("verticalMaskIntensity", Float ) = 0
        _lowScaleDisplacementNoise ("lowScaleDisplacementNoise", 2D) = "white" {}
        [MaterialToggle] _useVerticalGradientDissolve ("useVerticalGradientDissolve", Float ) = 0
        _growthMask ("growthMask", 2D) = "white" {}
        _growthPanX ("growthPanX", Range(0, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Assets/Alloy/AutoLight.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform sampler2D _EmissiveTex; uniform float4 _EmissiveTex_ST;
            uniform float _metallic;
            uniform float _roughness;
            uniform float4 _BaseCol;
            uniform float _emissiveIntensity;
            uniform float _emissiveFresnelExp;
            uniform float _emissiveFresnelIntensity;
            uniform sampler2D _subColorTex; uniform float4 _subColorTex_ST;
            uniform float _ParallaxRatio;
            uniform float _Depth;
            uniform sampler2D _uvDistTex; uniform float4 _uvDistTex_ST;
            uniform float _uvDist;
            uniform float _uvDistMaint;
            uniform sampler2D _opacityMask; uniform float4 _opacityMask_ST;
            uniform float _opacityMaskExp;
            uniform float _diff;
            uniform float _displace;
            uniform float _edgeHighlight;
            uniform fixed _useDisplacement;
            uniform sampler2D _displacementNoise; uniform float4 _displacementNoise_ST;
            uniform float4 _col_copy_copy;
            uniform float _e;
            uniform float _m;
            uniform fixed _dynamicDissolve;
            uniform fixed _use_emissive;
            uniform float4 _colorMod;
            uniform sampler2D _fresnelNoise; uniform float4 _fresnelNoise_ST;
            uniform float _verticalMaskIntensity;
            uniform sampler2D _lowScaleDisplacementNoise; uniform float4 _lowScaleDisplacementNoise_ST;
            uniform fixed _useVerticalGradientDissolve;
            uniform sampler2D _growthMask; uniform float4 _growthMask_ST;
            uniform float _growthPanX;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(7,8)
                UNITY_FOG_COORDS(9)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD10;
                #endif
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
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float4 texcoord0 : TEXCOORD0;
                    float4 texcoord1 : TEXCOORD1;
                    float2 texcoord2 : TEXCOORD2;
                    float4 vertexColor : COLOR;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float4 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    o.texcoord1 = v.texcoord1;
                    o.texcoord2 = v.texcoord2;
                    o.vertexColor = v.vertexColor;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float4 node_6827 = _Time;
                    float2 node_6043 = (v.texcoord0+node_6827.g*float2(0,0.05));
                    float4 _displacementNoise_var = tex2Dlod(_displacementNoise,float4(TRANSFORM_TEX(node_6043, _displacementNoise),0.0,0));
                    float3 node_1759 = ((_displace*v.texcoord0.b*float3(2,1,2))*_displacementNoise_var.r*v.normal);
                    float node_891 = 0.0;
                    float2 node_7747 = (v.texcoord0+node_6827.g*float2(0.05,0.05));
                    float4 _lowScaleDisplacementNoise_var = tex2Dlod(_lowScaleDisplacementNoise,float4(TRANSFORM_TEX(node_7747, _lowScaleDisplacementNoise),0.0,0));
                    v.vertex.xyz += lerp( 0.0, (node_1759+((float3(node_891,(_lowScaleDisplacementNoise_var.r*2.0+-1.0),node_891)*v.vertexColor.a)*v.texcoord0.r)), _useDisplacement );
                }
                float Tessellation(TessVertex v){
                    return 15.0;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    v.vertexColor = vi[0].vertexColor*bary.x + vi[1].vertexColor*bary.y + vi[2].vertexColor*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float2 node_7962 = float2(i.uv0.r,(i.uv0.g+i.uv0.a));
                float4 _opacityMask_var = tex2D(_opacityMask,TRANSFORM_TEX(node_7962, _opacityMask));
                float _dynamicDissolve_var = lerp( _opacityMaskExp, i.uv1.r, _dynamicDissolve );
                float node_563 = (_opacityMask_var.r-_dynamicDissolve_var);
                float node_9853 = saturate(node_563);
                float node_3096 = ceil(node_9853);
                float4 node_734 = _Time;
                float _useVerticalGradientDissolve_var = lerp( 0.0, i.uv1.r, _useVerticalGradientDissolve );
                float2 node_3690 = float2((i.uv0.r+(_growthPanX*node_734.g)),(i.uv0.g+_useVerticalGradientDissolve_var));
                float4 _growthMask_var = tex2D(_growthMask,TRANSFORM_TEX(node_3690, _growthMask));
                clip((node_3096*_growthMask_var.r) - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = 1.0 - _roughness; // Convert roughness to gloss
                float perceptualRoughness = _roughness;
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
                float3 specularColor = _metallic;
                float specularMonochrome;
                float3 diffuseColor = _BaseCol.rgb; // Need this for specular when using metallic
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
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float4 node_6827 = _Time;
                float2 node_5618 = (i.uv0+node_6827.g*float2(0.1,0.1));
                float4 _uvDistTex_var = tex2D(_uvDistTex,TRANSFORM_TEX(node_5618, _uvDistTex));
                float2 node_2968 = (i.uv0+(_uvDistMaint*_uvDistTex_var.r));
                float4 _EmissiveTex_var = tex2D(_EmissiveTex,TRANSFORM_TEX(node_2968, _EmissiveTex));
                float2 node_4465 = (_Depth*(_EmissiveTex_var.g - _ParallaxRatio)*mul(tangentTransform, viewDirection).xy + (i.uv0+(_uvDistTex_var.r*_uvDist)));
                float4 _subColorTex_var = tex2D(_subColorTex,TRANSFORM_TEX(node_4465.rg, _subColorTex));
                float3 node_3154 = (((_EmissiveTex_var.rgb*_emissiveIntensity)+((_EmissiveTex_var.rgb*pow(1.0-max(0,dot(normalDirection, viewDirection)),_emissiveFresnelExp))*_emissiveFresnelIntensity))+_subColorTex_var.rgb);
                float2 node_759 = (i.uv0+node_6827.g*float2(0.01,0.03));
                float4 _fresnelNoise_var = tex2D(_fresnelNoise,TRANSFORM_TEX(node_759, _fresnelNoise));
                float node_3348 = 1.0;
                float3 emissive = lerp( 0.0, (lerp(lerp(lerp(node_3154,(node_3154*_edgeHighlight),((node_3096-ceil(saturate((_opacityMask_var.r-(_dynamicDissolve_var+_diff)))))+ceil((_growthMask_var.r-ceil((_growthMask_var.r-_diff)))))),_col_copy_copy.rgb,saturate((pow((1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),0.1)),_e)*(_m*i.uv1.g)*_fresnelNoise_var.r))),dot(lerp(lerp(node_3154,(node_3154*_edgeHighlight),((node_3096-ceil(saturate((_opacityMask_var.r-(_dynamicDissolve_var+_diff)))))+ceil((_growthMask_var.r-ceil((_growthMask_var.r-_diff)))))),_col_copy_copy.rgb,saturate((pow((1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),0.1)),_e)*(_m*i.uv1.g)*_fresnelNoise_var.r))),float3(0.3,0.59,0.11)),i.uv1.b)*lerp(float3(node_3348,node_3348,node_3348),_colorMod.rgb,(i.uv1.b*2.0))*saturate((i.uv0.g*_verticalMaskIntensity))), _use_emissive );
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
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
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 5.0
            uniform sampler2D _opacityMask; uniform float4 _opacityMask_ST;
            uniform float _opacityMaskExp;
            uniform float _displace;
            uniform fixed _useDisplacement;
            uniform sampler2D _displacementNoise; uniform float4 _displacementNoise_ST;
            uniform fixed _dynamicDissolve;
            uniform sampler2D _lowScaleDisplacementNoise; uniform float4 _lowScaleDisplacementNoise_ST;
            uniform fixed _useVerticalGradientDissolve;
            uniform sampler2D _growthMask; uniform float4 _growthMask_ST;
            uniform float _growthPanX;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 uv0 : TEXCOORD1;
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
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float4 texcoord0 : TEXCOORD0;
                    float4 texcoord1 : TEXCOORD1;
                    float2 texcoord2 : TEXCOORD2;
                    float4 vertexColor : COLOR;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float4 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    o.texcoord1 = v.texcoord1;
                    o.texcoord2 = v.texcoord2;
                    o.vertexColor = v.vertexColor;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float4 node_7878 = _Time;
                    float2 node_6043 = (v.texcoord0+node_7878.g*float2(0,0.05));
                    float4 _displacementNoise_var = tex2Dlod(_displacementNoise,float4(TRANSFORM_TEX(node_6043, _displacementNoise),0.0,0));
                    float3 node_1759 = ((_displace*v.texcoord0.b*float3(2,1,2))*_displacementNoise_var.r*v.normal);
                    float node_891 = 0.0;
                    float2 node_7747 = (v.texcoord0+node_7878.g*float2(0.05,0.05));
                    float4 _lowScaleDisplacementNoise_var = tex2Dlod(_lowScaleDisplacementNoise,float4(TRANSFORM_TEX(node_7747, _lowScaleDisplacementNoise),0.0,0));
                    v.vertex.xyz += lerp( 0.0, (node_1759+((float3(node_891,(_lowScaleDisplacementNoise_var.r*2.0+-1.0),node_891)*v.vertexColor.a)*v.texcoord0.r)), _useDisplacement );
                }
                float Tessellation(TessVertex v){
                    return 15.0;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    v.vertexColor = vi[0].vertexColor*bary.x + vi[1].vertexColor*bary.y + vi[2].vertexColor*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float2 node_7962 = float2(i.uv0.r,(i.uv0.g+i.uv0.a));
                float4 _opacityMask_var = tex2D(_opacityMask,TRANSFORM_TEX(node_7962, _opacityMask));
                float _dynamicDissolve_var = lerp( _opacityMaskExp, i.uv1.r, _dynamicDissolve );
                float node_563 = (_opacityMask_var.r-_dynamicDissolve_var);
                float node_9853 = saturate(node_563);
                float node_3096 = ceil(node_9853);
                float4 node_734 = _Time;
                float _useVerticalGradientDissolve_var = lerp( 0.0, i.uv1.r, _useVerticalGradientDissolve );
                float2 node_3690 = float2((i.uv0.r+(_growthPanX*node_734.g)),(i.uv0.g+_useVerticalGradientDissolve_var));
                float4 _growthMask_var = tex2D(_growthMask,TRANSFORM_TEX(node_3690, _growthMask));
                clip((node_3096*_growthMask_var.r) - 0.5);
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
            #pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "Tessellation.cginc"
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
            #pragma target 5.0
            uniform sampler2D _EmissiveTex; uniform float4 _EmissiveTex_ST;
            uniform float _metallic;
            uniform float _roughness;
            uniform float4 _BaseCol;
            uniform float _emissiveIntensity;
            uniform float _emissiveFresnelExp;
            uniform float _emissiveFresnelIntensity;
            uniform sampler2D _subColorTex; uniform float4 _subColorTex_ST;
            uniform float _ParallaxRatio;
            uniform float _Depth;
            uniform sampler2D _uvDistTex; uniform float4 _uvDistTex_ST;
            uniform float _uvDist;
            uniform float _uvDistMaint;
            uniform sampler2D _opacityMask; uniform float4 _opacityMask_ST;
            uniform float _opacityMaskExp;
            uniform float _diff;
            uniform float _displace;
            uniform float _edgeHighlight;
            uniform fixed _useDisplacement;
            uniform sampler2D _displacementNoise; uniform float4 _displacementNoise_ST;
            uniform float4 _col_copy_copy;
            uniform float _e;
            uniform float _m;
            uniform fixed _dynamicDissolve;
            uniform fixed _use_emissive;
            uniform float4 _colorMod;
            uniform sampler2D _fresnelNoise; uniform float4 _fresnelNoise_ST;
            uniform float _verticalMaskIntensity;
            uniform sampler2D _lowScaleDisplacementNoise; uniform float4 _lowScaleDisplacementNoise_ST;
            uniform fixed _useVerticalGradientDissolve;
            uniform sampler2D _growthMask; uniform float4 _growthMask_ST;
            uniform float _growthPanX;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                return o;
            }
            #ifdef UNITY_CAN_COMPILE_TESSELLATION
                struct TessVertex {
                    float4 vertex : INTERNALTESSPOS;
                    float3 normal : NORMAL;
                    float4 tangent : TANGENT;
                    float4 texcoord0 : TEXCOORD0;
                    float4 texcoord1 : TEXCOORD1;
                    float2 texcoord2 : TEXCOORD2;
                    float4 vertexColor : COLOR;
                };
                struct OutputPatchConstant {
                    float edge[3]         : SV_TessFactor;
                    float inside          : SV_InsideTessFactor;
                    float3 vTangent[4]    : TANGENT;
                    float4 vUV[4]         : TEXCOORD;
                    float3 vTanUCorner[4] : TANUCORNER;
                    float3 vTanVCorner[4] : TANVCORNER;
                    float4 vCWts          : TANWEIGHTS;
                };
                TessVertex tessvert (VertexInput v) {
                    TessVertex o;
                    o.vertex = v.vertex;
                    o.normal = v.normal;
                    o.tangent = v.tangent;
                    o.texcoord0 = v.texcoord0;
                    o.texcoord1 = v.texcoord1;
                    o.texcoord2 = v.texcoord2;
                    o.vertexColor = v.vertexColor;
                    return o;
                }
                void displacement (inout VertexInput v){
                    float4 node_8627 = _Time;
                    float2 node_6043 = (v.texcoord0+node_8627.g*float2(0,0.05));
                    float4 _displacementNoise_var = tex2Dlod(_displacementNoise,float4(TRANSFORM_TEX(node_6043, _displacementNoise),0.0,0));
                    float3 node_1759 = ((_displace*v.texcoord0.b*float3(2,1,2))*_displacementNoise_var.r*v.normal);
                    float node_891 = 0.0;
                    float2 node_7747 = (v.texcoord0+node_8627.g*float2(0.05,0.05));
                    float4 _lowScaleDisplacementNoise_var = tex2Dlod(_lowScaleDisplacementNoise,float4(TRANSFORM_TEX(node_7747, _lowScaleDisplacementNoise),0.0,0));
                    v.vertex.xyz += lerp( 0.0, (node_1759+((float3(node_891,(_lowScaleDisplacementNoise_var.r*2.0+-1.0),node_891)*v.vertexColor.a)*v.texcoord0.r)), _useDisplacement );
                }
                float Tessellation(TessVertex v){
                    return 15.0;
                }
                float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
                    float tv = Tessellation(v);
                    float tv1 = Tessellation(v1);
                    float tv2 = Tessellation(v2);
                    return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
                }
                OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
                    OutputPatchConstant o = (OutputPatchConstant)0;
                    float4 ts = Tessellation( v[0], v[1], v[2] );
                    o.edge[0] = ts.x;
                    o.edge[1] = ts.y;
                    o.edge[2] = ts.z;
                    o.inside = ts.w;
                    return o;
                }
                [domain("tri")]
                [partitioning("fractional_odd")]
                [outputtopology("triangle_cw")]
                [patchconstantfunc("hullconst")]
                [outputcontrolpoints(3)]
                TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
                    return v[id];
                }
                [domain("tri")]
                VertexOutput domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
                    VertexInput v = (VertexInput)0;
                    v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
                    v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
                    v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
                    v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
                    v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
                    v.vertexColor = vi[0].vertexColor*bary.x + vi[1].vertexColor*bary.y + vi[2].vertexColor*bary.z;
                    displacement(v);
                    VertexOutput o = vert(v);
                    return o;
                }
            #endif
            float4 frag(VertexOutput i, float facing : VFACE) : SV_Target {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float4 node_8627 = _Time;
                float2 node_5618 = (i.uv0+node_8627.g*float2(0.1,0.1));
                float4 _uvDistTex_var = tex2D(_uvDistTex,TRANSFORM_TEX(node_5618, _uvDistTex));
                float2 node_2968 = (i.uv0+(_uvDistMaint*_uvDistTex_var.r));
                float4 _EmissiveTex_var = tex2D(_EmissiveTex,TRANSFORM_TEX(node_2968, _EmissiveTex));
                float2 node_4465 = (_Depth*(_EmissiveTex_var.g - _ParallaxRatio)*mul(tangentTransform, viewDirection).xy + (i.uv0+(_uvDistTex_var.r*_uvDist)));
                float4 _subColorTex_var = tex2D(_subColorTex,TRANSFORM_TEX(node_4465.rg, _subColorTex));
                float3 node_3154 = (((_EmissiveTex_var.rgb*_emissiveIntensity)+((_EmissiveTex_var.rgb*pow(1.0-max(0,dot(normalDirection, viewDirection)),_emissiveFresnelExp))*_emissiveFresnelIntensity))+_subColorTex_var.rgb);
                float2 node_7962 = float2(i.uv0.r,(i.uv0.g+i.uv0.a));
                float4 _opacityMask_var = tex2D(_opacityMask,TRANSFORM_TEX(node_7962, _opacityMask));
                float _dynamicDissolve_var = lerp( _opacityMaskExp, i.uv1.r, _dynamicDissolve );
                float node_563 = (_opacityMask_var.r-_dynamicDissolve_var);
                float node_9853 = saturate(node_563);
                float node_3096 = ceil(node_9853);
                float4 node_734 = _Time;
                float _useVerticalGradientDissolve_var = lerp( 0.0, i.uv1.r, _useVerticalGradientDissolve );
                float2 node_3690 = float2((i.uv0.r+(_growthPanX*node_734.g)),(i.uv0.g+_useVerticalGradientDissolve_var));
                float4 _growthMask_var = tex2D(_growthMask,TRANSFORM_TEX(node_3690, _growthMask));
                float2 node_759 = (i.uv0+node_8627.g*float2(0.01,0.03));
                float4 _fresnelNoise_var = tex2D(_fresnelNoise,TRANSFORM_TEX(node_759, _fresnelNoise));
                float node_3348 = 1.0;
                o.Emission = lerp( 0.0, (lerp(lerp(lerp(node_3154,(node_3154*_edgeHighlight),((node_3096-ceil(saturate((_opacityMask_var.r-(_dynamicDissolve_var+_diff)))))+ceil((_growthMask_var.r-ceil((_growthMask_var.r-_diff)))))),_col_copy_copy.rgb,saturate((pow((1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),0.1)),_e)*(_m*i.uv1.g)*_fresnelNoise_var.r))),dot(lerp(lerp(node_3154,(node_3154*_edgeHighlight),((node_3096-ceil(saturate((_opacityMask_var.r-(_dynamicDissolve_var+_diff)))))+ceil((_growthMask_var.r-ceil((_growthMask_var.r-_diff)))))),_col_copy_copy.rgb,saturate((pow((1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),0.1)),_e)*(_m*i.uv1.g)*_fresnelNoise_var.r))),float3(0.3,0.59,0.11)),i.uv1.b)*lerp(float3(node_3348,node_3348,node_3348),_colorMod.rgb,(i.uv1.b*2.0))*saturate((i.uv0.g*_verticalMaskIntensity))), _use_emissive );
                
                float3 diffColor = _BaseCol.rgb;
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _metallic, specColor, specularMonochrome );
                float roughness = _roughness;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
