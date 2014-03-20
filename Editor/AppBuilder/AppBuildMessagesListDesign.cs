using System.Collections.Generic;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// Helper class for an easier view modeling at design time.
	/// </summary>
	public class AppBuildMessagesListDesign
	{
		public AppBuildMessagesListDesign()
		{
			var appBuildMessage = new AppBuildMessage("AppBuilderMessage");
			appBuildMessage.Project = "TestProject";
			appBuildMessage.Filename = "TestFile";
			appBuildMessage.TextLine = "23";
			appBuildMessage.TextColumn = "12";
			MessagesMatchingCurrentFilter = new List<AppBuildMessageViewModel> {
				new AppBuildMessageViewModel(appBuildMessage) };
		}

		public List<AppBuildMessageViewModel> MessagesMatchingCurrentFilter { get; set; }
	}
}