using System;

namespace FractalZoomer
{
	/// <summary>
	/// Calculates mandelbrot fractal values with doubles (very fast in C# with some optimizations).
	/// Should be called in parallel. Obviously keeping the maxIterations low yields in the fastest
	/// results, but the fractals become more interesting and zoom-able if high values are used.
	/// </summary>
	public class Fractal
	{
		public Fractal(int maxIterations = 128, int smoothColorIndices = 1024, double colorScale = 48)
		{
			this.maxIterations = maxIterations;
			this.smoothColorIndices = smoothColorIndices;
			this.colorScale = colorScale;
			PreCalculateMuTable();
		}

		private readonly int maxIterations;
		private readonly int smoothColorIndices;
		private readonly double colorScale;
		/// <summary>
		/// To get smooth gradients as return values: http://linas.org/art-gallery/escape/smooth.html
		/// </summary>
		private void PreCalculateMuTable()
		{
			preCalculatedMu = new double[PreCalculatedLength];
			for (int i = 0; i < PreCalculatedLength; i++)
			{
				var input = MinInput + i * MaxInput / PreCalculatedLength;
				var modulus = Math.Sqrt(input);
				preCalculatedMu[i] = Math.Log(Math.Log(modulus)) / log2;
			}
		}
		private const int PreCalculatedLength = 1024;
		private const double MinInput = 1.001;
		private const double MaxInput = 512;
		private double[] preCalculatedMu;
		private readonly double log2 = Math.Log(2);

		public int GetColorIndexFromIterationsNeeded(double real, double imaginary)
		{
			return AbortIfInBulbThatDoesNotEscape(real, imaginary)
				? 0 : UseEscapeTimeAlgorithm(real, imaginary);
		}

		/// <summary>
		/// Optimization tricks from http://en.wikipedia.org/wiki/Mandelbrot_set#Optimizations
		/// </summary>
		private static bool AbortIfInBulbThatDoesNotEscape(double real, double imaginary)
		{
			double cardioidPosition = real - 0.25;
			double cardioid = cardioidPosition * cardioidPosition + imaginary * imaginary;
			double escape = cardioid * (cardioid + cardioidPosition);
			if (escape < 0.25 * imaginary * imaginary)
				return true;
			double realPlusOne = real + 1;
			return realPlusOne * realPlusOne + imaginary * imaginary < 1 / 16.0;
		}

		/// <summary>
		/// From http://en.wikipedia.org/wiki/Mandelbrot_set#Escape_time_algorithm
		/// This is basically just z = number and an optimized while loop with z = z * z + number
		/// </summary>
		private int UseEscapeTimeAlgorithm(double real, double imaginary)
		{
			double zr = real;
			double zi = imaginary;
			int iteration = 0;
			while (zr * zr + zi * zi < 4 && iteration < maxIterations)
			{
				var temp = zr * zr - zi * zi + real;
				zi = 2 * zr * zi + imaginary;
				zr = temp;
				iteration++;
			}
			if (iteration == 0 || iteration >= maxIterations)
				return 0;
			var extraIterationForSmootherColors = zr * zr - zi * zi + real;
			zi = 2 * zr * zi + imaginary;
			zr = extraIterationForSmootherColors;
			iteration++;
			var input = zr * zr + zi * zi - MinInput;
			if (input >= MaxInput-0.1)
				input = MaxInput-0.1;
			double mu = iteration - preCalculatedMu[(int)(input * PreCalculatedLength / MaxInput)];
			return (int)(mu * smoothColorIndices / colorScale) % smoothColorIndices;
		}
	}
}