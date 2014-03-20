using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;
using DeltaEngine.ScreenSpaces;
using DeltaNinja.Entities;
using DeltaNinja.Pages;
using DeltaNinja.Support;
using DeltaNinja.UI;

namespace $safeprojectname$
{
	internal class Match : Entity2D
	{
		public event EventHandler<GameOverEventArgs> GameEnded;

		public Match(ScreenSpace screen, NumberFactory numberFactory, LogoFactory logoFactory)
			: base(Rectangle.Zero)
		{
			this.screen = screen;
			this.logoFactory = logoFactory;
			hud = new HudScene(screen, numberFactory);
			pause = new PausePage(screen);
			pause.Hide();
			Slice = new Slice();
			PointsTips = new List<PointsTip>();
			ErrorFlags = new List<ErrorFlag>();
			HideScore();
			screen.ViewportSizeChanged += RefreshSize;
			RefreshSize();
		}

		private readonly ScreenSpace screen;
		private readonly LogoFactory logoFactory;
		private readonly HudScene hud;
		private readonly PausePage pause;
		private readonly List<Logo> logoSet = new List<Logo>();
		public readonly Slice Slice;
		private Score score;
		public readonly List<PointsTip> PointsTips;
		public readonly List<ErrorFlag> ErrorFlags;

		public int LogoCount
		{
			get { return logoSet.Count(x => x.IsActive); }
		}

		public IEnumerable<Logo> LogoArray
		{
			get { return logoSet.ToArray(); }
		}

		public bool IsPaused { get; private set; }

		public void Start()
		{
			Slice.Reset();
			mouseMovement = new Command(CheckMouse).Add(new MouseMovementTrigger());
			mouseLeftClick = new Command(StartSlice).Add(new MousePositionTrigger());
			score = new Score();
			hud.Reset();
			hud.Show();
			IsActive = true;
			Start<GameLogic>();
			IsPaused = false;
			pauseCommand = new Command(SwitchPause).Add(new KeyTrigger(Key.Space));
			pauseMouseCommand = new Command(SwitchPause).Add(new MouseButtonTrigger(MouseButton.Right));
		}

		private Command mouseMovement;
		private Command mouseLeftClick;
		private Command pauseCommand;
		private Command pauseMouseCommand;

		private void Reset()
		{
			IsActive = false;
			pauseCommand.IsActive = false;
			pauseMouseCommand.IsActive = false;
			if (mouseMovement != null)
			{
				mouseMovement.IsActive = false;
				mouseMovement = null;
			}
			if (mouseLeftClick != null)
			{
				mouseLeftClick.IsActive = false;
				mouseLeftClick = null;
			}
			foreach (var logo in logoSet)
				logo.IsActive = false;
			logoSet.Clear();
			Slice.Reset();
			foreach (var logo in logoSet)
				logo.ResetSlicing();
			foreach (var tip in PointsTips)
				tip.Reset();
			foreach (var flag in ErrorFlags)
				flag.Reset();
		}

		private void EndGame(bool abort)
		{
			Reset();
			if (GameEnded != null)
				GameEnded(this, abort ? null : new GameOverEventArgs(score));
				
		}

		public void HideScore()
		{
			hud.Hide();
		}

		public void CreateLogos(int count)
		{
			for (int i = 0; i < count; i++)
				logoSet.Add(logoFactory.Create());
		}

		public void RemoveLogo(Logo logo)
		{
			logoSet.Remove(logo);
		}

		private void CheckMouse(Vector2D position)
		{
			if (IsPaused)
				return;
			Slice.Check(position, logoSet);
		}

		private void StartSlice(Vector2D position)
		{
			if (IsPaused)
				return;
			Slice.Switch(position);
		}

		private void RefreshSize()
		{
			var view = screen.Viewport;
			//pointsNumber.Top = view.Top;
			//levelBox.Top = view.Top;
			//errorFlag.Top = view.Top;
		}

		public void AddOneMoreSlice()
		{
			score.Count += 1;
		}

		public bool AddError()
		{
			score.Errors += 1;
			hud.SetError(score.Errors);
			if (score.Errors < 3)
				return true;
			EndGame(false);
			return false;
		}

		public void AddPoints(int points)
		{
			score.Points += points;
			hud.SetPoints(score.Points);
		}

		public int CurrentLevel
		{
			get { return score.Level; }
		}

		public void NextLevel()
		{
			score.Level += 1;
			hud.SetLevel(score.Level);
		}

		public void ShowError(float left, float width)
		{
			ErrorFlags.Add(new ErrorFlag(left, width, screen.Viewport.Bottom));
		}

		public void ClearEntities()
		{
			foreach (var tip in PointsTips.ToArray())
				if (tip.Time + 1500 < GlobalTime.Current.Milliseconds)
				{
					tip.Reset();
					PointsTips.Remove(tip);
				}
			foreach (var flag in ErrorFlags.ToArray())
				if (flag.Time + 1500 < GlobalTime.Current.Milliseconds)
				{
					flag.IsActive = false;
					ErrorFlags.Remove(flag);
				}
		}

		private void SwitchPause()
		{
			IsPaused = !IsPaused;
			foreach (var logo in logoSet)
				logo.SetPause(IsPaused);
			if (IsPaused)
			{
				Slice.Reset();
				pause.Show();
				pause.ButtonClicked += OnPauseButtonClicked;
			}
			else
			{
				pause.ButtonClicked -= OnPauseButtonClicked;
				pause.Hide();
			}
		}

		protected void OnPauseButtonClicked(MenuButton code)
		{
			switch (code)
			{
			case (MenuButton.Resume):
				SwitchPause();
				break;
			case (MenuButton.NewGame):
				SwitchPause();
				Reset();
				Start();
				break;
			case (MenuButton.Abort):
				SwitchPause();
				EndGame(true);
				break;
			}
		}
	}
}