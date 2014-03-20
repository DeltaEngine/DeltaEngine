using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace $safeprojectname$
{
	/// <summary>
	/// Handles the falling and upcoming blocks.
	/// </summary>
	public class Controller : Entity, Updateable
	{
		public Controller(Orientation displayMode, BlocksContent content)
		{
			this.content = content;
			this.displayMode = displayMode;
			Add(new Grid(content));
			Add(new Soundbank(content));
			GameRunning = true;
		}

		public bool GameRunning { get; set; }

		public readonly BlocksContent content;
		public readonly Orientation displayMode;

		public event Action<int> AddToScore;
		public event Action Lose;

		private void GetNewFallingBlock()
		{
			if (UpcomingBlock == null)
				CreateUpcomingBlock();
			FallingBlock = UpcomingBlock;
			CreateUpcomingBlock();
			if (IsABrickOnTopRowOrIsNoRoomForNextBlock())
				GameLost();
		}

		public Block UpcomingBlock { get; internal set; }

		private void CreateUpcomingBlock()
		{
			UpcomingBlock = new Block(displayMode, content, Vector2D.Zero);
			UpcomingBlock.Left = upcomingBlockCenter.X - UpcomingBlock.Center.X;
			UpcomingBlock.Top = upcomingBlockCenter.Y - UpcomingBlock.Center.Y;
			UpcomingBlock.UpdateBrickDrawAreas(0.0f);
		}

		private readonly Vector2D upcomingBlockCenter = new Vector2D(9, -4);
		public Block FallingBlock { get; internal set; }

		private bool IsABrickOnTopRowOrIsNoRoomForNextBlock()
		{
			return Get<Grid>().IsABrickOnFirstRow() || !TryToPlaceFallingBlockOnGrid();
		}

		private bool TryToPlaceFallingBlockOnGrid()
		{
			List<int> validStartingPositions = Get<Grid>().GetValidStartingColumns(FallingBlock);
			if (validStartingPositions.Count == 0)
				return false; //ncrunch: no coverage
			int column = Randomizer.Current.Get(0, validStartingPositions.Count);
			FallingBlock.Left = validStartingPositions[column];
			return true;
		}

		private void GameLost()
		{
			GameRunning = false;
			Get<Soundbank>().GameLost.Play();
			if (Lose != null)
				Lose();
			GetRidOfGameObjects();
			ReinitializeGame();
		}

		private void GetRidOfGameObjects()
		{
			Get<Grid>().Clear();
			totalRowsRemoved = 0;
			UpcomingBlock.IsActive = false;
			UpcomingBlock = null;
			FallingBlock.IsActive = false;
			FallingBlock = null;
		}

		private void ReinitializeGame()
		{
			GetNewFallingBlock();
			GameRunning = true;
		}

		private void MoveFallingBlockDownwards()
		{
			float top = FallingBlock.Top;
			FallingBlock.UpdateBrickDrawAreas(FallSpeed);
			if (Get<Grid>().IsValidPosition(FallingBlock))
				return;
			FallingBlock.Top = top;
			FallingBlock.UpdateBrickDrawAreas(0.0f);
			Settle();
		}

		private void Settle()
		{
			settling += FallSpeed * Time.Delta;
			if (settling < SettleTime)
				return;
			AffixBlock();
			settling = 0.0f;
		}

		private float settling;
		private const float SettleTime = 1.0f;

		private float FallSpeed
		{
			get { return IsFallingFast ? FastFallSpeed : SlowFallSpeed; }
		}

		public bool IsFallingFast { get; set; }

		private float SlowFallSpeed
		{
			get { return BaseSpeed + totalRowsRemoved * SpeedUpPerRowRemoved; }
		}

		private const float BaseSpeed = 2.0f;
		private int totalRowsRemoved;
		private const float SpeedUpPerRowRemoved = 0.2f;
		private const float FastFallSpeed = 16.0f;

		private void AffixBlock()
		{
			int rowsRemoved = Get<Grid>().AffixBlock(FallingBlock);
			FallingBlock = null;
			totalRowsRemoved += rowsRemoved;
			PlayBlockAffixedSound(rowsRemoved);
			if (AddToScore != null)
				AddToScore(RowRemovedBonus * rowsRemoved * rowsRemoved + BlockPlacedBonus);
		}

		private void PlayBlockAffixedSound(int rowsRemoved)
		{
			if (rowsRemoved == 0)
				Get<Soundbank>().BlockAffixed.Play();
			//ncrunch: no coverage start
			else if (rowsRemoved == 1)
				Get<Soundbank>().RowRemoved.Play();
			else
				Get<Soundbank>().MultipleRowsRemoved.Play();
			//ncrunch: no coverage end
		}

		private const int RowRemovedBonus = 10;
		private const int BlockPlacedBonus = 1;

		//ncrunch: no coverage start, tests being ignored for automated testing
		public void MoveBlockLeftIfPossible()
		{
			if(FallingBlock == null)
				return;
			FallingBlock.Left--;
			if (Get<Grid>().IsValidPosition(FallingBlock))
				Get<Soundbank>().BlockMoved.Play();
			else
			{
				FallingBlock.Left++;
				Get<Soundbank>().BlockCouldNotMove.Play();
			}
		}

		public void MoveBlockRightIfPossible()
		{
			if(FallingBlock == null)
				return;
			FallingBlock.Left++;
			if (Get<Grid>().IsValidPosition(FallingBlock))
				Get<Soundbank>().BlockMoved.Play();
			else
			{
				FallingBlock.Left--;
				Get<Soundbank>().BlockCouldNotMove.Play();
			}
		}

		public void RotateBlockAntiClockwiseIfPossible()
		{
			if(FallingBlock == null)
				return;
			FallingBlock.RotateAntiClockwise();
			if (Get<Grid>().IsValidPosition(FallingBlock))
				Get<Soundbank>().BlockMoved.Play();
			else
			{
				FallingBlock.RotateClockwise();
				Get<Soundbank>().BlockCouldNotMove.Play();
			}
		}

		public void Update()
		{
			if(!GameRunning)
				return;
			if (FallingBlock == null)
				GetNewFallingBlock();
			MoveFallingBlockDownwards();
			UpdateElapsed();
			if (elapsed >= BlockMoveInterval)
				MoveBlockLeftOrRightIfRequired();
		}
		//ncrunch: no coverage end

		internal void UpdateElapsed()
		{
			if (isBlockMovingLeft || isBlockMovingRight)
				elapsed += Time.Delta;
			else
				elapsed = 0;
		}

		internal bool isBlockMovingLeft;
		internal bool isBlockMovingRight;
		internal float elapsed;

		//ncrunch: no coverage start
		internal void MoveBlockLeftOrRightIfRequired()
		{
			elapsed -= BlockMoveInterval;
			if (isBlockMovingLeft)
				MoveBlockLeftIfPossible();

			if (isBlockMovingRight)
				MoveBlockRightIfPossible();
		}
		//ncrunch: no coverage end

		private const float BlockMoveInterval = 0.166f;
	}
}