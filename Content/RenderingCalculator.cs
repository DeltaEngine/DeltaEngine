using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Given an AtlasRegion and desired UV, DrawArea and FlipMode, it will return the true UV
	/// and DrawArea.
	/// </summary>
	public class RenderingCalculator : IEquatable<RenderingCalculator>
	{
		internal RenderingCalculator()
			: this(new AtlasRegion(Rectangle.One)) {}

		public RenderingCalculator(AtlasRegion region)
		{
			uv = region.UV;
			padLeft = region.PadLeft;
			padRight = region.PadRight;
			padTop = region.PadTop;
			padBottom = region.PadBottom;
			isRotated = region.IsRotated;
			hasNoPadding = padLeft == 0 && padRight == 0 && padTop == 0 && padBottom == 0;
			hasNoAtlas = uv == Rectangle.One && hasNoPadding && !isRotated;
			originalWidth = uv.Size.Width + padLeft + padRight;
			originalHeight = uv.Size.Height + padTop + padBottom;
		}

		private readonly Rectangle uv;
		private readonly float padLeft;
		private readonly float padRight;
		private readonly float padTop;
		private readonly float padBottom;
		private readonly bool hasNoPadding;
		private readonly bool hasNoAtlas;
		private readonly float originalWidth;
		private readonly float originalHeight;
		private readonly bool isRotated;

		public RenderingData GetUVAndDrawArea(Rectangle userUV, Rectangle drawArea, 
			FlipMode flipMode = FlipMode.None)
		{
			if (hasNoAtlas)
				return GetUVAndDrawAreaWhenNoAtlas(userUV, drawArea, flipMode);
			if (isRotated)
				return GetUVAndDrawAreaWhenRotated(userUV, drawArea, flipMode);
			return GetUVAndDrawAreaWhenNotRotated(userUV, drawArea, flipMode);
		}

		private static RenderingData GetUVAndDrawAreaWhenNoAtlas(Rectangle userUV, Rectangle drawArea,
			FlipMode flipMode)
		{
			return new RenderingData
			{
				RequestedUserUV = userUV,
				RequestedDrawArea = drawArea,
				AtlasUV = Flip(userUV, flipMode),
				FlipMode = flipMode,
				DrawArea = drawArea,
				HasSomethingToRender = true
			};
		}

		private static Rectangle Flip(Rectangle uv, FlipMode flipMode)
		{
			if (flipMode == FlipMode.HorizontalAndVertical)
				return new Rectangle(uv.Right, uv.Bottom, -uv.Width, -uv.Height);
			if (flipMode == FlipMode.Horizontal)
				return new Rectangle(uv.Right, uv.Top, -uv.Width, uv.Height);
			if (flipMode == FlipMode.Vertical)
				return new Rectangle(uv.Left, uv.Bottom, uv.Width, -uv.Height);
			return uv;
		}

		public RenderingData GetUVAndDrawAreaWhenNotRotated(Rectangle userUV, Rectangle drawArea,
			FlipMode flipMode)
		{
			if (hasNoPadding)
				return GetUVAndDrawAreaWhenNoPaddingAndNotRotated(userUV, drawArea, flipMode);
			return GetUVAndDrawAreaWhenHasPaddingButNotRotated(userUV, drawArea, flipMode);
		}

		private RenderingData GetUVAndDrawAreaWhenNoPaddingAndNotRotated(Rectangle userUV,
			Rectangle drawArea, FlipMode flipMode)
		{
			return new RenderingData
			{
				RequestedUserUV = userUV,
				RequestedDrawArea = drawArea,
				AtlasUV = Flip(uv.GetInnerRectangle(userUV), flipMode),
				FlipMode = flipMode,
				DrawArea = drawArea,
				HasSomethingToRender = true
			};
		}

		private RenderingData GetUVAndDrawAreaWhenHasPaddingButNotRotated(Rectangle userUV,
			Rectangle drawArea, FlipMode flipMode)
		{
			Rectangle expandedUV = GetExpandedUV();
			Rectangle expandedUserUV = GetExpandedUserUV(userUV, expandedUV);
			if (HasNoRendering(expandedUserUV))
				return new RenderingData // ncrunch: no coverage
				{ RequestedUserUV = userUV, RequestedDrawArea = drawArea, FlipMode = flipMode, };
			Rectangle culledUserUV = GetCulledUserUV(expandedUserUV);
			return new RenderingData
			{
				RequestedUserUV = userUV,
				RequestedDrawArea = drawArea,
				FlipMode = flipMode,
				AtlasUV = Flip(GetAtlasUV(culledUserUV), flipMode),
				DrawArea = GetDrawArea(expandedUserUV, drawArea),
				HasSomethingToRender = true
			};
		}

		private Rectangle GetExpandedUV()
		{
			float left = -padLeft / originalWidth;
			float right = 1 + padRight / originalWidth;
			float top = -padTop / originalHeight;
			float bottom = 1 + padBottom / originalHeight;
			return new Rectangle(left, top, right - left, bottom - top);
		}

		private static Rectangle GetExpandedUserUV(Rectangle userUV, Rectangle expandedUV)
		{
			float left = expandedUV.Left.Lerp(expandedUV.Right, userUV.Left);
			float right = expandedUV.Left.Lerp(expandedUV.Right, userUV.Right);
			float top = expandedUV.Top.Lerp(expandedUV.Bottom, userUV.Top);
			float bottom = expandedUV.Top.Lerp(expandedUV.Bottom, userUV.Bottom);
			return new Rectangle(left, top, right - left, bottom - top);
		}

		private static bool HasNoRendering(Rectangle expandedUserUV)
		{
			if (expandedUserUV.Left < 0 && expandedUserUV.Right < 0)
				return true; // ncrunch: no coverage
			if (expandedUserUV.Left > 1 && expandedUserUV.Right > 1)
				return true; // ncrunch: no coverage
			if (expandedUserUV.Top < 0 && expandedUserUV.Bottom < 0)
				return true; // ncrunch: no coverage
			if (expandedUserUV.Top > 1 && expandedUserUV.Bottom > 1)
				return true; // ncrunch: no coverage
			return false;
		}

		private static Rectangle GetCulledUserUV(Rectangle expandedUserUV)
		{
			float left = MathExtensions.Max(expandedUserUV.Left, 0.0f);
			float right = MathExtensions.Min(expandedUserUV.Right, 1.0f);
			float top = MathExtensions.Max(expandedUserUV.Top, 0.0f);
			float bottom = MathExtensions.Min(expandedUserUV.Bottom, 1.0f);
			return new Rectangle(left, top, right - left, bottom - top);
		}

		private Rectangle GetAtlasUV(Rectangle culledUserUV)
		{
			float atlasLeft = uv.Left.Lerp(uv.Right, culledUserUV.Left);
			float atlasRight = uv.Left.Lerp(uv.Right, culledUserUV.Right);
			float atlasTop = uv.Top.Lerp(uv.Bottom, culledUserUV.Top);
			float atlasBottom = uv.Top.Lerp(uv.Bottom, culledUserUV.Bottom);
			return new Rectangle(atlasLeft, atlasTop, atlasRight - atlasLeft, atlasBottom - atlasTop);
		}

		private static Rectangle GetDrawArea(Rectangle expandedUserUV, Rectangle drawArea)
		{
			float drawAreaPadLeft = MathExtensions.Max(-expandedUserUV.Left, 0) * drawArea.Width /
				expandedUserUV.Width;
			float drawAreaPadRight = MathExtensions.Max((expandedUserUV.Right - 1), 0) * drawArea.Width /
				expandedUserUV.Width;
			float drawAreaPadTop = MathExtensions.Max(-expandedUserUV.Top, 0) * drawArea.Height /
				expandedUserUV.Height;
			float drawAreaPadBottom = MathExtensions.Max((expandedUserUV.Bottom - 1), 0) *
				drawArea.Height / expandedUserUV.Height;
			float paddedDrawAreaLeft = drawArea.Left + drawAreaPadLeft;
			float paddedDrawAreaRight = drawArea.Right - drawAreaPadRight;
			float paddedDrawAreaTop = drawArea.Top + drawAreaPadTop;
			float paddedDrawAreaBottom = drawArea.Bottom - drawAreaPadBottom;
			return new Rectangle(paddedDrawAreaLeft, paddedDrawAreaTop,
				paddedDrawAreaRight - paddedDrawAreaLeft, paddedDrawAreaBottom - paddedDrawAreaTop);
		}

		private RenderingData GetUVAndDrawAreaWhenRotated(Rectangle userUV, Rectangle drawArea,
			FlipMode flipMode)
		{
			var rotatedUserUV = new Rectangle(userUV.Top, 1 - userUV.Left - userUV.Width, userUV.Height,
				userUV.Width);
			var data = GetUVAndDrawAreaWhenNotRotated(rotatedUserUV, drawArea, flipMode);
			data.IsAtlasRotated = true;
			return data;
		}

		public bool Equals(RenderingCalculator other)
		{
			return uv == other.uv;
		}
	}
}