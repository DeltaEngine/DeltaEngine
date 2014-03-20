using System.Linq;
using DeltaEngine.Datatypes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace DeltaEngine.Physics2D.Farseer
{
	/// <summary>
	/// Convert between display and simulation units and between Farseer and Delta Engine datatypes 
	/// </summary>
	internal class UnitConverter
	{
		public UnitConverter(float displayUnitsToSimUnits)
		{
			this.displayUnitsToSimUnits = displayUnitsToSimUnits;
			simUnitsToDisplayUnits = 1.0f / displayUnitsToSimUnits;
		}

		private readonly float displayUnitsToSimUnits;
		private readonly float simUnitsToDisplayUnits;

		public Vector2 ToDisplayUnits(Vector2 simUnits)
		{
			return simUnits * simUnitsToDisplayUnits;
		}

		public float ToSimUnits(float displayUnits)
		{
			return displayUnits * displayUnitsToSimUnits;
		}

		public Vector2 ToSimUnits(Vector2 displayUnits)
		{
			return displayUnits * displayUnitsToSimUnits;
		}

		public Vector2D Convert(Vector2 value)
		{
			return new Vector2D(value.X, value.Y);
		}

		public Vector2 Convert(Vector2D value)
		{
			return new Vector2(value.X, value.Y);
		}

		public Vertices Convert(params Vector2D[] vertices)
		{
			var farseerVertices = new Vertices(vertices.Length);
			farseerVertices.AddRange(vertices.Select(t => ToSimUnits(Convert(t))));
			return farseerVertices;
		}
	}
}