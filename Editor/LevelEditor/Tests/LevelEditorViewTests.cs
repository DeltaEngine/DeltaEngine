using System.Windows;
using NUnit.Framework;

namespace DeltaEngine.Editor.LevelEditor.Tests
{
	[RequiresSTA]
	public class LevelEditorViewTests
	{
		//ncrunch: no coverage start
		[Test, Category("Slow"), Category("WPF")]
		public void ShowMaterialEditorInWindow()
		{
			var window = new Window
			{
				Title = "My User Control Dialog",
				Content = new LevelEditorView()
			};
			window.ShowDialog();
		}
	}
}