using System.IO;
using System.Linq;

namespace BatchEncoder
{
	public static class ExtensionChecker
	{
		static readonly string[] SupportedExtensions = {".mp4", ".flv", ".mov", ".m4v", ".mpg", ".mpeg", ".rm"};

		public static bool IsUnsupportedExtension(string path)
		{
			var extension = Path.GetExtension(path);
			return !SupportedExtensions.Contains(extension);
		}

		public static string GetAttributedExtension(EncodeSettings settings)
		{
			var extension = Path.GetExtension(settings.output);

			if (settings.concatenate) return "Concat.mp4";
			if (extension == ".mp4") return "Encoded.mp4";
			return ".mp4";
		}
	}
}