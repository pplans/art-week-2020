Shader "Water/Depth"
{
	Properties{ }
	SubShader
	{
		Tags {"RenderType" = "Opaque" "IgnoreProjector" = "True" "Queue" = "Transparent" "RenderPipeline" = "UniversalPipeline"}
		Pass
		{
			Name "DepthOnly"
			Tags{"LightMode" = "DepthOnly"}
			ZWrite On
			ColorMask 0

			HLSLPROGRAM

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0

			#pragma vertex DepthOnlyVertex
			#pragma fragment DepthOnlyFragment

			#include "Packages/com.unity.render-pipelines.universal/Shaders/UnlitInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"

			ENDHLSL
		}
	}
	FallBack "Hidden/InternalErrorShader"
}
