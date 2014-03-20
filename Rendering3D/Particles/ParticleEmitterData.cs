using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering3D.Particles
{
	/// <summary>
	/// Data for ParticleEmitter, usually created and saved in the Editor.
	/// </summary>
	public class ParticleEmitterData : ContentData
	{
		protected ParticleEmitterData(string contentName)
			: base(contentName) {}

		public ParticleEmitterData()
			: base("<GeneratedParticleEmitterData>")
		{
			StartVelocity = new RangeGraph<Vector3D>(Vector3D.Zero, Vector3D.Zero);
			Acceleration = new RangeGraph<Vector3D>(Vector3D.Zero, Vector3D.Zero);
			Size = new RangeGraph<Size>(new Size(), new Size());
			Color = new RangeGraph<Color>(Datatypes.Color.White, Datatypes.Color.White);
			PositionType = ParticleEmitterPositionType.Point;
			StartPosition = new RangeGraph<Vector3D>(Vector3D.Zero, Vector3D.Zero);
			StartRotation = new RangeGraph<ValueRange>(new ValueRange(0.0f), new ValueRange(0.0f));
			RotationSpeed = new RangeGraph<ValueRange>(new ValueRange(0.0f), new ValueRange(0.0f));
			ParticlesPerSpawn = new RangeGraph<ValueRange>(new ValueRange(1, 1), new ValueRange(1, 1));
		}

		public float SpawnInterval { get; set; }
		public float LifeTime { get; set; }
		public int MaximumNumberOfParticles { get; set; }
		public RangeGraph<Vector3D> StartVelocity { get; set; }
		public RangeGraph<Vector3D> Acceleration { get; set; }
		public RangeGraph<Size> Size { get; set; }
		public RangeGraph<ValueRange> StartRotation { get; set; }
		public RangeGraph<ValueRange> RotationSpeed { get; set; }
		public RangeGraph<Color> Color { get; set; }
		public Material ParticleMaterial { get; set; }
		public ParticleEmitterPositionType PositionType { get; set; }
		public RangeGraph<Vector3D> StartPosition { get; set; }
		public RangeGraph<ValueRange> ParticlesPerSpawn { get; set; }
		public bool DoParticlesTrackEmitter { get; set; }
		public BillboardMode BillboardMode { get; set; }

		protected override void LoadData(Stream fileData)
		{
			var emitterData = (ParticleEmitterData)new BinaryReader(fileData).Create();
			SpawnInterval = emitterData.SpawnInterval;
			LifeTime = emitterData.LifeTime;
			MaximumNumberOfParticles = emitterData.MaximumNumberOfParticles;
			StartVelocity = emitterData.StartVelocity;
			Acceleration = emitterData.Acceleration;
			Size = emitterData.Size;
			StartRotation = emitterData.StartRotation;
			Color = emitterData.Color;
			ParticleMaterial = emitterData.ParticleMaterial;
			ParticlesPerSpawn = emitterData.ParticlesPerSpawn;
			RotationSpeed = emitterData.RotationSpeed;
			PositionType = emitterData.PositionType;
			StartPosition = emitterData.StartPosition;
			ParticlesPerSpawn = emitterData.ParticlesPerSpawn;
			DoParticlesTrackEmitter = emitterData.DoParticlesTrackEmitter;
			BillboardMode = emitterData.BillboardMode;
		}

		protected override void DisposeData() {}

		public static ParticleEmitterData CopyFrom(ParticleEmitterData otherData)
		{
			var newEmitterData = new ParticleEmitterData();
			newEmitterData.SpawnInterval = otherData.SpawnInterval;
			newEmitterData.LifeTime = otherData.LifeTime;
			newEmitterData.MaximumNumberOfParticles = otherData.MaximumNumberOfParticles;
			newEmitterData.StartVelocity = new RangeGraph<Vector3D>(otherData.StartVelocity.Values);
			newEmitterData.Acceleration = new RangeGraph<Vector3D>(otherData.Acceleration.Values);
			newEmitterData.Size = new RangeGraph<Size>(otherData.Size.Values);
			newEmitterData.StartRotation = new RangeGraph<ValueRange>(otherData.StartRotation.Values);
			newEmitterData.Color = otherData.Color = new RangeGraph<Color>(otherData.Color.Values);
			newEmitterData.ParticleMaterial = otherData.ParticleMaterial;
			newEmitterData.ParticlesPerSpawn = otherData.ParticlesPerSpawn;
			newEmitterData.RotationSpeed = new RangeGraph<ValueRange>(otherData.RotationSpeed.Values);
			newEmitterData.PositionType = otherData.PositionType;
			newEmitterData.StartPosition = new RangeGraph<Vector3D>(otherData.StartPosition.Values);
			newEmitterData.ParticlesPerSpawn = otherData.ParticlesPerSpawn;
			newEmitterData.DoParticlesTrackEmitter = otherData.DoParticlesTrackEmitter;
			newEmitterData.BillboardMode = otherData.BillboardMode;
			return newEmitterData;
		}

		public void LoadFromFile(Stream fileData)
		{
			if (fileData.Length == 0)
				throw new EmptyEmitterDataFileGiven();
			var reader = new BinaryReader(fileData);
			reader.BaseStream.Position = 0;
			string shortName = reader.ReadString();
			var dataVersion = reader.ReadBytes(4);
			bool justSaveName = reader.ReadBoolean();
			bool noFieldData = reader.ReadBoolean();
			string name = reader.ReadString();
			reader.BaseStream.Position++;
			SpawnInterval = reader.ReadSingle();
			LifeTime = reader.ReadSingle();
			MaximumNumberOfParticles = reader.ReadInt32();
			StartVelocity = ReadRangeOfVectors(reader);
			Acceleration = ReadRangeOfVectors(reader);
			Size = ReadRangeOfSizes(reader);
			StartRotation = ReadRangeOfRanges(reader);
			RotationSpeed = ReadRangeOfRanges(reader);
			Color = ReadRangeOfColors(reader);
			reader.ReadBoolean();
			var materialType = reader.ReadString();
			var justName = reader.ReadBoolean();
			ParticleMaterial = justName
				? ContentLoader.Load<Material>(reader.ReadString()) : LoadCustomMaterial(reader);
			PositionType = (ParticleEmitterPositionType)reader.ReadInt32();
			StartPosition = ReadRangeOfVectors(reader);
			ParticlesPerSpawn = ReadRangeOfRanges(reader);
			DoParticlesTrackEmitter = reader.ReadBoolean();
			BillboardMode = (BillboardMode)reader.ReadInt32();
		}

		public class EmptyEmitterDataFileGiven : Exception {}

		public static RangeGraph<Vector3D> ReadRangeOfVectors(BinaryReader reader)
		{
			reader.ReadBoolean();
			var typeName = reader.ReadString();
			var start = ReadVector3D(reader);
			var end = ReadVector3D(reader);
			reader.ReadByte();
			var count = ReadNumberMostlyBelow255(reader);
			var arrayType = reader.ReadByte();
			var arrayTypeName = reader.ReadString();
			var values = new Vector3D[count];
			for (int i = 0; i < count; i++)
				values[i] = ReadVector3D(reader);
			return new RangeGraph<Vector3D>(values);
		}

		private static int ReadNumberMostlyBelow255(BinaryReader reader)
		{
			int number = reader.ReadByte();
			if (number == 255)
				number = reader.ReadInt32(); //ncrunch: no coverage
			return number;
		}

		private static Vector3D ReadVector3D(BinaryReader reader)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			var z = reader.ReadSingle();
			return new Vector3D(x, y, z);
		}

		private static RangeGraph<Size> ReadRangeOfSizes(BinaryReader reader)
		{
			reader.ReadBoolean();
			var typeName = reader.ReadString();
			var start = ReadSize(reader);
			var end = ReadSize(reader);
			reader.ReadByte();
			var count = ReadNumberMostlyBelow255(reader);
			var arrayType = reader.ReadByte();
			var arrayTypeName = reader.ReadString();
			var values = new Size[count];
			for (int i = 0; i < count; i++)
				values[i] = ReadSize(reader);
			return new RangeGraph<Size>(values);
		}

		private static Size ReadSize(BinaryReader reader)
		{
			var x = reader.ReadSingle();
			var y = reader.ReadSingle();
			return new Size(x, y);
		}

		private static RangeGraph<ValueRange> ReadRangeOfRanges(BinaryReader reader)
		{
			reader.ReadBoolean();
			var typeName = reader.ReadString();
			var start = ReadValueRange(reader);
			var end = ReadValueRange(reader);
			reader.ReadByte();
			var count = ReadNumberMostlyBelow255(reader);
			var arrayType = reader.ReadByte();
			var arrayTypeName = reader.ReadString();
			var values = new ValueRange[count];
			for (int i = 0; i < count; i++)
				values[i] = ReadValueRange(reader);
			return new RangeGraph<ValueRange>(values);
		}

		private static ValueRange ReadValueRange(BinaryReader reader)
		{
			var start = reader.ReadSingle();
			var end = reader.ReadSingle();
			return new ValueRange(start, end);
		}


		private static RangeGraph<Color> ReadRangeOfColors(BinaryReader reader)
		{
			reader.ReadBoolean();
			var typeName = reader.ReadString();
			var start = ReadColor(reader);
			var end = ReadColor(reader);
			reader.ReadByte();
			var count = ReadNumberMostlyBelow255(reader);
			var arrayType = reader.ReadByte();
			var arrayTypeName = reader.ReadString();
			var values = new Color[count];
			for (int i = 0; i < count; i++)
				values[i] = ReadColor(reader);
			return new RangeGraph<Color>(values);
		} 

		private static Color ReadColor(BinaryReader reader)
		{
			var r = reader.ReadByte();
			var g = reader.ReadByte();
			var b = reader.ReadByte();
			var a = reader.ReadByte();
			return new Color(r, g, b, a);
		}

		private static Material LoadCustomMaterial(BinaryReader reader)
		{
			var shaderFlags = (ShaderFlags)reader.ReadInt32();
			var customImageType = reader.ReadByte();
			var pixelSize = customImageType > 0
				? new Size(reader.ReadSingle(), reader.ReadSingle()) : Datatypes.Size.Zero;
			var imageOrAnimationName = customImageType > 0 ? "" : reader.ReadString();
			var customImage = customImageType == 1
				? ContentLoader.Create<Image>(new ImageCreationData(pixelSize)) : null;
			var color = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(),
				reader.ReadByte());
			var duration = reader.ReadSingle();
			var material = customImageType > 0
				? new Material(ContentLoader.Create<Shader>(new ShaderCreationData(shaderFlags)),
					customImage)
				: new Material(shaderFlags, imageOrAnimationName);
			material.DefaultColor = color;
			material.Duration = duration;
			return material;
		}
	}
}