using Markdig.Helpers;
using Markdig.Parsers;

namespace MarkdownExtension.KeyboardKeys
{
	public class KeyboardKeysParser : InlineParser
	{
		public KeyboardKeysParser()
		{
			OpeningCharacters = new[] { '[', };
			Prefix = "keys";
		}

		public string Prefix { get; }

		public override bool Match(InlineProcessor processor, ref StringSlice slice)
		{
			char c = slice.CurrentChar;
			if (c != '[')
			{
				return true;
			}
			bool colon = false;
			bool end = false;
			string prefix = "";
			string result = "";
			while (!end)
			{
				c = slice.NextChar();
				if (c == ']')
				{
					end = true;
					continue;
				}
				if (c == ':')
				{
					colon = true;
					if (prefix != Prefix)
					{
						return false;
					}
					continue;
				}
				if (colon)
				{
					result += c;
				}
				else
				{
					prefix += c;
				}
			}
			slice.NextChar();
			processor.Inline = new KeyboardKeysInline(result);
			return true;
		}
	}
}
