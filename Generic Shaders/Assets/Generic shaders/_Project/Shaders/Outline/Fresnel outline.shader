Shader "Holistic/Rim"
{
    Properties
    {
        _RimColor ("Rim color", Color) = (0,0.5,0,0.0)
        _RimPower ("Rim power", Range(0.5, 8.0)) = 3.0
        _Texture ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _Texture;

        struct Input
        {
            float3 viewDir;
            float2 uv_Texture;
        };

        float4 _RimColor;
        float _RimPower;

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = tex2D(_Texture, IN.uv_Texture).rgb;
            half rim = 1 - saturate(dot(normalize(IN.viewDir), o.Normal));
            o.Emission= /*_RimColor.rgb * pow(rim, _RimPower)*/rim > 0.6 ? _RimColor.rgb * pow(1, _RimPower) : 0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
