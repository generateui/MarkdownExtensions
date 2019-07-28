using MarkdownExtensions;

namespace MarkdownExtension.Excel
{
	// file:sheet:cellsfrom:cellsto
	// Sheet.xlsx:Sheet1:A2:B4
	public class ExcelTableSyntax : IParser
	{
		public IParseResult Parse(string text)
		{
			var split = text.Trim().Split(':');
			if (split.Length != 4)
			{
				return new ParseFailure(new Error("Reference should be in the format file:sheet:cellsfrom:cellsto"));
			}
			var excelSelection = new ExcelTableSelection
			{
				FileName = split[0],
				Sheet = split[1],
				CellsFrom = split[2],
				CellsTo = split[3].Replace("\n", "")
			};
			return new ParseSuccess(excelSelection);
		}
	}
}
