namespace MarkdownExtensions.Extensions.NestedBlock
{
	public class NestedBlockSyntax : IParser
    {
        public IParseResult Parse(string text)
        {
            return new ParseSuccess(new NestedBlock { Markdown = text });
        }
    }
}
