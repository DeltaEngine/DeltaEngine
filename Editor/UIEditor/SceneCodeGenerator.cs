using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;
using Button = DeltaEngine.Scenes.Controls.Button;
using Control = DeltaEngine.Scenes.Controls.Control;

namespace DeltaEngine.Editor.UIEditor
{
	public class SceneCodeGenerator
	{
		public SceneCodeGenerator(Service service, Scene scene, string sceneName)
		{
			this.service = service;
			this.scene = scene;
			sceneClassName = sceneName;
		}

		private readonly Service service;
		private readonly Scene scene;
		protected static string sceneClassName;

		public void GenerateSourceCodeClass()
		{
			ClassCodeString = "";
			ClassCodeString += CreateUsingStatement();
			ClassCodeString += CreateNamespaceAndClassName();
			ClassCodeString += CreateConstructor();
			ClassCodeString += CreateMethods();
			ClassCodeString += CreateEndOfClass();
			CreateNewSourceCodeClass();
		}

		public string ClassCodeString { get; set; }

		private static string CreateUsingStatement()
		{
			var usingStatementString = "";
			usingStatementString += SceneUsingStatement;
			usingStatementString += ControlUsingStatement;
			usingStatementString += ContentUsingStatement;
			usingStatementString += "\n";
			return usingStatementString;
		}

		private const string SceneUsingStatement = "using DeltaEngine.Scenes;\n";
		private const string ControlUsingStatement = "using DeltaEngine.Scenes.Controls;\n";
		private const string ContentUsingStatement = "using DeltaEngine.Content;\n";

		private string CreateNamespaceAndClassName()
		{
			var namespaceAndClassNameString = "";
			namespaceAndClassNameString += "namespace " + service.ProjectName + "\n";
			namespaceAndClassNameString += "{\n";
			namespaceAndClassNameString += "	public class " + sceneClassName + "\n";
			namespaceAndClassNameString += "	{\n";
			return namespaceAndClassNameString;
		}

		private string CreateConstructor()
		{
			var constructorString = "";
			constructorString += "		public " + sceneClassName + "()\n";
			constructorString += "		{\n";
			constructorString += "			scene = ContentLoader.Load<Scene>(" + '"' + sceneClassName + '"' +
				");\n";
			constructorString += CreateMethodToControlActionAssignmentString();
			constructorString += "		}\n";
			constructorString += "\n";
			return constructorString;
		}

		private string CreateMethodToControlActionAssignmentString()
		{
			var methodsToActionAssignmentString = "";
			foreach (var control in scene.Controls)
				if (control.GetType() == typeof(Button))
					methodsToActionAssignmentString += CreateMethodToButtonClickEventString((Button)control);
				else if (control.GetType() == typeof(Slider))
					methodsToActionAssignmentString +=
						CreateMethodToSliderValueChangedEventString((Slider)control);
			return methodsToActionAssignmentString;
		}

		private static string CreateMethodToButtonClickEventString(Button button)
		{
			string controlName = button.Name;
			var methodsToButtonClickAssignmentString = "";
			methodsToButtonClickAssignmentString += "			var " + controlName +
				" = scene.Controls.FirstOrDefault(control => ((Control)control).Name == " + '"' +
				controlName + '"' + ");\n";
			methodsToButtonClickAssignmentString += "			" + controlName + ".Clicked = () => " +
				controlName + "Clicked();\n";
			return methodsToButtonClickAssignmentString;
		}

		private static string CreateMethodToSliderValueChangedEventString(Slider slider)
		{
			string controlName = slider.Name;
			var methodsToSliderValueChangedAssignmentString = "";
			methodsToSliderValueChangedAssignmentString += "			var " + controlName +
				" = scene.Controls.FirstOrDefault(control => ((Control)control).Name == " + '"' +
				controlName + '"' + ");\n";
			methodsToSliderValueChangedAssignmentString += "			" + controlName +
				".ValueChanged = () => " + controlName + "ValueChanged(value);\n";
			return methodsToSliderValueChangedAssignmentString;
		}

		private string CreateMethods()
		{
			var methodsString = "";
			for (int index = 0; index < scene.Controls.Count; index++)
				methodsString += CreateMethodForControlEventString(index);
			return methodsString;
		}

		private string CreateMethodForControlEventString(int index)
		{
			var methodString = "";
			var control = scene.Controls[index];
			if (control.GetType() == typeof(Button))
				methodString += "		private void " + ((Control)control).Name + "Clicked() {}\n";
			else if (control.GetType() == typeof(Slider))
				methodString += "		private void " + ((Control)control).Name +
					"ValueChanged(int value) {}\n";
			if (index != scene.Controls.Count - 1)
				methodString += "\n";
			return methodString;
		}

		private static string CreateEndOfClass()
		{
			var classEndString = "";
			classEndString += "	}\n";
			classEndString += "}";
			return classEndString;
		}

		//ncrunch: no coverage start
		protected virtual void CreateNewSourceCodeClass()
		{
			if (StackTraceExtensions.IsStartedFromNCrunch())
				return;
			string path = service.GetAbsoluteSolutionFilePath(service.ProjectName);
			var filePath = Path.GetDirectoryName(path);
			bool canCreate = true;
			if (File.Exists(filePath + "\\" + sceneClassName + ".cs"))
				canCreate = GiveUserOptionOfOverwriting();
			if (canCreate)
				AddClassToProjectFileAndCsProj(filePath);
		}

		private void AddClassToProjectFileAndCsProj(string filePath)
		{
			File.WriteAllText(filePath + "\\" + sceneClassName + ".cs", ClassCodeString);
			var sb = new StringBuilder();
			using (var sr = new StreamReader(filePath + "\\" + service.ProjectName + ".csproj"))
				ReadTheCsprojFile(sr, sb);
			File.WriteAllText(filePath + "\\" + service.ProjectName + ".csproj", sb.ToString());
		}

		private static void ReadTheCsprojFile(StreamReader sr, StringBuilder sb)
		{
			string line;
			bool isAdded = false;
			while ((line = sr.ReadLine()) != null)
				isAdded = AddNewClassNameToCompileSection(sb, line, isAdded, sr);
		}

		private static bool AddNewClassNameToCompileSection(StringBuilder sb, string line,
			bool isAdded, StreamReader sr)
		{
			sb.AppendLine(line);
			if (!line.Contains("<Compile Include=") || isAdded)
				return isAdded;
			if (sr.ToString().Contains("<Compile Include=" + '"' + sceneClassName + ".cs"))
				return false;
			sb.AppendLine("<Compile Include=" + '"' + sceneClassName + ".cs" + '"' + " />");
			return true;
		}

		private bool GiveUserOptionOfOverwriting()
		{
			var messageBox = new MyButtonsMessageBox();
			return false;
		}

		public class MyButtonsMessageBox : Form
		{
			private void InitializeComponent()
			{
				var button1 = new System.Windows.Forms.Button();
				this.SuspendLayout();
				// 
				// button1
				// 
				button1.Location = new System.Drawing.Point(112, 65);
				button1.Name = "button1";
				button1.Size = new System.Drawing.Size(75, 23);
				button1.TabIndex = 0;
				button1.Text = "button1";
				button1.UseVisualStyleBackColor = true;
				button1.Visible = true;
				button1.Enabled = true;
				// 
				// Form1
				// 
				this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
				this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
				this.ClientSize = new System.Drawing.Size(284, 261);
				this.Controls.Add(button1);
				this.Name = "Form1";
				this.Text = "Form1";
				this.ResumeLayout(false);
			}
		}
	}
}