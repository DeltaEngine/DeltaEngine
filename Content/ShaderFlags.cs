using System;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Multiple values are combined to define a shader. This data is mainly used for fallback
	/// generation.  For more details see the ShaderEditor to view and edit materials and shaders.
	/// For details about shader flags, see https://deltaengine.fogbugz.com/default.asp?W282
	/// </summary>
	[Flags]
	public enum ShaderFlags
	{
		None = 1 << 0,
		Position2D = 1 << 1,
		Lit = 1 << 2,
		Colored = 1 << 3,
		Textured = 1 << 4,
		LightMap = 1 << 5,
		Skinned = 1 << 6,
		Fog = 1 << 7,
		AlphaTest = 1 << 8,
		VertexCompression = 1 << 9,
		NormalMap = 1 << 10,
		Position2DColored = Position2D | Colored,
		Position2DTextured = Position2D | Textured,
		Position2DColoredTextured = Position2D | Colored | Textured,
		ColoredTextured = Colored | Textured,
		TexturedLightMap = Textured | LightMap,
		LitTextured = Lit | Textured,
		LitTexturedSkinned = Lit | Textured | Skinned,
		TexturedSkinned = Textured | Skinned,
		ColoredFog = Colored | Fog,
		TexturedFog = Textured | Fog,
		ColoredTexturedFog = Colored | Textured | Fog,
		TexturedLightMapFog = Textured | LightMap | Fog,
		TexturedSkinnedFog = Textured | Skinned | Fog
	}
}