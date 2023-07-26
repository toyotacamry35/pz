struct VertexInput
{
    UNITY_VERTEX_INPUT_INSTANCE_ID
    float4 vertex : POSITION;
};

struct v2f
{
    float4 position : SV_POSITION;
    float4 screenPosMulByW : TEXCOORD1;
    float3 viewSpaceVertPos : TEXCOORD2;
    half3 decalNormal : TEXCOORD3;
    half3 decalTangent : TEXCOORD4;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

v2f vert(VertexInput v)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    o.position = UnityObjectToClipPos(v.vertex);
    o.screenPosMulByW = ComputeScreenPos(o.position);
    o.viewSpaceVertPos = UnityObjectToViewPos(v.vertex) * float3(-1, -1, 1);
    o.decalNormal  = normalize(mul((float3x3)unity_ObjectToWorld, float3(0, 1, 0))); // 'up' is '+Y"
    o.decalTangent = normalize(mul((float3x3)unity_ObjectToWorld, float3(1, 0, 0)));
    return o;
}

// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
// #pragma instancing_options assumeuniformscaling
UNITY_INSTANCING_BUFFER_START(Props)
// put more per-instance properties here
UNITY_DEFINE_INSTANCED_PROP(float, _MaskMultiplier)
UNITY_INSTANCING_BUFFER_END(Props)

#define FRAGMENT_START UNITY_SETUP_INSTANCE_ID(i); \
float2 screenPos = i.screenPosMulByW.xy / i.screenPosMulByW.w; \
float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenPos)); \
float3 viewSpaceDirection = (_ProjectionParams.z / i.viewSpaceVertPos.z) * i.viewSpaceVertPos; \
/* direction with z = 1 */ \
float3 viewSpacePosition = viewSpaceDirection * depth; \
float3 worldSpacePosition = mul(unity_CameraToWorld, float4(viewSpacePosition, 1)).xyz; \
float3 objectSpacePosition = mul(unity_WorldToObject, float4(worldSpacePosition,1)).xyz; \
/* clip all pixels outside of our cube */ \
clip(0.5 - abs(objectSpacePosition)); \
/* xz instead of xy here because default rotation in Unity is 'Y' axis pointing 'up'
float2 texUV = TRANSFORM_TEX((objectSpacePosition.xz + 0.5), COLOR_TEXTURE); */ \
float2 texUV = objectSpacePosition.xz + 0.5; \
float4 color = tex2D(COLOR_TEXTURE, texUV); \
color.a *= UNITY_ACCESS_INSTANCED_PROP(Props, _MaskMultiplier); \
/* right-handed CS for normals */ \
half3 decalBitangent = cross(i.decalNormal, i.decalTangent); \
float3 nonOrientedNormal = UnpackNormal(tex2D(_NormalTex, texUV)).xyz; \
normals.w = color.a; \
normals.xyz = (mul(nonOrientedNormal, half3x3(i.decalTangent, decalBitangent, i.decalNormal)) * 0.5 + 0.5);