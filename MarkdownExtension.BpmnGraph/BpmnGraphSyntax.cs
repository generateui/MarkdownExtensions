using MarkdownExtensions;

namespace MarkdownExtension.BpmnGraph
{
	public class BpmnGraphSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			return new ParseSuccess(new BpmnGraph { FileUri = text.Trim() });
		}
	}
}
