using System;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Extensions
{
	public class AssemblyStarterTests
	{
		//ncrunch: no coverage start
		[Test, Ignore]
		public void StartGlobalTimeTest()
		{
			using (var starter = new AssemblyStarter("DeltaEngine.Tests.dll", false))
				starter.Start("GlobalTimeTests", "CalculateFpsWithStopwatch");
		}

		[Test, Ignore]
		public void FindAllDeltaEngineTests()
		{
			using (var starter = new AssemblyStarter("DeltaEngine.Tests.dll", false))
				foreach (var test in starter.GetTestNames())
					Console.WriteLine(test);
		}

		[Test, Ignore]
		public void FindAllGraphicsTests()
		{
			const string TestAssemblyFilename =
				@"c:\code\DeltaEngine\Graphics\Tests\bin\Debug\DeltaEngine.Graphics.Tests.dll";
			using (var starter = new AssemblyStarter(TestAssemblyFilename, false))
				foreach (var test in starter.GetTestNames())
					Console.WriteLine(test);
		}

		[Test, Ignore]
		public void StartGraphicsVisualTest()
		{
			const string TestAssemblyFilename =
				@"c:\code\DeltaEngine\Graphics\Tests\bin\Debug\DeltaEngine.Graphics.Tests.dll";
			using (var starter = new AssemblyStarter(TestAssemblyFilename, true))
				starter.Start("DeviceTests", "DrawRedBackground");
		}
	}
}