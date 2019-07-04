using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownExtension.EnterpriseArchitect
{
    public class TableNotes : IMarkdownExtension
    {
        private class Table
        {
            public string Name { get; set; }
        }
        private class TableNotesModel
        {
            public IEnumerable<Table> Tables {get; set;}
        }
        private class TableNotesSyntax : ISyntax
        {
            public IParseResult Parse(string text)
            {
                // ```ea-table-notes
                // User
                // Customer
                // ```
                var lines = text.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
                return new ParseSuccess(new TableNotesModel { Tables = lines.Select(l => new Table { Name = l }) });
            }
        }
        private class TableNotesFormatter : IFormatter
        {
            private readonly IEaProvider _provider;

            public TableNotesFormatter(IEaProvider provider)
            {
                _provider = provider;
            }

            public FormatResult Format(object root, IFormatState state)
            {
                var tableNotes = root as TableNotesModel;
                var tables = _provider
                    .GetElements(e => e.Stereotype == "table")
                    .ToDictionary(x => x.Name, x => x);

                var all = new StringBuilder();
                foreach (var table in tableNotes.Tables)
                {
                    if (tables.ContainsKey(table.Name))
                    {
                        var sb = new StringBuilder();
                        var eaTable = tables[table.Name];
                        sb.AppendLine($@"<ul><b>{table.Name}</b>");
                        bool hasTableNotes = false;
                        if (!string.IsNullOrEmpty(eaTable.Notes))
                        {
                            hasTableNotes = true;
                            sb.AppendLine($@"<p>{eaTable.Notes}</p>");
                        }
                        bool hasFieldNotes = false;
                        foreach (var attribute in eaTable.Attributes)
                        {
                            if (!string.IsNullOrEmpty(attribute.Notes))
                            {
                                hasFieldNotes = true;
                                sb.AppendLine($@"<li>");
                                sb.AppendLine($@"<p>{attribute.Name}</p>");
                                sb.AppendLine($@"<p>{attribute.Notes}</p>");
                                sb.AppendLine($@"</li>");
                            }
                        }
                        sb.AppendLine($@"</ul>");
                        bool hasNotes = hasTableNotes || hasFieldNotes;
                        if (hasNotes)
                        {
                            all.Append(sb);
                        }
                    }
                }
                return FormatResult.FromHtml(all.ToString());
            }

            public ICodeByName GetCss() => null;
            public ICodeByName GetJs() => null;
        }

        public TableNotes(IEaProvider provider)
        {
            Syntax = new TableNotesSyntax();
            Formatter = new TableNotesFormatter(provider);
        }

        public string Prefix => "ea-table-notes";
        public IElementType Type => ElementType.Block;
        public ISyntax Syntax { get; }
        public Output Output => Output.Html;
        public IFormatter Formatter { get; }
        public static MarkdownExtensionName NAME => "EA element notes in a table";
        public MarkdownExtensionName Name => NAME;
        public IValidator Validator => null;
    }
}
