using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.BpmnGraph
{
	public class BpmnGraphBlock : FencedCodeBlock, IExtensionBlock
	{
		public BpmnGraphBlock(BlockParser parser) : base(parser) { }
	}
}
