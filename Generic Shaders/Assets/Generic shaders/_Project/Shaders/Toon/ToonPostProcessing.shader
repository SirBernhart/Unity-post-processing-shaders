Shader "Hidden/Bernardo/ToonPostProcessing"
{
    HLSLINCLUDE
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_CameraNormalsTexture, sampler_CameraNormalsTexture);

    float _ShadowBrightness;
    float _TransitionSmoothness;
    float _MaxShadedValue;
    float _MaxBrightness;
    int _NumberOfSteps;
    float _BandBrightnessArray[5];
    float _EdgesArray[4];

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

    float Posterize(float In, float steps)
    {
        return floor(In * steps/_MaxBrightness) * _MaxBrightness/steps;
    }

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        float3 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).xyz;
        float3 hsvColor = RgbToHsv(color);

        float newValue = _BandBrightnessArray[_NumberOfSteps - 1];

        for(int index = 0 ; index < _NumberOfSteps-1 ; index++)
        {
            float currEdge = _EdgesArray[index];
            float nextEdge = _EdgesArray[index + 1];
            if(hsvColor.z < (currEdge + nextEdge)/2)
            {
                newValue = _BandBrightnessArray[index] +
                    smoothstep(currEdge - _TransitionSmoothness, currEdge + _TransitionSmoothness, hsvColor.z)
                    * (_BandBrightnessArray[index+1] - _BandBrightnessArray[index]);
                break;
            }
        }

        hsvColor.z = newValue;

        //float lowerEdge = GetLowerEdge(hsvColor.z, _NumberOfSteps);
        //hsvColor.z = Posterize(hsvColor.z, _NumberOfSteps);
        /*hsvColor.z = min(hsvColor.z, _MaxBrightness);
        hsvColor.z = max(hsvColor.z, _ShadowBrightness);*/
        /*float upperEdge = lowerEdge + _TransitionSmoothness;
        hsvColor.z = smoothstep(lowerEdge, upperEdge, hsvColor.z);
        hsvColor.z = min(hsvColor.z, _MaxBrightness);*/

        /*float upperEdge = _MaxShadedValue + _TransitionSmoothness;
        hsvColor.z = _ShadowBrightness + smoothstep(_MaxShadedValue, upperEdge, hsvColor.z) * (1.0 - _ShadowBrightness);
        hsvColor.z = min(hsvColor.z, _MaxBrightness);*/

        color = HsvToRgb(hsvColor);

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