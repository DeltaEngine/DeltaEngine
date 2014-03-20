using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Networking;
using Drench.Logics;

namespace Drench.Games
{
	public class TwoHumanNetworkGame : TwoPlayerGame, Updateable
	{
		public TwoHumanNetworkGame(MessagingSession session, int width, int height)
			: base(new TwoHumanLogic(width, height))
		{
			this.session = session;
			UpdateTurnText();
		}

		private readonly MessagingSession session;

		private void UpdateTurnText()
		{
			if (logic.ActivePlayer == 0 && session.UniqueID == 0)
				UpdateText(" (Your turn)", "");
			else if (logic.ActivePlayer == 1 && session.UniqueID == 1)
				UpdateText("", " (Your turn)");
			else if (logic.ActivePlayer == 0 && session.UniqueID == 1)
				UpdateText(" (Waiting for other player's turn)", "");
			else
				UpdateText("", " (Waiting for other player's turn)");
		}

		internal TwoHumanNetworkGame(MessagingSession session, Board.Data boardData)
			: base(new TwoHumanLogic(boardData))
		{
			this.session = session;
			UpdateTurnText();
		}

		public void Update()
		{
			List<MessagingSession.Message> messages = session.GetMessages();
			foreach (MessagingSession.Message message in messages)
				ProcessMessage(message.SenderUniqueID, message.Data);
		}

		private void ProcessMessage(int sender, object message)
		{
			var move = message as Move;
			if (move != null)
				base.ButtonClicked(move.X, move.Y);
		}

		private class Move
		{
			protected Move() {} //ncrunch: no coverage

			public Move(int x, int y)
			{
				X = x;
				Y = y;
			}

			public int X { get; private set; }
			public int Y { get; private set; }
		}

		protected override void ButtonClicked(int x, int y)
		{
			if (logic.ActivePlayer == session.UniqueID)
				base.ButtonClicked(x, y);
			else if (logic.ActivePlayer == 0)
				UpdateText("(Waiting for other player's turn)", "- Not Your Turn!");
			else
				UpdateText("- Not Your Turn!", " (Waiting for other player's turn)"); //ncrunch: no coverage
		}

		protected override bool ProcessDesiredMove(int x, int y)
		{
			Color color = logic.Board.GetColor(x, y);
			bool isValid = logic.GetPlayerValidMoves(logic.ActivePlayer).Contains(color);
			if (isValid)
				MakeMove(color, x, y);
			else
				ReportMoveInvalid();
			while (!logic.HasPlayerAnyValidMoves(logic.ActivePlayer) && !logic.IsGameOver)
				logic.Pass(); //ncrunch: no coverage
			return isValid;
		}

		private void MakeMove(Color color, int x, int y)
		{
			if (logic.ActivePlayer == session.UniqueID)
				session.SendMessage(new Move(x, y));
			logic.MakeMove(color);
			UpdateTurnText();
		}

		private void ReportMoveInvalid()
		{
			if (logic.ActivePlayer == 0)
				UpdateText("- Invalid Move!", "");
			else
				UpdateText("", "- Invalid Move!"); //ncrunch: no coverage
		}

		internal Color[,] Colors
		{
			get { return logic.Board.colors; }
		}

		public override bool IsPauseable
		{
			get { return true; }
		}
	}
}