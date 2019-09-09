using Markdig.Parsers;
using Markdig.Syntax;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.DatamodelApi
{
	public class DatamodelApiBlock : FencedCodeBlock, IExtensionBlock
	{
		public DatamodelApiBlock(BlockParser parser) : base(parser) { }
	}
}
