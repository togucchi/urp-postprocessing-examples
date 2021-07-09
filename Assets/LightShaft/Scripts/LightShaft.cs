using UnityEngine.Rendering;

namespace Toguchi.Rendering
{
    public class LightShaft : VolumeComponent
    {
        public IntParameter maxIterations = new IntParameter(64);
        public FloatParameter maxDistance = new FloatParameter(12f);
        public FloatParameter minDistance = new FloatParameter(0.4f);

        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        public bool IsActive => intensity.value > 0f;
    }
}