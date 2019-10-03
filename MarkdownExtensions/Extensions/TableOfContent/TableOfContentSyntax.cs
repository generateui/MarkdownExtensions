using MarkdownExtensions.GenericSyntax;

namespace MarkdownExtensions.Extensions.TableOfContent
{
	internal class TableOfContentSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			string[] lines = text.Split('\n');
			TableOfContent toc = null;
			int lineNumber = -1;
			if (lines.Length > 0)
			{
				foreach (var line in lines)
				{
					lineNumber += 1;
					// example: "level1: decimal"
					var sanitized = line.Trim().ToLower();
					if (sanitized.StartsWith("level"))
					{
						if (sanitized.Length < 6 || !int.TryParse(sanitized[5].ToString(), out int level))
						{
							return new ParseFailure(lineNumber, @"Line must start with ""level:""");
						}
						if (level < 1 && level > 6)
						{
							return new ParseFailure(lineNumber, @"Level must be 1, 2, 3, 4, 5 or 6");
						}
						var split = sanitized.Split(':');
						if (split.Length != 2)
						{
							return new ParseFailure(lineNumber, @"After the ""level:"", specify the numbering style");
						}
						var numberingStyleText = split[1];
						NumberingStyle numberingStyle = NumberingStyle.TryParse(numberingStyleText);
						if (numberingStyle == null)
						{
							return new ParseFailure(lineNumber, $@"Unrecognized numbering style [{numberingStyleText}]");
						}
						toc = toc ?? TableOfContent.Empty();
						switch (level)
						{
							case 1: toc.Level1 = numberingStyle; break;
							case 2: toc.Level2 = numberingStyle; break;
							case 3: toc.Level3 = numberingStyle; break;
							case 4: toc.Level4 = numberingStyle; break;
							case 5: toc.Level5 = numberingStyle; break;
							case 6: toc.Level6 = numberingStyle; break;
						}
					}
					if (line.StartsWith("width:"))
					{
						var widthText = line.Substring(6).Trim();
						CssLenghtUnit width = CssLenghtUnit.Parse(widthText);
						toc.Width = width;
					}
				}
			}
			toc = toc ?? new TableOfContent();
			return new ParseSuccess(toc);
		}
	}
}
