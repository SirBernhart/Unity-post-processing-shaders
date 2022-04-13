using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ToonPostProcessingRenderer), PostProcessEvent.BeforeStack, "Bernardo/ToonPostProcessing")]
public sealed class ToonPostProcessing : PostProcessEffectSettings
{
    [Range(1f, 6f)]
    public FloatParameter posterizeSteps = new FloatParameter {value = 0.5f};
    [Range(0f, 1f)]
    public FloatParameter toonSmoothness = new FloatParameter {value = 0.5f};
    [Range(0f, 1f)]
    public FloatParameter toonOffset = new FloatParameter {value = 0.5f};

    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        return enabled.value
               && posterizeSteps.value > 0f;
    }
}

public sealed class ToonPostProcessingRenderer : PostProcessEffectRenderer<ToonPostProcessing>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Bernardo/ToonPostProcessing"));
        sheet.properties.SetFloat("_PosterizeSteps", settings.posterizeSteps);
        sheet.properties.SetFloat("_ToonSmoothness", settings.toonSmoothness);
        sheet.properties.SetFloat("_ToonOffset", settings.toonOffset);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}