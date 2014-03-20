using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Tests
{
	internal class MockEntity2D : Entity2D
	{
		public MockEntity2D(Rectangle drawArea)
			: base(drawArea) {}

		public List<object> GetLastTickLerpComponents()
		{
			return lastTickLerpComponents;
		}
	}
}