using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Tests
{
	public class SpriteSaveAndLoadTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void CreateSpriteFromComponents()
		{
			var components = new List<object>();
			components.Add(Rectangle.One);
			components.Add(true);
			var material = new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogo");
			components.Add(material);
			components.Add(material.DiffuseMap.BlendMode);
			components.Add(new RenderingData());
			var sprite = Activator.CreateInstance(typeof(Sprite),
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder,
				null, CultureInfo.CurrentCulture) as Sprite;
			sprite.SetComponents(components);
			Assert.AreEqual(material, sprite.Material);
			Assert.AreEqual(Rectangle.One, sprite.DrawArea);
			Assert.AreEqual(material.DiffuseMap.BlendMode, sprite.BlendMode);
		}

		[Test, CloseAfterFirstFrame]
		public void SaveAndLoadSprite()
		{
			var sprite = new Sprite("DeltaEngineLogo", Rectangle.One);
			var data = BinaryDataExtensions.SaveToMemoryStream(sprite);
			Assert.Greater(data.Length, 0);
			var loadedSprite = data.CreateFromMemoryStream() as Sprite;
			Assert.AreEqual(sprite.Material.Shader, loadedSprite.Material.Shader);
			Assert.AreEqual(sprite.Material.DiffuseMap, loadedSprite.Material.DiffuseMap);
			Assert.AreEqual(sprite.DrawArea, loadedSprite.DrawArea);
			Assert.AreEqual(sprite.BlendMode, loadedSprite.BlendMode);
			Assert.AreEqual(sprite.Rotation, loadedSprite.Rotation);
			Assert.AreEqual(1, loadedSprite.GetActiveBehaviors().Count);
			Assert.AreEqual(1, loadedSprite.GetDrawBehaviors().Count);
			Assert.AreEqual("SpriteRenderer",
				loadedSprite.GetDrawBehaviors()[0].GetShortNameOrFullNameIfNotFound());
		}
	}
}