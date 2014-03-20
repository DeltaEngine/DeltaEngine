using System.Windows;
using NUnit.Framework;

namespace DeltaEngine.Editor.Emulator.Tests
{
	[RequiresSTA]
	public class ViewportControlTests
	{
		//ncrunch: no coverage start
		[Test, Category("Slow"), Category("WPF")]
		public void ShowMaterialEditorInWindow()
		{
			var window = new Window
			{
				Title = "My User Control Dialog",
				Content = new ViewportControl()
			};
			window.ShowDialog();
		}
	}
}