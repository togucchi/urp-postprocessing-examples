using System.Collections;
using System.Collections.Generic;
using Toguchi.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GradientFogPass : CustomPostProcessingPass<GradientFog>
{
    protected override string RenderTag => "GradientFog";

    public override void BeforeRender(CommandBuffer commandBuffer)
    {
        
    }

    public GradientFogPass(RenderPassEvent renderPassEvent, Shader shader) : base(renderPassEvent, shader)
    {
    }
}