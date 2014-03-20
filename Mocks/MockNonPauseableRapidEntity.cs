using DeltaEngine.Entities;

namespace DeltaEngine.Mocks
{
	/// <summary>
	/// RapidUpdateable Entity that does nothing. For unit testing.
	/// </summary>
	public class MockNonPauseableRapidEntity : Entity, RapidUpdateable, VerifiableUpdate
	{
		public void RapidUpdate()
		{
			WasUpdated = true;
		}

		public bool WasUpdated { get; set; }

		public override bool IsPauseable { get { return false; } }
	}
}