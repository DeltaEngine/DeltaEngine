using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Entities
{
	public class HierarchyObjectTests
	{
		//[SetUp]
		//public void InitializeEntitiesRunner()
		//{
		//	new MockEntitiesRunner(typeof(MockUpdateBehavior), typeof(ComponentTests.Rotate),
		//		typeof(EntityTests.CreateEntityStartAndStopBehavior));
		//	hierarchyObject3D = new SimpleHierarchyObject3D();
		//	var child = new SimpleHierarchyObject3D();
		//	hierarchyObject3D.AddChild(child);
		//}

		//private SimpleHierarchyObject3D hierarchyObject3D;

		//private class SimpleHierarchyObject3D : HierarchyObject3D
		//{
		//	public SimpleHierarchyObject3D() {}

		//	public SimpleHierarchyObject3D(Vector3D position) {}
		//	public SimpleHierarchyObject3D(Quaternion rotation) {}

		//	public Vector3D Position { get; set; }
		//	public Quaternion Orientation { get; set; }
		//	public Vector3D LocalPosition { get; set; }
		//	public Quaternion LocalOrientation { get; set; }
		//	public HierarchyObject3D Parent { get; set; }
		//	public List<HierarchyObject3D> Children { get; set; }
		//	public void AddChild(HierarchyObject3D child) {}
		//	public void UpdateGlobalsFromParent() {}

		//	public object GetFirstChildOfType<T>() where T : HierarchyObject3D
		//	{
		//		return null;
		//	}
		//}

		//[Test]
		//public void CreateHierarchyObjectWithPosition()
		//{
		//	var unitXHierarchyObject = new SimpleHierarchyObject3D(Vector3D.UnitX);
		//	Assert.AreEqual(Vector3D.UnitX, unitXHierarchyObject.Position);
		//}

		//[Test]
		//public void ChangeHierarchyObjectPosition()
		//{
		//	hierarchyObject3D.Position = new Vector3D(1.0f, 1.0f, 1.0f);
		//	Assert.AreEqual(Vector3D.One, hierarchyObject3D.Position);
		//}

		//[Test]
		//public void CreateHierarchyObjectWithRotation()
		//{
		//	var axis = Vector3D.UnitY;
		//	const float Angle = 180.0f;
		//	var unitXHierarchyObject = new SimpleHierarchyObject3D(Quaternion.FromAxisAngle(axis, Angle));
		//	Assert.AreEqual(Vector3D.UnitY, unitXHierarchyObject.Orientation.Vector3D);
		//}

		//[Test]
		//public void ChangeHierarchyObjectRotation()
		//{
		//	var axis = Vector3D.UnitX;
		//	const float Angle = 90.0f;
		//	float invSqr2 = MathExtensions.InvSqrt(2);
		//	hierarchyObject3D.Orientation = Quaternion.FromAxisAngle(axis, Angle);
		//	Assert.AreEqual(new Vector3D(invSqr2, 0, 0), hierarchyObject3D.Orientation.Vector3D);
		//}

		//[Test]
		//public void AddChildrenToHierarchyObject()
		//{
		//	Assert.IsTrue(hierarchyObject3D.Children.Count == 1);
		//	hierarchyObject3D.AddChild(new SimpleHierarchyObject3D());
		//	Assert.IsTrue(hierarchyObject3D.Children.Count == 2);
		//	hierarchyObject3D.Children[0].AddChild(new SimpleHierarchyObject3D());
		//}

		//[Test]
		//public void ModifyPropertiesAlsoModifiesChildsHierarchy()
		//{
		//	hierarchyObject3D.Children[0].AddChild(new SimpleHierarchyObject3D());
		//	hierarchyObject3D.Position = Vector3D.One;
		//	Assert.AreEqual(hierarchyObject3D.Children[0].Children[0].Position, Vector3D.One);
		//}

		//[Test]
		//public void ChangePropertiesToObjectsWithChildren()
		//{
		//	hierarchyObject3D.Position = new Vector3D(1.0f, 1.0f, 1.0f);
		//	Assert.AreEqual(Vector3D.One, hierarchyObject3D.Children[0].Position);
		//	hierarchyObject3D.Orientation = Quaternion.FromAxisAngle(Vector3D.UnitY, 180.0f);
		//	Assert.AreEqual(Vector3D.UnitY, hierarchyObject3D.Children[0].Orientation.Vector3D);
		//}

		//[Test]
		//public void GetChildrenForNonExixtingTypeGivesNull()
		//{
		//	var firstChildOfType = hierarchyObject3D.GetFirstChildOfType<EmptyHierarchyObject3D>();
		//	Assert.IsNull(firstChildOfType);
		//}

		//private class EmptyHierarchyObject3D : HierarchyObject3D
		//{
		//	public Vector3D Position { get; set; }
		//	public Quaternion Orientation { get; set; }
		//	public Vector3D LocalPosition { get; set; }
		//	public Quaternion LocalOrientation { get; set; }
		//	public HierarchyObject3D Parent { get; set; }
		//	public List<HierarchyObject3D> Children { get; set; }
		//	public void AddChild(HierarchyObject3D child) {}
		//	public void UpdateGlobalsFromParent() {}

		//	public object GetFirstChildOfType<T>() where T : HierarchyObject3D
		//	{
		//		return null;
		//	}
		//}

		//[Test]
		//public void ChangePropertiesToFirstChildrenOfType()
		//{
		//	var secondChild = new EmptyHierarchyObject3D();
		//	hierarchyObject3D.AddChild(secondChild);
		//	hierarchyObject3D.Position = Vector3D.One;
		//	var firstChildOfType =
		//		(EmptyHierarchyObject3D)hierarchyObject3D.GetFirstChildOfType<EmptyHierarchyObject3D>();
		//	Assert.AreEqual(Vector3D.One, firstChildOfType.Position);
		//}
	}
}