using System;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Tells the GPU how to render 2D or 3D material data on screen. Usually loaded from content.
	/// This base class has no functionality, some common functionality can be found in
	/// ShaderWithFormat. Provide multiple framework specific shader codes if creating manually.
	/// </summary>
	public abstract class Shader : ContentData, IEquatable<Shader>
	{
		protected Shader(ShaderCreationData data)
			: base("<Generated" + data.Flags + "Shader>")
		{
			Data = data;
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			FillShaderCode();
		}

		protected ShaderCreationData Data { get; set; }
		public ShaderFlags Flags
		{
			get { return Data.Flags; }
		}

		protected override void LoadData(Stream fileData)
		{
			Data = new BinaryReader(fileData).Create() as ShaderCreationData;
			FillShaderCode();
			TryCreateShader();
		}

		protected abstract void FillShaderCode();

		protected void TryCreateShader()
		{
			try
			{
				CreateShader();
			}
			catch (Exception ex)
			{
				string reason = String.IsNullOrEmpty(ex.Message) ? ex.ToString() : ex.Message;
				throw new ShaderCreationHasFailed(reason);
			}
		}
		protected abstract void CreateShader();

		public class ShaderCreationHasFailed : Exception
		{
			public ShaderCreationHasFailed(string error)
				: base(error) {}
		}

		public abstract void SetModelViewProjection(Matrix model, Matrix view, Matrix projection);
		public abstract void SetModelViewProjection(Matrix matrix);
		public abstract void SetJointMatrices(Matrix[] jointMatrices);
		public abstract void SetDiffuseTexture(Image texture);
		public abstract void SetLightmapTexture(Image texture);
		public abstract void SetSunLight(SunLight light);
		public abstract void ApplyFogSettings(FogSettings fogSettings);
		public abstract void Bind();
		public abstract void BindVertexDeclaration();

		public override bool Equals(object other)
		{
			return other is Shader && Equals((Shader)other);
		}

		public bool Equals(Shader other)
		{
			return other != null && Flags == other.Flags;
		}

		// ncrunch: no coverage start
		public override int GetHashCode()
		{
			return (Data != null ? Data.GetHashCode() : 0);
		}
	}
}