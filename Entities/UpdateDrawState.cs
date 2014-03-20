namespace DeltaEngine.Entities
{
	/// <summary>
	/// The type of processing EntitiesRunner is currently undertaking.
	/// </summary>
	public enum UpdateDrawState
	{
		Initialization,
		RapidUpdate,
		Update,
		Draw,
		None,
	}
}