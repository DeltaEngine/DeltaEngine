using System.Windows;
using NUnit.Framework;

namespace DeltaEngine.Editor.Emulator.Tests
{
	public class WpfHostedFormsWindowTests
	{
		//ncrunch: no coverage start
		[Test, Category("Slow"), RequiresSTA]
		public void CreateNewWindow()
		{
			var window = new Window();
			new WpfHostedFormsWindow(new ViewportControl(), window);
		}

		[Test, Category("Slow"), RequiresSTA]
		public void AddLineInDeltaEngineWillShowLineInWpf()
		{
			var window = new Window { Title = "My User Control Dialog", Content = new MainWindow() };
			window.ShowDialog();
		}

		[Test]
		public void UseViewModelLocator()
		{
			var viewModel = new ViewModelLocator();
			Assert.IsNotNull(viewModel.Main);
		}
	}
}