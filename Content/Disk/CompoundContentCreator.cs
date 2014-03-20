using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DeltaEngine.Content.Disk
{
	internal class CompoundContentCreator
	{
		//ncrunch: no coverage start
		public CompoundContentCreator()
		{
			TryCreatingAnimations = true;
		}

		public bool TryCreatingAnimations { get; set; }

		public XDocument AddCompoundElementsToDocument(XDocument metaData)
		{
			if (TryCreatingAnimations)
				metaData = AddSequenceAnimations(metaData);
			return metaData;
		}

		private XDocument AddSequenceAnimations(XDocument metaData)
		{
			foreach (var contentElement in metaData.Root.Elements())
				if (NameFitsFirstSequenceElement(contentElement.Attribute("Name").Value) &&
						contentElement.Attribute("Type").Value == "Image")
				{
					var animation = BuildAnimationFromInitialName(contentElement.Attribute("Name").Value,
						metaData);
					metaData.Root.Add(animation);
				}
			return metaData;
		}

		private static bool NameFitsFirstSequenceElement(string contentName)
		{
			return Regex.IsMatch(contentName, @"\w.*0*" + "1" + @"\z");
		}

		private XElement BuildAnimationFromInitialName(string firstContentName, XDocument metaData)
		{
			var animationElement = new XElement("ContentMetaData");
			animationElement.Add(new XAttribute("Name", firstContentName.TrimEnd(trimNumbers)));
			animationElement.Add(new XAttribute("Type", "ImageAnimation"));
			var imagesString = firstContentName;
			var baseName = firstContentName.TrimEnd(trimNumbers);
			var sequenceList = GetFollowingSequenceForAttribute(baseName, 2, metaData);
			var duration =
				(((float)sequenceList.Count + 1) * 0.1f).ToString(CultureInfo.InvariantCulture);
			animationElement.Add(new XAttribute("Duration", duration));
			foreach (var sequenceName in sequenceList)
				imagesString += "," + sequenceName;
			animationElement.Add(new XAttribute("ImageNames", imagesString));
			return animationElement;
		}

		private static List<string> GetFollowingSequenceForAttribute(string baseName,
			int minSequenceNumber, XDocument metaData)
		{
			var sequenceList = new List<string>();
			var iterator = minSequenceNumber;
			var nextOneExistant = true;
			while (nextOneExistant)
			{
				var nextName = GetSequenceElementName(baseName, iterator, metaData);
				if (nextName == "")
					nextOneExistant = false;
				else
					sequenceList.Add(nextName);
				iterator++;
			}
			return sequenceList;
		}

		private static string GetSequenceElementName(string baseName, int sequenceNumber,
			XDocument metaData)
		{
			foreach (var element in metaData.Root.Elements())
			{
				var name = element.Attribute("Name").Value;
				if (Regex.IsMatch(name,
					baseName + "0*" + sequenceNumber.ToString(CultureInfo.InvariantCulture)))
					return name;
			}
			return "";
		}

		private readonly char[] trimNumbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
	}
}