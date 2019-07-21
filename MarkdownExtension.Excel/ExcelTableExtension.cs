using ExcelDataReader;
using MarkdownExtensions;
using System.Collections.Generic;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace MarkdownExtension.Excel
{
    // first row is header
    // first column is header
    // row uses 90deg angled text
    // styled cells
    // conditionally styled cells (use materialized)
    public class ExcelTable : IMarkdownExtension
    {
        private class Table
        {
            internal List<Row> Rows { get; set; }
        }
        private class Row
        {
            internal List<string> Cells { get; set; }
        }
        private class CellReference
        {
            public int Column { get; private set; }
            public int Row { get; private set; }
            public static CellReference Parse(string value)
            {
                int column = 0;
                string row = "";
                foreach (var c in value.ToLower())
                {
                    if (char.IsLetter(c))
                    {
                        // TODO: multiply
                        column += c.ToAlphabetIndex();
                    }
                    if (char.IsNumber(c))
                    {
                        row += c;
                    }
                }
                return new CellReference
                {
                    Column = column,
                    Row = int.Parse(row)
                };
            }
        }
        private class ExcelSelection
        {
            public string FileName { get; set; }
            public string Sheet { get; set; }
            public string CellsFrom { get; set; }
            public string CellsTo { get; set; }
        }
        // file:sheet:cellsfrom:cellsto
        // Sheet.xlsx:Sheet1:A2:B4
        private class SyntaxImpl : ISyntax
        {
            public IParseResult Parse(string text)
            {
                var split = text.Split(':');
                if (split.Length != 4)
                {
                    return new ParseFailure(new Error("Reference should be in the format file:sheet:cellsfrom:cellsto"));
                }
                var excelSelection = new ExcelSelection
                {
                    FileName = split[0],
                    Sheet = split[1],
                    CellsFrom = split[2],
                    CellsTo = split[3].Replace("\n", "")
                };
                return new ParseSuccess(excelSelection);
            }
        }

        private class ValidatorImpl : IValidator
        {
            public IErrors Validate(object tree, SourceSettings sourceSettings)
            {
                var excelSelection = tree as ExcelSelection;
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
        private class FormatterImpl : IFormatter
        {
            public FormatResult Format(object root, IFormatState state)
            {
                var excelSelection = root as ExcelSelection;
                var excelTable = new Table { Rows = new List<Row>() };
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
                return FormatResult.FromHtml(sb.ToString());
            }

            public ICodeByName GetCss() => new CodeByName(NAME, @"
.excel-table {
    border-collapse: collapse;
    border: 1px solid black;
}
");
            public ICodeByName GetJs() => null;
        }

        public ExcelTable()
        {
            Syntax = new SyntaxImpl();
            Validator = new ValidatorImpl();
            Formatter = new FormatterImpl();
        }

        public string Prefix => "excel-table";
        public MarkdownExtensionName Name => NAME;
        private static readonly MarkdownExtensionName NAME = "Excel table";
        public Output Output => Output.Html;
        public IElementType Type => ElementType.Block;

        public ISyntax Syntax { get; }
        public IValidator Validator { get; }
        public IFormatter Formatter { get; }
    }
}
