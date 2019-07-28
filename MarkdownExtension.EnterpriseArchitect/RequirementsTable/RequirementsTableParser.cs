using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.RequirementsTable
{
	public class RequirementsTableParser : BlockExtensionParser<RequirementsTableBlock>
	{
		public RequirementsTableParser()
		{
			InfoPrefix = "ea-requirements-table";
			_create = _ => new RequirementsTableBlock(this);
		}
	}
}
