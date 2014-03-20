namespace DeltaEngine.Graphics.Vertices
{
	/// <summary>
	/// Vertex elements are used for describe the vertex data layout.
	/// </summary>
	public enum VertexElementType
	{
		/// <summary>
		/// 2D position data.
		/// </summary>
		Position2D = 1,
		/// <summary>
		/// 3D position data.
		/// </summary>
		Position3D,
		/// <summary>
		/// Normal vector data.
		/// </summary>
		Normal,
		/// <summary>
		/// Tangent vector for normal mapping.
		/// </summary>
		Tangent,
		/// <summary>
		/// Binormal vector for normal mapping.
		/// </summary>
		Binormal,
		/// <summary>
		/// Color for this vertex.
		/// </summary>
		Color,
		/// <summary>
		/// UV data.
		/// </summary>
		TextureUV,
		/// <summary>
		/// UVW 3D texture data for cube mapping or reflection cube maps.
		/// </summary>
		TextureUVW,
		/// <summary>
		/// Optional light map UV channel (secondary texture channel).
		/// </summary>
		LightMapUV,
		/// <summary>
		/// Extra UV channel.
		/// </summary>
		ExtraUV,
		/// <summary>
		/// Skin bone indices.
		/// /// </summary>
		SkinIndices,
		/// <summary>
		/// Skin bone weight data for skinning.
		/// </summary>
		SkinWeights,
	}
}
