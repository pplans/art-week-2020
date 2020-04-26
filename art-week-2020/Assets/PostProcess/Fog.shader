Shader "PostProcessing/Fog"
{
    Properties
    {
		_LightPos("Light Position", Vector) = (0, 20, 0, 0)
    }
    SubShader
    {
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 100
		Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency

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
				float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD3;
				float4 screenPos : TEXCOORD1;
				float2 uv : TEXCOORD0;
            };

			sampler2D _CameraDepthTexture;
			sampler2D _CameraOpaqueTexture;
			float4 _LightPos;
			float4 _CameraDepthTexture_ST;

            v2f vert (appdata v)
            {
                v2f o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _CameraDepthTexture);
                return o;
            }

			float phaseHenyeyGreenstein(float g, float cosTeta)
			{
				return (1. - g * g) / (4.*3.14*pow(1. + g * g - 2.*g*cosTeta, 1.5))
					//+g*(1.+cosTeta)*.5 // from http://www.csroc.org.tw/journal/JOC25-3/JOC25-3-2.pdf
					;
			}

			float BeersPowder(float den, float dist, float extinction)
			{
				//return exp(-den*dist*extinction);
				float b = exp(-den * dist*extinction);
				float p = 2.*(1. - exp(-den * dist*extinction*2.));
				return b;//*p;
			}

			float Remap01(float x, float _oMin, float _oMax)
			{
				return (max(0., x - _oMin) / (_oMax - _oMin));
			}

			float3 RotXV3(in float3 P, float A)
			{
				float cA = cos(A), sA = sin(A);
				return mul(float3x3(1.0, 0.0, 0.0, 0.0, cA, -sA, 0.0, sA, cA), P);
			}

			float hash3D(float3 x)
			{
				x = RotXV3(x, x.y);
				float h = dot(x, float3(42.69, 51.42, 34.405));
				return frac(abs(sin(h))*50403.43434);
			}

			float smoothNoise(float3 x) {
				float3 p = floor(x);
				float3 n = frac(x);
				float3 f = n * n*(3.0 - 2.0*n);
				float winy = 157.0;
				float winz = 113.0;

				float wx = p.x + winy * p.y + winz * p.z;
				return lerp(
					lerp(
						lerp(hash3D(p + float3(0., 0., 0.)), hash3D(p + float3(1., 0., 0.)), f.x),
						lerp(hash3D(p + float3(0., 1., 0.)), hash3D(p + float3(1., 1., 0.)), f.x),
						f.y),
					lerp(
						lerp(hash3D(p + float3(0., 0., 1.)), hash3D(p + float3(1., 0., 1.)), f.x),
						lerp(hash3D(p + float3(0., 1., 1.)), hash3D(p + float3(1., 1., 1.)), f.x),
						f.y)
					, f.z);
			}

			float fbm3D(float3 p)
			{
				float total = 0.0;
				total = 0.5000* smoothNoise(p); p = p * 2.0;
				total += 0.2500* smoothNoise(p); p = p * 2.0;
				total += 0.1250* smoothNoise(p); p = p * 2.0;
				return total;
			}

			float heightSignal(float a, float3 P, float h)
			{
				return saturate(length(P.xz*0.01)*(P.y - a)*(P.y - a - h)*(-4. / (h*h)));
				//return (P.y - a)*(P.y - a - h)*(-4. / (h*h));
			}


			float2 sampleDensity(float3 P)
			{
				P.xz += _Time.x;
				// clouds shape based on height
				float H = heightSignal(.1, P, 8.);
				float s = 1.0;
				float c = 1.;
				{
					//c -= 1.*c*fbm3D(.00016*P);
					//c = clamp(c, 0., 1.);
				}
				c = fbm3D(20.*P+_Time.x);
				return float2(clamp(s*c*H, 0., 1.), H);
			}

			float4 fog(float3 ro, float3 rd, float tmin, float tmax)
			{
				float4 TrAndRadiance = 1.;
				float dist = tmin;
				float st = (tmax - tmin)/16.;
				return float4(ro+rd*tmax, 0.);
				for (int i = 0; i < 16; ++i)
				{
					float3 P = ro + rd * dist;
					float HP = P.y;
					float3 Ld = normalize(_LightPos.xyz - P);
					float2 dh = sampleDensity(P);
					float ambient = 0.001;
					st *= (1.-dh.x);
					if (dh.x > 0.0)
					{
						float mu = dot(rd, Ld);
						//d = 1.;
						float phase = 0.;
						{
							float cosAngle = 0., g = 0.;
							cosAngle = mu; g = lerp(-.20, .55, 0.5*(1. + cosAngle));
							phase = phaseHenyeyGreenstein(g, cosAngle);
						}
						float transmittance = BeersPowder(dh.x, st, 1.);
						// luminance scattered to this point from the sun
						float sumLuminance = 0.;
						float stL = 80 / 8.;
						float distL = stL; // introduce rand
						float accD = 0.;
						float accL = 0.;

						float2 lDL = dh;
						for (int j = 0; j < 8; ++j)
						{
							float3 Pl = P + Ld * distL;

							float2 dhL = sampleDensity(Pl);
							accD += max(0., dhL.x);
							accL += accD > 0. ? stL : 0.;
							distL += stL;
						}
						// https://www.shadertoy.com/view/4dSBDt accumulative beerLaw
						float beersLaw = BeersPowder(accD, accL, 1.);
						sumLuminance = clamp(phase * beersLaw, 0., 1.);

						float radiance = (ambient + sumLuminance)*dh.x;
						float extinction = 1. * max(0.0, dh.x);
						TrAndRadiance.y += TrAndRadiance.x * (radiance - transmittance * radiance) / dh.x;
						TrAndRadiance.x *= transmittance;
						if (TrAndRadiance.x <= 0.05) break;
					}
					dist += st;
				}
				return TrAndRadiance;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				// sample depth
				float2 screenUV = i.screenPos.xy / i.screenPos.w;
				float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, screenUV));
				depth = LinearEyeDepth(depth);
				float4 sceneColor = tex2D(_CameraOpaqueTexture, screenUV);

				fixed4 col = 0;
				//col.r = oDepth;
				//col.g = col.b = col.a = 1.;
				col.a = 0.5;

				float3 viewDir = i.worldPos - _WorldSpaceCameraPos;
				// so far so good
				//if(depth>0. && (_WorldSpaceCameraPos+ normalize(viewDir)*6.).y>0.)
				if(false)
				{
					float4 TrAndRadiance = fog(_WorldSpaceCameraPos, normalize(viewDir), 0.01, depth);
					col.a = TrAndRadiance.x;
					col.r = col.g = col.b = TrAndRadiance.y;
					//col.rgb = abs(TrAndRadiance.xyz/depth);
				}
				// col.r = col.b = col.g = depth*0.001;
				// col.a = 1.;
				// col.a = 1.;
				// col.r = col.g = col.b = depth*0.0001;
				//return float4(normalize(viewDir.xyz), 1.);
				// col.r = col.g = col.b = fbm3D(20.*i.worldPos);
				// col.rgb = normalize(viewDir);
                return col;
            }
            ENDCG
        }
    }
}
