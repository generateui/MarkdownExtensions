namespace MarkdownExtensions.Extensions.Snippet
{
	public class SnippetSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			text = text.Trim();
			var snippetReference = new Snippet();
			if (text.StartsWith("="))
			{
				snippetReference.AsSiblingHeading = true;
				text = text.Substring(1, text.Length - 1);
			}
			else if (text.StartsWith(">"))
			{
				snippetReference.AsChildHeading = true;
				text = text.Substring(1, text.Length - 1);
			}
			var split = text.Split(':');
			snippetReference.FileName = split[0];
			snippetReference.HeadingName = split[1];
			return new ParseSuccess(snippetReference);
		}
	}
	public class SnippetInlineSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			var snippetReference = new Snippet();
			if (text.StartsWith("="))
			{
				snippetReference.AsSiblingHeading = true;
				text = text.Substring(1, text.Length - 1);
			}
			else if (text.StartsWith(">"))
			{
				snippetReference.AsChildHeading = true;
				text = text.Substring(1, text.Length - 1);
			}
			var split = text.Split(':');
			snippetReference.FileName = split[0];
			snippetReference.HeadingName = split[1];
			return new ParseSuccess(snippetReference);
		}
	}
}
