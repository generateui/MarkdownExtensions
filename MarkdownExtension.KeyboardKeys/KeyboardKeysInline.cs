using Markdig.Syntax.Inlines;
using MarkdownExtensions;

namespace MarkdownExtension.KeyboardKeys
{
	public class KeyboardKeysInline : Inline, IExtensionInline
	{
		public KeyboardKeysInline(string text)
		{
			Text = text;
		}
		public string Text { get; }

		public string Content => Text;
	}
}
