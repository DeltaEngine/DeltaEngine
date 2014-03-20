using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class DropdownListTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			dropdownList = new DropdownList(Center, Values);
			InitializeMouse();
		}

		private DropdownList dropdownList;

		private static readonly Rectangle Center = new Rectangle(0.4f, 0.3f, 0.2f, 0.05f);
		private static readonly List<object> Values = new List<object>
		{
			"value 1",
			"value 2",
			"value 3"
		};

		private void InitializeMouse()
		{
			mouse = Resolve<Mouse>() as MockMouse;
			if (mouse == null)
				return; //ncrunch: no coverage
			mouse.SetNativePosition(Vector2D.Zero);
			AdvanceTimeAndUpdateEntities();
		}

		private MockMouse mouse;

		[Test]
		public void RenderDropdownListWithTenValues()
		{
			SetToTenValues();
		}

		private void SetToTenValues()
		{
			dropdownList.Values = new List<object>
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

		[Test, CloseAfterFirstFrame]
		public void DefaultProperties()
		{
			Assert.AreEqual(Values, dropdownList.Values);
			Assert.AreEqual(Values[0], dropdownList.SelectedValue);
			Assert.AreEqual(Center, dropdownList.DrawArea);
			Assert.AreEqual(Color.Gray, dropdownList.Color);
			Assert.AreEqual(3, dropdownList.SelectBox.texts.Count);
			Assert.IsFalse(dropdownList.IsPauseable);
			Assert.AreEqual(3, dropdownList.MaxDisplayCount);
		}

		[Test, CloseAfterFirstFrame]
		public void NoValuesThrowsException()
		{
			Assert.Throws<SelectBox.MustBeAtLeastOneValue>(() => new DropdownList(Rectangle.Zero, null));
			Assert.Throws<SelectBox.MustBeAtLeastOneValue>(
				() => new DropdownList(Rectangle.Zero, new List<object>()));
		}

		[Test, CloseAfterFirstFrame]
		public void IfValuesAreChangedButStillContainSelectedValueItRemainsSelected()
		{
			var newValues = new List<object> { 1, "value 1", 2 };
			dropdownList.Values = newValues;
			Assert.AreEqual(newValues, dropdownList.Values);
			Assert.AreEqual("value 1", dropdownList.SelectedValue);
			Assert.AreEqual(3, dropdownList.SelectBox.texts.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void IfValuesAreChangedAndNoLongerContainSelectedValueItRevertsToFirstValue()
		{
			var newValues = new List<object> { 1, 2 };
			dropdownList.Values = newValues;
			Assert.AreEqual(1, dropdownList.SelectedValue);
			Assert.AreEqual(2, dropdownList.SelectBox.texts.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void IfSelectedValueIsChangedToSomethingInValuesThatIsOk()
		{
			dropdownList.SelectedValue = "value 3";
			Assert.AreEqual("value 3", dropdownList.SelectedValue);
		}

		[Test, CloseAfterFirstFrame]
		public void IfSelectedValueIsChangedToSomethingNotInValuesItThrowsAnException()
		{
			Assert.Throws<DropdownList.SelectedValueMustBeOneOfTheListOfValues>(
				() => dropdownList.SelectedValue = "value 4");
		}

		[Test, CloseAfterFirstFrame]
		public void HidingDropdownListHidesEverything()
		{
			dropdownList.IsVisible = false;
			Assert.IsFalse(dropdownList.SelectBox.IsVisible);
			Assert.IsFalse(dropdownList.Get<FontText>().IsVisible);
			foreach (FontText text in dropdownList.SelectBox.texts)
				Assert.IsFalse(text.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void ClickingRevealsValues()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			Click(new Vector2D(0.5f, 0.31f));
			Assert.IsTrue(dropdownList.SelectBox.IsVisible);
		}

		private void Click(Vector2D position)
		{
			mouse.SetNativePosition(position);
			mouse.SetButtonState(MouseButton.Left, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			mouse.SetNativePosition(position);
			mouse.SetButtonState(MouseButton.Left, State.Releasing);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void ClickToRevealValuesThenClickToSelectNewValueAndHideValues()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			Click(new Vector2D(0.5f, 0.31f));
			Click(new Vector2D(0.5f, 0.41f));
			Assert.AreEqual("value 2", dropdownList.SelectedValue);
			Assert.IsFalse(dropdownList.SelectBox.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void HideSelectBoxAndThenClickOnSelectBoxLine()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			dropdownList.SelectBox.IsVisible = false;
			dropdownList.SelectBox.LineClicked(0);
			Assert.IsFalse(dropdownList.SelectBox.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void ClickingHiddenSelectionBoxDoesNothing()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			Click(new Vector2D(0.5f, 0.41f));
			Assert.AreEqual("value 1", dropdownList.SelectedValue);
		}

		[Test, CloseAfterFirstFrame]
		public void ClickToRevealValuesThenMouseoverToChangeValueColor()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			Click(new Vector2D(0.5f, 0.31f));
			MoveMouse(new Vector2D(0.5f, 0.41f));
			Assert.AreEqual(Color.VeryLightGray, dropdownList.SelectBox.texts[0].Color);
			Assert.AreEqual(Color.White, dropdownList.SelectBox.texts[1].Color);
			Assert.AreEqual(Color.VeryLightGray, dropdownList.SelectBox.texts[2].Color);
		}

		private void MoveMouse(Vector2D position)
		{
			mouse.SetNativePosition(position);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void WhenDisabledSelectionBoxIsHidden()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			Click(new Vector2D(0.5f, 0.31f));
			dropdownList.IsEnabled = false;
			Assert.IsFalse(dropdownList.SelectBox.IsVisible);
			foreach (FontText text in dropdownList.SelectBox.texts)
				Assert.IsFalse(text.IsVisible);
		}

		[Test]
		public void RenderGrowingDropdownList()
		{
			dropdownList.Values = new List<object> { "value 1", "value 2", "value 3", "value 4" };
			dropdownList.Start<Grow>();
		}

		//ncrunch: no coverage start
		private class Grow : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (DropdownList dropdownList in entities)
				{
					var center = dropdownList.DrawArea.Center + new Vector2D(0.01f, 0.01f) * Time.Delta;
					var size = dropdownList.DrawArea.Size * (1.0f + Time.Delta / 10);
					dropdownList.DrawArea = Rectangle.FromCenter(center, size);
				}
			}
		} //ncrunch: no coverage end

		[Test]
		public void RenderDropdownListAttachedToMouse()
		{
			dropdownList.Values = new List<object> { "value 1", "value 2", "value 3", "value 4" };
			new Command(
				point => dropdownList.DrawArea = //ncrunch: no coverage
					Rectangle.FromCenter(point, dropdownList.DrawArea.Size)).Add(new MouseMovementTrigger());
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeMaxDisplayCount()
		{
			dropdownList.MaxDisplayCount = 4;
			Assert.AreEqual(4, dropdownList.MaxDisplayCount);
		}

		[Test, CloseAfterFirstFrame]
		public void SaveAndLoad()
		{
			var stream = BinaryDataExtensions.SaveToMemoryStream(dropdownList);
			var loadedDropdownList = (DropdownList)stream.CreateFromMemoryStream();
			Assert.AreEqual(dropdownList.DrawArea, loadedDropdownList.DrawArea);
			Assert.AreEqual(3, loadedDropdownList.Values.Count);
			Assert.AreEqual(dropdownList.Values[1].ToString(), loadedDropdownList.Values[1].ToString());
		}

		[Test]
		public void DrawLoadedSelectBox()
		{
			SetToTenValues();
			var stream = BinaryDataExtensions.SaveToMemoryStream(dropdownList);
			dropdownList.IsActive = false;
			stream.CreateFromMemoryStream();
		}
	}
}