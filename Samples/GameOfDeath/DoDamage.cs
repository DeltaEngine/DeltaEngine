using System.Collections.Generic;
using DeltaEngine.Entities;
using GameOfDeath.Items;

namespace GameOfDeath
{
	/// <summary>
	/// Applies the damage from an item if it is time to do so
	/// </summary>
	public class DoDamage : UpdateBehavior
	{
		public override void Update(IEnumerable<Entity> entities)
		{
			foreach (var entity in entities)
			{
				var itemEffect = entity as ItemEffect;
				if (!itemEffect.IsDamageOverTime)
				{
					itemEffect.DoDamage();
					itemEffect.Stop<DoDamage>();
					return;
				}
				if (itemEffect.IsVisible && Time.CheckEvery(itemEffect.DamageInterval))
					itemEffect.DoDamage();
			}
		}
	}
}