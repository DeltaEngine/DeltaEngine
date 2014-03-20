using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Handles the angle of rotation of a Game of Death Mallet
	/// </summary>
	public class RotateMallet : UpdateBehavior
	{
		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (var mallet in entities.OfType<Mallet>())
			{
				var rotation = mallet.Rotation;
				var rotationAdjust = RotationSpeed * Time.Delta;
				if (rotation < 0 - rotationAdjust)
					mallet.Rotation = rotation + rotationAdjust;
				else
					mallet.Rotation = 0;
			}
		}

		private const float RotationSpeed = 300;
	}
}