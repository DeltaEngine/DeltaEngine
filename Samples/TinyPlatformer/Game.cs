using DeltaEngine.Commands;
using DeltaEngine.Input;

namespace TinyPlatformer
{
	public class Game
	{
		/// <summary>
		/// Load a map and bind keyboard controls to player actions
		/// </summary>
		public Game(Map map)
		{
			Actor player = map.Player;
			new Command(() => player.WantsToGoLeft = true).Add(new KeyTrigger(Key.CursorLeft));
			new Command(() => player.WantsToGoLeft = false).Add(new KeyTrigger(Key.CursorLeft,
				State.Releasing));
			new Command(() => player.WantsToGoRight = true).Add(new KeyTrigger(Key.CursorRight));
			new Command(() => player.WantsToGoRight = false).Add(new KeyTrigger(Key.CursorRight,
				State.Releasing));
			new Command(() => player.WantsToJump = true).Add(new KeyTrigger(Key.Space));
			new Command(() => player.WantsToJump = false).Add(new KeyTrigger(Key.Space, State.Releasing));
			new Command(() => player.WantsToJump = true).Add(new KeyTrigger(Key.CursorUp));
			new Command(() => player.WantsToJump = false).Add(new KeyTrigger(Key.CursorUp,
				State.Releasing));
		}
	}
}