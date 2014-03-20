using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Networking;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;
using Drench.Games;
using Drench.Logics;

namespace $safeprojectname$
{
	public class DrenchMenu : Entity, Updateable
	{
		public DrenchMenu()
		{
			scene = new Scene();
			CreateTheme();
			AddOptionButtons();
			AddSliders();
		}

		private readonly Scene scene;

		private void CreateTheme()
		{
			theme = new Theme
			{
				Button = new Material(ShaderFlags.Position2DTextured, "SimpleButtonNormal"),
				ButtonMouseover = new Material(ShaderFlags.Position2DTextured, "SimpleButtonHover"),
				ButtonPressed = new Material(ShaderFlags.Position2DTextured, "SimpleButtonPressed"),
			};
		}

		private Theme theme;

		private void AddOptionButtons()
		{
			scene.Add(new InteractiveButton(theme, SinglePlayerOption, "Single Player Game")
			{
				Clicked = StartSinglePlayerGame
			});
			scene.Add(new InteractiveButton(theme, HumanVsDumbAiOption, "Single Player vs Dumb AI")
			{
				Clicked = StartHumanVsDumbAiGame
			});
			scene.Add(new InteractiveButton(theme, HumanVsSmartAiOption, "Single Player vs Smart AI")
			{
				Clicked = StartHumanVsSmartAiGame
			});
			scene.Add(new InteractiveButton(theme, TwoHumanLocalOption, "Human vs Human (local)")
			{
				Clicked = StartTwoHumanLocalGame
			});
			//startTwoHumanNetworkButton = new InteractiveButton(theme, StartTwoHumanNetworkOption,
			//	"Start Human vs Human (networked)") { Clicked = StartTwoHumanNetworkGame };
			//scene.Add(startTwoHumanNetworkButton);
			//joinTwoHumanNetworkButton = new InteractiveButton(theme, JoinTwoHumanNetworkOption,
			//	"Join Human vs Human (networked)") { Clicked = JoinTwoHumanNetworkGame };
			//scene.Add(joinTwoHumanNetworkButton);
		}

		//private InteractiveButton startTwoHumanNetworkButton;
		//private InteractiveButton joinTwoHumanNetworkButton;
		private static readonly Rectangle SinglePlayerOption = new Rectangle(0.1f, 0.25f, 0.4f, 0.06f);
		private static readonly Rectangle HumanVsDumbAiOption = new Rectangle(0.1f, 0.335f, 0.4f,
			0.06f);
		private static readonly Rectangle HumanVsSmartAiOption = new Rectangle(0.1f, 0.42f, 0.4f,
			0.06f);
		private static readonly Rectangle TwoHumanLocalOption = new Rectangle(0.1f, 0.505f, 0.4f,
			0.06f);
		//private static readonly Rectangle StartTwoHumanNetworkOption = new Rectangle(0.1f, 0.59f,
		//	0.4f, 0.06f);
		//private static readonly Rectangle JoinTwoHumanNetworkOption = new Rectangle(0.1f, 0.675f,
		//	0.4f, 0.06f);

		private void StartSinglePlayerGame()
		{
			scene.Hide();
			game = new SingleHumanGame(boardWidth, boardHeight);
			game.Exited += scene.Show;
		}

		internal Game game;
		private int boardWidth = 10;
		private int boardHeight = 10;

		private void StartHumanVsDumbAiGame()
		{
			scene.Hide();
			game = new HumanVsAiGame(new HumanVsDumbAiLogic(boardWidth, boardHeight));
			game.Exited += scene.Show;
		}

		private void StartHumanVsSmartAiGame()
		{
			scene.Hide();
			game = new HumanVsAiGame(new HumanVsSmartAiLogic(boardWidth, boardHeight));
			game.Exited += scene.Show;
		}

		private void StartTwoHumanLocalGame()
		{
			scene.Hide();
			game = new TwoHumanGame(boardWidth, boardHeight);
			game.Exited += scene.Show;
		}

		//ncrunch: no coverage start
		private void StartTwoHumanNetworkGame()
		{
			serverSession = Messaging.StartSession(Port);
			//startTwoHumanNetworkButton.Text = "Waiting for second player...";
			isWaitingForSecondPlayer = true;
		}

		private const int Port = 19191;
		private MessagingSession serverSession;
		private bool isWaitingForSecondPlayer;

		public void Update()
		{
			if (isWaitingForSecondPlayer)
				CheckIfSecondPlayerHasConnected();
			else if (isWaitingToConnect)
				CheckIfHaveConnected();
		}

		private void CheckIfSecondPlayerHasConnected()
		{
			if (serverSession.NumberOfParticipants <= 0)
				return;
			isWaitingForSecondPlayer = false;
			//startTwoHumanNetworkButton.Text = "Start Human vs Human (networked)";
			scene.Hide();
			var networkGame = new TwoHumanNetworkGame(serverSession, boardWidth, boardHeight);
			serverSession.SendMessage(new Board.Data(boardWidth, boardHeight, networkGame.Colors));
		}

		private void CheckIfHaveConnected()
		{
			if (clientSession.UniqueID <= 0)
				return; //ncrunch: no coverage
			List<MessagingSession.Message> messages = clientSession.GetMessages();
			if (messages.Count == 0)
				return; //ncrunch: no coverage
			isWaitingToConnect = false;
			//joinTwoHumanNetworkButton.Text = "Join Human vs Human (networked)";
			scene.Hide();
			game = new TwoHumanNetworkGame(clientSession, (Board.Data)(messages[0].Data));
			game.Exited += scene.Show;
		}

		private void JoinTwoHumanNetworkGame()
		{
			clientSession = Messaging.JoinSession("localhost", Port);
			//joinTwoHumanNetworkButton.Text = "Trying to connect...";
			isWaitingToConnect = true;
		} //ncrunch: no coverage end

		private MessagingSession clientSession;
		private bool isWaitingToConnect;

		private void AddSliders()
		{
			scene.Add(new Slider(WidthSliderOption)
			{
				MinValue = 5,
				MaxValue = 15,
				Value = boardWidth,
				ValueChanged = WidthChanged
			});
			scene.Add(new Slider(HeightSliderOption)
			{
				MinValue = 5,
				MaxValue = 15,
				Value = boardHeight,
				ValueChanged = HeightChanged
			});
			UpdateBoardSizeText();
			scene.Add(boardSize);
		}

		private static readonly Rectangle WidthSliderOption = new Rectangle(0.6f, 0.3f, 0.3f, 0.05f);
		private static readonly Rectangle HeightSliderOption = new Rectangle(0.6f, 0.375f, 0.3f,
			0.05f);
		internal readonly FontText boardSize = new FontText(Font.Default, "",
			new Rectangle(0.6f, 0.425f, 0.3f, 0.1f));

		private void WidthChanged(int width)
		{
			boardWidth = width;
			UpdateBoardSizeText();
		}

		private void HeightChanged(int height)
		{
			boardHeight = height;
			UpdateBoardSizeText();
		}

		private void UpdateBoardSizeText()
		{
			boardSize.Text = "Board Size: " + boardWidth + " x " + boardHeight;
		}

		public override bool IsPauseable
		{
			get { return true; }
		}
	}
}