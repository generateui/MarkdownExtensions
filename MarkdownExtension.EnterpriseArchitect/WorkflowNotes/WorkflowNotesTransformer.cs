using Markdig;
using Markdig.Extensions.Hardlines;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using MarkdownExtensions.Extensions.Snippet;
using System.IO;
using System.Linq;
using System.Text;

namespace MarkdownExtension.EnterpriseArchitect.WorkflowNotes
{
	public class WorkflowNotesTransformer : TransformerBase<WorkflowNotesBlock, Diagram>
	{
		private readonly IEaProvider _provider;

		public WorkflowNotesTransformer(IEaProvider provider)
		{
			_provider = provider;
		}

		public override void Transform(WorkflowNotesBlock block, Diagram diagram)
		{
			var (element, elementsEnumerable) = _provider.GetBpmnElements(new EaProvider.Path(diagram.Name));
			var elements = elementsEnumerable.ToList();
			elements.Sort(new BpmnElement.AliasComparer());
			var sb = new StringBuilder();
			sb.AppendLine($@"# {element.Name}");
			var converter = new Html2Markdown.Converter();

			foreach (BpmnElement e in elements)
			{
				string name = string.IsNullOrEmpty(e.Name) ? e.Alias : e.Name;
				string notes = converter.Convert(e.Notes);
				MarkdownDocument notesMd = Markdown.Parse(notes);
				notesMd.IncreaseHeadingLevel(2);
				string normalizedNotes = null;
				using (var writer = new StringWriter())
				{
					var pipeline = new MarkdownPipelineBuilder().Build();
					pipeline.Extensions.AddIfNotAlready<SoftlineBreakAsHardlineExtension>();
					var renderer = new NormalizeRenderer(writer);
					pipeline.Setup(renderer);
					renderer.Render(notesMd);
					writer.Flush();
					normalizedNotes = writer.ToString();
				}

				sb.AppendLine($@"## {name}");
				sb.AppendLine($@"Lane: {e.Lane}");
				sb.AppendLine();
				sb.AppendLine($@"Description:");
				sb.AppendLine(normalizedNotes);
			}
			MarkdownDocument document = Markdown.Parse(sb.ToString());
			Replace(block, document);
		}
	}
}
