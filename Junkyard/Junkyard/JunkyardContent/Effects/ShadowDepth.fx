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

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 ScreenPos : TEXCOORD0;
	float2 UV : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.ScreenPos = output.Position;
	output.UV = input.UV;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 texCol = tex2D(TextureSampler, input.UV);
	clip(texCol.a - AlphaMinValue);

	float depth = clamp(input.ScreenPos.z / input.ScreenPos.w, 0, 1);
    return float4(depth, 0, 0, 1);    
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
