using System;
using System.Globalization;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Allows to write out date values as structured iso date strings and parses iso or english dates
	/// </summary>
	public static class DateExtensions
	{
		public static string GetIsoDateTime(this DateTime dateTime)
		{
			return GetIsoDate(dateTime) + " " + GetIsoTime(dateTime);
		}

		public static string GetIsoDate(this DateTime date)
		{
			return date.ToString("yyyy-MM-dd");
		}

		public static string GetIsoTime(this DateTime time)
		{
			return time.ToString("HH:mm:ss");
		}

		public static bool IsDateNewerByOneSecond(DateTime newerDate, DateTime olderDate)
		{
			return (newerDate - olderDate).TotalSeconds > 1;
		}

		public static DateTime Parse(string dateString)
		{
			dateString = dateString != null ? dateString.Trim() : "";
			if (dateString.Length == 0)
				return DateTime.MinValue;
			DateTime result;
			return DateTime.TryParse(dateString, new CultureInfo("en-US", false),
				DateTimeStyles.AssumeLocal, out result) ? result : DateTime.Now;
		}
	}
}