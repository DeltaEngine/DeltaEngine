using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering3D
{
	/// <summary>
	/// Consists of a number of frames of animation. Defaults to 30fps
	/// </summary>
	public class MeshAnimation : ContentData
	{
		//ncrunch: no coverage start
		protected MeshAnimation(string contentName)
			: base(contentName) {}
		//ncrunch: no coverage end

		public MeshAnimation(MeshAnimationCreationData creationData)
			: base("<GeneratedMeshAnimationCreationData>")
		{
			FramesPerSecond = creationData.FramesPerSecond;
			Frames = new MeshAnimationFrameData[creationData.NumberOfFrames];
		}

		public float FramesPerSecond { get; private set; }
		public MeshAnimationFrameData[] Frames { get; private set; }

		public int NumberOfFrames
		{
			get { return Frames.Length; }
		}

		public float Duration
		{
			get { return NumberOfFrames / FramesPerSecond; }
		}

		//ncrunch: no coverage start
		protected override void LoadData(Stream fileData)
		{
			// Outstanding feature, see case 11039
			//LoadData(new BinaryReader(fileData).Create() as MeshAnimationData);
			var loadedAnimationData = new MeshAnimationData();
			loadedAnimationData.LoadData(new BinaryReader(fileData));
			LoadData(loadedAnimationData);
		}
		//ncrunch: no coverage end

		public void LoadData(MeshAnimationData animationData)
		{
			FramesPerSecond = animationData.FramesPerSecond;
			Frames = animationData.FramesData;
		}

		public class MeshAnimationData
		{
			public float FramesPerSecond { get; set; }
			public MeshAnimationFrameData[] FramesData { get; set; }

			//ncrunch: no coverage start
			public void LoadData(BinaryReader dataReader)
			{
				int dataVersion = dataReader.ReadInt32();
				FramesPerSecond = dataReader.ReadSingle();
				int numberOfFrames = dataReader.ReadInt32();
				FramesData = new MeshAnimationFrameData[numberOfFrames];
				for (int i = 0; i < numberOfFrames; i++)
					FramesData[i] = new MeshAnimationFrameData(dataReader);
			}
		}
		//ncrunch: no coverage end

		public void UpdateFrameTime()
		{
			currentFrameIndex = (int)(Time.Total * FramesPerSecond) % NumberOfFrames;
		}

		private int currentFrameIndex;

		public MeshAnimationFrameData CurrentFrame
		{
			get { return Frames[currentFrameIndex]; }
		}

		protected override void DisposeData() {}
	}
}