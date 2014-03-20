using System.Collections.Generic;
using DeltaEngine.Content;

namespace $safeprojectname$.GUI
{
	public class AvatarSelectionMaterials
	{
		public AvatarSelectionMaterials()
		{
			AvatarIconsGreyMaterials = new List<Material>();
			AvatarIconsMaterials = new List<Material>();
			AvatarImageMaterials = new List<Material>();
			AvatarInfoMaterials = new List<Material>();
			InitializeMaterialsForAllGreyAvatarIcons();
			InitializeMaterialsForAllAvatarIcons();
			InitializeMaterialsForAvatarImages();
			InitializeMaterialsForAvatarInfo();
		}

		public List<Material> AvatarIconsGreyMaterials { get; private set; }
		public List<Material> AvatarIconsMaterials { get; private set; }
		public List<Material> AvatarImageMaterials { get; private set; }
		public List<Material> AvatarInfoMaterials { get; private set; }

		private void InitializeMaterialsForAllGreyAvatarIcons()
		{
			AvatarIconsGreyMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarIconsAlienGreyMat.ToString()));
			AvatarIconsGreyMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarIconsPenguinGreyMat.ToString()));
			AvatarIconsGreyMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarIconsDragonGreyMat.ToString()));
			AvatarIconsGreyMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarIconsPiggyBankGreyMat.ToString()));
			AvatarIconsGreyMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarIconsPandaGreyMat.ToString()));
			AvatarIconsGreyMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarIconsTeddyGreyMat.ToString()));
			AvatarIconsGreyMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarIconsRobotGreyMat.ToString()));
			AvatarIconsGreyMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarIconsUnicornGreyMat.ToString()));
		}

		private void InitializeMaterialsForAllAvatarIcons()
		{
			AvatarIconsMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarIconsAlienMat.ToString()));
			AvatarIconsMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarIconsPenguinMat.ToString()));
			AvatarIconsMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarIconsDragonMat.ToString()));
			AvatarIconsMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarIconsPiggyBankMat.ToString()));
			AvatarIconsMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarIconsPandaMat.ToString()));
			AvatarIconsMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarIconsTeddyMat.ToString()));
			AvatarIconsMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarIconsRobotMat.ToString()));
			AvatarIconsMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarIconsUnicornMat.ToString()));
		}

		private void InitializeMaterialsForAvatarImages()
		{
			AvatarImageMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarSelectionAlienMat.ToString()));
			AvatarImageMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarSelectionPenguinMat.ToString()));
			AvatarImageMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarSelectionDragonMat.ToString()));
			AvatarImageMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarSelectionPiggyBankMat.ToString()));
			AvatarImageMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarSelectionPandaMat.ToString()));
			AvatarImageMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarSelectionTeddyMat.ToString()));
			AvatarImageMaterials.Add(
				ContentLoader.Load<Material>(Content.AvatarSelectionMenu.AvatarSelectionRobotMat.ToString()));
			AvatarImageMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarSelectionUnicornMat.ToString()));
		}

		private void InitializeMaterialsForAvatarInfo()
		{
			AvatarInfoMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarSelectionAlienInformationMat.ToString()));
			AvatarInfoMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarSelectionPenguinInformationMat.ToString()));
			AvatarInfoMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarSelectionDragonInformationMat.ToString()));
			AvatarInfoMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarSelectionPiggyInformationMat.ToString()));
			AvatarInfoMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarSelectionPandaInformationMat.ToString()));
			AvatarInfoMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarSelectionTeddyInformationMat.ToString()));
			AvatarInfoMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarSelectionRobotInformationMat.ToString()));
			AvatarInfoMaterials.Add(
				ContentLoader.Load<Material>(
					Content.AvatarSelectionMenu.AvatarSelectionUnicornInformationMat.ToString()));
		}
	}
}