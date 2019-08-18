using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.ObjectText
{
	public class ObjectTextSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			return new ParseSuccess(new ObjectText { PackageName = text.Trim() });
		}
	}
}
