using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.RequirementText
{
	public class RequirementTextBlock : FencedCodeBlock, IExtensionBlock
	{
		public RequirementTextBlock(BlockParser parser) : base(parser) { }
	}
}
