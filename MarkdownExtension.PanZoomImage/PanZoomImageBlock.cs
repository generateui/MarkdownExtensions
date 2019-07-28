using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.PanZoomImage
{
	public class PanZoomImageBlock : FencedCodeBlock, IExtensionBlock
	{
		public PanZoomImageBlock(BlockParser parser) : base(parser) { }
	}
}
