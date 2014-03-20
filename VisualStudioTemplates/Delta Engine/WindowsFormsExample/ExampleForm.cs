using System.Windows.Forms;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Windows;
using DeltaEngine.Rendering2D.Shapes;

namespace $safeprojectname$
{
	public partial class ExampleForm : Form
	{
		public ExampleForm()
		{
			InitializeComponent();
			Show();
			new RenderApp(new RenderPanel(panel1));
		}
	}

	public class RenderPanel : FormsWindow
	{
		public RenderPanel(Control panel)
			: base(panel)
		{
		}
	}

	public class RenderApp : App
	{
		public RenderApp(RenderPanel renderPanel)
			: base(renderPanel)
		{
			new Line2D(Vector2D.Zero, Vector2D.One, Color.Yellow);
			Run();
		}
	}
}
