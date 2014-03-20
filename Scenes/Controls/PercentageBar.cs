using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Rendering2D.Shapes;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// Used to visualise how close to full or complete something is. eg. it could be used
	/// for a health bar floating above a creep or as a progress bar. The percentile colors are 
	/// lerped between; eg. Three colors would give the color at 0%, 50% and 100%
	/// </summary>
	public class PercentageBar : FilledRect
	{
		public PercentageBar(Rectangle maxDrawArea, Color[] percentileColors,
			HorizontalAlignment alignment = HorizontalAlignment.Left)
			: base(maxDrawArea, percentileColors[percentileColors.Length - 1])
		{
			minimum = 0.0f;
			maximum = 100.0f;
			value = 100.0f;
			maxWidth = maxDrawArea.Width;
			this.alignment = alignment;
			this.percentileColors = percentileColors;
		}

		private float minimum;
		private float maximum;
		private float value;
		private float maxWidth;
		private HorizontalAlignment alignment;
		private Color[] percentileColors;

		public float Percentage
		{
			get { return (value - minimum) / (maximum - minimum); }
		}

		public enum HorizontalAlignment
		{
			Left,
			Center
		}

		public float Minimum
		{
			get { return minimum; }
			set
			{
				minimum = value;
				ClampValueAndUpdateDrawAreaAndColor();
			}
		}

		private void ClampValueAndUpdateDrawAreaAndColor()
		{
			value = value.Clamp(minimum, maximum);
			UpdateDrawArea();
			UpdateColor();
		}

		private void UpdateDrawArea()
		{
			float width = maxWidth * Percentage;
			DrawArea = alignment == HorizontalAlignment.Left
				? new Rectangle(DrawArea.Left, DrawArea.Top, width, DrawArea.Height)
				: Rectangle.FromCenter(DrawArea.Center, new Size(width, DrawArea.Height));
		}

		private void UpdateColor()
		{
			if (percentileColors.Length == 1)
				return;
			float interval = 1.0f / (percentileColors.Length - 1);
			var lowerColor = (int)(Percentage * (percentileColors.Length - 1));
			int upperColor = lowerColor == percentileColors.Length - 1 ? lowerColor : lowerColor + 1;
			Color = percentileColors[lowerColor].Lerp(percentileColors[upperColor],
				(Percentage % interval) / interval);
		}

		public float Maximum
		{
			get { return maximum; }
			set
			{
				maximum = value;
				ClampValueAndUpdateDrawAreaAndColor();
			}
		}

		public float Value
		{
			get { return value; }
			set
			{
				this.value = value;
				ClampValueAndUpdateDrawAreaAndColor();
			}
		}

		public float MaxWidth
		{
			get { return maxWidth; }
			set
			{
				maxWidth = value;
				UpdateDrawArea();
			}
		}

		public HorizontalAlignment Alignment
		{
			get { return alignment; }
			set
			{
				alignment = value;
				UpdateDrawArea();
			}
		}

		public Color[] PercentileColors
		{
			get { return percentileColors; }
			set
			{
				percentileColors = value;
				if (percentileColors.Length == 1)
					Color = value[0];
				else
					UpdateColor();
			}
		}
	}
}