namespace DeltaEngine.Entities
{
	/// <summary>
	/// Adding this interface to an Entity will cause its Update method to get called at, by default,
	/// 20 times per second, at the given UpdatePriority for each active entity.
	/// If IsPauseable returns true, when the app is paused Update will not get called.
	/// </summary>
	public interface Updateable
	{
		void Update();
	}
}