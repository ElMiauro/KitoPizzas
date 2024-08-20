Shader "Custom/VerticalWaveShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _WaveSpeed("Wave Speed", Float) = 1.0
        _WaveFrequency("Wave Frequency", Float) = 1.0
        _WaveAmplitude("Wave Amplitude", Float) = 0.1
        _PlayerPosition("Player Position", Vector) = (0,0,0,0)
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _WaveSpeed;
                float _WaveFrequency;
                float _WaveAmplitude;
                float4 _PlayerPosition;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    float distanceToPlayer = length(worldPos - _PlayerPosition.xyz);
                    float wave = sin(distanceToPlayer * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveAmplitude;

                    // Apply the wave distortion only in the y-axis
                    worldPos.y += wave;

                    o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    return col;
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
