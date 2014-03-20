using System.Windows;
using NUnit.Framework;

namespace DeltaEngine.Editor.ImageAnimationEditor.Tests
{
	[RequiresSTA]
	public class AnimationEditorViewTests
	{
		//ncrunch: no coverage start
		[Test, Category("Slow"), Category("WPF")]
		public void ShowMaterialEditorInWindow()
		{
			var window = new Window
			{
				Title = "My User Control Dialog",
				Content = new AnimationEditorView()
			};
			window.ShowDialog();
		}
	}
}