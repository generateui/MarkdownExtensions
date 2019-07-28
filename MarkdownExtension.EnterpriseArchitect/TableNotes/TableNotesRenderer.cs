using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkdownExtension.EnterpriseArchitect.TableNotes
{
	public class TableNotesRenderer : BlockRendererBase<TableNotes, TableNotesBlock>
	{
		private readonly IEaProvider _provider;

		public TableNotesRenderer(IEaProvider provider)
		{
			_provider = provider;
		}
		public override void Render(ExtensionHtmlRenderer renderer, TableNotes tableNotes, IFormatState formatState)
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
			foreach (var table in tables)
			{
				var sb = new StringBuilder();
				sb.AppendLine($@"## {table.Name}");
				bool hasTableNotes = false;
				if (!string.IsNullOrEmpty(table.Notes))
				{
					hasTableNotes = true;
					sb.AppendLine(table.Notes);
					sb.AppendLine();
				}
				bool hasFieldNotes = false;
				foreach (var attribute in table.Attributes)
				{
					if (!string.IsNullOrEmpty(attribute.Notes))
					{
						hasFieldNotes = true;
						sb.AppendLine($@"### {attribute.Name}");
						sb.AppendLine(attribute.Notes);
						sb.AppendLine();
					}
				}
				bool hasNotes = hasTableNotes || hasFieldNotes;
				if (hasNotes)
				{
					all.Append(sb);
				}
			}
			renderer.Write(all.ToString());
		}
	}
}
