using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Toguchi.Rendering
{
    public class DepthLine : VolumeComponent, IPostProcessComponent
    {
        public FloatParameter bias = new FloatParameter(3f);

        public FloatParameter maxWeight = new FloatParameter(10f);

        public FloatParameter samplingScale = new FloatParameter(5f);

        public FloatParameter lineWidth = new FloatParameter(0f);
        
        public bool IsActive() => lineWidth.value > 0f;

        public bool IsTileCompatible() => false;
    }
}