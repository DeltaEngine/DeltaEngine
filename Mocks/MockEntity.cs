using DeltaEngine.Entities;

namespace DeltaEngine.Mocks
{
	/// <summary>
	/// Updateable Entity that does nothing. For unit testing.
	/// </summary>
	public class MockEntity : Entity, Updateable, VerifiableUpdate
	{
		public void Update()
		{
			WasUpdated = true;
		}

		public bool WasUpdated { get; set; }
	}
}