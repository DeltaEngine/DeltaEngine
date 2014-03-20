using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	public class ControlAnchorerTests : TestWithMocksOrVisually
	{
		[Test]
		public void AchorControlsVerticalControls()
		{
			var controlList = createfilledControlList();
			controlList[0].DrawArea = new Rectangle(0, -1.5f, 1, 1);
			controlList[1].DrawArea = new Rectangle(0, -0.5f, 1, 1);
			controlList[2].DrawArea = new Rectangle(0, 1.5f, 1, 1);
			var anchoredControl = new Button(Rectangle.One);
			ControlAnchorer.AnchorSelectedControls(anchoredControl, controlList);
			Assert.AreEqual(new Vector2D(0, -1.5f), controlList[0].DrawArea.TopLeft);
			Assert.AreEqual(new Vector2D(0, -0.5f), controlList[1].DrawArea.TopLeft);
			Assert.AreEqual(new Vector2D(0, 1.5f), controlList[2].DrawArea.TopLeft);
			anchoredControl.DrawArea = new Rectangle(0, 0.5f, 1, 1);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0, -1), controlList[0].DrawArea.TopLeft);
			Assert.AreEqual(new Vector2D(0, 0), controlList[1].DrawArea.TopLeft);
			Assert.AreEqual(new Vector2D(0, 2f), controlList[2].DrawArea.TopLeft);
		}

		private List<Entity2D> createfilledControlList()
		{
			var controlList = new List<Entity2D>();
			for (int i = 0; i < 3; i++)
				controlList.Add(new Button(Rectangle.One));
			return controlList;
		}

		[Test]
		public void HorizontalControls()
		{
			var controlList = createfilledControlList();
			controlList[0].DrawArea = new Rectangle(-2.5f, 0, 1, 1);
			controlList[1].DrawArea = new Rectangle(-0.5f, 0, 1, 1);
			controlList[2].DrawArea = new Rectangle(2.5f, 0, 1, 1);
			var anchoredControl = new Button(Rectangle.One);
			ControlAnchorer.AnchorSelectedControls(anchoredControl, controlList);
			Assert.AreEqual(new Vector2D(-2.5f,0), controlList[0].DrawArea.TopLeft);
			Assert.AreEqual(new Vector2D(-0.5f, 0), controlList[1].DrawArea.TopLeft);
			Assert.AreEqual(new Vector2D(2.5f, 0), controlList[2].DrawArea.TopLeft);
			anchoredControl.DrawArea = new Rectangle(0.5f, 0, 1, 1);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(-2, 0), controlList[0].DrawArea.TopLeft);
			Assert.AreEqual(new Vector2D(0, 0), controlList[1].DrawArea.TopLeft);
			Assert.AreEqual(new Vector2D(3f, 0), controlList[2].DrawArea.TopLeft);
		}

		[Test]
		public void UnAnchoreControls()
		{
			var controlList = createfilledControlList();
			controlList[0].DrawArea = new Rectangle(-1.5f, 0, 1, 1);
			controlList[1].DrawArea = new Rectangle(-0.5f, 0, 1, 1);
			controlList[2].DrawArea = new Rectangle(1.5f, 0, 1, 1);
			var anchoredControl = new Button(Rectangle.One);
			ControlAnchorer.AnchorSelectedControls(anchoredControl, controlList);
			anchoredControl.DrawArea = new Rectangle(0.5f, 0, 1, 1);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(-1, 0), controlList[0].DrawArea.TopLeft);
			Assert.AreEqual(new Vector2D(0, 0), controlList[1].DrawArea.TopLeft);
			Assert.AreEqual(new Vector2D(2f, 0), controlList[2].DrawArea.TopLeft);
			ControlAnchorer.UnAnchorSelectedControls(controlList);
			anchoredControl.DrawArea = new Rectangle(4f, 0, 1, 1);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(-1, 0), controlList[0].DrawArea.TopLeft);
			Assert.AreEqual(new Vector2D(0, 0), controlList[1].DrawArea.TopLeft);
			Assert.AreEqual(new Vector2D(2f, 0), controlList[2].DrawArea.TopLeft);
		}
	}
}