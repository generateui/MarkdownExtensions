using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using System.Collections.Generic;
using System.Text;

namespace MarkdownExtensions.Extensions.TableOfContent
{
	public class TableOfContentRenderer : BlockRendererBase<TableOfContent, TableOfContentBlock>
	{
		public override void Render(ExtensionHtmlRenderer renderer, TableOfContent model, IFormatState formatState)
		{
			List<Heading> headings = renderer.ContainerBlock.GetHeadings();
			var sb = new StringBuilder();
			sb.AppendLine("<nav>");
			foreach (var heading in headings)
			{
				var id = HtmlHelper.Unescape(heading.HeadingBlock.GetAttributes().Id);
				sb.AppendLine($@"<a href='#{id}' class='nav-level-{heading.Level}'>{heading.Title}</a>");
			}
			sb.AppendLine("</nav>");
			renderer.Write(sb.ToString());
		}

		public override IEnumerable<ICode> Css
		{
			get
			{
				yield return new Code("table-of-content", "0.0.1", () =>
				{
					return @"
						nav {
							height: 100%; /* Full-height: remove this if you want auto height */
							width: 160px; /* Set the width of the sidebar */
							position: fixed; /* Fixed Sidebar (stay in place on scroll) */
							top: 0; /* Stay at the top */
							left: 0;
							padding: 1em;
						}
						main {
							margin-left: 160px; /* Same as the width of the sidebar */
							padding: 0px 10px;
						}
						nav a {
							display: block;
						}
						nav a.nav-level-1 {
							padding-left: 0;
						}
						nav a.nav-level-2 {
							padding-left: 1em;
						}
						nav a.nav-level-3 {
							padding-left: 2em;
						}
						nav a.nav-level-4 {
							padding-left: 3em;
						}
						nav a.nav-level-5 {
							padding-left: 4em;
						}
						nav a.nav-level-6 {
							padding-left: 5em;
						}
					";
				});
			}
		}
	}
}
