using Markdig;
using Markdig.Syntax;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MarkdownExtensions.Extensions.XmlSnippet
{
	public class XmlSnippetTransformer : TransformerBase<XmlSnippetBlock, XmlSnippet>
	{
		private RenderSettings _renderSettings;

		public XmlSnippetTransformer(RenderSettings renderSettings)
		{
			_renderSettings = renderSettings;
		}
		public override void Transform(ExtensionHtmlRenderer extensionHtmlRenderer, XmlSnippetBlock block, XmlSnippet astNode)
		{
			// get xml content using xpath
			// create new xml fcb with xml content
			// replace
			var xmlFile = new File(_renderSettings.SourceFolder, astNode.File);
			XDocument document = XDocument.Load(xmlFile.AbsolutePath);
			IEnumerable<XElement> queried = document.Root.XPathSelectElements(astNode.Xpath);
			var sb = new StringBuilder();
			sb.AppendLine("```xml");
			foreach (var element in queried)
			{
				sb.AppendLine(element.ToString());
			}
			sb.AppendLine("```");
			MarkdownDocument markdownDocument = Markdown.Parse(sb.ToString());
			Replace(block, markdownDocument);
		}
	}
}
