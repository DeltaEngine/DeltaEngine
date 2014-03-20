using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.Controls;
using DeltaEngine.Scenes.EntityDebugger;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.EntityDebugger
{
	internal class EntityReaderTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			logo = new BouncingLogo();
			logo.Color = Color.White;
			reader = new EntityReader(logo);
		}

		private Sprite logo;
		private EntityReader reader;

		[Test]
		public void ViewLogoComponents() {}

		[Test, CloseAfterFirstFrame]
		public void UpdatingEntityRotationUpdatesRotationTextBox()
		{
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(logo.Rotation.ToInvariantString(), GetTextBox("Rotation").Text);
		}

		private TextBox GetTextBox(string name)
		{
			for (int i = 0; i < reader.scene.Controls.Count; i++)
			{
				var label = reader.scene.Controls[i] as Label;
				if (label != null && label.Text == name)
					return (TextBox)reader.scene.Controls[i + 1];
			}
			return null; //ncrunch: no coverage
		}

		[Test, CloseAfterFirstFrame, Timeout(10000)]
		public void AddComponent()
		{
			logo.Add(new Name("name"));
			AdvanceTimeAndUpdateEntities();
			if (GetTextBox("Name") != null)
				Assert.AreEqual("name", GetTextBox("Name").Text);
		}

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

		[Test, CloseAfterFirstFrame, Timeout(10000)]
		public void DisposingClearsSceneAndInactivates()
		{
			reader.Dispose();
			Assert.AreEqual(0, reader.scene.Controls.Count);
			Assert.IsFalse(reader.IsActive);
		}
	}
}