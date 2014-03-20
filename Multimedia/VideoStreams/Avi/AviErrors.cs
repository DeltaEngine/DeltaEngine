using System.Collections.Generic;

namespace DeltaEngine.Multimedia.VideoStreams.Avi
{
	/// <summary>
	/// Helper for translating Avi error codes.
	/// </summary>
	public static class AviErrors
	{
		internal static string GetError(uint code)
		{
			return ErrorCodes.ContainsKey(code) ? ErrorCodes[code] : "Unknown error (" + code + ")";
		}

		private static readonly Dictionary<uint, string> ErrorCodes = new Dictionary<uint, string>
		{
			{ 0x80044065, "AVIERR_UNSUPPORTED" },
			{ 0x80044066, "AVIERR_BADFORMAT" },
			{ 0x80044067, "AVIERR_MEMORY" },
			{ 0x80044068, "AVIERR_INTERNAL" },
			{ 0x80044069, "AVIERR_BADFLAGS" },
			{ 0x8004406A, "AVIERR_BADPARAM" },
			{ 0x8004406B, "AVIERR_BADSIZE" },
			{ 0x8004406C, "AVIERR_BADHANDLE" },
			{ 0x8004406D, "AVIERR_FILEREAD" },
			{ 0x8004406E, "AVIERR_FILEWRITE" },
			{ 0x8004406F, "AVIERR_FILEOPEN" },
			{ 0x80044070, "AVIERR_COMPRESSOR" },
			{ 0x80044071, "AVIERR_NOCOMPRESSOR" },
			{ 0x80044072, "AVIERR_READONLY" },
			{ 0x80044073, "AVIERR_NODATA" },
			{ 0x80044074, "AVIERR_BUFFERTOOSMALL" },
			{ 0x80044075, "AVIERR_CANTCOMPRESS" },
			{ 0x800440C6, "AVIERR_USERABORT" },
			{ 0x800440C7, "AVIERR_ERROR" },
		};
	}
}