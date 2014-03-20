using System;
using DeltaEngine.Core;

namespace DeltaEngine.Platforms.Tests
{
	public class TestCommand
	{
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

		public class TestException : Exception {}
	}
}