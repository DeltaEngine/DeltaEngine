using DeltaEngine.Scenes.Controls;

namespace CreepyTowers.GUI
{
	public class AvatarSlot
	{
		public Content.Avatars AvatarName { get; set; }
		public Picture LockImage { get; set; }
		public Picture GemImage { get; set; }
		public Picture IconImage { get; set; }
		public bool IsVisible
		{
			get { return isVisible; }
			set
			{
				isVisible = value;
				LockImage.IsVisible = isVisible;
				GemImage.IsVisible = isVisible;
				IconImage.IsVisible = isVisible;
			}
		}
		private bool isVisible;
	}
}