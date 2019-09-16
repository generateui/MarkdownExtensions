namespace MarkdownExtensions.Extensions.TableOfContent
{
	internal class TableOfContentParser : BlockExtensionParser<TableOfContentBlock>
	{
		public TableOfContentParser()
		{
			InfoPrefix = "toc";
			_create = _ => new TableOfContentBlock(this);
		}
	}
}
