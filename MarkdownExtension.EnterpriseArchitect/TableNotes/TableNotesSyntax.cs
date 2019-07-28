using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System;
using System.Linq;

namespace MarkdownExtension.EnterpriseArchitect.TableNotes
{
	public class TableNotesSyntax : IParser
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
			var model = new TableNotes();
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
}
