Shader "Hidden/Bernardo/HatchingPostProcessing"
{
    HLSLINCLUDE
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_HatchTex0, sampler_HatchTex0);
    TEXTURE2D_SAMPLER2D(_HatchTex1, sampler_HatchTex1);
    TEXTURE2D_SAMPLER2D(_HatchTex2, sampler_HatchTex2);
    TEXTURE2D_SAMPLER2D(_HatchTex3, sampler_HatchTex3);
    TEXTURE2D_SAMPLER2D(_HatchTex4, sampler_HatchTex4);
    TEXTURE2D_SAMPLER2D(_HatchTex5, sampler_HatchTex5);
    TEXTURE2D_SAMPLER2D(_HatchTex6, sampler_HatchTex6);

    float _TransitionSmoothness;
    int _NumberOfSteps;
    float _EdgesArray[7];
    float _HatchTilingFactor;
    float4 _BaseColor;
    bool _UseOriginalColor;

    // From https://github.com/Unity-Technologies/FPSSample/blob/master/Packages/com.unity.postprocessing/PostProcessing/Shaders/Colors.hlsl
    // Hue, Saturation, Value
    // Ranges:
    //  Hue [0.0, 1.0]
    //  Sat [0.0, 1.0]
    //  Lum [0.0, HALF_MAX]
    float3 RgbToHsv(float3 c)
    {
        const float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
        float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
        float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
        float d = q.x - min(q.w, q.y);
        const float e = 1.0e-4;
        return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
    }

    float3 HsvToRgb(float3 c)
    {
        const float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
        float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
        return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
    }

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float3 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).xyz;
        float3 hsvColor = RgbToHsv(color);
        float2 hatchTexCoord = float2(i.texcoord.x*_HatchTilingFactor, i.texcoord.y*_HatchTilingFactor);

        float3 hatch0Color = SAMPLE_TEXTURE2D(_HatchTex0, sampler_HatchTex0, hatchTexCoord).xyz;
        float3 hatch1Color = SAMPLE_TEXTURE2D(_HatchTex1, sampler_HatchTex1, hatchTexCoord).xyz;
        float3 hatch2Color = SAMPLE_TEXTURE2D(_HatchTex2, sampler_HatchTex2, hatchTexCoord).xyz;
        float3 hatch3Color = SAMPLE_TEXTURE2D(_HatchTex3, sampler_HatchTex3, hatchTexCoord).xyz;
        float3 hatch4Color = SAMPLE_TEXTURE2D(_HatchTex4, sampler_HatchTex4, hatchTexCoord).xyz;
        float3 hatch5Color = SAMPLE_TEXTURE2D(_HatchTex5, sampler_HatchTex5, hatchTexCoord).xyz;
        float3 hatch6Color = SAMPLE_TEXTURE2D(_HatchTex6, sampler_HatchTex6, hatchTexCoord).xyz;

        // Ordenados da hachura mais escura para a mais clara.
        float3 hatchColors[] =
        {
            hatch6Color,
            hatch5Color,
            hatch4Color,
            hatch3Color,
            hatch2Color,
            hatch1Color,
            hatch0Color
        };

        float3 newValue = hatchColors[_NumberOfSteps - 1];

        for(int index = 0 ; index < _NumberOfSteps-1 ; index++)
        {
            float currEdge = _EdgesArray[index];
            float nextEdge = _EdgesArray[index + 1];
            if(hsvColor.z < (currEdge + nextEdge)/2)
            {
                newValue = hatchColors[index] +
                    smoothstep(currEdge - _TransitionSmoothness, currEdge + _TransitionSmoothness, hsvColor.z)
                     * (hatchColors[index+1] - hatchColors[index]);
                break;
            }
        }

        newValue *= _UseOriginalColor ? color : 1;

        color = newValue * _BaseColor;

        return float4(color.x, color.y, color.z, 0);
    }
    ENDHLSL

    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment Frag
            ENDHLSL
        }
    }
}