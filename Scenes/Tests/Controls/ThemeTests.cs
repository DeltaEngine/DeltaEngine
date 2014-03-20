using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class ThemeTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void SaveAndLoadDefaultTheme()
		{
			var theme = new Theme();
			var stream = BinaryDataExtensions.SaveToMemoryStream(theme);
			var loadedTheme = (Theme)stream.CreateFromMemoryStream();
			Assert.IsTrue(AreMaterialsEqual(theme.ButtonMouseover, loadedTheme.ButtonMouseover));
			Assert.IsTrue(AreMaterialsEqual(theme.ScrollbarPointer, loadedTheme.ScrollbarPointer));
			Assert.IsTrue(AreMaterialsEqual(theme.TextBoxFocused, loadedTheme.TextBoxFocused));
		}

		private static bool AreMaterialsEqual(Material material1, Material material2)
		{
			return material1.DefaultColor == material2.DefaultColor &&
				material1.DiffuseMap.Name == material2.DiffuseMap.Name &&
				material1.Shader.Name == material2.Shader.Name;
		}

		[Test, CloseAfterFirstFrame]
		public void SaveAndLoadModifiedTheme()
		{
			var theme = new Theme();
			theme.Slider = new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogo")
			{
				DefaultColor = Color.Red
			};
			var stream = BinaryDataExtensions.SaveToMemoryStream(theme);
			var loadedTheme = (Theme)stream.CreateFromMemoryStream();
			Assert.IsTrue(AreMaterialsEqual(theme.Slider, loadedTheme.Slider));
		}

		[Test, CloseAfterFirstFrame]
		public void LoadThemeFromFile()
		{
			CreateSampleTheme();
			var loadedData = new Theme();
			loadedData.LoadFromFile(dataStream);
			Assert.AreEqual(theme.Label, loadedData.Label);
			Assert.AreEqual(theme.Button, loadedData.Button);
			Assert.AreEqual(theme.ButtonDisabled, loadedData.ButtonDisabled);
			Assert.AreEqual(theme.ButtonMouseover, loadedData.ButtonMouseover);
			Assert.AreEqual(theme.ButtonPressed, loadedData.ButtonPressed);
			Assert.AreEqual(theme.DropdownListBox, loadedData.DropdownListBox);
			Assert.AreEqual(theme.DropdownListBoxDisabled, loadedData.DropdownListBoxDisabled);
			Assert.AreEqual(theme.RadioButtonBackground, loadedData.RadioButtonBackground);
			Assert.AreEqual(theme.RadioButtonBackgroundDisabled, loadedData.RadioButtonBackgroundDisabled);
			Assert.AreEqual(theme.RadioButtonDisabled, loadedData.RadioButtonDisabled);
			Assert.AreEqual(theme.RadioButtonNotSelected, loadedData.RadioButtonNotSelected);
			Assert.AreEqual(theme.RadioButtonNotSelectedMouseover, loadedData.RadioButtonNotSelectedMouseover);
			Assert.AreEqual(theme.RadioButtonSelected, loadedData.RadioButtonSelected);
			Assert.AreEqual(theme.RadioButtonSelectedMouseover, loadedData.RadioButtonSelectedMouseover);
			Assert.AreEqual(theme.Scrollbar, loadedData.Scrollbar);
			Assert.AreEqual(theme.ScrollbarDisabled, loadedData.ScrollbarDisabled);
			Assert.AreEqual(theme.ScrollbarPointer, loadedData.ScrollbarPointer);
			Assert.AreEqual(theme.ScrollbarPointerDisabled, loadedData.ScrollbarPointerDisabled);
			Assert.AreEqual(theme.ScrollbarPointerMouseover, loadedData.ScrollbarPointerMouseover);
			Assert.AreEqual(theme.SelectBox, loadedData.SelectBox);
			Assert.AreEqual(theme.SelectBoxDisabled, loadedData.SelectBoxDisabled);
			Assert.AreEqual(theme.Slider, loadedData.Slider);
			Assert.AreEqual(theme.SliderDisabled, loadedData.SliderDisabled);
			Assert.AreEqual(theme.SliderPointer, loadedData.SliderPointer);
			Assert.AreEqual(theme.SliderPointerDisabled, loadedData.SliderPointerDisabled);
			Assert.AreEqual(theme.SliderPointerMouseover, loadedData.SliderPointerMouseover);
			Assert.AreEqual(theme.TextBox, loadedData.TextBox);
			Assert.AreEqual(theme.TextBoxDisabled, loadedData.TextBoxDisabled);
			Assert.AreEqual(theme.TextBoxFocused, loadedData.TextBoxFocused);
		}

		private void CreateSampleTheme()
		{
			theme = new Theme
			{
				Label = new Material(Color.Blue, ShaderFlags.Colored),
				Button = new Material(Color.Red, ShaderFlags.Colored),
				ButtonDisabled = new Material(Color.Blue, ShaderFlags.Colored),
				ButtonMouseover = new Material(Color.Red, ShaderFlags.Colored),
				ButtonPressed = new Material(Color.Blue, ShaderFlags.Colored),
				DropdownListBox = new Material(Color.Red, ShaderFlags.Colored),
				DropdownListBoxDisabled = new Material(Color.Blue, ShaderFlags.Colored),
				RadioButtonBackground = new Material(Color.Red, ShaderFlags.Colored),
				RadioButtonBackgroundDisabled = new Material(Color.Blue, ShaderFlags.Colored),
				RadioButtonDisabled = new Material(Color.Red, ShaderFlags.Colored),
				RadioButtonNotSelected = new Material(Color.Blue, ShaderFlags.Colored),
				RadioButtonNotSelectedMouseover = new Material(Color.Red, ShaderFlags.Colored),
				RadioButtonSelected = new Material(Color.Blue, ShaderFlags.Colored),
				RadioButtonSelectedMouseover = new Material(Color.Red, ShaderFlags.Colored),
				Scrollbar = new Material(Color.Blue, ShaderFlags.Colored),
				ScrollbarDisabled = new Material(Color.Red, ShaderFlags.Colored),
				ScrollbarPointerMouseover = new Material(Color.Blue, ShaderFlags.Colored),
				ScrollbarPointerDisabled = new Material(Color.Red, ShaderFlags.Colored),
				ScrollbarPointer = new Material(Color.Blue, ShaderFlags.Colored),
				SelectBox = new Material(Color.Red, ShaderFlags.Colored),
				SelectBoxDisabled = new Material(Color.Blue, ShaderFlags.Colored),
				Slider = new Material(Color.Red, ShaderFlags.Colored),
				SliderDisabled = new Material(Color.Blue, ShaderFlags.Colored),
				SliderPointer = new Material(Color.Red, ShaderFlags.Colored),
				SliderPointerDisabled = new Material(Color.Blue, ShaderFlags.Colored),
				SliderPointerMouseover = new Material(Color.Red, ShaderFlags.Colored),
				TextBox = new Material(Color.Blue, ShaderFlags.Colored),
				TextBoxFocused = new Material(Color.Red, ShaderFlags.Colored),
				TextBoxDisabled = new Material(Color.Blue, ShaderFlags.Colored),
			};
			SaveDataToStream();
		}

		private Theme theme;

		private void SaveDataToStream()
		{
			dataBytes = BinaryDataExtensions.ToByteArrayWithTypeInformation(theme);
			dataStream = new MemoryStream(dataBytes);
		}

		private byte[] dataBytes;
		private MemoryStream dataStream;
	}
}