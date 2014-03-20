using System;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Makes a screenshot after the first frame of any integration or visual test and then compares
	/// it with previous results. Works across frameworks to test OpenGL, DirectX and XNA together.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ApproveFirstFrameScreenshotAttribute : Attribute {}
}