using DeltaEngine.Platforms;

namespace ComplexMenu
{
	internal class Program : App
	{
		public Program()
		{
			new Menu();
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}
