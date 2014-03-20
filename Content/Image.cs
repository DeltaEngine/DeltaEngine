using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Used in Sprites and 3D models to draw image textures on screen via <see cref="Material"/>.
	/// </summary>
	public abstract class Image : ContentData
	{
		protected Image(string contentName)
			: base(contentName) {}

		protected Image(ImageCreationData data)
			: base("<GeneratedImage>")
		{
			PixelSize = data.PixelSize;
			BlendMode = data.BlendMode;
			UseMipmaps = data.UseMipmaps;
			AllowTiling = data.AllowTiling;
			DisableLinearFiltering = data.DisableLinearFiltering;
			RenderingCalculator = new RenderingCalculator();
		}

		public Size PixelSize { get; private set; }
		private static readonly Size DefaultTextureSize = new Size(4, 4);
		public BlendMode BlendMode { get; set; }
		public bool UseMipmaps { get; private set; }
		public bool AllowTiling { get; private set; }
		public bool DisableLinearFiltering { get; private set; }
		public RenderingCalculator RenderingCalculator { get; private set; }

		protected override bool AllowCreationIfContentNotFound
		{
			get { return !Debugger.IsAttached; }
		}

		protected override void LoadData(Stream fileData)
		{
			string atlasImageName = MetaData.Get("ImageName", "");
			if (atlasImageName == "")
				ProcessImage(fileData);
			else
				ProcessAtlas(atlasImageName);
		}

		private void ProcessImage(Stream fileData)
		{
			ExtractMetaData();
			SetSamplerStateAndTryToLoadImage(fileData);
			RenderingCalculator = new RenderingCalculator();
		}

		private void ExtractMetaData()
		{
			PixelSize = MetaData.Get("PixelSize", DefaultTextureSize);
			BlendMode = MetaData.Get("BlendMode", BlendMode.Normal);
			UseMipmaps = MetaData.Get("UseMipmaps", false);
			AllowTiling = MetaData.Get("AllowTiling", false);
			DisableLinearFiltering = MetaData.Get("DisableLinearFiltering", false);
		}

		protected abstract void SetSamplerStateAndTryToLoadImage(Stream fileData);

		protected void LoadImage(Stream fileData)
		{
			try
			{
				TryLoadImage(fileData);
			} //ncrunch: no coverage
			catch (Exception ex)
			{
				Logger.Error(ex);
				if (!Debugger.IsAttached)
					CreateDefault();
				else
					throw; //ncrunch: no coverage
			}
		}

		protected abstract void TryLoadImage(Stream fileData);

		// An Image object from an atlas has no texture of its own and only PixelSize for inferred 
		// metadata; Classes like Material wishing to use this Image object should extract the data 
		// they need and then make use of AtlasImage instead.
		private void ProcessAtlas(string atlasImageName)
		{
			DisposeData();
			AtlasImage = ContentLoader.Load<Image>(atlasImageName);
			var uv = new Rectangle(MetaData.Get("UV", ""));
			PixelSize = new Size(AtlasImage.PixelSize.Width * uv.Width,
				AtlasImage.PixelSize.Height * uv.Height);
			CreateUVCalculator(uv);
		}

		public Image AtlasImage { get; private set; }

		private void CreateUVCalculator(Rectangle uv)
		{
			var atlasRegion = new AtlasRegion
			{
				UV = uv,
				PadLeft = GetFloatOrZero("PadLeft"),
				PadRight = GetFloatOrZero("PadRight"),
				PadTop = GetFloatOrZero("PadTop"),
				PadBottom = GetFloatOrZero("PadBottom"),
				IsRotated = MetaData.Get("Rotated", "").ToLowerInvariant() == "true"
			};
			RenderingCalculator = new RenderingCalculator(atlasRegion);
		}

		private float GetFloatOrZero(string metaDataKey)
		{
			var value = MetaData.Get(metaDataKey, "");
			return value == "" ? 0.0f : value.Convert<float>();
		}

		protected void WarnAboutWrongAlphaFormat(bool imageHasAlphaFormat)
		{
			if (HasAlpha && !imageHasAlphaFormat)
				Logger.Warning("Image '" + Name +
					"' is supposed to have alpha pixels, but the image pixel format is not using alpha.");
			else if (!HasAlpha && imageHasAlphaFormat)
				Logger.Warning("Image '" + Name +
					"' is supposed to have no alpha pixels, but the image pixel format is using alpha.");
		}

		protected bool HasAlpha
		{
			get { return BlendMode == BlendMode.Normal || BlendMode == BlendMode.AlphaTest; }
		}

		protected override void CreateDefault()
		{
			PixelSize = DefaultTextureSize;
			DisableLinearFiltering = true;
			BlendMode = BlendMode.Opaque;
			Fill(CheckerMapColors);
			SetSamplerState();
			RenderingCalculator = new RenderingCalculator();
		}

		public void Fill(Color color)
		{
			var colors = new Color[(int)PixelSize.Width * (int)PixelSize.Height];
			for (int i = 0; i < colors.Length; i++)
				colors[i] = color;
			Fill(colors);
		}

		public void Fill(Color[] colors)
		{
			if (PixelSize.Width * PixelSize.Height != colors.Length)
				throw new InvalidNumberOfColors(PixelSize);
			FillRgbaData(Color.GetRgbaBytesFromArray(colors));
		}

		public class InvalidNumberOfColors : Exception
		{
			public InvalidNumberOfColors(Size pixelSize)
				: base(pixelSize.Width + "*" + pixelSize.Height) {}
		}

		/// <summary>
		/// Fill opaque or alpha images with 4 bytes per pixel (rgba). Using 3 bytes (24bit) is
		/// inefficient and would only work on some platforms that allow it. Use 16 or 8 bit if you want
		/// performance and use compressed textures as well. For small images this works fine.
		/// </summary>
		public abstract void FillRgbaData(byte[] rgbaColors);

		public class InvalidNumberOfBytes : Exception
		{
			public InvalidNumberOfBytes(Size pixelSize)
				: base(pixelSize.Width + "*" + pixelSize.Height + "*" + 4) {}
		}

		private static readonly Color[] CheckerMapColors =
		{
			Color.LightGray, Color.DarkGray, Color.LightGray, Color.DarkGray,
			Color.DarkGray, Color.LightGray, Color.DarkGray, Color.LightGray,
			Color.LightGray, Color.DarkGray, Color.LightGray, Color.DarkGray,
			Color.DarkGray, Color.LightGray, Color.DarkGray, Color.LightGray
		};

		protected abstract void SetSamplerState();

		protected void CompareActualSizeMetadataSize(Size actualSize)
		{
			if (actualSize != PixelSize)
				Logger.Warning("Image '" + Name + "' actual size " + actualSize +
					" is different from the MetaData PixelSize: " + PixelSize);
		}
	}
}