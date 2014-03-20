using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering3D.Cameras;

namespace DeltaEngine.Rendering3D
{
	/// <summary>
	/// Base entity for 3D objects. Usually used in Meshes or Models, both normally Actors.
	/// </summary>
	public class Entity3D : DrawableEntity
	{
		public Entity3D(Vector3D position)
			: this(position, Quaternion.Identity) {}

		public Entity3D(Vector3D position, Quaternion orientation)
		{
			lastPosition = Position = position;
			lastOrientation = Orientation = orientation;
			Scale = Vector3D.One;
			if (Camera.Current == null)
				Camera.Use<LookAtCamera>(); //ncrunch: no coverage
		}

		public Vector3D Position
		{
			get { return position; }
			set
			{
				position = value;
				OnPositionChanged();
			}
		}

		private Vector3D position;

		protected virtual void OnPositionChanged() {}

		public Quaternion Orientation
		{
			get { return orientation; }
			set
			{
				orientation = value;
				OnOrientationChanged();
			}
		}

		private Quaternion orientation;

		protected virtual void OnOrientationChanged() {}

		public Vector3D Scale
		{
			get { return scale; }
			set
			{
				scale = value;
				OnScaleChanged();
			}
		}

		private Vector3D scale;

		protected virtual void OnScaleChanged() {}

		protected internal override List<object> GetComponentsForSaving()
		{
			var componentsForSaving = new List<object> { Position, Orientation, Scale };
			foreach (var component in base.GetComponentsForSaving())
				if (!(component is Vector3D) && !(component is Quaternion) && !(component is int))
					componentsForSaving.Add(component);
			return componentsForSaving;
		}

		protected override void NextUpdateStarted()
		{
			lastPosition = Position;
			lastOrientation = Orientation;
			base.NextUpdateStarted();
		}

		protected Vector3D lastPosition;
		protected Quaternion lastOrientation;

		public override sealed T Get<T>()
		{
			float interpolation = EntitiesRunner.CurrentDrawInterpolation;
			if (EntitiesRunner.Current.State == UpdateDrawState.Draw && typeof(T) == typeof(Vector3D))
				return (T)(object)lastPosition.Lerp(Position, interpolation);
			if (EntitiesRunner.Current.State == UpdateDrawState.Draw && typeof(T) == typeof(Quaternion))
				return (T)(object)lastOrientation.Lerp(Orientation, interpolation); //ncrunch: no coverage
			if (typeof(T) == typeof(Vector3D))
				return (T)(object)Position;
			if (typeof(T) == typeof(Quaternion))
				return (T)(object)Orientation;
			return base.Get<T>();
		}

		public override sealed bool Contains<T>()
		{
			return typeof(T) == typeof(Vector3D) || typeof(T) == typeof(Quaternion) ||
				base.Contains<T>();
		}

		public override sealed Entity Add<T>(T component)
		{
			if (typeof(T) == typeof(Vector3D) || typeof(T) == typeof(Quaternion))
				throw new ComponentOfTheSameTypeAddedMoreThanOnce();
			return base.Add(component);
		}

		public override void SetWithoutInterpolation<T>(T component)
		{
			if (typeof(T) == typeof(Vector3D))
			{
				EntitiesRunner.Current.CheckIfInUpdateState();
				Position = (Vector3D)(object)component;
				lastPosition = Position;
			}
			else if (typeof(T) == typeof(Quaternion))
			{
				EntitiesRunner.Current.CheckIfInUpdateState();
				Orientation = (Quaternion)(object)component;
				lastOrientation = Orientation;
			}
			else
				base.SetWithoutInterpolation(component);
		}

		public override void Set(object component)
		{
			if (component is Vector3D)
			{
				EntitiesRunner.Current.CheckIfInUpdateState();
				Position = (Vector3D)component;
			}
			else if (component is Quaternion)
			{
				EntitiesRunner.Current.CheckIfInUpdateState();
				Orientation = (Quaternion)component;
			}
			else
				base.Set(component);
		}
	}
}