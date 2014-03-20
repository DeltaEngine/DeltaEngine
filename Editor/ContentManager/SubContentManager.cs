using System;
using System.Collections.ObjectModel;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Particles;

namespace DeltaEngine.Editor.ContentManager
{
	public class SubContentManager
	{
		public static void AddSubContentAndRemoveDuplicateContent(
			ObservableCollection<ContentIconAndName> setContentList, Service setService,
			ContentIconAndName content)
		{
			if (!setContentList.Contains(content))
				return;
			contentList = setContentList;
			service = setService;
			try
			{
				TryAddSubContentAndRemoveDuplicateContent(content);
			}
			catch (Exception ex)
			{
				Logger.Warning("Could not get the subcontent of " + content.Name + ": " + ex.Message);
			}
		}

		private static ObservableCollection<ContentIconAndName> contentList;
		private static Service service;

		private static void TryAddSubContentAndRemoveDuplicateContent(ContentIconAndName content)
		{
			if (service.GetTypeOfContent(content.Name) == ContentType.Model)
				GetModelSubContent(content);
			if (service.GetTypeOfContent(content.Name) == ContentType.Mesh)
				CreateMeshWithSubContent(content);
			if (service.GetTypeOfContent(content.Name) == ContentType.Material)
				CreateMaterialWithSubContent(content);
			if (service.GetTypeOfContent(content.Name) == ContentType.ImageAnimation)
				CreateImageAnimationWithSubContent(content);
			if (service.GetTypeOfContent(content.Name) == ContentType.SpriteSheetAnimation)
				CreateSpriteSheetWithSubContent(content);
			if (service.GetTypeOfContent(content.Name) == ContentType.Font)
				CreateFontWithSubContent(content);
			if (service.GetTypeOfContent(content.Name) == ContentType.ParticleSystem)
				CreateParticleSystemWithSubContent(content);
		}

		private static void GetModelSubContent(ContentIconAndName content)
		{
			var model = ContentLoader.Load<ModelData>(content.Name);
			var meshName = model.MetaData.Get("MeshNames", "mesh");
			var meshSubContent = new ObservableCollection<ContentIconAndName>();
			for (int i = 0; i < meshName.Split(new[] { ',', ' ' }).Length; i++)
			{
				var newMeshName = meshName.Split(new[] { ',', ' ' })[i];
				if (string.IsNullOrEmpty(newMeshName))
					continue;
				meshSubContent.Add(GetMeshSubContent(newMeshName, true));
			}
			var contentIndex = contentList.IndexOf(content);
			contentList.RemoveAt(contentIndex);
			contentList.Insert(contentIndex,
				new ContentIconAndName(content.Icon, content.Name, meshSubContent));
		}

		private static ContentIconAndName GetMeshSubContent(string meshName, bool isSubContent)
		{
			if (isSubContent)
				RemoveContentFromContentList(meshName);
			var mesh = ContentLoader.Load<Mesh>(meshName);
			var aditionalContent = new ObservableCollection<ContentIconAndName>();
			var geometryName = mesh.MetaData.Get("GeometryName", "geometry");
			aditionalContent.Add(GetGeometrySubContent(geometryName));
			var animationName = mesh.MetaData.Get("AnimationName", "animation");
			aditionalContent.Add(GetAnimationSubContent(animationName));
			var meshSubContent = new ObservableCollection<ContentIconAndName>();
			var materialName = mesh.MetaData.Get("MaterialName", "material");
			meshSubContent.Add(GetMaterialSubContent(materialName, true));
			return new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(ContentType.Mesh),
				meshName, meshSubContent, aditionalContent);
		}

		private static ContentIconAndName GetGeometrySubContent(string geometryName)
		{
			RemoveContentFromContentList(geometryName);
			return new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(ContentType.Geometry),
				geometryName);
		}

		private static ContentIconAndName GetAnimationSubContent(string animationName)
		{
			RemoveContentFromContentList(animationName);
			return
				new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(ContentType.MeshAnimation),
					animationName);
		}

		private static ContentIconAndName GetMaterialSubContent(string contentName, bool isSubContent)
		{
			if (isSubContent)
				RemoveContentFromContentList(contentName);
			var material = ContentLoader.Load<Material>(contentName);
			var images = material.MetaData.Get("ImageOrAnimationName", "image");
			var materialSubContent = new ObservableCollection<ContentIconAndName>();
			materialSubContent.Add(
				new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(ContentType.Image), images));
			RemoveContentFromContentList(images);
			return new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(ContentType.Material),
				contentName, materialSubContent);
		}

		private static void RemoveContentFromContentList(string contentName)
		{
			for (int i = 0; i < contentList.Count; i++)
			{
				var contentIconAndName = contentList[i];
				if (contentIconAndName.Name == contentName)
					contentList.Remove(contentIconAndName);
			}
		}

		private static void CreateMeshWithSubContent(ContentIconAndName content)
		{
			var meshSubContent = GetMeshSubContent(content.Name, false);
			var contentIndex = contentList.IndexOf(content);
			contentList.RemoveAt(contentIndex);
			contentList.Insert(contentIndex, meshSubContent);
		}

		private static void CreateMaterialWithSubContent(ContentIconAndName content)
		{
			var materialSubContent = GetMaterialSubContent(content.Name, false);
			var contentIndex = contentList.IndexOf(content);
			contentList.RemoveAt(contentIndex);
			contentList.Insert(contentIndex, materialSubContent);
		}

		private static void CreateImageAnimationWithSubContent(ContentIconAndName content)
		{
			var imageAnimation = ContentLoader.Load<ImageAnimation>(content.Name);
			var images = imageAnimation.MetaData.Get("ImageNames", "image");
			var imageAnimationSubContent = new ObservableCollection<ContentIconAndName>();
			for (int i = 0; i < images.Split(new[] { ',', ' ' }).Length; i++)
			{
				var newMeshName = images.Split(new[] { ',', ' ' })[i];
				if (string.IsNullOrEmpty(newMeshName))
					continue;
				imageAnimationSubContent.Add(
					new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(ContentType.Image),
						newMeshName));
			}
			var contentIndex = contentList.IndexOf(content);
			contentList.RemoveAt(contentIndex);
			contentList.Insert(contentIndex,
				new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(ContentType.ImageAnimation),
					content.Name, imageAnimationSubContent));
		}

		private static void CreateSpriteSheetWithSubContent(ContentIconAndName content)
		{
			var spriteSheetAnimation = ContentLoader.Load<SpriteSheetAnimation>(content.Name);
			var image = spriteSheetAnimation.MetaData.Get("ImageName", "image");
			var spriteSheetSubContent = new ObservableCollection<ContentIconAndName>
			{
				new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(ContentType.Image), image)
			};
			var contentIndex = contentList.IndexOf(content);
			contentList.RemoveAt(contentIndex);
			contentList.Insert(contentIndex,
				new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(ContentType.ImageAnimation),
					content.Name, spriteSheetSubContent));
		}

		private static void CreateFontWithSubContent(ContentIconAndName content)
		{
			var fontSubContent = new ObservableCollection<ContentIconAndName>
			{
				new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(ContentType.Image),
					content.Name + "Font")
			};
			RemoveContentFromContentList(content.Name + "Font");
			var contentIndex = contentList.IndexOf(content);
			contentList.RemoveAt(contentIndex);
			contentList.Insert(contentIndex,
				new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(ContentType.Font),
					content.Name, fontSubContent));
		}

		private static void CreateParticleSystemWithSubContent(ContentIconAndName content)
		{
			var particleSystem = ContentLoader.Load<ParticleSystemData>(content.Name);
			var emitterNames = particleSystem.MetaData.Get("EmitterNames", "emitter");
			var particleSystemSubContent = new ObservableCollection<ContentIconAndName>();
			for (int i = 0; i < emitterNames.Split(new[] { ',', ' ' }).Length; i++)
			{
				var newEmitterName = emitterNames.Split(new[] { ',', ' ' })[i];
				RemoveContentFromContentList(newEmitterName);
				if (string.IsNullOrEmpty(newEmitterName))
					continue;
				particleSystemSubContent.Add(
					new ContentIconAndName(
						ContentIconAndName.GetContentTypeIcon(ContentType.ParticleEmitter), newEmitterName));
			}
			var contentIndex = contentList.IndexOf(content);
			contentList.RemoveAt(contentIndex);
			contentList.Insert(contentIndex,
				new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(ContentType.ParticleSystem),
					content.Name, particleSystemSubContent));
		}
	}
}