using System;
using System.Runtime.InteropServices;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics.OpenGL.Wgl;
using DeltaEngine.Graphics.Vertices;

namespace DeltaEngine.Graphics.OpenGL
{
	public sealed class OpenGLDevice : Device
	{
		private BaseGraphicsContext context;
		private const int BitsPerPixel = 32;
		private BlendMode currentBlendMode = BlendMode.Opaque;
		public const int InvalidHandle = -1;
		private bool isCullingEnabled;

		public OpenGLDevice(Window window)
			: base(window)
		{
			CreateContext();
			SetViewport(window.ViewportPixelSize);
			SetupFrontFaceDirection();
		}

		private void CreateContext()
		{
			context = new WglGraphicsContext(window.Handle);
			context.UpdateDeviceParameters(24, 8, 0);
			context.VSync = Settings.Current.UseVSync;
			context.MakeCurrent();
			string version = GLHelper.GetString(StringName.Version);
			if (string.IsNullOrEmpty(version))
				return;
			int majorVersion = int.Parse(version[0] + "");
			if (majorVersion < 3 || string.IsNullOrEmpty(GLHelper.GetString(StringName.Extensions)))
				throw new OpenGLVersionDoesNotSupportShaders();
		}

		public override void SetViewport(Size viewportSize)
		{
			if (Settings.Current.StartInFullscreen)
			{
				if (!TryChangeResolution(viewportSize))
					throw new ResolutionRequestFailedCouldNotFindResolution(viewportSize);
			}
			GLCore.Viewport(0, 0, (int)viewportSize.Width, (int)viewportSize.Height);
			SetModelViewProjectionMatrixFor2D();
		}

		private static bool TryChangeResolution(Size resolution)
		{
			WglGraphicsContext.DeviceMode lpDevMode = new WglGraphicsContext.DeviceMode();
			lpDevMode.PelsWidth = (int)resolution.Width;
			lpDevMode.PelsHeight = (int)resolution.Height;
			lpDevMode.BitsPerPel = BitsPerPixel;
			lpDevMode.DisplayFrequency = 0;
			lpDevMode.Fields = 6029312;
			return WglGraphicsContext.ChangeDisplaySettingsEx(null, lpDevMode, IntPtr.Zero, WglGraphicsContext.ChangeDisplaySettingsEnum.Fullscreen, IntPtr.Zero) == 0;
		}

		private static void SetupFrontFaceDirection()
		{
			GLCore.FrontFace(FrontFaceDirection.Ccw);
		}

		public override void Clear()
		{
			Color color = window.BackgroundColor;
			if (color.A == 0)
				return;
			GLCore.ClearColor(color.RedValue, color.GreenValue, color.BlueValue, color.AlphaValue);
			GLCore.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
		}

		public override void Present()
		{
			if (!StackTraceExtensions.StartedFromNCrunchOrNunitConsole && context.IsCurrent)
				context.SwapBuffers();
		}

		public override void Dispose()
		{
			context.Dispose();
		}

		public override void SetBlendMode(BlendMode blendMode)
		{
			if (currentBlendMode == blendMode)
				return;
			currentBlendMode = blendMode;
			switch (blendMode)
			{
				case BlendMode.Opaque:
					GLCore.Disable(EnableCap.Blend);
					break;
				case BlendMode.Normal:
					GLCore.Enable(EnableCap.Blend);
					GLCore.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
					GLCore.BlendEquation(BlendEquationMode.FuncAdd);
					break;
				case BlendMode.AlphaTest:
					GLCore.Disable(EnableCap.Blend);
					GLCore.Enable(EnableCap.AlphaTest);
					break;
				case BlendMode.Additive:
					GLCore.Enable(EnableCap.Blend);
					GLCore.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
					GLCore.BlendEquation(BlendEquationMode.FuncAdd);
					break;
				case BlendMode.Subtractive:
					GLCore.Enable(EnableCap.Blend);
					GLCore.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
					GLCore.BlendEquation(BlendEquationMode.FuncReverseSubtract);
					break;
				case BlendMode.LightEffect:
					GLCore.Enable(EnableCap.Blend);
					GLCore.BlendFunc(BlendingFactorSrc.DstColor, BlendingFactorDest.One);
					GLCore.BlendEquation(BlendEquationMode.FuncAdd);
					break;
			}
		}

		public int CreateVertexBuffer(int sizeInBytes, OpenGL20BufferMode mode)
		{
			uint bufferHandle = GLHelper.GenBuffer();
			GLCore.BindBuffer(BufferTarget.ArrayBuffer, bufferHandle);
			GLCore.BufferData(BufferTarget.ArrayBuffer, (IntPtr)sizeInBytes, IntPtr.Zero, GetBufferMode(mode));
			return (int)bufferHandle;
		}

		private static BufferUsageHint GetBufferMode(OpenGL20BufferMode mode)
		{
			return mode == OpenGL20BufferMode.Static ? BufferUsageHint.StaticDraw : mode == OpenGL20BufferMode.Dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StreamDraw;
		}

		public void BindVertexBuffer(int bufferHandle)
		{
			GLCore.BindBuffer(BufferTarget.ArrayBuffer, (uint)bufferHandle);
		}

		public void LoadVertexData<T>(int offset, T[] vertices, int vertexDataSizeInBytes)
			where T : struct
		{
			GLHelper.BufferSubData(BufferTarget.ArrayBuffer, offset, vertexDataSizeInBytes, vertices);
		}

		public int CreateIndexBuffer(int sizeInBytes, OpenGL20BufferMode mode)
		{
			uint bufferHandle = GLHelper.GenBuffer();
			GLCore.BindBuffer(BufferTarget.ElementArrayBuffer, bufferHandle);
			GLCore.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)sizeInBytes, IntPtr.Zero, GetBufferMode(mode));
			return (int)bufferHandle;
		}

		public void BindIndexBuffer(int bufferHandle)
		{
			GLCore.BindBuffer(BufferTarget.ElementArrayBuffer, (uint)bufferHandle);
		}

		public void LoadIndices(int offset, short[] indices, int indexDataSizeInBytes)
		{
			GLHelper.BufferSubData(BufferTarget.ElementArrayBuffer, offset, indexDataSizeInBytes, indices);
		}

		public void DeleteBuffer(int bufferHandle)
		{
			GLHelper.DeleteBuffer((uint)bufferHandle);
		}

		private void NativeEnableCulling()
		{
			GLCore.Enable(EnableCap.CullFace);
			GLCore.CullFace(CullFaceMode.Back);
		}

		private void NativeDisableCulling()
		{
			GLCore.Disable(EnableCap.CullFace);
		}

		public override void EnableDepthTest()
		{
			GLCore.Enable(EnableCap.DepthTest);
		}

		public override void DisableDepthTest()
		{
			GLCore.Disable(EnableCap.DepthTest);
		}

		public int GenerateTexture()
		{
			return (int)GLHelper.GenTexture();
		}

		public void BindTexture(int glHandle, int samplerIndex = 0)
		{
			GLCore.ActiveTexture(GetSamplerFrom(samplerIndex));
			GLCore.BindTexture(TextureTarget.Texture2D, (uint)glHandle);
		}

		private static TextureUnit GetSamplerFrom(int samplerIndex)
		{
			if (samplerIndex == 0)
				return TextureUnit.Texture0;
			if (samplerIndex == 1)
				return TextureUnit.Texture1;
			throw new UnsupportedTextureUnit();
		}

		public void LoadTextureInNativePlatformFormat(int width, int height, IntPtr nativeLoadedData, bool hasAlpha)
		{
			GLCore.TexImage2D(TextureTarget.Texture2D, 0, (int)(hasAlpha ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb), width, height, 0, (PixelInternalFormat)(hasAlpha ? PixelFormat.Bgra : PixelFormat.Bgr), PixelType.UnsignedByte, nativeLoadedData);
		}

		public unsafe void FillTexture(Size size, byte[] rgbaData, bool hasAlpha)
		{
			fixed (byte* ptr = &rgbaData[0])
				GLCore.TexImage2D(TextureTarget.Texture2D, 0, (int)(hasAlpha ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb), (int)size.Width, (int)size.Height, 0, (PixelInternalFormat)PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)ptr);
		}

		public void DeleteTexture(int glHandle)
		{
			GLHelper.DeleteTexture((uint)glHandle);
		}

		public void SetTextureSamplerState(bool disableLinearFiltering = false, bool allowTiling = false)
		{
			TextureMinFilter minFilter = disableLinearFiltering ? TextureMinFilter.Nearest : TextureMinFilter.Linear;
			TextureMagFilter magFilter = disableLinearFiltering ? TextureMagFilter.Nearest : TextureMagFilter.Linear;
			TextureWrapMode clampMode = allowTiling ? TextureWrapMode.Repeat : TextureWrapMode.ClampToEdge;
			SetSamplerState((int)minFilter, (int)magFilter, (int)clampMode);
		}

		private static void SetSamplerState(int textureMinFilter, int textureMagFilter, int clampMode)
		{
			SetTexture2DParameter(TextureParameterName.TextureMinFilter, textureMinFilter);
			SetTexture2DParameter(TextureParameterName.TextureMagFilter, textureMagFilter);
			SetTexture2DParameter(TextureParameterName.TextureWrapS, clampMode);
			SetTexture2DParameter(TextureParameterName.TextureWrapT, clampMode);
		}

		private static void SetTexture2DParameter(TextureParameterName name, int value)
		{
			GLCore.TexParameteri(TextureTarget.Texture2D, name, value);
		}

		public void UseShaderProgram(int programHandle)
		{
			GLCore.UseProgram((uint)programHandle);
		}

		public void DeleteShaderProgram(int programHandle)
		{
			GLCore.DeleteProgram((uint)programHandle);
		}

		public int GetShaderAttributeLocation(int programHandle, string attributeName)
		{
			return GLCore.GetAttribLocation((uint)programHandle, attributeName);
		}

		public int GetShaderUniformLocation(int programHandle, string uniformName)
		{
			return GLCore.GetUniformLocation((uint)programHandle, uniformName);
		}

		public void DefineVertexAttributeWithFloats(int attributeLocation, int numberOfFloatComponents, int vertexTotalSize, int attributeOffsetInVertex)
		{
			GLCore.EnableVertexAttribArray((uint)attributeLocation);
			GLCore.VertexAttribPointer((uint)attributeLocation, numberOfFloatComponents, VertexAttribPointerType.Float, false, vertexTotalSize, (IntPtr)attributeOffsetInVertex);
		}

		public void DefineVertexAttributeWithBytes(int attributeLocation, int numberOfByteComponents, int vertexTotalSize, int attributeOffsetInVertex)
		{
			GLCore.EnableVertexAttribArray((uint)attributeLocation);
			GLCore.VertexAttribPointer((uint)attributeLocation, numberOfByteComponents, VertexAttribPointerType.UnsignedByte, true, vertexTotalSize, (IntPtr)attributeOffsetInVertex);
		}

		public void SetUniformValue(int location, int value)
		{
			GLCore.Uniform1i(location, value);
		}

		public void SetUniformValue(int location, float value)
		{
			GLCore.Uniform1f(location, value);
		}

		public void SetUniformValue(int location, Matrix matrix)
		{
			GLCore.UniformMatrix4fv(location, 1, false, matrix.GetValues);
		}

		public void SetUniformValue(int location, Vector3D vector)
		{
			GLCore.Uniform3f(location, vector.X, vector.Y, vector.Z);
		}

		public void SetUniformValue(int location, float r, float g, float b, float a)
		{
			GLCore.Uniform4f(location, r, g, b, a);
		}

		public void SetUniformValues(int uniformLocation, Matrix[] matrices)
		{
			float[] values = new float[matrices.Length * 16];
			for (int matrixIndex = 0; matrixIndex < matrices.Length; ++matrixIndex)
				matrices[matrixIndex].GetValues.CopyTo(values, matrixIndex * 16);
			GLCore.UniformMatrix4fv(uniformLocation, matrices.Length, false, values);
		}

		public void DrawTriangles(int indexOffsetInBytes, int numberOfIndicesToRender)
		{
			Shader.BindVertexDeclaration();
			GLCore.DrawElements(BeginMode.Triangles, numberOfIndicesToRender, DrawElementsType.UnsignedShort, (IntPtr)indexOffsetInBytes);
		}

		public void DrawLines(int vertexOffset, int verticesCount)
		{
			Shader.BindVertexDeclaration();
			GLCore.DrawArrays(BeginMode.Lines, vertexOffset, verticesCount);
		}

		public unsafe void ReadPixels(Rectangle frame, byte[] bufferToStoreData)
		{
			fixed (byte* ptr = &bufferToStoreData[0])
				GLCore.ReadPixels((int)frame.Left, (int)frame.Top, (int)frame.Width, (int)frame.Height, PixelFormat.Rgb, PixelType.UnsignedByte, (IntPtr)ptr);
		}

		public override CircularBuffer CreateCircularBuffer(ShaderWithFormat shader, BlendMode blendMode, VerticesMode drawMode = VerticesMode.Triangles)
		{
			return new OpenGL20CircularBuffer(this, shader, blendMode, drawMode);
		}

		protected override void EnableClockwiseBackfaceCulling()
		{
			if (isCullingEnabled)
				return;
			isCullingEnabled = true;
			NativeEnableCulling();
		}

		protected override void DisableCulling()
		{
			if (!isCullingEnabled)
				return;
			isCullingEnabled = false;
			NativeDisableCulling();
		}

		private class OpenGLVersionDoesNotSupportShaders : Exception {}

		private class ResolutionRequestFailedCouldNotFindResolution : Exception
		{
			public ResolutionRequestFailedCouldNotFindResolution(Size resolution)
				: base(resolution.ToString()) {}
		}

		private class UnsupportedTextureUnit : Exception {}
	}
}