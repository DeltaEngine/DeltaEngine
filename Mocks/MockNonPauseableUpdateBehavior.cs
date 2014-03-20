using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Entities;

namespace DeltaEngine.Mocks
{
	/// <summary>
	/// A non-pauseable update behavior that does nothing during a unit test.
	/// </summary>
	public class MockNonPauseableUpdateBehavior : UpdateBehavior
	{
		public MockNonPauseableUpdateBehavior()
			: base(Priority.Normal, false) {}

		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (var verifable in entities.OfType<VerifiableUpdate>())
				verifable.WasUpdated = true;
		}
	}
}