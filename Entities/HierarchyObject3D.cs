using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Entities
{
	public interface HierarchyObject3D
	{
		Vector3D Position { get; set; }

		Quaternion Orientation { get; set; }

		Vector3D LocalPosition { get; set; }

		Quaternion LocalOrientation { get; set; }

		HierarchyObject3D Parent { get; set; }

// ReSharper disable ReturnTypeCanBeEnumerable.Global
		List<HierarchyObject3D> Children { get; }
// ReSharper restore ReturnTypeCanBeEnumerable.Global

		void AddChild(HierarchyObject3D child);

		void UpdateGlobalsFromParent();

		object GetFirstChildOfType<T>() where T : HierarchyObject3D;

		bool IsActive { get; set; }
	}
}