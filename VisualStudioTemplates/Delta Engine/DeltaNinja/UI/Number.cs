using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes;

namespace $safeprojectname$.UI
{
	internal class Number
	{
		public Number(Scene scene, Material[] materials, float left, float top, float height,
			Alignment align, int digitCount, Color color, GameRenderLayer layer = GameRenderLayer.Hud)
		{
			this.scene = scene;
			this.materials = materials;
			this.align = align;
			this.digitCount = digitCount;
			this.layer = layer;
			this.color = color;
			digitHeight = height;
			digitWidth = height / materials[0].DiffuseMap.PixelSize.AspectRatio;
			this.left = left;
			this.top = top;
		}

		private readonly Scene scene;
		private readonly Material[] materials;
		private readonly List<Sprite> sprites = new List<Sprite>();
		private readonly Alignment align;
		private readonly int digitCount;
		private readonly GameRenderLayer layer;
		private readonly Color color;
		private readonly float digitWidth;
		private readonly float digitHeight;

		private float left;
		public float Left
		{
			get { return left; }
			set
			{
				left = value;
				RefreshPosition();
			}
		}

		private float top;
		public float Top
		{
			get { return top; }
			set
			{
				top = value;
				RefreshPosition();
			}
		}

		public float Width
		{
			get { return digitWidth * sprites.Count; }
		}
		public float Height
		{
			get { return digitHeight; }
		}

		public void Show(int value = 0)
		{
			Hide();
			AddDigits(digitCount);
			SetValue(value);
		}

		public void Hide()
		{
			foreach (var sprite in sprites)
			{
				if (scene != null)
					scene.Remove(sprite);
				sprite.IsActive = false;
			}
			sprites.Clear();
		}

		public void Fade()
		{
			foreach (var sprite in sprites)
				sprite.Alpha -= GameSettings.FadeStep;
		}

		public void SetValue(int value)
		{
			var digits = new List<int>();
			var letters = value.ToString(CultureInfo.InvariantCulture).ToCharArray();
			digits.AddRange(letters.Select(letter => letter - '0'));
			if (digits.Count > sprites.Count)
				AddDigits(digits.Count);
			int n = sprites.Count - digits.Count;
			for (int i = 0; i < n; i++)
				digits.Insert(0, digitCount > 0 ? 0 : 10);
			for (int i = 0; i < sprites.Count; i++)
				sprites[i].Material = materials[digits[i]];
		}

		private void AddDigits(int count)
		{
			var empty = materials.Last();
			for (int i = sprites.Count; i < count; i++)
			{
				var digitArea = new Rectangle(left, top, digitWidth, digitHeight);
				var digit = new Sprite(empty, digitArea)
				{
					Color = color,
					IsVisible = true,
					RenderLayer = (int)layer
				};
				sprites.Insert(0, digit);
				if (scene != null)
					scene.Add(digit);
			}
			RefreshPosition();
		}

		private void RefreshPosition()
		{
			float x = left;
			switch (align)
			{
			case Alignment.Center:
				x -= (digitWidth * sprites.Count) / 2f;
				break;
			case Alignment.Right:
				x -= (digitWidth * sprites.Count);
				break;
			}
			foreach (var sprite in sprites)
			{
				sprite.TopLeft = new Vector2D(x, top);
				x += digitWidth;
			}
		}
	}
}