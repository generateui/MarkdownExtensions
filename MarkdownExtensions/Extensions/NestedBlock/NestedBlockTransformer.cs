using Markdig;
using Markdig.Syntax;

namespace MarkdownExtensions.Extensions.NestedBlock
{
	public class NestedBlockTransformer : ITransformer
	{
		public void Transform(ExtensionHtmlRenderer extensionHtmlRenderer, FencedCodeBlock block, object astNode)
		{
			NestedBlock nestedBlock = astNode as NestedBlock;
			// 1. parse markdown
			// 2. normalize markdown headings
			// 3. replace fencedblock with markdown node
			var node = Markdown.Parse(nestedBlock.Markdown);
			var index = block.Parent.IndexOf(block);
			block.Parent.Insert(index, node);
			block.Parent.RemoveAt(index + 1);
		}
	}
}
