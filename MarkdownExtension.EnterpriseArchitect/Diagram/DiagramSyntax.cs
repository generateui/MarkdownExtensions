using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.Diagram
{
	public class DiagramSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			//return new ParseFailure(new ParseError(new Range(new Position(1, 1), new Position(1, 10)), "derpified"));
			return new ParseSuccess(new Diagram { Name = text });
		}
	}
}
