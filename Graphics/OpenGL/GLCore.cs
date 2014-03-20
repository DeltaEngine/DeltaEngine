using System;
using System.Text;

namespace DeltaEngine.Graphics.OpenGL
{
	public static class GLCore
	{
		private delegate void glActiveTexture(TextureUnit texture);
		private delegate void glAttachShader(uint program, uint shader);
		private delegate void glBeginConditionalRender(uint id, ConditionalRenderType mode);
		private delegate void glEndConditionalRender();
		private delegate void glBeginQuery(QueryTarget target, uint id);
		private delegate void glEndQuery(QueryTarget target);
		private delegate void glBeginTransformFeedback(ExtTransformFeedback primitiveMode);
		private delegate void glEndTransformFeedback();
		private delegate void glBindAttribLocation(uint program, uint index, string name);
		private delegate void glBindBuffer(BufferTarget target, uint buffer);
		private delegate void glBindBufferBase(ExtTransformFeedback target, uint index, uint buffer);
		private delegate void glBindBufferRange(ExtTransformFeedback target, uint index, uint buffer, IntPtr offset, IntPtr size);
		private delegate void glBindFragDataLocation(uint program, uint colorNumber, string name);
		private delegate void glBindFragDataLocationIndexed(uint program, uint colorNumber, uint index, string name);
		private delegate void glBindFramebuffer(FramebufferTarget target, uint framebuffer);
		private delegate void glBindRenderbuffer(RenderbufferTarget target, uint renderbuffer);
		private delegate void glBindSampler(uint unit, uint sampler);
		private delegate void glBindTexture(TextureTarget target, uint texture);
		private delegate void glBindVertexArray(uint array);
		private delegate void glBlendColor(float red, float green, float blue, float alpha);
		private delegate void glBlendEquation(BlendEquationMode mode);
		private delegate void glBlendEquationSeparate(BlendEquationMode modeRGB, BlendEquationMode modeAlpha);
		private delegate void glBlendFunc(BlendingFactorSrc sfactor, BlendingFactorDest dfactor);
		private delegate void glBlendFuncSeparate(BlendingFactorSrc srcRGB, BlendingFactorDest dstRGB, BlendingFactorSrc srcAlpha, BlendingFactorDest dstAlpha);
		private delegate void glBlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, ClearBufferMask mask, BlitFramebufferFilter filter);
		private delegate void glBufferData(BufferTarget target, IntPtr size, IntPtr data, BufferUsageHint usage);
		private delegate void glBufferSubData(BufferTarget target, IntPtr offset, IntPtr size, IntPtr data);
		private delegate FramebufferErrorCode glCheckFramebufferStatus(FramebufferTarget target);
		private delegate void glClampColor(ArbColorBufferFloat target, ArbColorBufferFloat clamp);
		private delegate void glClear(ClearBufferMask mask);
		private delegate void glClearBufferiv(ClearBuffer buffer, int drawBuffer, int[] value);
		private unsafe delegate void glClearBufferuiv(ClearBuffer buffer, int drawBuffer, uint* value);
		private delegate void glClearBufferfv(ClearBuffer buffer, int drawBuffer, float[] value);
		private delegate void glClearBufferfi(ClearBuffer buffer, int drawBuffer, float depth, int stencil);
		private delegate void glClearColor(float red, float green, float blue, float alpha);
		private delegate void glClearDepth(double depth);
		private delegate void glClearStencil(int s);
		private delegate All glClientWaitSync(IntPtr sync, All flags, ulong timeout);
		private delegate void glColorMask(bool red, bool green, bool blue, bool alpha);
		private delegate void glColorMaski(uint buf, bool red, bool green, bool blue, bool alpha);
		private delegate void glCompileShader(uint shader);
		private delegate void glCompressedTexImage1D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int border, int imageSize, IntPtr data);
		private delegate void glCompressedTexImage2D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int border, int imageSize, IntPtr data);
		private delegate void glCompressedTexImage3D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int depth, int border, int imageSize, IntPtr data);
		private delegate void glCompressedTexSubImage1D(TextureTarget target, int level, int xoffset, int width, PixelFormat format, int imageSize, IntPtr data);
		private delegate void glCompressedTexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, int imageSize, IntPtr data);
		private delegate void glCompressedTexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, int imageSize, IntPtr data);
		private delegate void glCopyBufferSubData(BufferTarget readtarget, BufferTarget writetarget, IntPtr readoffset, IntPtr writeoffset, IntPtr size);
		private delegate void glCopyTexImage1D(TextureTarget target, int level, PixelInternalFormat internalformat, int x, int y, int width, int border);
		private delegate void glCopyTexImage2D(TextureTarget target, int level, PixelInternalFormat internalformat, int x, int y, int width, int height, int border);
		private delegate void glCopyTexSubImage1D(TextureTarget target, int level, int xoffset, int x, int y, int width);
		private delegate void glCopyTexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int x, int y, int width, int height);
		private delegate void glCopyTexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width, int height);
		private delegate uint glCreateProgram();
		private delegate uint glCreateShader(ShaderType shaderType);
		private delegate void glCullFace(CullFaceMode mode);
		private unsafe delegate void glDeleteBuffers(int n, uint* buffers);
		private unsafe delegate void glDeleteFramebuffers(int n, uint* framebuffers);
		private delegate void glDeleteProgram(uint program);
		private unsafe delegate void glDeleteQueries(int n, uint* ids);
		private unsafe delegate void glDeleteRenderbuffers(int n, uint* renderbuffers);
		private unsafe delegate void glDeleteSamplers(int n, uint* ids);
		private delegate void glDeleteShader(uint shader);
		private delegate void glDeleteSync(IntPtr sync);
		private unsafe delegate void glDeleteTextures(int n, uint* textures);
		private unsafe delegate void glDeleteVertexArrays(int n, uint* arrays);
		private delegate void glDepthFunc(DepthFunction func);
		private delegate void glDepthMask(bool flag);
		private delegate void glDepthRange(double nearVal, double farVal);
		private delegate void glDetachShader(uint program, uint shader);
		private delegate void glDrawArrays(BeginMode mode, int first, int count);
		private delegate void glDrawArraysInstanced(BeginMode mode, int first, int count, int primcount);
		private delegate void glDrawBuffer(DrawBufferMode mode);
		private delegate void glDrawBuffers(int n, IntPtr bufs);
		private delegate void glDrawElements(BeginMode mode, int count, DrawElementsType type, IntPtr indices);
		private delegate void glDrawElementsBaseVertex(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int basevertex);
		private delegate void glDrawElementsInstanced(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount);
		private delegate void glDrawElementsInstancedBaseVertex(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount, int basevertex);
		private delegate void glDrawRangeElements(BeginMode mode, uint start, uint end, int count, DrawElementsType type, IntPtr indices);
		private delegate void glDrawRangeElementsBaseVertex(BeginMode mode, uint start, uint end, int count, DrawElementsType type, IntPtr indices, int basevertex);
		private delegate void glEnable(EnableCap cap);
		private delegate void glDisable(EnableCap cap);
		private delegate void glEnablei(EnableCap cap, uint index);
		private delegate void glDisablei(EnableCap cap, uint index);
		private delegate void glEnableVertexAttribArray(uint index);
		private delegate void glDisableVertexAttribArray(uint index);
		private delegate IntPtr glFenceSync(IntPtr condition, ArbSync flags);
		private delegate void glFinish();
		private delegate void glFlush();
		private delegate IntPtr glFlushMappedBufferRange(BufferTarget target, IntPtr offset, IntPtr length);
		private delegate IntPtr glFramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbuffertarget, uint renderbuffer);
		private delegate void glFramebufferTexture(FramebufferTarget target, FramebufferAttachment attachment, uint texture, int level);
		private delegate void glFramebufferTexture1D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level);
		private delegate void glFramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level);
		private delegate void glFramebufferTexture3D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level, int layer);
		private delegate void glFramebufferTextureFace(Version32 target, Version32 attachment, uint texture, int level, Version32 face);
		private delegate void glFramebufferTextureLayer(FramebufferTarget target, FramebufferAttachment attachment, uint texture, int level, int layer);
		private delegate void glFrontFace(FrontFaceDirection mode);
		private unsafe delegate void glGenBuffers(int n, uint* buffers);
		private delegate void glGenerateMipmap(GenerateMipmapTarget target);
		private unsafe delegate void glGenFramebuffers(int n, uint* ids);
		private unsafe delegate void glGenQueries(int n, uint* ids);
		private unsafe delegate void glGenRenderbuffers(int n, uint* renderbuffers);
		private unsafe delegate void glGenSamplers(int n, uint* samplers);
		private unsafe delegate void glGenTextures(int n, uint* textures);
		private unsafe delegate void glGenVertexArrays(int n, uint* arrays);
		private delegate void glGetboolv(All pname, IntPtr parameters);
		private delegate void glGetDoublev(All pname, double[] parameters);
		private delegate void glGetFloatv(All pname, float[] parameters);
		private delegate void glGetIntegerv(All pname, int[] parameters);
		private delegate void glGetInteger64v(All pname, long[] parameters);
		private delegate void glGetbooli_v(All pname, uint index, IntPtr data);
		private delegate void glGetIntegeri_v(All pname, uint index, int[] data);
		private delegate void glGetInteger64i_v(All pname, uint index, long[] data);
		private unsafe delegate void glGetActiveAttrib(uint program, uint index, int bufSize, int* length, int[] size, IntPtr type, StringBuilder name);
		private unsafe delegate void glGetActiveUniform(uint program, uint index, int bufSize, int* length, int[] size, IntPtr type, string name);
		private delegate void glGetActiveUniformBlockiv(uint program, uint uniformBlockIndex, ActiveUniformBlockParameter pname, int parameters);
		private unsafe delegate void glGetActiveUniformBlockName(uint program, uint uniformBlockIndex, int bufSize, int* length, string uniformBlockName);
		private unsafe delegate void glGetActiveUniformName(uint program, uint uniformIndex, int bufSize, int* length, StringBuilder uniformName);
		private unsafe delegate void glGetActiveUniformsiv(uint program, int uniformCount, uint* uniformIndices, ActiveUniformParameter pname, int[] parameters);
		private unsafe delegate void glGetAttachedShaders(uint program, int maxCount, int* count, uint* shaders);
		private delegate int glGetAttribLocation(uint program, string name);
		private delegate void glGetBufferParameteriv(BufferTarget target, BufferParameterName value, int[] data);
		private delegate void glGetBufferPointerv(BufferTarget target, BufferPointer pname, IntPtr parameters);
		private delegate void glGetBufferSubData(BufferTarget target, IntPtr offset, IntPtr size, IntPtr data);
		private delegate void glGetCompressedTexImage(TextureTarget target, int lod, IntPtr img);
		private delegate ErrorCode glGetError();
		private delegate int glGetFragDataIndex(uint program, string name);
		private delegate int glGetFragDataLocation(uint program, string name);
		private delegate void glGetFramebufferAttachmentParameter(FramebufferTarget target, FramebufferAttachment attachment, FramebufferParameterName pname, int[] parameters);
		private delegate void glGetMultisamplefv(GetMultisamplePName pname, uint index, float[] val);
		private delegate void glGetProgramiv(uint program, ProgramParameter pname, int[] parameters);
		private unsafe delegate void glGetProgramInfoLog(uint program, int maxLength, int* length, string infoLog);
		private delegate void glGetQueryiv(QueryTarget target, GetQueryParam pname, int[] parameters);
		private delegate void glGetQueryObjectiv(uint id, GetQueryObjectParam pname, int[] parameters);
		private unsafe delegate void glGetQueryObjectuiv(uint id, GetQueryObjectParam pname, uint* parameters);
		private delegate void glGetQueryObjecti64v(uint id, GetQueryObjectParam pname, long[] parameters);
		private delegate void glGetQueryObjectui64v(uint id, GetQueryObjectParam pname, ulong[] parameters);
		private delegate void glGetRenderbufferParameteriv(RenderbufferTarget target, RenderbufferParameterName pname, int[] parameters);
		private delegate void glGetSamplerParameterfv(uint sampler, All pname, float[] parameters);
		private delegate void glGetSamplerParameteriv(uint sampler, All pname, int[] parameters);
		private delegate void glGetShaderiv(uint shader, All pname, int[] parameters);
		private unsafe delegate void glGetShaderInfoLog(uint shader, int maxLength, int* length, StringBuilder infoLog);
		private unsafe delegate void glGetShaderSource(uint shader, int bufSize, int* length, string source);
		private unsafe delegate sbyte* glGetString(StringName name);
		private unsafe delegate sbyte* glGetStringi(StringName name, uint index);
		private unsafe delegate void glGetSynciv(IntPtr sync, ArbSync pname, int bufSize, int* length, int[] values);
		private delegate void glGetTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, IntPtr img);
		private delegate void glGetTexLevelParameterfv(TextureTarget target, int level, GetTextureParameter pname, float[] parameters);
		private delegate void glGetTexLevelParameteriv(TextureTarget target, int level, GetTextureParameter pname, int[] parameters);
		private delegate void glGetTexParameterfv(TextureTarget target, GetTextureParameter pname, float[] parameters);
		private delegate void glGetTexParameteriv(TextureTarget target, GetTextureParameter pname, int[] parameters);
		private unsafe delegate void glGetTransformFeedbackVarying(uint program, uint index, int bufSize, int* length, int size, IntPtr type, IntPtr name);
		private delegate void glGetUniformfv(uint program, int location, float[] parameters);
		private delegate void glGetUniformiv(uint program, int location, int[] parameters);
		private delegate uint glGetUniformBlockIndex(uint program, string uniformBlockName);
		private unsafe delegate uint glGetUniformIndices(uint program, int uniformCount, string[] uniformNames, uint* uniformIndices);
		private delegate int glGetUniformLocation(uint program, string name);
		private delegate void glGetVertexAttribdv(uint index, VertexAttribParameter pname, double[] parameters);
		private delegate void glGetVertexAttribfv(uint index, VertexAttribParameter pname, float[] parameters);
		private delegate void glGetVertexAttribiv(uint index, VertexAttribParameter pname, int[] parameters);
		private delegate void glGetVertexAttribIiv(uint index, VertexAttribParameter pname, int[] parameters);
		private unsafe delegate void glGetVertexAttribIuiv(uint index, VertexAttribParameter pname, uint* parameters);
		private delegate void glGetVertexAttribPointerv(uint index, VertexAttribPointerParameter pname, IntPtr pointer);
		private delegate void glHint(HintTarget target, HintMode mode);
		private delegate bool glIsBuffer(uint buffer);
		private delegate bool glIsEnabled(EnableCap cap);
		private delegate bool glIsEnabledi(EnableCap cap, uint index);
		private delegate bool glIsFramebuffer(uint framebuffer);
		private delegate bool glIsProgram(uint program);
		private delegate bool glIsQuery(uint id);
		private delegate bool glIsRenderbuffer(uint renderbuffer);
		private delegate bool glIsSampler(uint id);
		private delegate bool glIsShader(uint shader);
		private delegate bool glIsSync(IntPtr sync);
		private delegate bool glIsTexture(uint texture);
		private delegate bool glIsVertexArray(uint array);
		private delegate void glLineWidth(float width);
		private delegate void glLinkProgram(uint program);
		private delegate void glLogicOp(LogicOp opcode);
		private delegate IntPtr glMapBuffer(BufferTarget target, BufferAccess access);
		private delegate bool glUnmapBuffer(BufferTarget target);
		private delegate IntPtr glMapBufferRange(BufferTarget target, IntPtr offset, IntPtr length, BufferAccessMask access);
		private delegate void glMultiDrawArrays(BeginMode mode, int[] first, int count, int primcount);
		private delegate void glMultiDrawElements(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount);
		private delegate void glMultiDrawElementsBaseVertex(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount, int[] basevertex);
		private delegate void glPixelStoreference(All pname, float param);
		private delegate void glPixelStorei(PixelStoreParameter pname, int param);
		private delegate void glPointParameterf(PointParameterName pname, float param);
		private delegate void glPointParameteri(PointParameterName pname, int param);
		private delegate void glPointParameterfv(PointParameterName pname, float[] parameters);
		private delegate void glPointParameteriv(PointParameterName pname, int[] parameters);
		private delegate void glPointSize(float size);
		private delegate void glPolygonMode(MaterialFace face, PolygonMode mode);
		private delegate void glPolygonOffset(float factor, float units);
		private delegate void glPrimitiveRestartIndex(uint index);
		private delegate void glProvokingVertex(ProvokingVertexMode provokeMode);
		private delegate void glQueryCounter(uint id, All target);
		private delegate void glReadBuffer(ReadBufferMode mode);
		private delegate void glReadPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, IntPtr data);
		private delegate void glRenderbufferStorage(RenderbufferTarget target, RenderbufferStorage internalformat, int width, int height);
		private delegate void glRenderbufferStorageMultisample(RenderbufferTarget target, int samples, RenderbufferStorage internalformat, int width, int height);
		private delegate void glSampleCoverage(float value, bool invert);
		private delegate void glSampleMaski(uint maskNumber, All mask);
		private delegate void glSamplerParameterf(uint sampler, All pname, float param);
		private delegate void glSamplerParameteri(uint sampler, All pname, int param);
		private delegate void glSamplerParameterfv(uint sampler, All pname, float[] parameters);
		private delegate void glSamplerParameteriv(uint sampler, All pname, int[] parameters);
		private delegate void glScissor(int x, int y, int width, int height);
		private delegate void glShaderSource(uint shader, int count, string[] text, int[] length);
		private delegate void glStencilFunc(StencilFunction func, int reference, uint mask);
		private delegate void glStencilFuncSeparate(StencilFace face, StencilFunction func, int reference, uint mask);
		private delegate void glStencilMask(uint mask);
		private delegate void glStencilMaskSeparate(StencilFace face, uint mask);
		private delegate void glStencilOp(StencilOp sfail, StencilOp dpfail, StencilOp dppass);
		private delegate void glStencilOpSeparate(StencilFace face, StencilOp sfail, StencilOp dpfail, StencilOp dppass);
		private delegate void glTexBuffer(TextureBufferTarget target, SizedInternalFormat internalFormat, uint buffer);
		private delegate void glTexImage1D(TextureTarget target, int level, int internalFormat, int width, int border, PixelInternalFormat format, PixelType type, IntPtr data);
		private delegate void glTexImage2D(TextureTarget target, int level, int internalFormat, int width, int height, int border, PixelInternalFormat format, PixelType type, IntPtr data);
		private delegate void glTexImage2DMultisample(TextureTargetMultisample target, int samples, int internalformat, int width, int height, bool fixedsamplelocations);
		private delegate void glTexImage3D(TextureTarget target, int level, int internalFormat, int width, int height, int depth, int border, PixelInternalFormat format, PixelType type, IntPtr data);
		private delegate void glTexImage3DMultisample(TextureTargetMultisample target, int samples, int internalformat, int width, int height, int depth, bool fixedsamplelocations);
		private delegate void glTexParameterf(TextureTarget target, TextureParameterName pname, float param);
		private delegate void glTexParameteri(TextureTarget target, TextureParameterName pname, int param);
		private delegate void glTexParameterfv(TextureTarget target, TextureParameterName pname, float[] parameters);
		private delegate void glTexParameteriv(TextureTarget target, TextureParameterName pname, int[] parameters);
		private delegate void glTexParameterIiv(TextureTarget target, TextureParameterName pname, int[] parameters);
		private unsafe delegate void glTexParameterIuiv(TextureTarget target, TextureParameterName pname, uint* parameters);
		private delegate void glTexSubImage1D(TextureTarget target, int level, int xoffset, int width, PixelFormat format, PixelType type, IntPtr data);
		private delegate void glTexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, IntPtr data);
		private delegate void glTexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, PixelType type, IntPtr data);
		private delegate void glTransformFeedbackVaryings(uint program, int count, string[] varyings, ExtTransformFeedback bufferMode);
		private delegate void glUniform1f(int location, float v0);
		private delegate void glUniform2f(int location, float v0, float v1);
		private delegate void glUniform3f(int location, float v0, float v1, float v2);
		private delegate void glUniform4f(int location, float v0, float v1, float v2, float v3);
		private delegate void glUniform1i(int location, int v0);
		private delegate void glUniform2i(int location, int v0, int v1);
		private delegate void glUniform3i(int location, int v0, int v1, int v2);
		private delegate void glUniform4i(int location, int v0, int v1, int v2, int v3);
		private delegate void glUniform1ui(int location, uint v0);
		private delegate void glUniform2ui(int location, int v0, uint v1);
		private delegate void glUniform3ui(int location, int v0, int v1, uint v2);
		private delegate void glUniform4ui(int location, int v0, int v1, int v2, uint v3);
		private delegate void glUniform1fv(int location, int count, float[] value);
		private delegate void glUniform2fv(int location, int count, float[] value);
		private delegate void glUniform3fv(int location, int count, float[] value);
		private delegate void glUniform4fv(int location, int count, float[] value);
		private delegate void glUniform1iv(int location, int count, int[] value);
		private delegate void glUniform2iv(int location, int count, int[] value);
		private delegate void glUniform3iv(int location, int count, int[] value);
		private delegate void glUniform4iv(int location, int count, int[] value);
		private unsafe delegate void glUniform1uiv(int location, int count, uint* value);
		private unsafe delegate void glUniform2uiv(int location, int count, uint* value);
		private unsafe delegate void glUniform3uiv(int location, int count, uint* value);
		private unsafe delegate void glUniform4uiv(int location, int count, uint* value);
		private delegate void glUniformMatrix2fv(int location, int count, bool transpose, float[] value);
		private delegate void glUniformMatrix3fv(int location, int count, bool transpose, float[] value);
		private delegate void glUniformMatrix4fv(int location, int count, bool transpose, float[] value);
		private delegate void glUniformMatrix2x3fv(int location, int count, bool transpose, float[] value);
		private delegate void glUniformMatrix3x2fv(int location, int count, bool transpose, float[] value);
		private delegate void glUniformMatrix2x4fv(int location, int count, bool transpose, float[] value);
		private delegate void glUniformMatrix4x2fv(int location, int count, bool transpose, float[] value);
		private delegate void glUniformMatrix3x4fv(int location, int count, bool transpose, float[] value);
		private delegate void glUniformMatrix4x3fv(int location, int count, bool transpose, float[] value);
		private delegate void glUniformBlockBinding(uint program, uint uniformBlockIndex, uint uniformBlockBinding);
		private delegate void glUseProgram(uint program);
		private delegate void glValidateProgram(uint program);
		private delegate void glVertexAttrib1f(uint index, float v0);
		private delegate void glVertexAttrib1s(uint index, short v0);
		private delegate void glVertexAttrib1d(uint index, double v0);
		private delegate void glVertexAttribI1i(uint index, int v0);
		private delegate void glVertexAttribI1ui(uint index, uint v0);
		private delegate void glVertexAttrib2f(uint index, float v0, float v1);
		private delegate void glVertexAttrib2s(uint index, short v0, short v1);
		private delegate void glVertexAttrib2d(uint index, double v0, double v1);
		private delegate void glVertexAttribI2i(uint index, int v0, int v1);
		private delegate void glVertexAttribI2ui(uint index, uint v0, uint v1);
		private delegate void glVertexAttrib3f(uint index, float v0, float v1, float v2);
		private delegate void glVertexAttrib3s(uint index, short v0, short v1, short v2);
		private delegate void glVertexAttrib3d(uint index, double v0, double v1, double v2);
		private delegate void glVertexAttribI3i(uint index, int v0, int v1, int v2);
		private delegate void glVertexAttribI3ui(uint index, uint v0, uint v1, uint v2);
		private delegate void glVertexAttrib4f(uint index, float v0, float v1, float v2, float v3);
		private delegate void glVertexAttrib4s(uint index, short v0, short v1, short v2, short v3);
		private delegate void glVertexAttrib4d(uint index, double v0, double v1, double v2, double v3);
		private delegate void glVertexAttrib4Nub(uint index, byte v0, byte v1, byte v2, byte v3);
		private delegate void glVertexAttribI4i(uint index, int v0, int v1, int v2, int v3);
		private delegate void glVertexAttribI4ui(uint index, uint v0, uint v1, uint v2, uint v3);
		private delegate void glVertexAttrib1fv(uint index, float[] v);
		private delegate void glVertexAttrib1sv(uint index, short[] v);
		private delegate void glVertexAttrib1dv(uint index, double[] v);
		private delegate void glVertexAttribI1iv(uint index, int[] v);
		private unsafe delegate void glVertexAttribI1uiv(uint index, uint* v);
		private delegate void glVertexAttrib2fv(uint index, float[] v);
		private delegate void glVertexAttrib2sv(uint index, short[] v);
		private delegate void glVertexAttrib2dv(uint index, double[] v);
		private delegate void glVertexAttribI2iv(uint index, int[] v);
		private unsafe delegate void glVertexAttribI2uiv(uint index, uint* v);
		private delegate void glVertexAttrib3fv(uint index, float[] v);
		private delegate void glVertexAttrib3sv(uint index, short[] v);
		private delegate void glVertexAttrib3dv(uint index, double[] v);
		private delegate void glVertexAttribI3iv(uint index, int[] v);
		private unsafe delegate void glVertexAttribI3uiv(uint index, uint* v);
		private delegate void glVertexAttrib4fv(uint index, float[] v);
		private delegate void glVertexAttrib4sv(uint index, short[] v);
		private delegate void glVertexAttrib4dv(uint index, double[] v);
		private delegate void glVertexAttrib4iv(uint index, int[] v);
		private delegate void glVertexAttrib4bv(uint index, byte[] v);
		private delegate void glVertexAttrib4ubv(uint index, sbyte[] v);
		private delegate void glVertexAttrib4usv(uint index, ushort[] v);
		private unsafe delegate void glVertexAttrib4uiv(uint index, uint* v);
		private delegate void glVertexAttrib4Nbv(uint index, byte[] v);
		private delegate void glVertexAttrib4Nsv(uint index, short[] v);
		private delegate void glVertexAttrib4Niv(uint index, int[] v);
		private delegate void glVertexAttrib4Nubv(uint index, sbyte[] v);
		private delegate void glVertexAttrib4Nusv(uint index, ushort[] v);
		private unsafe delegate void glVertexAttrib4Nuiv(uint index, uint* v);
		private delegate void glVertexAttribI4bv(uint index, byte[] v);
		private delegate void glVertexAttribI4ubv(uint index, sbyte[] v);
		private delegate void glVertexAttribI4sv(uint index, short[] v);
		private delegate void glVertexAttribI4usv(uint index, ushort[] v);
		private delegate void glVertexAttribI4iv(uint index, int[] v);
		private unsafe delegate void glVertexAttribI4uiv(uint index, uint* v);
		private delegate void glVertexAttribP1ui(uint index, All type, bool normalized, uint value);
		private delegate void glVertexAttribP2ui(uint index, All type, bool normalized, uint value);
		private delegate void glVertexAttribP3ui(uint index, All type, bool normalized, uint value);
		private delegate void glVertexAttribP4ui(uint index, All type, bool normalized, uint value);
		private delegate void glVertexAttribDivisor(uint index, uint divisor);
		private delegate void glVertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalized, int stride, IntPtr pointer);
		private delegate void glVertexAttribIPointer(uint index, int size, VertexAttribIPointerType type, int stride, IntPtr pointer);
		private delegate void glViewport(int x, int y, int width, int height);
		private delegate void glWaitSync(IntPtr sync, All flags, ulong timeout);

		public static void ActiveTexture(TextureUnit texture)
		{
			glActiveTexture deleg = BaseGraphicsContext.Current.Loader.Get<glActiveTexture>();
			if (deleg != null)
				deleg(texture);
		}

		public static void AttachShader(uint program, uint shader)
		{
			glAttachShader deleg = BaseGraphicsContext.Current.Loader.Get<glAttachShader>();
			if (deleg != null)
				deleg(program, shader);
		}

		public static void BeginConditionalRender(uint id, ConditionalRenderType mode)
		{
			glBeginConditionalRender deleg = BaseGraphicsContext.Current.Loader.Get<glBeginConditionalRender>();
			if (deleg != null)
				deleg(id, mode);
		}

		public static void EndConditionalRender()
		{
			glEndConditionalRender deleg = BaseGraphicsContext.Current.Loader.Get<glEndConditionalRender>();
			if (deleg != null)
				deleg();
		}

		public static void BeginQuery(QueryTarget target, uint id)
		{
			glBeginQuery deleg = BaseGraphicsContext.Current.Loader.Get<glBeginQuery>();
			if (deleg != null)
				deleg(target, id);
		}

		public static void EndQuery(QueryTarget target)
		{
			glEndQuery deleg = BaseGraphicsContext.Current.Loader.Get<glEndQuery>();
			if (deleg != null)
				deleg(target);
		}

		public static void BeginTransformFeedback(ExtTransformFeedback primitiveMode)
		{
			glBeginTransformFeedback deleg = BaseGraphicsContext.Current.Loader.Get<glBeginTransformFeedback>();
			if (deleg != null)
				deleg(primitiveMode);
		}

		public static void EndTransformFeedback()
		{
			glEndTransformFeedback deleg = BaseGraphicsContext.Current.Loader.Get<glEndTransformFeedback>();
			if (deleg != null)
				deleg();
		}

		public static void BindAttribLocation(uint program, uint index, string name)
		{
			glBindAttribLocation deleg = BaseGraphicsContext.Current.Loader.Get<glBindAttribLocation>();
			if (deleg != null)
				deleg(program, index, name);
		}

		public static void BindBuffer(BufferTarget target, uint buffer)
		{
			glBindBuffer deleg = BaseGraphicsContext.Current.Loader.Get<glBindBuffer>();
			if (deleg != null)
				deleg(target, buffer);
		}

		public static void BindBufferBase(ExtTransformFeedback target, uint index, uint buffer)
		{
			glBindBufferBase deleg = BaseGraphicsContext.Current.Loader.Get<glBindBufferBase>();
			if (deleg != null)
				deleg(target, index, buffer);
		}

		public static void BindBufferRange(ExtTransformFeedback target, uint index, uint buffer, IntPtr offset, IntPtr size)
		{
			glBindBufferRange deleg = BaseGraphicsContext.Current.Loader.Get<glBindBufferRange>();
			if (deleg != null)
				deleg(target, index, buffer, offset, size);
		}

		public static void BindFragDataLocation(uint program, uint colorNumber, string name)
		{
			glBindFragDataLocation deleg = BaseGraphicsContext.Current.Loader.Get<glBindFragDataLocation>();
			if (deleg != null)
				deleg(program, colorNumber, name);
		}

		public static void BindFragDataLocationIndexed(uint program, uint colorNumber, uint index, string name)
		{
			glBindFragDataLocationIndexed deleg = BaseGraphicsContext.Current.Loader.Get<glBindFragDataLocationIndexed>();
			if (deleg != null)
				deleg(program, colorNumber, index, name);
		}

		public static void BindFramebuffer(FramebufferTarget target, uint framebuffer)
		{
			glBindFramebuffer deleg = BaseGraphicsContext.Current.Loader.Get<glBindFramebuffer>();
			if (deleg != null)
				deleg(target, framebuffer);
		}

		public static void BindRenderbuffer(RenderbufferTarget target, uint renderbuffer)
		{
			glBindRenderbuffer deleg = BaseGraphicsContext.Current.Loader.Get<glBindRenderbuffer>();
			if (deleg != null)
				deleg(target, renderbuffer);
		}

		public static void BindSampler(uint unit, uint sampler)
		{
			glBindSampler deleg = BaseGraphicsContext.Current.Loader.Get<glBindSampler>();
			if (deleg != null)
				deleg(unit, sampler);
		}

		public static void BindTexture(TextureTarget target, uint texture)
		{
			glBindTexture deleg = BaseGraphicsContext.Current.Loader.Get<glBindTexture>();
			if (deleg != null)
				deleg(target, texture);
		}

		public static void BindVertexArray(uint array)
		{
			glBindVertexArray deleg = BaseGraphicsContext.Current.Loader.Get<glBindVertexArray>();
			if (deleg != null)
				deleg(array);
		}

		public static void BlendColor(float red, float green, float blue, float alpha)
		{
			glBlendColor deleg = BaseGraphicsContext.Current.Loader.Get<glBlendColor>();
			if (deleg != null)
				deleg(red, green, blue, alpha);
		}

		public static void BlendEquation(BlendEquationMode mode)
		{
			glBlendEquation deleg = BaseGraphicsContext.Current.Loader.Get<glBlendEquation>();
			if (deleg != null)
				deleg(mode);
		}

		public static void BlendEquationSeparate(BlendEquationMode modeRGB, BlendEquationMode modeAlpha)
		{
			glBlendEquationSeparate deleg = BaseGraphicsContext.Current.Loader.Get<glBlendEquationSeparate>();
			if (deleg != null)
				deleg(modeRGB, modeAlpha);
		}

		public static void BlendFunc(BlendingFactorSrc sfactor, BlendingFactorDest dfactor)
		{
			glBlendFunc deleg = BaseGraphicsContext.Current.Loader.Get<glBlendFunc>();
			if (deleg != null)
				deleg(sfactor, dfactor);
		}

		public static void BlendFuncSeparate(BlendingFactorSrc srcRGB, BlendingFactorDest dstRGB, BlendingFactorSrc srcAlpha, BlendingFactorDest dstAlpha)
		{
			glBlendFuncSeparate deleg = BaseGraphicsContext.Current.Loader.Get<glBlendFuncSeparate>();
			if (deleg != null)
				deleg(srcRGB, dstRGB, srcAlpha, dstAlpha);
		}

		public static void BlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, ClearBufferMask mask, BlitFramebufferFilter filter)
		{
			glBlitFramebuffer deleg = BaseGraphicsContext.Current.Loader.Get<glBlitFramebuffer>();
			if (deleg != null)
				deleg(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask, filter);
		}

		public static void BufferData(BufferTarget target, IntPtr size, IntPtr data, BufferUsageHint usage)
		{
			glBufferData deleg = BaseGraphicsContext.Current.Loader.Get<glBufferData>();
			if (deleg != null)
				deleg(target, size, data, usage);
		}

		public static void BufferSubData(BufferTarget target, IntPtr offset, IntPtr size, IntPtr data)
		{
			glBufferSubData deleg = BaseGraphicsContext.Current.Loader.Get<glBufferSubData>();
			if (deleg != null)
				deleg(target, offset, size, data);
		}

		public static FramebufferErrorCode CheckFramebufferStatus(FramebufferTarget target)
		{
			glCheckFramebufferStatus deleg = BaseGraphicsContext.Current.Loader.Get<glCheckFramebufferStatus>();
			if (deleg != null)
				return deleg(target);
			return default(FramebufferErrorCode);
		}

		public static void ClampColor(ArbColorBufferFloat target, ArbColorBufferFloat clamp)
		{
			glClampColor deleg = BaseGraphicsContext.Current.Loader.Get<glClampColor>();
			if (deleg != null)
				deleg(target, clamp);
		}

		public static void Clear(ClearBufferMask mask)
		{
			glClear deleg = BaseGraphicsContext.Current.Loader.Get<glClear>();
			if (deleg != null)
				deleg(mask);
		}

		public static void ClearBufferiv(ClearBuffer buffer, int drawBuffer, int[] value)
		{
			glClearBufferiv deleg = BaseGraphicsContext.Current.Loader.Get<glClearBufferiv>();
			if (deleg != null)
				deleg(buffer, drawBuffer, value);
		}

		public static unsafe void ClearBufferuiv(ClearBuffer buffer, int drawBuffer, uint* value)
		{
			glClearBufferuiv deleg = BaseGraphicsContext.Current.Loader.Get<glClearBufferuiv>();
			if (deleg != null)
				deleg(buffer, drawBuffer, value);
		}

		public static void ClearBufferfv(ClearBuffer buffer, int drawBuffer, float[] value)
		{
			glClearBufferfv deleg = BaseGraphicsContext.Current.Loader.Get<glClearBufferfv>();
			if (deleg != null)
				deleg(buffer, drawBuffer, value);
		}

		public static void ClearBufferfi(ClearBuffer buffer, int drawBuffer, float depth, int stencil)
		{
			glClearBufferfi deleg = BaseGraphicsContext.Current.Loader.Get<glClearBufferfi>();
			if (deleg != null)
				deleg(buffer, drawBuffer, depth, stencil);
		}

		public static void ClearColor(float red, float green, float blue, float alpha)
		{
			glClearColor deleg = BaseGraphicsContext.Current.Loader.Get<glClearColor>();
			if (deleg != null)
				deleg(red, green, blue, alpha);
		}

		public static void ClearDepth(double depth)
		{
			glClearDepth deleg = BaseGraphicsContext.Current.Loader.Get<glClearDepth>();
			if (deleg != null)
				deleg(depth);
		}

		public static void ClearStencil(int s)
		{
			glClearStencil deleg = BaseGraphicsContext.Current.Loader.Get<glClearStencil>();
			if (deleg != null)
				deleg(s);
		}

		public static All ClientWaitSync(IntPtr sync, All flags, ulong timeout)
		{
			glClientWaitSync deleg = BaseGraphicsContext.Current.Loader.Get<glClientWaitSync>();
			if (deleg != null)
				return deleg(sync, flags, timeout);
			return default(All);
		}

		public static void ColorMask(bool red, bool green, bool blue, bool alpha)
		{
			glColorMask deleg = BaseGraphicsContext.Current.Loader.Get<glColorMask>();
			if (deleg != null)
				deleg(red, green, blue, alpha);
		}

		public static void ColorMaski(uint buf, bool red, bool green, bool blue, bool alpha)
		{
			glColorMaski deleg = BaseGraphicsContext.Current.Loader.Get<glColorMaski>();
			if (deleg != null)
				deleg(buf, red, green, blue, alpha);
		}

		public static void CompileShader(uint shader)
		{
			glCompileShader deleg = BaseGraphicsContext.Current.Loader.Get<glCompileShader>();
			if (deleg != null)
				deleg(shader);
		}

		public static void CompressedTexImage1D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int border, int imageSize, IntPtr data)
		{
			glCompressedTexImage1D deleg = BaseGraphicsContext.Current.Loader.Get<glCompressedTexImage1D>();
			if (deleg != null)
				deleg(target, level, internalformat, width, border, imageSize, data);
		}

		public static void CompressedTexImage2D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int border, int imageSize, IntPtr data)
		{
			glCompressedTexImage2D deleg = BaseGraphicsContext.Current.Loader.Get<glCompressedTexImage2D>();
			if (deleg != null)
				deleg(target, level, internalformat, width, height, border, imageSize, data);
		}

		public static void CompressedTexImage3D(TextureTarget target, int level, PixelInternalFormat internalformat, int width, int height, int depth, int border, int imageSize, IntPtr data)
		{
			glCompressedTexImage3D deleg = BaseGraphicsContext.Current.Loader.Get<glCompressedTexImage3D>();
			if (deleg != null)
				deleg(target, level, internalformat, width, height, depth, border, imageSize, data);
		}

		public static void CompressedTexSubImage1D(TextureTarget target, int level, int xoffset, int width, PixelFormat format, int imageSize, IntPtr data)
		{
			glCompressedTexSubImage1D deleg = BaseGraphicsContext.Current.Loader.Get<glCompressedTexSubImage1D>();
			if (deleg != null)
				deleg(target, level, xoffset, width, format, imageSize, data);
		}

		public static void CompressedTexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, int imageSize, IntPtr data)
		{
			glCompressedTexSubImage2D deleg = BaseGraphicsContext.Current.Loader.Get<glCompressedTexSubImage2D>();
			if (deleg != null)
				deleg(target, level, xoffset, yoffset, width, height, format, imageSize, data);
		}

		public static void CompressedTexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, int imageSize, IntPtr data)
		{
			glCompressedTexSubImage3D deleg = BaseGraphicsContext.Current.Loader.Get<glCompressedTexSubImage3D>();
			if (deleg != null)
				deleg(target, level, xoffset, yoffset, zoffset, width, height, depth, format, imageSize, data);
		}

		public static void CopyBufferSubData(BufferTarget readtarget, BufferTarget writetarget, IntPtr readoffset, IntPtr writeoffset, IntPtr size)
		{
			glCopyBufferSubData deleg = BaseGraphicsContext.Current.Loader.Get<glCopyBufferSubData>();
			if (deleg != null)
				deleg(readtarget, writetarget, readoffset, writeoffset, size);
		}

		public static void CopyTexImage1D(TextureTarget target, int level, PixelInternalFormat internalformat, int x, int y, int width, int border)
		{
			glCopyTexImage1D deleg = BaseGraphicsContext.Current.Loader.Get<glCopyTexImage1D>();
			if (deleg != null)
				deleg(target, level, internalformat, x, y, width, border);
		}

		public static void CopyTexImage2D(TextureTarget target, int level, PixelInternalFormat internalformat, int x, int y, int width, int height, int border)
		{
			glCopyTexImage2D deleg = BaseGraphicsContext.Current.Loader.Get<glCopyTexImage2D>();
			if (deleg != null)
				deleg(target, level, internalformat, x, y, width, height, border);
		}

		public static void CopyTexSubImage1D(TextureTarget target, int level, int xoffset, int x, int y, int width)
		{
			glCopyTexSubImage1D deleg = BaseGraphicsContext.Current.Loader.Get<glCopyTexSubImage1D>();
			if (deleg != null)
				deleg(target, level, xoffset, x, y, width);
		}

		public static void CopyTexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int x, int y, int width, int height)
		{
			glCopyTexSubImage2D deleg = BaseGraphicsContext.Current.Loader.Get<glCopyTexSubImage2D>();
			if (deleg != null)
				deleg(target, level, xoffset, yoffset, x, y, width, height);
		}

		public static void CopyTexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width, int height)
		{
			glCopyTexSubImage3D deleg = BaseGraphicsContext.Current.Loader.Get<glCopyTexSubImage3D>();
			if (deleg != null)
				deleg(target, level, xoffset, yoffset, zoffset, x, y, width, height);
		}

		public static uint CreateProgram()
		{
			glCreateProgram deleg = BaseGraphicsContext.Current.Loader.Get<glCreateProgram>();
			if (deleg != null)
				return deleg();
			return default(uint);
		}

		public static uint CreateShader(ShaderType shaderType)
		{
			glCreateShader deleg = BaseGraphicsContext.Current.Loader.Get<glCreateShader>();
			if (deleg != null)
				return deleg(shaderType);
			return default(uint);
		}

		public static void CullFace(CullFaceMode mode)
		{
			glCullFace deleg = BaseGraphicsContext.Current.Loader.Get<glCullFace>();
			if (deleg != null)
				deleg(mode);
		}

		public static unsafe void DeleteBuffers(int n, uint* buffers)
		{
			glDeleteBuffers deleg = BaseGraphicsContext.Current.Loader.Get<glDeleteBuffers>();
			if (deleg != null)
				deleg(n, buffers);
		}

		public static unsafe void DeleteFramebuffers(int n, uint* framebuffers)
		{
			glDeleteFramebuffers deleg = BaseGraphicsContext.Current.Loader.Get<glDeleteFramebuffers>();
			if (deleg != null)
				deleg(n, framebuffers);
		}

		public static void DeleteProgram(uint program)
		{
			glDeleteProgram deleg = BaseGraphicsContext.Current.Loader.Get<glDeleteProgram>();
			if (deleg != null)
				deleg(program);
		}

		public static unsafe void DeleteQueries(int n, uint* ids)
		{
			glDeleteQueries deleg = BaseGraphicsContext.Current.Loader.Get<glDeleteQueries>();
			if (deleg != null)
				deleg(n, ids);
		}

		public static unsafe void DeleteRenderbuffers(int n, uint* renderbuffers)
		{
			glDeleteRenderbuffers deleg = BaseGraphicsContext.Current.Loader.Get<glDeleteRenderbuffers>();
			if (deleg != null)
				deleg(n, renderbuffers);
		}

		public static unsafe void DeleteSamplers(int n, uint* ids)
		{
			glDeleteSamplers deleg = BaseGraphicsContext.Current.Loader.Get<glDeleteSamplers>();
			if (deleg != null)
				deleg(n, ids);
		}

		public static void DeleteShader(uint shader)
		{
			glDeleteShader deleg = BaseGraphicsContext.Current.Loader.Get<glDeleteShader>();
			if (deleg != null)
				deleg(shader);
		}

		public static void DeleteSync(IntPtr sync)
		{
			glDeleteSync deleg = BaseGraphicsContext.Current.Loader.Get<glDeleteSync>();
			if (deleg != null)
				deleg(sync);
		}

		public static unsafe void DeleteTextures(int n, uint* textures)
		{
			glDeleteTextures deleg = BaseGraphicsContext.Current.Loader.Get<glDeleteTextures>();
			if (deleg != null)
				deleg(n, textures);
		}

		public static unsafe void DeleteVertexArrays(int n, uint* arrays)
		{
			glDeleteVertexArrays deleg = BaseGraphicsContext.Current.Loader.Get<glDeleteVertexArrays>();
			if (deleg != null)
				deleg(n, arrays);
		}

		public static void DepthFunc(DepthFunction func)
		{
			glDepthFunc deleg = BaseGraphicsContext.Current.Loader.Get<glDepthFunc>();
			if (deleg != null)
				deleg(func);
		}

		public static void DepthMask(bool flag)
		{
			glDepthMask deleg = BaseGraphicsContext.Current.Loader.Get<glDepthMask>();
			if (deleg != null)
				deleg(flag);
		}

		public static void DepthRange(double nearVal, double farVal)
		{
			glDepthRange deleg = BaseGraphicsContext.Current.Loader.Get<glDepthRange>();
			if (deleg != null)
				deleg(nearVal, farVal);
		}

		public static void DetachShader(uint program, uint shader)
		{
			glDetachShader deleg = BaseGraphicsContext.Current.Loader.Get<glDetachShader>();
			if (deleg != null)
				deleg(program, shader);
		}

		public static void DrawArrays(BeginMode mode, int first, int count)
		{
			glDrawArrays deleg = BaseGraphicsContext.Current.Loader.Get<glDrawArrays>();
			if (deleg != null)
				deleg(mode, first, count);
		}

		public static void DrawArraysInstanced(BeginMode mode, int first, int count, int primcount)
		{
			glDrawArraysInstanced deleg = BaseGraphicsContext.Current.Loader.Get<glDrawArraysInstanced>();
			if (deleg != null)
				deleg(mode, first, count, primcount);
		}

		public static void DrawBuffer(DrawBufferMode mode)
		{
			glDrawBuffer deleg = BaseGraphicsContext.Current.Loader.Get<glDrawBuffer>();
			if (deleg != null)
				deleg(mode);
		}

		public static void DrawBuffers(int n, IntPtr bufs)
		{
			glDrawBuffers deleg = BaseGraphicsContext.Current.Loader.Get<glDrawBuffers>();
			if (deleg != null)
				deleg(n, bufs);
		}

		public static void DrawElements(BeginMode mode, int count, DrawElementsType type, IntPtr indices)
		{
			glDrawElements deleg = BaseGraphicsContext.Current.Loader.Get<glDrawElements>();
			if (deleg != null)
				deleg(mode, count, type, indices);
		}

		public static void DrawElementsBaseVertex(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int basevertex)
		{
			glDrawElementsBaseVertex deleg = BaseGraphicsContext.Current.Loader.Get<glDrawElementsBaseVertex>();
			if (deleg != null)
				deleg(mode, count, type, indices, basevertex);
		}

		public static void DrawElementsInstanced(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount)
		{
			glDrawElementsInstanced deleg = BaseGraphicsContext.Current.Loader.Get<glDrawElementsInstanced>();
			if (deleg != null)
				deleg(mode, count, type, indices, primcount);
		}

		public static void DrawElementsInstancedBaseVertex(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount, int basevertex)
		{
			glDrawElementsInstancedBaseVertex deleg = BaseGraphicsContext.Current.Loader.Get<glDrawElementsInstancedBaseVertex>();
			if (deleg != null)
				deleg(mode, count, type, indices, primcount, basevertex);
		}

		public static void DrawRangeElements(BeginMode mode, uint start, uint end, int count, DrawElementsType type, IntPtr indices)
		{
			glDrawRangeElements deleg = BaseGraphicsContext.Current.Loader.Get<glDrawRangeElements>();
			if (deleg != null)
				deleg(mode, start, end, count, type, indices);
		}

		public static void DrawRangeElementsBaseVertex(BeginMode mode, uint start, uint end, int count, DrawElementsType type, IntPtr indices, int basevertex)
		{
			glDrawRangeElementsBaseVertex deleg = BaseGraphicsContext.Current.Loader.Get<glDrawRangeElementsBaseVertex>();
			if (deleg != null)
				deleg(mode, start, end, count, type, indices, basevertex);
		}

		public static void Enable(EnableCap cap)
		{
			glEnable deleg = BaseGraphicsContext.Current.Loader.Get<glEnable>();
			if (deleg != null)
				deleg(cap);
		}

		public static void Disable(EnableCap cap)
		{
			glDisable deleg = BaseGraphicsContext.Current.Loader.Get<glDisable>();
			if (deleg != null)
				deleg(cap);
		}

		public static void Enablei(EnableCap cap, uint index)
		{
			glEnablei deleg = BaseGraphicsContext.Current.Loader.Get<glEnablei>();
			if (deleg != null)
				deleg(cap, index);
		}

		public static void Disablei(EnableCap cap, uint index)
		{
			glDisablei deleg = BaseGraphicsContext.Current.Loader.Get<glDisablei>();
			if (deleg != null)
				deleg(cap, index);
		}

		public static void EnableVertexAttribArray(uint index)
		{
			glEnableVertexAttribArray deleg = BaseGraphicsContext.Current.Loader.Get<glEnableVertexAttribArray>();
			if (deleg != null)
				deleg(index);
		}

		public static void DisableVertexAttribArray(uint index)
		{
			glDisableVertexAttribArray deleg = BaseGraphicsContext.Current.Loader.Get<glDisableVertexAttribArray>();
			if (deleg != null)
				deleg(index);
		}

		public static IntPtr FenceSync(IntPtr condition, ArbSync flags)
		{
			glFenceSync deleg = BaseGraphicsContext.Current.Loader.Get<glFenceSync>();
			if (deleg != null)
				return deleg(condition, flags);
			return default(IntPtr);
		}

		public static void Finish()
		{
			glFinish deleg = BaseGraphicsContext.Current.Loader.Get<glFinish>();
			if (deleg != null)
				deleg();
		}

		public static void Flush()
		{
			glFlush deleg = BaseGraphicsContext.Current.Loader.Get<glFlush>();
			if (deleg != null)
				deleg();
		}

		public static IntPtr FlushMappedBufferRange(BufferTarget target, IntPtr offset, IntPtr length)
		{
			glFlushMappedBufferRange deleg = BaseGraphicsContext.Current.Loader.Get<glFlushMappedBufferRange>();
			if (deleg != null)
				return deleg(target, offset, length);
			return default(IntPtr);
		}

		public static IntPtr FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbuffertarget, uint renderbuffer)
		{
			glFramebufferRenderbuffer deleg = BaseGraphicsContext.Current.Loader.Get<glFramebufferRenderbuffer>();
			if (deleg != null)
				return deleg(target, attachment, renderbuffertarget, renderbuffer);
			return default(IntPtr);
		}

		public static void FramebufferTexture(FramebufferTarget target, FramebufferAttachment attachment, uint texture, int level)
		{
			glFramebufferTexture deleg = BaseGraphicsContext.Current.Loader.Get<glFramebufferTexture>();
			if (deleg != null)
				deleg(target, attachment, texture, level);
		}

		public static void FramebufferTexture1D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level)
		{
			glFramebufferTexture1D deleg = BaseGraphicsContext.Current.Loader.Get<glFramebufferTexture1D>();
			if (deleg != null)
				deleg(target, attachment, textarget, texture, level);
		}

		public static void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level)
		{
			glFramebufferTexture2D deleg = BaseGraphicsContext.Current.Loader.Get<glFramebufferTexture2D>();
			if (deleg != null)
				deleg(target, attachment, textarget, texture, level);
		}

		public static void FramebufferTexture3D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, uint texture, int level, int layer)
		{
			glFramebufferTexture3D deleg = BaseGraphicsContext.Current.Loader.Get<glFramebufferTexture3D>();
			if (deleg != null)
				deleg(target, attachment, textarget, texture, level, layer);
		}

		public static void FramebufferTextureFace(Version32 target, Version32 attachment, uint texture, int level, Version32 face)
		{
			glFramebufferTextureFace deleg = BaseGraphicsContext.Current.Loader.Get<glFramebufferTextureFace>();
			if (deleg != null)
				deleg(target, attachment, texture, level, face);
		}

		public static void FramebufferTextureLayer(FramebufferTarget target, FramebufferAttachment attachment, uint texture, int level, int layer)
		{
			glFramebufferTextureLayer deleg = BaseGraphicsContext.Current.Loader.Get<glFramebufferTextureLayer>();
			if (deleg != null)
				deleg(target, attachment, texture, level, layer);
		}

		public static void FrontFace(FrontFaceDirection mode)
		{
			glFrontFace deleg = BaseGraphicsContext.Current.Loader.Get<glFrontFace>();
			if (deleg != null)
				deleg(mode);
		}

		public static unsafe void GenBuffers(int n, uint* buffers)
		{
			glGenBuffers deleg = BaseGraphicsContext.Current.Loader.Get<glGenBuffers>();
			if (deleg != null)
				deleg(n, buffers);
		}

		public static void GenerateMipmap(GenerateMipmapTarget target)
		{
			glGenerateMipmap deleg = BaseGraphicsContext.Current.Loader.Get<glGenerateMipmap>();
			if (deleg != null)
				deleg(target);
		}

		public static unsafe void GenFramebuffers(int n, uint* ids)
		{
			glGenFramebuffers deleg = BaseGraphicsContext.Current.Loader.Get<glGenFramebuffers>();
			if (deleg != null)
				deleg(n, ids);
		}

		public static unsafe void GenQueries(int n, uint* ids)
		{
			glGenQueries deleg = BaseGraphicsContext.Current.Loader.Get<glGenQueries>();
			if (deleg != null)
				deleg(n, ids);
		}

		public static unsafe void GenRenderbuffers(int n, uint* renderbuffers)
		{
			glGenRenderbuffers deleg = BaseGraphicsContext.Current.Loader.Get<glGenRenderbuffers>();
			if (deleg != null)
				deleg(n, renderbuffers);
		}

		public static unsafe void GenSamplers(int n, uint* samplers)
		{
			glGenSamplers deleg = BaseGraphicsContext.Current.Loader.Get<glGenSamplers>();
			if (deleg != null)
				deleg(n, samplers);
		}

		public static unsafe void GenTextures(int n, uint* textures)
		{
			glGenTextures deleg = BaseGraphicsContext.Current.Loader.Get<glGenTextures>();
			if (deleg != null)
				deleg(n, textures);
		}

		public static unsafe void GenVertexArrays(int n, uint* arrays)
		{
			glGenVertexArrays deleg = BaseGraphicsContext.Current.Loader.Get<glGenVertexArrays>();
			if (deleg != null)
				deleg(n, arrays);
		}

		public static void Getboolv(All pname, IntPtr parameters)
		{
			glGetboolv deleg = BaseGraphicsContext.Current.Loader.Get<glGetboolv>();
			if (deleg != null)
				deleg(pname, parameters);
		}

		public static void GetDoublev(All pname, double[] parameters)
		{
			glGetDoublev deleg = BaseGraphicsContext.Current.Loader.Get<glGetDoublev>();
			if (deleg != null)
				deleg(pname, parameters);
		}

		public static void GetFloatv(All pname, float[] parameters)
		{
			glGetFloatv deleg = BaseGraphicsContext.Current.Loader.Get<glGetFloatv>();
			if (deleg != null)
				deleg(pname, parameters);
		}

		public static void GetIntegerv(All pname, int[] parameters)
		{
			glGetIntegerv deleg = BaseGraphicsContext.Current.Loader.Get<glGetIntegerv>();
			if (deleg != null)
				deleg(pname, parameters);
		}

		public static void GetInteger64v(All pname, long[] parameters)
		{
			glGetInteger64v deleg = BaseGraphicsContext.Current.Loader.Get<glGetInteger64v>();
			if (deleg != null)
				deleg(pname, parameters);
		}

		public static void Getbooli_v(All pname, uint index, IntPtr data)
		{
			glGetbooli_v deleg = BaseGraphicsContext.Current.Loader.Get<glGetbooli_v>();
			if (deleg != null)
				deleg(pname, index, data);
		}

		public static void GetIntegeri_v(All pname, uint index, int[] data)
		{
			glGetIntegeri_v deleg = BaseGraphicsContext.Current.Loader.Get<glGetIntegeri_v>();
			if (deleg != null)
				deleg(pname, index, data);
		}

		public static void GetInteger64i_v(All pname, uint index, long[] data)
		{
			glGetInteger64i_v deleg = BaseGraphicsContext.Current.Loader.Get<glGetInteger64i_v>();
			if (deleg != null)
				deleg(pname, index, data);
		}

		public static unsafe void GetActiveAttrib(uint program, uint index, int bufSize, int* length, int[] size, IntPtr type, StringBuilder name)
		{
			glGetActiveAttrib deleg = BaseGraphicsContext.Current.Loader.Get<glGetActiveAttrib>();
			if (deleg != null)
				deleg(program, index, bufSize, length, size, type, name);
		}

		public static unsafe void GetActiveUniform(uint program, uint index, int bufSize, int* length, int[] size, IntPtr type, string name)
		{
			glGetActiveUniform deleg = BaseGraphicsContext.Current.Loader.Get<glGetActiveUniform>();
			if (deleg != null)
				deleg(program, index, bufSize, length, size, type, name);
		}

		public static void GetActiveUniformBlockiv(uint program, uint uniformBlockIndex, ActiveUniformBlockParameter pname, int parameters)
		{
			glGetActiveUniformBlockiv deleg = BaseGraphicsContext.Current.Loader.Get<glGetActiveUniformBlockiv>();
			if (deleg != null)
				deleg(program, uniformBlockIndex, pname, parameters);
		}

		public static unsafe void GetActiveUniformBlockName(uint program, uint uniformBlockIndex, int bufSize, int* length, string uniformBlockName)
		{
			glGetActiveUniformBlockName deleg = BaseGraphicsContext.Current.Loader.Get<glGetActiveUniformBlockName>();
			if (deleg != null)
				deleg(program, uniformBlockIndex, bufSize, length, uniformBlockName);
		}

		public static unsafe void GetActiveUniformName(uint program, uint uniformIndex, int bufSize, int* length, StringBuilder uniformName)
		{
			glGetActiveUniformName deleg = BaseGraphicsContext.Current.Loader.Get<glGetActiveUniformName>();
			if (deleg != null)
				deleg(program, uniformIndex, bufSize, length, uniformName);
		}

		public static unsafe void GetActiveUniformsiv(uint program, int uniformCount, uint* uniformIndices, ActiveUniformParameter pname, int[] parameters)
		{
			glGetActiveUniformsiv deleg = BaseGraphicsContext.Current.Loader.Get<glGetActiveUniformsiv>();
			if (deleg != null)
				deleg(program, uniformCount, uniformIndices, pname, parameters);
		}

		public static unsafe void GetAttachedShaders(uint program, int maxCount, int* count, uint* shaders)
		{
			glGetAttachedShaders deleg = BaseGraphicsContext.Current.Loader.Get<glGetAttachedShaders>();
			if (deleg != null)
				deleg(program, maxCount, count, shaders);
		}

		public static int GetAttribLocation(uint program, string name)
		{
			glGetAttribLocation deleg = BaseGraphicsContext.Current.Loader.Get<glGetAttribLocation>();
			if (deleg != null)
				return deleg(program, name);
			return default(int);
		}

		public static void GetBufferParameteriv(BufferTarget target, BufferParameterName value, int[] data)
		{
			glGetBufferParameteriv deleg = BaseGraphicsContext.Current.Loader.Get<glGetBufferParameteriv>();
			if (deleg != null)
				deleg(target, value, data);
		}

		public static void GetBufferPointerv(BufferTarget target, BufferPointer pname, IntPtr parameters)
		{
			glGetBufferPointerv deleg = BaseGraphicsContext.Current.Loader.Get<glGetBufferPointerv>();
			if (deleg != null)
				deleg(target, pname, parameters);
		}

		public static void GetBufferSubData(BufferTarget target, IntPtr offset, IntPtr size, IntPtr data)
		{
			glGetBufferSubData deleg = BaseGraphicsContext.Current.Loader.Get<glGetBufferSubData>();
			if (deleg != null)
				deleg(target, offset, size, data);
		}

		public static void GetCompressedTexImage(TextureTarget target, int lod, IntPtr img)
		{
			glGetCompressedTexImage deleg = BaseGraphicsContext.Current.Loader.Get<glGetCompressedTexImage>();
			if (deleg != null)
				deleg(target, lod, img);
		}

		public static ErrorCode GetError()
		{
			glGetError deleg = BaseGraphicsContext.Current.Loader.Get<glGetError>();
			if (deleg != null)
				return deleg();
			return default(ErrorCode);
		}

		public static int GetFragDataIndex(uint program, string name)
		{
			glGetFragDataIndex deleg = BaseGraphicsContext.Current.Loader.Get<glGetFragDataIndex>();
			if (deleg != null)
				return deleg(program, name);
			return default(int);
		}

		public static int GetFragDataLocation(uint program, string name)
		{
			glGetFragDataLocation deleg = BaseGraphicsContext.Current.Loader.Get<glGetFragDataLocation>();
			if (deleg != null)
				return deleg(program, name);
			return default(int);
		}

		public static void GetFramebufferAttachmentParameter(FramebufferTarget target, FramebufferAttachment attachment, FramebufferParameterName pname, int[] parameters)
		{
			glGetFramebufferAttachmentParameter deleg = BaseGraphicsContext.Current.Loader.Get<glGetFramebufferAttachmentParameter>();
			if (deleg != null)
				deleg(target, attachment, pname, parameters);
		}

		public static void GetMultisamplefv(GetMultisamplePName pname, uint index, float[] val)
		{
			glGetMultisamplefv deleg = BaseGraphicsContext.Current.Loader.Get<glGetMultisamplefv>();
			if (deleg != null)
				deleg(pname, index, val);
		}

		public static void GetProgramiv(uint program, ProgramParameter pname, int[] parameters)
		{
			glGetProgramiv deleg = BaseGraphicsContext.Current.Loader.Get<glGetProgramiv>();
			if (deleg != null)
				deleg(program, pname, parameters);
		}

		public static unsafe void GetProgramInfoLog(uint program, int maxLength, int* length, string infoLog)
		{
			glGetProgramInfoLog deleg = BaseGraphicsContext.Current.Loader.Get<glGetProgramInfoLog>();
			if (deleg != null)
				deleg(program, maxLength, length, infoLog);
		}

		public static void GetQueryiv(QueryTarget target, GetQueryParam pname, int[] parameters)
		{
			glGetQueryiv deleg = BaseGraphicsContext.Current.Loader.Get<glGetQueryiv>();
			if (deleg != null)
				deleg(target, pname, parameters);
		}

		public static void GetQueryObjectiv(uint id, GetQueryObjectParam pname, int[] parameters)
		{
			glGetQueryObjectiv deleg = BaseGraphicsContext.Current.Loader.Get<glGetQueryObjectiv>();
			if (deleg != null)
				deleg(id, pname, parameters);
		}

		public static unsafe void GetQueryObjectuiv(uint id, GetQueryObjectParam pname, uint* parameters)
		{
			glGetQueryObjectuiv deleg = BaseGraphicsContext.Current.Loader.Get<glGetQueryObjectuiv>();
			if (deleg != null)
				deleg(id, pname, parameters);
		}

		public static void GetQueryObjecti64v(uint id, GetQueryObjectParam pname, long[] parameters)
		{
			glGetQueryObjecti64v deleg = BaseGraphicsContext.Current.Loader.Get<glGetQueryObjecti64v>();
			if (deleg != null)
				deleg(id, pname, parameters);
		}

		public static void GetQueryObjectui64v(uint id, GetQueryObjectParam pname, ulong[] parameters)
		{
			glGetQueryObjectui64v deleg = BaseGraphicsContext.Current.Loader.Get<glGetQueryObjectui64v>();
			if (deleg != null)
				deleg(id, pname, parameters);
		}

		public static void GetRenderbufferParameteriv(RenderbufferTarget target, RenderbufferParameterName pname, int[] parameters)
		{
			glGetRenderbufferParameteriv deleg = BaseGraphicsContext.Current.Loader.Get<glGetRenderbufferParameteriv>();
			if (deleg != null)
				deleg(target, pname, parameters);
		}

		public static void GetSamplerParameterfv(uint sampler, All pname, float[] parameters)
		{
			glGetSamplerParameterfv deleg = BaseGraphicsContext.Current.Loader.Get<glGetSamplerParameterfv>();
			if (deleg != null)
				deleg(sampler, pname, parameters);
		}

		public static void GetSamplerParameteriv(uint sampler, All pname, int[] parameters)
		{
			glGetSamplerParameteriv deleg = BaseGraphicsContext.Current.Loader.Get<glGetSamplerParameteriv>();
			if (deleg != null)
				deleg(sampler, pname, parameters);
		}

		public static void GetShaderiv(uint shader, All pname, int[] parameters)
		{
			glGetShaderiv deleg = BaseGraphicsContext.Current.Loader.Get<glGetShaderiv>();
			if (deleg != null)
				deleg(shader, pname, parameters);
		}

		public static unsafe void GetShaderInfoLog(uint shader, int maxLength, int* length, StringBuilder infoLog)
		{
			glGetShaderInfoLog deleg = BaseGraphicsContext.Current.Loader.Get<glGetShaderInfoLog>();
			if (deleg != null)
				deleg(shader, maxLength, length, infoLog);
		}

		public static unsafe void GetShaderSource(uint shader, int bufSize, int* length, string source)
		{
			glGetShaderSource deleg = BaseGraphicsContext.Current.Loader.Get<glGetShaderSource>();
			if (deleg != null)
				deleg(shader, bufSize, length, source);
		}

		public static unsafe sbyte* GetString(StringName name)
		{
			glGetString deleg = BaseGraphicsContext.Current.Loader.Get<glGetString>();
			if (deleg != null)
				return deleg(name);
			return default(sbyte*);
		}

		public static unsafe sbyte* GetStringi(StringName name, uint index)
		{
			glGetStringi deleg = BaseGraphicsContext.Current.Loader.Get<glGetStringi>();
			if (deleg != null)
				return deleg(name, index);
			return default(sbyte*);
		}

		public static unsafe void GetSynciv(IntPtr sync, ArbSync pname, int bufSize, int* length, int[] values)
		{
			glGetSynciv deleg = BaseGraphicsContext.Current.Loader.Get<glGetSynciv>();
			if (deleg != null)
				deleg(sync, pname, bufSize, length, values);
		}

		public static void GetTexImage(TextureTarget target, int level, PixelFormat format, PixelType type, IntPtr img)
		{
			glGetTexImage deleg = BaseGraphicsContext.Current.Loader.Get<glGetTexImage>();
			if (deleg != null)
				deleg(target, level, format, type, img);
		}

		public static void GetTexLevelParameterfv(TextureTarget target, int level, GetTextureParameter pname, float[] parameters)
		{
			glGetTexLevelParameterfv deleg = BaseGraphicsContext.Current.Loader.Get<glGetTexLevelParameterfv>();
			if (deleg != null)
				deleg(target, level, pname, parameters);
		}

		public static void GetTexLevelParameteriv(TextureTarget target, int level, GetTextureParameter pname, int[] parameters)
		{
			glGetTexLevelParameteriv deleg = BaseGraphicsContext.Current.Loader.Get<glGetTexLevelParameteriv>();
			if (deleg != null)
				deleg(target, level, pname, parameters);
		}

		public static void GetTexParameterfv(TextureTarget target, GetTextureParameter pname, float[] parameters)
		{
			glGetTexParameterfv deleg = BaseGraphicsContext.Current.Loader.Get<glGetTexParameterfv>();
			if (deleg != null)
				deleg(target, pname, parameters);
		}

		public static void GetTexParameteriv(TextureTarget target, GetTextureParameter pname, int[] parameters)
		{
			glGetTexParameteriv deleg = BaseGraphicsContext.Current.Loader.Get<glGetTexParameteriv>();
			if (deleg != null)
				deleg(target, pname, parameters);
		}

		public static unsafe void GetTransformFeedbackVarying(uint program, uint index, int bufSize, int* length, int size, IntPtr type, IntPtr name)
		{
			glGetTransformFeedbackVarying deleg = BaseGraphicsContext.Current.Loader.Get<glGetTransformFeedbackVarying>();
			if (deleg != null)
				deleg(program, index, bufSize, length, size, type, name);
		}

		public static void GetUniformfv(uint program, int location, float[] parameters)
		{
			glGetUniformfv deleg = BaseGraphicsContext.Current.Loader.Get<glGetUniformfv>();
			if (deleg != null)
				deleg(program, location, parameters);
		}

		public static void GetUniformiv(uint program, int location, int[] parameters)
		{
			glGetUniformiv deleg = BaseGraphicsContext.Current.Loader.Get<glGetUniformiv>();
			if (deleg != null)
				deleg(program, location, parameters);
		}

		public static uint GetUniformBlockIndex(uint program, string uniformBlockName)
		{
			glGetUniformBlockIndex deleg = BaseGraphicsContext.Current.Loader.Get<glGetUniformBlockIndex>();
			if (deleg != null)
				return deleg(program, uniformBlockName);
			return default(uint);
		}

		public static unsafe uint GetUniformIndices(uint program, int uniformCount, string[] uniformNames, uint* uniformIndices)
		{
			glGetUniformIndices deleg = BaseGraphicsContext.Current.Loader.Get<glGetUniformIndices>();
			if (deleg != null)
				return deleg(program, uniformCount, uniformNames, uniformIndices);
			return default(uint);
		}

		public static int GetUniformLocation(uint program, string name)
		{
			glGetUniformLocation deleg = BaseGraphicsContext.Current.Loader.Get<glGetUniformLocation>();
			if (deleg != null)
				return deleg(program, name);
			return default(int);
		}

		public static void GetVertexAttribdv(uint index, VertexAttribParameter pname, double[] parameters)
		{
			glGetVertexAttribdv deleg = BaseGraphicsContext.Current.Loader.Get<glGetVertexAttribdv>();
			if (deleg != null)
				deleg(index, pname, parameters);
		}

		public static void GetVertexAttribfv(uint index, VertexAttribParameter pname, float[] parameters)
		{
			glGetVertexAttribfv deleg = BaseGraphicsContext.Current.Loader.Get<glGetVertexAttribfv>();
			if (deleg != null)
				deleg(index, pname, parameters);
		}

		public static void GetVertexAttribiv(uint index, VertexAttribParameter pname, int[] parameters)
		{
			glGetVertexAttribiv deleg = BaseGraphicsContext.Current.Loader.Get<glGetVertexAttribiv>();
			if (deleg != null)
				deleg(index, pname, parameters);
		}

		public static void GetVertexAttribIiv(uint index, VertexAttribParameter pname, int[] parameters)
		{
			glGetVertexAttribIiv deleg = BaseGraphicsContext.Current.Loader.Get<glGetVertexAttribIiv>();
			if (deleg != null)
				deleg(index, pname, parameters);
		}

		public static unsafe void GetVertexAttribIuiv(uint index, VertexAttribParameter pname, uint* parameters)
		{
			glGetVertexAttribIuiv deleg = BaseGraphicsContext.Current.Loader.Get<glGetVertexAttribIuiv>();
			if (deleg != null)
				deleg(index, pname, parameters);
		}

		public static void GetVertexAttribPointerv(uint index, VertexAttribPointerParameter pname, IntPtr pointer)
		{
			glGetVertexAttribPointerv deleg = BaseGraphicsContext.Current.Loader.Get<glGetVertexAttribPointerv>();
			if (deleg != null)
				deleg(index, pname, pointer);
		}

		public static void Hint(HintTarget target, HintMode mode)
		{
			glHint deleg = BaseGraphicsContext.Current.Loader.Get<glHint>();
			if (deleg != null)
				deleg(target, mode);
		}

		public static bool IsBuffer(uint buffer)
		{
			glIsBuffer deleg = BaseGraphicsContext.Current.Loader.Get<glIsBuffer>();
			if (deleg != null)
				return deleg(buffer);
			return default(bool);
		}

		public static bool IsEnabled(EnableCap cap)
		{
			glIsEnabled deleg = BaseGraphicsContext.Current.Loader.Get<glIsEnabled>();
			if (deleg != null)
				return deleg(cap);
			return default(bool);
		}

		public static bool IsEnabledi(EnableCap cap, uint index)
		{
			glIsEnabledi deleg = BaseGraphicsContext.Current.Loader.Get<glIsEnabledi>();
			if (deleg != null)
				return deleg(cap, index);
			return default(bool);
		}

		public static bool IsFramebuffer(uint framebuffer)
		{
			glIsFramebuffer deleg = BaseGraphicsContext.Current.Loader.Get<glIsFramebuffer>();
			if (deleg != null)
				return deleg(framebuffer);
			return default(bool);
		}

		public static bool IsProgram(uint program)
		{
			glIsProgram deleg = BaseGraphicsContext.Current.Loader.Get<glIsProgram>();
			if (deleg != null)
				return deleg(program);
			return default(bool);
		}

		public static bool IsQuery(uint id)
		{
			glIsQuery deleg = BaseGraphicsContext.Current.Loader.Get<glIsQuery>();
			if (deleg != null)
				return deleg(id);
			return default(bool);
		}

		public static bool IsRenderbuffer(uint renderbuffer)
		{
			glIsRenderbuffer deleg = BaseGraphicsContext.Current.Loader.Get<glIsRenderbuffer>();
			if (deleg != null)
				return deleg(renderbuffer);
			return default(bool);
		}

		public static bool IsSampler(uint id)
		{
			glIsSampler deleg = BaseGraphicsContext.Current.Loader.Get<glIsSampler>();
			if (deleg != null)
				return deleg(id);
			return default(bool);
		}

		public static bool IsShader(uint shader)
		{
			glIsShader deleg = BaseGraphicsContext.Current.Loader.Get<glIsShader>();
			if (deleg != null)
				return deleg(shader);
			return default(bool);
		}

		public static bool IsSync(IntPtr sync)
		{
			glIsSync deleg = BaseGraphicsContext.Current.Loader.Get<glIsSync>();
			if (deleg != null)
				return deleg(sync);
			return default(bool);
		}

		public static bool IsTexture(uint texture)
		{
			glIsTexture deleg = BaseGraphicsContext.Current.Loader.Get<glIsTexture>();
			if (deleg != null)
				return deleg(texture);
			return default(bool);
		}

		public static bool IsVertexArray(uint array)
		{
			glIsVertexArray deleg = BaseGraphicsContext.Current.Loader.Get<glIsVertexArray>();
			if (deleg != null)
				return deleg(array);
			return default(bool);
		}

		public static void LineWidth(float width)
		{
			glLineWidth deleg = BaseGraphicsContext.Current.Loader.Get<glLineWidth>();
			if (deleg != null)
				deleg(width);
		}

		public static void LinkProgram(uint program)
		{
			glLinkProgram deleg = BaseGraphicsContext.Current.Loader.Get<glLinkProgram>();
			if (deleg != null)
				deleg(program);
		}

		public static void LogicOp(LogicOp opcode)
		{
			glLogicOp deleg = BaseGraphicsContext.Current.Loader.Get<glLogicOp>();
			if (deleg != null)
				deleg(opcode);
		}

		public static IntPtr MapBuffer(BufferTarget target, BufferAccess access)
		{
			glMapBuffer deleg = BaseGraphicsContext.Current.Loader.Get<glMapBuffer>();
			if (deleg != null)
				return deleg(target, access);
			return default(IntPtr);
		}

		public static bool UnmapBuffer(BufferTarget target)
		{
			glUnmapBuffer deleg = BaseGraphicsContext.Current.Loader.Get<glUnmapBuffer>();
			if (deleg != null)
				return deleg(target);
			return default(bool);
		}

		public static IntPtr MapBufferRange(BufferTarget target, IntPtr offset, IntPtr length, BufferAccessMask access)
		{
			glMapBufferRange deleg = BaseGraphicsContext.Current.Loader.Get<glMapBufferRange>();
			if (deleg != null)
				return deleg(target, offset, length, access);
			return default(IntPtr);
		}

		public static void MultiDrawArrays(BeginMode mode, int[] first, int count, int primcount)
		{
			glMultiDrawArrays deleg = BaseGraphicsContext.Current.Loader.Get<glMultiDrawArrays>();
			if (deleg != null)
				deleg(mode, first, count, primcount);
		}

		public static void MultiDrawElements(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount)
		{
			glMultiDrawElements deleg = BaseGraphicsContext.Current.Loader.Get<glMultiDrawElements>();
			if (deleg != null)
				deleg(mode, count, type, indices, primcount);
		}

		public static void MultiDrawElementsBaseVertex(BeginMode mode, int count, DrawElementsType type, IntPtr indices, int primcount, int[] basevertex)
		{
			glMultiDrawElementsBaseVertex deleg = BaseGraphicsContext.Current.Loader.Get<glMultiDrawElementsBaseVertex>();
			if (deleg != null)
				deleg(mode, count, type, indices, primcount, basevertex);
		}

		public static void PixelStoreference(All pname, float param)
		{
			glPixelStoreference deleg = BaseGraphicsContext.Current.Loader.Get<glPixelStoreference>();
			if (deleg != null)
				deleg(pname, param);
		}

		public static void PixelStorei(PixelStoreParameter pname, int param)
		{
			glPixelStorei deleg = BaseGraphicsContext.Current.Loader.Get<glPixelStorei>();
			if (deleg != null)
				deleg(pname, param);
		}

		public static void PointParameterf(PointParameterName pname, float param)
		{
			glPointParameterf deleg = BaseGraphicsContext.Current.Loader.Get<glPointParameterf>();
			if (deleg != null)
				deleg(pname, param);
		}

		public static void PointParameteri(PointParameterName pname, int param)
		{
			glPointParameteri deleg = BaseGraphicsContext.Current.Loader.Get<glPointParameteri>();
			if (deleg != null)
				deleg(pname, param);
		}

		public static void PointParameterfv(PointParameterName pname, float[] parameters)
		{
			glPointParameterfv deleg = BaseGraphicsContext.Current.Loader.Get<glPointParameterfv>();
			if (deleg != null)
				deleg(pname, parameters);
		}

		public static void PointParameteriv(PointParameterName pname, int[] parameters)
		{
			glPointParameteriv deleg = BaseGraphicsContext.Current.Loader.Get<glPointParameteriv>();
			if (deleg != null)
				deleg(pname, parameters);
		}

		public static void PointSize(float size)
		{
			glPointSize deleg = BaseGraphicsContext.Current.Loader.Get<glPointSize>();
			if (deleg != null)
				deleg(size);
		}

		public static void PolygonMode(MaterialFace face, PolygonMode mode)
		{
			glPolygonMode deleg = BaseGraphicsContext.Current.Loader.Get<glPolygonMode>();
			if (deleg != null)
				deleg(face, mode);
		}

		public static void PolygonOffset(float factor, float units)
		{
			glPolygonOffset deleg = BaseGraphicsContext.Current.Loader.Get<glPolygonOffset>();
			if (deleg != null)
				deleg(factor, units);
		}

		public static void PrimitiveRestartIndex(uint index)
		{
			glPrimitiveRestartIndex deleg = BaseGraphicsContext.Current.Loader.Get<glPrimitiveRestartIndex>();
			if (deleg != null)
				deleg(index);
		}

		public static void ProvokingVertex(ProvokingVertexMode provokeMode)
		{
			glProvokingVertex deleg = BaseGraphicsContext.Current.Loader.Get<glProvokingVertex>();
			if (deleg != null)
				deleg(provokeMode);
		}

		public static void QueryCounter(uint id, All target)
		{
			glQueryCounter deleg = BaseGraphicsContext.Current.Loader.Get<glQueryCounter>();
			if (deleg != null)
				deleg(id, target);
		}

		public static void ReadBuffer(ReadBufferMode mode)
		{
			glReadBuffer deleg = BaseGraphicsContext.Current.Loader.Get<glReadBuffer>();
			if (deleg != null)
				deleg(mode);
		}

		public static void ReadPixels(int x, int y, int width, int height, PixelFormat format, PixelType type, IntPtr data)
		{
			glReadPixels deleg = BaseGraphicsContext.Current.Loader.Get<glReadPixels>();
			if (deleg != null)
				deleg(x, y, width, height, format, type, data);
		}

		public static void RenderbufferStorage(RenderbufferTarget target, RenderbufferStorage internalformat, int width, int height)
		{
			glRenderbufferStorage deleg = BaseGraphicsContext.Current.Loader.Get<glRenderbufferStorage>();
			if (deleg != null)
				deleg(target, internalformat, width, height);
		}

		public static void RenderbufferStorageMultisample(RenderbufferTarget target, int samples, RenderbufferStorage internalformat, int width, int height)
		{
			glRenderbufferStorageMultisample deleg = BaseGraphicsContext.Current.Loader.Get<glRenderbufferStorageMultisample>();
			if (deleg != null)
				deleg(target, samples, internalformat, width, height);
		}

		public static void SampleCoverage(float value, bool invert)
		{
			glSampleCoverage deleg = BaseGraphicsContext.Current.Loader.Get<glSampleCoverage>();
			if (deleg != null)
				deleg(value, invert);
		}

		public static void SampleMaski(uint maskNumber, All mask)
		{
			glSampleMaski deleg = BaseGraphicsContext.Current.Loader.Get<glSampleMaski>();
			if (deleg != null)
				deleg(maskNumber, mask);
		}

		public static void SamplerParameterf(uint sampler, All pname, float param)
		{
			glSamplerParameterf deleg = BaseGraphicsContext.Current.Loader.Get<glSamplerParameterf>();
			if (deleg != null)
				deleg(sampler, pname, param);
		}

		public static void SamplerParameteri(uint sampler, All pname, int param)
		{
			glSamplerParameteri deleg = BaseGraphicsContext.Current.Loader.Get<glSamplerParameteri>();
			if (deleg != null)
				deleg(sampler, pname, param);
		}

		public static void SamplerParameterfv(uint sampler, All pname, float[] parameters)
		{
			glSamplerParameterfv deleg = BaseGraphicsContext.Current.Loader.Get<glSamplerParameterfv>();
			if (deleg != null)
				deleg(sampler, pname, parameters);
		}

		public static void SamplerParameteriv(uint sampler, All pname, int[] parameters)
		{
			glSamplerParameteriv deleg = BaseGraphicsContext.Current.Loader.Get<glSamplerParameteriv>();
			if (deleg != null)
				deleg(sampler, pname, parameters);
		}

		public static void Scissor(int x, int y, int width, int height)
		{
			glScissor deleg = BaseGraphicsContext.Current.Loader.Get<glScissor>();
			if (deleg != null)
				deleg(x, y, width, height);
		}

		public static void ShaderSource(uint shader, int count, string[] text, int[] length)
		{
			glShaderSource deleg = BaseGraphicsContext.Current.Loader.Get<glShaderSource>();
			if (deleg != null)
				deleg(shader, count, text, length);
		}

		public static void StencilFunc(StencilFunction func, int reference, uint mask)
		{
			glStencilFunc deleg = BaseGraphicsContext.Current.Loader.Get<glStencilFunc>();
			if (deleg != null)
				deleg(func, reference, mask);
		}

		public static void StencilFuncSeparate(StencilFace face, StencilFunction func, int reference, uint mask)
		{
			glStencilFuncSeparate deleg = BaseGraphicsContext.Current.Loader.Get<glStencilFuncSeparate>();
			if (deleg != null)
				deleg(face, func, reference, mask);
		}

		public static void StencilMask(uint mask)
		{
			glStencilMask deleg = BaseGraphicsContext.Current.Loader.Get<glStencilMask>();
			if (deleg != null)
				deleg(mask);
		}

		public static void StencilMaskSeparate(StencilFace face, uint mask)
		{
			glStencilMaskSeparate deleg = BaseGraphicsContext.Current.Loader.Get<glStencilMaskSeparate>();
			if (deleg != null)
				deleg(face, mask);
		}

		public static void StencilOp(StencilOp sfail, StencilOp dpfail, StencilOp dppass)
		{
			glStencilOp deleg = BaseGraphicsContext.Current.Loader.Get<glStencilOp>();
			if (deleg != null)
				deleg(sfail, dpfail, dppass);
		}

		public static void StencilOpSeparate(StencilFace face, StencilOp sfail, StencilOp dpfail, StencilOp dppass)
		{
			glStencilOpSeparate deleg = BaseGraphicsContext.Current.Loader.Get<glStencilOpSeparate>();
			if (deleg != null)
				deleg(face, sfail, dpfail, dppass);
		}

		public static void TexBuffer(TextureBufferTarget target, SizedInternalFormat internalFormat, uint buffer)
		{
			glTexBuffer deleg = BaseGraphicsContext.Current.Loader.Get<glTexBuffer>();
			if (deleg != null)
				deleg(target, internalFormat, buffer);
		}

		public static void TexImage1D(TextureTarget target, int level, int internalFormat, int width, int border, PixelInternalFormat format, PixelType type, IntPtr data)
		{
			glTexImage1D deleg = BaseGraphicsContext.Current.Loader.Get<glTexImage1D>();
			if (deleg != null)
				deleg(target, level, internalFormat, width, border, format, type, data);
		}

		public static void TexImage2D(TextureTarget target, int level, int internalFormat, int width, int height, int border, PixelInternalFormat format, PixelType type, IntPtr data)
		{
			glTexImage2D deleg = BaseGraphicsContext.Current.Loader.Get<glTexImage2D>();
			if (deleg != null)
				deleg(target, level, internalFormat, width, height, border, format, type, data);
		}

		public static void TexImage2DMultisample(TextureTargetMultisample target, int samples, int internalformat, int width, int height, bool fixedsamplelocations)
		{
			glTexImage2DMultisample deleg = BaseGraphicsContext.Current.Loader.Get<glTexImage2DMultisample>();
			if (deleg != null)
				deleg(target, samples, internalformat, width, height, fixedsamplelocations);
		}

		public static void TexImage3D(TextureTarget target, int level, int internalFormat, int width, int height, int depth, int border, PixelInternalFormat format, PixelType type, IntPtr data)
		{
			glTexImage3D deleg = BaseGraphicsContext.Current.Loader.Get<glTexImage3D>();
			if (deleg != null)
				deleg(target, level, internalFormat, width, height, depth, border, format, type, data);
		}

		public static void TexImage3DMultisample(TextureTargetMultisample target, int samples, int internalformat, int width, int height, int depth, bool fixedsamplelocations)
		{
			glTexImage3DMultisample deleg = BaseGraphicsContext.Current.Loader.Get<glTexImage3DMultisample>();
			if (deleg != null)
				deleg(target, samples, internalformat, width, height, depth, fixedsamplelocations);
		}

		public static void TexParameterf(TextureTarget target, TextureParameterName pname, float param)
		{
			glTexParameterf deleg = BaseGraphicsContext.Current.Loader.Get<glTexParameterf>();
			if (deleg != null)
				deleg(target, pname, param);
		}

		public static void TexParameteri(TextureTarget target, TextureParameterName pname, int param)
		{
			glTexParameteri deleg = BaseGraphicsContext.Current.Loader.Get<glTexParameteri>();
			if (deleg != null)
				deleg(target, pname, param);
		}

		public static void TexParameterfv(TextureTarget target, TextureParameterName pname, float[] parameters)
		{
			glTexParameterfv deleg = BaseGraphicsContext.Current.Loader.Get<glTexParameterfv>();
			if (deleg != null)
				deleg(target, pname, parameters);
		}

		public static void TexParameteriv(TextureTarget target, TextureParameterName pname, int[] parameters)
		{
			glTexParameteriv deleg = BaseGraphicsContext.Current.Loader.Get<glTexParameteriv>();
			if (deleg != null)
				deleg(target, pname, parameters);
		}

		public static void TexParameterIiv(TextureTarget target, TextureParameterName pname, int[] parameters)
		{
			glTexParameterIiv deleg = BaseGraphicsContext.Current.Loader.Get<glTexParameterIiv>();
			if (deleg != null)
				deleg(target, pname, parameters);
		}

		public static unsafe void TexParameterIuiv(TextureTarget target, TextureParameterName pname, uint* parameters)
		{
			glTexParameterIuiv deleg = BaseGraphicsContext.Current.Loader.Get<glTexParameterIuiv>();
			if (deleg != null)
				deleg(target, pname, parameters);
		}

		public static void TexSubImage1D(TextureTarget target, int level, int xoffset, int width, PixelFormat format, PixelType type, IntPtr data)
		{
			glTexSubImage1D deleg = BaseGraphicsContext.Current.Loader.Get<glTexSubImage1D>();
			if (deleg != null)
				deleg(target, level, xoffset, width, format, type, data);
		}

		public static void TexSubImage2D(TextureTarget target, int level, int xoffset, int yoffset, int width, int height, PixelFormat format, PixelType type, IntPtr data)
		{
			glTexSubImage2D deleg = BaseGraphicsContext.Current.Loader.Get<glTexSubImage2D>();
			if (deleg != null)
				deleg(target, level, xoffset, yoffset, width, height, format, type, data);
		}

		public static void TexSubImage3D(TextureTarget target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, PixelFormat format, PixelType type, IntPtr data)
		{
			glTexSubImage3D deleg = BaseGraphicsContext.Current.Loader.Get<glTexSubImage3D>();
			if (deleg != null)
				deleg(target, level, xoffset, yoffset, zoffset, width, height, depth, format, type, data);
		}

		public static void TransformFeedbackVaryings(uint program, int count, string[] varyings, ExtTransformFeedback bufferMode)
		{
			glTransformFeedbackVaryings deleg = BaseGraphicsContext.Current.Loader.Get<glTransformFeedbackVaryings>();
			if (deleg != null)
				deleg(program, count, varyings, bufferMode);
		}

		public static void Uniform1f(int location, float v0)
		{
			glUniform1f deleg = BaseGraphicsContext.Current.Loader.Get<glUniform1f>();
			if (deleg != null)
				deleg(location, v0);
		}

		public static void Uniform2f(int location, float v0, float v1)
		{
			glUniform2f deleg = BaseGraphicsContext.Current.Loader.Get<glUniform2f>();
			if (deleg != null)
				deleg(location, v0, v1);
		}

		public static void Uniform3f(int location, float v0, float v1, float v2)
		{
			glUniform3f deleg = BaseGraphicsContext.Current.Loader.Get<glUniform3f>();
			if (deleg != null)
				deleg(location, v0, v1, v2);
		}

		public static void Uniform4f(int location, float v0, float v1, float v2, float v3)
		{
			glUniform4f deleg = BaseGraphicsContext.Current.Loader.Get<glUniform4f>();
			if (deleg != null)
				deleg(location, v0, v1, v2, v3);
		}

		public static void Uniform1i(int location, int v0)
		{
			glUniform1i deleg = BaseGraphicsContext.Current.Loader.Get<glUniform1i>();
			if (deleg != null)
				deleg(location, v0);
		}

		public static void Uniform2i(int location, int v0, int v1)
		{
			glUniform2i deleg = BaseGraphicsContext.Current.Loader.Get<glUniform2i>();
			if (deleg != null)
				deleg(location, v0, v1);
		}

		public static void Uniform3i(int location, int v0, int v1, int v2)
		{
			glUniform3i deleg = BaseGraphicsContext.Current.Loader.Get<glUniform3i>();
			if (deleg != null)
				deleg(location, v0, v1, v2);
		}

		public static void Uniform4i(int location, int v0, int v1, int v2, int v3)
		{
			glUniform4i deleg = BaseGraphicsContext.Current.Loader.Get<glUniform4i>();
			if (deleg != null)
				deleg(location, v0, v1, v2, v3);
		}

		public static void Uniform1ui(int location, uint v0)
		{
			glUniform1ui deleg = BaseGraphicsContext.Current.Loader.Get<glUniform1ui>();
			if (deleg != null)
				deleg(location, v0);
		}

		public static void Uniform2ui(int location, int v0, uint v1)
		{
			glUniform2ui deleg = BaseGraphicsContext.Current.Loader.Get<glUniform2ui>();
			if (deleg != null)
				deleg(location, v0, v1);
		}

		public static void Uniform3ui(int location, int v0, int v1, uint v2)
		{
			glUniform3ui deleg = BaseGraphicsContext.Current.Loader.Get<glUniform3ui>();
			if (deleg != null)
				deleg(location, v0, v1, v2);
		}

		public static void Uniform4ui(int location, int v0, int v1, int v2, uint v3)
		{
			glUniform4ui deleg = BaseGraphicsContext.Current.Loader.Get<glUniform4ui>();
			if (deleg != null)
				deleg(location, v0, v1, v2, v3);
		}

		public static void Uniform1fv(int location, int count, float[] value)
		{
			glUniform1fv deleg = BaseGraphicsContext.Current.Loader.Get<glUniform1fv>();
			if (deleg != null)
				deleg(location, count, value);
		}

		public static void Uniform2fv(int location, int count, float[] value)
		{
			glUniform2fv deleg = BaseGraphicsContext.Current.Loader.Get<glUniform2fv>();
			if (deleg != null)
				deleg(location, count, value);
		}

		public static void Uniform3fv(int location, int count, float[] value)
		{
			glUniform3fv deleg = BaseGraphicsContext.Current.Loader.Get<glUniform3fv>();
			if (deleg != null)
				deleg(location, count, value);
		}

		public static void Uniform4fv(int location, int count, float[] value)
		{
			glUniform4fv deleg = BaseGraphicsContext.Current.Loader.Get<glUniform4fv>();
			if (deleg != null)
				deleg(location, count, value);
		}

		public static void Uniform1iv(int location, int count, int[] value)
		{
			glUniform1iv deleg = BaseGraphicsContext.Current.Loader.Get<glUniform1iv>();
			if (deleg != null)
				deleg(location, count, value);
		}

		public static void Uniform2iv(int location, int count, int[] value)
		{
			glUniform2iv deleg = BaseGraphicsContext.Current.Loader.Get<glUniform2iv>();
			if (deleg != null)
				deleg(location, count, value);
		}

		public static void Uniform3iv(int location, int count, int[] value)
		{
			glUniform3iv deleg = BaseGraphicsContext.Current.Loader.Get<glUniform3iv>();
			if (deleg != null)
				deleg(location, count, value);
		}

		public static void Uniform4iv(int location, int count, int[] value)
		{
			glUniform4iv deleg = BaseGraphicsContext.Current.Loader.Get<glUniform4iv>();
			if (deleg != null)
				deleg(location, count, value);
		}

		public static unsafe void Uniform1uiv(int location, int count, uint* value)
		{
			glUniform1uiv deleg = BaseGraphicsContext.Current.Loader.Get<glUniform1uiv>();
			if (deleg != null)
				deleg(location, count, value);
		}

		public static unsafe void Uniform2uiv(int location, int count, uint* value)
		{
			glUniform2uiv deleg = BaseGraphicsContext.Current.Loader.Get<glUniform2uiv>();
			if (deleg != null)
				deleg(location, count, value);
		}

		public static unsafe void Uniform3uiv(int location, int count, uint* value)
		{
			glUniform3uiv deleg = BaseGraphicsContext.Current.Loader.Get<glUniform3uiv>();
			if (deleg != null)
				deleg(location, count, value);
		}

		public static unsafe void Uniform4uiv(int location, int count, uint* value)
		{
			glUniform4uiv deleg = BaseGraphicsContext.Current.Loader.Get<glUniform4uiv>();
			if (deleg != null)
				deleg(location, count, value);
		}

		public static void UniformMatrix2fv(int location, int count, bool transpose, float[] value)
		{
			glUniformMatrix2fv deleg = BaseGraphicsContext.Current.Loader.Get<glUniformMatrix2fv>();
			if (deleg != null)
				deleg(location, count, transpose, value);
		}

		public static void UniformMatrix3fv(int location, int count, bool transpose, float[] value)
		{
			glUniformMatrix3fv deleg = BaseGraphicsContext.Current.Loader.Get<glUniformMatrix3fv>();
			if (deleg != null)
				deleg(location, count, transpose, value);
		}

		public static void UniformMatrix4fv(int location, int count, bool transpose, float[] value)
		{
			glUniformMatrix4fv deleg = BaseGraphicsContext.Current.Loader.Get<glUniformMatrix4fv>();
			if (deleg != null)
				deleg(location, count, transpose, value);
		}

		public static void UniformMatrix2x3fv(int location, int count, bool transpose, float[] value)
		{
			glUniformMatrix2x3fv deleg = BaseGraphicsContext.Current.Loader.Get<glUniformMatrix2x3fv>();
			if (deleg != null)
				deleg(location, count, transpose, value);
		}

		public static void UniformMatrix3x2fv(int location, int count, bool transpose, float[] value)
		{
			glUniformMatrix3x2fv deleg = BaseGraphicsContext.Current.Loader.Get<glUniformMatrix3x2fv>();
			if (deleg != null)
				deleg(location, count, transpose, value);
		}

		public static void UniformMatrix2x4fv(int location, int count, bool transpose, float[] value)
		{
			glUniformMatrix2x4fv deleg = BaseGraphicsContext.Current.Loader.Get<glUniformMatrix2x4fv>();
			if (deleg != null)
				deleg(location, count, transpose, value);
		}

		public static void UniformMatrix4x2fv(int location, int count, bool transpose, float[] value)
		{
			glUniformMatrix4x2fv deleg = BaseGraphicsContext.Current.Loader.Get<glUniformMatrix4x2fv>();
			if (deleg != null)
				deleg(location, count, transpose, value);
		}

		public static void UniformMatrix3x4fv(int location, int count, bool transpose, float[] value)
		{
			glUniformMatrix3x4fv deleg = BaseGraphicsContext.Current.Loader.Get<glUniformMatrix3x4fv>();
			if (deleg != null)
				deleg(location, count, transpose, value);
		}

		public static void UniformMatrix4x3fv(int location, int count, bool transpose, float[] value)
		{
			glUniformMatrix4x3fv deleg = BaseGraphicsContext.Current.Loader.Get<glUniformMatrix4x3fv>();
			if (deleg != null)
				deleg(location, count, transpose, value);
		}

		public static void UniformBlockBinding(uint program, uint uniformBlockIndex, uint uniformBlockBinding)
		{
			glUniformBlockBinding deleg = BaseGraphicsContext.Current.Loader.Get<glUniformBlockBinding>();
			if (deleg != null)
				deleg(program, uniformBlockIndex, uniformBlockBinding);
		}

		public static void UseProgram(uint program)
		{
			glUseProgram deleg = BaseGraphicsContext.Current.Loader.Get<glUseProgram>();
			if (deleg != null)
				deleg(program);
		}

		public static void ValidateProgram(uint program)
		{
			glValidateProgram deleg = BaseGraphicsContext.Current.Loader.Get<glValidateProgram>();
			if (deleg != null)
				deleg(program);
		}

		public static void VertexAttrib1f(uint index, float v0)
		{
			glVertexAttrib1f deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib1f>();
			if (deleg != null)
				deleg(index, v0);
		}

		public static void VertexAttrib1s(uint index, short v0)
		{
			glVertexAttrib1s deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib1s>();
			if (deleg != null)
				deleg(index, v0);
		}

		public static void VertexAttrib1d(uint index, double v0)
		{
			glVertexAttrib1d deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib1d>();
			if (deleg != null)
				deleg(index, v0);
		}

		public static void VertexAttribI1i(uint index, int v0)
		{
			glVertexAttribI1i deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI1i>();
			if (deleg != null)
				deleg(index, v0);
		}

		public static void VertexAttribI1ui(uint index, uint v0)
		{
			glVertexAttribI1ui deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI1ui>();
			if (deleg != null)
				deleg(index, v0);
		}

		public static void VertexAttrib2f(uint index, float v0, float v1)
		{
			glVertexAttrib2f deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib2f>();
			if (deleg != null)
				deleg(index, v0, v1);
		}

		public static void VertexAttrib2s(uint index, short v0, short v1)
		{
			glVertexAttrib2s deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib2s>();
			if (deleg != null)
				deleg(index, v0, v1);
		}

		public static void VertexAttrib2d(uint index, double v0, double v1)
		{
			glVertexAttrib2d deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib2d>();
			if (deleg != null)
				deleg(index, v0, v1);
		}

		public static void VertexAttribI2i(uint index, int v0, int v1)
		{
			glVertexAttribI2i deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI2i>();
			if (deleg != null)
				deleg(index, v0, v1);
		}

		public static void VertexAttribI2ui(uint index, uint v0, uint v1)
		{
			glVertexAttribI2ui deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI2ui>();
			if (deleg != null)
				deleg(index, v0, v1);
		}

		public static void VertexAttrib3f(uint index, float v0, float v1, float v2)
		{
			glVertexAttrib3f deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib3f>();
			if (deleg != null)
				deleg(index, v0, v1, v2);
		}

		public static void VertexAttrib3s(uint index, short v0, short v1, short v2)
		{
			glVertexAttrib3s deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib3s>();
			if (deleg != null)
				deleg(index, v0, v1, v2);
		}

		public static void VertexAttrib3d(uint index, double v0, double v1, double v2)
		{
			glVertexAttrib3d deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib3d>();
			if (deleg != null)
				deleg(index, v0, v1, v2);
		}

		public static void VertexAttribI3i(uint index, int v0, int v1, int v2)
		{
			glVertexAttribI3i deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI3i>();
			if (deleg != null)
				deleg(index, v0, v1, v2);
		}

		public static void VertexAttribI3ui(uint index, uint v0, uint v1, uint v2)
		{
			glVertexAttribI3ui deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI3ui>();
			if (deleg != null)
				deleg(index, v0, v1, v2);
		}

		public static void VertexAttrib4f(uint index, float v0, float v1, float v2, float v3)
		{
			glVertexAttrib4f deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4f>();
			if (deleg != null)
				deleg(index, v0, v1, v2, v3);
		}

		public static void VertexAttrib4s(uint index, short v0, short v1, short v2, short v3)
		{
			glVertexAttrib4s deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4s>();
			if (deleg != null)
				deleg(index, v0, v1, v2, v3);
		}

		public static void VertexAttrib4d(uint index, double v0, double v1, double v2, double v3)
		{
			glVertexAttrib4d deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4d>();
			if (deleg != null)
				deleg(index, v0, v1, v2, v3);
		}

		public static void VertexAttrib4Nub(uint index, byte v0, byte v1, byte v2, byte v3)
		{
			glVertexAttrib4Nub deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4Nub>();
			if (deleg != null)
				deleg(index, v0, v1, v2, v3);
		}

		public static void VertexAttribI4i(uint index, int v0, int v1, int v2, int v3)
		{
			glVertexAttribI4i deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI4i>();
			if (deleg != null)
				deleg(index, v0, v1, v2, v3);
		}

		public static void VertexAttribI4ui(uint index, uint v0, uint v1, uint v2, uint v3)
		{
			glVertexAttribI4ui deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI4ui>();
			if (deleg != null)
				deleg(index, v0, v1, v2, v3);
		}

		public static void VertexAttrib1fv(uint index, float[] v)
		{
			glVertexAttrib1fv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib1fv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib1sv(uint index, short[] v)
		{
			glVertexAttrib1sv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib1sv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib1dv(uint index, double[] v)
		{
			glVertexAttrib1dv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib1dv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttribI1iv(uint index, int[] v)
		{
			glVertexAttribI1iv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI1iv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static unsafe void VertexAttribI1uiv(uint index, uint* v)
		{
			glVertexAttribI1uiv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI1uiv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib2fv(uint index, float[] v)
		{
			glVertexAttrib2fv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib2fv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib2sv(uint index, short[] v)
		{
			glVertexAttrib2sv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib2sv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib2dv(uint index, double[] v)
		{
			glVertexAttrib2dv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib2dv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttribI2iv(uint index, int[] v)
		{
			glVertexAttribI2iv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI2iv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static unsafe void VertexAttribI2uiv(uint index, uint* v)
		{
			glVertexAttribI2uiv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI2uiv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib3fv(uint index, float[] v)
		{
			glVertexAttrib3fv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib3fv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib3sv(uint index, short[] v)
		{
			glVertexAttrib3sv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib3sv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib3dv(uint index, double[] v)
		{
			glVertexAttrib3dv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib3dv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttribI3iv(uint index, int[] v)
		{
			glVertexAttribI3iv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI3iv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static unsafe void VertexAttribI3uiv(uint index, uint* v)
		{
			glVertexAttribI3uiv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI3uiv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib4fv(uint index, float[] v)
		{
			glVertexAttrib4fv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4fv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib4sv(uint index, short[] v)
		{
			glVertexAttrib4sv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4sv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib4dv(uint index, double[] v)
		{
			glVertexAttrib4dv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4dv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib4iv(uint index, int[] v)
		{
			glVertexAttrib4iv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4iv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib4bv(uint index, byte[] v)
		{
			glVertexAttrib4bv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4bv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib4ubv(uint index, sbyte[] v)
		{
			glVertexAttrib4ubv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4ubv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib4usv(uint index, ushort[] v)
		{
			glVertexAttrib4usv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4usv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static unsafe void VertexAttrib4uiv(uint index, uint* v)
		{
			glVertexAttrib4uiv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4uiv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib4Nbv(uint index, byte[] v)
		{
			glVertexAttrib4Nbv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4Nbv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib4Nsv(uint index, short[] v)
		{
			glVertexAttrib4Nsv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4Nsv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib4Niv(uint index, int[] v)
		{
			glVertexAttrib4Niv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4Niv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib4Nubv(uint index, sbyte[] v)
		{
			glVertexAttrib4Nubv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4Nubv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttrib4Nusv(uint index, ushort[] v)
		{
			glVertexAttrib4Nusv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4Nusv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static unsafe void VertexAttrib4Nuiv(uint index, uint* v)
		{
			glVertexAttrib4Nuiv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttrib4Nuiv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttribI4bv(uint index, byte[] v)
		{
			glVertexAttribI4bv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI4bv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttribI4ubv(uint index, sbyte[] v)
		{
			glVertexAttribI4ubv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI4ubv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttribI4sv(uint index, short[] v)
		{
			glVertexAttribI4sv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI4sv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttribI4usv(uint index, ushort[] v)
		{
			glVertexAttribI4usv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI4usv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttribI4iv(uint index, int[] v)
		{
			glVertexAttribI4iv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI4iv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static unsafe void VertexAttribI4uiv(uint index, uint* v)
		{
			glVertexAttribI4uiv deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribI4uiv>();
			if (deleg != null)
				deleg(index, v);
		}

		public static void VertexAttribP1ui(uint index, All type, bool normalized, uint value)
		{
			glVertexAttribP1ui deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribP1ui>();
			if (deleg != null)
				deleg(index, type, normalized, value);
		}

		public static void VertexAttribP2ui(uint index, All type, bool normalized, uint value)
		{
			glVertexAttribP2ui deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribP2ui>();
			if (deleg != null)
				deleg(index, type, normalized, value);
		}

		public static void VertexAttribP3ui(uint index, All type, bool normalized, uint value)
		{
			glVertexAttribP3ui deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribP3ui>();
			if (deleg != null)
				deleg(index, type, normalized, value);
		}

		public static void VertexAttribP4ui(uint index, All type, bool normalized, uint value)
		{
			glVertexAttribP4ui deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribP4ui>();
			if (deleg != null)
				deleg(index, type, normalized, value);
		}

		public static void VertexAttribDivisor(uint index, uint divisor)
		{
			glVertexAttribDivisor deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribDivisor>();
			if (deleg != null)
				deleg(index, divisor);
		}

		public static void VertexAttribPointer(uint index, int size, VertexAttribPointerType type, bool normalized, int stride, IntPtr pointer)
		{
			glVertexAttribPointer deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribPointer>();
			if (deleg != null)
				deleg(index, size, type, normalized, stride, pointer);
		}

		public static void VertexAttribIPointer(uint index, int size, VertexAttribIPointerType type, int stride, IntPtr pointer)
		{
			glVertexAttribIPointer deleg = BaseGraphicsContext.Current.Loader.Get<glVertexAttribIPointer>();
			if (deleg != null)
				deleg(index, size, type, stride, pointer);
		}

		public static void Viewport(int x, int y, int width, int height)
		{
			glViewport deleg = BaseGraphicsContext.Current.Loader.Get<glViewport>();
			if (deleg != null)
				deleg(x, y, width, height);
		}

		public static void WaitSync(IntPtr sync, All flags, ulong timeout)
		{
			glWaitSync deleg = BaseGraphicsContext.Current.Loader.Get<glWaitSync>();
			if (deleg != null)
				deleg(sync, flags, timeout);
		}
	}
}