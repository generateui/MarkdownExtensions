using MarkdownExtensions;
using System.Data.SqlClient;
using System.Text;

namespace MarkdownExtension.MsSql
{
	public class MsSqlTableValidator : ValidatorBase<MsSqlTable>
	{
		public override IErrors ValidateTyped(MsSqlTable table, RenderSettings renderSettings)
		{
			var scsb = new SqlConnectionStringBuilder();
			using (var connection = new SqlConnection(@"Data Source=DESKTOP-A1T0LHV\SQLEXPRESS;Integrated Security=true;"))
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
}
