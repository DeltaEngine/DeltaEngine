namespace Blocks
{
	/// <summary>
	/// Loads FruitBlocks related content and settings
	/// </summary>
	public class FruitBlocksContent : BlocksContent
	{
		public FruitBlocksContent()
			: base("FruitBlocks_")
		{
			DoBricksSplitInHalfWhenRowFull = true;
		}
	}
}