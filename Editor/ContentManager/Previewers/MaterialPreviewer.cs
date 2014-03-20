using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class MaterialPreviewer : ContentPreview
	{
		protected override void Init() {}

		protected override void Preview(string contentName)
		{
			var material = ContentLoader.Load<Material>(contentName);
			var shaderWithFormat = material.Shader as ShaderWithFormat;
			if (!shaderWithFormat.Format.Is3D)
			{
				var imageSize = material.DiffuseMap.PixelSize;
				var aspectRatio = imageSize.Height / imageSize.Width;
				currentDisplayEntity = new Sprite(material,
					Rectangle.FromCenter(new Vector2D(0.5f, 0.5f), new Size(0.5f, 0.5f * aspectRatio)));
			}
			else if (shaderWithFormat.Format.HasUV)
				currentDisplayEntity = new Billboard(Vector3D.Zero, Size.One, material);
		}

		private Entity currentDisplayEntity;
	}
}