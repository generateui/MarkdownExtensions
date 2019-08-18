using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.ObjectText
{
	public class ObjectTextParser : BlockExtensionParser<ObjectTextBlock>
	{
		public ObjectTextParser()
		{
			InfoPrefix = "ea-object-text";
			_create = _ => new ObjectTextBlock(this);
		}
	}
}
