using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering3D
{
	/// <summary>
	/// 3D geometry with material information ready to be rendered in a scene
	/// </summary>
	public class Mesh : ContentData
	{
		public Mesh(string contentName)
			: base(contentName) {}

		public Mesh(Geometry geometry, Material material)
			: base("<GeneratedMesh Geometry=" + geometry + ", material=" + material + ">")
		{
			Geometry = geometry;
			Material = material;
			LocalTransform = Matrix.Identity;
		}

		public Geometry Geometry { get; set; }
		public Material Material { get; set; }
		public Matrix LocalTransform { get; set; }

		protected override void LoadData(Stream fileData)
		{
			Geometry = ContentLoader.Load<Geometry>(MetaData.Get("GeometryName", ""));
			Material = ContentLoader.Load<Material>(MetaData.Get("MaterialName", ""));
			string matrix = MetaData.Get("LocalTransform", "");
			LocalTransform = String.IsNullOrEmpty(matrix) ? Matrix.Identity : new Matrix(matrix);
			string animationName = MetaData.Get("AnimationName", "");
			if (!string.IsNullOrEmpty(animationName))
				Animation = ContentLoader.Load<MeshAnimation>(animationName); //ncrunch: no coverage
		}

		public bool IsAnimated
		{
			get { return Animation != null; }
		}

		public MeshAnimation Animation { get; set; }

		protected override void DisposeData() {}
	}
}