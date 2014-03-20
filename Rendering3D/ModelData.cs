using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Extensions;

namespace DeltaEngine.Rendering3D
{
	/// <summary>
	/// Collections of meshes to be rendered which can be animated
	/// </summary>
	public class ModelData : ContentData
	{
		protected ModelData(string contentName)
			: base(contentName) {}

		public ModelData(params Mesh[] meshes)
			: base("<GeneratedModel>")
		{
			Meshes = meshes;
		}

		public Mesh[] Meshes { get; private set; }

		protected override void LoadData(Stream fileData)
		{
			var meshNames = MetaData.Get("MeshNames", "").SplitAndTrim(',');
			if (meshNames.Length < 1)
				throw new NoMeshesGivenNeedAtLeastOne();
			Meshes = new Mesh[meshNames.Length];
			for (int num = 0; num < meshNames.Length; num++)
				Meshes[num] = ContentLoader.Load<Mesh>(meshNames[num]);
		}

		public class NoMeshesGivenNeedAtLeastOne : Exception {}

		protected override void DisposeData() {}
	}
}