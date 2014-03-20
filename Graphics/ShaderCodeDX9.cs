namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Default source code for the DirectX 9 shaders
	/// Note: When the next ShaderFlag feature will be added the shader code needs to be generated on
	/// the Server.
	/// </summary>
	public static class ShaderCodeDX9
	{
		public const string Position3DColorDX9 =  @"
float4x4 WorldViewProjection;
struct VS_OUTPUT
{
	float4 Pos       : POSITION;
	float4 Color     : COLOR0;
};

VS_OUTPUT VS( float3 Pos : POSITION, float4 Color : COLOR )
{ 
	VS_OUTPUT output = (VS_OUTPUT)0; 
	output.Pos = mul(float4(Pos, 1.0f), WorldViewProjection); 
	output.Color = float4(Color[2], Color[1], Color[0], Color[3]);  
	return output;  
}

float4 PS( VS_OUTPUT input ) : COLOR0  
{  
	return input.Color;  
}";

		public const string Position3DColorUVDX9 = @"
float4x4 WorldViewProjection;
struct VS_OUTPUT  
{
	float4 Pos       : POSITION;
	float4 Color     : COLOR0;  
	float2 TextureUV : TEXCOORD0;  
};

VS_OUTPUT VS( float3 Pos : POSITION, float4 Color : COLOR, float2 TextureUV : TEXCOORD0 ) 
{ 
	VS_OUTPUT output = (VS_OUTPUT)0;
	output.Pos = mul(float4(Pos, 1.0f), WorldViewProjection);
	output.Color = float4(Color[2], Color[1], Color[0], Color[3]);
	output.TextureUV = TextureUV;  
	return output;  
}

sampler DiffuseTexture; 
float4 PS( VS_OUTPUT input ) : COLOR0  
{
	return tex2D(DiffuseTexture, input.TextureUV) * input.Color;  
}";

		public const string Position3DUVDX9 = @"
float4x4 WorldViewProjection;
struct VS_OUTPUT  
{  
	float4 Pos				: POSITION; 
	float2 TextureUV : TEXCOORD0;  
};

VS_OUTPUT VS( float3 Pos : POSITION, float2 TextureUV : TEXCOORD0 )  
{ 
	VS_OUTPUT output = (VS_OUTPUT)0; 
	output.Pos = mul(float4(Pos, 1.0f), WorldViewProjection); 
	output.TextureUV = TextureUV;  
	return output;  
}

sampler DiffuseTexture;
float4 PS( VS_OUTPUT input ) : COLOR0  
{
	return tex2D(DiffuseTexture, input.TextureUV);  
}";

		public const string Position2DColorDX9 = @"
float4x4 WorldViewProjection;
struct VS_OUTPUT  
{  
	float4 Pos       : POSITION; 
	float4 Color     : COLOR0;  
}; 

VS_OUTPUT VS( float2 Pos : POSITION, float4 Color : COLOR )  
{ 
	VS_OUTPUT output = (VS_OUTPUT)0; 
	output.Pos = mul(float4(Pos[0], Pos[1], 0.0f, 1.0f), WorldViewProjection); 
	output.Color = float4(Color[2], Color[1], Color[0], Color[3]);  
	return output;  
}
 
float4 PS( VS_OUTPUT input ) : COLOR0  
{  
	return input.Color;  
}";
		
		/// <summary>
		/// The -0.5 pixel offset is needed in DirectX 9 to correctly match the texture coordinates:
		/// http://msdn.microsoft.com/en-us/library/windows/desktop/bb219690(v=vs.85).aspx
		/// </summary>
		public const string Position2DColorUVDX9 = @"
float4x4 WorldViewProjection;
struct VS_OUTPUT  
{  
	float4 Pos       : POSITION;
	float4 Color     : COLOR0;  
	float2 TextureUV : TEXCOORD0;  
}; 

VS_OUTPUT VS( float2 Pos : POSITION, float4 Color : COLOR, float2 TextureUV : TEXCOORD0 ) 
{
	const float Offset = -0.5;
	VS_OUTPUT output = (VS_OUTPUT)0;
	output.Pos = mul(float4(Pos[0] + Offset, Pos[1] + Offset, 0.0f, 1.0f), WorldViewProjection);
	output.Color = float4(Color[2], Color[1], Color[0], Color[3]);
	output.TextureUV = TextureUV;  
	return output;  
}

sampler DiffuseTexture;
float4 PS( VS_OUTPUT input ) : COLOR0  
{ 
	return tex2D(DiffuseTexture, input.TextureUV) * input.Color;  
}";

		public const string Position2DUVDX9 = @"
float4x4 WorldViewProjection;  
struct VS_OUTPUT  
{  
	float4 Pos       : POSITION;
	float2 TextureUV : TEXCOORD0;  
}; 

VS_OUTPUT VS( float2 Pos : POSITION, float2 TextureUV : TEXCOORD0 )  
{ 
	const float Offset = -0.5;
	VS_OUTPUT output = (VS_OUTPUT)0; 
	output.Pos = mul(float4(Pos[0] + Offset, Pos[1] + Offset, 0.0f, 1.0f), WorldViewProjection); 
	output.TextureUV = TextureUV;  
	return output;  
}  

sampler DiffuseTexture; 
float4 PS( VS_OUTPUT input ) : COLOR0  
{ 
	return tex2D(DiffuseTexture, input.TextureUV);  
}";

		public const string DX9Position3DLightMap = @"
float4x4 WorldViewProjection;  
struct VS_OUTPUT  
{  
	float4 Pos       : POSITION;
	float2 TextureUV : TEXCOORD0;
	float2 LightMapUV: TEXCOORD1;  
}; 

VS_OUTPUT VS( float3 Pos : POSITION, float2 TextureUV : TEXCOORD0, float2 LightMapUV : TEXCOORD1 ) 
{  
	VS_OUTPUT output = (VS_OUTPUT)0;
	output.Pos = mul(float4(Pos, 1.0f), WorldViewProjection); 
	output.TextureUV = TextureUV;  
	output.LightMapUV = LightMapUV;  
	return output; 
}  

sampler DiffuseTexture : register(s0);  
sampler LightmapTexture : register(s1);
float4 PS( VS_OUTPUT input ) : COLOR0  
{ 
	return tex2D(DiffuseTexture, input.TextureUV) * tex2D(LightmapTexture, input.LightMapUV); 
}";

		public const string PositionUVSkinnedDX9 = @"
float4x4 WorldViewProjection;
float4x4 JointTransforms[32];

struct VS_OUTPUT  
{
	float4 Pos				  : POSITION;
	float2 TextureUV    : TEXCOORD0;
};

VS_OUTPUT VS(float4 Pos : POSITION, float2 TextureUV : TEXCOORD0,
	float2 blendIndices : BLENDINDICES, float2 blendWeights : BLENDWEIGHT)
{ 
	VS_OUTPUT output = (VS_OUTPUT)0;
	float4 skinnedPosition = float4(0.0, 0.0, 0.0, 0.0);
	for (int jointIndex = 0; jointIndex < 2; jointIndex++)
	{
		int index = int(blendIndices[jointIndex]);
		float weight = blendWeights[jointIndex];
		skinnedPosition += mul(Pos, JointTransforms[index]) * weight;
	}

	output.Pos = mul(skinnedPosition, WorldViewProjection);
//	output.Pos = mul(Pos, WorldViewProjection);
	output.TextureUV = TextureUV;
	return output;
}

sampler DiffuseTexture;
float4 PS( VS_OUTPUT input ) : COLOR0
{
	return tex2D(DiffuseTexture, input.TextureUV);
}";

		public const string PositionColorFogDX9 = @"
float4x4 WorldViewProjection;
float4x4 World;
float4 CameraPosition;
float4 FogColor;
float2 FogRange;

struct VS_OUTPUT
{
	float4 Pos         : POSITION;
	float4 Color       : COLOR0;
	float4 FogMixColor : COLOR1;
};

VS_OUTPUT VS(float3 Pos : POSITION, float4 Color : COLOR)
{ 
	VS_OUTPUT output = (VS_OUTPUT)0;
	output.Pos = mul(float4(Pos, 1.0f), WorldViewProjection);
	output.Color = float4(Color[2], Color[1], Color[0], Color[3]);

	float3 worldPos = mul(Pos.xyz, World);
	float3 vertexPositionInView = CameraPosition - worldPos;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - FogRange.x) / (FogRange.y - FogRange.x), 0.0f, 1.0f);
	output.FogMixColor = float4(FogColor.rgb, fogFactor);

	return output;
} 

float4 PS( VS_OUTPUT input ) : COLOR0  
{  
		return lerp(input.Color, float4(input.FogMixColor.rgb, 1.0), input.FogMixColor.a);
}";

		public const string PositionUVFogDX9 = @"
float4x4 WorldViewProjection;
float4x4 World;
float4 CameraPosition;
float4 FogColor;
float2 FogRange;

struct VS_OUTPUT
{
	float4 Pos         : POSITION;
	float2 TextureUV   : TEXCOORD0;
	float4 FogMixColor : COLOR1;
};

VS_OUTPUT VS( float3 Pos : POSITION, float2 TextureUV : TEXCOORD0 )  
{ 
	VS_OUTPUT output = (VS_OUTPUT)0; 
	output.Pos = mul(float4(Pos, 1.0f), WorldViewProjection); 
	output.TextureUV = TextureUV;  

	float3 worldPos = mul(Pos.xyz, World);
	float3 vertexPositionInView = CameraPosition - worldPos;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - FogRange.x) / (FogRange.y - FogRange.x), 0.0f, 1.0f);
	output.FogMixColor = float4(FogColor.rgb, fogFactor);

	return output;
}

sampler DiffuseTexture;
float4 PS( VS_OUTPUT input ) : COLOR0  
{
	float4 diffuseColor = tex2D(DiffuseTexture, input.TextureUV);  
	return lerp(diffuseColor, float4(input.FogMixColor.rgb, diffuseColor.a), input.FogMixColor.a);
}";

		public const string PositionColorUVFogDX9 = @"
float4x4 WorldViewProjection;
float4 CameraPosition;
float4 FogColor;
float2 FogRange;

struct VS_OUTPUT
{
	float4 Pos         : POSITION;
	float4 Color       : COLOR0;
	float2 TextureUV   : TEXCOORD0;
	float4 FogMixColor : COLOR1;
};

VS_OUTPUT VS( float3 Pos : POSITION, float4 Color : COLOR, float2 TextureUV : TEXCOORD0 )  
{ 
	VS_OUTPUT output = (VS_OUTPUT)0;
	output.Pos = mul(float4(Pos, 1.0f), WorldViewProjection);
	output.Color = float4(Color[2], Color[1], Color[0], Color[3]);
	output.TextureUV = TextureUV;

	float3 vertexPositionInView = CameraPosition - Pos;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - FogRange.x) / (FogRange.y - FogRange.x), 0.0f, 1.0f);
	output.FogMixColor = float4(FogColor.rgb, fogFactor);

	return output;
}

sampler DiffuseTexture;
float4 PS( VS_OUTPUT input ) : COLOR0  
{
	float4 diffuseColor = tex2D(DiffuseTexture, input.TextureUV) * input.Color;  
	return lerp(diffuseColor, float4(input.FogMixColor.rgb, diffuseColor.a), input.FogMixColor.a);
}";

		public const string PositionUVLightmapFogDX9 = @"
float4x4 WorldViewProjection;
float4x4 World;
float4 CameraPosition;
float4 FogColor;
float2 FogRange;

struct VS_OUTPUT  
{  
	float4 Pos       : POSITION;
	float2 TextureUV : TEXCOORD0;
	float2 LightMapUV: TEXCOORD1;
	float4 FogMixColor : COLOR1;
}; 

VS_OUTPUT VS( float3 Pos : POSITION, float2 TextureUV : TEXCOORD0, float2 LightMapUV : TEXCOORD1 )
{  
	VS_OUTPUT output = (VS_OUTPUT)0;
	output.Pos = mul(float4(Pos, 1.0f), WorldViewProjection);
	output.TextureUV = TextureUV;
	output.LightMapUV = LightMapUV;

	float3 worldPos = mul(Pos.xyz, World);
	float3 vertexPositionInView = CameraPosition - worldPos;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - FogRange.x) / (FogRange.y - FogRange.x), 0.0f, 1.0f);
	output.FogMixColor = float4(FogColor.rgb, fogFactor);

	return output;
}  

sampler DiffuseTexture : register(s0);
sampler LightmapTexture : register(s1);
float4 PS( VS_OUTPUT input ) : COLOR0
{ 
	float4 diffuseColor = tex2D(DiffuseTexture, input.TextureUV) *
		tex2D(LightmapTexture, input.LightMapUV);  
	return lerp(diffuseColor, float4(input.FogMixColor.rgb, diffuseColor.a), input.FogMixColor.a);
}";

		public const string PositionUVSkinnedFogDX9 = @"
float4x4 WorldViewProjection;
float4x4 JointTransforms[32];
float4x4 World;
float4 CameraPosition;
float4 FogColor;
float2 FogRange;

struct VS_OUTPUT  
{
	float4 Pos				  : POSITION;
	float2 TextureUV    : TEXCOORD0;
	float4 FogMixColor : COLOR1;
};

VS_OUTPUT VS(float4 Pos : POSITION, float2 TextureUV : TEXCOORD0,
	float2 blendIndices : BLENDINDICES, float2 blendWeights : BLENDWEIGHT)
{ 
	VS_OUTPUT output = (VS_OUTPUT)0;
	float4 skinnedPosition = float4(0.0, 0.0, 0.0, 0.0);
	for (int jointIndex = 0; jointIndex < 2; jointIndex++)
	{
		int index = int(blendIndices[jointIndex]);
		float weight = blendWeights[jointIndex];
		skinnedPosition += mul(Pos, JointTransforms[index]) * weight;
	}

	output.Pos = mul(skinnedPosition, WorldViewProjection);
//	output.Pos = mul(Pos, WorldViewProjection);
	output.TextureUV = TextureUV;

	float3 worldPos = mul(Pos.xyz, World);
	float3 vertexPositionInView = CameraPosition - worldPos;
	float distanceToCamera = length(vertexPositionInView);
	float fogFactor = clamp((distanceToCamera - FogRange.x) / (FogRange.y - FogRange.x), 0.0f, 1.0f);
	output.FogMixColor = float4(FogColor.rgb, fogFactor);

	return output;
}

sampler DiffuseTexture;
float4 PS( VS_OUTPUT input ) : COLOR0
{
	float4 diffuseColor = tex2D(DiffuseTexture, input.TextureUV);
	return lerp(diffuseColor, float4(input.FogMixColor.rgb, diffuseColor.a), input.FogMixColor.a);
}";

		public const string PositionUVNormal = @"
float4x4 WorldViewProjection;
float4x4 World;
float4 lightDir;
float4 lightColor;

struct VS_OUTPUT
{
	float4 position : SV_POSITION;
	float2 texCoord : TEXCOORD;
	float4 lightingColor : COLOR0;
};

VS_OUTPUT VS( float4 Pos : POSITION, float3 Normal : NORMAL, float2 TextureUV : TEXCOORD0 )  
{
	VS_OUTPUT output = (VS_OUTPUT)0;
	output.position = mul(Pos, WorldViewProjection);
	output.texCoord = TextureUV;
	float3 eyeSpaceVertexNormal = normalize(mul(Normal, World)).xyz;
	float lightingFactor = max(0.0, dot(eyeSpaceVertexNormal, lightDir.xyz));
  output.lightingColor = float4(0.2, 0.2, 0.2, 1.0) + lightingFactor * lightColor;
	return output;
}

sampler DiffuseTexture : register(s0);

float4 PS( VS_OUTPUT input ) : COLOR0
{
	float4 diffuseColor = tex2D(DiffuseTexture, input.texCoord);
	return diffuseColor * input.lightingColor;
}";
	}
}