using CreepyTowers.Content;
using CreepyTowers.GUI;
using Nightmare1 = CreepyTowers.Content.Nightmare1;

namespace CreepyTowers.Nightmares
{
	public class Nightmare1ChapterSelector
	{
		public static void SelectChapter(Nightmare1 chapterName)
		{
			switch (chapterName)
			{
			case (Nightmare1.ButtonChapter1):
				new KidsRoomChapter();
				break;

			case (Nightmare1.ButtonChapter2):
				new KidsRoomChapterV2();
				break;

			case (Nightmare1.ButtonChapter3):
				new LivingRoomChapter();
				break;

			default:
				new KidsRoomChapter();
				break;
			}
			ToggleMenuVisibility();
		}

		private static void ToggleMenuVisibility()
		{
			MenuController.Current.HideMenu(GameMenus.SceneNightmare1);
			MenuController.Current.ShowMenu(GameMenus.SceneGameHud);
			var hud = (Hud)MenuController.Current.GetMenu(GameMenus.SceneGameHud);
			hud.UpdateLevelValuesHud();
		}
	}
}