using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Toguchi.Rendering
{
    public abstract class CustomPostProcessingPass<T> : ScriptableRenderPass where T : VolumeComponent
    {
        private static readonly int TempColorBufferId = UnityEngine.Shader.PropertyToID("_TempColorBuffer");
        
        protected Shader Shader;
        protected Material Material;
        protected T Component;

        private RenderTargetIdentifier _renderTargetIdentifier;
        private RenderTargetIdentifier _tempRenderTargetIdentifier;
        
        protected abstract string RenderTag { get; }
        
        public CustomPostProcessingPass(RenderPassEvent renderPassEvent, Shader shader)
        {
            this.renderPassEvent = renderPassEvent;
            Shader = shader;

            if (shader == null)
            {
                return;
            }
            
            Material = CoreUtils.CreateEngineMaterial(shader);
        }
        
        public virtual void Setup(in RenderTargetIdentifier renderTargetIdentifier)
        {
            _renderTargetIdentifier = renderTargetIdentifier;
            _tempRenderTargetIdentifier = new RenderTargetIdentifier(TempColorBufferId);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (Material == null || !renderingData.cameraData.postProcessEnabled)
            {
                return;
            }

            var volumeStack = VolumeManager.instance.stack;
            Component = volumeStack.GetComponent<T>();
            if (Component == null)
            {
                return;
            }
            
            var commandBuffer = CommandBufferPool.Get(RenderTag);
            Render(commandBuffer, ref renderingData);
            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        public virtual void Render(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;

            var source = _renderTargetIdentifier;
            var dest = _tempRenderTargetIdentifier;
            
            var desc = new RenderTextureDescriptor(cameraData.camera.scaledPixelWidth, cameraData.camera.scaledPixelHeight);
            desc.colorFormat = cameraData.isHdrEnabled ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
            
            // RT確保
            commandBuffer.GetTemporaryRT(TempColorBufferId, desc);
            
            commandBuffer.Blit(source, dest);
            
            BeforeRender(commandBuffer);
            
            commandBuffer.Blit(dest, source, Material);
            
            // RT開放
            commandBuffer.ReleaseTemporaryRT(TempColorBufferId);
        }

        public abstract void BeforeRender(CommandBuffer commandBuffer);
    }
}