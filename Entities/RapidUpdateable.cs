namespace DeltaEngine.Entities
{
	/// <summary>
	/// Certain entities need to be updated more often than normally - eg. Physics. RapidUpdate is 
	/// called 60 times per second by default (no matter how fast the game runs).
	/// If IsPauseable returns true, when the app is paused RapidUpdate will not get called.
	/// </summary>
	public interface RapidUpdateable
	{
		void RapidUpdate();
	}
}