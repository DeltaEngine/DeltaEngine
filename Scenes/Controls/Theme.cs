using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// Holds a set of materials and colors for Scenes UI controls, as well as the font to be used.
	/// </summary>
	public class Theme : ContentData
	{
		public Theme()
			: base("<GeneratedDefaultTheme>")
		{
			DefaultButtonAppearance();
			DefaultDropdownListAppearance();
			Font = Font.Default;
			Label = new Material(Color.TransparentWhite);
			DefaultRadioButtonAppearance();
			DefaultScrollbarAppearance();
			DefaultSelectBoxAppearance();
			DefaultSliderAppearance();
			DefaultTextBoxAppearance();
		}

		public Font Font
		{
			get { return font; }
			set { font = value; }
		}

		[NonSerialized]
		private Font font;

		public Material Label { get; set; }

		private void DefaultButtonAppearance()
		{
			Button = new Material(Color.Gray);
			ButtonDisabled = new Material(Color.DarkGray);
			ButtonMouseover = new Material(Color.LightGray);
			ButtonPressed = new Material(Color.LightBlue);
		}

		public Material Button { get; set; }
		public Material ButtonDisabled { get; set; }
		public Material ButtonMouseover { get; set; }
		public Material ButtonPressed { get; set; }

		private void DefaultDropdownListAppearance()
		{
			DropdownListBox = new Material(Color.Gray);
			DropdownListBoxDisabled = new Material(Color.DarkGray);
		}

		public Material DropdownListBox { get; set; }
		public Material DropdownListBoxDisabled { get; set; }

		private void DefaultRadioButtonAppearance()
		{
			RadioButtonBackground = new Material(Color.Gray);
			RadioButtonBackgroundDisabled = new Material(Color.DarkGray);
			RadioButtonDisabled = new Material(Color.Gray);
			RadioButtonNotSelected = new Material(Color.LightGray);
			RadioButtonNotSelectedMouseover = new Material(Color.VeryLightGray);
			RadioButtonSelected = new Material(Color.White);
			RadioButtonSelectedMouseover = new Material(Color.LightBlue);
		}

		public Material RadioButtonBackground { get; set; }
		public Material RadioButtonBackgroundDisabled { get; set; }
		public Material RadioButtonDisabled { get; set; }
		public Material RadioButtonNotSelected { get; set; }
		public Material RadioButtonNotSelectedMouseover { get; set; }
		public Material RadioButtonSelected { get; set; }
		public Material RadioButtonSelectedMouseover { get; set; }

		private void DefaultScrollbarAppearance()
		{
			Scrollbar = new Material(Color.Gray);
			ScrollbarDisabled = new Material(Color.DarkGray);
			ScrollbarPointerMouseover = new Material(Color.LightBlue);
			ScrollbarPointerDisabled = new Material(Color.Gray);
			ScrollbarPointer = new Material(Color.LightGray);
		}

		public Material Scrollbar { get; set; }
		public Material ScrollbarDisabled { get; set; }
		public Material ScrollbarPointer { get; set; }
		public Material ScrollbarPointerDisabled { get; set; }
		public Material ScrollbarPointerMouseover { get; set; }

		private void DefaultSelectBoxAppearance()
		{
			SelectBox = new Material(Color.Gray);
			SelectBoxDisabled = new Material(Color.DarkGray);
		}

		public Material SelectBox { get; set; }
		public Material SelectBoxDisabled { get; set; }

		private void DefaultSliderAppearance()
		{
			Slider = new Material(Color.Gray);
			SliderDisabled = new Material(Color.DarkGray);
			SliderPointer = new Material(Color.LightGray);
			SliderPointerDisabled = new Material(Color.Gray);
			SliderPointerMouseover = new Material(Color.LightBlue);
		}

		public Material Slider { get; set; }
		public Material SliderDisabled { get; set; }
		public Material SliderPointer { get; set; }
		public Material SliderPointerDisabled { get; set; }
		public Material SliderPointerMouseover { get; set; }

		private void DefaultTextBoxAppearance()
		{
			TextBox = new Material(Color.Gray);
			TextBoxFocused = new Material(Color.LightGray);
			TextBoxDisabled = new Material(Color.DarkGray);
		}

		public Material TextBox { get; set; }
		public Material TextBoxDisabled { get; set; }
		public Material TextBoxFocused { get; set; }

		protected override void DisposeData() {}

		//ncrunch: no coverage start
		protected override void LoadData(Stream fileData)
		{
			var theme = (Theme)new BinaryReader(fileData).Create();
			Label = theme.Label;
			Button = theme.Button;
			ButtonDisabled = theme.ButtonDisabled;
			ButtonMouseover = theme.ButtonMouseover;
			ButtonPressed = theme.ButtonPressed;
			DropdownListBox = theme.DropdownListBox;
			DropdownListBoxDisabled = theme.DropdownListBoxDisabled;
			RadioButtonBackground = theme.RadioButtonBackground;
			RadioButtonBackgroundDisabled = theme.RadioButtonBackgroundDisabled;
			RadioButtonDisabled = theme.RadioButtonDisabled;
			RadioButtonNotSelected = theme.RadioButtonNotSelected;
			RadioButtonNotSelectedMouseover = theme.RadioButtonNotSelectedMouseover;
			RadioButtonSelected = theme.RadioButtonSelected;
			RadioButtonSelectedMouseover = theme.RadioButtonSelectedMouseover;
			Scrollbar = theme.Scrollbar;
			ScrollbarDisabled = theme.ScrollbarDisabled;
			ScrollbarPointerMouseover = theme.ScrollbarPointerMouseover;
			ScrollbarPointerDisabled = theme.ScrollbarPointerDisabled;
			ScrollbarPointer = theme.ScrollbarPointer;
			SelectBox = theme.SelectBox;
			SelectBoxDisabled = theme.SelectBoxDisabled;
			Slider = theme.Slider;
			SliderDisabled = theme.SliderDisabled;
			SliderPointer = theme.SliderPointer;
			SliderPointerDisabled = theme.SliderPointerDisabled;
			SliderPointerMouseover = theme.SliderPointerMouseover;
			TextBox = theme.TextBox;
			TextBoxDisabled = theme.TextBoxDisabled;
			TextBoxFocused = theme.TextBoxFocused;
		}

		public void LoadFromFile(Stream fileData)
		{
			if (fileData.Length == 0)
				throw new EmptyThemeFileGiven();
			var reader = new BinaryReader(fileData);
			reader.BaseStream.Position = 0;
			string shortName = reader.ReadString();
			var dataVersion = reader.ReadBytes(4);
			bool justSaveName = reader.ReadBoolean();
			bool noFieldData = reader.ReadBoolean();
			string name = reader.ReadString();
			reader.BaseStream.Position++;

			Label = ReadMaterialInfoFromStream(reader);
			Button = ReadMaterialInfoFromStream(reader);
			ButtonDisabled = ReadMaterialInfoFromStream(reader);
			ButtonMouseover = ReadMaterialInfoFromStream(reader);
			ButtonPressed = ReadMaterialInfoFromStream(reader);
			DropdownListBox = ReadMaterialInfoFromStream(reader);
			DropdownListBoxDisabled = ReadMaterialInfoFromStream(reader);
			RadioButtonBackground = ReadMaterialInfoFromStream(reader);
			RadioButtonBackgroundDisabled = ReadMaterialInfoFromStream(reader);
			RadioButtonDisabled = ReadMaterialInfoFromStream(reader);
			RadioButtonNotSelected = ReadMaterialInfoFromStream(reader);
			RadioButtonNotSelectedMouseover = ReadMaterialInfoFromStream(reader);
			RadioButtonSelected = ReadMaterialInfoFromStream(reader);
			RadioButtonSelectedMouseover = ReadMaterialInfoFromStream(reader);
			Scrollbar = ReadMaterialInfoFromStream(reader);
			ScrollbarDisabled = ReadMaterialInfoFromStream(reader);
			ScrollbarPointerMouseover = ReadMaterialInfoFromStream(reader);
			ScrollbarPointerDisabled = ReadMaterialInfoFromStream(reader);
			ScrollbarPointer = ReadMaterialInfoFromStream(reader);
			SelectBox = ReadMaterialInfoFromStream(reader);
			SelectBoxDisabled = ReadMaterialInfoFromStream(reader);
			Slider = ReadMaterialInfoFromStream(reader);
			SliderDisabled = ReadMaterialInfoFromStream(reader);
			SliderPointer = ReadMaterialInfoFromStream(reader);
			SliderPointerDisabled = ReadMaterialInfoFromStream(reader);
			SliderPointerMouseover = ReadMaterialInfoFromStream(reader);
			TextBox = ReadMaterialInfoFromStream(reader);
			TextBoxDisabled = ReadMaterialInfoFromStream(reader);
			TextBoxFocused = ReadMaterialInfoFromStream(reader);
		}

		private static Material ReadMaterialInfoFromStream(BinaryReader reader)
		{
			reader.ReadBoolean();
			reader.ReadString();
			var justMaterialName = reader.ReadBoolean();
			if (!justMaterialName)
				return LoadCustomMaterial(reader);
			var materialName = reader.ReadString();
			return ContentLoader.Load<Material>(materialName);
		}

		private static Material LoadCustomMaterial(BinaryReader reader)
		{
			var shaderFlags = (ShaderFlags)reader.ReadInt32();
			var customImageType = reader.ReadByte();
			var pixelSize = customImageType > 0
				? new Size(reader.ReadSingle(), reader.ReadSingle()) : Size.Zero;
			var imageOrAnimationName = customImageType > 0 ? "" : reader.ReadString();
			var customImage = customImageType == 1
				? ContentLoader.Create<Image>(new ImageCreationData(pixelSize)) : null;
			var color = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(),
				reader.ReadByte());
			var duration = reader.ReadSingle();
			var material = customImageType > 0
				? new Material(ContentLoader.Create<Shader>(new ShaderCreationData(shaderFlags)),
					customImage) : new Material(shaderFlags, imageOrAnimationName);
			material.DefaultColor = color;
			material.Duration = duration;
			return material;
		}

		public class EmptyThemeFileGiven : Exception {}
	}
}