using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Rendering3D.Tests
{
	public class LightingTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void StaticLight()
		{
			var camera = Camera.Use<LookAtCamera>();
			camera.Position = new Vector3D(0.0f, 4.0f, 2.0f);
			SunLight.Current = new SunLight(new Vector3D(1.0f, 0.0f, -1.0f), Color.Red);
			new RotatingBox(new Vector3D(-2.0f, 0.0f, 0.0f));
			new Box(Vector3D.Zero);
			new RotatingBox(new Vector3D(2.0f, 0.0f, 0.0f));
		}

		private class Box : Model
		{
			public Box(Vector3D position)
				: base(new ModelData(new BoxMesh(Vector3D.One,
					new Material(ShaderFlags.LitTextured, "BoxDiffuse"))), position) {}
		}

		private class RotatingBox : Box
		{
			public RotatingBox(Vector3D position)
				: base(position)
			{
				Angle = 0.0f;
				Speed = Randomizer.Current.Get(-80.0f, 80.0f);
				Start<Rotate>();
			}

			public float Speed { get; private set; }
			public float Angle { get; set; }
		}

		private class Rotate : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (RotatingBox box in entities)
				{
					box.Angle += Time.Delta * box.Speed;
					box.Angle = MathExtensions.WrapRotationToMinus180ToPlus180(box.Angle);
					box.Orientation = Quaternion.FromAxisAngle(Vector3D.UnitZ, box.Angle);
				}
			}
		}

		[Test]
		public void DynamicLightChangeColorByRightClicking()
		{
			var camera = Camera.Use<LookAtCamera>();
			camera.Position = 1.5f * Vector3D.One;
			SunLight.Current = new SunLight(Vector3D.One, colors[0]);
			var cube = new Box(Vector3D.Zero);
			cube.Start<UpdateLightDirection>();
			new Command(Command.RightClick, () =>
			{ //ncrunch: no coverage start
				SunLight.Current.Color = colors[currentColor++ % colors.Length];
			}); //ncrunch: no coverage end
		}

		private readonly Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Yellow };
		private int currentColor;

		private class UpdateLightDirection : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				angle += Time.Delta * Speed;
				angle = MathExtensions.WrapRotationToMinus180ToPlus180(angle);
				SunLight.Current.Direction =
					new Vector3D(MathExtensions.Cos(angle), MathExtensions.Sin(angle), 1.0f);
			}

			private float angle;
			private const float Speed = 100.0f;
		}
	}
}