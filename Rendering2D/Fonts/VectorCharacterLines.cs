using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Fonts
{
	/// <summary>
	/// Points for the VectorText lines for each character supported.
	/// </summary>
	internal class VectorCharacterLines
	{
		public VectorCharacterLines()
		{
			AddNumbers();
			AddLetters();
			AddPoint();
		}

		internal readonly Dictionary<char, Vector2D[]> linePoints = new Dictionary<char, Vector2D[]>();

		private void AddNumbers()
		{
			linePoints.Add('0',
				new[]
				{
					new Vector2D(0.128f, 0), new Vector2D(0.384f, 0), new Vector2D(0.384f, 0),
					new Vector2D(0.512f, 0.114285f), new Vector2D(0.512f, 0.114285f),
					new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0.571425f),
					new Vector2D(0.384f, 0.68571f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.128f, 0.68571f), new Vector2D(0.128f, 0.68571f), new Vector2D(0, 0.571425f)
					, new Vector2D(0, 0.571425f), new Vector2D(0, 0.114285f), new Vector2D(0, 0.114285f),
					new Vector2D(0.128f, 0)
				});
			linePoints.Add('1', new[] { new Vector2D(0.256f, 0), new Vector2D(0.256f, 0.68571f) });
			linePoints.Add('2',
				new[]
				{
					new Vector2D(0, 0.114285f), new Vector2D(0.128f, 0), new Vector2D(0.128f, 0),
					new Vector2D(0.384f, 0), new Vector2D(0.384f, 0), new Vector2D(0.512f, 0.114285f),
					new Vector2D(0.512f, 0.114285f), new Vector2D(0.512f, 0.22857f),
					new Vector2D(0.512f, 0.22857f), new Vector2D(0, 0.68571f), new Vector2D(0, 0.68571f),
					new Vector2D(0.512f, 0.68571f)
				});
			linePoints.Add('3',
				new[]
				{
					new Vector2D(0, 0.114285f), new Vector2D(0.128f, 0), new Vector2D(0.128f, 0),
					new Vector2D(0.384f, 0), new Vector2D(0.384f, 0), new Vector2D(0.512f, 0.114285f),
					new Vector2D(0.512f, 0.114285f), new Vector2D(0.512f, 0.22857f),
					new Vector2D(0.512f, 0.22857f), new Vector2D(0.384f, 0.342855f),
					new Vector2D(0.384f, 0.342855f), new Vector2D(0, 0.342855f), new Vector2D(0, 0.342855f),
					new Vector2D(0.384f, 0.342855f), new Vector2D(0.384f, 0.342855f),
					new Vector2D(0.512f, 0.45714f), new Vector2D(0.512f, 0.45714f),
					new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0.571425f),
					new Vector2D(0.384f, 0.68571f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.128f, 0.68571f), new Vector2D(0.128f, 0.68571f), new Vector2D(0, 0.571425f)
				});
			linePoints.Add('4',
				new[]
				{
					new Vector2D(0.512f, 0.342855f), new Vector2D(0, 0.342855f), new Vector2D(0, 0.342855f),
					new Vector2D(0.384f, 0), new Vector2D(0.384f, 0), new Vector2D(0.384f, 0.68571f)
				});
			linePoints.Add('5',
				new[]
				{
					new Vector2D(0.512f, 0), new Vector2D(0, 0), new Vector2D(0, 0),
					new Vector2D(0, 0.342855f), new Vector2D(0, 0.342855f), new Vector2D(0.384f, 0.342855f),
					new Vector2D(0.384f, 0.342855f), new Vector2D(0.512f, 0.45714f),
					new Vector2D(0.512f, 0.45714f), new Vector2D(0.512f, 0.571425f),
					new Vector2D(0.512f, 0.571425f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.384f, 0.68571f), new Vector2D(0, 0.68571f)
				});
			linePoints.Add('6',
				new[]
				{
					new Vector2D(0.512f, 0), new Vector2D(0.128f, 0), new Vector2D(0.128f, 0),
					new Vector2D(0, 0.114285f), new Vector2D(0, 0.114285f), new Vector2D(0, 0.571425f),
					new Vector2D(0, 0.571425f), new Vector2D(0.128f, 0.68571f), new Vector2D(0.128f, 0.68571f)
					, new Vector2D(0.384f, 0.68571f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0.571425f),
					new Vector2D(0.512f, 0.45714f), new Vector2D(0.512f, 0.45714f),
					new Vector2D(0.384f, 0.342855f), new Vector2D(0.384f, 0.342855f),
					new Vector2D(0, 0.342855f)
				});
			linePoints.Add('7',
				new[]
				{
					new Vector2D(0, 0), new Vector2D(0.512f, 0), new Vector2D(0.512f, 0),
					new Vector2D(0, 0.68571f)
				});
			linePoints.Add('8',
				new[]
				{
					new Vector2D(0.128f, 0), new Vector2D(0.384f, 0), new Vector2D(0.384f, 0),
					new Vector2D(0.512f, 0.114285f), new Vector2D(0.512f, 0.114285f),
					new Vector2D(0.512f, 0.22857f), new Vector2D(0.512f, 0.22857f),
					new Vector2D(0.384f, 0.342855f), new Vector2D(0.384f, 0.342855f),
					new Vector2D(0.128f, 0.342855f), new Vector2D(0.128f, 0.342855f),
					new Vector2D(0, 0.45714f), new Vector2D(0, 0.45714f), new Vector2D(0, 0.571425f),
					new Vector2D(0, 0.571425f), new Vector2D(0.128f, 0.68571f), new Vector2D(0.128f, 0.68571f)
					, new Vector2D(0.384f, 0.68571f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0.571425f),
					new Vector2D(0.512f, 0.45714f), new Vector2D(0.512f, 0.45714f),
					new Vector2D(0.384f, 0.342855f), new Vector2D(0.384f, 0.342855f),
					new Vector2D(0.128f, 0.342855f), new Vector2D(0.128f, 0.342855f),
					new Vector2D(0, 0.22857f), new Vector2D(0, 0.22857f), new Vector2D(0, 0.114285f),
					new Vector2D(0, 0.114285f), new Vector2D(0.128f, 0)
				});
			linePoints.Add('9',
				new[]
				{
					new Vector2D(0, 0.68571f), new Vector2D(0.384f, 0.68571f), new Vector2D(0.384f, 0.68571f)
					, new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0.571425f),
					new Vector2D(0.512f, 0.114285f), new Vector2D(0.512f, 0.114285f), new Vector2D(0.384f, 0),
					new Vector2D(0.384f, 0), new Vector2D(0.128f, 0), new Vector2D(0.128f, 0),
					new Vector2D(0, 0.114285f), new Vector2D(0, 0.114285f), new Vector2D(0, 0.22857f),
					new Vector2D(0, 0.22857f), new Vector2D(0.128f, 0.342855f),
					new Vector2D(0.128f, 0.342855f), new Vector2D(0.512f, 0.342855f)
				});
		}

		public void AddLetters()
		{
			linePoints.Add('A',
				new[]
				{
					new Vector2D(0, 0.68571f), new Vector2D(0, 0.114285f), new Vector2D(0, 0.114285f),
					new Vector2D(0.128f, 0), new Vector2D(0.128f, 0), new Vector2D(0.384f, 0),
					new Vector2D(0.384f, 0), new Vector2D(0.512f, 0.114285f), new Vector2D(0.512f, 0.114285f),
					new Vector2D(0.512f, 0.68571f), new Vector2D(0.512f, 0.68571f),
					new Vector2D(0.512f, 0.342855f), new Vector2D(0.512f, 0.342855f),
					new Vector2D(0, 0.342855f)
				});
			linePoints.Add('B',
				new[]
				{
					new Vector2D(0, 0.342855f), new Vector2D(0, 0), new Vector2D(0, 0),
					new Vector2D(0.384f, 0), new Vector2D(0.384f, 0), new Vector2D(0.512f, 0.114285f),
					new Vector2D(0.512f, 0.114285f), new Vector2D(0.512f, 0.22857f),
					new Vector2D(0.512f, 0.22857f), new Vector2D(0.384f, 0.342855f),
					new Vector2D(0.384f, 0.342855f), new Vector2D(0, 0.342855f), new Vector2D(0, 0.342855f),
					new Vector2D(0, 0.68571f), new Vector2D(0, 0.68571f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.384f, 0.68571f), new Vector2D(0.512f, 0.571425f),
					new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0.45714f),
					new Vector2D(0.512f, 0.45714f), new Vector2D(0.384f, 0.342855f)
				});
			linePoints.Add('C',
				new[]
				{
					new Vector2D(0.512f, 0), new Vector2D(0.128f, 0), new Vector2D(0.128f, 0),
					new Vector2D(0, 0.114285f), new Vector2D(0, 0.114285f), new Vector2D(0, 0.571425f),
					new Vector2D(0, 0.571425f), new Vector2D(0.128f, 0.68571f), new Vector2D(0.128f, 0.68571f)
					, new Vector2D(0.512f, 0.68571f)
				});
			linePoints.Add('D',
				new[]
				{
					new Vector2D(0, 0.68571f), new Vector2D(0, 0), new Vector2D(0, 0),
					new Vector2D(0.384f, 0), new Vector2D(0.384f, 0), new Vector2D(0.512f, 0.114285f),
					new Vector2D(0.512f, 0.114285f), new Vector2D(0.512f, 0.571425f),
					new Vector2D(0.512f, 0.571425f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.384f, 0.68571f), new Vector2D(0, 0.68571f)
				});
			linePoints.Add('E',
				new[]
				{
					new Vector2D(0.512f, 0), new Vector2D(0, 0), new Vector2D(0, 0),
					new Vector2D(0, 0.342855f), new Vector2D(0, 0.342855f), new Vector2D(0.512f, 0.342855f),
					new Vector2D(0.512f, 0.342855f), new Vector2D(0, 0.342855f), new Vector2D(0, 0.342855f),
					new Vector2D(0, 0.68571f), new Vector2D(0, 0.68571f), new Vector2D(0.512f, 0.68571f)
				});
			linePoints.Add('F',
				new[]
				{
					new Vector2D(0.512f, 0), new Vector2D(0, 0), new Vector2D(0, 0),
					new Vector2D(0, 0.342855f), new Vector2D(0, 0.342855f), new Vector2D(0.512f, 0.342855f),
					new Vector2D(0.512f, 0.342855f), new Vector2D(0, 0.342855f), new Vector2D(0, 0.342855f),
					new Vector2D(0, 0.68571f)
				});
			linePoints.Add('G',
				new[]
				{
					new Vector2D(0.512f, 0), new Vector2D(0.128f, 0), new Vector2D(0.128f, 0),
					new Vector2D(0, 0.114285f), new Vector2D(0, 0.114285f), new Vector2D(0, 0.571425f),
					new Vector2D(0, 0.571425f), new Vector2D(0.128f, 0.68571f), new Vector2D(0.128f, 0.68571f)
					, new Vector2D(0.384f, 0.68571f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0.571425f),
					new Vector2D(0.512f, 0.342855f), new Vector2D(0.512f, 0.342855f),
					new Vector2D(0.256f, 0.342855f)
				});
			linePoints.Add('H',
				new[]
				{
					new Vector2D(0, 0), new Vector2D(0, 0.68571f), new Vector2D(0, 0.68571f),
					new Vector2D(0, 0.342855f), new Vector2D(0, 0.342855f), new Vector2D(0.512f, 0.342855f),
					new Vector2D(0.512f, 0.342855f), new Vector2D(0.512f, 0), new Vector2D(0.512f, 0),
					new Vector2D(0.512f, 0.68571f)
				});
			linePoints.Add('I',
				new[]
				{
					new Vector2D(0.128f, 0), new Vector2D(0.384f, 0), new Vector2D(0.384f, 0),
					new Vector2D(0.256f, 0), new Vector2D(0.256f, 0), new Vector2D(0.256f, 0.68571f),
					new Vector2D(0.256f, 0.68571f), new Vector2D(0.128f, 0.68571f),
					new Vector2D(0.128f, 0.68571f), new Vector2D(0.384f, 0.68571f)
				});
			linePoints.Add('J',
				new[]
				{
					new Vector2D(0.512f, 0), new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0.571425f)
					, new Vector2D(0.384f, 0.68571f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.128f, 0.68571f), new Vector2D(0.128f, 0.68571f), new Vector2D(0, 0.571425f)
				});
			linePoints.Add('K',
				new[]
				{
					new Vector2D(0, 0), new Vector2D(0, 0.68571f), new Vector2D(0, 0.68571f),
					new Vector2D(0, 0.342855f), new Vector2D(0, 0.342855f), new Vector2D(0.512f, 0),
					new Vector2D(0.512f, 0), new Vector2D(0, 0.342855f), new Vector2D(0, 0.342855f),
					new Vector2D(0.512f, 0.68571f)
				});
			linePoints.Add('L',
				new[]
				{
					new Vector2D(0, 0), new Vector2D(0, 0.68571f), new Vector2D(0, 0.68571f),
					new Vector2D(0.512f, 0.68571f)
				});
			linePoints.Add('M',
				new[]
				{
					new Vector2D(0, 0.68571f), new Vector2D(0, 0), new Vector2D(0, 0),
					new Vector2D(0.256f, 0.22857f), new Vector2D(0.256f, 0.22857f), new Vector2D(0.512f, 0),
					new Vector2D(0.512f, 0), new Vector2D(0.512f, 0.68571f)
				});
			linePoints.Add('N',
				new[]
				{
					new Vector2D(0, 0.68571f), new Vector2D(0, 0), new Vector2D(0, 0),
					new Vector2D(0.512f, 0.68571f), new Vector2D(0.512f, 0.68571f), new Vector2D(0.512f, 0)
				});
			linePoints.Add('O',
				new[]
				{
					new Vector2D(0.128f, 0), new Vector2D(0.384f, 0), new Vector2D(0.384f, 0),
					new Vector2D(0.512f, 0.114285f), new Vector2D(0.512f, 0.114285f),
					new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0.571425f),
					new Vector2D(0.384f, 0.68571f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.128f, 0.68571f), new Vector2D(0.128f, 0.68571f), new Vector2D(0, 0.571425f)
					, new Vector2D(0, 0.571425f), new Vector2D(0, 0.114285f), new Vector2D(0, 0.114285f),
					new Vector2D(0.128f, 0)
				});
			linePoints.Add('P',
				new[]
				{
					new Vector2D(0, 0.68571f), new Vector2D(0, 0), new Vector2D(0, 0),
					new Vector2D(0.384f, 0), new Vector2D(0.384f, 0), new Vector2D(0.512f, 0.114285f),
					new Vector2D(0.512f, 0.114285f), new Vector2D(0.512f, 0.22857f),
					new Vector2D(0.512f, 0.22857f), new Vector2D(0.384f, 0.342855f),
					new Vector2D(0.384f, 0.342855f), new Vector2D(0, 0.342855f)
				});
			linePoints.Add('Q',
				new[]
				{
					new Vector2D(0.384f, 0.68571f), new Vector2D(0.128f, 0.68571f),
					new Vector2D(0.128f, 0.68571f), new Vector2D(0, 0.571425f), new Vector2D(0, 0.571425f),
					new Vector2D(0, 0.114285f), new Vector2D(0, 0.114285f), new Vector2D(0.128f, 0),
					new Vector2D(0.128f, 0), new Vector2D(0.384f, 0), new Vector2D(0.384f, 0),
					new Vector2D(0.512f, 0.114285f), new Vector2D(0.512f, 0.114285f),
					new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0.571425f),
					new Vector2D(0.384f, 0.68571f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.512f, 0.799995f)
				});
			linePoints.Add('R',
				new[]
				{
					new Vector2D(0, 0.68571f), new Vector2D(0, 0), new Vector2D(0, 0),
					new Vector2D(0.384f, 0), new Vector2D(0.384f, 0), new Vector2D(0.512f, 0.114285f),
					new Vector2D(0.512f, 0.114285f), new Vector2D(0.512f, 0.22857f),
					new Vector2D(0.512f, 0.22857f), new Vector2D(0.384f, 0.342855f),
					new Vector2D(0.384f, 0.342855f), new Vector2D(0, 0.342855f), new Vector2D(0, 0.342855f),
					new Vector2D(0.512f, 0.68571f)
				});
			linePoints.Add('S',
				new[]
				{
					new Vector2D(0, 0.68571f), new Vector2D(0.384f, 0.68571f), new Vector2D(0.384f, 0.68571f)
					, new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0.571425f),
					new Vector2D(0.512f, 0.45714f), new Vector2D(0.512f, 0.45714f),
					new Vector2D(0.384f, 0.342855f), new Vector2D(0.384f, 0.342855f),
					new Vector2D(0.128f, 0.342855f), new Vector2D(0.128f, 0.342855f),
					new Vector2D(0, 0.22857f), new Vector2D(0, 0.22857f), new Vector2D(0, 0.114285f),
					new Vector2D(0, 0.114285f), new Vector2D(0.128f, 0), new Vector2D(0.128f, 0),
					new Vector2D(0.512f, 0)
				});
			linePoints.Add('T',
				new[]
				{
					new Vector2D(0, 0), new Vector2D(0.512f, 0), new Vector2D(0.512f, 0),
					new Vector2D(0.256f, 0), new Vector2D(0.256f, 0), new Vector2D(0.256f, 0.68571f)
				});
			linePoints.Add('U',
				new[]
				{
					new Vector2D(0, 0), new Vector2D(0, 0.571425f), new Vector2D(0, 0.571425f),
					new Vector2D(0.128f, 0.68571f), new Vector2D(0.128f, 0.68571f),
					new Vector2D(0.384f, 0.68571f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0)
				});
			linePoints.Add('V',
				new[]
				{
					new Vector2D(0, 0), new Vector2D(0, 0.45714f), new Vector2D(0, 0.45714f),
					new Vector2D(0.256f, 0.68571f), new Vector2D(0.256f, 0.68571f),
					new Vector2D(0.512f, 0.45714f), new Vector2D(0.512f, 0.45714f), new Vector2D(0.512f, 0)
				});
			linePoints.Add('W',
				new[]
				{
					new Vector2D(0, 0), new Vector2D(0, 0.571425f), new Vector2D(0, 0.571425f),
					new Vector2D(0.128f, 0.68571f), new Vector2D(0.128f, 0.68571f),
					new Vector2D(0.256f, 0.571425f), new Vector2D(0.256f, 0.571425f),
					new Vector2D(0.384f, 0.68571f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0.571425f), new Vector2D(0.512f, 0)
				});
			linePoints.Add('X',
				new[]
				{
					new Vector2D(0, 0), new Vector2D(0.512f, 0.68571f), new Vector2D(0.512f, 0.68571f),
					new Vector2D(0.256f, 0.342855f), new Vector2D(0.256f, 0.342855f),
					new Vector2D(0, 0.68571f), new Vector2D(0, 0.68571f), new Vector2D(0.512f, 0)
				});
			linePoints.Add('Y',
				new[]
				{
					new Vector2D(0, 0), new Vector2D(0.256f, 0.22857f), new Vector2D(0.256f, 0.22857f),
					new Vector2D(0.512f, 0), new Vector2D(0.512f, 0), new Vector2D(0.256f, 0.22857f),
					new Vector2D(0.256f, 0.22857f), new Vector2D(0.256f, 0.68571f)
				});
			linePoints.Add('Z',
				new[]
				{
					new Vector2D(0, 0), new Vector2D(0.512f, 0), new Vector2D(0.512f, 0),
					new Vector2D(0, 0.68571f), new Vector2D(0, 0.68571f), new Vector2D(0.512f, 0.68571f)
				});
		}

		private void AddPoint()
		{
			linePoints.Add('.',
				new[]
				{
					new Vector2D(0.256f, 0.571425f), new Vector2D(0.384f, 0.571425f),
					new Vector2D(0.384f, 0.571425f), new Vector2D(0.384f, 0.68571f),
					new Vector2D(0.384f, 0.68571f), new Vector2D(0.256f, 0.68571f),
					new Vector2D(0.256f, 0.68571f), new Vector2D(0.256f, 0.571425f)
				});
		}

		public Vector2D[] GetPoints(char c)
		{
			c = char.ToUpperInvariant(c);
			Vector2D[] points;
			return linePoints.TryGetValue(c, out points) ? points : new Vector2D[0];
		}
	}
}