using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// For example, the horizontal and vertical scrollbars on a browser, document etc.
	/// </summary>
	public class Scrollbar : BaseSlider
	{
		protected Scrollbar() {}

		public Scrollbar(Rectangle drawArea)
			: this(new Theme(), drawArea) { }

		public Scrollbar(Theme theme, Rectangle drawArea)
			: base(theme, theme.Scrollbar, drawArea)
		{
			var data = new Data { MinValue = 0, MaxValue = 99, LeftValue = 90, ValueWidth = 10 };
			Add(data);
			Add(new Picture(theme, theme.ScrollbarPointer, Rectangle.Unused));
			AddChild(Pointer);
		}

		private class Data
		{
			public int MinValue { get; set; }
			public int MaxValue { get; set; }
			public int LeftValue { get; set; }
			public int ValueWidth { get; set; }
		}

		protected override void UpdateSliderAppearance()
		{
			SetAppearance(IsEnabled ? Theme.Scrollbar : Theme.ScrollbarDisabled);
		}

		protected override void UpdatePointerAppearance()
		{
			if (!Pointer.IsEnabled)
				Pointer.SetAppearance(Theme.ScrollbarPointerDisabled);
			else if (Pointer.State.IsInside || Pointer.State.IsPressed)
				Pointer.SetAppearance(Theme.ScrollbarPointerMouseover);
			else
				Pointer.SetAppearance(Theme.ScrollbarPointer);
		}

		protected override void UpdatePointerValue()
		{
			if (!State.IsPressed)
				return;
			float percentage = State.RelativePointerPosition.X.Clamp(0.0f, 1.0f);
			CenterValue = (int)(MinValue + percentage * (MaxValue - MinValue + 1));
		}

		private float GetPointerPercentageWidth()
		{
			var percentageWidth = (float)ValueWidth / (MaxValue - MinValue + 1);
			return percentageWidth.Clamp(MinimumPointerPercentageWidth, MaximumPointerPercentageWidth);
		}

		private const float MinimumPointerPercentageWidth = 0.05f;
		private const float MaximumPointerPercentageWidth = 1.0f;

		protected override void UpdatePointerDrawArea()
		{
			Rectangle drawArea = DrawArea;
			var size = new Size(drawArea.Width * GetPointerPercentageWidth(), drawArea.Height);
			float percentage = (LeftValue - MinValue) / (float)(MaxValue - MinValue - ValueWidth + 1);
			var min = new Vector2D(drawArea.Left, drawArea.Top);
			var max = new Vector2D(drawArea.Right - size.Width, drawArea.Top);
			Pointer.DrawArea = new Rectangle(min.Lerp(max, percentage), size);
		}

		public int CenterValue
		{
			get { return LeftValue + ValueWidth / 2; }
			set
			{
				int leftValue = value - ValueWidth / 2;
				if (leftValue + ValueWidth - 1 > MaxValue)
					leftValue = MaxValue - ValueWidth + 1;
				if (leftValue < MinValue)
					leftValue = MinValue;
				Get<Data>().LeftValue = leftValue;
			}
		}

		public int LeftValue
		{
			get { return Get<Data>().LeftValue; }
		}

		public int ValueWidth
		{
			get { return Get<Data>().ValueWidth; }
			set
			{
				Get<Data>().ValueWidth = value;
				CenterValue = CenterValue;
			}
		}

		public int MinValue
		{
			get { return Get<Data>().MinValue; }
			set
			{
				Get<Data>().MinValue = value;
				CenterValue = CenterValue;
			}
		}

		public int MaxValue
		{
			get { return Get<Data>().MaxValue; }
			set
			{
				Get<Data>().MaxValue = value;
				CenterValue = CenterValue;
			}
		}

		public int RightValue
		{
			get { return LeftValue + ValueWidth - 1; }
		}
	}
}