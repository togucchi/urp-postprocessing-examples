using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Toguchi.Rendering
{
    public class SimpleRaymarchPass : CustomPostProcessingPass<SimpleRaymarch>
    {
        public SimpleRaymarchPass(RenderPassEvent renderPassEvent, Shader shader) : base(renderPassEvent, shader)
        {
        }

        protected override string RenderTag => "SimpleRaymarch";
        
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
            
            Material.SetVector("_SpherePos", Component.spherePos.value);
            Material.SetFloat("_SphereRadius", Component.sphereRadius.value);
        }
        
        // http://hventura.com/unity-post-process-v2-raymarching.html
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