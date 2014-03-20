using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// App settings loaded from and saved to file.
	/// </summary>
	public class FileSettings : Settings
	{
		//ncrunch: no coverage start
		public FileSettings()
		{
			if (Current == null)
				Current = this;
			filePath = Path.Combine(GetMyDocumentsAppFolder(), SettingsFilename);
			CustomSettingsExists = File.Exists(filePath);
			data = CustomSettingsExists ? new XmlFile(filePath).Root : new XmlData("Settings");
		}

		private readonly string filePath;
		private XmlData data;

		public override void LoadDefaultSettings()
		{
			var dataChangedBeforeLoading = data;
			data = ContentLoader.Load<XmlContent>("DefaultSettings").Data;
			if (dataChangedBeforeLoading != null)
				foreach (var child in dataChangedBeforeLoading.Children)
					SetValue(child.Name, child.Value); //ncrunch: no coverage
		}

		public override void Save()
		{
			new XmlFile(data).Save(filePath);
		}

		public override T GetValue<T>(string name, T defaultValue)
		{
			return data == null ? defaultValue : data.GetChildValue(name, defaultValue);
		}

		public override void SetValue(string name, object value)
		{
			if (data.GetChild(name) == null)
				data.AddChild(name, StringExtensions.ToInvariantString(value)); //ncrunch: no coverage
			else
				data.GetChild(name).Value = StringExtensions.ToInvariantString(value);
			wasChanged = true;
		}
	}
}