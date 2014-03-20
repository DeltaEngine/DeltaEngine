using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering3D
{
	public abstract class HierarchyEntity3D : Entity3D, HierarchyObject3D
	{
		protected HierarchyEntity3D(Vector3D position)
			: this(position, Quaternion.Identity) {}

		protected HierarchyEntity3D(Vector3D position, Quaternion orientation)
			: base(position, orientation)
		{
			Children = new List<HierarchyObject3D>();
			localOrientation = Quaternion.Identity;
			localPosition = Vector3D.Zero;
		}

		public HierarchyObject3D Parent
		{
			get { return parent; }
			set
			{
				parent = value;
				localPosition = Position;
				localOrientation = Orientation;
				UpdateGlobalsFromParent();
			}
		}

		private HierarchyObject3D parent;

		public List<HierarchyObject3D> Children { get; private set; }

		public Vector3D LocalPosition
		{
			get { return localPosition; }
			set
			{
				localPosition = value;
				UpdateGlobalsFromParent();
			}
		}

		private Vector3D localPosition;

		public Quaternion LocalOrientation
		{
			get { return localOrientation; }
			set
			{
				localOrientation = value;
				UpdateGlobalsFromParent();
			}
		}

		private Quaternion localOrientation;

		protected override void OnOrientationChanged()
		{
			UpdateChildren();
			base.OnOrientationChanged();
		}

		protected override void OnPositionChanged()
		{
			UpdateChildren();
			base.OnPositionChanged();
		}

		protected override void OnScaleChanged()
		{
			UpdateChildren();
			base.OnScaleChanged();
		}

		public void AddChild(HierarchyObject3D child)
		{
			Children.Add(child);
			child.Parent = this;
			child.UpdateGlobalsFromParent();
		}

		public void UpdateGlobalsFromParent()
		{
			if (parent == null)
			{
				Position = LocalPosition;
				Orientation = LocalOrientation;
			}
			else
			{
				Position = parent.Position + LocalPosition.Transform(Parent.Orientation);
				Orientation = parent.Orientation * LocalOrientation;
			}
			foreach (var child in Children)
				child.UpdateGlobalsFromParent();
		}

		private void UpdateChildren()
		{
			if (Children != null)
				foreach (var child in Children)
					child.UpdateGlobalsFromParent();
		}

		public object GetFirstChildOfType<T>() where T : HierarchyObject3D
		{
			for (int i = 0; i < Children.Count; i++)
				if (Children[i].GetType() == typeof(T))
					return (T)Children[i];
			return null; //ncrunch: no coverage
		}

		public override bool IsActive
		{
			get
			{
				return base.IsActive;
			}
			set
			{
				foreach (var child in Children)
				{
					child.IsActive = value;
				}
				base.IsActive = value;
			}
		}
	}
}