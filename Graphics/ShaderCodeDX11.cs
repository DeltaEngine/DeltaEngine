namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Default source code for the DirectX 11 shaders
	/// Note: When the next ShaderFlag feature will be added the shader code needs to be generated on
	/// the Server.
	/// </summary>
	public static class ShaderCodeDX11
	{
		public const string PositionColorDX11 = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};
struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};
float4x4 WorldViewProjection;

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	output.position = mul(input.position, WorldViewProjection);
	output.color = input.color;
	return output;
}

float4 PsMain(PixelInputType input) : SV_TARGET
{
	return input.color;
}";

		public const string PositionUVDX11 = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD;
};
struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD;
};
float4x4 WorldViewProjection;

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	output.position = mul(input.position, WorldViewProjection);
	output.texCoord = input.texCoord;
	return output;
}

Texture2D DiffuseTexture : register(t0);
SamplerState TextureSamplerState : register(s0);

float4 PsMain(PixelInputType input) : SV_TARGET
{
	return DiffuseTexture.Sample(TextureSamplerState, input.texCoord);
}";

		public const string PositionColorUVDX11 = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
	float2 texCoord : TEXCOORD;
};
struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
	float2 texCoord : TEXCOORD;
};
float4x4 WorldViewProjection;

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	output.position = mul(input.position, WorldViewProjection);
	output.color = input.color;
	output.texCoord = input.texCoord;
	return output;
}

Texture2D DiffuseTexture : register(t0);
SamplerState TextureSamplerState : register(s0);

float4 PsMain(PixelInputType input) : SV_TARGET
{
	return DiffuseTexture.Sample(TextureSamplerState, input.texCoord) * input.color;
}";

		public const string UVLightmapHLSLCode = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD0;
	float2 lightMapUV : TEXCOORD1;
};
struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD0;
	float2 lightMapUV : TEXCOORD1;
};
float4x4 WorldViewProjection;

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	output.position = mul(input.position, WorldViewProjection);
	output.texCoord = input.texCoord;
	output.lightMapUV = input.lightMapUV;
	return output;
}

Texture2D DiffuseTexture : register(t0);
Texture2D Lightmap : register(t1);
SamplerState TextureSamplerState : register(s0);

float4 PsMain(PixelInputType input) : SV_TARGET
{
	return DiffuseTexture.Sample(TextureSamplerState, input.texCoord) *
		Lightmap.Sample(TextureSamplerState, input.lightMapUV);
}";

		public const string PositionUVSkinnedDX11 = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD;
	float2 blendIndices : BLENDINDICES;
	float2 blendWeights : BLENDWEIGHT;
};
struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD;
};

float4x4 WorldViewProjection;
float4x4 JointTransforms[32];

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	float4 skinnedPosition = float4(0.0, 0.0, 0.0, 0.0);
	for (int jointIndex = 0; jointIndex < 2; jointIndex++)
	{
		int index = int(input.blendIndices[jointIndex]);
		float weight = input.blendWeights[jointIndex];
		skinnedPosition += mul(input.position, JointTransforms[index]) * weight;
	}
	output.position = mul(skinnedPosition, WorldViewProjection);
	output.texCoord = input.texCoord;
	return output;
}

Texture2D DiffuseTexture : register(t0);
SamplerState TextureSamplerState : register(s0);

float4 PsMain(PixelInputType input) : SV_TARGET
{
	return DiffuseTexture.Sample(TextureSamplerState, input.texCoord);
}";

		public const string PositionColorFogDX11 = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};
struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
	float4 FogMixColor : COLOR1;
};

float4x4 WorldViewProjection;
float4x4 World;
float4 CameraPosition;
float4 FogColor;
float FogStart;
float FogEnd;

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	output.position = mul(input.position, WorldViewProjection);
	output.color = input.color;

	float3 worldPos = mul(input.position.xyz, World);
	float3 vertexPositionInView = CameraPosition - worldPos;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - FogStart) / (FogEnd - FogStart), 0.0f, 1.0f);
	output.FogMixColor = float4(FogColor.rgb, fogFactor);

	return output;
}

float4 PsMain(PixelInputType input) : SV_TARGET
{
	return lerp(input.color, float4(input.FogMixColor.rgb, 1.0), input.FogMixColor.a);
}";

		public const string PositionUVFogDX11 = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD;
};
struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD;
	float4 FogMixColor : COLOR1;
};

float4x4 WorldViewProjection;
float4x4 World;
float4 CameraPosition;
float4 FogColor;
float FogStart;
float FogEnd;

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	output.position = mul(input.position, WorldViewProjection);
	output.texCoord = input.texCoord;

	float3 worldPos = mul(input.position.xyz, World);
	float3 vertexPositionInView = CameraPosition - worldPos;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - FogStart) / (FogEnd - FogStart), 0.0f, 1.0f);
	output.FogMixColor = float4(FogColor.rgb, fogFactor);

	return output;
}

Texture2D DiffuseTexture : register(t0);
SamplerState TextureSamplerState : register(s0);

float4 PsMain(PixelInputType input) : SV_TARGET
{
	float4 diffuseColor =  DiffuseTexture.Sample(TextureSamplerState, input.texCoord);
	return lerp(diffuseColor, float4(input.FogMixColor.rgb, diffuseColor.a), input.FogMixColor.a);
}";

		public const string PositionColorUVFogDX11 = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
	float2 texCoord : TEXCOORD;
};
struct PixelInputType
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
	float2 texCoord : TEXCOORD;
	float4 FogMixColor : COLOR1;
};

float4x4 WorldViewProjection;
float4x4 World;
float4 CameraPosition;
float4 FogColor;
float FogStart;
float FogEnd;

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	output.position = mul(input.position, WorldViewProjection);
	output.color = input.color;
	output.texCoord = input.texCoord;

	float3 worldPos = mul(input.position.xyz, World);
	float3 vertexPositionInView = CameraPosition - worldPos;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - FogStart) / (FogEnd - FogStart), 0.0f, 1.0f);
	output.FogMixColor = float4(FogColor.rgb, fogFactor);

	return output;
}

Texture2D DiffuseTexture : register(t0);
SamplerState TextureSamplerState : register(s0);

float4 PsMain(PixelInputType input) : SV_TARGET
{
	float4 diffuseColor =  DiffuseTexture.Sample(TextureSamplerState, input.texCoord) * input.color;
	return lerp(diffuseColor, float4(input.FogMixColor.rgb, diffuseColor.a), input.FogMixColor.a);
}";

		public const string PositionUVLightmapFogDX11 = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD0;
	float2 lightMapUV : TEXCOORD1;
};
struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD0;
	float2 lightMapUV : TEXCOORD1;
	float4 FogMixColor : COLOR1;
};

float4x4 WorldViewProjection;
float4x4 World;
float4 CameraPosition;
float4 FogColor;
float FogStart;
float FogEnd;

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	output.position = mul(input.position, WorldViewProjection);
	output.texCoord = input.texCoord;
	output.lightMapUV = input.lightMapUV;

	float3 worldPos = mul(input.position.xyz, World);
	float3 vertexPositionInView = CameraPosition - worldPos;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - FogStart) / (FogEnd - FogStart), 0.0f, 1.0f);
	output.FogMixColor = float4(FogColor.rgb, fogFactor);

	return output;
}

Texture2D DiffuseTexture : register(t0);
Texture2D Lightmap : register(t1);
SamplerState TextureSamplerState : register(s0);

float4 PsMain(PixelInputType input) : SV_TARGET
{
	float4 diffuseColor = DiffuseTexture.Sample(TextureSamplerState, input.texCoord) *
		Lightmap.Sample(TextureSamplerState, input.lightMapUV);
	return lerp(diffuseColor, float4(input.FogMixColor.rgb, diffuseColor.a), input.FogMixColor.a);
}";

		public const string PositionUVSkinnedFogDX11 = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD;
	float2 blendIndices : BLENDINDICES;
	float2 blendWeights : BLENDWEIGHT;
};
struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD;
	float4 FogMixColor : COLOR1;
};

float4x4 WorldViewProjection;
float4x4 JointTransforms[32];
float4x4 World;
float4 CameraPosition;
float4 FogColor;
float FogStart;
float FogEnd;

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	float4 skinnedPosition = float4(0.0, 0.0, 0.0, 0.0);
	for (int jointIndex = 0; jointIndex < 2; jointIndex++)
	{
		int index = int(input.blendIndices[jointIndex]);
		float weight = input.blendWeights[jointIndex];
		skinnedPosition += mul(input.position, JointTransforms[index]) * weight;
	}
	output.position = mul(skinnedPosition, WorldViewProjection);
	output.texCoord = input.texCoord;

	float3 worldPos = mul(input.position.xyz, World);
	float3 vertexPositionInView = CameraPosition - worldPos;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - FogStart) / (FogEnd - FogStart), 0.0f, 1.0f);
	output.FogMixColor = float4(FogColor.rgb, fogFactor);

	return output;
}

Texture2D DiffuseTexture : register(t0);
SamplerState TextureSamplerState : register(s0);

float4 PsMain(PixelInputType input) : SV_TARGET
{
	float4 diffuseColor =  DiffuseTexture.Sample(TextureSamplerState, input.texCoord);
	return lerp(diffuseColor, float4(input.FogMixColor.rgb, diffuseColor.a), input.FogMixColor.a);
}";

		public const string PositionUVNormal = @"
struct VertexInputType
{
	float4 position : SV_POSITION;
	float3 normal   : NORMAL;
	float2 texCoord : TEXCOORD;
};
struct PixelInputType
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD;
	float4 lightingColor : COLOR0;
};

float4x4 WorldViewProjection;
float4x4 World;
float4 lightDir;
float4 lightColor;

PixelInputType VsMain(VertexInputType input)
{
	PixelInputType output;
	output.position = mul(input.position, WorldViewProjection);
	output.texCoord = input.texCoord;
	float3 eyeSpaceVertexNormal = normalize(mul(input.normal, World)).xyz;
	float lightingFactor = max(0.0, dot(eyeSpaceVertexNormal, lightDir.xyz));
  output.lightingColor = float4(0.2, 0.2, 0.2, 1.0) + lightingFactor * lightColor;
	return output;
}

Texture2D DiffuseTexture : register(t0);
SamplerState TextureSamplerState : register(s0);

float4 PsMain(PixelInputType input) : SV_TARGET
{
	float4 diffuseColor = DiffuseTexture.Sample(TextureSamplerState, input.texCoord);
	return diffuseColor * input.lightingColor;
}";
	}
}