using System;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Must be used when a method should be visible and executable from the Console.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class ConsoleCommandAttribute : Attribute
	{
		public ConsoleCommandAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; protected set; }
	}
}