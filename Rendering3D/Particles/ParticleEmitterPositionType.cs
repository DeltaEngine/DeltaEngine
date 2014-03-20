namespace DeltaEngine.Rendering3D.Particles
{
	/// <summary>
	/// By default emitters come out of the position of the emitter. Use one of the other types
	/// defined here to come out of a line, box, sphere or even mesh, or circle around the position.
	/// </summary>
	public enum ParticleEmitterPositionType
	{
		Point,
		Line,
		Box,
		Sphere,
		SphereBorder,
		CircleAroundCenter,
		CircleEscaping,
		Mesh
	}
}