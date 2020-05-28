sampler TextureSampler : register(s0);

float4 main(float2 coords : TEXCOORD0) : SV_TARGET
{
	float4 Color = tex2D(TextureSampler, coords);

	float m = .33333f * (Color.x + Color.y + Color.z);

	return float4(m,m,m, Color.w);
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 main();
	}
}