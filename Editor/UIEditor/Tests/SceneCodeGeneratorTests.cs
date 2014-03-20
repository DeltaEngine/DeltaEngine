using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	public class SceneCodeGeneratorTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Setup()
		{
			scene = new Scene();
			for (int i = 0; i < 5; ++i)
				scene.Add(new Button(new Rectangle()));
			for (int i = 0; i < 5; ++i)
				scene.Add(new Slider(new Rectangle()));
			generator = new SceneCodeGenerator(new MockService("", "TestProject"), scene, "TestScene");
			generator.GenerateSourceCodeClass();
		}

		private SceneCodeGenerator generator;
		private Scene scene;

		[Test]
		public void GeneratedStringShouldNotBeEmpty()
		{
			Assert.IsNotEmpty(generator.ClassCodeString);
		}

		[Test]
		public void AddUsingStatementToTheClassString()
		{
			Assert.IsTrue(generator.ClassCodeString.Contains("using DeltaEngine.Scenes;"));
			Assert.IsTrue(generator.ClassCodeString.Contains("using DeltaEngine.Scenes.Controls;"));
			Assert.IsTrue(generator.ClassCodeString.Contains("using DeltaEngine.Content;"));
		}

		[Test]
		public void AddNameSpaceAndClassNameToClassString()
		{
			Assert.IsTrue(generator.ClassCodeString.Contains("namespace TestProject"));
			Assert.IsTrue(generator.ClassCodeString.Contains("public class TestScene"));
		}

		[Test]
		public void AddConstructorToClassString()
		{
			Assert.IsTrue(generator.ClassCodeString.Contains("public TestScene()"));
			Assert.IsTrue(
				generator.ClassCodeString.Contains("scene = ContentLoader.Load<Scene>(" + '"' + "TestScene" +
					'"' + ")"));
			AssignControlEventsToMethods();
		}

		private void AssignControlEventsToMethods()
		{
			buttonCount = 1;
			sliderCount = 1;
			foreach (var control in scene.Controls.Cast<Control>())
				if (control.GetType() == typeof(Button))
					AssignMethodeToButtonClickEvent();
				else if (control.GetType() == typeof(Slider))
					AssignMethodToSliderValueChangedEvent();
		}

		private void AssignMethodeToButtonClickEvent()
		{
			Assert.IsTrue(
				generator.ClassCodeString.Contains("var Button" + buttonCount +
					" = scene.Controls.FirstOrDefault(control => ((Control)control).Name == " + '"' + "Button" +
					buttonCount + '"' + ")"));
			Assert.IsTrue(
				generator.ClassCodeString.Contains("Button" + buttonCount + ".Clicked = () => Button" +
					buttonCount + "Clicked()"));
			buttonCount++;
		}

		private static int buttonCount;

		private void AssignMethodToSliderValueChangedEvent()
		{
			Assert.IsTrue(
				generator.ClassCodeString.Contains("var Slider" + sliderCount +
					" = scene.Controls.FirstOrDefault(control => ((Control)control).Name == " + '"' + "Slider" +
					sliderCount + '"' + ")"));
			Assert.IsTrue(
				generator.ClassCodeString.Contains("Slider" + sliderCount + ".ValueChanged = () => Slider" +
					sliderCount + "ValueChanged(value)"));
			sliderCount++;
		}

		private static int sliderCount;

		[Test]
		public void AddMethodsForControlEvents()
		{
			buttonCount = 1;
			sliderCount = 1;
			foreach (var control in scene.Controls.Cast<Control>())
				if (control.GetType() == typeof(Button))
					AddMethodeForButtonClickEvent();
				else if (control.GetType() == typeof(Slider))
					AddMethodForSliderValueChangedEvent();
		}

		private void AddMethodeForButtonClickEvent()
		{
			Assert.IsTrue(
				generator.ClassCodeString.Contains("private void Button" + buttonCount + "Clicked() {}"));
			buttonCount++;
		}

		private void AddMethodForSliderValueChangedEvent()
		{
			Assert.IsTrue(
				generator.ClassCodeString.Contains("private void Slider" + sliderCount +
					"ValueChanged(int value) {}"));
			sliderCount++;
		}

		[Test]
		public void GenerateSourceCodeShouldApplyToCodingStyle()
		{
			var newScene = new Scene();
			newScene.Add(new Button(new Rectangle()));
			newScene.Add(new Slider(new Rectangle()));
			generator = new SceneCodeGenerator(new MockService("", "TestProject"), newScene,
				"" + "FullTestScene");
			generator.GenerateSourceCodeClass();
			CheckIfGeneratedCodeIsValid();
		}

		private void CheckIfGeneratedCodeIsValid()
		{
			Assert.AreEqual(TestClass, generator.ClassCodeString);
		}

		private static readonly string TestClass = "using DeltaEngine.Scenes;\n" +
			"using DeltaEngine.Scenes.Controls;\n" + "using DeltaEngine.Content;\n" + "\n" +
			"namespace TestProject\n" + "{\n" + "	public class FullTestScene\n" + "	{\n" +
			"		public FullTestScene()\n" + "		{\n" + "			scene = ContentLoader.Load<Scene>(" + '"' +
			"FullTestScene" + '"' + ");\n" +
			"			var Button6 = scene.Controls.FirstOrDefault(control => ((Control)control).Name == " +
			'"' + "Button6" + '"' + ");\n" + "			Button6.Clicked = () => Button6Clicked();\n" +
			"			var Slider6 = scene.Controls.FirstOrDefault(control => ((Control)control).Name == " +
			'"' + "Slider6" + '"' + ");\n" +
			"			Slider6.ValueChanged = () => Slider6ValueChanged(value);\n" + "		}\n" + "\n" +
			"		private void Button6Clicked() {}\n" + "\n" +
			"		private void Slider6ValueChanged(int value) {}\n" + "	}\n" + "}";

		[Test]
		public void CreateClassInProjectFolder()
		{
			var sceneSpy = new Scene();
			var spyGenerator = new SceneCodeGeneratorSpy(new MockService("", "TestProject"), sceneSpy,
				"TestScene");
			spyGenerator.GenerateSourceCodeClass();
			Assert.IsTrue(spyGenerator.CreatedSourceClass);
			Assert.IsTrue(
				spyGenerator.csprojString.Contains("<Compile Include=" + '"' + "TestScene" + ".cs" + '"' +
					" />"));
		}

		[Test]
		public void OverwriteCreatedClass()
		{
			var sceneSpy = new Scene();
			var spyGenerator = new SceneCodeGeneratorSpy(new MockService("", "TestProject"), sceneSpy,
				"TestScene");
			spyGenerator.GenerateSourceCodeClass();
			var firstClassString = spyGenerator.CreatedSourceClass;
			Assert.IsTrue(spyGenerator.CreatedSourceClass);
			sceneSpy.Add(new Button(new Rectangle()));
			spyGenerator.GenerateSourceCodeClass();
			Assert.AreNotEqual(firstClassString, spyGenerator.CreatedClassString);
		}
	}
}