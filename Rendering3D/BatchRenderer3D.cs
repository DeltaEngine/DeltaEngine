using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Rendering3D
{
	public class BatchRenderer3D : BatchRenderer
	{
		public BatchRenderer3D(Drawing drawing)
			: base(drawing) {}

		protected override Batch GetNewInstanceOfBatch(Material material, BlendMode blendMode,
			int numberOfQuadsToAdd)
		{
			if (!(material.Shader as ShaderWithFormat).Format.Is3D)
				throw new BatchRenderer3DCannotBeUsedToRender2D(); //ncrunch: no coverage
			return new Batch3D(material, blendMode, numberOfQuadsToAdd);
		}

		public class BatchRenderer3DCannotBeUsedToRender2D : Exception {}
	}
}