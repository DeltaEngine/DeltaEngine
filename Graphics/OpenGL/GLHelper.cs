using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DeltaEngine.Graphics.OpenGL
{
	public static class GLHelper
	{
		private const ClearBufferMask ColorDepthMask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit;
		private const float ColorMultiplier = 1f / 255f;

		public static unsafe string GetString(StringName stringName)
		{
			return new string(GLCore.GetString(stringName));
		}

		public static unsafe uint GetQueryObjectuiv(uint query, GetQueryObjectParam param)
		{
			uint[] result = new uint[1];
			fixed (uint* ptr = &result[0])
				GLCore.GetQueryObjectuiv(query, param, ptr);
			return result[0];
		}

		public static void DrawElements(BeginMode mode, int count, DrawElementsType type)
		{
			GLCore.DrawElements(mode, count, type, IntPtr.Zero);
		}

		public static unsafe uint GenQuery()
		{
			uint[] queries = new uint[1];
			fixed (uint* ptr = &queries[0])
				GLCore.GenQueries(1, ptr);
			return queries[0];
		}

		public static unsafe void DeleteQuery(uint query)
		{
			uint[] queries = { query };
			fixed (uint* ptr = &queries[0])
				GLCore.DeleteQueries(1, ptr);
		}

		public static unsafe void DeleteQueries(uint[] queries)
		{
			fixed (uint* ptr = &queries[0])
				GLCore.DeleteQueries(queries.Length, ptr);
		}

		public static unsafe uint[] GenQueries(int count)
		{
			uint[] queries = new uint[count];
			fixed (uint* ptr = &queries[0])
				GLCore.GenQueries(count, ptr);
			return queries;
		}

		public static unsafe uint GenBuffer()
		{
			uint[] buffers = new uint[1];
			fixed (uint* ptr = &buffers[0])
				GLCore.GenBuffers(1, ptr);
			return buffers[0];
		}

		public static void ClearColor(byte r, byte g, byte b)
		{
			GLCore.ClearColor(r * ColorMultiplier, g * ColorMultiplier, b * ColorMultiplier, 0f);
		}

		public static void ClearDepthAndColor()
		{
			GLCore.Clear(ColorDepthMask);
		}

		public static unsafe void DeleteBuffer(uint buffer)
		{
			uint[] buffers = { buffer };
			fixed (uint* ptr = &buffers[0])
				GLCore.DeleteBuffers(1, ptr);
		}

		public static void BufferData<T>(BufferTarget target, int size, T[] data, BufferUsageHint usage)
		{
			GCHandle ptr = GCHandle.Alloc(data, GCHandleType.Pinned);
			GLCore.BufferData(target, (IntPtr)size, ptr.AddrOfPinnedObject(), usage);
			ptr.Free();
		}

		public static void BufferSubData<T>(BufferTarget target, int offset, int size, T[] data)
		{
			GCHandle ptr = GCHandle.Alloc(data, GCHandleType.Pinned);
			GLCore.BufferSubData(target, (IntPtr)offset, (IntPtr)size, ptr.AddrOfPinnedObject());
			ptr.Free();
		}

		public static unsafe uint GenTexture()
		{
			uint[] textures = new uint[1];
			fixed (uint* ptr = &textures[0])
				GLCore.GenTextures(1, ptr);
			return textures[0];
		}

		public static unsafe void DeleteTexture(uint texture)
		{
			uint[] textures = { texture };
			fixed (uint* ptr = &textures[0])
				GLCore.DeleteTextures(1, ptr);
		}

		public static unsafe uint GenRenderbuffer()
		{
			uint[] buffers = new uint[1];
			fixed (uint* ptr = &buffers[0])
				GLCore.GenRenderbuffers(1, ptr);
			return buffers[0];
		}

		public static unsafe void DeleteRenderbuffer(uint renderBuffer)
		{
			uint[] buffers = { renderBuffer };
			fixed (uint* ptr = &buffers[0])
				GLCore.DeleteRenderbuffers(1, ptr);
		}

		public static unsafe uint GenFramebuffer()
		{
			uint[] buffers = new uint[1];
			fixed (uint* ptr = &buffers[0])
				GLCore.GenFramebuffers(1, ptr);
			return buffers[0];
		}

		public static unsafe void DeleteFramebuffer(uint frameBuffer)
		{
			uint[] buffers = { frameBuffer };
			fixed (uint* ptr = &buffers[0])
				GLCore.DeleteFramebuffers(1, ptr);
		}

		public static void ShaderSource(uint shader, string shaderSource)
		{
			GLCore.ShaderSource(shader, 1, new string[] { shaderSource }, new int[] { shaderSource.Length });
		}

		public static unsafe string GetShaderInfoLog(uint shader)
		{
			StringBuilder builder = new StringBuilder(1024);
			int[] length = new int[1];
			fixed (int* ptr = &length[0])
				GLCore.GetShaderInfoLog(shader, builder.Capacity, ptr, builder);
			return builder.ToString();
		}

		public static int GetProgramiv(uint program, ProgramParameter name)
		{
			int[] result = new int[1];
			GLCore.GetProgramiv(program, name, result);
			return result[0];
		}

		public static unsafe string GetActiveUniformName(uint program, uint uniformIndex)
		{
			int[] length = new int[1];
			StringBuilder result = new StringBuilder();
			fixed (int* lenPtr = &length[0])
			{
				GLCore.GetActiveUniformName(program, uniformIndex, 2048, lenPtr, result);
			}
			return result.ToString();
		}

		public static unsafe string GetActiveAttrib(uint program, uint attributeIndex, out int attributeSize, out ActiveAttribType attributeType)
		{
			int[] length = new int[1];
			int[] size = new int[1];
			StringBuilder result = new StringBuilder();
			ActiveAttribType[] type = new ActiveAttribType[1];
			fixed (int* lenPtr = &length[0])
				fixed (ActiveAttribType* typePtr = &type[0])
				{
					GLCore.GetActiveAttrib(program, attributeIndex, 1024, lenPtr, size, (IntPtr)typePtr, result);
				}
			attributeSize = size[0];
			attributeType = type[0];
			return result.ToString();
		}

		public static int GetBufferParameteriv(BufferTarget target, BufferParameterName parameterName)
		{
			int[] result = new int[1];
			GLCore.GetBufferParameteriv(target, parameterName, result);
			return result[0];
		}
	}
}