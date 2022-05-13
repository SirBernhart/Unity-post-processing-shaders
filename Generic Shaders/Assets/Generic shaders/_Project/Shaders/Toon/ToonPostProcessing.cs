using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ToonPostProcessingRenderer), PostProcessEvent.BeforeStack, "Bernardo/ToonPostProcessing")]
public sealed class ToonPostProcessing : PostProcessEffectSettings
{
    [Range(0f, 1f)]
    public FloatParameter shadowBrightness = new FloatParameter {value = 0.5f};
    [Range(0f, 1f)]
    public FloatParameter maxBrightness = new FloatParameter {value = 0.75f};
    [Range(0f, 1f)]
    public FloatParameter transitionSmoothness = new FloatParameter {value = 0.5f};
    [Range(0f, 1f)]
    public FloatParameter maxShadedValue = new FloatParameter {value = 0.75f};
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
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}