using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WaterRenderFeature : ScriptableRendererFeature
{
	public GameObject m_waterGO = null;
	public Mesh m_waterMesh = null;
    class CustomRenderPass : ScriptableRenderPass
	{
		public GameObject m_go = null;
		public Material m_mat = null;
		public string CmdBufferName = "";
		public string ShaderName = "";
		public bool bDepth = false;
		public Mesh m_mesh = null;
		public RenderQueueRange renderQueueRange = RenderQueueRange.all;

		// This method is called before executing the render pass.
		// It can be used to configure render targets and their clear state. Also to create temporary render target textures.
		// When empty this render pass will render to the active camera render target.
		// You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
		// The render pipeline will ensure target setup and clearing happens in an performance manner.
		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			/*ConfigureTarget(Shader.PropertyToID(bDepth ? "_CameraDepthTexture" : "_CameraColorTexture"));
			if (bDepth)
				ConfigureTarget(-1, Shader.PropertyToID("_CameraDepthTexture"));
			else
				ConfigureTarget(Shader.PropertyToID("_CameraColorTexture"), -1);*/
		}

		public void SetMaterial(Material mat)
		{
			m_mat = mat;
		}

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (m_mesh == null)
				return;
			if(m_mat==null)
			{
				m_mat = new Material(Shader.Find(ShaderName));
			}
			CommandBuffer cmd = CommandBufferPool.Get();
			cmd.name = CmdBufferName;

			//int myLayerIndexVariable = LayerMask.NameToLayer("Water"); // ...or LayerMask.NameToLayer(myLayerName)
			//int myLayerMaskVariable = 1 << myLayerIndexVariable;
			//FilteringSettings filters = new FilteringSettings(renderQueueRange, myLayerMaskVariable);
			//DrawingSettings drawingSettings = new DrawingSettings();
			//
			//drawingSettings.overrideMaterial =m_mat;
			//drawingSettings.overrideMaterialPassIndex = 0;
			//
			//RenderStateBlock stateBlock = new RenderStateBlock(RenderStateMask.Depth);
			//stateBlock.depthState = new DepthState(bDepth, bDepth ? CompareFunction.LessEqual:CompareFunction.Disabled);
			//
			//stateBlock.rasterState = new RasterState(CullMode.Back);
			//stateBlock.mask = new RenderStateMask();
			RenderTexture tempRT = null;
			context.SetupCameraProperties(Camera.current==null?Camera.main:Camera.current);
			int idRT = 0;
			if (bDepth)
			{
				idRT = Shader.PropertyToID("_CameraDepthTexture");
				cmd.SetRenderTarget(idRT);
			}
			else
			{
				tempRT = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB2101010);
				cmd.SetRenderTarget(tempRT);
			}

			cmd.DrawMesh(m_mesh, m_go.transform.localToWorldMatrix, m_mat);
			if (!bDepth)
			{
				cmd.Blit(tempRT, Shader.PropertyToID("_CameraColorTexture"));
			}

			//context.DrawRenderers(
			//	renderingData.cullResults,
			//	ref drawingSettings,
			//	ref filters,
			//	ref stateBlock);

			// execution
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
			cmd.SetRenderTarget(Shader.PropertyToID("_CameraColorTexture"));
			if(tempRT!=null)
				tempRT.Release();
		}

		/// Cleanup any allocated resources that were created during the execution of this render pass.
		public override void FrameCleanup(CommandBuffer cmd)
		{
			//cmd.SetRenderTarget(Shader.PropertyToID("_CameraColorTexture"));
		}
    }

    CustomRenderPass m_DepthPass;
	CustomRenderPass m_LightingPass;

	public override void Create()
    {
		if (m_waterGO == null)
		{
			m_waterGO = GameObject.FindGameObjectWithTag("Water");
		}
		if(m_waterMesh==null)
		{
			m_waterMesh = m_waterGO.GetComponent<MeshFilter>().sharedMesh;
			if (m_waterMesh == null)
				return;
		}
		m_DepthPass = new CustomRenderPass();
		// Configures where the render pass should be injected.
		m_DepthPass.CmdBufferName = "Water Depth Prepass";
		m_DepthPass.ShaderName = "Water/Depth";
		m_DepthPass.renderQueueRange = RenderQueueRange.opaque;
		m_DepthPass.bDepth = true;
		m_DepthPass.m_mesh = m_waterMesh;
		m_DepthPass.m_go = m_waterGO;
		m_DepthPass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;

		m_LightingPass = new CustomRenderPass();
		// Configures where the render pass should be injected.
		m_LightingPass.CmdBufferName = "Water Lighting";
		m_LightingPass.ShaderName = "Water/Lit";
		m_LightingPass.renderQueueRange = RenderQueueRange.transparent;
		m_LightingPass.bDepth = false;
		m_LightingPass.m_mesh = m_waterMesh;
		m_LightingPass.m_go = m_waterGO;
		m_LightingPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
		m_LightingPass.SetMaterial(m_waterGO.GetComponent<MeshRenderer>().material);
	}

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		renderer.EnqueuePass(m_DepthPass);
		renderer.EnqueuePass(m_LightingPass);
	}
}


