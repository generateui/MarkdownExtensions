using Markdig;
using Markdig.Syntax;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using MarkdownExtensions.Extensions.Snippet;
using System;
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

		public override void Transform(ExtensionHtmlRenderer extensionHtmlRenderer, TableNotesBlock block, TableNotes tableNotes)
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
				//tables = _provider
				//	.GetElements(e => e.Stereotype == "table")
				//	.Where(e => TableNotes.Include(tableNotes, e));
				throw new NotImplementedException();

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
					var notes = Helper.Converter(table.Notes, transform, extensionHtmlRenderer.Pipeline);
					sb.AppendLine(notes);
				}
				bool hasFieldNotes = false;
				sb.AppendLine($@"### Properties");
				foreach (var taggedValue in table.TaggedValues)
				{
					if (string.IsNullOrEmpty(taggedValue.Value))
					{
						continue;
					}
					sb.AppendLine($@"- **{taggedValue.Key}**: {taggedValue.Value}");
				}

				sb.AppendLine($@"### Attributes");
				foreach (var attribute in table.Attributes)
				{
					if (string.IsNullOrEmpty(attribute.Notes))
					{
						continue;
					}
					hasFieldNotes = true;
					var notes = Helper.Converter(attribute.Notes, transform, extensionHtmlRenderer.Pipeline);
					sb.AppendLine($@"#### {attribute.Name}");
					sb.AppendLine(notes);
				}
				bool hasNotes = hasTableNotes || hasFieldNotes;
				if (hasNotes)
				{
					all.Append(sb);
				}
			}

			MarkdownDocument document = Markdown.Parse(all.ToString(), extensionHtmlRenderer.Pipeline);
			Replace(block, document);
		}
	}
}
