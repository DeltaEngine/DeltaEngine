using DeltaEngine.Multimedia;

namespace $safeprojectname$
{
	/// <summary>
	/// Plays sound effects when key events occur
	/// </summary>
	public class Soundbank
	{
		public Soundbank(BlocksContent content)
		{
			BlockAffixed = content.Load<Sound>("BlockAffixed");
			BlockCouldNotMove = content.Load<Sound>("BlockCantMove");
			BlockMoved = content.Load<Sound>("BlockMoved");
			GameLost = content.Load<Sound>("GameLost");
			RowRemoved = content.Load<Sound>("RowRemoved");
			MultipleRowsRemoved = content.Load<Sound>("MultipleRowsRemoved");
		}

		public Sound BlockAffixed { get; private set; }
		public Sound BlockCouldNotMove { get; private set; }
		public Sound BlockMoved { get; private set; }
		public Sound GameLost { get; private set; }
		public Sound RowRemoved { get; private set; }
		public Sound MultipleRowsRemoved { get; private set; }
	}
}