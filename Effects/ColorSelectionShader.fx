float3 HSV2RGB(float3 HSV)
{
	float3 RGB = HSV.z;

	float h = (HSV.x) / 60.0;
	float s = HSV.y;
	float v = HSV.z;

	float i = floor(h);
	float f = h - i;

	float p = (1.0 - s);
	float q = (1.0 - s * f);
	float t = (1.0 - s * (1 - f));

	if (i == 0) { RGB = float3(1, t, p); }
	else if (i == 1) { RGB = float3(q, 1, p); }
	else if (i == 2) { RGB = float3(p, 1, t); }
	else if (i == 3) { RGB = float3(p, q, 1); }
	else if (i == 4) { RGB = float3(t, p, 1); }
	else { RGB = float3(1, p, q); }

	RGB *= v;

	return RGB;
}

float4 main(float2 coords : TEXCOORD0) : SV_TARGET
{
	if (coords.y < 0.5) return float4(HSV2RGB(float3(coords.x * 360.0, coords.y * 2.0, 1.0)), 1.0);
	else return float4(HSV2RGB(float3(coords.x * 360.0, 1.0, 1.0 - (coords.y * 2 - 1.0))), 1.0);
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 main();
	}
}