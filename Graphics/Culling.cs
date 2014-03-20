namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Defines which mode the graphics device will use for culling. By default culling is on in DE.
	/// </summary>
	public enum Culling
	{
		/// <summary>
		/// Enables clockwise backface culling which is the default behavior in OpenGL. In DirectX it
		/// would be the opposite but we unify all implementations to the OpenGL behavior because
		/// (almost) all mobile platforms are OpenGL based. Front facing triangles are counter clockwise.
		/// OpenGL: http://www.opengl.org/wiki/Face_Culling
		/// DirectX: http://xboxforums.create.msdn.com/forums/p/1469/62375.aspx
		/// XNA: http://msdn.microsoft.com/en-us/library/microsoft.xna.framework.graphics.rasterizerstate.cullmode.aspx
		/// </summary>
		Enabled,
		/// <summary>
		/// No culling is used at all. Backfaces will be visible, should only be used for debugging.
		/// </summary>
		Disabled
	}
}