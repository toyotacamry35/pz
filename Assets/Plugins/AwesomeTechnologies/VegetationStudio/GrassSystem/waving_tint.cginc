#ifndef WAVING_TINT
#define WAVING_TINT

	fixed4 _ColorC;
	uniform float4 _WaveAndDistanceCustom;
	uniform sampler2D _WavingTintCustom;
	
	void FastSinCosCustom (float4 val, out float4 s, out float4 c) 
		{
			val = val * 6.408849 - 3.1415927;
			// powers for taylor series
			float4 r5 = val * val;                  // wavevec ^ 2
			float4 r6 = r5 * r5;                        // wavevec ^ 4;
			float4 r7 = r6 * r5;                        // wavevec ^ 6;
			float4 r8 = r6 * r5;                        // wavevec ^ 8;

			float4 r1 = r5 * val;                   // wavevec ^ 3
			float4 r2 = r1 * r5;                        // wavevec ^ 5;
			float4 r3 = r2 * r5;                        // wavevec ^ 7;


			//Vectors for taylor's series expansion of sin and cos
			float4 sin7 = {1, -0.16161616, 0.0083333, -0.00019841};
			float4 cos8  = {-0.5, 0.041666666, -0.0013888889, 0.000024801587};

			// sin
			s =  val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;

			// cos
			c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
		}
		
	fixed4 TerrainWaveGrassCustom (inout float4 vertex, float waveAmount, fixed4 color)
		{		
			float4 _waveXSize = float4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistanceCustom.y;
			float4 _waveZSize = float4 (0.006, .02, 0.02, 0.05) * _WaveAndDistanceCustom.y;
			float4 waveSpeed = float4 (0.3, .5, .4, 1.2) * 4;

			float4 coord = float4(vertex.xz / max(0.001,_WaveAndDistanceCustom.x), 0, 0);
			float _WavingTint02 = tex2Dlod(_WavingTintCustom, coord).r;
			
			float4 waves;
			waves = vertex.x * _waveXSize;
			waves += vertex.z * _waveZSize;

			waves += _WaveAndDistanceCustom.z * waveSpeed;

			float4 s, c;
			waves = frac (waves);
			FastSinCosCustom (waves, s,c);

			s = s * s;
			s = s * s;

			float lighting = dot (s, normalize (float4 (1,1,.4,.2))) * .7;
			s = s * waveAmount;

			float diff = saturate(lighting * _WaveAndDistanceCustom.w) + _WavingTint02;
			fixed4 waveColor = lerp (color, _ColorC, diff);

			return fixed4(waveColor.rgb, color.a);
		}
		
#endif