using MarkdownExtensions;
using System.Linq;

namespace MarkdownExtension.KeyboardKeys
{
	public class KeyboardKeysSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			var keys = text.Split('+').Select(k => k.Trim()).ToList();
			return new ParseSuccess(new KeyboardKeys { Keys = keys });
		}
	}
}
