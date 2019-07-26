using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System;
using System.Linq;
using System.Text;

namespace MarkdownExtension.EnterpriseArchitect
{
	public class WorkflowNotesExtension : IMarkdownExtension
	{
		class Diagram
		{
			public string Name { get; set; }
		}
		class SyntaxImpl : ISyntax
		{
			public IParseResult Parse(string text)
			{
				string diagramName = text.Trim();
				return new ParseSuccess(new Diagram { Name = diagramName });
			}
		}
		class FormatterImpl : IFormatter
		{
			private readonly IEaProvider _provider;

			public FormatterImpl(IEaProvider provider)
			{
				_provider = provider;
			}
			// ARS T&TT.Products.SOSPES Permit.Tenants.Antwerp.SSS SOSPES Permit Antwerp.Workflows.Resident permit.Request.Request.Request resident
			public FormatResult Format(object root, IFormatState state)
			{
				var diagramName = (root as Diagram).Name;
				var (element, elementsEnumerable) = _provider.GetBpmnElements(new Path(diagramName));
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
				return FormatResult.FromHtml(sb.ToString());
			}

			public ICodeByName GetCss() => null;
			public ICodeByName GetJs() => null;
		}

		public WorkflowNotesExtension(IEaProvider provider)
		{
			Syntax = new SyntaxImpl();
			Formatter = new FormatterImpl(provider);
		}

		public string Prefix => "ea-workflow-notes";
		public MarkdownExtensionName Name => "EA workflow notes";
		public Output Output => Output.Html;
		public IElementType Type => ElementType.Block;

		public ISyntax Syntax { get; }
		public IValidator Validator => null;
		public IFormatter Formatter { get; }
	}
}
