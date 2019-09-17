using Markdig.Helpers;
using Markdig.Renderers.Html;
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
			sb.AppendLine("<div class='title'>Contents</div>");
			foreach (var heading in headings)
			{
				var id = HtmlHelper.Unescape(heading.HeadingBlock.GetAttributes().Id);
				sb.AppendLine($@"<a href='#{id}' class='nav-level-{heading.Level}'>{heading.Title}</a>");
			}
			sb.AppendLine("</nav>");
			renderer.Write(sb.ToString());

			var dynamicCss = GenerateCss(model);
			if (dynamicCss != null)
			{
				renderer.RegisterDynamicCss(dynamicCss);
			}
		}

		private ICode GenerateCss(TableOfContent toc)
		{
			var sb = new StringBuilder();
			string contentPrefix = string.Empty;
			void AddContent(string content)
			{
				if (contentPrefix == string.Empty)
				{
					contentPrefix += content;
				}
				else
				{
					contentPrefix += "'.' " + content;
				}
			}
			if (!toc.Level1.Equals(NumberingStyle.None))
			{
				AddContent($@" counter(h1counter, {toc.Level1.Name})");
				sb.AppendLine($@"
					nav, main {{
						counter-reset: h1counter;
					}}
					nav a.nav-level-1:before, h1:before {{
						content: {contentPrefix} '.\0000a0\0000a0';
						counter-increment: h1counter;
					}}
				");
			}
			if (!toc.Level2.Equals(NumberingStyle.None))
			{
				AddContent($@" counter(h2counter, {toc.Level2.Name})");
				sb.AppendLine($@"
					nav a.nav-level-1, h1 {{
						counter-reset: h2counter;
					}}
					nav a.nav-level-2:before, h2:before {{
						content: {contentPrefix} '.\0000a0\0000a0';
						counter-increment: h2counter;
					}}
				");
			}
			if (!toc.Level3.Equals(NumberingStyle.None))
			{
				AddContent($@" counter(h3counter, {toc.Level3.Name})");
				sb.AppendLine($@"
					nav a.nav-level-2 {{
						counter-reset: h3counter;
					}}
					nav a.nav-level-3:before, h3:before {{
						content: {contentPrefix} '.\0000a0\0000a0';
						counter-increment: h3counter;
					}}
				");
			}
			if (!toc.Level4.Equals(NumberingStyle.None))
			{
				AddContent($@" counter(h4counter, {toc.Level4.Name})");
				sb.AppendLine($@"
					nav a.nav-level-3 {{
						counter-reset: h4counter;
					}}
					nav a.nav-level-4:before, h4:before {{
						content: {contentPrefix} '.\0000a0\0000a0';
						counter-increment: h4counter;
					}}
				");
			}
			if (!toc.Level5.Equals(NumberingStyle.None))
			{
				AddContent($@" counter(h5counter, {toc.Level5.Name})");
				sb.AppendLine($@"
					nav a.nav-level-4 {{
						counter-reset: h5counter;
					}}
					nav a.nav-level-5:before, h5:before {{
						content: {contentPrefix} '.\0000a0\0000a0';
						counter-increment: h5counter;
					}}
				");
			}
			if (!toc.Level6.Equals(NumberingStyle.None))
			{
				AddContent($@" counter(h6counter, {toc.Level6.Name})");
				sb.AppendLine($@"
					nav a.nav-level-5 {{
						counter-reset: h6counter;
					}}
					nav a.nav-level-6:before, h6:before {{
						content: {contentPrefix} '.\0000a0\0000a0';
						counter-increment: h6counter;
					}}
				");
			}
			var css = sb.ToString();
			if (css != string.Empty)
			{
				return new Code("table-of-content", "0.0.1 dynamic", () => css);
			}
			return null;
		}

		private readonly string _staticCss = @"
			nav {
				height: 100%;
				width: 200px;
				position: fixed;
				top: 0;
				left: 0;
				padding: 1em;
				background-color: #eee;
			}
			main {
				margin-left: 200px; /* Same as the width of the sidebar */
				padding: 0px 10px;
			}
			nav div.title {
				font-size: 125%;
				padding-bottom: 0.25em;
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

		public override IEnumerable<ICode> Css
		{
			get
			{
				yield return new Code("table-of-content", "0.0.1", () => _staticCss);
			}
		}
	}
}
