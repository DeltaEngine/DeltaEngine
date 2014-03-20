using NUnit.Framework;

namespace FractalZoomer.Tests
{
	public class FractalTests
	{
		[SetUp]
		public void CreateFractal()
		{
			fractal = new Fractal();
		}

		private Fractal fractal;

		[Test]
		public void AnythingOutsideTheRadiusTwoHasNoIterations()
		{
			Assert.AreEqual(0, fractal.GetColorIndexFromIterationsNeeded(3, 0));
			Assert.AreEqual(0, fractal.GetColorIndexFromIterationsNeeded(-2, 0));
			Assert.AreEqual(0, fractal.GetColorIndexFromIterationsNeeded(0, 2));
			Assert.AreEqual(0, fractal.GetColorIndexFromIterationsNeeded(0, -100));
		}

		[Test]
		public void MostNumbersCloseToTheRadiusOfTwoHaveLowValues()
		{
			Assert.AreEqual(7, fractal.GetColorIndexFromIterationsNeeded(1.977371f, 0.3f));
			Assert.AreEqual(28, fractal.GetColorIndexFromIterationsNeeded(-1.8f, 0.7f));
			Assert.AreEqual(11, fractal.GetColorIndexFromIterationsNeeded(0, 1.9f));
			Assert.AreEqual(17, fractal.GetColorIndexFromIterationsNeeded(0, -1.7f));
			Assert.AreEqual(19, fractal.GetColorIndexFromIterationsNeeded(-1.54f, 1.2f));
			Assert.AreEqual(24, fractal.GetColorIndexFromIterationsNeeded(-1.4f, -1.1f));
			Assert.AreEqual(17, fractal.GetColorIndexFromIterationsNeeded(1f, 1f));
		}

		[Test]
		public void SomeNumbersInsideTheRadiusHaveHigherValues()
		{
			Assert.AreEqual(37, fractal.GetColorIndexFromIterationsNeeded(-1f, 1f));
			Assert.AreEqual(32, fractal.GetColorIndexFromIterationsNeeded(0.9f, 0.0f));
		}

		[TestCase(109, 0.1f, 0.8f)]
		[TestCase(240, 0.3f, 0.0f)]
		[TestCase(162, -0.25f, 0.759f)]
		[TestCase(0, 0.2f, 0.0f)]
		[TestCase(0, -0.8f, 0.0f)]
		public void SomeMoreChecksToMakeSureOptimizationsWorkJustFine(int iterations, float real,
			float imaginary)
		{
			Assert.AreEqual(iterations, fractal.GetColorIndexFromIterationsNeeded(real, imaginary));
		}

		[Test]
		public void NumberOfIterationsIsAlwaysTheSameForNegativeAndPositiveImaginaryValues()
		{
			Assert.AreEqual(fractal.GetColorIndexFromIterationsNeeded(-1, 1),
				fractal.GetColorIndexFromIterationsNeeded(-1, -1));
			Assert.AreEqual(fractal.GetColorIndexFromIterationsNeeded(-2, 1),
				fractal.GetColorIndexFromIterationsNeeded(-2, -1));
			Assert.AreEqual(fractal.GetColorIndexFromIterationsNeeded(-1.5f, 1.1f),
				fractal.GetColorIndexFromIterationsNeeded(-1.5f, -1.1f));
		}
	}
}