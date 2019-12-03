sampler TextureSampler : register(s0);

uniform float2 u_Dimensions;
uniform float u_Radius;

float roundedRect( float2 p, float2 b, float r )
{
    return length(max(abs(p) - b + r, 0.0)) - r;
}

float4 main(float4 color : COLOR0, float2 uv : TEXCOORD0) : SV_TARGET
{
    float b = -roundedRect(uv * u_Dimensions - u_Dimensions * 0.5, u_Dimensions * 0.5, u_Radius);
   
	if (b < 0) return 0;
    else return tex2D(TextureSampler, uv) * color;
}


technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 main();
	}
}