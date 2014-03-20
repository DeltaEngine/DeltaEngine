using System.Windows;
using ApprovalTests.Reporters;
using ApprovalTests.Wpf;
using NUnit.Framework;

namespace DeltaEngine.Editor.SampleBrowser.Tests
{
	[UseReporter(typeof(DiffReporter)), Category("Slow")]
	public class SampleItemViewTests
	{
		[Test]
		public void InitialView()
		{
			WpfApprovals.Verify(
				() => new Window { Content = new SampleItemView(), Width = 800, Height = 600 });
		}
	}
}