using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering3D.Cameras
{
	public class CameraPath
	{
		public CameraPath(Matrix[] viewMatrices)
		{
			ViewMatrices = viewMatrices;
		}

		public Matrix[] ViewMatrices;
	}
}
