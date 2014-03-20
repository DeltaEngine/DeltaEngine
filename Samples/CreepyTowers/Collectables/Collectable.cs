using CreepyTowers.Towers;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;

namespace CreepyTowers.Collectables
{
	/// <summary>
	/// Sits in the grid and can be collected by the player; eg. a coin
	/// Can also do damage to nearby creeps
	/// </summary>
	public abstract class Collectable : Actor3D
	{
		protected Collectable(Vector3D position)
			: base(position) {}

		public TowerType DamageType { get; set; }
		public float DamageRangeSquared { get { return DamageRange * DamageRange; } }
		public float DamageRange { get; set; }
		public float Damage { get; set; }
	}
}