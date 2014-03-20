using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// Allows a range of values to be selected via a visible slider.
	/// </summary>
	public class Slider : BaseSlider
	{
		internal protected Slider() {}

		public Slider(Rectangle drawArea)
			: this(new Theme(), drawArea) { }

		public Slider(Theme theme, Rectangle drawArea)
			: base(theme, theme.Slider, drawArea)
		{
			var data = new Data { MinValue = 0, Value = 100, MaxValue = 100 };
			Add(data);
			Add(new Picture(theme, theme.SliderPointer, Rectangle.Unused));
			AddChild(Pointer);
		}

		internal class Data
		{
			public int MinValue { get; set; }
			public int Value { get; set; }
			public int MaxValue { get; set; }
		}

		protected override void UpdateSliderAppearance()
		{
			SetAppearance(IsEnabled ? Theme.Slider : Theme.SliderDisabled);
		}

		protected override void UpdatePointerAppearance()
		{
			if (!Pointer.IsEnabled)
				Pointer.SetAppearance(Theme.SliderPointerDisabled);
			else if (Pointer.State.IsInside || Pointer.State.IsPressed)
				Pointer.SetAppearance(Theme.SliderPointerMouseover);
			else
				Pointer.SetAppearance(Theme.SliderPointer);
		}

		protected override void UpdatePointerValue()
		{
			if (!State.IsPressed)
				return;
			float percentage = State.RelativePointerPosition.X.Clamp(0.0f, 1.0f);
			float aspectRatio = Pointer.Material.DiffuseMap != null
				? Pointer.Material.DiffuseMap.PixelSize.AspectRatio : DefaultPointerAspectRatio;
			float unusable = aspectRatio / DrawArea.Aspect;
			float expandedPercentage = ((percentage - 0.5f) * (1.0f + unusable) + 0.5f).Clamp(0.0f, 1.0f);
			Value = (MinValue + expandedPercentage * (MaxValue - MinValue)).Round();
			if (Value == lastPointerValue)
				return;
			lastPointerValue = Value;
			if (ValueChanged != null)
				ValueChanged(Value);
		}

		private const float DefaultPointerAspectRatio = 0.5f;

		private int lastPointerValue = int.MaxValue;

		public Action<int> ValueChanged;

		protected override void UpdatePointerDrawArea()
		{
			Rectangle drawArea = DrawArea;
			float aspectRatio = Pointer.Material.DiffuseMap != null
				? Pointer.Material.DiffuseMap.PixelSize.AspectRatio : DefaultPointerAspectRatio;
			var size = new Size(aspectRatio * drawArea.Height, drawArea.Height);
			float percentage = (Value - MinValue) / (float)(MaxValue - MinValue);
			var leftCenter = new Vector2D(drawArea.Left + size.Width / 2, drawArea.Center.Y);
			var rightCenter = new Vector2D(drawArea.Right - size.Width / 2, drawArea.Center.Y);
			Pointer.DrawArea = Rectangle.FromCenter(leftCenter.Lerp(rightCenter, percentage), size);
		}

		public int MinValue
		{
			get { return Get<Data>().MinValue; }
			set { Get<Data>().MinValue = value; }
		}

		public int Value
		{
			get { return Get<Data>().Value; }
			set { Get<Data>().Value = value; }
		}

		public int MaxValue
		{
			get { return Get<Data>().MaxValue; }
			set { Get<Data>().MaxValue = value; }
		}
	}
}