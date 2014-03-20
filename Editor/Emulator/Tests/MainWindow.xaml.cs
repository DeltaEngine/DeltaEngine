using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D.Shapes;
using Color = DeltaEngine.Datatypes.Color;
using EngineApp = DeltaEngine.Platforms.App;
using Window = DeltaEngine.Core.Window;

namespace DeltaEngine.Editor.Emulator.Tests
{
	/// <summary>
	/// Hosts the Viewport as UserControl
	/// </summary>
	public partial class MainWindow
	{
		//ncrunch: no coverage start
		public MainWindow()
		{
			InitializeComponent();
			if (DesignerProperties.GetIsInDesignMode(this))
				return;
			Show();
			var window = new WpfHostedFormsWindow(TestControl.EngineViewport, this);
			Closing += (sender, args) => window.Dispose();
			new BlockingViewportApp(window);
		}

		private class BlockingViewportApp : EngineApp
		{
			public BlockingViewportApp(Window windowToRegister)
				: base(windowToRegister)
			{
				Run();
			}
		}

		private void OnClickButton(object sender, RoutedEventArgs e)
		{
			TestButton.Background = new SolidColorBrush(Colors.Yellow);
			new Line2D(new Vector2D(0, 1), new Vector2D(Time.Total / 5f, 0), Color.Yellow);
		}
	}
}