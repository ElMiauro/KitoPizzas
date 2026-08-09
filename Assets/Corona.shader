Shader "Custom/PositionBasedAlphaShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard

        struct Input
        {
            float3 worldPos; // Keep the input struct for later use
        };

        float4 _BaseColor;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Set the final color
            o.Albedo = _BaseColor.rgb;
            o.Alpha = 1.0; // Set alpha to 1 for now
        }
        ENDCG
    }
        FallBack "Diffuse"
}
