using System;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Displays images or shapes via shaders in 2D or 3D. Always need a shader and a diffuse map,
	/// which can be a single <see cref="Image"/>, an <see cref="ImageAnimation"/>, or a
	/// <see cref="SpriteSheet"/>
	/// </summary>
	public sealed class Material : ContentData, IEquatable<Material>
	{
		/// <summary>
		/// Create material from content created via the Editor, contentName is NOT just the image name.
		/// </summary>
		private Material(string contentName)
			: base(contentName) {}

		/// <summary>
		/// As opposed to loading a material from content it can also be created with this constructor.
		/// If imageOrAnimationName is used you need to provide either a DiffuseMap or an Animation.
		/// </summary>
		public Material(ShaderFlags shaderFlags, string imageOrAnimationName)
			: base("<GeneratedMaterial:" + shaderFlags + ":" + imageOrAnimationName + ">")
		{
			Initialize(shaderFlags, imageOrAnimationName, Color.White);
			if (MetaData == null)
				MetaData = new ContentMetaData();
		}

		private void Initialize(ShaderFlags flags, string imageOrAnimationName, Color defaultColor)
		{
			Shader = ContentLoader.Create<Shader>(new ShaderCreationData(flags));
			DefaultColor = defaultColor;
			RenderingCalculator = new RenderingCalculator();
			if (!String.IsNullOrEmpty(imageOrAnimationName))
				DetermineUseCaseData(imageOrAnimationName);
		}

		private void DetermineUseCaseData(string imageOrAnimationName)
		{
			if (ContentLoader.Exists(imageOrAnimationName, ContentType.ImageAnimation))
				Animation = ContentLoader.Load<ImageAnimation>(imageOrAnimationName);
			else if (ContentLoader.Exists(imageOrAnimationName, ContentType.SpriteSheetAnimation))
				SpriteSheet = ContentLoader.Load<SpriteSheetAnimation>(imageOrAnimationName);
			else
				DiffuseMap = ContentLoader.Load<Image>(imageOrAnimationName);	
		}

		public ImageAnimation Animation
		{
			get { return animation; }
			set
			{
				animation = value;
				DiffuseMap = animation.Frames[0];
				Duration = animation.DefaultDuration;
			}
		}

		private ImageAnimation animation;
		public float Duration { get; set; }

		public SpriteSheetAnimation SpriteSheet
		{
			get { return spriteSheet; }
			set
			{
				spriteSheet = value;
				DiffuseMap = spriteSheet.Image;
				Duration = spriteSheet.DefaultDuration;
			}
		}

		private SpriteSheetAnimation spriteSheet;

		public Shader Shader { get; private set; }
		public Color DefaultColor { get; set; }
		public RenderingCalculator RenderingCalculator { get; set; }

		public Image DiffuseMap
		{
			get { return diffuseMap; }
			set
			{
				if (value == null)
				{
					diffuseMap = null;
					RenderingCalculator = new RenderingCalculator();
				}
				else
				{
					diffuseMap = value.AtlasImage ?? value;
					pixelSize = value.PixelSize;
					RenderingCalculator = value.RenderingCalculator;
				}
			}
		}

		private Image diffuseMap;
		internal Size pixelSize;

		/// <summary>
		/// Special constructor for creating custom shaders and images or reusing existing instances.
		/// </summary>
		public Material(Shader customShader, Image customDiffuseMap)
			: base("<GeneratedCustomMaterial:" + customShader + ":" + customDiffuseMap + ">")
		{
			Shader = customShader;
			DiffuseMap = customDiffuseMap;
			DefaultColor = Color.White;
			MetaData = new ContentMetaData();
			RenderingCalculator = new RenderingCalculator();
		}

		/// <summary>
		/// Creates an 1x1 image with a colored shader material for simple solid flat surfaces.
		/// </summary>
		public Material(Color nonUVShaderColor)
			: base("<GeneratedCustomMaterial:" + nonUVShaderColor + ">")
		{
			Shader = ContentLoader.Create<Shader>(new ShaderCreationData(ShaderFlags.Position2DColored));
			DiffuseMap = ContentLoader.Create<Image>(new ImageCreationData(Size.One));
			DiffuseMap.Fill(Color.White);
			pixelSize = Size.One;
			DefaultColor = nonUVShaderColor;
			MetaData = new ContentMetaData();
			RenderingCalculator = new RenderingCalculator();
		}

		/// <summary>
		/// Creates a new image with the specified size to be filled dynamically (via .DiffuseMap.Fill).
		/// </summary>
		public Material(Size customPixelSize, ShaderFlags shaderFlags = ShaderFlags.Position2DTextured)
			: base("<GeneratedCustomMaterial:" + shaderFlags + ":" + customPixelSize + ">")
		{
			Shader = ContentLoader.Create<Shader>(new ShaderCreationData(shaderFlags));
			DiffuseMap = ContentLoader.Create<Image>(new ImageCreationData(customPixelSize));
			pixelSize = customPixelSize;
			DefaultColor = Color.White;
			MetaData = new ContentMetaData();
			RenderingCalculator = new RenderingCalculator();
		}

		/// <summary>
		/// When using the Sprite(Material, Vector2D) constructor this size is used for the draw area.
		/// It is calculated from the DiffuseMap.PixelSize and the default content resolution, i.e.
		/// a 100x200 pixel image will be displayed aspect ratio correct relative to the window size.
		/// </summary>
		public Size MaterialRenderSize
		{
			get { return GetRenderSize(spriteSheet != null ? spriteSheet.SubImageSize : pixelSize); }
		}

		public Material(Color nonUvColor, ShaderFlags nonUvShaderFlags)
			: base("<GeneratedCustomMaterial:" + nonUvColor + ":" + nonUvShaderFlags + ">")
		{
			Shader = ContentLoader.Create<Shader>(new ShaderCreationData(nonUvShaderFlags));
			DefaultColor = nonUvColor;
			MetaData = new ContentMetaData();
			RenderingCalculator = new RenderingCalculator();
		}

		private Size GetRenderSize(Size imagePixelSize)
		{
			if (RenderSizeMode == RenderSizeMode.PixelBased)
				return ScreenSpace.Current.FromPixelSpace(imagePixelSize);
			if (RenderSizeMode == RenderSizeMode.SizeFor800X480)
				return imagePixelSize / new Size(800);
			if (RenderSizeMode == RenderSizeMode.SizeFor1024X768)
				return imagePixelSize / new Size(1024);
			if (RenderSizeMode == RenderSizeMode.SizeFor1280X720)
				return imagePixelSize / new Size(1280);
			if (RenderSizeMode == RenderSizeMode.SizeFor1920X1080)
				return imagePixelSize / new Size(1920);
			if (RenderSizeMode == RenderSizeMode.SizeForSettingsResolution)
				return GetRenderSizeBasedOnSettings(imagePixelSize);
			return imagePixelSize; // ncrunch: no coverage
		}

		public RenderSizeMode RenderSizeMode { get; set; }

		private static Size GetRenderSizeBasedOnSettings(Size pixelSize)
		{
			Settings settings = Settings.Current;
			var quadSize = new Size(Math.Max(settings.Resolution.Width, settings.Resolution.Height));
			return pixelSize / quadSize;
		}

		protected override void LoadData(Stream fileData)
		{
			var shaderFlags = MetaData.Get("ShaderFlags", ShaderFlags.Position2DTextured);
			Shader = ContentLoader.Create<Shader>(new ShaderCreationData(shaderFlags));
			DefaultColor = MetaData.Get("Color", Color.White);
			RenderSizeMode = MetaData.Get("RenderSizeMode", RenderSizeMode.PixelBased);
			LoadImageData();
		}

		private void LoadImageData()
		{
			string imageOrAnimationName = MetaData.Get("ImageOrAnimationName", "");
			if (string.IsNullOrEmpty(imageOrAnimationName))
				return; // ncrunch: no coverage
			DetermineUseCaseData(imageOrAnimationName);
			if (DiffuseMap != null)
				DiffuseMap.BlendMode = TryLoadBlendMode();
			LoadLightMap();
		}

		private BlendMode TryLoadBlendMode()
		{
			return MetaData.Get("BlendMode", "Normal").TryParse(BlendMode.Normal);
		}

		private void LoadLightMap()
		{
			var lightMapName = MetaData.Get("LightMapName", "");
			if (!string.IsNullOrEmpty(lightMapName))
				LightMap = ContentLoader.Load<Image>(lightMapName);
		}

		public Image LightMap { get; set; }

		protected override void DisposeData() {}

		public bool Equals(Material other)
		{
			return diffuseMap == other.diffuseMap && Shader.Equals(other.Shader) &&
				RenderSizeMode == other.RenderSizeMode && DefaultColor == other.DefaultColor &&
				animation == other.animation && Duration == other.Duration &&
				spriteSheet == other.spriteSheet && RenderingCalculator.Equals(other.RenderingCalculator);
		}

		public override string ToString()
		{
			return "Material: Shader=" + Shader + ", DiffuseMap=" + DiffuseMap + ", DefaultColor=" +
				DefaultColor + (Animation != null ? ", Animation=" + Animation : "") +
				(SpriteSheet != null ? ", SpriteSheet=" + SpriteSheet : "");
		}
	}
}