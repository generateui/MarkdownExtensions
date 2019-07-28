using ExcelDataReader;
using MarkdownExtensions;
using System;
using System.Data;
using System.IO;

namespace MarkdownExtension.Excel
{
	public class ExcelTableValidator : ValidatorBase<ExcelTableSelection>
	{
		public override IErrors ValidateTyped(ExcelTableSelection excelSelection, SourceSettings sourceSettings)
		{
			var fullFilePath = Path.Combine(sourceSettings.Folder, excelSelection.FileName);
			if (!File.Exists(fullFilePath))
			{
				return new ValidationFailure(new Error($@"File [{excelSelection.FileName}] not found, tried looking at [{fullFilePath}]"));
			}
			try
			{
				using (var stream = File.Open(fullFilePath, FileMode.Open, FileAccess.Read))
				using (var reader = ExcelReaderFactory.CreateReader(stream))
				{
					DataSet result = reader.AsDataSet();
					bool hasTable = result.Tables.Contains(excelSelection.Sheet);
					if (!hasTable)
					{
						return new ValidationFailure(new Error($@"The workbook [{excelSelection.FileName}] does not contain the sheet [{excelSelection.Sheet}]"));
					}
				}
			}
			catch (Exception)
			{
				new ValidationFailure(new Error($@"Something went wrong trying to open the excel workbook"));
			}
			// check if cellreferences are coorect
			// check if cellreference go up (i.e. b2:a1)
			return new Valid();
		}
	}
}
