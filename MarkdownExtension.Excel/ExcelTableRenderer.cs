using ExcelDataReader;
using MarkdownExtensions;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace MarkdownExtension.Excel
{
	public class ExcelTableRenderer : BlockRendererBase<ExcelTableSelection, ExcelTableBlock>
	{
		public override void Render(ExtensionHtmlRenderer renderer, ExcelTableSelection excelSelection, IFormatState formatState)
		{
			var excelTable = new ExcelTable { Rows = new List<Row>() };
			using (var stream = File.Open(excelSelection.FileName, FileMode.Open, FileAccess.Read))
			{
				using (var reader = ExcelReaderFactory.CreateReader(stream))
				{
					DataSet result = reader.AsDataSet();
					DataTable sheet = result.Tables[excelSelection.Sheet];
					CellReference from = CellReference.Parse(excelSelection.CellsFrom);
					CellReference to = CellReference.Parse(excelSelection.CellsTo);
					for (int i = from.Row - 1; i <= to.Row - 1; i++)
					{
						//var row = sheet.Rows[i];
						var row = new Row { Cells = new List<string>() };
						for (int j = from.Column; j <= to.Column; j++)
						{
							var value = sheet.Rows[i].ItemArray[j].ToString();
							row.Cells.Add(value);
						}
						excelTable.Rows.Add(row);
					}
					// The result of each spreadsheet is in result.Tables
				}
			}
			var sb = new StringBuilder();
			sb.AppendLine("<table class='excel-table'>");
			foreach (var row in excelTable.Rows)
			{
				sb.AppendLine("<tr>");
				foreach (var column in row.Cells)
				{
					sb.AppendLine($@"<td>{column}</td>");
				}
				sb.AppendLine("</tr>");
			}
			sb.AppendLine("</table>");
			renderer.Write(sb.ToString());
		}

		public IEnumerable<ICode> Css
		{
			get
			{
				yield return new Code("excel-table", "0.0.1", () => @"
						.excel-table {
							border-collapse: collapse;
							border: 1px solid black;
						}
					");
			}
		}
	}
}
