using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Fonts.Tests
{
	public class VectorTextTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void DrawHi()
		{
			new VectorText("Hi", Vector2D.Half);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawSampleText()
		{
			new VectorText("The Quick Brown Fox...", Vector2D.Half) { Color = Color.Red };
			new VectorText("Jumps Over The Lazy Dog", new Vector2D(0.5f, 0.6f)) { Color = Color.Teal };
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawBigText()
		{
			new VectorText("Yo yo, whats up", Vector2D.Half) { Size = new Size(0.1f) };
		}

		[Test, CloseAfterFirstFrame]
		public void DrawingTwoVectorTextsWithTheSameRenderLayerOnlyIssuesOneDrawCall()
		{
			new VectorText("Yo yo, whats up", Vector2D.Half) { Size = new Size(0.1f) };
			new VectorText("Jumps Over The Lazy Dog", new Vector2D(0.5f, 0.6f)) { Color = Color.Teal };
			RunAfterFirstFrame(
				() => Assert.AreEqual(1, Resolve<Drawing>().NumberOfDynamicDrawCallsThisFrame));
		}

		[Test, CloseAfterFirstFrame]
		public void DrawingTwoVectorTextsWithDifferentRenderLayersIssuesTwoDrawCalls()
		{
			new VectorText("Yo yo, whats up", Vector2D.Half) { Size = new Size(0.1f), RenderLayer = 1 };
			new VectorText("Jumps Over The Lazy Dog", Vector2D.One) { Color = Color.Teal, RenderLayer = 2 };
			RunAfterFirstFrame(
				() => Assert.AreEqual(2, Resolve<Drawing>().NumberOfDynamicDrawCallsThisFrame));
		}

		[Test, CloseAfterFirstFrame]
		public void RenderingHiddenVectorTextDoesNotThrowException()
		{
			new VectorText("The Quick Brown Fox...", Vector2D.Half) { IsVisible = false };
			Assert.DoesNotThrow(() => AdvanceTimeAndUpdateEntities());
		}

		[Test, ApproveFirstFrameScreenshot]
		public void ChangeText()
		{
			var text = new VectorText("Unchanged", Vector2D.Half) { Text = "Changed" };
			Assert.AreEqual("Changed", text.Text);
		}
	}
}