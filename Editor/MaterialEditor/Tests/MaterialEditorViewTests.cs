using System.Windows;
using NUnit.Framework;

namespace DeltaEngine.Editor.MaterialEditor.Tests
{
	[RequiresSTA]
	public class MaterialEditorViewTests
	{
		//ncrunch: no coverage start
		[Test, Category("Slow"), Category("WPF")]
		public void ShowMaterialEditorInWindow()
		{
			var window = new Window
			{
				Title = "My User Control Dialog",
				Content = new MaterialEditorView()
			};
			window.ShowDialog();
		}
	}
}