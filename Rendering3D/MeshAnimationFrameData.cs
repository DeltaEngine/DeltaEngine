using System.IO;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering3D
{
	/// <summary>
	/// Defines the transform for each bone of the animation frame.
	/// </summary>
	public class MeshAnimationFrameData
	{
		public MeshAnimationFrameData() { }

		//ncrunch: no coverage start
		public MeshAnimationFrameData(BinaryReader dataReader)
		{
			int numberOfBones = dataReader.ReadInt32();
			TransformsOfBones = new Matrix[numberOfBones];
			for (int i = 0; i < numberOfBones; i++)
				TransformsOfBones[i] = GetBoneTransform(dataReader);
		}

		public Matrix[] TransformsOfBones;

		private static Matrix GetBoneTransform(BinaryReader dataReader)
		{
			int numberOfValues = dataReader.ReadInt32();
			var matrixValues = new float[numberOfValues];
			for (int i = 0; i < numberOfValues; i++)
				matrixValues[i] = dataReader.ReadSingle();
			return new Matrix(matrixValues);
		}
	}
}