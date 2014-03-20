using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public sealed class SoundPreviewer : ContentPreview
	{
		protected override void Init() {}

		protected override void Preview(string contentName)
		{
			var verdana = ContentLoader.Load<Font>("Verdana12");
			new FontText(verdana, "Play", Rectangle.One);
			if (Sound != null)
				DisposeData();
			Sound = ContentLoader.Load<Sound>(contentName);
			trigger = new MouseButtonTrigger();
			trigger.AddTag("temporary");
			soundCommand = new Command(() => Sound.Play(1)).Add(trigger);
			soundCommand.AddTag("temporary");
		}

		public Sound Sound { get; private set; }
		private MouseButtonTrigger trigger;
		private Command soundCommand;

		private void DisposeData()
		{
			if (!Sound.IsDisposed)
				Sound.Dispose();
			trigger.IsActive = false;
			soundCommand.IsActive = false;
		}
	}
}