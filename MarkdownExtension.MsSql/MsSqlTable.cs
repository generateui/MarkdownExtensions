using Dapper;
using MarkdownExtensions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MarkdownExtension.MsSql
{
    public class MsSqlTable : IMarkdownExtension
    {
        class TableName
        {
            public string Name { get; set; }
        }
        class SyntaxImpl : ISyntax
        {
            public IParseResult Parse(string text)
            {
                return new ParseSuccess(new TableName { Name = text.Replace("\n", string.Empty) });
            }
        }
        class ValidatorImpl : IValidator
        {
            public IErrors Validate(object tree)
            {
                var table = tree as TableName;
                var scsb = new SqlConnectionStringBuilder();
                using (var connection = new SqlConnection(@"Data Source=DESKTOP-5S5IDQL\SQLEXPRESS;Integrated Security=true;"))
                {
                    connection.Open();
                    var q = $@"
                        SELECT count(*)
                        FROM Test.INFORMATION_SCHEMA.COLUMNS
                        WHERE TABLE_NAME = '{table.Name}'
                    ";

                    // FK, PK, constraints
                    var sb = new StringBuilder();
                    sb.AppendLine($@"<table>");
                    sb.AppendLine($@"<thead>");
                    sb.AppendLine($@"<tr><th>{table.Name}</th></tr>");
                    sb.AppendLine($@"</thead>");
                    sb.AppendLine($@"<tbody>");
                    using (var sqlCommand = new SqlCommand(q, connection))
                    {
                        var count = sqlCommand.ExecuteScalar() as int?;
                        if (count.HasValue && count.Value == 0)
                        {
                            return new ValidationFailure(new Error("Table named {table.Name} not found"));
                        }
                    }
                }
                return null;
            }
        }
        class FormatterImpl : IFormatter
        {
            public FormatResult Format(object root, IFormatState state)
            {
                var tableName = root as TableName;
                var table = GetTable(tableName.Name);
                var sb = new StringBuilder();
                sb.AppendLine($@"<table class='sql-table'>");

                sb.AppendLine($@"<thead>");
                sb.AppendLine($@"<tr><th colspan='2'>{tableName.Name}</th></tr>");
                sb.AppendLine($@"</thead>");

                sb.AppendLine($@"<tbody class='sql-fields'>");
                foreach (var field in table.Fields)
                {
                    sb.AppendLine($@"<tr>");
                    if (field.IsNullable || field.IsPrimaryKey)
                    {
                        string nullable = field.IsNullable ? "*" : "";
                        string primaryKey = field.IsPrimaryKey ? "PK" : "";
                        sb.AppendLine($@"<td>{nullable}{primaryKey}</td>");
                    }
                    else
                    {
                        sb.AppendLine($@"<td></td>");
                    }
                    sb.AppendLine($@"<td>{field.ToText()}</td></tr>");
                }
                sb.AppendLine($@"</tbody>");

                sb.AppendLine($@"<tbody class='sql-foreign-keys-header'>");
                sb.AppendLine($@"<tr><td colspan='2'>Foreign keys</td></tr>");
                sb.AppendLine($@"</tbody'>");
                sb.AppendLine($@"<tbody class='sql-foreign-keys'>");
                foreach (var fk in table.ForeignKeys)
                {
                    sb.AppendLine($@"<tr><td></td><td>{fk.Name} {fk.Table}.{fk.Column} ↔ {fk.ReferencedTable}.{fk.ReferencedColumn}</td></tr>");
                }
                sb.AppendLine($@"</tbody>");

                sb.AppendLine($@"<tbody class='sql-indexes-header'>");
                sb.AppendLine($@"<tr><td colspan='2'>Indexes</td></tr>");
                sb.AppendLine($@"</tbody'>");
                sb.AppendLine($@"<tbody class='sql-indexes'>");
                foreach (var index in table.Indexes)
                {
                    sb.AppendLine($@"<tr><td></td><td>{index.Name}({index.IndexType})</td></tr>");
                }
                sb.AppendLine($@"</tbody>");

                //sb.AppendLine($@"<tbody>");
                //sb.AppendLine($@"<tr><td>{table.PrimaryKey.Name}({table.PrimaryKey.Type})</td></tr>");
                //sb.AppendLine($@"</tbody>");
                sb.AppendLine("</table>");
                return FormatResult.FromHtml(sb.ToString());
            }

            private Model.Table GetTable(string tableName)
            {
                var scsb = new SqlConnectionStringBuilder();
                Model.Table table = new Model.Table();
                using (var connection = new SqlConnection(@"Data Source=DESKTOP-5S5IDQL\SQLEXPRESS;Integrated Security=true;"))
                {
                    connection.Open();

                    var q1 = GetType().Assembly.GetFileContent("Fields.sql");
                    q1 = $@"use Test; {q1}";
                    q1 = string.Format(q1, tableName);
                    table.Fields.AddRange(connection.Query<Model.Field>(q1));

                    var q2 = GetType().Assembly.GetFileContent("ForeignKeys.sql");
                    q2 = $@"use Test; {q2}";
                    q2 = string.Format(q2, tableName);
                    IEnumerable<Model.ForeignKey> foreignKeys = connection.Query<Model.ForeignKey>(q2);
                    table.ForeignKeys.AddRange(foreignKeys);

                    var q3 = GetType().Assembly.GetFileContent("Indexes.sql");
                    q3 = $@"use Test; {q3}";
                    q3 = string.Format(q3, tableName);
                    IEnumerable<Model.Index> indexes = connection.Query<Model.Index>(q3);
                    table.Indexes.AddRange(indexes);
                }
                return table;
            }
            
            public ICodeByName GetCss() => new CodeByName(NAME, @"
.sql-table {
    border-collapse: collapse;
    border: 1px solid black;
}
.sql-fields {
    border: 1px solid black;
    background-color: lightgrey;
}
.sql-foreign-keys-header {
    background-color: darkgrey;
}
.sql-foreign-keys {
    border: 1px solid black;
    background-color: lightgrey;
}
.sql-indexes-header {
    background-color: darkgrey;
}
.sql-indexes {
    border: 1px solid black;
    background-color: lightgrey;
}
            ");
            public ICodeByName GetJs() => null;
        }

        public MsSqlTable()
        {
            Syntax = new SyntaxImpl();
            Formatter = new FormatterImpl();
            Validator = new ValidatorImpl();
        }
        public string Prefix => "sql-table";

        public MarkdownExtensionName Name => NAME;
        private static readonly MarkdownExtensionName NAME = "Sql table";
        public IElementType Type => ElementType.Block;
        public ISyntax Syntax { get; }
        public IValidator Validator { get; }
        public IFormatter Formatter { get; }

        public Output Output => Output.Html;
    }
}
