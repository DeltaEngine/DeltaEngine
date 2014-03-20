using DeltaEngine.Content;

namespace DeltaEngine.Rendering3D
{
	/// <summary>
	/// For creating MeshAnimations without loading through the content system
	/// </summary>
	public class MeshAnimationCreationData : ContentCreationData
	{
		public MeshAnimationCreationData(int numberOfAnimationFrames, float framesPerSecond)
		{
			NumberOfFrames = numberOfAnimationFrames;
			FramesPerSecond = framesPerSecond;
		}

		public int NumberOfFrames { get; private set; }
		public float FramesPerSecond { get; private set; }
	}
}