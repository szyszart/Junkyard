float4x4 World;
float4x4 View;
float4x4 Projection;

texture2D Texture;
sampler2D TextureSampler = sampler_state
{
	texture = <Texture>;
	addressU = wrap;
	addressV = wrap;
	minfilter = point;
	magfilter = point;
	mipfilter = point;
};
float AlphaMinValue = 0.7;

bool NormalMapEnabled = false;
texture2D NormalMap;
	sampler2D normalSampler = sampler_state {
	texture = <NormalMap>;
	minfilter = point;
	magfilter = point;
	mipfilter = point;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 Depth : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float2 UV : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float4x4 viewProjection = mul(View, Projection);
	float4x4 worldViewProjection = mul(World, viewProjection);
	output.Position = mul(input.Position, worldViewProjection);
	output.Normal = mul(input.Normal, World);
	output.Depth.xy = output.Position.zw;
	output.UV = input.UV;
	return output;
}

struct PixelShaderOutput
{
	float4 Normal : COLOR0;
	float4 Depth : COLOR1;
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	float4 texCol = tex2D(TextureSampler, input.UV);
	clip(texCol.a - AlphaMinValue);

	PixelShaderOutput output;

	if (NormalMapEnabled) 
		output.Normal = float4(tex2D(normalSampler, input.UV).rgb, 1);
	else 
		output.Normal = float4((normalize(input.Normal).xyz / 2) + .5, 1);

	output.Depth = input.Depth.x / input.Depth.y;
	output.Depth.a = 1;

	return output;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
