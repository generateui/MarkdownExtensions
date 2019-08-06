using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.Diagram
{
	public class DiagramBlockParser : BlockExtensionParser<DiagramBlock>
	{
		public DiagramBlockParser()
		{
			InfoPrefix = "ea-diagram";
			_create = _ => new DiagramBlock(this);
		}
	}
}
