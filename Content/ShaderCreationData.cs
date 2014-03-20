namespace DeltaEngine.Content
{
	/// <summary>
	/// Creates shaders from flags, specific framework implemantation can be found in graphics.
	/// </summary>
	public class ShaderCreationData : ContentCreationData
	{
		protected ShaderCreationData() { }

		public ShaderCreationData(ShaderFlags flags)
		{
			Flags = flags;
		}

		public ShaderFlags Flags { get; set; }
	}
}