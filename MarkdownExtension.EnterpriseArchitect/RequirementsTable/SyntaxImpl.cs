using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect
{
	public class RequirementsTableSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			return new ParseSuccess(new RequirementsTableName { Name = text.Trim() });
		}
	}
}
