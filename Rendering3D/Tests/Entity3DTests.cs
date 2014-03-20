using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Tests
{
	public class Entity3DTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void CreateEntity3D()
		{
			var entity = new Entity3D(Vector3D.Zero);
			entity.Add(Rectangle.One);
			Assert.AreEqual(Vector3D.Zero, entity.Position);
			Assert.AreEqual(Quaternion.Identity, entity.Orientation);
			Assert.IsTrue(entity.IsVisible);
			Assert.AreEqual(4, entity.GetComponentsForSaving().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateEntity3DPositionAndOrientation()
		{
			var position = new Vector3D(10.0f, -3.0f, 27.0f);
			var orientation = Quaternion.Identity;
			var entity = new Entity3D(position, orientation);
			Assert.AreEqual(position, entity.Position);
			Assert.AreEqual(orientation, entity.Orientation);
		}

		[Test, CloseAfterFirstFrame]
		public void SetAndGetEntity3DComponentsDirectly()
		{
			var entity = new Entity3D(Vector3D.Zero);
			entity.Set(Vector3D.One);
			Assert.AreEqual(Vector3D.One, entity.Get<Vector3D>());
			entity.Set(Quaternion.Identity);
			Assert.AreEqual(Quaternion.Identity, entity.Get<Quaternion>());
		}

		[Test, CloseAfterFirstFrame]
		public void CannotAddTheSameTypeOfComponentTwice()
		{
			var entity = new Entity3D(Vector3D.Zero);
			Assert.Throws<Entity.ComponentOfTheSameTypeAddedMoreThanOnce>(() => entity.Add(Vector3D.One));
		}

		[Test, CloseAfterFirstFrame]
		public void SetPositionProperty()
		{
			var entity = new Entity3D(Vector3D.Zero) { Position = Vector3D.One };
			Assert.AreEqual(Vector3D.One, entity.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void SetOrientationProperty()
		{
			var entity = new Entity3D(Vector3D.Zero) { Orientation = Quaternion.Identity };
			Assert.AreEqual(Quaternion.Identity, entity.Orientation);
		}

		[Test, CloseAfterFirstFrame]
		public void SettingPositionWithoutInterpolationSetsLastPositionAlso()
		{
			var entity = new MockEntity3D(Vector3D.One);
			Assert.AreEqual(Vector3D.One, entity.GetLastPosition());
			entity.SetWithoutInterpolation(Vector3D.UnitX);
			Assert.AreEqual(Vector3D.UnitX, entity.Position);
			Assert.AreEqual(Vector3D.UnitX, entity.GetLastPosition());
		}

		[Test, CloseAfterFirstFrame]
		public void SettingOrientationWithoutInterpolationSetsLastOrientationAlso()
		{
			var entity = new MockEntity3D(Vector3D.Zero, Orientation1);
			Assert.AreEqual(Orientation1, entity.GetLastOrientation());
			entity.SetWithoutInterpolation(Orientation2);
			Assert.AreEqual(Orientation2, entity.Orientation);
			Assert.AreEqual(Orientation2, entity.GetLastOrientation());
		}

		private static readonly Quaternion Orientation1 = Quaternion.FromAxisAngle(Vector3D.UnitX, 90);
		private static readonly Quaternion Orientation2 = Quaternion.FromAxisAngle(Vector3D.UnitY, 90);

		[Test, CloseAfterFirstFrame]
		public void SettingFloatWithoutInterpolationSetsLastFloatAlso()
		{
			var entity = new MockEntity3D(Vector3D.Zero);
			entity.Add(90.0f);
			Assert.AreEqual(90.0f, entity.GetLastTickLerpComponents()[0]);
			entity.SetWithoutInterpolation(180.0f);
			Assert.AreEqual(180.0f, entity.Get<float>());
			Assert.AreEqual(180.0f, entity.GetLastTickLerpComponents()[0]);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateLookAtCameraIfCurrentCamNull()
		{
			Camera.Current = null;
			new Entity3D(Vector3D.Zero);
			Assert.AreEqual(typeof(LookAtCamera), Camera.Current.GetType());
		}
	}
}