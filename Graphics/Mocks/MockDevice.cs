using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Graphics.Mocks
{
	/// <summary>
	/// Mock device used in unit tests.
	/// </summary>
	public class MockDevice : Device
	{
		public MockDevice(Window window)
			: base(window)
		{
			OnSet3DMode += () => OnSet3DModeCalled = true;
		}

		public override void Dispose() {}
		public override void Clear() {}
		public override void Present() {}
		public override void EnableDepthTest() {}
		public override void SetViewport(Size viewportSize) {}
		public override void DisableDepthTest() {}
		public override void SetBlendMode(BlendMode blendMode) {}

		public override CircularBuffer CreateCircularBuffer(ShaderWithFormat shader,
			BlendMode blendMode, VerticesMode drawMode = VerticesMode.Triangles)
		{
			return new MockCircularBuffer(this, shader, blendMode, drawMode);
		}

		protected override void OnFullscreenChanged(Size displaySize, bool wantFullscreen)
		{
			OnFullscreenChangedCalled = true;
			base.OnFullscreenChanged(displaySize, wantFullscreen);
		}

		protected override void EnableClockwiseBackfaceCulling() {}
		protected override void DisableCulling() {}

		public bool OnFullscreenChangedCalled { get; private set; }
		public bool OnSet3DModeCalled { get; private set; }
	}
}