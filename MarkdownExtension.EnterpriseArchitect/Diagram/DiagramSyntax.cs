using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.Diagram
{
	public class DiagramSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			var trimmed = text.Trim();
			if (trimmed.StartsWith("package: "))
			{
				var packagePath = text.Substring(9, text.Length - 9);
				return new ParseSuccess(new Diagram { PackagePath = packagePath });
			}
			return new ParseSuccess(new Diagram { Path = text });
		}
	}
}
