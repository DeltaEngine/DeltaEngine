using System.Globalization;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Converters;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.LevelEditor
{
	public static class StringToVectorConverter
	{
		public static Vector3D? Convert(string value)
		{
			var stringSplit = value.SplitAndTrim(new[] { ',' });
			if (stringSplit.Length != 3)
				return null;
			var converter = new FloatStringConverter();
			var x = converter.ConvertBack(stringSplit[0], typeof(float), null,
				CultureInfo.CurrentCulture);
			var y = converter.ConvertBack(stringSplit[1], typeof(float), null,
				CultureInfo.CurrentCulture);
			var z = converter.ConvertBack(stringSplit[2], typeof(float), null,
				CultureInfo.CurrentCulture);
			return new Vector3D((float)x, (float)y, (float)z);
		}
	}
}