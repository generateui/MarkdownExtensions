namespace MarkdownExtensions.Extensions.TableOfContent
{
	internal class TableOfContentSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			string[] lines = text.Split('\n');
			TableOfContent toc = null;
			if (lines.Length > 0)
			{
				foreach (var line in lines)
				{
					// level1: decimal
					var sanitized = line.Trim().ToLower();
					if (sanitized.StartsWith("level"))
					{
						if (!int.TryParse(sanitized[5].ToString(), out int level))
						{
							continue;
						}
						if (level < 1 && level > 6)
						{
							continue;
						}
						var split = sanitized.Split(':');
						if (split.Length != 2)
						{
							continue;
						}
						var numberingStyleText = split[1];
						NumberingStyle numberingStyle = NumberingStyle.TryParse(numberingStyleText);
						if (numberingStyle == null)
						{
							continue;
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
				}
			}
			toc = toc ?? new TableOfContent();
			return new ParseSuccess(toc);
		}
	}
}
