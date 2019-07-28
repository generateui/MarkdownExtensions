namespace MarkdownExtensions.Extensions.Snippet
{
	public class SnippetParser : BlockExtensionParser<SnippetBlock>
	{
		public SnippetParser()
		{
			InfoPrefix = "md-snippet";
			_create = _ => new SnippetBlock(this);
		}
	}
}
