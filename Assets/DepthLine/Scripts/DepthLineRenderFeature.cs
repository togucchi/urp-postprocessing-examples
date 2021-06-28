using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Toguchi.Rendering
{
    public class DepthLineRenderFeature : ScriptableRendererFeature
    {
        private DepthLinePass _depthLinePass;


        public override void Create()
        {
            _depthLinePass = new DepthLinePass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _depthLinePass.SetupPass(renderer.cameraColorTarget);
            renderer.EnqueuePass(_depthLinePass);
        }
    }
}