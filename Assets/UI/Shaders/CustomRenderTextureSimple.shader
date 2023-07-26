Shader "CustomRenderTexture/Simple"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("InputTex", 2D) = "white" {}
    }

    SubShader
    {
        Lighting Off
        Blend One Zero

        Pass
        {
            Name "MainPass"
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4 _Color;
            sampler2D _MainTex;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                return _Color * tex2D(_MainTex, IN.localTexcoord.xy);
            }
            ENDCG
        }
    }
}