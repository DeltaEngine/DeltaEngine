using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.ScreenSpaces;

namespace CarGame2D
{
	/// <summary>
	/// Handles the game logic and draws the background, track and car.
	/// </summary>
	public class Game : Sprite, Updateable
	{
		public Game(Track track, Window window)
			: base("Background", ScreenSpace.Current.Viewport)
		{
			car = new Car(track.TrackStart, track);
			info = new FontText(Font.Default, "Total Time: 0s",
				new Rectangle(0.0f, ScreenSpace.Current.Top, 1.0f, 0.1f))
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top
			};
			RenderLayer = DefaultRenderLayer - 1;
			if (Material.DiffuseMap.AllowTiling)
				UV = new Rectangle(Vector2D.Zero, window.ViewportPixelSize / Material.DiffuseMap.PixelSize);
			lastCarTrackIndex = car.NextTrackIndex;
			finishedRounds = -1;
			new Command(Command.Exit, window.CloseAfterFrame);
		}

		private readonly Car car;
		private readonly FontText info;
		private int lastCarTrackIndex;
		private int finishedRounds;
		private float elapsedTotalTime;
		private float bestRoundTime;
		private float elapsedRoundTime;

		public void Update()
		{
			elapsedTotalTime += Time.Delta;
			elapsedRoundTime += Time.Delta;
			info.Text = "Total Time: " + elapsedTotalTime.Round(2).ToString("00.00") + "s";
			if (finishedRounds > 0)
			{
				info.Text += "\nBest Round Time: " + bestRoundTime.Round(2).ToString("00.00") + "s";
				info.Text += "\nRounds finished: " + finishedRounds;
			}
			CheckForAFinishedRound();
		}

		private void CheckForAFinishedRound()
		{
			if (lastCarTrackIndex != car.NextTrackIndex && car.NextTrackIndex == 1)
			{
				if (finishedRounds == 0)
					bestRoundTime = elapsedRoundTime;
				else if (elapsedRoundTime < bestRoundTime)
					bestRoundTime = elapsedRoundTime;
				finishedRounds++;
				elapsedRoundTime = 0f;
			}
			lastCarTrackIndex = car.NextTrackIndex;
		}
	}
}