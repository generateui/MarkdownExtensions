using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.RequirementText
{
	public class RequirementTextSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			// [ea:req-arch:Primary keys must be named Id]
			if (text.StartsWith("req-arch"))
			{
				var value = text.Split(new[] { ':' }, 2)[1];
				return new ParseSuccess(new Element { Name = value });
			}
			return null;
		}
	}
}
