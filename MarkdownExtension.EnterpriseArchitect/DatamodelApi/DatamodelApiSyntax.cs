using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.DatamodelApi
{
	public class DatamodelApiSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			var packagePath = text.Trim();
			return new ParseSuccess(new DatamodelApi { PackagePath = packagePath });
		}
	}
}
