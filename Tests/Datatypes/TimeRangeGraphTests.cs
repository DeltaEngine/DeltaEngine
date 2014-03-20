using System.Collections.Generic;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	internal class TimeRangeGraphTests
	{
		[Test]
		public void SettingDefaultPercentagesForNumberOfValues()
		{
			var points =
				new List<Vector2D>(new[]
				{ Vector2D.Zero, Vector2D.UnitX, Vector2D.UnitY, Vector2D.UnitY, Vector2D.One });
			var pointsTimeRange = new TimeRangeGraph<Vector2D>(points);
			Assert.AreEqual(new[] { 0.0f, 0.25f, 0.5f, 0.75f, 1.0f }, pointsTimeRange.Percentages);
			Assert.AreEqual(points.ToArray(), pointsTimeRange.Values);
		}

		[Test]
		public void SetDefaultPercentagesWithJustStartAndEnd()
		{
			var vectorTimeRange = new TimeRangeGraph<Vector3D>();
			var colors = new[] { Color.Black, Color.Green };
			var colorTimeRange = new TimeRangeGraph<Color>(colors[0], colors[1]);
			Assert.AreEqual(new[] { 0.0f, 1.0f }, vectorTimeRange.Percentages);
			Assert.AreEqual(new[] { 0.0f, 1.0f }, colorTimeRange.Percentages);
			Assert.AreEqual(colors, colorTimeRange.Values);
		}

		[Test]
		public void GetInterpolationValue()
		{
			var points =
				new List<Vector2D>(new[] { Vector2D.UnitX, Vector2D.Zero, Vector2D.One, Vector2D.UnitY });
			var pointsTimeRange = new TimeRangeGraph<Vector2D>(points);
			Assert.IsTrue(
				pointsTimeRange.TrySetAllPercentagesNoOrderChange(
					new List<float>(new[] { 0.0f, 0.2f, 0.7f, 1.0f })));
			const float ExpectedInterpolation = 4.0f / 5.0f;
			Assert.IsTrue(
				pointsTimeRange.GetInterpolatedValue(0.6f).IsNearlyEqual(Vector2D.One *
					ExpectedInterpolation));
			Assert.AreEqual(Vector2D.UnitY, pointsTimeRange.GetInterpolatedValue(1.2f));
			Assert.AreEqual(Vector2D.UnitX, pointsTimeRange.GetInterpolatedValue(-0.1f));
		}

		[Test]
		public void CannotSetPercentagesToInconsistentValues()
		{
			var colors = new List<Color>(new[] { Color.Orange, Color.PaleGreen, Color.Gold });
			var colorsTimeRange = new TimeRangeGraph<Color>(colors);
			Assert.IsFalse(
				colorsTimeRange.TrySetAllPercentagesNoOrderChange(new List<float>(new[] { 0.0f })));
			Assert.IsFalse(
				colorsTimeRange.TrySetAllPercentagesNoOrderChange(
					new List<float>(new[] { 0.1f, 1.0f, 0.2f })));
		}

		[Test]
		public void TrySettingPercentagesWithoutOrderChange()
		{
			var colors =
				new List<Color>(new[] { Color.Orange, Color.PaleGreen, Color.Gold, Color.Purple });
			var colorsTimeRange = new TimeRangeGraph<Color>(colors);
			Assert.IsFalse(colorsTimeRange.TrySetPercentageNoOrderChange(1, 0.0f));
			Assert.IsFalse(colorsTimeRange.TrySetPercentageNoOrderChange(1, 0.8f));
			Assert.IsFalse(colorsTimeRange.TrySetPercentageNoOrderChange(0, 0.1f));
			Assert.IsTrue(colorsTimeRange.TrySetPercentageNoOrderChange(1, 0.2f));
			Assert.IsTrue(colorsTimeRange.TrySetPercentageNoOrderChange(2, 0.9f));
		}

		[Test]
		public void SetValue()
		{
			var colors = CreateColorsList();
			var colorsTimeRange = new TimeRangeGraph<Color>(colors);
			colorsTimeRange.SetValueAt(2.0f / 3.0f, Color.Green);
			Assert.AreEqual(Color.Green, colorsTimeRange.Values[2]);
		}

		private static List<Color> CreateColorsList()
		{
			return new List<Color>(new[] { Color.Orange, Color.PaleGreen, Color.Gold, Color.Purple });
		}

		[Test]
		public void AddValueAtPercentage()
		{
			var colors = CreateColorsList();
			var colorsTimeRange = new TimeRangeGraph<Color>(colors);
			colorsTimeRange.AddValueAt(0.9f, Color.Black);
			var expectedColors = new[] { colors[0], colors[1], colors[2], Color.Black, colors[3] };
			Assert.AreEqual(expectedColors, colorsTimeRange.Values);
			Assert.AreEqual(Color.Black, colorsTimeRange.GetInterpolatedValue(0.9f));
		}

		[Test]
		public void AddValueBySettingAtInexistantPercentage()
		{
			var colors = CreateColorsList();
			var colorsTimeRange = new TimeRangeGraph<Color>(colors);
			colorsTimeRange.SetValueAt(0.2f, Color.Green);
			Assert.AreEqual(5, colorsTimeRange.Values.Length);
		}

		[Test]
		public void CannotAddOrSetValueOutsideScope()
		{
			var colorsTimeRange = new TimeRangeGraph<Color>();
			Assert.Throws<TimeRangeGraph<Color>.PercentageOutsideScope>(
				() => { colorsTimeRange.AddValueAt(1.2f, Color.Green); });
			Assert.Throws<TimeRangeGraph<Color>.PercentageOutsideScope>(
				() => { colorsTimeRange.AddValueAt(-0.2f, Color.Black); });
		}

		[Test]
		public void TryingToSetValueWithInvalidIndexHasNotEffect()
		{
			var colors = CreateColorsList();
			var colorsTimeRange = new TimeRangeGraph<Color>(colors);
			Assert.AreEqual(4, colorsTimeRange.Values.Length);
			colorsTimeRange.SetValue(100, Color.Green);
			Assert.AreEqual(4, colorsTimeRange.Values.Length);
		}

		[Test]
		public void ConvertToStringAndReverse()
		{
			var points =
				new List<Vector2D>(new[]
				{ Vector2D.Zero, Vector2D.UnitX, Vector2D.UnitY, Vector2D.UnitY, Vector2D.One });
			var pointsTimeRange = new TimeRangeGraph<Vector2D>(points);
			var stringRange = pointsTimeRange.ToString();
			var retrievedTimeRange = new TimeRangeGraph<Vector2D>(stringRange);
			Assert.AreEqual("(0: {0, 0}, 0.25: {1, 0}, 0.5: {0, 1}, 0.75: {0, 1}, 1: {1, 1})",
				stringRange);
			Assert.AreEqual(pointsTimeRange.Values, retrievedTimeRange.Values);
			Assert.AreEqual(pointsTimeRange.Percentages, retrievedTimeRange.Percentages);
		}

		[Test]
		public void TryingToCreateFromInvalidStringThrows()
		{
			Assert.Throws<Range<Color>.InvalidStringFormat>(
				() => { new TimeRangeGraph<Color>("({123 ; 458})"); });
			Assert.Throws<Range<Color>.InvalidStringFormat>(() => { new TimeRangeGraph<Color>("asdf"); });
			Assert.Throws<Range<Color>.TypeInStringNotEqualToInitializedType>(
				() =>
				{
					new TimeRangeGraph<Color>(
						"(0: {0, 0}, 0.25: {1, 0}, 0.5: {0, 1}, 0.75: {0, 1}, 1: {1, 1})");
				});
		}
	}
}