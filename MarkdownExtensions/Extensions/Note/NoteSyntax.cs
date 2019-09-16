namespace MarkdownExtensions.Extensions.Note
{
	public class NoteSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			return new ParseSuccess(new Note());
		}
	}
}