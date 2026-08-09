Shader "Unlit/Coronam"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _SubstractionFloat("Height", Float) = 0.5
        _DividingFloat("Stretch", Float) = -1.52
        _StepCount("Alpha Steps", Float) = 10
    }
        SubShader
        {
            Tags { "RenderType" = "Transparent" }
            LOD 100

            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert lpha:fade
                #pragma fragment frag
                // make fog work
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                    float3 localPos : TEXCOORD1; // Pass local position to the fragment shader
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _BaseColor;
                float _SubstractionFloat;
                float _DividingFloat;
                float _StepCount;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);

                    // Pass the world position to the fragment shader
                    o.localPos = v.vertex.xyz;

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Sample the texture
                    fixed4 col = tex2D(_MainTex, i.uv);

                // Calculate the alpha based on the Green component of the world position
                float gValue = i.localPos.g;
                float alpha = saturate((gValue - _SubstractionFloat) / _DividingFloat);

                // Apply step quantization to the alpha
                alpha = round(alpha * _StepCount) / _StepCount;

                // Apply the calculated alpha to the base color
                col.rgb = _BaseColor.rgb;
                col.a = _BaseColor.a * alpha;
                //col.a = alpha;

                // Apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
        }
}
