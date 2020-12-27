using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Toguchi.Rendering
{
    public class StarGlow : VolumeComponent, IPostProcessComponent
    {
        public FloatParameter Threshold = new FloatParameter(0.8f);

        public FloatParameter Intensity = new FloatParameter(0f);

        public FloatParameter Attenuation = new FloatParameter(0.95f);

        [Range(0f, 360f)]
        public FloatParameter Angle = new FloatParameter(0f);

        [Range(1, 16)]
        public IntParameter StreakCount = new IntParameter(4);

        [Range(1, 5)]
        public IntParameter Iteration = new IntParameter(5);

        [Range(1, 20)]
        public IntParameter Divide = new IntParameter(3);

        public bool IsActive() => Intensity.value > 0f;
        public bool IsTileCompatible() => false;
    }
}