using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseHoverTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void CountdownOnMouseHover()
		{
			var trigger = new MouseHoverTrigger(3.0f);
			new Countdown(new FontText(Font.Default, "", Rectangle.One), trigger);
			var drawArea = Rectangle.One;
			var counter = 0;
			var text = new FontText(Font.Default, "", drawArea.Move(new Vector2D(0.0f, 0.1f)));
			new Command(() => text.Text = "MouseHover triggered " + ++counter + " times.").Add(trigger);
		}

		private sealed class Countdown : Entity
		{
			public Countdown(FontText text, MouseHoverTrigger trigger)
			{
				Add(text);
				Add(trigger);
				Start<UpdateCountdown>();
			}

			private class UpdateCountdown : UpdateBehavior
			{
				public override void Update(IEnumerable<Entity> entities)
				{
					foreach (var entity in entities)
						entity.Get<FontText>().Text = GetText(entity);
				}

				private static string GetText(Entity entity)
				{
					return "Hold mouse still for " + entity.Get<MouseHoverTrigger>().Elapsed + " seconds.";
				}
			}
		}

		[Test, CloseAfterFirstFrame]
		public void HoverTriggersIfMouseDoesntMove()
		{
			bool isTriggered = false;
			new Command(() => isTriggered = true).Add(new MouseHoverTrigger());
			Resolve<Mouse>().SetNativePosition(Vector2D.Zero);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.IsFalse(isTriggered);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.IsTrue(isTriggered);
		}

		[Test, CloseAfterFirstFrame]
		public void HoverDoesntTriggersIfMouseMoves()
		{
			bool isTriggered = false;
			new Command(() => isTriggered = true).Add(new MouseHoverTrigger());
			Resolve<Mouse>().SetNativePosition(Vector2D.Zero);
			AdvanceTimeAndUpdateEntities(0.5f);
			Resolve<Mouse>().SetNativePosition(Vector2D.One);
			AdvanceTimeAndUpdateEntities(0.5f);
			Assert.IsFalse(isTriggered);
		}

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			var trigger = new MouseHoverTrigger(3.0f);
			Assert.AreEqual(3.0f, trigger.HoverTime);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			var trigger = new MouseHoverTrigger("3.0");
			Assert.AreEqual(3.0f, trigger.HoverTime);
		}
	}
}