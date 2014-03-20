namespace DeltaEngine.Graphics.Tests
{
	public static class Program
	{
		//ncrunch: no coverage start
		public static void Main()
		{
			var tests = new ImageTests();
			tests.InitializeResolver();
			tests.DrawOpaqueImageWithVertexColors();
			tests.RunTestAndDisposeResolverWhenDone();
		}
	}
}