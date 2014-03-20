namespace DeltaEngine.Editor.Messages
{
	/// <summary>
	/// Request to change meta data of a content file in a project
	/// </summary>
	public class ChangeMetaData
	{
		public ChangeMetaData(string contentName, string attributeName, string attributeValue)
		{
			ContentName = contentName;
			AttributeName = attributeName;
			AttributeValue = attributeValue;
		}

		public string ContentName { get; set; }
		protected object AttributeName { get; set; }
		protected string AttributeValue { get; set; }
	}
}