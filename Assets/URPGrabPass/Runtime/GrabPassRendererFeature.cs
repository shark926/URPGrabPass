using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace URPGrabPass.Runtime
{
    /// <summary>
    ///     Renderer feature to grab color texture and render objects that use it.
    /// </summary>
    [Serializable]
    public class GrabPassRendererFeature : ScriptableRendererFeature
    {
        private const string DefaultShaderLightMode = "UseColorTexture";
        private const string DefaultGrabbedTextureName = "_GrabbedTexture";

        [SerializeField] [Tooltip("When to grab color texture.")]
        private GrabTiming _timing = GrabTiming.AfterTransparents;

        [SerializeField] [Tooltip("Texture name to use in the shader.")]
        private string _grabbedTextureName = DefaultGrabbedTextureName;

        private GrabColorTexturePass _grabColorTexturePass;


        public override void Create()
        {
            _grabColorTexturePass = new GrabColorTexturePass(_timing, _grabbedTextureName);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _grabColorTexturePass.BeforeEnqueue(renderer);
            renderer.EnqueuePass(_grabColorTexturePass);
        }
    }
}
