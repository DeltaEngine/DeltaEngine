using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering3D.Shapes
{
	/// <summary>
	/// A line in 3D space. Just a start and end point plus a color, but you can add more points.
	/// </summary>
	public class Line3D : Entity3D
	{
		public Line3D(Vector3D start, Vector3D end, Color color)
			: base(Vector3D.Zero)
		{
			Add(color);
			Add(new List<Vector3D> { start, end });
			OnDraw<Line3DRenderer>();
		}

		public List<Vector3D> Points
		{
			get { return Get<List<Vector3D>>(); }
			set { Set(value); }
		}

		public Vector3D StartPoint
		{
			get { return Points[0]; }
			set { Points[0] = value; }
		}

		public Vector3D EndPoint
		{
			get { return Points[1]; }
			set { Points[1] = value; }
		}
	}
}
