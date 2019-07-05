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
            public IEnumerable<Table> Tables { get; set; }
            public bool AllTables { get; set; }
            public Path PackagePath { get; set; }
            public static bool Include(TableNotesModel tnm, Element e)
            {
                if (e.Stereotype != "table")
                {
                    return false;
                }
                if (tnm.AllTables)
                {
                    return true;
                }
                return tnm.Tables.Any(t => Equals(t, e.Name));
            }
        }
        private class TableNotesSyntax : ISyntax
        {
            public IParseResult Parse(string text)
            {
                // ```ea-table-notes
                // User
                // Customer
                // ```
                var lines = text.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Count() == 0)
                {
                    return new ParseFailure(
                        new ParseError(new Range(new Position(0, 0), new Position(0, 0)), "specify optional package and tables"));
                }
                var model = new TableNotesModel();
                var path = lines[0].Replace("package: ", string.Empty);
                model.PackagePath = new Path(path);

                if (lines.Count() > 1)
                {
                    model.Tables = lines.Skip(1).Select(l => new Table { Name = l });
                }
                else
                {
                    model.AllTables = true;
                }
                return new ParseSuccess(model);
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
                IEnumerable<Element> tables = null;
                if (tableNotes.PackagePath != null)
                {
                    var package = _provider.GetElementsByPackage(tableNotes.PackagePath);
                    tables = package
                        .GetElementsRecursively()
                        .Where(e => TableNotesModel.Include(tableNotes, e));
                }
                else
                {
                    tables = _provider
                        .GetElements(e => e.Stereotype == "table")
                        .Where(e => TableNotesModel.Include(tableNotes, e));
                }

                var all = new StringBuilder();
                foreach (var table in tables)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($@"## {table.Name}");
                    bool hasTableNotes = false;
                    if (!string.IsNullOrEmpty(table.Notes))
                    {
                        hasTableNotes = true;
                        sb.AppendLine(table.Notes);
                    }
                    bool hasFieldNotes = false;
                    foreach (var attribute in table.Attributes)
                    {
                        if (!string.IsNullOrEmpty(attribute.Notes))
                        {
                            hasFieldNotes = true;
                            sb.AppendLine($@"### {attribute.Name}");
                            sb.AppendLine(attribute.Notes);
                        }
                    }
                    bool hasNotes = hasTableNotes || hasFieldNotes;
                    if (hasNotes)
                    {
                        all.Append(sb);
                    }
                }
                return FormatResult.FromMarkdown(all.ToString());
            }

            public ICodeByName GetCss() => null;
            public ICodeByName GetJs() => null;
        }

        private class ValidatorImpl : IValidator
        {
            private readonly IEaProvider _provider;

            public ValidatorImpl(IEaProvider provider)
            {
                _provider = provider;
            }
            public IErrors Validate(object tree)
            {
                var tableNotes = tree as TableNotesModel;
                if (tableNotes.PackagePath != null)
                {
                    IEnumerable<string> nameCollisions = _provider
                        .GetElementsByPackage(tableNotes.PackagePath)
                        .GetElementsRecursively()
                        .Where(e => TableNotesModel.Include(tableNotes, e))
                        .GroupBy(x => x.Name)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key);
                    if (nameCollisions.Count() > 0)
                    {
                        var errors = nameCollisions.Select(nc => new Error($@"Element named {nc} has multiple elements in package"));
                        return new ValidationFailure(errors);
                    }
                }
                return new Valid();
            }
        }

        public TableNotes(IEaProvider provider)
        {
            Syntax = new TableNotesSyntax();
            Formatter = new TableNotesFormatter(provider);
            Validator = new ValidatorImpl(provider);
        }

        public string Prefix => "ea-table-notes";
        public IElementType Type => ElementType.Block;
        public ISyntax Syntax { get; }
        public Output Output => Output.Markdown;
        public IFormatter Formatter { get; }
        public static MarkdownExtensionName NAME => "EA element notes in a table";
        public MarkdownExtensionName Name => NAME;
        public IValidator Validator { get; }
    }
}
