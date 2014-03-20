using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// Most basic visible control which is just a Sprite which can change appearance on request
	/// </summary>
	public class Picture : Control
	{
		internal protected Picture() {}

		public Picture(Theme theme, Material material, Rectangle drawArea)
			: base(drawArea)
		{
			Theme = theme;
			SetAppearanceWithoutInterpolation(material);
		}

		public Theme Theme
		{
			get
			{
				if (Contains<Theme>())
					return Get<Theme>();
				//ncrunch: no coverage start
				var theme = new Theme(); 
				Add(theme);
				return theme;
				//ncrunch: no coverage end
			}
			set { Set(value); }
		}

		public void SetAppearanceWithoutInterpolation(Material material)
		{
			if (material == null)
				return;
			Material = material;
			SetWithoutInterpolation(material.DefaultColor);
		}

		public void SetAppearance(Material material)
		{
			if (material == null)
				return;
			Material = material;
			Color = material.DefaultColor;
		}

		//ncrunch: no coverage start
		internal void LoadFromStream(Stream fileData)
		{
			fileData.Seek(0, SeekOrigin.Begin);
			var reader = new BinaryReader(fileData);
			string shortName = reader.ReadString();
			var dataVersion = reader.ReadBytes(4);
			Scene.LoadControl(this, reader, dataVersion);
		}
	}
}