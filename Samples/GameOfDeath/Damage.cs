using System;

namespace GameOfDeath
{
	/// <summary>
	/// Holds how much and how often an item does damage
	/// </summary>
	public class Damage
	{
		public float Interval { get; set; }
		public Action DoDamage { get; set; }
	}
}