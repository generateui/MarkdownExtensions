using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.TableNotes
{
	public class TableNotesParser : BlockExtensionParser<TableNotesBlock>
	{
		public TableNotesParser()
		{
			InfoPrefix = "ea-table-notes";
			_create = _ => new TableNotesBlock(this);
		}
	}
}
