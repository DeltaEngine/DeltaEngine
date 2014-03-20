using System;
using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.Scenes.Controls;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Scenes.Terminal
{
	/// <summary>
	/// Displays a console consisting of a command line, a number of history lines above it, and
	/// autocompletion information below it.
	/// </summary>
	public sealed class Console : FilledRect, KeyboardControllable
	{
		public Console()
			: base(Rectangle.Zero, Color.Black)
		{
			ScreenSpace.Scene.ViewportSizeChanged += UpdateDrawArea;
			CreateText();
			UpdateDrawArea();
			Add(new InteractiveState { WantsFocus = true });
			Start<ControlUpdater>();
			Start<Keyboard>();
			AddKeyboardCommands();
			IsEnabled = true;
		}

		private void CreateText()
		{
			font = Font.Default;
			history = new FontText(font, "", Rectangle.Zero)
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Bottom
			};
			command = new FontText(font, "> _", Rectangle.Zero)
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top
			};
			autoCompletions = new FontText(font, "", Rectangle.Zero)
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top
			};
		}

		private Font font;
		internal FontText history;
		internal FontText command;
		internal FontText autoCompletions;

		private void UpdateDrawArea()
		{
			fontHeight = ScreenSpace.Scene.FromPixelSpace(new Size(FontPixelSize)).Height;
			DrawArea = GetDrawArea();
			history.DrawArea = new Rectangle(DrawArea.Left, DrawArea.Top, DrawArea.Width,
				MaxHistoryLines * fontHeight);
			command.DrawArea = new Rectangle(DrawArea.Left, history.DrawArea.Bottom, DrawArea.Width,
				fontHeight);
			autoCompletions.DrawArea = new Rectangle(DrawArea.Left, command.DrawArea.Bottom,
				DrawArea.Width, autoCompletionCount * fontHeight);
		}

		private float fontHeight;
		public const float FontPixelSize = 18.0f;
		internal const int MaxHistoryLines = 6;
		private int autoCompletionCount;

		private Rectangle GetDrawArea()
		{
			var screen1 = ScreenSpace.Scene;
			return new Rectangle
			{
				Left = screen1.Viewport.Left + LeftMargin,
				Top = screen1.Viewport.Top + TopMargin,
				Width = screen1.Viewport.Width - (LeftMargin + RightMargin),
				Height = (MaxHistoryLines + 1 + autoCompletionCount) * fontHeight
			};
		}

		private const float LeftMargin = 0.01f;
		private const float RightMargin = 0.01f;
		private const float TopMargin = 0.1f;

		private void AddKeyboardCommands()
		{
			new Command(ToggleActivation()).Add(new KeyTrigger(Key.F12));
			new Command(ExecuteCommandAndMoveToHistory).Add(new KeyTrigger(Key.Enter));
			new Command(GetPreviousCommandFromHistory).Add(new KeyTrigger(Key.CursorUp));
			new Command(GetNextCommandFromHistory).Add(new KeyTrigger(Key.CursorDown));
			new Command(AutoCompleteText).Add(new KeyTrigger(Key.Tab));
		}

		private Action ToggleActivation()
		{
			return () =>
			{
				IsActive = !IsActive;
				if (IsActive)
					Get<InteractiveState>().WantsFocus = true;
				else
					Get<InteractiveState>().HasFocus = false;
			};
		}

		internal static readonly Color EnabledBackgroundColor = new Color(0, 167, 255, 127);
		internal static readonly Color DisabledBackgroundColor = new Color(127, 127, 127, 127);

		public string Text
		{
			get { return command.Text.Substring(2, command.Text.Length - 3); }
			set
			{
				command.Text = "> " + value + "_";
				UpdateAutoCompletions();
				UpdateDrawArea();
			}
		}

		private void UpdateAutoCompletions()
		{
			string trimmedCommand = Text.Trim();
			List<string> autoCompletionList = consoleCommands.GetAutoCompletionList(trimmedCommand);
			autoCompletionCount = trimmedCommand == "" ? 0 : autoCompletionList.Count;
			autoCompletions.Text = trimmedCommand == "" ? "" : string.Join("\n", autoCompletionList);
		}

		private readonly ConsoleCommands consoleCommands = ConsoleCommands.Current;

		private void ExecuteCommandAndMoveToHistory()
		{
			string input = string.Join(" ", Text.SplitAndTrim(" "));
			commands.Add(input);
			commandsPosition = commands.Count;
			AddToHistory("> " + input);
			AddToHistory(consoleCommands.ExecuteCommand(input));
			history.Text = string.Join("\n", historyLines);
			command.Text = "> _";
		}

		internal readonly List<string> commands = new List<string>();
		private int commandsPosition;

		private void AddToHistory(string line)
		{
			historyLines.Add(line);
			if (historyLines.Count > MaxHistoryLines)
				historyLines.RemoveAt(0);
		}

		private readonly List<string> historyLines = new List<string>();

		private void GetPreviousCommandFromHistory()
		{
			commandsPosition--;
			if (commandsPosition < 0)
				commandsPosition = 0;
			if (commandsPosition < commands.Count)
				command.Text = "> " + commands[commandsPosition] + "_";
		}

		private void GetNextCommandFromHistory()
		{
			commandsPosition++;
			if (commandsPosition >= commands.Count)
				commandsPosition = commands.Count;
			if (commandsPosition < commands.Count)
				command.Text = "> " + commands[commandsPosition] + "_";
			else
				command.Text = "> _";
		}

		private void AutoCompleteText()
		{
			Text = consoleCommands.AutoCompleteString(Text.Trim());
		}

		public bool IsEnabled
		{
			get { return isEnabled; }
			set
			{
				isEnabled = value;
				Color = isEnabled ? EnabledBackgroundColor : DisabledBackgroundColor;
			}
		}

		private bool isEnabled;

		public override bool IsActive
		{
			get { return base.IsActive; }
			set
			{
				base.IsActive = value;
				history.IsActive = value;
				command.IsActive = value;
				autoCompletions.IsActive = value;
				if (value)
					Get<InteractiveState>().WantsFocus = true;
			}
		}

		public void UpdateTextFromKeyboardInput(Func<string, string> handleInput)
		{
			if (IsEnabled && Get<InteractiveState>().HasFocus)
				Text = handleInput(Text);
		}
	}
}