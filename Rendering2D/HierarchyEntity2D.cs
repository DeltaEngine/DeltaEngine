using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering2D
{
	public class HierarchyEntity2D : Entity2D, HierarchyObject2D
	{
		protected HierarchyEntity2D()
			: this(Rectangle.Zero) {}

		protected HierarchyEntity2D(Vector2D position)
			: this(Rectangle.FromCenter(position, Size.Zero)) {}

		protected HierarchyEntity2D(Rectangle drawArea, float orientation = 0.0f)
			: base(drawArea)
		{
			Children = new List<HierarchyObject2D>();
			Rotation = orientation;
			localPosition = Vector2D.Zero;
			localRotation = 0;
		}

		public Vector2D Position
		{
			get { return DrawArea.Center; }
			set
			{
				DrawArea = Rectangle.FromCenter(value, DrawArea.Size);
				UpdateChildren();
			}
		}

		public Vector2D LocalPosition
		{
			get { return localPosition; }
			set
			{
				localPosition = value;
				UpdateGlobalsFromParent();
			}
		}

		private Vector2D localPosition;

		public float LocalRotation
		{
			get { return localRotation; }
			set
			{
				localRotation = value;
				UpdateGlobalsFromParent();
			}
		}

		private float localRotation;

		public HierarchyObject2D Parent
		{
			get { return parent; }
			set
			{
				if (value == null && parent != null)
					parent.RemoveChild(this);
				parent = value;
				localPosition = Position;
				localRotation = Rotation;
				UpdateGlobalsFromParent();
			}
		}

		private HierarchyObject2D parent;

		public List<HierarchyObject2D> Children { get; private set; }

		public void AddChild(HierarchyObject2D child)
		{
			Children.Add(child);
			child.Parent = this;
		}

		public void RemoveChild(HierarchyObject2D child)
		{
			if (Children != null)
				Children.Remove(child);
		}

		public void UpdateGlobalsFromParent()
		{
			if (parent == null)
			{
				Position = localPosition;
				Rotation = localRotation;
			}
			else
			{
				Position = parent.Position + localPosition;
				Rotation = parent.Rotation + localRotation;
			}
			foreach (var child in Children)
				child.UpdateGlobalsFromParent();
		}

		public object GetFirstChildOfType<T>() where T : HierarchyObject2D
		{
			for (int i = 0; i < Children.Count; i++)
				if (Children[i].GetType() == typeof(T))
					return (T)Children[i];
			return null;
		}

		protected override void OnPositionChanged()
		{
			UpdateChildren();
			base.OnPositionChanged();
		}

		protected override void OnRotationChanged()
		{
			UpdateChildren();
			base.OnRotationChanged();
		}

		private void UpdateChildren()
		{
			if (Children == null)
				return;
			foreach (var child in Children)
				child.UpdateGlobalsFromParent();
		}

		public override bool IsActive
		{
			get { return base.IsActive; }
			set
			{
				foreach (var child in Children)
					child.IsActive = value;
				base.IsActive = value;
			}
		}
	}
}