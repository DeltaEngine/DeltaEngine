using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;

namespace DeltaEngine.Rendering2D.Fonts
{
	/// <summary>
	/// Holds the image and data for rendering a smoothly drawn font.
	/// </summary>
	public class Font : XmlContent
	{
		protected Font(string contentName)
			: base(contentName) {}

		public static Font Default
		{
			get { return ContentLoader.Load<Font>(DefaultFontName); }
		}

		private const string DefaultFontName = "Verdana12";

		//ncrunch: no coverage start
		protected override void LoadData(Stream fileData)
		{
			base.LoadData(fileData);
			InitializeDescriptionAndMaterial();
		}

		protected override void DisposeData() {}

		protected override bool AllowCreationIfContentNotFound
		{
			get { return true; }
		}

		protected void InitializeDescriptionAndMaterial()
		{
			if (Data == null || Data.Children.Count == 0)
			{
				Logger.Warning("Could not load '" + Name + "' font");
				return;
			}
			Description = new FontDescription(Data);
			Material = new Material(ShaderFlags.Position2DColoredTextured, Description.FontMapName);
			WasLoadedOk = true;
		}

		internal FontDescription Description { get; private set; }
		internal Material Material { get; private set; }
		internal bool WasLoadedOk { get; set; }

		protected override void CreateDefault()
		{
			if (Name == DefaultFontName)
			{
				if (Debugger.IsAttached)
					throw new DefaultFontContentNotFound();
				Logger.Warning("Could not load default '" + DefaultFontName + "' font");
				return;
			}
			Data = ContentLoader.Load<Font>(DefaultFontName).Data;
			InitializeDescriptionAndMaterial();
		}

		public class DefaultFontContentNotFound : Exception {}
	}
}