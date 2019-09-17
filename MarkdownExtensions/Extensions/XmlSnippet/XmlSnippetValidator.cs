using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MarkdownExtensions.Extensions.XmlSnippet
{
	public class XmlSnippetValidator : ValidatorBase<XmlSnippet>
	{
		private readonly RenderSettings _renderSettings;

		public XmlSnippetValidator(RenderSettings renderSettings)
		{
			_renderSettings = renderSettings;
		}
		public override IErrors ValidateTyped(XmlSnippet tree, RenderSettings renderSettings)
		{
			var errors = new List<IError>();
			bool fileExists = true;
			try
			{
				XPathExpression expr = XPathExpression.Compile(tree.Xpath);
			}
			catch (XPathException xpe)
			{
				var error = new Error($@"Invalid XPath expression: [{tree.Xpath}]. The XPath compilers' exception message: [{xpe.Message}]");
				errors.Add(error);
			}
			var xmlFile = new File(_renderSettings.SourceFolder, tree.File);
			if (!xmlFile.Exists())
			{
				var error = new Error($@"Referenced xml file [{tree.File}] could not be found at location [{xmlFile.AbsolutePath}]");
				errors.Add(error);
				fileExists = false;
			}
			if (fileExists)
			{
				XDocument document = null;
				bool isValidXmlContent = true;
				try
				{
					document = XDocument.Load(xmlFile.AbsolutePath);
				}
				catch (XmlException)
				{
					var error = new Error($@"Contents of the xml file [{tree.File}] are not valid xml");
					errors.Add(error);
					isValidXmlContent = false;
				}

				if (isValidXmlContent)
				{
					IEnumerable<XElement> queried = document.Root.XPathSelectElements(tree.Xpath);
					if (!queried.Any())
					{
						var error = new Error($@"No xml contents found using the XPath expression [{tree.Xpath}]");
						errors.Add(error);
					}
				}
			}
			return new ValidationResult(errors);
		}
	}
}
