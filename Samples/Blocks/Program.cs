using DeltaEngine.Core;
using DeltaEngine.Platforms;

namespace Blocks
{
	/// <summary>
	/// Falling blocks can be moved and rotated. Points are scored by filling rows.
	/// </summary>
	public class Program : App
	{
		//ncrunch: no coverage start, this is equal to the default DE app initialization
		public Program()
		{
			new Game(Resolve<Window>());
		}

		public static void Main()
		{
			new Program().Run();
		}
		//ncrunch: no coverage end
	}
}