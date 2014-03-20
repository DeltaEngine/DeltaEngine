using System.Windows;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	[RequiresSTA]
	public class UIEditorViewTests
	{
		//ncrunch: no coverage start
		[Test, Category("Slow"), Category("WPF")]
		public void ShowMaterialEditorInWindow()
		{
			var window = new Window
			{
				Title = "My User Control Dialog",
				Content = new UIEditorView()
			};
			window.ShowDialog();
		}
	}
}