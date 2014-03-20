using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.ScreenSpaces;
using Spine;
using AtlasRegion = Spine.AtlasRegion;

namespace DeltaEngine.Rendering2D.Spine
{
	internal class SpineRenderer : DrawBehavior
	{
		public SpineRenderer(BatchRenderer2D renderer)
		{
			this.renderer = renderer;
			Bone.yDown = true;
		}

		protected readonly BatchRenderer2D renderer;

		public void Draw(List<DrawableEntity> visibleEntities)
		{
			currentBatch = null;
			foreach (SpineSkeleton skeleton in visibleEntities)
				DrawSkeleton(skeleton);
		}

		private Batch2D currentBatch;

		private void DrawSkeleton(SpineSkeleton skeleton)
		{
			var drawArea = skeleton.Get<Rectangle>();
			origin = ScreenSpace.Current.ToPixelSpaceRounded(drawArea.TopLeft);
			scale = drawArea.Size;
			color = skeleton.Get<Color>();
			ProcessRotation(skeleton.Get<float>());
			DrawSlots(skeleton.skeleton);
		}

		private void ProcessRotation(float rotation)
		{
			isRotated = rotation != 0.0f;
			if (!isRotated)
				return;
			sin = MathExtensions.Sin(rotation);
			cos = MathExtensions.Cos(rotation);
		}

		private Vector2D origin;
		private Size scale;
		private Color color;
		private bool isRotated;
		private float sin;
		private float cos;

		private void DrawSlots(Skeleton skeleton)
		{
			isNormalTriangleWindingOrder = skeleton.FlipX ^ skeleton.FlipY;
			List<Slot> drawOrder = skeleton.DrawOrder;
			foreach (Slot slot in drawOrder)
			{
				var attachment = slot.Attachment as RegionAttachment;
				if (attachment != null)
					DrawSlot(skeleton, attachment, slot);
			}
		}

		private bool isNormalTriangleWindingOrder;

		private void DrawSlot(Skeleton skeleton, RegionAttachment attachment, Slot slot)
		{
			var region = (AtlasRegion)attachment.RendererObject;
			var thisMaterial = (Material)region.page.rendererObject;
			var thisBlendMode = slot.Data.AdditiveBlending ? BlendMode.Additive : BlendMode.Normal;
			var thisBatch = (Batch2D)renderer.FindOrCreateBatch(thisMaterial, thisBlendMode);
			if (currentBatch != null && currentBatch != thisBatch)
				renderer.FlushDrawBuffer(currentBatch);
			currentBatch = thisBatch;
			attachment.ComputeWorldVertices(skeleton.X, skeleton.Y, slot.Bone, vertices);
			currentColor = new Color(color.RedValue * slot.R, color.GreenValue * slot.G,
				color.BlueValue * slot.B, color.AlphaValue * slot.A);
			AddSlotIndicesAndVertices(attachment.UVs);
		}

		private readonly float[] vertices = new float[8];
		private Color currentColor;

		private void AddSlotIndicesAndVertices(float[] uvs)
		{
			if (isNormalTriangleWindingOrder)
				currentBatch.AddIndices();
			else
				currentBatch.AddIndicesReversedWinding();
			currentBatch.verticesColorUV[currentBatch.verticesIndex++] = GetTopLeftVertex(uvs);
			currentBatch.verticesColorUV[currentBatch.verticesIndex++] = GetBottomLeftVertex(uvs);
			currentBatch.verticesColorUV[currentBatch.verticesIndex++] = GetBottomRightVertex(uvs);
			currentBatch.verticesColorUV[currentBatch.verticesIndex++] = GetTopRightVertex(uvs);
		}

		private VertexPosition2DColorUV GetTopLeftVertex(float[] uvs)
		{
			var position = GetScaledRotatedPosition(RegionAttachment.X1, RegionAttachment.Y1);
			var uv = new Vector2D(uvs[RegionAttachment.X1], uvs[RegionAttachment.Y1]);
			return new VertexPosition2DColorUV(position, currentColor, uv);
		}

		private Vector2D GetScaledRotatedPosition(int xIndex, int yIndex)
		{
			var position = new Vector2D(vertices[xIndex] * scale.Width, vertices[yIndex] * scale.Height);
			if (isRotated)
				position = position.RotateAround(Vector2D.Zero, sin, cos);
			return origin + position;
		}

		private VertexPosition2DColorUV GetTopRightVertex(float[] uvs)
		{
			var position = GetScaledRotatedPosition(RegionAttachment.X4, RegionAttachment.Y4);
			var uv = new Vector2D(uvs[RegionAttachment.X4], uvs[RegionAttachment.Y4]);
			return new VertexPosition2DColorUV(position, currentColor, uv);
		}

		private VertexPosition2DColorUV GetBottomRightVertex(float[] uvs)
		{
			var position = GetScaledRotatedPosition(RegionAttachment.X3, RegionAttachment.Y3);
			var uv = new Vector2D(uvs[RegionAttachment.X3], uvs[RegionAttachment.Y3]);
			return new VertexPosition2DColorUV(position, currentColor, uv);
		}

		private VertexPosition2DColorUV GetBottomLeftVertex(float[] uvs)
		{
			var position = GetScaledRotatedPosition(RegionAttachment.X2, RegionAttachment.Y2);
			var uv = new Vector2D(uvs[RegionAttachment.X2], uvs[RegionAttachment.Y2]);
			return new VertexPosition2DColorUV(position, currentColor, uv);
		}
	}
}