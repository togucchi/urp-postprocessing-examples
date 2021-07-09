using System.Collections;
using System.Collections.Generic;
using Toguchi.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Toguchi.Rendering
{
    public class GradientFogPass : CustomPostProcessingPass<GradientFog>
    {
        private static readonly int RampTexId = UnityEngine.Shader.PropertyToID("_RampTex");
        private static readonly int IntensityId = UnityEngine.Shader.PropertyToID("_Intensity");
        
        protected override string RenderTag => "GradientFog";

        protected override void BeforeRender(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            Material.SetTexture(RampTexId, Component.gradientTexture.value);
            Material.SetFloat(IntensityId, Component.intensity.value);
        }

        protected override bool IsActive()
        {
            return Component.IsActive;
        }

        public GradientFogPass(RenderPassEvent renderPassEvent, Shader shader) : base(renderPassEvent, shader)
        {
        }
    }
}