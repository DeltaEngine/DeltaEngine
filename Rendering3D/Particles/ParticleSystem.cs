using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering3D.Particles
{
	public class ParticleSystem : HierarchyEntity3D
	{
		public ParticleSystem()
			: base(Vector3D.Zero)
		{
			AttachedEmitters = new List<ParticleEmitter>();
		}

		public ParticleSystem(ParticleSystemData emittersToLoad)
			: base(Vector3D.Zero)
		{
			AttachedEmitters = new List<ParticleEmitter>();
			foreach (string emitterName in emittersToLoad.emitterNames)
				AttachEmitter(new ParticleEmitter(ContentLoader.Load<ParticleEmitterData>(emitterName),
					Position));
		}

		public List<ParticleEmitter> AttachedEmitters { get; private set; }

		protected override void OnPositionChanged()
		{
			if (AttachedEmitters == null || AttachedEmitters.Count == 0)
				return;
			foreach (ParticleEmitter attachedEmitter in AttachedEmitters)
				attachedEmitter.Position = Position;
			base.OnPositionChanged();
		}

		protected override void OnOrientationChanged()
		{
			if (AttachedEmitters == null || AttachedEmitters.Count == 0)
				return;
			foreach (ParticleEmitter attachedEmitter in AttachedEmitters)
				attachedEmitter.Rotation = Orientation;
			base.OnOrientationChanged();
		}

		public void AttachEmitter(ParticleEmitter emitter)
		{
			AttachedEmitters.Add(emitter);
			emitter.Position = Position;
			emitter.Rotation = Orientation;
		}

		public void RemoveEmitter(ParticleEmitter emitter)
		{
			AttachedEmitters.Remove(emitter);
		}

		public void RemoveEmitter(int indexOfEmitter)
		{
			AttachedEmitters.RemoveAt(indexOfEmitter);
		}

		public void DisposeEmitter(ParticleEmitter emitter)
		{
			AttachedEmitters.Remove(emitter);
			emitter.IsActive = false;
		}

		public void DisposeEmitter(int indexOfEmitter)
		{
			var emitter = AttachedEmitters[indexOfEmitter];
			AttachedEmitters.RemoveAt(indexOfEmitter);
			emitter.IsActive = false;
		}

		public void DisposeSystem()
		{
			foreach (var attachedEmitter in AttachedEmitters)
				attachedEmitter.IsActive = false;
			AttachedEmitters.Clear();
		}

		public void FireBurstOfAllEmitters()
		{
			foreach (var emitter in AttachedEmitters)
				emitter.Spawn();
		}

		public void FireBurstAtRelativePosition(Vector3D position)
		{
			foreach (var emitter in AttachedEmitters)
				emitter.SpawnAtRelativePosition(position);
		}

		public void FireBurstAtRelativePositionAndRotation(Vector3D position, float rotation)
		{
			foreach (var emitter in AttachedEmitters)
				emitter.SpawnAtRelativePositionAndRotation(position, rotation);
		}

		public override bool IsActive
		{
			get { return base.IsActive; }
			set
			{
				if (!value && IsActive)
					DisposeSystem();
				base.IsActive = value;
			}
		}
	}
}