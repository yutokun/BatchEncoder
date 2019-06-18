using System.Collections.Generic;
using System.Text;

namespace BatchEncoder
{
	public class ArgumentsComposer
	{
		readonly List<string> arguments = new List<string>();

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