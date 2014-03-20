using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Entities
{
	internal class DrawableEntityTests
	{
		[SetUp]
		public void InitializeEntitiesRunner()
		{
			new MockEntitiesRunner(typeof(Draw), typeof(DrawToCopyArrayListLength));
			new MockEntity();
		}

		[Test]
		public void TryToGetListWillThrowExceptionIfNoListsAvailable()
		{
			var draw = new MockDrawableEntity();
			draw.OnDraw<Draw>();
			EntitiesRunner.Current.UpdateAndDrawAllEntities(() => { });
		}

		private class Draw : DrawBehavior
		{
			void DrawBehavior.Draw(List<DrawableEntity> visibleEntities)
			{
				foreach (var drawableEntity in visibleEntities)
					ThrowExceptionsWhenInterpolationElementsAreNotFound(drawableEntity);
			}

			private static void ThrowExceptionsWhenInterpolationElementsAreNotFound(
				DrawableEntity drawableEntity)
			{
				Assert.Throws<DrawableEntity.ListWithLerpElementsForInterpolationWasNotFound>(
					() => { drawableEntity.GetInterpolatedList<MockLerp>(); });
				Assert.Throws<DrawableEntity.ArrayWithLerpElementsForInterpolationWasNotFound>(
					() => { drawableEntity.GetInterpolatedArray<MockLerp>(); });
			}
		}

		private class MockLerp : Lerp<MockLerp>
		{
			public MockLerp Lerp(MockLerp other, float interpolation)
			{
				return new MockLerp();
			}
		}

		[Test]
		public void ChangeLengthToCopyLimit()
		{
			var draw = new MockDrawableEntity();
			draw.OnDraw<DrawToCopyArrayListLength>();
			var mockLerp = new MockLerp().Lerp(new MockLerp(), 1);
			var lerp = new MockLerp[3];
			lerp[0] = mockLerp;
			lerp[1] = mockLerp;
			lerp[2] = mockLerp;
			draw.Add(lerp);
			EntitiesRunner.Current.UpdateAndDrawAllEntities(() => { });
		}

		private class DrawToCopyArrayListLength : DrawBehavior
		{
			void DrawBehavior.Draw(List<DrawableEntity> visibleEntities)
			{
				foreach (var drawableEntity in visibleEntities)
					drawableEntity.GetInterpolatedArray<MockLerp>(2);
			}
		}

		[Test]
		public void SettingLerpableComponentAddsToLastTickComponents()
		{
			var draw = new MockDrawableEntity();
			draw.Set(1.0f);
			Assert.AreEqual(1.0f, draw.GetLastTickLerpComponents()[0]);
		}

		[Test]
		public void ChangingLerpableComponentLeavesLastTickValueUnchanged()
		{
			var draw = new MockDrawableEntity();
			draw.Set(1.0f);
			EntitiesRunner.Current.UpdateAndDrawAllEntities(() => { });
			draw.Set(2.0f);
			Assert.AreEqual(1.0f, draw.GetLastTickLerpComponents()[0]);
		}

		[Test]
		public void SettingWithoutInterpolationANonLerpableComponentDoesNotAffectLastTickLerpComponents()
		{
			var draw = new MockDrawableEntity();
			draw.SetWithoutInterpolation(1);
			Assert.AreEqual(0, draw.GetLastTickLerpComponents().Count);
		}

		[Test]
		public void SettingWithoutInterpolationALerpableComponentChangesLastTickValue()
		{
			var draw = new MockDrawableEntity();
			draw.Set(1.0f);
			draw.IsActive = true;
			EntitiesRunner.Current.UpdateAndDrawAllEntities(() => { });
			draw.SetWithoutInterpolation(2.0f);
			Assert.AreEqual(2.0f, draw.GetLastTickLerpComponents()[0]);
		}

		[Test]
		public void DoesNotIncludeRenderLayerForSavingIfItIsDefaultValue()
		{
			var draw = new MockDrawableEntity();
			Assert.AreEqual(0, draw.GetComponentsForSaving().Count);
		}

		[Test]
		public void IncludesRenderLayerForSavingIfItIsNotDefaultValue()
		{
			var draw = new MockDrawableEntity { RenderLayer = 1 };
			Assert.AreEqual(1, draw.GetComponentsForSaving().Count);
			Assert.AreEqual(1, draw.GetComponentsForSaving()[0]);
		}
	}
}