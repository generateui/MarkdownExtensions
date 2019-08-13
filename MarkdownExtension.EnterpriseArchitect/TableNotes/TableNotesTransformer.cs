using Markdig;
using Markdig.Syntax;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using MarkdownExtensions.Extensions.Snippet;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownExtension.EnterpriseArchitect.TableNotes
{
	class TableNotesTransformer : TransformerBase<TableNotesBlock, TableNotes>
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
			void transform(MarkdownDocument md) { md.IncreaseHeadingLevel(1); }
			foreach (var table in tables)
			{
				var sb = new StringBuilder();
				sb.AppendLine($@"## {table.Name}");
				bool hasTableNotes = false;

				if (!string.IsNullOrEmpty(table.Notes))
				{
					hasTableNotes = true;
					var notes = Helper.Converter(table.Notes, transform);
					sb.AppendLine(notes);
				}
				bool hasFieldNotes = false;
				foreach (var attribute in table.Attributes)
				{
					if (string.IsNullOrEmpty(attribute.Notes))
					{
						continue;
					}
					hasFieldNotes = true;
					var notes = Helper.Converter(attribute.Notes, transform);
					sb.AppendLine($@"### {attribute.Name}");
					sb.AppendLine(notes);
				}
				bool hasNotes = hasTableNotes || hasFieldNotes;
				if (hasNotes)
				{
					all.Append(sb);
				}
			}
			MarkdownDocument document = Markdown.Parse(all.ToString());
			Replace(block, document);
		}
	}
}
