namespace MarkdownExtensions.Extensions.TableOfContent
{
	internal class TableOfContentSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			return new ParseSuccess(new TableOfContent());
		}
	}
}
