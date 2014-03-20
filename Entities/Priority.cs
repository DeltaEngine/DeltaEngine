namespace DeltaEngine.Entities
{
	/// <summary>
	/// Entity behaviors are processed in order from First priority to Last. Normal is the default.
	/// </summary>
	public enum Priority : byte
	{
		First,
		High,
		Normal,
		Low,
		Last
	}
}