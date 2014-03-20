using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using NUnit.Framework;
using Spine;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Rendering2D.Spine.Tests
{
	public class SpineSkeletonTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateSpineboy()
		{
			spineboy = new SpineSkeleton("SpineboyAtlas", "SpineboySkeleton", Origin) { Scale = Scale };
			walk = spineboy.SkeletonData.FindAnimation("walk");
			jump = spineboy.SkeletonData.FindAnimation("jump");
			spineboy.SetAnimationLooped("walk");
		}

		private SpineSkeleton spineboy;
		private Animation walk;
		private Animation jump;

		[Test, ApproveFirstFrameScreenshot]
		public void DrawSpineboyWalkingToRightEdge()
		{
			spineboy.DefineAnimationMix("walk", "jump", 0.2f);
			spineboy.DefineAnimationMix("jump", "walk", 0.4f);
			new Command(Jump).Add(new MouseButtonTrigger());
			spineboy.Start<Moving>();
			new FontText(Font.Default, "Click mouse to make Spineboy jump",
				Rectangle.FromCenter(0.5f, 0.25f, 1.0f, 0.1f));
		}

		//ncrunch: no coverage start
		private static readonly Vector2D Origin = new Vector2D(0.1f, 0.75f);
		private static readonly Size Scale = new Size(0.8f, 0.8f);
		private static readonly Rectangle OriginAndScale = new Rectangle(Origin, Scale);

		private void Jump()
		{
			spineboy.AddAnimation("jump");
			spineboy.AddAnimationLooped("walk");
		} //ncrunch: no coverage end

		private class Moving : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (SpineSkeleton entity in entities.OfType<SpineSkeleton>())
				{
					entity.DrawArea = entity.DrawArea.Move(Time.Delta / 3.1f, 0.0f);
					if (entity.DrawArea.Left > 1.0f)
						entity.LastDrawArea = entity.DrawArea = OriginAndScale;
				}
			}
		}

		[Test]
		public void DrawSpineboyWalkingLeft()
		{
			Assert.AreEqual(FlipMode.None, spineboy.FlipMode);
			spineboy.Origin = new Vector2D(0.5f, 0.7f);
			spineboy.FlipMode = FlipMode.Horizontal;
			Assert.AreEqual(FlipMode.Horizontal, spineboy.FlipMode);
		}

		[Test]
		public void DrawUpsideDownSpineboyWalkingRight()
		{
			spineboy.Origin = new Vector2D(0.5f, 0.3f);
			spineboy.FlipMode = FlipMode.Vertical;
			Assert.AreEqual(FlipMode.Vertical, spineboy.FlipMode);
		}

		[Test]
		public void DrawSpineboyRotatingAroundFeet()
		{
			spineboy.Origin = Vector2D.Half;
			spineboy.Start<Rotating>();
			AdvanceTimeAndUpdateEntities();
		}

		private class Rotating : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (SpineSkeleton entity in entities.OfType<SpineSkeleton>())
					entity.Rotation += 45 * Time.Delta;
			}
		}

		[Test]
		public void DrawUpsideDownSpineboyWalkingLeft()
		{
			spineboy.Origin = new Vector2D(0.5f, 0.3f);
			spineboy.FlipMode = FlipMode.HorizontalAndVertical;
			Assert.AreEqual(FlipMode.HorizontalAndVertical, spineboy.FlipMode);
		}

		[Test]
		public void DrawManySpineboys()
		{
			for (int num = 0; num < 20; num++)
				CreateRandomizedSpineboy();
		}

		private static void CreateRandomizedSpineboy()
		{
			var origin = new Vector2D(Randomizer.Current.Get(0.2f, 0.8f), Randomizer.Current.Get(0.4f));
			var boy = new SpineSkeleton("SpineboyAtlas", "SpineboySkeleton", origin)
			{
				Scale = new Size(Randomizer.Current.Get(0.2f, 0.8f))
			};
			boy.SetAnimationLooped("walk");
			var startTime = Randomizer.Current.Get();
			boy.state.Update(startTime);
			boy.skeleton.Update(startTime);
		}

		[Test, CloseAfterFirstFrame]
		public void SetOrigin()
		{
			Assert.AreEqual(Origin, spineboy.Origin);
			spineboy.Origin = Vector2D.UnitX;
			Assert.AreEqual(Vector2D.UnitX, spineboy.Origin);
			Assert.AreEqual(Scale, spineboy.Scale);
		}

		[Test, CloseAfterFirstFrame]
		public void SetScale()
		{
			Assert.AreEqual(Scale, spineboy.Scale);
			spineboy.Scale = Size.Half;
			Assert.AreEqual(Origin, spineboy.Origin);
			Assert.AreEqual(Size.Half, spineboy.Scale);
		}

		[Test, CloseAfterFirstFrame]
		public void SetOriginAndScale()
		{
			Assert.AreEqual(OriginAndScale, spineboy.OriginAndScale);
			spineboy.OriginAndScale = Rectangle.HalfCentered;
			Assert.AreEqual(Rectangle.HalfCentered, spineboy.OriginAndScale);
		}

		[Test, CloseAfterFirstFrame]
		public void SetLastOriginAndScale()
		{
			Assert.AreEqual(new Rectangle(Origin, Size.One), spineboy.LastOriginAndScale);
			spineboy.LastOriginAndScale = Rectangle.HalfCentered;
			Assert.AreEqual(Rectangle.HalfCentered, spineboy.LastOriginAndScale);
		}

		[Test, CloseAfterFirstFrame]
		public void DrawUntilItLoopsBackToTheBeginning()
		{
			spineboy.SetAnimationLooped("walk");
			spineboy.Start<Moving>();
			AdvanceTimeAndUpdateEntities(3.2f);
		}

		[Test, CloseAfterFirstFrame]
		public void Dispose()
		{
			spineboy.Dispose();
		}

		[Test, CloseAfterFirstFrame]
		public void IsPauseable()
		{
			Assert.IsTrue(spineboy.IsPauseable);
		}

		[Test, CloseAfterFirstFrame]
		public void DefineAnimationMix()
		{
			spineboy.DefineAnimationMix("walk", "jump", 0.2f);
			Assert.AreEqual(0.2f, spineboy.stateData.GetMix(walk, jump));
		}

		[Test, CloseAfterFirstFrame]
		public void SetAnimation()
		{
			spineboy.SetAnimation("walk");
			Assert.AreEqual(walk, spineboy.state.GetCurrent(0).Animation);
		}

		[Test, CloseAfterFirstFrame]
		public void SetAnimationForTrack1()
		{
			spineboy.SetAnimation("walk", 1);
			Assert.AreEqual(walk, spineboy.state.GetCurrent(1).Animation);
		}

		[Test, CloseAfterFirstFrame]
		public void SetAnimationEnds()
		{
			bool isFinished = false;
			spineboy.SetAnimation("walk", () => isFinished = true);
			AdvanceTimeAndUpdateEntities(2.0f);
			Assert.IsTrue(isFinished);
		}

		[Test, CloseAfterFirstFrame]
		public void SetAnimationLooped()
		{
			spineboy.SetAnimationLooped("walk");
			AdvanceTimeAndUpdateEntities(2.5f);
			Assert.AreEqual(walk, spineboy.state.GetCurrent(0).Animation);
		}

		[Test, CloseAfterFirstFrame]
		public void AddAnimation()
		{
			spineboy.SetAnimation("walk");
			spineboy.AddAnimation("jump", 0.2f);
			Assert.AreEqual(walk, spineboy.state.GetCurrent(0).Animation);
			AdvanceTimeAndUpdateEntities(0.5f);
			Assert.AreEqual(jump, spineboy.state.GetCurrent(0).Animation);
		}

		[Test, CloseAfterFirstFrame, Timeout(5000)]
		public void AddAnimationEnds()
		{
			bool isFinished = false;
			spineboy.SetAnimation("walk");
			spineboy.AddAnimation("jump", () => isFinished = true, 0.2f);
			AdvanceTimeAndUpdateEntities(2.5f);
			Assert.IsTrue(isFinished);
		}

		[Test, CloseAfterFirstFrame]
		public void AddAnimationForTrack1()
		{
			spineboy.AddAnimation("walk", 0.1f, 1);
			Assert.AreEqual(walk, spineboy.state.GetCurrent(1).Animation);
		}

		[Test, CloseAfterFirstFrame]
		public void AddAnimationLooped()
		{
			spineboy.AddAnimationLooped("walk");
			AdvanceTimeAndUpdateEntities(2.0f);
			Assert.AreEqual(walk, spineboy.state.GetCurrent(0).Animation);
		}

		// ncrunch: no coverage start
		[Test, ApproveFirstFrameScreenshot, Ignore]
		public void RenderDragon()
		{
			spineboy.Dispose();
			new FontText(Font.Default, "Click mouse to make the dragon bite",
				Rectangle.FromCenter(0.5f, 0.25f, 1.0f, 0.1f));
			var dragon = new SpineSkeleton("DragonAtlas", "DragonSkeleton",
				new Rectangle(0.4f, 0.75f, 0.35f, 0.35f));
			new Command(() => dragon.SetAnimation("animation")).Add(new MouseButtonTrigger());
		}
	}
}