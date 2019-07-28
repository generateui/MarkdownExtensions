using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.Diagram
{
	public class DiagramBlock : FencedCodeBlock, IExtensionBlock
	{
		public DiagramBlock(BlockParser parser) : base(parser) { }
	}
}
