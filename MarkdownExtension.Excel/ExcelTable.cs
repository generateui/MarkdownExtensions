using System.Collections.Generic;

namespace MarkdownExtension.Excel
{
	public class ExcelTable
    {
        internal List<Row> Rows { get; set; }
    }
	public class Row
	{
		internal List<string> Cells { get; set; }
	}
}
