using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using DeltaEngine.Core;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Emulator;
using DeltaEngine.Editor.Frameworks;
using DeltaEngine.Editor.Helpers;
using DeltaEngine.Extensions;
using DeltaEngine.Platforms;
using Xceed.Wpf.AvalonDock.Layout;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor
{
	/// <summary>
	/// The editor main window manages the login and plugin selection (see EditorWindowModel).
	/// </summary>
	public partial class EditorView
	{
		public EditorView()
			: this(new EditorViewModel()) {}

		public EditorView(EditorViewModel viewModel)
		{
			InitializeComponent();
			viewModel.TextLogger.NewLogMessage +=
				() => Dispatcher.BeginInvoke(new Action(LogOutput.ScrollToEnd));
			Loaded += LoadSettings;
			Closing += SaveSettings;
			DataContext = this.viewModel = viewModel;
			viewModel.maximizer = new MaximizerForEmptyWindows(this);
			HandleStartupArguments();
			StartEngineAndBlock();
		}

		private void LoadSettings(object sender, RoutedEventArgs e)
		{
			if (!Settings.Current.CustomSettingsExists)
				return;
			Width = Settings.Current.Resolution.Width;
			Height = Settings.Current.Resolution.Height;
			if (viewModel.StartEditorMaximized)
				viewModel.maximizer.MaximizeWindow();
		}

		private void SaveSettings(object sender, CancelEventArgs e)
		{
			viewModel.StartEditorMaximized = viewModel.maximizer.isMaximized;
			Settings.Current.Resolution = new Size((float)Width, (float)Height);
			Settings.Current.Dispose();
		}

		private readonly EditorViewModel viewModel;

		private void LoadEditorPlugins()
		{
			viewModel.AddAllPlugins();
			viewModel.Service.StartEditorPlugin += type => StartEditorPlugin(GetPluginByType(type));
		}

		private UserControl GetPluginByType(Type type)
		{
			foreach (var plugin in viewModel.EditorPlugins.Where(plugin => plugin.GetType() == type))
				return plugin as UserControl;
			return null;
		}

		private void StartEditorPlugin(UserControl plugin)
		{
			EditorPluginSelection.SelectedItem = null;
			if (CheckIfPluginIsAlreadyRunningAndFocus(plugin))
				return;
			if (viewport != null)
			{
				viewport.DestroyRenderedEntities();
				viewport.ResetViewportArea();
			}
			if (!InitializePlugin(plugin))
				return;
			var document = CreateDocumentForPlugin(plugin);
			var pane = CreatePaneForPlugins(plugin);
			pane.Children.Add(document);
			FocusDocumentToSeePlugin(document);
		}

		private bool CheckIfPluginIsAlreadyRunningAndFocus(UserControl plugin)
		{
			foreach (var existingPane in PluginGroup.Children)
				foreach (var existingDocument in existingPane.Children.OfType<LayoutDocument>())
					if (Equals(existingDocument.Content, plugin))
					{
						existingDocument.IsActive = true;
						return true;
					}
			return false;
		}

		private bool InitializePlugin(UserControl plugin)
		{
			try
			{
				TryInitializePlugin(plugin);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				return false;
			}
			return true;
		}

		private void TryInitializePlugin(UserControl plugin)
		{
			((EditorPluginView)plugin).Init(viewModel.Service);
		}

		private LayoutDocument CreateDocumentForPlugin(UserControl plugin)
		{
			var layoutDocument = new LayoutDocument
			{
				Content = plugin,
				CanClose = true,
				Title = ((EditorPluginView)plugin).ShortName
			};
			layoutDocument.IsActiveChanged += ChangeActivePlugin;
			layoutDocument.Closed -= ChangeActivePlugin;
			return layoutDocument;
		}

		private void ChangeActivePlugin(object sender, EventArgs e)
		{
			if (app == null)
				return;
			var userControl = ((UserControl)((LayoutDocument)sender).Content) as EditorPluginView;
			if (userControl.GetType() == typeof(ViewportControl))
			{
				userControl.Activate();
				return;
			}
			// ReSharper disable once PossibleUnintendedReferenceComparison
			if (userControl == activePlugin)
				return;
			if (activePlugin != null)
				activePlugin.Deactivate();
			activePlugin = userControl;
			if (viewModel.Service.Viewport != null)
				viewModel.Service.Viewport.DestroyRenderedEntities();
			activePlugin.Activate();
		}

		private EditorPluginView activePlugin;

		private LayoutDocumentPane CreatePaneForPlugins(UserControl plugin)
		{
			LayoutDocumentPane pane;
			if (((EditorPluginView)plugin).RequiresLargePane)
				pane = PluginGroup.Children.Count < 1
					? CreateAndAddLayoutDocumentPane(0.9) : PluginGroup.Children[0] as LayoutDocumentPane;
			else
				pane = PluginGroup.Children.Count < 2
					? CreateAndAddLayoutDocumentPane(0.1) : PluginGroup.Children[1] as LayoutDocumentPane;
			return pane;
		}

		private LayoutDocumentPane CreateAndAddLayoutDocumentPane(double widthValue)
		{
			var pane = new LayoutDocumentPane();
			pane.DockWidth = new GridLength(widthValue, GridUnitType.Star);
			pane.DockMinWidth = widthValue < 0.5 ? SmallPaneMinWidth : LargePaneMinWidth;
			PluginGroup.Children.Add(pane);
			return pane;
		}

		private const int SmallPaneMinWidth = 300;
		private const int LargePaneMinWidth = 506;

		private static void FocusDocumentToSeePlugin(LayoutDocument document)
		{
			document.IsActive = true;
		}

		private void HandleStartupArguments()
		{
			var arguments = Environment.GetCommandLineArgs();
			if (arguments.Length > 1)
				ProcessArguments(arguments);
			App.InvokeCommandLineArguments += ProcessArguments;
		}

		private void ProcessArguments(IList<string> arguments)
		{
			var mode = arguments[1];
			if (mode == PathExtensions.EnginePathEnvironmentVariableName)
				Environment.SetEnvironmentVariable(PathExtensions.EnginePathEnvironmentVariableName,
					arguments[2]);
			else if (mode == "ShowPlugin")
				ShowPlugin(arguments[2]);
			else if (arguments.Count == 4)
			{
				EditorPluginView continuousUpdater = ShowPlugin("Continuous Updater");
				continuousUpdater.Send(arguments);
			}
			else
				Logger.Warning("Invalid arguments: " + arguments.ToText());
		}

		private EditorPluginView ShowPlugin(string pluginName)
		{
			var editorPlugin = viewModel.EditorPlugins.FirstOrDefault(p => p.ShortName == pluginName);
			StartEditorPlugin(editorPlugin as UserControl);
			viewModel.maximizer.BringWindowToForeground();
			return editorPlugin;
		}

		private void StartEngineAndBlock()
		{
			try
			{
				TryStartViewportAndBlock();
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				if (StackTraceExtensions.StartedFromNCrunchOrNunitConsole || Debugger.IsAttached)
					throw;
				if (window != null)
					window.CopyTextToClipboard(ex.ToString());
				MessageBox.Show("Failed to initialize: " + ex + Resolver.ErrorWasCopiedToClipboardMessage,
					"Delta Engine Editor - Fatal Error");
			}
		}

		private void TryStartViewportAndBlock()
		{
			if (DesignerProperties.GetIsInDesignMode(this))
				return;
			LoadEditorPlugins();
			AddToDocumentPane(CreateViewportControl());
			AddToDocumentPane(GetPluginByType(typeof(ContentManagerView)));
			SetupEmulator();
			window = new WpfHostedFormsWindow(viewportControl, this);
			ElementHost.EnableModelessKeyboardInterop(this);
			StartViewportAndWaitUntilWindowIsClosed();
		}
		
		private UserControl CreateViewportControl()
		{
			viewportControl = GetPluginByType(typeof(ViewportControl)) as ViewportControl;
			viewModel.Service.UpdateToolboxVisibility += viewportControl.ShowToolboxPane;
			return viewportControl;
		}

		private void AddToDocumentPane(UserControl control)
		{
			var document = CreateDocumentForPlugin(control);
			var pane = CreatePaneForPlugins(control);
			pane.Children.Add(document);
		}

		private ViewportControl viewportControl;
		private WpfHostedFormsWindow window;

		private void SetupEmulator()
		{
			var emulatorControl = GetPluginByType(typeof(EmulatorControl)) as EmulatorControl;
			emulatorControl.EmulatorViewModel.EmulatorChanged +=
				emulator => viewportControl.UpdateEmulator(emulator);
		}

		private void StartViewportAndWaitUntilWindowIsClosed()
		{
			Closing += (sender, args) => window.Dispose();
			app = new BlockingViewportApp(window);
			viewport = new EditorViewport(window);
			viewModel.Service.Viewport = viewport;
			InitializeViewportAndContentManager();
			Show();
			app.RunAndBlock();
		}

		private void InitializeViewportAndContentManager()
		{
			viewportControl.Init(viewModel.Service);
			var contentManagerControl = GetPluginByType(typeof(ContentManagerView)) as EditorPluginView;
			contentManagerControl.Init(viewModel.Service);
			contentManagerControl.Activate();
		}

		private EditorViewport viewport;
		private BlockingViewportApp app;

		private void OnMinimize(object sender, MouseButtonEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}

		private void OnMaximize(object sender, MouseButtonEventArgs e)
		{
			viewModel.maximizer.ToggleMaximize(false, viewModel);
		}

		private void OnExit(object sender, MouseButtonEventArgs e)
		{
			Application.Current.Shutdown();
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			if (WindowState == WindowState.Maximized)
				viewModel.maximizer.MaximizeWindow();
			base.OnRenderSizeChanged(sizeInfo);
		}

		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			var mousePos = e.MouseDevice.GetPosition(this);
			if (e.ClickCount == 2 && mousePos.Y < 50)
				viewModel.maximizer.ToggleMaximize(false, viewModel);
			else if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Pressed)
			{
				if (mousePos.Y < 5 && viewModel.maximizer.isMaximized)
					viewModel.maximizer.ToggleMaximize(true, viewModel);
				if (!viewModel.maximizer.isMaximized)
					DragMove();
			}
		}

		private void OnEditorPluginSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count >= 1)
				StartEditorPlugin(e.AddedItems[0] as UserControl);
		}

		private void AccountClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://DeltaEngine.net/Account");
		}

		private void ProfileClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://forum.deltaengine.net/yaf_cp_profile.aspx");
		}

		private void ErrorClick(object sender, MouseButtonEventArgs e)
		{
			Process.Start("http://deltaengine.net/Account/ApiKey");
		}

		private void FirstStepsClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/learn/firststeps");
		}

		private void TutorialsClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/learn/tutorials");
		}

		private void AboutTheEditorClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/features/editor");
		}

		private void StartingWithCsharpClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/learn/startingwithcsharp");
		}

		private void TroubleshootingClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/learn/troubleshootingchecklist");
		}

		private void AppBuilderClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/features/appbuilder");
		}

		private void DocumentationClick(object sender, RoutedEventArgs e)
		{
			Process.Start("http://help.deltaengine.net/");
		}

		private void OnContentDrop(object sender, DragEventArgs e)
		{
			IDataObject dataObject = e.Data;
			if (!IsFile(dataObject))
				return;
			var files = (string[])dataObject.GetData(DataFormats.FileDrop);
			viewModel.Service.StartPlugin(typeof(ContentManagerView));
			viewModel.Service.UploadContentFilesToService(files);
		}

		private static bool IsFile(IDataObject dropObject)
		{
			return dropObject.GetDataPresent(DataFormats.FileDrop);
		}

		private void OnHelp(object sender, MouseButtonEventArgs e)
		{
			Process.Start("http://deltaengine.net/features/editor");
		}

		private void DeleteButtonClicked(object sender, RoutedEventArgs e)
		{
			viewModel.ShowMessageBoxToDeleteCurrentProject();
		}

		private void OnContentProjectDropDownOpened(object sender, EventArgs e)
		{
			viewport.IsZoomingEnabled = false;
		}

		private void OnContentProjectDropDownClosed(object sender, EventArgs e)
		{
			viewport.IsZoomingEnabled = true;
		}
	}
}