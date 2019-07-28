using MarkdownExtensions;

namespace MarkdownExtension.MsSql
{
	public class MsSqlTableParser : BlockExtensionParser<MsSqlTableBlock>
	{
		public MsSqlTableParser()
		{
			InfoPrefix = "sql-table";
			_create = _ => new MsSqlTableBlock(this);
		}
	}
}
