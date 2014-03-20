using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Tests
{
	internal class HierarchyEntity2DTests : TestWithMocksOrVisually
	{
		[Test]
		public void PositionSetToChildOnAdd()
		{
			var parentEntity = new MockHierarchyEntity2D(Rectangle.FromCenter(Vector2D.Half, Size.Zero));
			var childEntity = new MockHierarchyEntity2D(Rectangle.Zero);
			var grandChildRelativePosition = new Vector2D(0.2f, 0.0f);
			var grandChildEntity =
				new MockHierarchyEntity2D(Rectangle.FromCenter(grandChildRelativePosition, Size.Zero));
			childEntity.AddChild(grandChildEntity);
			childEntity.Parent = parentEntity;
			Assert.AreEqual(parentEntity.Position, childEntity.Position);
			Assert.AreEqual(parentEntity.Position + grandChildRelativePosition,
				grandChildEntity.Position);
			Assert.AreEqual(grandChildRelativePosition, grandChildEntity.LocalPosition);
		}

		private class MockHierarchyEntity2D : HierarchyEntity2D
		{
			public MockHierarchyEntity2D() {}

			public MockHierarchyEntity2D(Vector2D position)
				: base(position) {}

			public MockHierarchyEntity2D(Rectangle drawArea, float rotation = 0.0f)
				: base(drawArea, rotation) {}
		}

		[Test]
		public void SetLocalsUpdatesGlobals()
		{
			var parentEntity = new MockHierarchyEntity2D(Rectangle.FromCenter(Vector2D.Half, Size.Zero));
			var childEntity = new MockHierarchyEntity2D(Rectangle.Zero);
			parentEntity.AddChild(childEntity);
			var childRelativePosition = new Vector2D(0.2f, 0.0f);
			childEntity.LocalPosition = childRelativePosition;
			childEntity.LocalRotation = 10;
			Assert.AreEqual(parentEntity.Position + childRelativePosition, childEntity.Position);
			Assert.AreEqual(10, childEntity.Rotation);
		}

		[Test]
		public void BuildHierarchyOfDifferentEntities()
		{
			var basicEntity = new MockHierarchyEntity2D();
			var entityGivenPosition = new MockHierarchyEntity2D(new Vector2D(0.3f, 0.4f));
			var entityGivenRect = new MockHierarchyEntity2D(new Rectangle(0.1f, 0.0f, 0.5f, 0.4f), 20);
			entityGivenPosition.Parent = basicEntity;
			entityGivenRect.Parent = basicEntity;
			Assert.AreEqual(basicEntity, entityGivenPosition.Parent);
			Assert.AreEqual(new Vector2D(0.3f, 0.4f), entityGivenPosition.LocalPosition);
			Assert.AreEqual(basicEntity, entityGivenRect.Parent);
			Assert.AreEqual(20, entityGivenRect.LocalRotation);
		}

		[Test]
		public void SettingLocalWithoutParentSetsGlobal()
		{
			var entity = new MockHierarchyEntity2D();
			var setPosition = new Vector2D(0.2f,0.5f);
			const float SetRotation = 30.0f;
			entity.LocalPosition = setPosition;
			entity.LocalRotation = SetRotation;
			Assert.AreEqual(setPosition, entity.Position);
			Assert.AreEqual(SetRotation, entity.Rotation);
		}

		[Test]
		public void GetChildAndRemove()
		{
			var parentEntity = new MockHierarchyEntity2D();
			parentEntity.AddChild(new MockHierarchyEntity2D());
			var childEntity = (MockHierarchyEntity2D)parentEntity.GetFirstChildOfType<MockHierarchyEntity2D>();
			childEntity.Parent = null;
			Assert.AreEqual(0, parentEntity.Children.Count);
		}

		[Test]
		public void NoChildrenGivesNullWhenSearching()
		{
			var parentEntity = new MockHierarchyEntity2D();
			Assert.IsNull(parentEntity.GetFirstChildOfType<MockHierarchyEntity2D>());
		}

		[Test]
		public void SetActiveAlsoSetsForChildren()
		{
			var parentEntity = new MockHierarchyEntity2D();
			var childEntity = new MockHierarchyEntity2D();
			parentEntity.AddChild(childEntity);
			parentEntity.IsActive = false;
			Assert.AreEqual(parentEntity.IsActive, childEntity.IsActive);
		}
	}
}