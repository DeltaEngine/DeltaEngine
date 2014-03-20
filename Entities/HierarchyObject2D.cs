using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Entities
{
	public interface HierarchyObject2D
	{
		Vector2D Position { get; set; }

		float Rotation { get; set; }

		Vector2D LocalPosition { get; set; }

		float LocalRotation { get; set; }

		HierarchyObject2D Parent { get; set; }

		// ReSharper disable ReturnTypeCanBeEnumerable.Global
		List<HierarchyObject2D> Children { get; }
		// ReSharper restore ReturnTypeCanBeEnumerable.Global

		void AddChild(HierarchyObject2D child);

		void RemoveChild(HierarchyObject2D child);

		void UpdateGlobalsFromParent();

		object GetFirstChildOfType<T>() where T : HierarchyObject2D;

		bool IsActive { get; set; }
	}
}
