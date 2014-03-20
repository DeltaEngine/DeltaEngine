using System.Drawing;
using System.Windows.Forms;
using DeltaEngine.Extensions;

namespace DeltaEngine.Platforms
{
	internal class DownloadContentTooltip : Form
	{
		//ncrunch: no coverage start
		public DownloadContentTooltip()
		{
			SuspendLayout();
			Label message = CreateMessage();
			CreateWindow(message);
			ResumeLayout(false);
			PerformLayout();
		}

		private static Label CreateMessage()
		{
			return new Label
			{
				AutoSize = true,
				Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, 0),
				Location = new Point(13, 13),
				Size = new Size(461, 20),
				TabIndex = 0,
				Text = "No content available. Waiting until OnlineService sends it to us ..."
			};
		}

		private void CreateWindow(Control message)
		{
			AutoScaleDimensions = new SizeF(6F, 13F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(484, 41);
			Controls.Add(message);
			FormBorderStyle = FormBorderStyle.FixedToolWindow;
			StartPosition = FormStartPosition.CenterScreen;
			Text = AssemblyExtensions.GetEntryAssemblyForProjectName();
		}
	}
}