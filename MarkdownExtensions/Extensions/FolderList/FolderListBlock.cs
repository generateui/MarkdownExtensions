using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownExtensions.Extensions.FolderList
{
	// for now, just the fencedcodeblock with raw content
	public class FolderListBlock : FencedCodeBlock, IExtensionBlock
	{
		public FolderListBlock(BlockParser parser) : base(parser) { }
	}
}
