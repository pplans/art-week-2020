﻿using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WaterDepthPassRenderFeature : ScriptableRendererFeature
{
	class CustomRenderPass : ScriptableRenderPass
	{

		private RenderTargetIdentifier Source { get; set; }
		private RenderTargetHandle Destination { get; set; }
		public Material mat = null;

		// This method is called before executing the render pass.
		// It can be used to configure render targets and their clear state. Also to create temporary render target textures.
		// When empty this render pass will render to the active camera render target.
		// You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
		// The render pipeline will ensure target setup and clearing happens in an performance manner.
		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			mat = new Material(Shader.Find("Water/Depth"));
		}

		// Here you can implement the rendering logic.
		// Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
		// https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
		// You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			CommandBuffer cmd = CommandBufferPool.Get();
			cmd.name = "Water Depth Prepass";

			int myLayerIndexVariable = LayerMask.NameToLayer("Water"); // ...or LayerMask.NameToLayer(myLayerName)
			int myLayerMaskVariable = 1 << myLayerIndexVariable;
			RenderQueueRange myRenderQueueRange = RenderQueueRange.transparent;
			FilteringSettings filters = new FilteringSettings(myRenderQueueRange, myLayerMaskVariable);
			DrawingSettings drawingSettings = new DrawingSettings();

			drawingSettings.overrideMaterial = mat;

			context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filters);

			// execution
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}

		public void Setup(RenderTargetIdentifier src, RenderTargetHandle dst)
		{
			Source = src;
			Destination = dst;
		}

		/// Cleanup any allocated resources that were created during the execution of this render pass.
		public override void FrameCleanup(CommandBuffer cmd)
		{
		}
	}

	CustomRenderPass m_ScriptablePass;

	public override void Create()
	{
		m_ScriptablePass = new CustomRenderPass();

		// Configures where the render pass should be injected.
		m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
	}

	// Here you can inject one or multiple render passes in the renderer.
	// This method is called when setting up the renderer once per-camera.
	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		m_ScriptablePass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget);
		renderer.EnqueuePass(m_ScriptablePass);
	}
}


