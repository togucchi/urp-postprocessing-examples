using UnityEngine;
using UnityEngine.Rendering;

namespace Toguchi.Rendering
{
    public class SimpleRaymarch : VolumeComponent
    {
        public IntParameter maxIterations = new IntParameter(64);
        public FloatParameter maxDistance = new FloatParameter(100f);
        public FloatParameter minDistance = new FloatParameter(0.01f);

        public Vector3Parameter spherePos = new Vector3Parameter(Vector3.zero);
        public FloatParameter sphereRadius = new FloatParameter(0f);

        public bool IsActive => sphereRadius.value > 0f;
    }
}