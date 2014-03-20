using System.Diagnostics;
using System.Runtime.InteropServices;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Graphics.Vertices
{
	/// <summary>
	/// Skinning vertex data with two joint transform indices and weights.
	/// </summary>
	[DebuggerDisplay("SkinningData({FirstIndex}, {SecondIndex}, {FirstWeight}, {SecondWeight})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct SkinningData : Lerp<SkinningData>
	{
		public SkinningData(int firstIndex, int secondIndex, float firstWeight, float secondWeight)
		{
			FirstIndex = firstIndex;
			SecondIndex = secondIndex;
			FirstWeight = firstWeight;
			SecondWeight = secondWeight;
		}

		public float FirstIndex;
		public float SecondIndex;
		public float FirstWeight;
		public float SecondWeight;

		public SkinningData Lerp(SkinningData other, float interpolation)
		{
			return new SkinningData((int)FirstIndex, (int)SecondIndex,
				FirstWeight.Lerp(other.FirstWeight, interpolation),
				SecondWeight.Lerp(other.SecondWeight, interpolation));
		}
	}
}