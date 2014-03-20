using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Tutorials.Entities10LogoRacer
{
	public class Player : Sprite, Updateable
	{
		public Player()
			: base(ContentLoader.Load<Material>("Logo"), new Vector2D(0.5f, 0.7f))
		{
			commands.Add(new Command(Command.MoveLeft,
				() => Center -= new Vector2D(Time.Delta * 0.5f, 0)));
			commands.Add(new Command(Command.MoveRight,
				() => Center += new Vector2D(Time.Delta * 0.5f, 0)));
		}

		private readonly List<Command> commands = new List<Command>();

		public void Update()
		{
			var earths = EntitiesRunner.Current.GetEntitiesOfType<Enemy>();
			if (Center.X < Size.Width / 2)
				Center = new Vector2D(Size.Width / 2, Center.Y);
			if (Center.X > 1 - Size.Width / 2)
				Center = new Vector2D(1 - Size.Width / 2, Center.Y);
			if (!earths.Any(e => Center.DistanceTo(e.Center) < Size.Width / 3 + e.Size.Width / 2))
				return;
			Set(Color.Red);
			foreach (var command in commands)
				command.IsActive = false;
		}

		public bool IsPauseable { get { return true; } }
	}
}