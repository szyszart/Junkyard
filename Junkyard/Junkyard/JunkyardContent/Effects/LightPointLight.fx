#include "Shared.vsi"

float4x4 WorldViewProjection;
float4x4 InvViewProjection;

texture2D DepthTexture;
sampler2D DepthSampler = sampler_state
{
	texture = <DepthTexture>;
	minfilter = point;
	magfilter = point;
	mipfilter = point;
};

texture2D NormalTexture;
sampler2D NormalSampler = sampler_state
{
	texture = <NormalTexture>;
	minfilter = point;
	magfilter = point;
	mipfilter = point;
};

float3 LightColor;
float3 LightPosition;
float LightAttenuation;
float3 CameraPosition;

float SpecularPower = 16;
float3 SpecularColor = float3(1, 1, 1);

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 LightPosition : TEXCOORD0;
	float3 ViewDirection : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	output.Position = mul(input.Position, WorldViewProjection);
	output.LightPosition = output.Position;
	output.ViewDirection = output.Position - CameraPosition;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float2 texCoord = postProjToScreen(input.LightPosition) + halfPixel();
	float4 depth = tex2D(DepthSampler, texCoord);
	float4 position = float4(texCoord.x * 2 - 1,
							 2 * (1 - texCoord.y) - 1,
							 depth.r,
							 1.0f);
	position = mul(position, InvViewProjection);
	position.xyz /= position.w;
	
	float4 normal = (tex2D(NormalSampler, texCoord) - .5) * 2;
	float3 lightDirection = normalize(LightPosition - position);
	float lighting = clamp(dot(normal, lightDirection), 0, 1);
	float d = distance(LightPosition, position);
	float att = 1 - pow(d / LightAttenuation, 6);

	float3 refl = reflect(lightDirection, normal);
	float3 view = normalize(input.ViewDirection);
	lighting += pow(saturate(dot(refl, view)), SpecularPower) * SpecularColor;

	float3 output = saturate(lighting) * LightColor * lighting * att;
	return float4(output, 1);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
