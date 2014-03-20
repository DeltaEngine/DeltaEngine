using System.Windows;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests
{
	[RequiresSTA]
	public class ContentManagerViewTests
	{
		//ncrunch: no coverage start
		[Test, Category("Slow"), Category("WPF")]
		public void ShowMaterialEditorInWindow()
		{
			var window = new Window
			{
				Title = "My User Control Dialog",
				Content = new ContentManagerView()
			};
			window.ShowDialog();
		}
	}
}
