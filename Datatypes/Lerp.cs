namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Base interface to use the generic Lerp, do not use this interface on its own. Used to find
	/// out if some object implements the generic Lerp without having to go through reflection.
	/// </summary>
	public interface Lerp {}

	/// <summary>
	/// Forces datatypes derived from this to implement a Lerp method to interpolate between values.
	/// </summary>
	public interface Lerp<T> : Lerp
	{
		T Lerp(T other, float interpolation);
	}
}