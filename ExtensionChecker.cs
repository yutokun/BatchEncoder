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

		public static bool IsSameExtension(string path)
		{
			var extension = Path.GetExtension(path);
			return extension == ".mp4";
		}
	}
}