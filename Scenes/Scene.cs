using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes.Controls;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Scenes
{
	/// <summary>
	/// Groups Entities such that they can be activated and deactivated together. 
	/// </summary>
	public class Scene : ContentData
	{
		protected Scene(string contentName)
			: base(contentName) {}

		public Scene()
			: base("<GeneratedScene>") {}

		public void Add(IEnumerable<Entity2D> newControls)
		{
			foreach (Entity2D control in newControls)
				Add(control);
		}

		public virtual void Add(Entity2D control)
		{
			if (controls.Contains(control))
				return;
			if (control is Control)
				((Control)control).SceneDrawArea = DrawArea;
			controls.Add(control);
			control.IsActive = isShown;
		}

		private readonly List<Entity2D> controls = new List<Entity2D>();
		private bool isShown = true;

		public List<Entity2D> Controls
		{
			get { return controls; }
		}

		public void Remove(Entity2D control)
		{
			controls.Remove(control);
			if (control is Control)
				((Control)control).SceneDrawArea = Rectangle.Unused;
			control.Dispose();
		}

		public void Show()
		{
			foreach (var control in controls)
				control.IsVisible = true;
			isShown = true;
		}

		public void Hide()
		{
			foreach (var control in controls)
				control.IsVisible = false;
			isShown = false;
		}

		public void ToBackground()
		{
			foreach (Control control in controls.OfType<Control>())
				control.Stop<ControlUpdater>();
		}

		public void ToForeground()
		{
			foreach (Control control in controls.OfType<Control>())
				control.Start<ControlUpdater>();
		}

		public virtual void Clear()
		{
			foreach (Entity2D control in controls)
			{
				control.IsActive = false;
				if (control is Control)
					((Control)control).SceneDrawArea = Rectangle.Unused;
			}
			controls.Clear();
			ContentLoader.RemoveResource(Name);
		}

		public void SetQuadraticBackground(string imageName)
		{
			SetQuadraticBackground(new Material(ShaderFlags.Position2DColoredTextured, imageName));
		}

		public void SetQuadraticBackground(Material material)
		{
			if (background != null)
				Remove(background);
			background = new Sprite(material, Rectangle.One) { RenderLayer = int.MinValue };
			Add(background);
		}

		protected Sprite background;

		public void SetViewportBackground(string imageName)
		{
			SetViewportBackground(new Material(ShaderFlags.Position2DColoredTextured, imageName));
		}

		public void SetViewportBackground(Material material)
		{
			if (background != null)
				Remove(background);
			background = new Sprite(material, ScreenSpace.Scene.Viewport) { RenderLayer = int.MinValue };
			ScreenSpace.Scene.ViewportSizeChanged +=
				() => background.SetWithoutInterpolation(ScreenSpace.Scene.Viewport);
			Add(background);
		}

		protected override void DisposeData()
		{
			Clear();
		}

		public Rectangle DrawArea
		{
			get { return drawArea; }
			set
			{ //ncrunch: no coverage start
				foreach (Control control in Controls.OfType<Control>())
					control.SceneDrawArea = value;
				ControlUpdater.ProcessAnchoring(Controls);
				foreach (Entity2D control in Controls)
					RepositionControl(control, drawArea, value);
				drawArea = value;
			} //ncrunch: no coverage end
		}

		private Rectangle drawArea = Rectangle.One;

		private static void RepositionControl(Entity2D control, Rectangle lastDrawArea,
			Rectangle newDrawArea)
		{
			var relativeLeft = (control.TopLeft.X - lastDrawArea.Left) / lastDrawArea.Width;
			var relativeTop = (control.TopLeft.Y - lastDrawArea.Top) / lastDrawArea.Height;
			var left = newDrawArea.Left + relativeLeft * newDrawArea.Width;
			var top = newDrawArea.Top + relativeTop * newDrawArea.Height;
			var lastQuadraticDim = Math.Max(lastDrawArea.Width, lastDrawArea.Height);
			var newQuadraticDim = Math.Max(newDrawArea.Width, newDrawArea.Height);
			var stretchFactor = newQuadraticDim / lastQuadraticDim;
			var width = control.DrawArea.Width * stretchFactor;
			var height = control.DrawArea.Height * stretchFactor;
			control.DrawArea = new Rectangle(left, top, width, height);
		}

		protected override void LoadData(Stream fileData)
		{
			fileData.Position = 0;
			var loadedScene = (Scene)new BinaryReader(fileData).Create();
			foreach (var control in loadedScene.controls)
				AddControlToScene((Control)control);
			if (loadedScene.background != null)
				SetViewportBackground(loadedScene.background.Material); //ncrunch: no coverage
			loadedScene.Dispose();
		}

		//ncrunch: no coverage start
		public void AddControlToScene(Control control)
		{
			Control newControl = null;
			if (control.GetType() == typeof(Picture))
				newControl = new Picture(control.Get<Theme>(), control.Get<Material>(), control.DrawArea);
			else if (control.GetType() == typeof(Label))
			{
				newControl = new Label(control.Get<Theme>(), control.DrawArea, (control as Label).Text);
				newControl.Set(control.Get<BlendMode>());
				newControl.Set(control.Get<Material>());
			}
			else if (control.GetType() == typeof(Button))
				newControl = new Button(control.Get<Theme>(), control.DrawArea, (control as Button).Text);
			else if (control.GetType() == typeof(InteractiveButton))
				newControl = new InteractiveButton(control.Get<Theme>(), control.DrawArea,
					(control as Button).Text);
			else if (control.GetType() == typeof(Slider))
				newControl = new Slider(control.Get<Theme>(), control.DrawArea);
			newControl.Name = control.Name;
			if (newControl.Name == null && newControl.GetTags()[0] != null)
				newControl.Name = newControl.GetTags()[0];
			newControl.RenderLayer = control.RenderLayer;
			if (!control.Contains<AnchoringState>())
				newControl.Add(new AnchoringState()); //ncrunch: no coverage
			else
				newControl.Set(control.Get<AnchoringState>());
			Add(newControl);
		}

		public void LoadFromFile(Stream fileData)
		{
			fileData.Seek(0, SeekOrigin.Begin);
			var reader = new BinaryReader(fileData);
			string className = reader.ReadString();
			var dataVersion = reader.ReadBytes(4);
			var justSaveName = reader.ReadBoolean();
			var notNull = reader.ReadBoolean();
			var sceneName = reader.ReadString();
			var isDisposed = reader.ReadBoolean();
			var notNull2 = reader.ReadBoolean();
			var count = reader.ReadByte();
			var arrayTypeOrAreTypesAllDifferent = reader.ReadByte();
			for (int i = 0; i < count; i++)
				Add(LoadControl(reader, count != 1, dataVersion));
		}

		private static Control LoadControl(BinaryReader reader, bool doNotNullRead,
			byte[] versionBytes)
		{
			var controlType = reader.ReadString();
			Control control;
			if (controlType == "Button")
				control = new Button();
			else if (controlType == "InteractiveButton")
				control = new InteractiveButton();
			else if (controlType == "Picture")
				control = new Picture();
			else if (controlType == "Label")
				control = new Label();
			else if (controlType == "Slider")
				control = new Slider();
			else
				throw new ControlTypeNotImplemented(controlType);
			if (doNotNullRead)
				reader.ReadBoolean();
			LoadControl(control, reader, versionBytes);
			return control;
		}

		public class ControlTypeNotImplemented : Exception
		{
			public ControlTypeNotImplemented(string controlType)
				: base(controlType) {}
		}

		internal static void LoadControl(Control control, BinaryReader reader, byte[] versionBytes)
		{
			control.SetComponents(LoadComponents(control, reader, versionBytes));
			var tagCount = reader.ReadByte();
			if (tagCount > 0)
			{
				var arrayType1 = reader.ReadByte();
				reader.ReadString();
				for (int i = 0; i < tagCount; i++)
					control.AddTag(reader.ReadString());
			}
			var behaviorCount = reader.ReadByte();
			var arrayType2 = reader.ReadByte();
			reader.ReadString();
			for (int i = 0; i < behaviorCount; i++)
				reader.ReadString();
			var hasOneDrawBehavior = reader.ReadByte();
			var arrayType3 = reader.ReadByte();
			reader.ReadString();
			reader.ReadString();
			control.Start<UpdateRenderingCalculations>();
			control.Start<ControlUpdater>();
			control.OnDraw<SpriteRenderer>();
		}

		private static IEnumerable<object> LoadComponents(Entity entity, BinaryReader reader,
			byte[] versionBytes)
		{
			int count = reader.ReadByte();
			if (count == 255)
				count = reader.ReadInt32();
			var components = new List<object>(count);
			var arrayElementType = (ArrayElementType)reader.ReadByte();
			if (arrayElementType == ArrayElementType.AllTypesAreNull)
				return components;
			if (arrayElementType == ArrayElementType.AllTypesAreDifferent)
				LoadComponentsOfDifferentTypes(components, reader, count, versionBytes);
			else
			{
				Type elementType = arrayElementType == ArrayElementType.AllTypesAreTypes
					? typeof(Type)
					: BinaryDataExtensions.GetTypeFromShortNameOrFullNameIfNotFound(reader.ReadString());
				LoadComponentsOfTheSameType(components, reader, count, elementType, versionBytes);
			}
			return components;
		}

		public enum ArrayElementType : byte
		{
			AllTypesAreDifferent,
			AllTypesAreTypes,
			AllTypesAreTheSame,
			AllTypesAreNull
		}

		private static void LoadComponentsOfTheSameType(List<object> components, BinaryReader reader,
			int count, Type elementType, byte[] versionBytes)
		{
			for (int i = 0; i < count; i++)
				components.Add(LoadComponent(elementType, reader, versionBytes));
		}

		private static object LoadComponent(Type elementType, BinaryReader reader,
			byte[] versionBytes)
		{
			Console.WriteLine(elementType);
			if (elementType == typeof(Rectangle))
				return LoadRectangle(reader);
			if (elementType == typeof(String))
				return reader.ReadString();
			if (elementType == typeof(Boolean))
				return reader.ReadBoolean();
			if (elementType == typeof(Int32))
				return reader.ReadInt32();
			if (elementType == typeof(Color))
				return new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
			if (elementType == typeof(Material))
				return LoadMaterial(reader);
			if (elementType == typeof(BlendMode))
				return (BlendMode)reader.ReadInt32();
			if (elementType == typeof(InteractiveState))
				return LoadInteractiveState(reader);
			if (elementType == typeof(AnchoringState))
				return LoadAnchoringState(reader, versionBytes);
			if (elementType == typeof(Theme))
			{
				var justSaveContentName = reader.ReadBoolean();
				if (justSaveContentName)
					throw new Exception();
				return LoadTheme(reader);
			}
			if (elementType == typeof(FontText))
				return LoadFontText(reader, versionBytes);
			if (elementType == typeof(GlyphDrawData[]))
				return LoadGlyphDrawData(reader);
			if (elementType == typeof(Size))
				return new Size(reader.ReadSingle(), reader.ReadSingle());
			if (elementType == typeof(Vector2D))
				return new Vector2D(reader.ReadSingle(), reader.ReadSingle());
			if (elementType == typeof(FontText.FontName))
				return LoadFontName(reader);
			if (elementType == typeof(RenderingData))
				return LoadRenderingData(reader);
			if (elementType == typeof(Slider.Data))
				return LoadSliderData(reader);
			if (elementType == typeof(Picture))
				return LoadPicture(reader, versionBytes);
			throw new Exception();
		}

		private static Rectangle LoadRectangle(BinaryReader reader)
		{
			return new Rectangle(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
				reader.ReadSingle());
		}

		private static Material LoadMaterial(BinaryReader reader)
		{
			var justSaveContentName = reader.ReadBoolean();
			if (justSaveContentName)
				return ContentLoader.Load<Material>(reader.ReadString());
			return LoadCustomMaterial(reader);
		}

		internal static Material LoadCustomMaterial(BinaryReader reader)
		{
			var shaderFlags = (ShaderFlags)reader.ReadInt32();
			var customImageType = reader.ReadByte();
			var pixelSize = customImageType > 0
				? new Size(reader.ReadSingle(), reader.ReadSingle()) : Size.Zero;
			var imageOrAnimationName = customImageType > 0 ? "" : reader.ReadString();
			var customImage = customImageType == 1
				? ContentLoader.Create<Image>(new ImageCreationData(pixelSize)) : null;
			var color = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(),
				reader.ReadByte());
			var duration = reader.ReadSingle();
			var material = customImageType > 0
				? new Material(ContentLoader.Create<Shader>(new ShaderCreationData(shaderFlags)),
					customImage)
				: new Material(shaderFlags, imageOrAnimationName);
			material.DefaultColor = color;
			material.Duration = duration;
			return material;
		}

		private static void LoadComponentsOfDifferentTypes(List<object> components,
			BinaryReader reader, int count, byte[] versionBytes)
		{
			for (int i = 0; i < count; i++)
				LoadNextComponent(components, reader, versionBytes);
		}

		private static void LoadNextComponent(List<object> components, BinaryReader reader,
			byte[] versionBytes)
		{
			var name = reader.ReadString();
			Type elementType = BinaryDataExtensions.GetTypeFromShortNameOrFullNameIfNotFound(name);
			bool isNotNull = reader.ReadBoolean();
			if (isNotNull)
				components.Add(LoadComponent(elementType, reader, versionBytes));
		}

		private static InteractiveState LoadInteractiveState(BinaryReader reader)
		{
			var state = new InteractiveState
			{
				IsInside = reader.ReadBoolean(),
				IsPressed = reader.ReadBoolean(),
				IsSelected = reader.ReadBoolean(),
				RelativePointerPosition = new Vector2D(reader.ReadSingle(), reader.ReadSingle()),
				CanHaveFocus = reader.ReadBoolean(),
				HasFocus = reader.ReadBoolean(),
				WantsFocus = reader.ReadBoolean(),
				DragStart = new Vector2D(reader.ReadSingle(), reader.ReadSingle()),
				DragEnd = new Vector2D(reader.ReadSingle(), reader.ReadSingle()),
				DragDone = reader.ReadBoolean()
			};
			return state;
		}

		private static AnchoringState LoadAnchoringState(BinaryReader reader, byte[] versionBytes)
		{
			var state = new AnchoringState
			{
				TopMargin = LoadMargin(reader),
				BottomMargin = LoadMargin(reader),
				LeftMargin = LoadMargin(reader),
				RightMargin = LoadMargin(reader),
				PercentageSpan = reader.ReadSingle(),
				NoIdea = reader.ReadInt32()
			};
			return state;
		}

		private static Margin LoadMargin(BinaryReader reader)
		{
			var otherControlID = reader.ReadInt32();
			var edge = (Edge)reader.ReadInt32();
			var distance = reader.ReadSingle();
			return new Margin("control" + otherControlID, edge, distance);
		}

		private static Theme LoadTheme(BinaryReader reader)
		{
			var theme = new Theme();
			var isNonNullInstance = reader.ReadBoolean();
			var name = reader.ReadString();
			var isDisposed = reader.ReadBoolean();
			theme.Label = LoadMaterialWithHeader(reader);
			theme.Button = LoadMaterialWithHeader(reader);
			theme.ButtonDisabled = LoadMaterialWithHeader(reader);
			theme.ButtonMouseover = LoadMaterialWithHeader(reader);
			theme.ButtonPressed = LoadMaterialWithHeader(reader);
			theme.DropdownListBox = LoadMaterialWithHeader(reader);
			theme.DropdownListBoxDisabled = LoadMaterialWithHeader(reader);
			theme.RadioButtonBackground = LoadMaterialWithHeader(reader);
			theme.RadioButtonBackgroundDisabled = LoadMaterialWithHeader(reader);
			theme.RadioButtonDisabled = LoadMaterialWithHeader(reader);
			theme.RadioButtonNotSelected = LoadMaterialWithHeader(reader);
			theme.RadioButtonNotSelectedMouseover = LoadMaterialWithHeader(reader);
			theme.RadioButtonSelected = LoadMaterialWithHeader(reader);
			theme.RadioButtonSelectedMouseover = LoadMaterialWithHeader(reader);
			theme.Scrollbar = LoadMaterialWithHeader(reader);
			theme.ScrollbarDisabled = LoadMaterialWithHeader(reader);
			theme.ScrollbarPointerMouseover = LoadMaterialWithHeader(reader);
			theme.ScrollbarPointerDisabled = LoadMaterialWithHeader(reader);
			theme.ScrollbarPointer = LoadMaterialWithHeader(reader);
			theme.SelectBox = LoadMaterialWithHeader(reader);
			theme.SelectBoxDisabled = LoadMaterialWithHeader(reader);
			theme.Slider = LoadMaterialWithHeader(reader);
			theme.SliderDisabled = LoadMaterialWithHeader(reader);
			theme.SliderPointer = LoadMaterialWithHeader(reader);
			theme.SliderPointerDisabled = LoadMaterialWithHeader(reader);
			theme.SliderPointerMouseover = LoadMaterialWithHeader(reader);
			theme.TextBox = LoadMaterialWithHeader(reader);
			theme.TextBoxFocused = LoadMaterialWithHeader(reader);
			theme.TextBoxDisabled = LoadMaterialWithHeader(reader);
			return theme;
		}

		private static Material LoadMaterialWithHeader(BinaryReader reader)
		{
			var notNull = reader.ReadBoolean();
			var name = reader.ReadString();
			return LoadMaterial(reader);
		}

		private static FontText LoadFontText(BinaryReader reader, byte[] versionBytes)
		{
			var fontText = new FontText();
			fontText.SetComponents(LoadComponents(fontText, reader, versionBytes));
			var hasZeroTags = reader.ReadByte();
			var hasZeroBehaviors = reader.ReadByte();
			var hasOneDrawBehavior = reader.ReadByte();
			reader.ReadByte();
			reader.ReadString();
			reader.ReadString();
			fontText.OnDraw<FontRenderer>();
			return fontText;
		}

		private static GlyphDrawData[] LoadGlyphDrawData(BinaryReader reader)
		{
			int count = reader.ReadByte();
			if (count == 255)
				count = reader.ReadInt32();
			var data = new GlyphDrawData[count];
			if (count == 0)
				return data;
			var arrayType = reader.ReadByte();
			var name = reader.ReadString();
			for (int i = 0; i < count; i++)
			{
				data[i].DrawArea = LoadRectangle(reader);
				data[i].UV = LoadRectangle(reader);
			}
			return data;
		}

		private static FontText.FontName LoadFontName(BinaryReader reader)
		{
			var notNull = reader.ReadBoolean();
			return new FontText.FontName(reader.ReadString());
		}

		private static RenderingData LoadRenderingData(BinaryReader reader)
		{
			var data = new RenderingData();
			data.RequestedUserUV = LoadRectangle(reader);
			data.RequestedDrawArea = LoadRectangle(reader);
			data.FlipMode = (FlipMode)reader.ReadInt32();
			data.AtlasUV = LoadRectangle(reader);
			data.DrawArea = LoadRectangle(reader);
			data.HasSomethingToRender = reader.ReadBoolean();
			data.IsAtlasRotated = reader.ReadBoolean();
			return data;
		}

		private static Slider.Data LoadSliderData(BinaryReader reader)
		{
			var data = new Slider.Data();
			data.MinValue = reader.ReadInt32();
			data.Value = reader.ReadInt32();
			data.MaxValue = reader.ReadInt32();
			return data;
		}

		private static Picture LoadPicture(BinaryReader reader, byte[] versionBytes)
		{
			var picture = new Picture();
			picture.SetComponents(LoadComponents(picture, reader, versionBytes));
			var hasZeroTags = reader.ReadByte();
			var hasTwoBehaviors = reader.ReadByte();
			var arrayType1 = reader.ReadByte();
			var x = reader.ReadString();
			var y = reader.ReadString();
			var z = reader.ReadString();
			var hasOneDrawBehavior = reader.ReadByte();
			var arrayType2 = reader.ReadByte();
			var a = reader.ReadString();
			var b = reader.ReadString();
			picture.Start<UpdateRenderingCalculations>();
			picture.Start<ControlUpdater>();
			picture.OnDraw<SpriteRenderer>();
			return picture;
		}
	}
}