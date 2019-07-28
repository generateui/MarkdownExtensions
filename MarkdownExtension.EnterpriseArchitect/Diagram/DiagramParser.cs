using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.Diagram
{
	public class DiagramParser : BlockExtensionParser<DiagramBlock>
	{
		public DiagramParser()
		{
			InfoPrefix = "ea-diagram";
			_create = _ => new DiagramBlock(this);
		}
	}
}
