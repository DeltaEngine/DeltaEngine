using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering3D.Particles;

namespace DeltaEngine.Editor.ParticleEditor
{
	public class ParticleEditorDesign
	{
		public ParticleEditorDesign()
		{
			ParticleSystemName = "MyParticleSystem";
			ParticleEmitterNameToAdd = "ExistingEmitter";
			AvailableEmitterIndices = new List<string>(new[] { "0" });
			CurrentEmitterInEffect = "0";
			SpawnerTypeList = new List<string>(new[] { "Point" });
			SelectedSpawnerType = "Point";
			MaterialList = new List<string>(new[] { "Default2D" });
			SelectedMaterialName = "Default2D";
			Size = new RangeGraph<Size>(new Size(0.01f, 0.01f), new Size(0.1f, 0.1f));
			Color = new RangeGraph<Color>(Datatypes.Color.White, Datatypes.Color.TransparentWhite);
			LifeTime = 1.0f;
			BillboardModeList = new List<string>(new[] { "CameraFacing" });
			SelectedBillboardMode = "CameraFacing";
			StartPosition = new RangeGraph<Vector3D>(new Vector3D(0.0f, 0.0f, 0.0f),
				new Vector3D(0.0f, 0.0f, 0.0f));
			StartRotation = new RangeGraph<ValueRange>(new ValueRange(0.0f), new ValueRange(0.0f));
			StartVelocity = new RangeGraph<Vector3D>(new Vector3D(-0.1f, -0.2f, 0.0f),
				new Vector3D(0.2f, -0.2f, 0.0f));
			Acceleration = new RangeGraph<Vector3D>(new Vector3D(0.0f, 0.1f, 0.0f),
				new Vector3D(0.0f, 0.3f, 0.0f));
			RotationSpeed = new RangeGraph<ValueRange>(new ValueRange(0.0f), new ValueRange(0.0f));
			SpawnInterval = 0.1f;
			MaxNumberOfParticles = 128;
			ParticlesPerSpawn = 1;
		}

		public string ParticleSystemName { get; set; }
		public string ParticleEmitterNameToAdd { get; set; }
		public List<string> AvailableEmitterIndices { get; set; }
		public string CurrentEmitterInEffect { get; set; }
		public List<string> SpawnerTypeList { get; set; }
		public string SelectedSpawnerType { get; set; }
		public List<string> MaterialList { get; set; }
		public string SelectedMaterialName { get; set; }
		public RangeGraph<Size> Size { get; set; }
		public RangeGraph<Color> Color { get; set; }
		public float LifeTime { get; set; }
		public List<string> BillboardModeList { get; set; }
		public string SelectedBillboardMode { get; set; }
		public RangeGraph<Vector3D> StartPosition { get; set; }
		public RangeGraph<ValueRange> StartRotation { get; set; }
		public RangeGraph<Vector3D> StartVelocity { get; set; }
		public RangeGraph<Vector3D> Acceleration { get; set; }
		public RangeGraph<ValueRange> RotationSpeed { get; set; }
		public int ParticlesPerSpawn { get; set; } 
		public float SpawnInterval { get; set; }
		public int MaxNumberOfParticles { get; set; }
		public bool DoParticlesTrackEmitter { get; set; }
	}
}