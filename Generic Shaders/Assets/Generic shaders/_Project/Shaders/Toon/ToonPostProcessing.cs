using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ToonPostProcessingRenderer), PostProcessEvent.BeforeStack, "Bernardo/ToonPostProcessing")]
public sealed class ToonPostProcessing : PostProcessEffectSettings
{
    [Range(0f, 1f)]
    public FloatParameter transitionSmoothness = new FloatParameter {value = 0.5f};
    [Range(2f, 7f)]
    public IntParameter steps = new IntParameter {value = 3};
    [Range(0f, 1f)]
    public FloatParameter step1Brightness = new FloatParameter() {value = 0.2f};
    [Range(0f, 1f)]
    public FloatParameter step2Brightness = new FloatParameter() {value = 0.4f};
    [Range(0f, 1f)]
    public FloatParameter step3Brightness = new FloatParameter() {value = 0.6f};
    [Range(0f, 1f)]
    public FloatParameter step4Brightness = new FloatParameter() {value = 0.8f};
    [Range(0f, 1f)]
    public FloatParameter step5Brightness = new FloatParameter() {value = 1.0f};
    [Range(0f, 1f)]
    public FloatParameter step6Brightness = new FloatParameter() {value = 1.0f};
    [Range(0f, 1f)]
    public FloatParameter step7Brightness = new FloatParameter() {value = 1.0f};
    [Range(0f,1f)]
    public FloatParameter edge1 = new FloatParameter() {value = 0.2f};
    [Range(0f,1f)]
    public FloatParameter edge2 = new FloatParameter() {value = 0.4f};
    [Range(0f,1f)]
    public FloatParameter edge3 = new FloatParameter() {value = 0.6f};
    [Range(0f,1f)]
    public FloatParameter edge4 = new FloatParameter() {value = 0.8f};
    [Range(0f,1f)]
    public FloatParameter edge5 = new FloatParameter() {value = 1f};
    [Range(0f,1f)]
    public FloatParameter edge6 = new FloatParameter() {value = 1f};
    [Range(0f,1f)]
    public FloatParameter edge7 = new FloatParameter() {value = 1f};

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
                    step1Brightness.value = currentStep;
                    edge1.value = currentStep;
                    continue;
                case 1:
                    step2Brightness.value = currentStep;
                    edge2.value = currentStep;
                    continue;
                case 2:
                    step3Brightness.value = currentStep;
                    edge3.value = currentStep;
                    continue;
                case 3:
                    step4Brightness.value = currentStep;
                    edge4.value = currentStep;
                    continue;
                case 4:
                    step5Brightness.value = currentStep;
                    edge5.value = currentStep;
                    continue;
                case 5:
                    step6Brightness.value = currentStep;
                    edge6.value = currentStep;
                    continue;
                case 6:
                    step7Brightness.value = currentStep;
                    edge7.value = currentStep;
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
        sheet.properties.SetFloat("_TransitionSmoothness", settings.transitionSmoothness);
        sheet.properties.SetInt("_NumberOfSteps", settings.steps);

        float[] bandBrightnessArray =
        {
            settings.step1Brightness,
            settings.step2Brightness,
            settings.step3Brightness,
            settings.step4Brightness,
            settings.step5Brightness,
            settings.step6Brightness,
            settings.step7Brightness
        };

        float[] edgesArray =
        {
            settings.edge1,
            settings.edge2,
            settings.edge3,
            settings.edge4,
            settings.edge5,
            settings.edge6,
            settings.edge7
        };

        sheet.properties.SetFloatArray("_BandBrightnessArray", bandBrightnessArray);
        sheet.properties.SetFloatArray("_EdgesArray", edgesArray);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}