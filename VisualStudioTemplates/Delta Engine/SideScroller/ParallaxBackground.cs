using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;

namespace $safeprojectname$
{
	public class ParallaxBackground : Entity, RapidUpdateable
	{
		public ParallaxBackground(int numberOfLayers, string[] layerImageNames, float[] scrollFactors)
		{
			layers = new BackgroundLayer[numberOfLayers];
			for (int i = 0; i < numberOfLayers; i++)
				layers[i] = new BackgroundLayer(layerImageNames[i], scrollFactors[i]);
			RenderLayer = -(layerImageNames.Length + 1);
		}

		public int RenderLayer
		{
			get { return layers[0].RenderLayer; }
			set
			{
				for (int i = 0; i < layers.Length; i++)
					layers[i].RenderLayer = value + i;
			}
		}

		private readonly BackgroundLayer[] layers;
		public float BaseSpeed { get; set; }

		public void RapidUpdate()
		{
			foreach (var backgroundLayer in layers)
				backgroundLayer.ScrollByBaseSpeed(BaseSpeed);
		}

		private class BackgroundLayer : Sprite
		{
			public BackgroundLayer(string imageName, float speedFactor)
				: base(CreateMaterial(imageName), Rectangle.One)
			{
				this.speedFactor = speedFactor;
			}

			private static Material CreateMaterial(string imageName)
			{
				return new Material(ShaderFlags.Position2DTextured, imageName);
			}

			private readonly float speedFactor;

			internal void ScrollByBaseSpeed(float baseSpeed)
			{
				UV = UV.Move(baseSpeed * speedFactor * Time.Delta, 0.0f);
			}
		}

		public override bool IsActive
		{
			get { return base.IsActive; }
			set
			{
				foreach (var backgroundLayer in layers)
					backgroundLayer.IsActive = value;
				base.IsActive = value;
			}
		}

		public override bool IsPauseable
		{
			get { return true; }
		}
	}
}