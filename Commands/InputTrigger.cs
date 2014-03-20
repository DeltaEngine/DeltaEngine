namespace DeltaEngine.Commands
{
	public abstract class InputTrigger : Trigger
	{
		protected InputTrigger()
		{
			// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			StartInputDevice();
		}

		protected abstract void StartInputDevice();

		protected const float PositionEpsilon = 0.0025f;
	}
}