using System;
using System.Collections.Generic;

namespace DeltaEngine.Editor.Core
{
	/// <summary>
	/// Provides mapping to internal names and references
	/// </summary>
	public static class DeltaEngineFrameworkExtensions
	{
		static DeltaEngineFrameworkExtensions()
		{
			FrameworkNames = CreateFrameworkNames("Windows");
			FrameworkShortNames = CreateFrameworkNames("");
		}

		private static readonly Dictionary<DeltaEngineFramework, string> FrameworkNames;

		private static Dictionary<DeltaEngineFramework, string> CreateFrameworkNames(string prefix)
		{
			return new Dictionary<DeltaEngineFramework, string>
			{
				{ DeltaEngineFramework.GLFW, prefix + "GLFW3" },
				{ DeltaEngineFramework.MonoGame, prefix + "MonoGame" },
				{ DeltaEngineFramework.OpenTK, prefix + "OpenTK20" },
				{ DeltaEngineFramework.SharpDX, prefix + "SharpDX" },
				{ DeltaEngineFramework.SlimDX, prefix + "SlimDX" },
				{ DeltaEngineFramework.Xna, prefix + "Xna" }
			};
		}

		private static readonly Dictionary<DeltaEngineFramework, string> FrameworkShortNames;

		public static string ToInternalName(this DeltaEngineFramework framework)
		{
			if (framework == DeltaEngineFramework.Default)
				throw new ArgumentException(framework.ToString());
			return FrameworkNames[framework];
		}

		public static string ToInternalShortName(this DeltaEngineFramework framework)
		{
			if (framework == DeltaEngineFramework.Default)
				throw new ArgumentException(framework.ToString());
			return FrameworkShortNames[framework];
		}

		public static DeltaEngineFramework FromString(string frameworkName)
		{
			DeltaEngineFramework result;
			return Enum.TryParse(frameworkName, true, out result)
				? result : DeltaEngineFramework.Default;
		}
	}
}