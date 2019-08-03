using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System.Linq;
using System.Text;

namespace MarkdownExtension.EnterpriseArchitect.WorkflowNotes
{
	public class WorkflowNotesRenderer : BlockRendererBase<Diagram, WorkflowNotesBlock>
	{
		private readonly IEaProvider _provider;

		public WorkflowNotesRenderer(IEaProvider provider)
		{
			_provider = provider;
		}
		// ARS T&TT.Products.SOSPES Permit.Tenants.Antwerp.SSS SOSPES Permit Antwerp.Workflows.Resident permit.Request.Request.Request resident
		public override void Render(ExtensionHtmlRenderer renderer, Diagram diagram, IFormatState formatState)
		{
			var (element, elementsEnumerable) = _provider.GetBpmnElements(new Path(diagram.Name));
			var elements = elementsEnumerable.ToList();
			elements.Sort(new BpmnElement.AliasComparer());
			var sb = new StringBuilder();
			sb.AppendLine($@"<h1>{element.Name}</h1>");
			foreach (var e in elements)
			{
				string name = string.IsNullOrEmpty(e.Name) ? e.Alias : e.Name;
				sb.AppendLine($@"<h2>{name}</h2>");
				sb.AppendLine($@"<p>Lane: {e.Lane}</p>");
				sb.AppendLine($@"<p>Description:<br>");
				sb.AppendLine($@"{e.Notes}</p>");
			}
			renderer.Write(sb.ToString());
		}
	}
}
