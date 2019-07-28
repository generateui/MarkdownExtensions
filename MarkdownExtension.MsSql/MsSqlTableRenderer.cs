using Dapper;
using MarkdownExtensions;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace MarkdownExtension.MsSql
{
	class MsSqlTableRenderer : BlockRendererBase<MsSqlTable, MsSqlTableBlock>
    {
		public override void Render(ExtensionHtmlRenderer renderer, MsSqlTable msSqlTable, IFormatState formatState)
		{
            var table = GetTable(msSqlTable.Name);
            var sb = new StringBuilder();
            sb.AppendLine($@"<table class='sql-table'>");

            sb.AppendLine($@"<thead>");
            sb.AppendLine($@"<tr><th colspan='2'>{msSqlTable.Name}</th></tr>");
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
			renderer.Write(sb.ToString());
        }

        private Model.Table GetTable(string tableName)
        {
            var scsb = new SqlConnectionStringBuilder();
            Model.Table table = new Model.Table();
            using (var connection = new SqlConnection(@"Data Source=DESKTOP-A1T0LHV\SQLEXPRESS;Integrated Security=true;"))
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
            
        public IEnumerable<ICode> Css
		{
			get
			{
				yield return new Code("ms-sql-table", "0.0.1", () => @"
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
			}
		}
    }
}
