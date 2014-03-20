using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class SelectBoxTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			selectBox = new SelectBox(Center, Values);
		}

		private SelectBox selectBox;

		private static readonly Rectangle Center = new Rectangle(0.4f, 0.3f, 0.2f, 0.05f);
		private static readonly List<object> Values = new List<object>
		{
			"value 1",
			"value 2",
			"value 3"
		};

		[Test]
		public void RenderSelectBoxWithThreeValuesAndThreeLines()
		{
			var text = new FontText(Font.Default, "", new Rectangle(0.4f, 0.7f, 0.2f, 0.1f));
			selectBox.LineClicked += lineNo => text.Text = selectBox.Values[lineNo] + " clicked";
		}

		[Test]
		public void RenderSelectBoxWithTenValuesAndThreeLines()
		{
			SetToTenValues();
			var text = new FontText(Font.Default, "", new Rectangle(0.4f, 0.7f, 0.2f, 0.1f));
			selectBox.LineClicked += lineNo => text.Text = selectBox.Values[lineNo] + " clicked";
		}

		private void SetToTenValues()
		{
			selectBox.Values = new List<object>
			{
				"value 1",
				"value 2",
				"value 3",
				"value 4",
				"value 5",
				"value 6",
				"value 7",
				"value 8",
				"value 9",
				"value 10"
			};
		}

		[Test]
		public void RenderSelectBoxAttachedToMouse()
		{
			selectBox.Values = new List<object> { "value 1", "value 2", "value 3", "value 4" };
			new Command(
				point => selectBox.DrawArea = Rectangle.FromCenter(point, selectBox.DrawArea.Size)).Add(
					new MouseMovementTrigger());
		}

		[Test]
		public void SetValuesAsNull()
		{			
			Assert.Throws<SelectBox.MustBeAtLeastOneValue>(() => selectBox.Values = null);
		}

		[Test]
		public void RenderGrowingSelectBox()
		{
			selectBox.Values = new List<object> { "value 1", "value 2", "value 3", "value 4" };
			selectBox.Start<Grow>();
			var text = new FontText(Font.Default, "", new Rectangle(0.4f, 0.7f, 0.2f, 0.1f));
			selectBox.LineClicked += lineNo => text.Text = selectBox.Values[lineNo] + " clicked";
		}

		[Test, CloseAfterFirstFrame]
		public void SaveAndLoad()
		{
			var stream = BinaryDataExtensions.SaveToMemoryStream(selectBox);
			var loadedSelectBox = (SelectBox)stream.CreateFromMemoryStream();
			Assert.AreEqual(selectBox.DrawArea, loadedSelectBox.DrawArea);
			Assert.AreEqual(3, loadedSelectBox.Values.Count);
			Assert.AreEqual(selectBox.Values[1].ToString(), loadedSelectBox.Values[1].ToString());
		}

		[Test]
		public void DrawLoadedSelectBox()
		{
			SetToTenValues();
			var stream = BinaryDataExtensions.SaveToMemoryStream(selectBox);
			selectBox.IsActive = false;
			var loadedSelectBox = (SelectBox)stream.CreateFromMemoryStream(); 
			var text = new FontText(Font.Default, "", new Rectangle(0.4f, 0.7f, 0.2f, 0.1f));
			loadedSelectBox.LineClicked +=
				lineNo => text.Text = loadedSelectBox.Values[lineNo] + " clicked"; //ncrunch: no coverage
		}
	}
}