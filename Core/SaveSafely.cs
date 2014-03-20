using System;

namespace DeltaEngine.Core
{
	/// <summary>
	/// If a class is labeled with this attribute, when instances are saved and loaded, if the class
	/// structure has subsequently changed, other classes within the same save will still load 
	/// correctly. Using this attribute adds 4 bytes to the amount of data saved per instance.
	/// </summary>
	public class SaveSafely : Attribute {}
}