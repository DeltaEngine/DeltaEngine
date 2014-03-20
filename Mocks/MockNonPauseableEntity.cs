using DeltaEngine.Entities;

namespace DeltaEngine.Mocks
{
	/// <summary>
	/// Updateable Entity that does nothing. For unit testing.
	/// </summary>
	public class MockNonPauseableEntity : Entity, Updateable, VerifiableUpdate
	{
		public void Update()
		{
			WasUpdated = true;
		}

		public bool WasUpdated { get; set; }

		public override bool IsPauseable { get { return false; } }
	}
}