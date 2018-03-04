#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D BaseTexture;
float BaseWidth;
float BaseHeight;

Texture2D MaskTexture;
float MaskWidth;
float MaskHeight;
float MaskCenterX;
float MaskCenterY;
float MaskScale;
float MaskRotation; //Radians

sampler2D BaseTextureSampler = sampler_state
{
	Texture = <BaseTexture>;
};

sampler2D MaskTextureSampler = sampler_state
{
	Texture = <MaskTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float inBounds(float2 coords, float2 topLeft, float2 bottomRight)
{
	//if (coords.x >= topLeft.x && coords.y >= topLeft.y && coords.x < bottomRight.x && coords.y < bottomRight.y)
	//{
	//	return 1.0;
	//}
	//return 0.0;
	float2 s = step(topLeft, coords) - step(bottomRight, coords);
	return s.x * s.y;
}

float4 Mask(VertexShaderOutput input) : COLOR
{

	float4 tex = tex2D(BaseTextureSampler, input.TextureCoordinates) * input.Color;

	MaskWidth = MaskWidth * MaskScale;
	MaskHeight = MaskHeight * MaskScale;

	float maskX = MaskCenterX - (MaskWidth / 2);
	float maskY = MaskCenterY - (MaskHeight / 2);

	float baseX = input.TextureCoordinates.x * BaseWidth;
	float baseY = input.TextureCoordinates.y * BaseHeight;
	
	float applyMask = inBounds(float2(baseX, baseY), float2(maskX, maskY), float2(maskX + MaskWidth, maskY + MaskHeight));

	float alpha = 0.0;

	float2 maskCoords = float2((baseX - maskX) / MaskWidth, (baseY - maskY) / MaskHeight);

	float s = sin(MaskRotation);
	float c = cos(MaskRotation);

	float2 oldCoords = maskCoords - 0.5;

	maskCoords.x = clamp((oldCoords.x * c) - (oldCoords.y * s), -0.5, 0.5);
	maskCoords.y = clamp((oldCoords.x * s) + (oldCoords.y * c), -0.5, 0.5);
		
	maskCoords = maskCoords + 0.5;
		
	float4 maskTex = tex2D(MaskTextureSampler, maskCoords) * input.Color;

	alpha = maskTex.a * applyMask;

	tex = tex * alpha;

	return tex;
}

technique Mask
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL Mask();
	}
};