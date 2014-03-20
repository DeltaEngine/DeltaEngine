using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Mocks
{
	/// <summary>
	/// Mocks a settings file during unit tests instead just having a simple lookup.
	/// </summary>
	public class MockSettings : Settings
	{
		public MockSettings()
		{
			values = new Dictionary<string, string>();
			Current = this;
			SetRapidUpdatesPerSecondToUpdatesPerSecondForBetterPerformance();
			wasChanged = false;
			UseOnlineLogging = true;
		}

		private void SetRapidUpdatesPerSecondToUpdatesPerSecondForBetterPerformance()
		{
			RapidUpdatesPerSecond = UpdatesPerSecond;
		}

		private readonly Dictionary<string, string> values;

		public override void Save() {}

		public override T GetValue<T>(string name, T defaultValue)
		{
			string value;
			return values.TryGetValue(name, out value) ? value.Convert<T>() : defaultValue;
		}

		public override void SetValue(string name, object value)
		{
			if (values.ContainsKey(name))
				values[name] = StringExtensions.ToInvariantString(value);
			else
				values.Add(name, StringExtensions.ToInvariantString(value));
			Change();
		}

		public void Change()
		{
			wasChanged = true;
		}

		public bool AreChanged
		{
			get { return wasChanged; }
		}
	}
}