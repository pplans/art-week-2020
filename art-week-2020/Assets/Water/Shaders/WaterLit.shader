Shader "Water/Lit"
{
    Properties
    {
		_NormTex("Normal", 2D) = "white" {}
		_Norm2Tex("Normal2", 2D) = "white" {}
		_SandTex("Sand", 2D) = "white" {}
		_TileNormTex("Tile N", Vector) = (1, 1, 0, 0)
		_LightDir("LightDir", Vector) = (0, 1, 0, 0)
		_LightColor("LightColor", Color) = (1, 1, 1, 1)
		_LowSeaColor("LowSeaColor", Color) = (1, 1, 1, 1)
		_HighSeaColor("HighSeaColor", Color) = (1, 1, 1, 1)
		_WaterFadeDistance("Fade Distance", Range(0.1, 25.)) = 10.
		_WaterSandDistance("Sand Distance", Range(0.1, 25.)) = 10.
		_Wind("Wind", Vector) = (1, 1, 1, 1)
		_Offset("Offset", Vector) = (1, 1, 1, 1)
    }
    SubShader
    {
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 N : NORMAL;
                float2 uv : TEXCOORD0;
				float2 NXY : TEXCOORD1;
				float2 NZ : TEXCOORD2;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
				float3 N : NORMAL;
                float4 vertex : SV_POSITION;
				float4 wVertex : TEXCOORD1;
				float4 data : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
            };

			sampler2D _CameraDepthTexture;
			sampler2D _CameraOpaqueTexture;
            sampler2D _NormTex;
			sampler2D _Norm2Tex;
			sampler2D _SandTex;
            float4 _NormTex_ST;
			float4 _TileNormTex;
			float4 _LightDir;
			float4 _LightColor;
			float4 _LowSeaColor;
			float4 _HighSeaColor;
			float4 _Wind;
			float4 _Offset;
			float _WaterFadeDistance;
			float _WaterSandDistance;

            v2f vert (appdata v)
            {
                v2f o = (v2f)0;
				float3 NXYZ = normalize(float3(v.NXY.xy, v.NZ.x));
				o.data.x = dot(NXYZ, float3(0.0, 1.0, 0.0));
				o.wVertex = v.vertex;
				o.N = v.N;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _NormTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float Remap01(float a, float min, float max)
			{
				return saturate((a-min)/(max-min));
			}

			float Fresnel(float n0, float n1, float cosTeta)
			{
				float R0 = (n1-n0)/(n1+n0); R0 *= R0;
				float Omc = (1. - cosTeta);
				Omc *= Omc; Omc*= Omc; Omc*= (1.-cosTeta);
				return R0+(1.-R0)*Omc;
			}

			float bdistrib(float r, float NdotH)
			{
				float NdotH2 = NdotH * NdotH;
				float r2 = r * r;
				float r1 = 1.0 / max(0.0001, 4.0 * r2 * NdotH2*NdotH2);
				r2 = (NdotH2 - 1.0) / (r2 * NdotH2);
				return r1 * exp(r2);
			}
			
			float _LinearEyeDepth(float rawdepth, float _NearClip, float _FarClip)
			{
				float x, y, z, w;
#if SHADER_API_GLES3 // insted of UNITY_REVERSED_Z
				x = -1.0 + _NearClip / FarClip;
				y = 1;
				z = x / _NearClip;
				w = 1 / _NearClip;
#else
				x = 1.0 - _NearClip / _FarClip;
				y = _NearClip / _FarClip;
				z = x / _NearClip;
				w = y / _NearClip;
#endif

				return 1.-1.0 / (z * rawdepth + w);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				// sample depth
				float2 screenUV = i.screenPos.xy / i.screenPos.w;
				float oDepth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, screenUV));
				float vz = abs(UnityObjectToViewPos(i.wVertex).z);
				float sceneAlpha = 1.-min(_WaterFadeDistance, LinearEyeDepth(oDepth) - vz) / _WaterFadeDistance;
				float sandAlpha = 1. - min(_WaterFadeDistance, _WaterSandDistance) / _WaterFadeDistance;

                fixed3 Ndet = lerp(
								UnpackNormal(tex2D(_NormTex, (i.uv + _Offset.xz)*_TileNormTex.xy +_Wind.xy*_Time.xx))
								, UnpackNormal(tex2D(_Norm2Tex, (i.uv + _Offset.xz)*_TileNormTex.xy - _Wind.xy*_Time.xx))
								, 0.5);
				float3 N = normalize(i.N*float3(10., 1., 10.)+ Ndet);
				float3 L = normalize(_LightDir);

				float2 disp = 0.1*Ndet.xz;

				float3 sand = tex2D(_SandTex, (i.uv + _Offset.xz)*10. + disp).rgb;

				float4 col = float4(sand, 0.);
				float Ldot = max(0., dot(N, L));
				float3 V = normalize(i.vertex);
				float H = normalize(-V+L);
				float Sdot = max(0., dot(N, H));
				float F = Fresnel(1., 1.07, Ldot);
				sand = lerp(sand, float3(0., 0., 0.), smoothstep(0., 1., saturate((1. - LinearEyeDepth(oDepth)*0.01))));

				col.rgb =
					lerp(
						lerp(_LowSeaColor, _HighSeaColor, Remap01(i.wVertex.y, 0., 5.))
						, sand//*Ldot
						, sandAlpha*F//saturate(1.-(i.wVertex.y))*.25
					);
				// disabled, bad flow on cameras no time
				#if 0
				col.rgb = lerp(
							col.rgb
							, tex2D(_CameraOpaqueTexture, screenUV+ 0.2*disp).rgb
							, sceneAlpha);
				#endif

				if (i.data.x > 0.99)
				{
					col = col+bdistrib(0.6, dot(N,H))*_LightColor;
				}

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
				//col.r = col.g = col.b = sandDepth;
				//col.rg = i.uv;
                return col;
            }
            ENDCG
        }
    }
}
