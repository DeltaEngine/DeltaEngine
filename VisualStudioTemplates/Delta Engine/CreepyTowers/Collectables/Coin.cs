using DeltaEngine.Datatypes;

namespace $safeprojectname$.Collectables
{
	/// <summary>
	/// Adds to player gold when collected
	/// </summary>
	public class Coin : Collectable
	{
		public Coin(Vector3D position, int gold)
			: base(position)
		{
			this.gold = gold;
		}

		private readonly int gold;

		public override void RenderModel() {}

		public void Collect()
		{
			Player.Current.Gold += gold;
			Dispose();
		}
	}
}