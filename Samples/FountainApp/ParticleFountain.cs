using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering3D.Particles;

namespace FountainApp
{
	public class ParticleFountain
	{
		public ParticleFountain(Vector2D position)
		{
			new ParticleEmitter(EmitterData, position);
			CreateCommands();
		}

		private static ParticleEmitterData EmitterData
		{
			get
			{
				return emitterData =
						new ParticleEmitterData
						{
							StartVelocity =
								new RangeGraph<Vector3D>(new Vector2D(0.0f, -1.0f), new Vector2D(0.5f, 0.1f)),
							Acceleration = new RangeGraph<Vector3D>(new Vector2D(0, 0.9f), new Vector2D(0, 0.9f)),
							LifeTime = 1.0f,
							MaximumNumberOfParticles = 512,
							Size = new RangeGraph<Size>(new Size(0.01f), new Size(0.015f)),
							ParticleMaterial = new Material(ShaderFlags.Position2DColoredTextured, "Particle"),
							SpawnInterval = 0.01f,
							Color = new RangeGraph<Color>(Color.Red, Color.Orange)
						};
			}
		}

		private static ParticleEmitterData emitterData;

		private static void CreateCommands()
		{
			new Command(() =>
			{
				emitterData.StartVelocity =
					new RangeGraph<Vector3D>(
						emitterData.StartVelocity.Start + new Vector2D(0, -0.3f) * Time.Delta,
						emitterData.StartVelocity.End + new Vector2D(0, -0.3f) * Time.Delta);
			}).Add(new KeyTrigger(Key.CursorUp, State.Pressed));
			new Command(() =>
			{
				emitterData.StartVelocity =
					new RangeGraph<Vector3D>(
						emitterData.StartVelocity.Start + new Vector2D(0, 0.3f) * Time.Delta,
						emitterData.StartVelocity.End + new Vector2D(0, 0.3f) * Time.Delta);
			}).Add(new KeyTrigger(Key.CursorDown, State.Pressed));
			new Command(() =>
			{
				emitterData.Acceleration =
					new RangeGraph<Vector3D>(
						emitterData.Acceleration.Start + new Vector2D(0, -0.3f) * Time.Delta,
						emitterData.Acceleration.Start + new Vector2D(0, -0.3f) * Time.Delta);
			}).Add(new KeyTrigger(Key.CursorLeft, State.Pressed));
			new Command(() =>
			{
				emitterData.Acceleration =
					new RangeGraph<Vector3D>(
						emitterData.Acceleration.Start + new Vector2D(0, 0.3f) * Time.Delta,
						emitterData.Acceleration.Start + new Vector2D(0, 0.3f) * Time.Delta);
			}).Add(new KeyTrigger(Key.CursorRight, State.Pressed));
		}
	}
}