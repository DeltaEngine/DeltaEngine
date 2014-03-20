using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering3D.Tests
{
	internal class MockEntity3D : Entity3D
	{
		public MockEntity3D(Vector3D position)
			: base(position) {}

		public MockEntity3D(Vector3D position, Quaternion orientation)
			: base(position, orientation) {}

		public Vector3D GetLastPosition()
		{
			return lastPosition;
		}

		public Quaternion GetLastOrientation()
		{
			return lastOrientation;
		}

		public List<object> GetLastTickLerpComponents()
		{
			return lastTickLerpComponents;
		}
	}
}