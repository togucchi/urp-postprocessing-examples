using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Toguchi.Rendering
{
    public class DepthLineRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            public Shader shader;
        }
        
        public Settings settings = new Settings();

        private DepthLinePass _depthLinePass;


        public override void Create()
        {
            this.name = "DepthLine";
            _depthLinePass = new DepthLinePass(settings.renderPassEvent, settings.shader);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _depthLinePass.Setup(renderer.cameraColorTarget);
            renderer.EnqueuePass(_depthLinePass);
        }
    }
}