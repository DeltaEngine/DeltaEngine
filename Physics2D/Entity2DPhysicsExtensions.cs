using System;
using System.Collections.Generic;
using System.Text;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Physics2D
{
	/// <summary>
	/// Entity2D extensions for accelerated usage of 2D physics
	/// </summary>
	public static class Entity2DPhysicsExtensions
	{
		public static void AffixToPhysics(this Entity2D entity, PhysicsBody body)
		{
			entity.Add(body);
			entity.Start<AffixToPhysics2D>();
		}
	}
}
