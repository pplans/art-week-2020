Shader "Water/Lit"
{
    Properties
    {
		_NormTex("Normal", 2D) = "white" {}
		_Norm2Tex("Normal2", 2D) = "white" {}
		_TileNormTex("Tile N", Vector) = (1, 1, 0, 0)
		_LightDir("LightDir", Vector) = (0, 1, 0, 0)
		_LightColor("LightColor", Color) = (1, 1, 1, 1)
		_LowSeaColor("LowSeaColor", Color) = (1, 1, 1, 1)
		_HighSeaColor("HighSeaColor", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
				float4 data : TEXCOORD3;
            };

            sampler2D _NormTex;
			sampler2D _Norm2Tex;
            float4 _NormTex_ST;
			float4 _TileNormTex;
			float4 _LightDir;
			float4 _LightColor;
			float4 _LowSeaColor;
			float4 _HighSeaColor;

            v2f vert (appdata v)
            {
                v2f o;
				float3 NXYZ = normalize(float3(v.NXY.xy, v.NZ.x));
				o.data.x = dot(NXYZ, float3(0.0, 1.0, 0.0));
				o.wVertex = v.vertex;
				o.N = v.N;
                o.vertex = UnityObjectToClipPos(v.vertex);
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

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed3 Ndet = lerp(
								UnpackNormal(tex2D(_NormTex, i.uv*_TileNormTex.xy+float2(1., .2)*_Time.xx))
								, UnpackNormal(tex2D(_Norm2Tex, i.uv*_TileNormTex.xy - float2(1., .2)*_Time.xx))
								, 0.5);
				float3 N = normalize(i.N+ Ndet);
				float3 L = normalize(_LightDir);

				float4 col = float4(0, 0, 0, 1);
				float Ldot = max(0., dot(N, L));
				float3 V = normalize(i.vertex);
				float H = normalize(-V+L);
				float Sdot = max(0., dot(N, H));
				col.rgb = lerp(_LowSeaColor, _HighSeaColor, Remap01(i.wVertex.y, 0., 5.));

				if (i.data.x > 0.99)
				{
					float F = Fresnel(1., 1.07, Ldot);
					col = col+bdistrib(0.6, dot(N,H))*_LightColor;
				}

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
