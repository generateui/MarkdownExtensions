using MarkdownExtensions;

namespace MarkdownExtension.Excel
{
	public class ExcelTableParser : BlockExtensionParser<ExcelTableBlock>
	{
		public ExcelTableParser()
		{
			InfoPrefix = "excel-table";
			_create = _ => new ExcelTableBlock(this);
		}
	}
}
