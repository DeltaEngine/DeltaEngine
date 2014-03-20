using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Tests
{
	public class Sprite3DTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void UseSpriteAsFarPlanetBackground()
		{
			Create3DCamera();
			var sprite = new Sprite(new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogo"),
				new Rectangle(0, 0, 1, 1));
			sprite.RenderLayer = -1;
			sprite.UV = new Rectangle(0.25f, 0.25f, 0.7f, 0.7f);
		new Model(new ModelData(new BoxMesh(0.1f * Vector3D.One, Color.Red)), Vector3D.Zero);
			sprite.Start<BackgroundUVUpdater>();
		}

		public void Create3DCamera()
		{
			camera = Camera.Use<LookAtCamera>();
			camera.Position = 4 * Vector3D.One;
			new Grid3D(new Size(10));
		}

		private LookAtCamera camera;

		public class BackgroundUVUpdater : UpdateBehavior
		{
			public BackgroundUVUpdater(Device device)
			{
				this.device = device;
			}

			private readonly Device device;

			public override void Update(IEnumerable<Entity> entities)
			{
				var matrix = device.CameraInvertedViewMatrix;
				var angle = ((float)Math.Acos(matrix.Forward.Z / matrix.Forward.Length)).RadiansToDegrees();
				foreach (var entity in entities.OfType<Sprite>())
					RecalculateUV(matrix, entity, angle);
			}

			private static void RecalculateUV(Matrix matrix, Sprite entity, float angle)
			{
				var yScaled = RecalculateYCoordinate(matrix, angle);
				var uv = new Rectangle(entity.UV.Left,
					0.25f - ((1 - entity.UV.Height)) * yScaled, entity.UV.Width,
					entity.UV.Height);
				entity.UV = uv;
			}

			private static float RecalculateYCoordinate(Matrix matrix, float angle)
			{
				var yScaled = (matrix.Translation.Z + 2) / 4.0f;
				var scaledAngle = (angle - 45) / 90.0f;
				if (scaledAngle < 0)
					scaledAngle = 0; //ncrunch: no coverage
				yScaled = (yScaled + scaledAngle * 4) / 5;
				return yScaled < 0 ? 0 : yScaled;
			}
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawAtlasSprite()
		{
			if (StackTraceExtensions.IsStartedFromNCrunch())
				return;
			// ncrunch: no coverage start
			//test: ContentLoader.Use<DiskContentLoader>();
			new FilledRect(Rectangle.HalfCentered, Color.Brown);
			var sprite1 = new Sprite("headPL", new Rectangle(0.25f, 0.25f, 0.5f, 0.25f));
			sprite1.UV = new Rectangle(0, 0, 1, 0.5f);
			sprite1.Color = Color.Red;
			var sprite2 = new Sprite("headPL", new Rectangle(0.25f, 0.5f, 0.5f, 0.25f));
			sprite2.UV = new Rectangle(0, 0.5f, 1, 0.5f);
			sprite2.Color = Color.Blue;
		}
	}
}