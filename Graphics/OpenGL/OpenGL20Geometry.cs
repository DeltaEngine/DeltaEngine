using System;

namespace DeltaEngine.Graphics.OpenGL
{
	public class OpenGL20Geometry : Geometry
	{
		private readonly OpenGLDevice device;
		private int vertexBufferHandle = OpenGLDevice.InvalidHandle;
		private int indexBufferHandle = OpenGLDevice.InvalidHandle;

		protected OpenGL20Geometry(string contentName, OpenGLDevice device)
			: base(contentName)
		{
			this.device = device;
		}

		private OpenGL20Geometry(GeometryCreationData creationData, OpenGLDevice device)
			: base(creationData)
		{
			this.device = device;
		}

		protected override void SetNativeData(byte[] vertexData, short[] indices)
		{
			if (vertexBufferHandle == OpenGLDevice.InvalidHandle)
				CreateBuffers();
			device.LoadVertexData(0, vertexData, vertexData.Length);
			device.LoadIndices(0, indices, indices.Length * sizeof(short));
		}

		private void CreateBuffers()
		{
			int vertexDataSize = NumberOfVertices * Format.Stride;
			vertexBufferHandle = device.CreateVertexBuffer(vertexDataSize, OpenGL20BufferMode.Static);
			if (vertexBufferHandle == OpenGLDevice.InvalidHandle)
				throw new UnableToCreateOpenGLGeometry();
			indexBufferHandle = device.CreateIndexBuffer(NumberOfIndices * sizeof(short), OpenGL20BufferMode.Static);
		}

		public override void Draw()
		{
			if (vertexBufferHandle == OpenGLDevice.InvalidHandle)
				throw new UnableToDrawDynamicGeometrySetDataNeedsToBeCalledFirst();
			device.BindVertexBuffer(vertexBufferHandle);
			device.BindIndexBuffer(indexBufferHandle);
			device.DrawTriangles(0, NumberOfIndices);
		}

		protected override void DisposeData()
		{
			if (vertexBufferHandle == OpenGLDevice.InvalidHandle)
				return;
			device.DeleteBuffer(vertexBufferHandle);
			device.DeleteBuffer(indexBufferHandle);
			vertexBufferHandle = OpenGLDevice.InvalidHandle;
		}

		private class UnableToCreateOpenGLGeometry : Exception {}

		private class UnableToDrawDynamicGeometrySetDataNeedsToBeCalledFirst : Exception {}
	}
}