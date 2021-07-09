using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Toguchi.Rendering
{
    public class LightShaftPass : CustomPostProcessingPass<LightShaft>
    {
        public LightShaftPass(RenderPassEvent renderPassEvent, Shader shader) : base(renderPassEvent, shader)
        {
        }

        private static readonly int LightShaftTempId = UnityEngine.Shader.PropertyToID("_LightShaftTempTex");

        protected override string RenderTag => "LightShaft";
        
        protected override void BeforeRender(CommandBuffer commandBuffer, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;
            var camera = cameraData.camera;
            
            Material.SetMatrix("_CamFrustum", FrustumCorners(camera));
            Material.SetMatrix("_CamToWorld", camera.cameraToWorldMatrix);
            Material.SetVector("_CamWorldSpace", camera.transform.position);
            Material.SetInt("_MaxIterations", Component.maxIterations.value);
            Material.SetFloat("_MaxDistance", Component.maxDistance.value);
            Material.SetFloat("_MinDistance", Component.minDistance.value);
            Material.SetFloat("_Intensity", Component.intensity.value);
        }
        
        protected override void Render(CommandBuffer commandBuffer, ref RenderingData renderingData, RenderTargetIdentifier source,
            RenderTargetIdentifier dest)
        {
            ref var cameraData = ref renderingData.cameraData;
            
            commandBuffer.GetTemporaryRT(LightShaftTempId,cameraData.camera.scaledPixelWidth / 4, cameraData.camera.scaledPixelHeight / 4);
            
            // LightShaft生成
            commandBuffer.Blit(null, LightShaftTempId, Material, 0);
            commandBuffer.SetGlobalTexture(LightShaftTempId, new RenderTargetIdentifier(LightShaftTempId));
            commandBuffer.Blit(source, dest, Material, 1);
            
            commandBuffer.ReleaseTemporaryRT(LightShaftTempId);
        }

        // 参考: http://hventura.com/unity-post-process-v2-raymarching.html
        private Matrix4x4 FrustumCorners(Camera cam)
        {
            Transform camtr = cam.transform;

            Vector3[] frustumCorners = new Vector3[4];
            cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1),
                cam.farClipPlane, cam.stereoActiveEye, frustumCorners);

            Matrix4x4 frustumVectorsArray = Matrix4x4.identity;
            
            frustumVectorsArray.SetRow(0,  camtr.TransformVector(frustumCorners[0]));
            frustumVectorsArray.SetRow(1, camtr.TransformVector(frustumCorners[3]));
            frustumVectorsArray.SetRow(2, camtr.TransformVector(frustumCorners[1]));
            frustumVectorsArray.SetRow(3, camtr.TransformVector(frustumCorners[2]));
            
            return frustumVectorsArray;
        }

        protected override bool IsActive()
        {
            return Component.IsActive;
        }
    }
}