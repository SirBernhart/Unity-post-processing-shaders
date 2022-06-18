using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(HatchingPostProcessingRenderer), PostProcessEvent.BeforeStack, "Bernardo/HatchingPostProcessing")]
public sealed class HatchingPostProcessing : PostProcessEffectSettings
{
    [Range(0f, 1f)]
    public FloatParameter transitionSmoothness = new FloatParameter {value = 0.5f};
    [Range(2, 7)]
    public IntParameter steps = new IntParameter {value = 3};
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

    public TextureParameter hatching0 = new TextureParameter();
    public TextureParameter hatching1 = new TextureParameter();
    public TextureParameter hatching2 = new TextureParameter();
    public TextureParameter hatching3 = new TextureParameter();
    public TextureParameter hatching4 = new TextureParameter();
    public TextureParameter hatching5 = new TextureParameter();
    [Range(1f, 10f)]
    public FloatParameter hatchTilingFactor = new FloatParameter() {value = 1f};
    public ColorParameter baseColor = new ColorParameter() {value = Color.white};
    public BoolParameter useOriginalColor = new BoolParameter();

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
                    edge1.value = currentStep;
                    continue;
                case 1:
                    edge2.value = currentStep;
                    continue;
                case 2:
                    edge3.value = currentStep;
                    continue;
                case 3:
                    edge4.value = currentStep;
                    continue;
                case 4:
                    edge5.value = currentStep;
                    continue;
                case 5:
                    edge6.value = currentStep;
                    continue;
                case 6:
                    edge7.value = currentStep;
                    continue;
            }
        }
    }
}

public sealed class HatchingPostProcessingRenderer : PostProcessEffectRenderer<HatchingPostProcessing>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Bernardo/HatchingPostProcessing"));
        sheet.properties.SetFloat("_TransitionSmoothness", settings.transitionSmoothness);
        sheet.properties.SetInt("_NumberOfSteps", settings.steps);

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

        sheet.properties.SetFloatArray("_EdgesArray", edgesArray);
        sheet.properties.SetTexture("_HatchTex0", settings.hatching0);
        sheet.properties.SetTexture("_HatchTex1", settings.hatching1);
        sheet.properties.SetTexture("_HatchTex2", settings.hatching2);
        sheet.properties.SetTexture("_HatchTex3", settings.hatching3);
        sheet.properties.SetTexture("_HatchTex4", settings.hatching4);
        sheet.properties.SetTexture("_HatchTex5", settings.hatching5);
        sheet.properties.SetFloat("_HatchTilingFactor",settings.hatchTilingFactor);
        sheet.properties.SetColor("_BaseColor", settings.baseColor);
        sheet.properties.SetInt("_UseOriginalColor", settings.useOriginalColor ? 1 : 0);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}