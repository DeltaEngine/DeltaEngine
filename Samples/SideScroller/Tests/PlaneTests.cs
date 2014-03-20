using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace SideScroller.Tests
{
	internal class PlaneTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreatePlayerPlane()
		{
			playerPlane = new PlayerPlane(Vector2D.Half);
		}

		private PlayerPlane playerPlane;

		[Test, ApproveFirstFrameScreenshot]
		public void ShowPlayerPlane() {}

		[Test, CloseAfterFirstFrame]
		public void MovePlaneVertically()
		{
			CheckMoveUp();
			CheckMoveDown();
			CheckStop();
		}

		private void CheckMoveUp()
		{
			var originalYCoord = playerPlane.YPosition;
			playerPlane.AccelerateVertically(1);
			AdvanceTimeAndUpdateEntities(0.5f);
			Assert.Greater(playerPlane.YPosition, originalYCoord);
		}

		private void CheckMoveDown()
		{
			var originalYCoord = playerPlane.YPosition;
			playerPlane.AccelerateVertically(-1);
			AdvanceTimeAndUpdateEntities(0.5f);
			Assert.Less(playerPlane.YPosition, originalYCoord);
		}

		private void CheckStop()
		{
			playerPlane.AccelerateVertically(3);
			var originalSpeed = playerPlane.Get<Velocity2D>().velocity.Y;
			playerPlane.StopVertically();
			AdvanceTimeAndUpdateEntities(0.5f);
			Assert.Less(playerPlane.Get<Velocity2D>().velocity.Y, originalSpeed);
		}

		[Test, CloseAfterFirstFrame]
		public void HittingTopBorder()
		{
			playerPlane.DrawArea = new Rectangle(new Vector2D(playerPlane.DrawArea.Left, -0.5f),
				playerPlane.DrawArea.Size);
			playerPlane.Set(new Velocity2D(new Vector2D(0, -0.1f), 0.5f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(ScreenSpace.Current.Viewport.Top, playerPlane.DrawArea.Top, 0.01f);
		}

		[Test, CloseAfterFirstFrame]
		public void HittingBottomBorder()
		{
			playerPlane.DrawArea = new Rectangle(new Vector2D(playerPlane.DrawArea.Left, 1.5f),
				playerPlane.DrawArea.Size);
			playerPlane.Set(new Velocity2D(new Vector2D(0, 0.1f), 0.5f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(ScreenSpace.Current.Viewport.Bottom, playerPlane.DrawArea.Bottom, 0.01f);
		}
	}
}