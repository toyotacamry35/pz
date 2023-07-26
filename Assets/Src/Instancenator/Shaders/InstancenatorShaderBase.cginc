#ifndef INSTANCENATOR_SHADER_BASE

#define INSTANCENATOR_SHADER_BASE


#include "UnityStandardUtils.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityCG.cginc"

//========================================================================================
//Common parameters
//========================================================================================

//Per instance
CBUFFER_START(Instancenator)
//Unpack part
float3 instancePosition;
float4 instanceQuaternion;
float4 instanceValue;
float3x3 instanceRotMat;
float4 instanceTransitionMask;
uint instanceOptions;
//Calculate part
float3 instanceTransitionScale;
float3 instanceWorldPosition;
CBUFFER_END

struct InstanceData
{
	//Unpack part
	float3 instancePosition;
	float4 instanceQuaternion;
	float4 instanceValue;
	float3x3 instanceRotMat;
	uint instanceOptions;
	//Calculate part
	float3 instanceTransitionScale;
	float3 instanceWorldPosition;
};


InstanceData GetInstanceData()
{
	InstanceData data;
	data.instancePosition = instancePosition;
	data.instanceQuaternion = instanceQuaternion;
	data.instanceValue = instanceValue;
	data.instanceRotMat = instanceRotMat;
	data.instanceOptions = instanceOptions;
	data.instanceTransitionScale = instanceTransitionScale;
	data.instanceWorldPosition = instanceWorldPosition;
	return data;
}


//Global
float3 BoxSize;
int InstancesOffset;
float3 BoxMin;
float LodBiasReciprocal;
float4 ValueMul;
float4 ValueAdd;
float4 TransitionMul;
float4 TransitionAdd;
float4x4 ObjectToWorldMtx;


//========================================================================================
//system part

struct PackInstanceData
{
	uint posz16_posx16;
	uint quatw16_posy16;
	uint opts3_tr2_quatys1_quatz13_quatx13;
	uint value8888;
};


float3x3 QuaternionToMatrix(float4 quat)
{
	float3 quat2 = quat.xyz * 2;

	float wx, wy, wz, xx, yy, yz, xy, xz, zz;
	xx = quat.x * quat2.x;   xy = quat.x * quat2.y;   xz = quat.x * quat2.z;
	yy = quat.y * quat2.y;   yz = quat.y * quat2.z;   zz = quat.z * quat2.z;
	wx = quat.w * quat2.x;   wy = quat.w * quat2.y;   wz = quat.w * quat2.z;

	float3x3 mat = float3x3(1.0f - (yy + zz), xy - wz, xz + wy,
							xy + wz, 1.0f - (xx + zz), yz - wx,
							xz - wy, yz + wx, 1.0f - (xx + yy));

	return mat;
}

#define UNPACK_FLOAT_FROM_UINT(v, shift, mask)	(((v >> (shift)) & (mask)) / ((float)(mask)))

void Unpack(PackInstanceData packData)
{
	//Unpack position
	instancePosition.x = UNPACK_FLOAT_FROM_UINT(packData.posz16_posx16, 0, 0xffff);
	instancePosition.y = UNPACK_FLOAT_FROM_UINT(packData.quatw16_posy16, 0, 0xffff);
	instancePosition.z = UNPACK_FLOAT_FROM_UINT(packData.posz16_posx16, 16, 0xffff);
	instancePosition = instancePosition * BoxSize + BoxMin;

	//Unpack quaternion
	instanceQuaternion.x = UNPACK_FLOAT_FROM_UINT(packData.opts3_tr2_quatys1_quatz13_quatx13, 0, 0x1fff);
	instanceQuaternion.z = UNPACK_FLOAT_FROM_UINT(packData.opts3_tr2_quatys1_quatz13_quatx13, 13, 0x1fff);
	instanceQuaternion.w = UNPACK_FLOAT_FROM_UINT(packData.quatw16_posy16, 16, 0xffff);

	instanceQuaternion.xzw = instanceQuaternion.xzw * 2.0 - 1.0;
	instanceQuaternion.y = sqrt(saturate(1.0 - dot(instanceQuaternion.xzw, instanceQuaternion.xzw)));
	instanceQuaternion.y = (packData.opts3_tr2_quatys1_quatz13_quatx13 & (1 << 26)) == 0 ? instanceQuaternion.y : -instanceQuaternion.y;	

	//Unpack value
	instanceValue.x = UNPACK_FLOAT_FROM_UINT(packData.value8888, 0, 0xff);
	instanceValue.y = UNPACK_FLOAT_FROM_UINT(packData.value8888, 8, 0xff);
	instanceValue.z = UNPACK_FLOAT_FROM_UINT(packData.value8888, 16, 0xff);
	instanceValue.w = UNPACK_FLOAT_FROM_UINT(packData.value8888, 24, 0xff);
	instanceValue = instanceValue * ValueMul + ValueAdd;

	//Rotation matrix
	instanceRotMat = QuaternionToMatrix(instanceQuaternion);

	//Options
	instanceOptions = (packData.opts3_tr2_quatys1_quatz13_quatx13 >> 29) & 7;

	//Transition mask
	instanceTransitionMask = (packData.opts3_tr2_quatys1_quatz13_quatx13 &  0x10000000) ? float4(0, 0, 1, 1) : float4(1, 1, 0, 0);
	instanceTransitionMask *= (packData.opts3_tr2_quatys1_quatz13_quatx13 & 0x08000000) ? float4(0, 1, 0, 1) : float4(1, 0, 1, 0);
}

float3 CalculateTransitionScale()
{
	float distHor = length(_WorldSpaceCameraPos.xz - instanceWorldPosition.xz);
	float distVert = abs(_WorldSpaceCameraPos.y - instanceWorldPosition.y);
	float4 transitionMulCorrected = LodBiasReciprocal * TransitionMul;
	float4 transitionScaleHor = 1 - saturate(distHor * transitionMulCorrected + TransitionAdd);
	float4 transitionScaleVert = 1 - saturate(distVert * transitionMulCorrected + TransitionAdd);
	float4 transitionScale = transitionScaleHor * transitionScaleVert;
	float scale = dot(instanceTransitionMask * transitionScale, 1);
	float kDifScale = 3;
	return saturate(float3(scale, scale * kDifScale, scale));
}



#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
StructuredBuffer<PackInstanceData> InstanceBuffer;
#endif


void setup()
{
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
	PackInstanceData data = InstanceBuffer[unity_InstanceID + InstancesOffset];
	Unpack(data);
	instanceWorldPosition = mul(ObjectToWorldMtx, float4(instancePosition, 1));
	instanceTransitionScale = CalculateTransitionScale();

	float4x4 localToObj;
	
	localToObj._11_21_31_41 = float4(instanceRotMat._11_21_31, 0);
	localToObj._12_22_32_42 = float4(instanceRotMat._12_22_32, 0);
	localToObj._13_23_33_43 = float4(instanceRotMat._13_23_33, 0);
	localToObj._14_24_34_44 = float4(instancePosition, 1);

	unity_ObjectToWorld = mul(ObjectToWorldMtx, localToObj);

	// this matrix inversion algorhytm ignores scale
	// let R - rotation matrix => R^-1 = transpose(R)
	unity_WorldToObject._11_21_31_41 = float4(unity_ObjectToWorld._11_12_13, 0);
	unity_WorldToObject._12_22_32_42 = float4(unity_ObjectToWorld._21_22_23, 0);
	unity_WorldToObject._13_23_33_43 = float4(unity_ObjectToWorld._31_32_33, 0);
	// let T - translation matrix => T^-1 = -T
	// (AB)^-1 = B^-1 * A^-1
	float3 invPos = mul((float3x3)unity_WorldToObject, -unity_ObjectToWorld._14_24_34);

	unity_WorldToObject._14_24_34_44 = float4(invPos, 1);
	
#endif
}


#endif
