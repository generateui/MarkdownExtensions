using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.RequirementText
{
	public class RequirementTextFormatter : BlockRendererBase<Element, RequirementTextBlock>
	{
		private readonly IEaProvider _provider;

		public RequirementTextFormatter(IEaProvider provider)
		{
			_provider = provider;
		}
		public override void Render(ExtensionHtmlRenderer renderer, Element element, IFormatState formatState)
		{
			var el = _provider.GetElementByName(element.Name);
			if (el != null)
			{
				renderer.Write(el.Notes); // todo: parse content and generate html recursively
			}
		}
	}
}
