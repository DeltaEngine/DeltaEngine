using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class MusicPreviewer : ContentPreview
	{
		protected override void Init() {}

		protected override void Preview(string contentName)
		{
			verdana = ContentLoader.Load<Font>("Verdana12");
			new FontText(verdana, "Play/Stop", Rectangle.One);
			music = ContentLoader.Load<Music>(contentName);
			music.Play(1);
			var trigger = new MouseButtonTrigger();
			trigger.AddTag("temporary");
			var musicCommand = new Command(() => //ncrunch: no coverage start
			{
				if (music.IsPlaying())
					music.Stop();
				else
					music.Play(1);
			}).Add(trigger);
			musicCommand.AddTag("temporary");
			//ncrunch: no coverage end
		}

		private Font verdana;
		public Music music;
	}
}