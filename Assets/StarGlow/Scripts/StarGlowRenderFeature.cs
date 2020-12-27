using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Toguchi.Rendering
{
    public class StarGlowRenderFeature : ScriptableRendererFeature
    {
        private StarGlowPass starGlowPass;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            starGlowPass.SetupPass(renderer.cameraColorTarget);
            renderer.EnqueuePass(starGlowPass);
        }

        public override void Create()
        {
            starGlowPass = new StarGlowPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }
    }
}