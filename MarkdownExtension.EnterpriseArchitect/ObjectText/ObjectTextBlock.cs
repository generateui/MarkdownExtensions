using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.ObjectText
{
	public class ObjectTextBlock : FencedCodeBlock, IExtensionBlock
	{
		public ObjectTextBlock(BlockParser parser) : base(parser) { }
	}
}
