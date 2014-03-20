using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Physics3D;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes;

namespace Physics3DDemo
{
	public class Game : Entity
	{
		public Game(Physics physics, Window window)
		{
			this.physics = physics;
			physics.SetGroundPlane(true, 0.0f);
			new Model(new ModelData(new BoxMesh(new Vector3D(25, 25, 0.1f), Color.DarkGreen)),
				Vector3D.Zero);
			CreatePyramid();
			SetupInputCommands(window);
			Camera.Current.Position = Vector3D.One * 10;
		}

		private readonly Physics physics;

		private void CreatePyramid()
		{
			var boxMesh = new BoxMesh(BoxSize, new Material(ShaderFlags.LitTextured, "BoxDiffuse"));
			for (int height = 0; height < PyramidSize; height++)
				for (int width = height; width < PyramidSize; width++)
				{
					var box = new PhysicalEntity3D();
					box.Position = new Vector3D(
						(width - height * 0.5f) * BoxSize.X * 1.1f - BoxSize.X * 1.1f * PyramidSize / 2.0f + 1.1f,
						0.0f, 1.0f + height * BoxSize.Z * 1.0f);
					var shape = new PhysicsShape(ShapeType.Box) { Size = BoxSize };
					box.PhysicsBody = physics.CreateBody(shape, box.Position, box.Mass, 0.0f);
					box.AddChild(new Model(new ModelData(boxMesh), Vector3D.Zero));
				}
		}

		private const int PyramidSize = 20;
		private static readonly Vector3D BoxSize = new Vector3D(1, 1, 1);

		private void SetupInputCommands(Window window)
		{
			new Command(Command.Click, position =>
			{
				Ray ray = Camera.Current.ScreenPointToRay(position);
				var rayCast = physics.DoRayCastExcludingGround(ray);
				if (rayCast.PhysicsBody != null)
					rayCast.PhysicsBody.ApplyLinearImpulse(ray.Direction * 50.0f);
			});
			new Command(Command.Exit, window.CloseAfterFrame);
		}
	}
}