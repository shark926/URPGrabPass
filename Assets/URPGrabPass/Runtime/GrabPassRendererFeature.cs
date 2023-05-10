using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace URPGrabPass.Runtime
{
    public class GrabPassRendererFeature : ScriptableRendererFeature
    {
        private const string DefaultShaderLightMode = "UseColorTexture";
        private const string DefaultGrabbedTextureName = "_GrabbedTexture";

        [SerializeField]
        [Tooltip("When to grab color texture.")]
        private RenderPassEvent _timing = RenderPassEvent.AfterRenderingTransparents;

        [SerializeField]
        [Tooltip("Texture name to use in the shader.")]
        private string _grabbedTextureName = DefaultGrabbedTextureName;

        private GrabColorTexturePass _grabColorTexturePass;

        public override void Create()
        {
            _grabColorTexturePass = new GrabColorTexturePass(_grabbedTextureName);
            _grabColorTexturePass.renderPassEvent = _timing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _grabColorTexturePass.BeforeEnqueue(renderer);
            renderer.EnqueuePass(_grabColorTexturePass);
        }

        public class GrabColorTexturePass : ScriptableRenderPass
        {
            private const string GrabbedTextureIdentifier = "_GrabbedTexture";

            private readonly RTHandle _grabbedTextureHandle;

            private readonly int _grabbedTexturePropertyId;

            private ScriptableRenderer _renderer;

            public GrabColorTexturePass(string grabbedTextureName)
            {

                //_grabbedTextureHandle = RTHandles.Alloc(grabbedTextureName);
                _grabbedTexturePropertyId = Shader.PropertyToID(grabbedTextureName);
            }

            public void BeforeEnqueue(ScriptableRenderer renderer)
            {
                _renderer = renderer;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                cmd.GetTemporaryRT(_grabbedTexturePropertyId, cameraTextureDescriptor);

                //cmd.SetGlobalTexture(_grabbedTexturePropertyId, _grabbedTextureHandle);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cmd = CommandBufferPool.Get(nameof(GrabColorTexturePass));
                cmd.Clear();
                Blit(cmd, _renderer.cameraColorTarget, _grabbedTexturePropertyId);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(_grabbedTexturePropertyId);
            }
        }
    }
}
