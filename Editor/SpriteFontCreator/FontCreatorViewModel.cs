using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Rendering2D.Fonts;
using GalaSoft.MvvmLight;
using Microsoft.Win32;

namespace DeltaEngine.Editor.SpriteFontCreator
{
	public class FontCreatorViewModel : ViewModelBase
	{
		public FontCreatorViewModel(Service service)
		{
			this.service = service;
			targetProjectPath = Path.Combine("Content", service.ProjectName);
			settings = new FontGeneratorSettings();
			BestFontSize = 12;
			UseDefaultFont = true;
			settings.OutlineThicknessPercent = 0;
			AvailableDefaultFontNames = new List<string>();
			InitializeLists();
			FillContentNames();
			UpdateAvailableDefaultFonts();
		}

		private readonly Service service;
		public FontGeneratorSettings settings;
		public string targetProjectPath;
		public List<string> AvailableDefaultFontNames { get; set; }

		private void InitializeLists()
		{
			ContentNamesList = new ObservableCollection<string>();
		}

		public ObservableCollection<string> ContentNamesList { get; set; }

		private void FillContentNames()
		{
			ContentNamesList.Clear();
			foreach (var font in service.GetAllContentNamesByType(ContentType.Font))
				ContentNamesList.Add(font);
			if (ContentNamesList.Count > 0)
				ContentName = ContentNamesList[0];
			RaisePropertyChanged("ContentName");
			RaisePropertyChanged("ContentNamesList");
		}

		public void OpenImportdialogue()
		{
			var dialog = new OpenFileDialog();
			dialog.Filter = "TrueType Font |*.ttf|OpenType Font|*.otf|PostScript Font|*.pfa|" +
				"PostScript Font|*.pfb|PostScript Type3|*.pt3|Spline Font Database|*.sfd|" +
				"X11 OTB bitmap|*.otb|PostScript Type42|*.t42|Compact Embedded Font|*.cef|" +
				"Compact Font Format|*.cff|GhostScript Font|*.gsf|TrueType Collection|*.ttc|" +
				"Scalable Vector Graphics|*.svg|ik files|*.ik|Metafont|*.mf|" +
				"Datafork TrueType Font|*.dfont|Bitmap Distribution Format|*.bdf";
			var result = dialog.ShowDialog();
			if (result == true)
				FamilyFilename = dialog.FileName;
		}

		public void GenerateFontFromSettings()
		{
			try
			{
				TrySendForGeneration();
			}
			catch (Exception ex)
			{
				Logger.Info(ex.ToString());
			}
		}

		private void TrySendForGeneration()
		{
			if (string.IsNullOrEmpty(ContentName))
				throw new CannotSaveFontWithoutSpecifiedContentName();
			if (string.IsNullOrEmpty(FamilyFilename))
				throw new GettingFontWithEmptyNameNotPossible();
			var metaDataToSend = SetMetaDataForFont();
			byte[] fontFileData;
			using (var fontFileReader = new BinaryReader(new FileStream(FamilyFilename, FileMode.Open)))
				fontFileData = fontFileReader.ReadBytes((int)fontFileReader.BaseStream.Length);
			var dataAndName = new Dictionary<string, byte[]> { { ContentName + ".ttf", fontFileData } };
			service.UploadContent(metaDataToSend, dataAndName);
		}

		private ContentMetaData SetMetaDataForFont()
		{
			var metaDataToSend = new ContentMetaData { Name = ContentName, Type = ContentType.Font };
			metaDataToSend.Values.Add("FontSize", BestFontSize.ToString(CultureInfo.InvariantCulture));
			metaDataToSend.Values.Add("FontStyle", settings.Style.ToString());
			metaDataToSend.Values.Add("FontColor", settings.FontColor.ToString());
			metaDataToSend.Values.Add("ShadowColor", settings.ShadowColor.ToString());
			metaDataToSend.Values.Add("OutlineColor", settings.OutlineColor.ToString());
			metaDataToSend.Values.Add("OutlineThickness",
				settings.OutlineThicknessPercent.ToString(CultureInfo.InvariantCulture));
			metaDataToSend.Values.Add("LineHeight",
				settings.LineHeight.ToString(CultureInfo.InvariantCulture));
			metaDataToSend.Values.Add("Tracking",
				settings.Tracking.ToString(CultureInfo.InvariantCulture));
			metaDataToSend.Values.Add("CharacterMarginLeft",
				settings.CharacterMarginLeft.ToString(CultureInfo.InvariantCulture));
			metaDataToSend.Values.Add("CharacterMarginTop",
				settings.CharacterMarginTop.ToString(CultureInfo.InvariantCulture));
			metaDataToSend.Values.Add("CharacterMarginRight",
				settings.CharacterMarginRight.ToString(CultureInfo.InvariantCulture));
			metaDataToSend.Values.Add("CharacterMarginBottom",
				settings.CharacterMarginBottom.ToString(CultureInfo.InvariantCulture));
			metaDataToSend.Values.Add("CharactersToGenerate", settings.CharactersToGenerate.ToString());
			metaDataToSend.Values.Add("FontDpi", settings.FontDpi.ToString(CultureInfo.InvariantCulture));
			metaDataToSend.Values.Add("UseKerning", settings.UseKerning.ToString());
			metaDataToSend.Values.Add("DebugMode", settings.DebugMode.ToString());
			metaDataToSend.Values.Add("DefaultFontColor", settings.DefaultFontColor.ToString());
			metaDataToSend.Values.Add("DefaultOutlineColor", settings.DefaultOutlineColor.ToString());
			metaDataToSend.Values.Add("DefaultShadowColor", settings.DefaultShadowColor.ToString());
			metaDataToSend.Values.Add("OutlineThicknessPercent",
				settings.OutlineThicknessPercent.ToString(CultureInfo.InvariantCulture));
			metaDataToSend.Values.Add("ShadowDistancePercent",
				settings.ShadowDistancePercent.ToString(CultureInfo.InvariantCulture));
			return metaDataToSend;
		}

		public class CannotSaveFontWithoutSpecifiedContentName : Exception {}

		public class GettingFontWithEmptyNameNotPossible : Exception {}

		public void UpdateAvailableDefaultFonts()
		{
			var systemFonts = new InstalledFontCollection();
			for (int i = 0; i < systemFonts.Families.Length; i++)
				AvailableDefaultFontNames.Add(systemFonts.Families[i].Name);
			FamilyFilename = AvailableDefaultFontNames[0];
			RaisePropertyChanged("AvailableDefaultFontNames");
			RaisePropertyChanged("FamilyFilename");
		}

		public string ContentName { get; set; }

		public string FamilyFilename
		{
			get { return familyFilename; }
			set
			{
				if (!string.IsNullOrEmpty(value))
					familyFilename = value;
				RaisePropertyChanged("FamilyFilename");
			}
		}

		private string familyFilename;
		public bool UseDefaultFont { get; set; }
		public float BestFontSize { get; set; }

		public float BestFontTracking
		{
			get { return settings.Tracking; }
			set
			{
				settings.Tracking = value;
				RaisePropertyChanged("BestFontTracking");
			}
		}

		public float BestFontLineHeight
		{
			get { return settings.LineHeight; }
			set
			{
				settings.LineHeight = value;
				RaisePropertyChanged("BestFontLineHeight");
			}
		}

		public void Reset()
		{
			FillContentNames();
			SetControls();
		}

		public void SetControls()
		{
			var font = ContentLoader.Load<Font>("Verdana12");
			Bold = font.MetaData.Get("Bold", false);
			Italic = font.MetaData.Get("Italic", false);
			Underline = font.MetaData.Get("Underline", false);
			AddOutline = font.MetaData.Get("AddOutline", false);
			AddShadow = font.MetaData.Get("AddShadow", false);
			RaiseAllPropertyChanged();
		}

		private void RaiseAllPropertyChanged()
		{
			RaisePropertyChanged("Bold");
			RaisePropertyChanged("Italic");
			RaisePropertyChanged("Underline");
			RaisePropertyChanged("AddOutline");
			RaisePropertyChanged("AddShadow");
			RaisePropertyChanged("FontColor");
			RaisePropertyChanged("ShadowColor");
			RaisePropertyChanged("OutlineColor");
			RaisePropertyChanged("OutlineThickness");
			RaisePropertyChanged("BestFontSize");
			RaisePropertyChanged("BestFontLineHeight");
			RaisePropertyChanged("BestFontTracking");
		}

		public bool Italic
		{
			get { return settings.IsFontStyleSet(FontGeneratorSettings.FontStyle.Italic); }
			set
			{
				if (value)
					settings.AddStyle(FontGeneratorSettings.FontStyle.Italic);
				else
					settings.RemoveStyle(FontGeneratorSettings.FontStyle.Italic);
				RaisePropertyChanged("Italic");
			}
		}
		public bool Bold
		{
			get { return settings.IsFontStyleSet(FontGeneratorSettings.FontStyle.Bold); }
			set
			{
				if (value)
					settings.AddStyle(FontGeneratorSettings.FontStyle.Bold);
				else
					settings.RemoveStyle(FontGeneratorSettings.FontStyle.Bold);
				RaisePropertyChanged("Bold");
			}
		}
		public bool Underline
		{
			get { return settings.IsFontStyleSet(FontGeneratorSettings.FontStyle.Underline); }
			set
			{
				if (value)
					settings.AddStyle(FontGeneratorSettings.FontStyle.Underline);
				else
					settings.RemoveStyle(FontGeneratorSettings.FontStyle.Underline);
				RaisePropertyChanged("Underline");
			}
		}
		public bool AddShadow
		{
			get { return settings.IsFontStyleSet(FontGeneratorSettings.FontStyle.AddShadow); }
			set
			{
				if (value)
					settings.AddStyle(FontGeneratorSettings.FontStyle.AddShadow);
				else
					settings.RemoveStyle(FontGeneratorSettings.FontStyle.AddShadow);
				RaisePropertyChanged("AddShadow");
			}
		}
		public bool AddOutline
		{
			get { return settings.IsFontStyleSet(FontGeneratorSettings.FontStyle.AddOutline); }
			set
			{
				if (value)
					settings.AddStyle(FontGeneratorSettings.FontStyle.AddOutline);
				else
					settings.RemoveStyle(FontGeneratorSettings.FontStyle.AddOutline);
				RaisePropertyChanged("AddOutline");
			}
		}
	}
}