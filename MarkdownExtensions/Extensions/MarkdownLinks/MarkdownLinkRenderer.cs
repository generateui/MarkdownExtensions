using Markdig.Renderers;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace MarkdownExtensions.Extensions.MarkdownLinks
{
	public class MarkdownLinkRenderer : LinkInlineRenderer
	{
        protected override void Write(HtmlRenderer renderer, LinkInline link)
		{
			if (link.Url != null && link.Url.EndsWith(".md"))
			{
				link.Url = link.Url.Substring(0, link.Url.Length - 3) + ".html";
			}
			base.Write(renderer, link);
		}
	}
}
