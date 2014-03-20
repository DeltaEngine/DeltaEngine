using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class ContentViewer
	{
		public ContentViewer()
		{
			viewers.Add(ContentType.Image, new ImagePreviewer());
			viewers.Add(ContentType.ImageAnimation, new ImageAnimationPreviewer());
			viewers.Add(ContentType.SpriteSheetAnimation, new SpriteSheetPreviewer());
			viewers.Add(ContentType.Material, new MaterialPreviewer());
			viewers.Add(ContentType.ParticleEmitter, new ParticlePreviewer());
			viewers.Add(ContentType.ParticleSystem, new ParticleSystemPreviewer());
			viewers.Add(ContentType.Font, new FontPreviewer());
			viewers.Add(ContentType.Sound, new SoundPreviewer());
			viewers.Add(ContentType.Music, new MusicPreviewer());
			viewers.Add(ContentType.Xml, new XmlPreviewer());
			viewers.Add(ContentType.Scene, new UIPreviewer());
			viewers.Add(ContentType.Mesh, new MeshPreviewer());
			viewers.Add(ContentType.Model, new ModelPreviewer());
			viewers.Add(ContentType.Level, new LevelPreviewer());
		}

		private readonly Dictionary<ContentType, ContentPreview> viewers =
			new Dictionary<ContentType, ContentPreview>();

		public void View(string contentName, ContentType type)
		{
			if (viewers.ContainsKey(type))
				viewers[type].PreviewContent(contentName);
			else
				ShowNoPreviewText(contentName);
		}

		protected virtual void ShowNoPreviewText(string contentName)
		{
			var verdana = ContentLoader.Load<Font>("Verdana12");
			new FontText(verdana, "No preview available of " + contentName, Rectangle.One);
		}
	}
}