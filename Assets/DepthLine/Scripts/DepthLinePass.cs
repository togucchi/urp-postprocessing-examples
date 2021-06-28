using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Toguchi.Rendering
{
    public class DepthLinePass : ScriptableRenderPass
    {
        private const string RenderTag = "DepthLine";
        private const string ShaderName = "Hidden/Toguchi/PostProcessing/DepthLine";
        
        private DepthLine _depthLine;
        private Material _depthLineMaterial;
        private RenderTargetIdentifier _currentRenderTarget;

        private bool _isCreatedMaterial = false;

        public DepthLinePass(RenderPassEvent renderPassEvent)
        {
            this.renderPassEvent = renderPassEvent;

            var shader = Shader.Find(ShaderName);
            if(shader == null)
            {
                Debug.LogError($"Shader = {ShaderName} が存在しません");
                return;
            }

            _depthLineMaterial = CoreUtils.CreateEngineMaterial(shader);
            _isCreatedMaterial = true;
        }
        
        public void SetupPass(in RenderTargetIdentifier currentRenderTarget)
        {
            this._currentRenderTarget = currentRenderTarget;
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!_isCreatedMaterial || !renderingData.cameraData.postProcessEnabled)
            {
                return;
            }

            var volumeStack = VolumeManager.instance.stack;
            _depthLine = volumeStack.GetComponent<DepthLine>();
            if (_depthLine == null || !_depthLine.IsActive())
            {
                return;
            }

            var commandBuffer = CommandBufferPool.Get(RenderTag);
            Render(commandBuffer, ref renderingData);
            context.ExecuteCommandBuffer(commandBuffer);
            CommandBufferPool.Release(commandBuffer);
        }

        private void Render(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;

            var source = _currentRenderTarget;
            var dest = _currentRenderTarget;
            
            commandBuffer.SetGlobalFloat("_DepthLineBias", _depthLine.bias.value);
            commandBuffer.SetGlobalFloat("_DepthLineWidth", _depthLine.lineWidth.value);
            commandBuffer.SetGlobalFloat("_DepthLineSamplingRate", _depthLine.samplingScale.value);
            commandBuffer.SetGlobalFloat("_DepthLineMaxWeight", _depthLine.maxWeight.value);
            
            commandBuffer.Blit(source, dest, _depthLineMaterial);
        }
    }
}