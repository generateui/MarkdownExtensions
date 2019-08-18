using Markdig;
using Markdig.Syntax;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using MarkdownExtensions.Extensions.Snippet;
using System.Text;

namespace MarkdownExtension.EnterpriseArchitect.ObjectText
{
	public class ObjectTextTransformer : TransformerBase<ObjectTextBlock, ObjectText>
	{
		private readonly IEaProvider _provider;

		public ObjectTextTransformer(IEaProvider provider)
		{
			_provider = provider;
		}

		public override void Transform(ExtensionHtmlRenderer extensionHtmlRenderer, ObjectTextBlock block, ObjectText astNode)
		{
			var pb = new MarkdownPipelineBuilder()
				.UseSoftlineBreakAsHardlineBreak()
				.UseAdvancedExtensions();
			var pipeline = pb.Build();

			var path = new Path(astNode.PackageName);
			var elements = _provider.GetElements(path, recursive: false);
			var sb = new StringBuilder();
			var headingLevel = block.GetHeadingLevel();
			void transform(MarkdownDocument md) { md.IncreaseHeadingLevel(1); }
			foreach (var element in elements)
			{
				sb.AppendLine($@"# {element.Name}");
				var notes = Helper.Converter(element.Notes, transform, pipeline);
				sb.AppendLine(element.Notes);
			}
			var document = Markdown.Parse(sb.ToString(), pipeline);
			document.IncreaseHeadingLevel(headingLevel);
			Replace(block, document);
		}
	}
}
