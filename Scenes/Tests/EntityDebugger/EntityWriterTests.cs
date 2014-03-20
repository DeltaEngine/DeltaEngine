using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.Scenes.Controls;
using DeltaEngine.Scenes.EntityDebugger;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.EntityDebugger
{
	internal class EntityWriterTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			ellipse = new Ellipse(Vector2D.Half, 0.2f, 0.1f, Color.Blue);
			ellipse.Add(new Name("name"));
			writer = new EntityWriter(ellipse);
			drawAreaBox = writer.scene.Controls[1] as TextBox;
			colorRSlider = writer.scene.Controls[3] as Slider;
			colorGSlider = writer.scene.Controls[5] as Slider;
			colorBSlider = writer.scene.Controls[7] as Slider;
			colorASlider = writer.scene.Controls[9] as Slider;
			nameBox = writer.scene.Controls[11] as TextBox;
			rotationBox = writer.scene.Controls[13] as TextBox;
			rotationCenterBox = writer.scene.Controls[15] as TextBox;
		}

		private Ellipse ellipse;
		private EntityWriter writer;
		private Slider colorRSlider;
		private Slider colorGSlider;
		private Slider colorBSlider;
		private Slider colorASlider;
		private TextBox nameBox;
		private TextBox drawAreaBox;
		private TextBox rotationBox;
		private TextBox rotationCenterBox;

		private class Name
		{
			public Name(string name)
			{
				Text = name;
			}

			private string Text { get; set; }

			public override string ToString()
			{
				return Text;
			}
		}

		[Test]
		public void EditEllipseComponents() {}

		[Test, CloseAfterFirstFrame]
		public void IsNotPauseable()
		{
			Assert.IsFalse(writer.IsPauseable);
		}

		[Test, CloseAfterFirstFrame]
		public void ConstructorTakesEntity()
		{
			Assert.AreEqual(ellipse, writer.Entity);
		}

		[Test, CloseAfterFirstFrame]
		public void RotationBecomesTextBox()
		{
			Assert.AreEqual("0", rotationBox.Text);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeRotationViaTextBox()
		{
			rotationBox.Text = "90";
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(90, ellipse.Rotation);
		}

		[Test, CloseAfterFirstFrame]
		public void NonNumericRotationSetsRotationToZero()
		{
			ellipse.Rotation = 45;
			rotationBox.Text = "abc";
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(0, ellipse.Rotation);
		}

		[Test, CloseAfterFirstFrame]
		public void InitialLabelAndControlPosition()
		{
			var expectedNextLabelDrawArea = new Rectangle(EntityEditor.SceneLeftEdge,
				EntityEditor.SceneTopEdge + 0.21875f, EntityEditor.LabelWidth, EntityEditor.ControlHeight);
			Assert.IsTrue(writer.scene.Controls[0].DrawArea.IsNearlyEqual(expectedNextLabelDrawArea),
				writer.scene.Controls[0].DrawArea.ToString());
			var expectedNextControlDrawArea = new Rectangle(
					EntityEditor.SceneLeftEdge + EntityEditor.LabelWidth + EntityEditor.LabelToControlGap,
					EntityEditor.SceneTopEdge + 0.21875f, EntityEditor.ControlWidth,
					EntityEditor.ControlHeight);
			Assert.IsTrue(writer.scene.Controls[1].DrawArea.IsNearlyEqual(expectedNextControlDrawArea),
				writer.scene.Controls[1].DrawArea.ToString());
		}

		[Test, CloseAfterFirstFrame]
		public void ResizingViewportRepositionsControls()
		{
			Resolve<Window>().ViewportPixelSize = new Size(200, 200);
			var expectedNextLabelDrawArea = new Rectangle(EntityEditor.SceneLeftEdge,
				EntityEditor.SceneTopEdge, EntityEditor.LabelWidth, EntityEditor.ControlHeight);
			Assert.IsTrue(writer.scene.Controls[0].DrawArea.IsNearlyEqual(expectedNextLabelDrawArea),
				writer.scene.Controls[0].DrawArea.ToString());
			var expectedNextControlDrawArea = new Rectangle(
					EntityEditor.SceneLeftEdge + EntityEditor.LabelWidth + EntityEditor.LabelToControlGap,
					EntityEditor.SceneTopEdge, EntityEditor.ControlWidth, EntityEditor.ControlHeight);
			Assert.IsTrue(writer.scene.Controls[1].DrawArea.IsNearlyEqual(expectedNextControlDrawArea),
				writer.scene.Controls[1].DrawArea.ToString());
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeRotationCenterViaTextBoxes()
		{
			ellipse.RotationCenter = Vector2D.Zero;
			rotationCenterBox.Text = Vector2D.One.ToString();
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(Vector2D.One, ellipse.RotationCenter);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeDrawAreaViaTextBox()
		{
			ellipse.DrawArea = Rectangle.Zero;
			drawAreaBox.Text = Rectangle.HalfCentered.ToString();
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(Rectangle.HalfCentered, ellipse.DrawArea);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeColorViaTextBox()
		{
			ellipse.Color = Color.Black;
			colorRSlider.Value = 1;
			colorGSlider.Value = 2;
			colorBSlider.Value = 3;
			colorASlider.Value = 4;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Color(1, 2, 3, 4), ellipse.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void NonNumericDrawAreaSetsItToZero()
		{
			ellipse.DrawArea = new Rectangle(1, 2, 3, 4);
			drawAreaBox.Text = "abc";
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(Rectangle.Zero, ellipse.DrawArea);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangingDrawAreaChangesUnchangedRotationCenter()
		{
			drawAreaBox.Text = "0 0 0.5 0.5";
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.25f, 0.25f), ellipse.RotationCenter);
			Assert.AreEqual("0.25, 0.25", rotationCenterBox.Text);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangingRotationCenterThenDrawAreaLeavesRotationCenterUnchanged()
		{
			rotationCenterBox.Text = Vector2D.One.ToString();
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(Vector2D.One, ellipse.RotationCenter);
			drawAreaBox.Text = "0 0 1 1";
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(Vector2D.One, ellipse.RotationCenter);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeName()
		{
			nameBox.Text = "name2";
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual("name2", ellipse.Get<Name>().ToString());
		}
	}
}