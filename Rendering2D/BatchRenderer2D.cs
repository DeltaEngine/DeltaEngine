using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering2D
{
	public class BatchRenderer2D : BatchRenderer
	{
		public BatchRenderer2D(Drawing drawing)
			: base(drawing) {}

		protected override Batch GetNewInstanceOfBatch(Material material, BlendMode blendMode,
			int numberOfQuadsToAdd)
		{
			if ((material.Shader as ShaderWithFormat).Format.Is3D)
				throw new BatchRenderer2DCannotBeUsedToRender3D(); //ncrunch: no coverage
			return new Batch2D(material, blendMode, numberOfQuadsToAdd);
		}

		public class BatchRenderer2DCannotBeUsedToRender3D : Exception {}
	}
}