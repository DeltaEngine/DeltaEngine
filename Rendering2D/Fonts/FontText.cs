using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Fonts
{
	/// <summary>
	/// Entity used to render text on screen. Can be aligned horizontally and vertically within a
	/// draw area. Can also contain line breaks.
	/// </summary>
	public class FontText : Entity2D
	{
		internal protected FontText() {}

		public FontText(Font font, string text, Rectangle drawArea)
			: base(drawArea)
		{
			RenderLayer = DefaultFontRenderLayer;
			// ReSharper disable DoNotCallOverridableMethodsInConstructor once
			Add(text);
			if (font.WasLoadedOk)
				RenderAsFontText(font);
			else
				RenderAsVectorText();
			Add(new FontName(font.Name));
		}

		public const int DefaultFontRenderLayer = 100;

		internal class FontName
		{
			protected FontName() {}

			public FontName(string name)
			{
				if (name.StartsWith("<GeneratedMockFont:"))
					name = name.Substring(19, name.Length - 20);
				Name = name;
			}

			public string Name { get; private set; }
		}

		public string Text
		{
			get { return Get<string>(); }
			set
			{
				Set(value);
				if (WasLoadedOk)
					UpdateFontTextRendering();
				else
					Get<VectorText.Data>().Text = value;
			}
		}

		private bool WasLoadedOk
		{
			get { return Contains<GlyphDrawData[]>(); }
		}

		private void RenderAsFontText(Font font)
		{
			description = font.Description;
			description.Generate(Text, HorizontalAlignment.Center);
			Add(font.Material);
			Add(description.Glyphs);
			Add(description.DrawSize);
			OnDraw<FontRenderer>();
		}

		internal FontDescription description;

	  private void RenderAsVectorText()
		{
			Add(new VectorText.Data(Text));
			Start<VectorText.ProcessText>();
			OnDraw<VectorText.Render>();
		}

		private void UpdateFontTextRendering()
		{
			description.Generate(Text, HorizontalAlignment);
			Set(description.Glyphs);
			SetWithoutInterpolation(description.DrawSize);
		}

		public HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return Contains<HorizontalAlignment>()
					? Get<HorizontalAlignment>() : HorizontalAlignment.Center;
			}
			set
			{
				Set(value);
				if (WasLoadedOk)
					UpdateFontTextRendering();
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			get
			{
				return Contains<VerticalAlignment>() ? Get<VerticalAlignment>() : VerticalAlignment.Center;
			}
			set
			{
				Set(value);
				if (WasLoadedOk)
					UpdateFontTextRendering();
			}
		}

		public override void Set(object component)
		{
			var fontName = component as FontName;
			if (component is FontName)
				description = ContentLoader.Load<Font>(fontName.Name).Description;
			base.Set(component);
		}

		internal Material CachedMaterial
		{
			get { return cachedMaterial ?? (cachedMaterial = Get<Material>()); }
		}
		private Material cachedMaterial;
	}
}