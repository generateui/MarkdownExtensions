using MarkdownExtensions;

namespace MarkdownExtension.MsSql
{
	public class MsSqlTableSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			return new ParseSuccess(new MsSqlTable { Name = text.Replace("\n", string.Empty) });
		}
	}
}
