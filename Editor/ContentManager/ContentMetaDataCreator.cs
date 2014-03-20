using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;

namespace DeltaEngine.Editor.ContentManager
{
	/// <summary>
	/// Used by ContentManager to generate MetaData to be sent via UpdateContent to the OnlineService
	/// </summary>
	public class ContentMetaDataCreator
	{
		//ncrunch: no coverage start
		public ContentMetaData CreateMetaDataFromFile(string filePath)
		{
			var contentMetaData = new ContentMetaData();
			contentMetaData.Name = Path.GetFileNameWithoutExtension(filePath);
			contentMetaData.Type = ContentTypeIdentifier.ExtensionToType(filePath);
			contentMetaData.LastTimeUpdated = File.GetLastWriteTime(filePath);
			contentMetaData.LocalFilePath = Path.GetFileName(filePath);
			contentMetaData.PlatformFileId = 0;
			contentMetaData.FileSize = (int)new FileInfo(filePath).Length;
			if (contentMetaData.Type == ContentType.Image)
				AddImageDataFromBitmapToContentMetaData(filePath, contentMetaData);
			return contentMetaData;
		}

		private static void AddImageDataFromBitmapToContentMetaData(string filePath,
			ContentMetaData metaData)
		{
			try
			{
				TryAddImageDataFromBitmapToContentMetaData(filePath, metaData);
			}
			catch (Exception)
			{
				throw new UnknownImageFormatUnableToAquirePixelSize(filePath);
			}
		}

		private static void TryAddImageDataFromBitmapToContentMetaData(string filePath,
			ContentMetaData metaData)
		{
			var bitmap = new Bitmap(filePath);
			metaData.Values.Add("PixelSize", "(" + bitmap.Width + "," + bitmap.Height + ")");
			if (!HasBitmapAlphaPixels(bitmap))
				metaData.Values.Add("BlendMode", "Opaque");
		}

		private static unsafe bool HasBitmapAlphaPixels(Bitmap bitmap)
		{
			int width = bitmap.Width;
			int height = bitmap.Height;
			var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
				PixelFormat.Format32bppArgb);
			var bitmapPointer = (byte*)bitmapData.Scan0.ToPointer();
			var foundAlphaPixel = HasImageDataAlpha(width, height, bitmapPointer);
			bitmap.UnlockBits(bitmapData);
			return foundAlphaPixel;
		}

		private static unsafe bool HasImageDataAlpha(int width, int height, byte* bitmapPointer)
		{
			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
					if (bitmapPointer[(y * width + x) * 4 + 3] != 255)
						return true;
			return false;
		}

		private class UnknownImageFormatUnableToAquirePixelSize : Exception
		{
			public UnknownImageFormatUnableToAquirePixelSize(string message)
				: base(message) {}
		}

		public ContentMetaData CreateMetaDataFromImageAnimation(string animationName,
			ImageAnimation animation)
		{
			var contentMetaData = new ContentMetaData();
			SetDefaultValues(contentMetaData, animationName);
			contentMetaData.Type = ContentType.ImageAnimation;
			contentMetaData.Values.Add("DefaultDuration",
				animation.DefaultDuration.ToString(CultureInfo.InvariantCulture));
			string images = "";
			for (int index = 0; index < animation.Frames.Length; index++)
				images = AddImageToMetaData(animation, index, images);
			contentMetaData.Values.Add("ImageNames", images);
			return contentMetaData;
		}

		private static void SetDefaultValues(ContentMetaData contentMetaData, string name)
		{
			contentMetaData.Name = name;
			contentMetaData.LastTimeUpdated = DateTime.Now;
			contentMetaData.PlatformFileId = 0;
			contentMetaData.Language = "en";
		}

		private static string AddImageToMetaData(ImageAnimation animation, int index, string images)
		{
			var image = animation.Frames[index];
			if (images == "")
				images += (image.Name);
			else
				images += (", " + image.Name);
			return images;
		}

		public ContentMetaData CreateMetaDataFromSpriteSheetAnimation(string animationName,
			SpriteSheetAnimation spriteSheetAnimation)
		{
			var contentMetaData = new ContentMetaData();
			SetDefaultValues(contentMetaData, animationName);
			contentMetaData.Type = ContentType.SpriteSheetAnimation;
			contentMetaData.Values.Add("DefaultDuration",
				spriteSheetAnimation.DefaultDuration.ToString(CultureInfo.InvariantCulture));
			contentMetaData.Values.Add("SubImageSize", spriteSheetAnimation.SubImageSize.ToString());
			contentMetaData.Values.Add("ImageName", spriteSheetAnimation.Image.Name);
			return contentMetaData;
		}

		public ContentMetaData CreateMetaDataFromParticle(string particleName, byte[] byteArray)
		{
			var contentMetaData = new ContentMetaData();
			SetDefaultValues(contentMetaData, particleName);
			contentMetaData.Type = ContentType.ParticleEmitter;
			contentMetaData.LocalFilePath = particleName + ".deltaparticle";
			contentMetaData.FileSize = byteArray.Length;
			return contentMetaData;
		}

		public ContentMetaData CreateMetaDataFromMaterial(string materialName, Material material)
		{
			var contentMetaData = new ContentMetaData();
			SetDefaultValues(contentMetaData, materialName);
			contentMetaData.Type = ContentType.Material;
			contentMetaData.Values.Add("ShaderFlags", material.Shader.Flags.ToString());
			contentMetaData.Values.Add("BlendMode", material.DiffuseMap.BlendMode.ToString());
			if (material.Animation != null)
				contentMetaData.Values.Add("ImageOrAnimationName", material.Animation.Name);
			else if (material.SpriteSheet != null)
				contentMetaData.Values.Add("ImageOrAnimationName", material.SpriteSheet.Name);
			else
				contentMetaData.Values.Add("ImageOrAnimationName", material.DiffuseMap.Name);
			contentMetaData.Values.Add("Color", material.DefaultColor.ToString());
			contentMetaData.Values.Add("RenderSizeMode", material.RenderSizeMode.ToString());
			return contentMetaData;
		}

		public ContentMetaData CreateParticleSystemData(string name, List<string> emitterDataNames)
		{
			if (emitterDataNames == null || emitterDataNames.Count == 0)
				return null;
			var contentMetaData = new ContentMetaData { Name = name, Type = ContentType.ParticleSystem};
			SetDefaultValues(contentMetaData, name);
			var rowOfNames = emitterDataNames[0];
			for (int i = 1; i < emitterDataNames.Count; i++)
				rowOfNames += ", " + emitterDataNames[i];
			contentMetaData.Values.Add("EmitterNames", rowOfNames);
			return contentMetaData;
		}

		public ContentMetaData CreateMetaDataFromUI(string uiName, byte[] byteArray)
		{
			var contentMetaData = new ContentMetaData();
			SetDefaultValues(contentMetaData, uiName);
			contentMetaData.Type = ContentType.Scene;
			contentMetaData.LocalFilePath = uiName + ".deltaUI";
			contentMetaData.FileSize = byteArray.Length;
			return contentMetaData;
		}

		public ContentMetaData CreateMetaDataFromInputData(byte[] byteArray)
		{
			var contentMetaData = new ContentMetaData();
			SetDefaultValues(contentMetaData, "InputCommands");
			contentMetaData.Type = ContentType.InputCommand;
			contentMetaData.LocalFilePath = "InputCommands.xml";
			contentMetaData.FileSize = byteArray.Length;
			return contentMetaData;
		}

		public ContentMetaData CreateMetaDataFromLevelData(string levelName, byte[] byteArray)
		{
			var contentMetaData = new ContentMetaData();
			SetDefaultValues(contentMetaData, levelName);
			contentMetaData.Type = ContentType.Level;
			contentMetaData.LocalFilePath = levelName + ".xml";
			contentMetaData.FileSize = byteArray.Length;
			return contentMetaData;
		}
	}
}