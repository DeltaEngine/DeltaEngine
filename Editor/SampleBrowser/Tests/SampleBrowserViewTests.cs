using System.Windows;
using ApprovalTests.Reporters;
using ApprovalTests.Wpf;
using NUnit.Framework;

namespace DeltaEngine.Editor.SampleBrowser.Tests
{
	/// <summary>
	/// Tests for the View of the Sample Browser.
	/// </summary>
	[UseReporter(typeof(DiffReporter)), Category("Slow")]
	public class SampleBrowserViewTests
	{
		[Test]
		public void InitialView()
		{
			WpfApprovals.Verify(
				() => new Window { Content = new SampleBrowserView(), Width = 800, Height = 600 });
		}
	}
}