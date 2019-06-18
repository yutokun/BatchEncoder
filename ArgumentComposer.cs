using System.Collections.Generic;
using System.Text;

namespace BatchEncoder
{
	class EncodeSettings
	{
		public string path;
		public string videoCodec;
		public string videoBitrate;
		public string framerate;
		public string videoSize;
		public string audioCodec;
		public string startSec;
		public string duration;
	}

	public class ArgumentComposer
	{
		List<string> arguments = new List<string>();

		public void Add(string argument)
		{
			arguments.Add(argument);
		}

		public void Add(string argument, string condition)
		{
			if (string.IsNullOrEmpty(condition)) return;
			arguments.Add(argument);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach (var argument in arguments) sb.Append(argument + " ");
			sb.Remove(sb.Length - 1, 1); // Remove space at EOF.
			return sb.ToString();
		}
	}
}