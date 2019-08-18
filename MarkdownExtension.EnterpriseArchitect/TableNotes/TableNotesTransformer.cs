using Markdig;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownExtension.EnterpriseArchitect.TableNotes
{
	public class TableNotesTransformer : TransformerBase<TableNotesBlock, TableNotes>
	{
		private readonly IEaProvider _provider;

		public TableNotesTransformer(IEaProvider provider)
		{
			_provider = provider;
		}
		public override void Transform(TableNotesBlock block, TableNotes tableNotes)
		{
			IEnumerable<Element> tables = null;
			if (tableNotes.PackagePath != null)
			{
				var package = _provider.GetElementsByPackage(tableNotes.PackagePath);
				tables = package
					.GetElementsRecursively()
					.Where(e => TableNotes.Include(tableNotes, e));
			}
			else
			{
				tables = _provider
					.GetElements(e => e.Stereotype == "table")
					.Where(e => TableNotes.Include(tableNotes, e));
			}

			var all = new StringBuilder();
			var eaNormalizer = new EaNormalizer();
			foreach (var table in tables)
			{
				var sb = new StringBuilder();
				sb.AppendLine($@"## {table.Name}");
				bool hasTableNotes = false;
				if (!string.IsNullOrEmpty(table.Notes))
				{
					hasTableNotes = true;
					var notes = eaNormalizer.Normalize(table.Notes);
					sb.AppendLine(notes);
					sb.AppendLine();
				}
				bool hasFieldNotes = false;
				foreach (var attribute in table.Attributes)
				{
					if (!string.IsNullOrEmpty(attribute.Notes))
					{
						hasFieldNotes = true;
						var notes = eaNormalizer.Normalize(table.Notes);
						sb.AppendLine($@"### {attribute.Name}");
						sb.AppendLine(notes);
						sb.AppendLine();
					}
				}
				bool hasNotes = hasTableNotes || hasFieldNotes;
				if (hasNotes)
				{
					all.Append(sb);
				}
			}
			var document = Markdown.Parse(all.ToString());
			Replace(block, document);
		}
	}
}
