#include "Shared.vsi"

float4x4 World;
float4x4 View;
float4x4 Projection;

float4x4 ShadowView;
float4x4 ShadowProjection;
float3 ShadowLightPosition;
float3 ShadowLightDirection;
float ShadowMult = 0.3f;
float ShadowBias = 0.00001f;
float AlphaMinValue = 0.7;

texture2D ShadowMap;
sampler2D ShadowSampler = sampler_state {
	texture = <ShadowMap>;
	minfilter = point;
	magfilter = point;
	mipfilter = point;
};

texture2D Texture;
sampler2D TextureSampler = sampler_state
{
	texture = <Texture>;
	addressU = wrap;
	addressV = wrap;
	minfilter = anisotropic;
	magfilter = anisotropic;
	mipfilter = point;
};

texture2D LightTexture;
sampler2D LightSampler = sampler_state
{
	texture = <LightTexture>;
	minfilter = point;
	magfilter = point;
	mipfilter = point;
};
float3 AmbientColor;
float3 DiffuseColor;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 ShadowScreenPosition : TEXCOORD0;
	float4 ScreenPosition : TEXCOORD1;
	float2 UV : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.ScreenPosition = output.Position;
	output.ShadowScreenPosition = mul(worldPosition, mul(ShadowView, ShadowProjection));
	output.UV = input.UV;
    return output;
}

float sampleShadowMap(float2 UV)
{
	if (UV.x < 0 || UV.x > 1 || UV.y < 0 || UV.y > 1)
		return 1;
	return tex2D(ShadowSampler, UV).r;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 texCol = tex2D(TextureSampler, input.UV);
	//if (texCol.a < AlphaMinValue)
		//return float4(0.8, 0.8, 0.8, 1);
	clip(texCol.a - AlphaMinValue);

	float2 texCoord = postProjToScreen(input.ScreenPosition) + halfPixel();
	float3 light = tex2D(LightSampler, texCoord) + AmbientColor;
	float4 compCol = float4(texCol * DiffuseColor * light, 1);

	float2 shadowTexCoord = postProjToScreen(input.ShadowScreenPosition) + halfPixel();
	float mapDepth = sampleShadowMap(shadowTexCoord);

	float realDepth = input.ShadowScreenPosition.z / input.ShadowScreenPosition.w;
	float shadow = 1;
	if (realDepth - ShadowBias > mapDepth)
		shadow = ShadowMult;
	//return float4(compCol.rgb * shadow, texCol.a);
	return float4(compCol.rgb * shadow, 1);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
