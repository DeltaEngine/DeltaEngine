using System;

namespace DeltaEngine.Networking
{
	/// <summary>
	/// Provides the networking client functionality to send and receive any data object.
	/// </summary>
	public interface Client : IDisposable
	{
		int UniqueID { get; set; }
		void Connect(string targetAddress, int targetPort, Action optionalTimedOut = null);
		bool IsConnected { get; }
		string TargetAddress { get; }
		void Send(object message, bool allowToCompressMessage = true);
		event Action<object> DataReceived;
		event Action Connected;
		event Action Disconnected;
	}
}