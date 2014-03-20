using System;
using System.Collections.Generic;
using DeltaEngine.Entities;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Entities
{
	internal class FilteredEntitiesTests
	{
		[Test]
		public void SelectingEntityHandlerProcessesEntitiesThatPassTheSelectionCriteria()
		{
			var entities = new MockEntitiesRunner(typeof(IncludeOnlyEntitiesWithPositiveFloats));
			var first = new MockEntity().Start<IncludeOnlyEntitiesWithPositiveFloats>().Add(3.0f);
			var excluded = new MockEntity().Start<IncludeOnlyEntitiesWithPositiveFloats>().Add(-1.0f);
			var second = new MockEntity().Start<IncludeOnlyEntitiesWithPositiveFloats>().Add(2.0f);
			entities.RunEntities();
			var start = first.Get<int>();
			Assert.IsFalse(excluded.Contains<int>());
			Assert.AreEqual(start + 1, second.Get<int>());
		}

		private class IncludeOnlyEntitiesWithPositiveFloats : UpdateBehavior, Filtered
		{
			public IncludeOnlyEntitiesWithPositiveFloats()
			{
				Filter = entity =>
				{
					return entity.Get<float>() > 0.0f;
				};
			}

			public Func<Entity, bool> Filter { get; set; }

			public override void Update(IEnumerable<Entity> entities)
			{
				int position = 0;
				foreach (Entity entity in entities)
					entity.Add(++position);
				Assert.AreEqual(2, position);
			}
		}

		[Test]
		public void SelectingEntitiesWithDifferentRenderLayer()
		{
			var entities = new MockEntitiesRunner(typeof(SortByRenderLayer));
			var last = new MockDrawableEntity { RenderLayer = 13 };
			last.OnDraw<SortByRenderLayer>();
			var first = new MockDrawableEntity { RenderLayer = -1 };
			first.OnDraw<SortByRenderLayer>();
			var middle = new MockDrawableEntity { RenderLayer = 5 };
			middle.OnDraw<SortByRenderLayer>();
			SortedResult.Clear();
			entities.RunEntities();
			Assert.AreEqual(first, SortedResult[0]);
			Assert.AreEqual(middle, SortedResult[1]);
			Assert.AreEqual(last, SortedResult[2]);
		}

		public static readonly List<Entity> SortedResult = new List<Entity>(); //ncrunch: no coverage

		private class SortByRenderLayer : DrawBehavior
		{
			public void Draw(List<DrawableEntity> visibleEntities)
			{
				foreach (var entity in visibleEntities)
					SortedResult.Add(entity);
			}
		}

		[Test]
		public void SelectingMultipleDifferentEntitiesWithDifferentRenderLayer()
		{
			var entities = new MockEntitiesRunner(typeof(SortByRenderLayer),
				typeof(AnotherSortByRenderLayer));
			var last = new MockDrawableEntity();
			last.OnDraw<SortByRenderLayer>();
			last.RenderLayer = 13;
			var first = new MockDrawableEntity { RenderLayer = -1 };
			first.OnDraw<AnotherSortByRenderLayer>();
			var middle1 = new MockDrawableEntity { RenderLayer = 5 };
			middle1.OnDraw<SortByRenderLayer>();
			var middle2 = new MockDrawableEntity { RenderLayer = 5 };
			middle2.OnDraw<AnotherSortByRenderLayer>();
			SortedResult.Clear();
			entities.RunEntities();
			Assert.AreEqual(first, SortedResult[0]);
			Assert.AreEqual(middle1, SortedResult[1]);
			Assert.AreEqual(middle2, SortedResult[2]);
			Assert.AreEqual(last, SortedResult[3]);
		}

		private class AnotherSortByRenderLayer : DrawBehavior
		{
			public void Draw(List<DrawableEntity> visibleEntities)
			{
				foreach (var entity in visibleEntities)
					SortedResult.Add(entity);
			}
		}

		[Test]
		public void GetDrawBehaviorFromNegativeRenderLayerEntity()
		{
			var entities = new MockEntitiesRunner(typeof(SortByRenderLayer),
				typeof(AnotherSortByRenderLayer));
			var first = new MockDrawableEntity { RenderLayer = -1 };
			first.OnDraw<SortByRenderLayer>();
			var middle1 = new MockDrawableEntity { RenderLayer = -1 };
			middle1.OnDraw<SortByRenderLayer>();
			entities.RunEntities();
		}
	}
}