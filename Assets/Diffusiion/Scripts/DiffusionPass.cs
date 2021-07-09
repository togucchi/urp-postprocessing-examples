using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Toguchi.Rendering
{
    public class DiffusionPass : CustomPostProcessingPass<Diffusion>
    {
        private static readonly int TempBlurBuffer1 = UnityEngine.Shader.PropertyToID("_TempBlurBuffer1");
        private static readonly int TempBlurBuffer2 = UnityEngine.Shader.PropertyToID("_TempBlurBuffer2");
        
        private static readonly int BlurTexId = UnityEngine.Shader.PropertyToID("_BlurTex");
        private static readonly int ContrastId = UnityEngine.Shader.PropertyToID("_Contrast");
        private static readonly int IntensityId = UnityEngine.Shader.PropertyToID("_Intensity");
        
        public DiffusionPass(RenderPassEvent renderPassEvent, Shader shader) : base(renderPassEvent, shader)
        {
        }

        protected override string RenderTag => "Diffusion";
        
        protected override void BeforeRender(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            Material.SetFloat(ContrastId, Component.contrast.value);
            Material.SetFloat(IntensityId, Component.intensity.value);
        }

        protected override void Render(CommandBuffer commandBuffer, ref RenderingData renderingData, RenderTargetIdentifier source,
            RenderTargetIdentifier dest)
        {
            ref var cameraData = ref renderingData.cameraData;
            commandBuffer.GetTemporaryRT(TempBlurBuffer1, cameraData.camera.scaledPixelWidth / 2, cameraData.camera.scaledPixelHeight / 2);
            commandBuffer.GetTemporaryRT(TempBlurBuffer2, cameraData.camera.scaledPixelWidth / 2, cameraData.camera.scaledPixelHeight / 2);

            commandBuffer.Blit(source, TempBlurBuffer1, Material, 0);
            commandBuffer.Blit(TempBlurBuffer1, TempBlurBuffer2, Material, 1);
            commandBuffer.Blit(TempBlurBuffer2, TempBlurBuffer1, Material, 2);
            
            commandBuffer.SetGlobalTexture(BlurTexId, TempBlurBuffer1);
            commandBuffer.Blit(source, dest, Material, 3);
            
            commandBuffer.ReleaseTemporaryRT(TempBlurBuffer1);
            commandBuffer.ReleaseTemporaryRT(TempBlurBuffer2);
        }

        protected override bool IsActive()
        {
            return Component.IsActive;
        }
    }
}