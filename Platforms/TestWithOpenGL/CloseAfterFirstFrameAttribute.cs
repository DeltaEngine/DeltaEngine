using System;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Marks a specific test or a whole test class via SetUp to close the test after the first frame.
	/// This is not needed for MockResolver tests as they will be closed after one frame anyway.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class CloseAfterFirstFrameAttribute : Attribute {}
}