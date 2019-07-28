using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.RequirementsTable
{
	public class RequirementsTableBlock : FencedCodeBlock, IExtensionBlock
	{
		public RequirementsTableBlock(BlockParser parser) : base(parser) { }
	}
}
