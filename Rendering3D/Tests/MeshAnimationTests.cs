using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Tests
{
	internal class MeshAnimationTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void InitializeCamera()
		{
			LookAtCamera camera = Camera.Use<LookAtCamera>();
			camera.Position = new Vector3D(0, 3, 0);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateMeshAnimationByCreationData()
		{
			MeshAnimation animation = CreateAnimationWith75Frames(FramesPerSecond);
			Assert.AreEqual(NumberOfAnimationFrames, animation.NumberOfFrames);
			Assert.AreEqual(FramesPerSecond, animation.FramesPerSecond);
			Assert.AreEqual(NumberOfAnimationFrames, animation.Frames.Length);
			Assert.AreEqual(3, animation.Duration);
		}

		private static MeshAnimation CreateAnimationWith75Frames(float framesPerSecond)
		{
			var animationData = new MeshAnimationCreationData(NumberOfAnimationFrames, framesPerSecond);
			return ContentLoader.Create<MeshAnimation>(animationData);
		}

		private const float FramesPerSecond = 25;
		private const int NumberOfAnimationFrames = 75;

		[Test]
		public void ShowWavingSkinnedRectangleFromModelData()
		{
			var geometry = CreateSkinnedGeometry();
			var skinnedMesh = new Mesh(geometry,
				new Material(ShaderFlags.TexturedSkinned, "DeltaEngineLogoOpaque"));
			skinnedMesh.Animation = CreateWavingMeshAnimationWith60Fps();
			new Model(new ModelData(skinnedMesh), Vector3D.Zero);
		}

		private static Geometry CreateSkinnedGeometry()
		{
			Vertex[] planeVertices = GetSkinnedPlaneVerticesFromBottomToTop();
			short[] planeIndices = GetSkinnedPlaneIndices();
			var geometry = ContentLoader.Create<Geometry>(new GeometryCreationData(
				VertexFormat.Position3DUVSkinned, planeVertices.Length, planeIndices.Length));
			geometry.SetData(planeVertices, planeIndices);
			return geometry;
		}

		private static Vertex[] GetSkinnedPlaneVerticesFromBottomToTop()
		{
			return new Vertex[]
			{
				new VertexPosition3DUVSkinned(new Vector3D(-0.5f, 0.0f, -1.0f), Vector2D.One,
					new SkinningData(0, 0, 1.0f, 0.0f)),
				new VertexPosition3DUVSkinned(new Vector3D(0.5f, 0.0f, -1.0f), Vector2D.UnitY,
					new SkinningData(0, 0, 1.0f, 0.0f)),
				new VertexPosition3DUVSkinned(new Vector3D(-0.5f, 0.0f, 0.0f), Vector2D.UnitX,
					new SkinningData(0, 1, 0.5f, 0.5f)),
				new VertexPosition3DUVSkinned(new Vector3D(0.5f, 0.0f, 0.0f), Vector2D.Zero,
					new SkinningData(0, 1, 0.5f, 0.5f)),
				new VertexPosition3DUVSkinned(new Vector3D(-0.5f, 0.0f, 1.0f), Vector2D.One,
					new SkinningData(1, 1, 1.0f, 0.0f)),
				new VertexPosition3DUVSkinned(new Vector3D(0.5f, 0.0f, 1.0f), Vector2D.UnitY,
					new SkinningData(1, 1, 1.0f, 0.0f))
			};
		}

		private static short[] GetSkinnedPlaneIndices()
		{
			return new short[] { 1, 0, 3, 3, 0, 2, 3, 2, 5, 5, 2, 4 };
		}

		private static MeshAnimation CreateWavingMeshAnimationWith60Fps()
		{
			const float WavingAnimationFramesPerSecond = 60;
			MeshAnimation animation = CreateAnimationWith75Frames(WavingAnimationFramesPerSecond);
			animation.LoadData(GetWavingAnimationData(WavingAnimationFramesPerSecond));
			return animation;
		}

		private static MeshAnimation.MeshAnimationData GetWavingAnimationData(float framesPerSecond)
		{
			var animationData = new MeshAnimation.MeshAnimationData();
			animationData.FramesPerSecond = framesPerSecond;
			animationData.FramesData = new MeshAnimationFrameData[NumberOfAnimationFrames];
			for (int frameIndex = 0; frameIndex < NumberOfAnimationFrames; frameIndex++)
				animationData.FramesData[frameIndex] = CreateFrameDataWithBonesTransforms(frameIndex);
			return animationData;
		}

		private static MeshAnimationFrameData CreateFrameDataWithBonesTransforms(int frameIndex)
		{
			var frameData = new MeshAnimationFrameData { TransformsOfBones = new Matrix[NumberOfBones] };
			for (int boneIndex = 0; boneIndex < NumberOfBones; boneIndex++)
				frameData.TransformsOfBones[boneIndex] = GetBoneTransform(frameIndex, boneIndex);
			return frameData;
		}

		private const int NumberOfBones = 2;

		private static Matrix GetBoneTransform(int frameIndex, int boneIndex)
		{
			if (boneIndex != (NumberOfBones - 1))
				return Matrix.Identity;
			float circleStep = ((float)frameIndex / NumberOfAnimationFrames) * 360.0f;
			const float AnimationMaximumBendAngle = 45.0f;
			return Matrix.CreateRotationZYX(MathExtensions.Sin(circleStep) * AnimationMaximumBendAngle,
				MathExtensions.Sin(circleStep) * AnimationMaximumBendAngle,
				MathExtensions.Cos(circleStep) * AnimationMaximumBendAngle);
		}
	}
}