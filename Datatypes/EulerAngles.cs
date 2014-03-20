using System.Runtime.InteropServices;
using DeltaEngine.Extensions;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Yaw, Pitch and Roll used for 3D rotations. See: http://en.wikipedia.org/wiki/Euler_angles
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct EulerAngles
	{
		public EulerAngles(float pitch, float yaw, float roll)
			: this()
		{
			Pitch = pitch;
			Yaw = yaw;
			Roll = roll;
		}

		public float Pitch { get; set; }
		public float Yaw { get; set; }
		public float Roll { get; set; }

		public static bool operator !=(EulerAngles euler1, EulerAngles euler2)
		{
			return euler1.Equals(euler2) == false;
		}

		public static bool operator ==(EulerAngles euler1, EulerAngles euler2)
		{
			return euler1.Equals(euler2);
		}

		public bool Equals(EulerAngles other)
		{
			return Pitch.IsNearlyEqual(other.Pitch) && Yaw.IsNearlyEqual(other.Yaw) &&
				Roll.IsNearlyEqual(other.Roll);
		}

		public override bool Equals(object other)
		{
			return other is EulerAngles ? Equals((EulerAngles)other) : base.Equals(other);
		}

		public override int GetHashCode()
		{
			return Pitch.GetHashCode() ^ Yaw.GetHashCode() ^ Roll.GetHashCode();
		}

		public override string ToString()
		{
			return "Pitch: " + Pitch + " Yaw: " + Yaw + " Roll: " + Roll;
		}
	}
}
