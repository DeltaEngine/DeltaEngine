using System.Windows.Forms;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class DownloadContentTooltipTests
	{
		//ncrunch: no coverage start
		[Test, Ignore]
		public void ShowDownloadContentTooltip()
		{
			Form form = new DownloadContentTooltip();
			form.Show();
			form.Refresh();
			bool isClosed = StackTraceExtensions.StartedFromNCrunchOrNunitConsole;
			form.Closed += (sender, args) => isClosed = true;
			while (!isClosed)
				Application.DoEvents();
			form.Close();
		}
	}
}