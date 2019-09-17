namespace MarkdownExtensions.Extensions.XmlSnippet
{
	public class XmlSnippetParser : BlockExtensionParser<XmlSnippetBlock>
	{
		public XmlSnippetParser()
		{
			InfoPrefix = "xml-snippet";
			_create = _ => new XmlSnippetBlock(this);
		}
	}
}
