using System;
using System.IO;
using System.Text;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering3D.Particles;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace DeltaEngine.Content.Mocks
{
	/// <summary>
	/// Loads mock content used in unit tests.
	/// </summary>
	public class MockContentLoader : ContentLoader
	{
		//ncrunch: no coverage start (for performance reasons)
		protected override Stream GetContentDataStream(ContentData content)
		{
			var stream = Stream.Null;
			if (content.MetaData.Type == ContentType.Xml && content.Name.Equals("Texts"))
				stream = new XmlFile(new XmlData("Texts").AddChild(GoLocalizationNode)).ToMemoryStream();
			else if (content.Name.Contains("Verdana12") || content.Name.Contains("Tahoma30"))
				stream = CreateFontXml().ToMemoryStream();
			else if (content.MetaData.Type == ContentType.Xml && content.Name != "NonExisting")
				stream = new XmlFile(new XmlData("Root").AddChild(new XmlData("Hi"))).ToMemoryStream();
			else if (content.Name.Equals("EmptyScene"))
				stream = SaveEmptyScene();
			else if (content.Name.Equals("SceneWithAButton"))
				stream = SaveSceneWithAButton();
			else if (content.Name.Equals("SceneWithAButtonWithChangedMaterial"))
				stream = SaveSceneWithAButtonWithMaterial();
			else if (content.Name.Equals("SceneWithAButtonWithDifferentFontText"))
				stream = SaveSceneWithAButtonWithFontText();
			else if (content.Name.Equals("SceneWithASlider"))
				stream = SaveSceneWithASlider();
			else if (content.MetaData.Type == ContentType.Scene)
				stream = SaveEmptyScene();
			else if (content.Name.Contains("Theme"))
				stream = SaveTestTheme();
			else if (content.Name.Equals("TestMenuXml"))
				stream = SaveTestMenu();
			else if (content.Name.Equals("3DMaterial"))
				stream = Save3DMaterial();
			else if (content.Name.Contains("PointEmitter3D"))
				stream = CreateEmitter3D(ParticleEmitterPositionType.Point);
			else if (content.Name.Contains("LineEmitter3D"))
				stream = CreateEmitter3D(ParticleEmitterPositionType.Line);
			else if (content.Name.Contains("BoxEmitter3D"))
				stream = CreateEmitter3D(ParticleEmitterPositionType.Box);
			else if (content.Name.Contains("SphereEmitter3D"))
				stream = CreateEmitter3D(ParticleEmitterPositionType.Sphere);
			else if (content.Name.Contains("SphereBorderEmitter3D"))
				stream = CreateEmitter3D(ParticleEmitterPositionType.SphereBorder);
			else if (content.Name.Contains("CircleAroundCenterEmitter3D"))
				stream = CreateEmitter3D(ParticleEmitterPositionType.CircleAroundCenter);
			else if (content.Name.Contains("CircleEscapingEmitter3D"))
				stream = CreateEmitter3D(ParticleEmitterPositionType.CircleEscaping);
			else if (content.Name.Contains("MeshEmitter3D"))
				stream = CreateEmitter3D(ParticleEmitterPositionType.Mesh);
			else if (content.MetaData.Type == ContentType.ParticleEmitter)
				stream = SaveTestEmitter();
			else if (content.Name == "SpineboyAtlas")
				stream = new MemoryStream(Encoding.Default.GetBytes(MockSpineAtlas.Text));
			else if (content.Name == "SpineboySkeleton")
				stream = new MemoryStream(Encoding.Default.GetBytes(MockSpineSkeleton.Text));
			else if (content.Name.Contains("MyMaterial"))
				stream = SaveMaterial();
			return stream;
		}

		protected override bool HasValidContentAndMakeSureItIsLoaded()
		{
			return true;
		}

		public override DateTime LastTimeUpdated
		{
			get { return DateTime.Now; }
		}

		private static XmlData GoLocalizationNode
		{
			get
			{
				var keyNode = new XmlData("Go");
				keyNode.AddAttribute("en", "Go");
				keyNode.AddAttribute("de", "Los");
				keyNode.AddAttribute("es", "¡vamos!");
				return keyNode;
			}
		}

		private static XmlFile CreateFontXml()
		{
			var glyph1 = CreateGlyph("Glyph", ' ');
			var glyph2 = CreateGlyph("Glyph", 'a');
			var glyph3 = CreateGlyph("Glyph", 'A');
			var glyphs = new XmlData("Glyphs").AddChild(glyph1).AddChild(glyph2).AddChild(glyph3);
			var kerningPair = CreateKerningPair("Kerning");
			var kernings = new XmlData("Kernings");
			kernings.AddChild(kerningPair);
			var bitmap = CreateBitmap("Bitmap");
			var font = CreateFont("Font");
			font.AddChild(bitmap).AddChild(glyphs).AddChild(kernings);
			return new XmlFile(font);
		}

		private static XmlData CreateGlyph(string name, char character)
		{
			var xmlData = new XmlData(name);
			xmlData.AddAttribute("Character", character);
			xmlData.AddAttribute("UV", "0 0 1 16");
			xmlData.AddAttribute("AdvanceWidth", "7.34875");
			xmlData.AddAttribute("LeftBearing", "0");
			xmlData.AddAttribute("RightBearing", "4.21875");
			return xmlData;
		}

		private static XmlData CreateKerningPair(string name)
		{
			var xmlData = new XmlData(name);
			xmlData.AddAttribute("First", " ");
			xmlData.AddAttribute("Second", "a");
			xmlData.AddAttribute("Distance", "1");
			return xmlData;
		}

		private static XmlData CreateBitmap(string name)
		{
			var xmlData = new XmlData(name);
			xmlData.AddAttribute("Name", "Verdana12Font");
			xmlData.AddAttribute("Width", "128");
			xmlData.AddAttribute("Height", "128");
			return xmlData;
		}

		private static XmlData CreateFont(string name)
		{
			var xmlData = new XmlData(name);
			xmlData.AddAttribute("Family", "Verdana");
			xmlData.AddAttribute("Size", "12");
			xmlData.AddAttribute("Style", "AddOutline");
			xmlData.AddAttribute("LineHeight", "16");
			return xmlData;
		}

		private static MemoryStream SaveTestEmitter()
		{
			var particleEmitterData = new ParticleEmitterData();
			var shader = Create<Shader>(new ShaderCreationData(ShaderFlags.Position2DColoredTextured));
			var image = Create<Image>(new ImageCreationData(new Size(8.0f, 8.0f)));
			particleEmitterData.ParticleMaterial = new Material(shader, image);
			particleEmitterData.Size = new RangeGraph<Size>(new Size(0.1f, 0.1f));
			particleEmitterData.MaximumNumberOfParticles = 256;
			particleEmitterData.LifeTime = 5.0f;
			particleEmitterData.SpawnInterval = 0.1f;
			particleEmitterData.Color = new RangeGraph<Color>(Color.Red, Color.Green);
			var data = BinaryDataExtensions.SaveToMemoryStream(particleEmitterData);
			data.Seek(0, SeekOrigin.Begin);
			return data;
		}

		private static MemoryStream SaveEmptyScene()
		{
			var emptyScene = new Scene();
			var data = BinaryDataExtensions.SaveToMemoryStream(emptyScene);
			data.Seek(0, SeekOrigin.Begin);
			return data;
		}

		private static Stream SaveSceneWithAButton()
		{
			var scene = new Scene();
			var button = new Button(new Theme(), Rectangle.One, "Hello");
			button.RenderLayer = 5;
			scene.Controls.Add(button);
			var data = BinaryDataExtensions.SaveToMemoryStream(scene);
			data.Seek(0, SeekOrigin.Begin);
			return data;
		}

		private static Stream SaveSceneWithAButtonWithMaterial()
		{
			var scene = new Scene();
			scene.Controls.Add(new Button(new Theme(), Rectangle.One, "Hello"));
			var material = new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogo");
			scene.Controls[0].Get<Theme>().Button = material;
			scene.Controls[0].Get<Theme>().ButtonDisabled = material;
			scene.Controls[0].Get<Theme>().ButtonMouseover = material;
			scene.Controls[0].Get<Theme>().ButtonPressed = material;
			var data = BinaryDataExtensions.SaveToMemoryStream(scene);
			data.Seek(0, SeekOrigin.Begin);
			return data;
		}

		private static Stream SaveSceneWithAButtonWithFontText()
		{
			var scene = new Scene();
			scene.Controls.Add(new Button(new Theme(), Rectangle.One, "Hello"));
			var data = BinaryDataExtensions.SaveToMemoryStream(scene);
			data.Seek(0, SeekOrigin.Begin);
			return data;
		}

		private static Stream SaveSceneWithASlider()
		{
			var scene = new Scene();
			scene.Controls.Add(new Slider(new Theme(), Rectangle.One));
			var data = BinaryDataExtensions.SaveToMemoryStream(scene);
			data.Seek(0, SeekOrigin.Begin);
			return data;
		}

		private static MemoryStream SaveTestMenu()
		{
			var emptyScene = new AutoArrangingMenu(Size.One);
			var data = BinaryDataExtensions.SaveToMemoryStream(emptyScene);
			data.Seek(0, SeekOrigin.Begin);
			return data;
		}

		private static Stream SaveTestTheme()
		{
			var theme = new Theme();
			theme.SelectBox = new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogo")
			{
				DefaultColor = Color.Blue
			};
			var data = BinaryDataExtensions.SaveToMemoryStream(theme);
			data.Seek(0, SeekOrigin.Begin);
			return data;
		}

		private static MemoryStream Save3DMaterial()
		{
			var material = new Material(ShaderFlags.ColoredTextured, "DeltaEngineLogo");
			var data = BinaryDataExtensions.SaveToMemoryStream(material);
			data.Seek(0, SeekOrigin.Begin);
			return data;
		}

		private static Stream CreateEmitter3D(ParticleEmitterPositionType positionType)
		{
			var pointEmitter = new ParticleEmitterData();
			pointEmitter.ParticleMaterial = new Material(ShaderFlags.ColoredTextured, "DeltaEngineLogo");
			pointEmitter.PositionType = positionType;
			var data = BinaryDataExtensions.SaveToMemoryStream(pointEmitter);
			data.Seek(0, SeekOrigin.Begin);
			return data;
		}

		private static Stream SaveMaterial()
		{
			var image = Create<Image>(new ImageCreationData(new Size(8, 8)));
			var material = new Material(Create<Shader>(
				new ShaderCreationData(ShaderFlags.Position2DColoredTextured)), image);
			var data = BinaryDataExtensions.SaveToMemoryStream(material);
			data.Seek(0, SeekOrigin.Begin);
			return data;
		}

		public override ContentMetaData GetMetaData(string contentName, Type contentClassType = null)
		{
			if (IsNoMetaDataAllowed(contentName, contentClassType) || string.IsNullOrEmpty(contentName))
				return null;
			ContentType contentType = ConvertClassTypeToContentType(contentClassType);
			if (contentType == ContentType.Material || contentName.Contains("NewMaterial"))
				return CreateMaterialMetaData(contentName);
			if (contentName.Contains("SpriteSheet") || contentType == ContentType.SpriteSheetAnimation)
				return CreateSpriteSheetAnimationMetaData(contentName);
			if (contentName.Contains("Scene") || contentType == ContentType.Scene)
				return CreateSceneMetaData(contentName);
			if (contentName == "ImageAnimationNoImages")
				return CreateImageAnimationNoImagesMetaData(contentName);
			if (contentName.EndsWith("ImageAnimation") || contentType == ContentType.ImageAnimation)
				return CreateImageAnimationMetaData(contentName);
			if (contentType == ContentType.Image || contentName == "DeltaEngineLogo")
				return CreateImageMetaData(contentName);
			if (contentType == ContentType.Model)
				return CreateModelMetaData(contentName);
			if (contentType == ContentType.Mesh)
				return CreateMeshMetaData(contentName);
			if (contentType == ContentType.Shader)
				return CreateShaderData(contentName);
			if (contentType == ContentType.Video)
				return CreateVideoData(contentName);
			if (contentType == ContentType.Sound)
				return CreateSoundData(contentName);
			if (contentType == ContentType.ParticleSystem || contentName.Contains("LoadParticleSystem"))
				return CreateParticleSystemMetaData(contentName);
			return new ContentMetaData { Name = contentName, Type = contentType };
		}

		private static bool IsNoMetaDataAllowed(string contentName, Type classOfContent)
		{
			return contentName.StartsWith("Unavailable") || contentName.StartsWith("NoData") ||
				(classOfContent != null && classOfContent.Name.StartsWith("NoData"));
		}

		private static ContentType ConvertClassTypeToContentType(Type contentClassType)
		{
			if (contentClassType == null)
				return ContentType.Xml;
			var typeName = contentClassType.Name;
			if (typeName.Contains("ImageAnimation"))
				return ContentType.ImageAnimation;
			if (typeName.Contains("Image") || typeName.Contains("Texture"))
				return ContentType.Image;
			if (typeName.Contains("Font"))
				return ContentType.Font;
			if (typeName.Contains("Xml"))
				return ContentType.Xml;
			if (typeName.Contains("InputCommands"))
				return ContentType.InputCommand;
			if (typeName.Contains("Material"))
				return ContentType.Material;
			if (typeName.Contains("Music"))
				return ContentType.Music;
			if (typeName.Contains("Sound"))
				return ContentType.Sound;
			if (typeName.Contains("Video"))
				return ContentType.Video;
			if (typeName.Contains("MeshAnimation"))
				return ContentType.MeshAnimation; // ncrunch: no coverage (slow test)
			if (typeName.Contains("Mesh"))
				return ContentType.Mesh;
			if (typeName.Contains("Geometry"))
				return ContentType.Geometry;
			if (typeName.Contains("ModelData"))
				return ContentType.Model;
			if(typeName.Contains("ParticleEmitterData"))
				return ContentType.ParticleEmitter;
			if (typeName.Contains("ParticleSystemData"))
				return ContentType.ParticleSystem;
			if (typeName.Contains("AtlasData") || typeName.Contains("SkeletonData"))
				return ContentType.JustStore;
			foreach (var contentType in EnumExtensions.GetEnumValues<ContentType>())
				if (contentType != ContentType.Image && contentType != ContentType.Mesh &&
					typeName.Contains(contentType.ToString()))
					return contentType;
			return ContentType.Xml;
		}

		private static ContentMetaData CreateMaterialMetaData(string name)
		{
			var metaData = new ContentMetaData { Name = name, Type = ContentType.Material };
			if (!name.Contains("NoShader"))
				AddShaderNameToMetaData(metaData);
			if (!name.Contains("NoImage"))
				if (name.Contains("ImageAnimation"))
					metaData.Values.Add("ImageOrAnimationName", "ImageAnimation");
				else if (name.Contains("SpriteSheet"))
					metaData.Values.Add("ImageOrAnimationName", "SpriteSheet");
				else
					metaData.Values.Add("ImageOrAnimationName", "DeltaEngineLogo");
			metaData.Values.Add("LightMapName", "lightMap");
			return metaData;
		}

		private static void AddShaderNameToMetaData(ContentMetaData metaData)
		{
			bool isData3D = metaData.Name.Contains("3D") || metaData.Name.Contains("Mesh") ||
				metaData.Name.Contains("Box");
			ShaderFlags finalFlags = isData3D ? ShaderFlags.Textured : ShaderFlags.Position2DTextured;
			if (metaData.Name.Contains("Lightmap"))
				finalFlags |= ShaderFlags.LightMap;
			if (metaData.Name.Contains("Animated"))
				finalFlags |= ShaderFlags.Skinned;
			metaData.Values.Add("ShaderFlags", finalFlags.ToString());
		}

		private static ContentMetaData CreateSpriteSheetAnimationMetaData(string name)
		{
			var metaData = new ContentMetaData { Name = name, Type = ContentType.SpriteSheetAnimation };
			metaData.Values.Add("ImageName", "EarthImages");
			metaData.Values.Add("DefaultDuration", "1.0");
			metaData.Values.Add("SubImageSize", "107, 80");
			return metaData;
		}

		private static ContentMetaData CreateSceneMetaData(string contentName)
		{
			var metaData = new ContentMetaData { Name = contentName, Type = ContentType.Scene };
			return metaData;
		}

		private static ContentMetaData CreateImageAnimationMetaData(string name)
		{
			var metaData = new ContentMetaData { Name = name, Type = ContentType.ImageAnimation };
			metaData.Values.Add("ImageNames", "ImageAnimation01,ImageAnimation02,ImageAnimation03");
			metaData.Values.Add("DefaultDuration", "3");
			return metaData;
		}

		private static ContentMetaData CreateImageAnimationNoImagesMetaData(string name)
		{
			var metaData = new ContentMetaData { Name = name, Type = ContentType.ImageAnimation };
			metaData.Values.Add("ImageNames", "");
			metaData.Values.Add("DefaultDuration", "3");
			return metaData;
		}

		private static ContentMetaData CreateImageMetaData(string contentName)
		{
			var metaData = new ContentMetaData { Name = contentName, Type = ContentType.Image };
			metaData.Values.Add("PixelSize", contentName == "EarthImages" ? "428, 240" : "128, 128");
			if (contentName.Contains("ParticleFire"))
				metaData.Values.Add("BlendMode", "Additive");
			return metaData;
		}

		private static ContentMetaData CreateModelMetaData(string contentName)
		{
			var metaData = new ContentMetaData { Name = contentName, Type = ContentType.Model };
			if (contentName == "InvalidModel")
				return metaData;
			metaData.Values.Add("MeshNames", "Mock" + contentName);
			return metaData;
		}

		private static ContentMetaData CreateMeshMetaData(string contentName)
		{
			var metaData = new ContentMetaData { Name = contentName, Type = ContentType.Mesh };
			metaData.Values.Add("GeometryName", contentName + "Geometry");
			metaData.Values.Add("MaterialName", contentName + "Material");
			if (contentName.Contains("Animation"))
				metaData.Values.Add("AnimationName", contentName + "Animation");
			if (contentName.Contains("CustomTransform"))
				metaData.Values.Add("LocalTransform", "2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 1, 3, 5, 1");
			return metaData;
		}

		private static ContentMetaData CreateShaderData(string contentName)
		{
			var metaData = new ContentMetaData { Name = contentName, Type = ContentType.Shader };
			metaData.Values.Add("Flags", "UI2D, UV, Color");
			return metaData;
		}

		private static ContentMetaData CreateVideoData(string contentName)
		{
			if (contentName.Contains("No"))
				throw new Video.VideoNotFoundOrAccessible(contentName, null);
			return new ContentMetaData { Name = contentName, Type = ContentType.Video };
		}

		private static ContentMetaData CreateSoundData(string contentName)
		{
			var metaData = new ContentMetaData { Name = contentName, Type = ContentType.Sound };
			metaData.Values.Add("Length", "1.3");
			return metaData;
		}

		private static ContentMetaData CreateParticleSystemMetaData(string contentName)
		{
			var metaData = new ContentMetaData
			{
				Name = contentName,
				Type = ContentType.ParticleSystem
			};
			metaData.Values.Add("EmitterNames", "EmitterAlpha, EmitterBeta");
			return metaData;
		}

		public string GetContentMetaDataFilePath()
		{
			return ContentMetaDataFilePath;
		}
	}
}