using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ToonPostProcessingRenderer), PostProcessEvent.BeforeStack, "Bernardo/ToonPostProcessing")]
public sealed class ToonPostProcessing : PostProcessEffectSettings
{
    [Range(0f, 1f)]
    public FloatParameter shadowBrightness = new FloatParameter {value = 0.5f};
    [Range(0f, 2f)]
    public FloatParameter maxBrightness = new FloatParameter {value = 0.75f};
    [Range(0f, 1f)]
    public FloatParameter transitionSmoothness = new FloatParameter {value = 0.5f};
    [Range(0f, 1f)]
    public FloatParameter maxShadedValue = new FloatParameter {value = 0.75f};
    [Range(2f, 5f)]
    public IntParameter steps = new IntParameter {value = 3};
    [Range(0f, 1f)]
    public FloatParameter band1Brightness = new FloatParameter() {value = 0.2f};
    [Range(0f, 1f)]
    public FloatParameter band2Brightness = new FloatParameter() {value = 0.4f};
    [Range(0f, 1f)]
    public FloatParameter band3Brightness = new FloatParameter() {value = 0.6f};
    [Range(0f, 1f)]
    public FloatParameter band4Brightness = new FloatParameter() {value = 0.8f};
    [Range(0f, 1f)]
    public FloatParameter band5Brightness = new FloatParameter() {value = 1.0f};
    [Range(0f,1f)]
    public FloatParameter edge1 = new FloatParameter() {value = 0.2f};
    [Range(0f,1f)]
    public FloatParameter edge2 = new FloatParameter() {value = 0.4f};
    [Range(0f,1f)]
    public FloatParameter edge3 = new FloatParameter() {value = 0.6f};
    [Range(0f,1f)]
    public FloatParameter edge4 = new FloatParameter() {value = 0.8f};

    private int lastSetSteps = 0;
    private void OnValidate()
    {
        if (lastSetSteps == steps)
        {
            return;
        }

        lastSetSteps = steps;
        float stepInterval = 1.0f / steps;
        float currentStep = stepInterval;
        for (int i = 0; i < steps; i++, currentStep += stepInterval)
        {
            switch (i)
            {
                case 0:
                    band1Brightness.value = currentStep;
                    edge1.value = currentStep;
                    continue;
                case 1:
                    band2Brightness.value = currentStep;
                    edge2.value = currentStep;
                    continue;
                case 2:
                    band3Brightness.value = currentStep;
                    edge3.value = currentStep;
                    continue;
                case 3:
                    band4Brightness.value = currentStep;
                    edge4.value = currentStep;
                    continue;
                case 4:
                    band5Brightness.value = currentStep;
                    continue;
            }
        }
    }
}

public sealed class ToonPostProcessingRenderer : PostProcessEffectRenderer<ToonPostProcessing>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Bernardo/ToonPostProcessing"));
        sheet.properties.SetFloat("_ShadowBrightness", settings.shadowBrightness);
        sheet.properties.SetFloat("_MaxBrightness", settings.maxBrightness);
        sheet.properties.SetFloat("_TransitionSmoothness", settings.transitionSmoothness);
        sheet.properties.SetFloat("_MaxShadedValue", settings.maxShadedValue);
        sheet.properties.SetInt("_NumberOfSteps", settings.steps);

        float[] bandBrightnessArray =
        {
            settings.band1Brightness,
            settings.band2Brightness,
            settings.band3Brightness,
            settings.band4Brightness,
            settings.band5Brightness,
        };

        float[] edgesArray =
        {
            settings.edge1,
            settings.edge2,
            settings.edge3,
            settings.edge4
        };

        sheet.properties.SetFloatArray("_BandBrightnessArray", bandBrightnessArray);
        sheet.properties.SetFloatArray("_EdgesArray", edgesArray);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}