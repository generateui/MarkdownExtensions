using System.Collections.Generic;
using System.Linq;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace MarkdownExtensions.Extensions.Note
{
	public class NoteRenderer : HtmlObjectRenderer<NoteParagraphBlock>, IRenderer<Note>
	{
		public IEnumerable<ICode> Css
		{
			get
			{
				yield return new Code("note", "0.0.1", () =>
				{
					return @"
						.note {
							background-color: #fff3cd;
							border: 1px solid orange;
							padding: 1em;
							padding-left: 1em;
						}
						.note:before {
							font-size: 150%;
							content: '\1F4D9';
							padding-right: 0.25em;
						}
					";
				});
			}
		}
		public IEnumerable<ICode> Javascript => null;

		public void Render(ExtensionHtmlRenderer renderer, Note model, IFormatState formatState)
		{
			renderer.RegisterDynamicCss(Css.First());
		}

		protected override void Write(HtmlRenderer renderer, NoteParagraphBlock obj)
		{
			if (!renderer.ImplicitParagraph && renderer.EnableHtmlForBlock)
			{
				if (!renderer.IsFirstInContainer)
				{
					renderer.EnsureLine();
				}
				renderer.Write("<p class='note' ").WriteAttributes(obj).Write(">");
			}
			renderer.WriteLeafInline(obj);
			if (!renderer.ImplicitParagraph)
			{
				if (renderer.EnableHtmlForBlock)
				{
					renderer.WriteLine("</p>");
				}
				renderer.EnsureLine();
			}
		}
	}
}
