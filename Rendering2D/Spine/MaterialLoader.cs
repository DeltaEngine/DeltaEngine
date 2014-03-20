using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using Spine;
using System;
using System.IO;

namespace DeltaEngine.Rendering2D.Spine
{
	internal class MaterialLoader : TextureLoader
	{
		public void Load(AtlasPage page, String path)
		{
			var image = ContentLoader.Load<Image>(Path.GetFileNameWithoutExtension(path));
			var shader =
				ContentLoader.Create<Shader>(new ShaderCreationData(ShaderFlags.Position2DColoredTextured));
			var material = new Material(shader, image);
			page.rendererObject = material;
			Size size = material.DiffuseMap.PixelSize;
			page.width = (int)size.Width;
			page.height = (int)size.Height;
		}

		public void Unload(Object material) {}
	}
}