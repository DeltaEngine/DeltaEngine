using System;
using DeltaEngine.Core;

namespace DeltaEngine.Scenes.Terminal.Tests
{
	public class TestCommand
	{
		//ncrunch: no coverage start
		[ConsoleCommand("AddFloats")]
		public float AddFloats(float a, float b)
		{
			return a + b;
		}

		[ConsoleCommand("AddInts")]
		public float AddInts(int a, int b)
		{
			return a + b;
		}

		[ConsoleCommand("ThrowsException")]
		public float ThrowsException()
		{
			throw new TestException();
		}

		public class TestException : Exception { }
	}
}