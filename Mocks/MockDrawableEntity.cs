using System.Collections.Generic;
using DeltaEngine.Entities;

namespace DeltaEngine.Mocks
{
	/// <summary>
	/// A drawable entity that does nothing; useful for unit testing.
	/// </summary>
	public class MockDrawableEntity : DrawableEntity
	{
		public List<object> GetLastTickLerpComponents()
		{
			return lastTickLerpComponents;
		}
	}
}