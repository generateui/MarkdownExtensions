using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdownExtensions.Extensions.FolderFromDisk
{
	// for now, just the fencedcodeblock with raw content
	public sealed class FolderFromDiskBlock : FencedCodeBlock, IExtensionBlock
	{
		public FolderFromDiskBlock(BlockParser parser) : base(parser) { }
	}
}
