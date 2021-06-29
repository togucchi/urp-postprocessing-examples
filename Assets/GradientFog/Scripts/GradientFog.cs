using System.Collections;
using System.Collections.Generic;
using Toguchi.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Toguchi.Rendering
{
    public class GradientFog : VolumeComponent
    {
        public TextureParameter gradientTexture = new TextureParameter(null);

        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

        public bool IsActive => intensity.value > 0f;
    }
}