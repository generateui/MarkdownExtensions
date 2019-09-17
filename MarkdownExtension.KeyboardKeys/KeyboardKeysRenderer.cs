using MarkdownExtensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MarkdownExtension.KeyboardKeys
{
	public class KeyboardKeysRenderer : InlineRendererBase<KeyboardKeys, KeyboardKeysInline>
	{
		public override void Render(ExtensionHtmlRenderer renderer, KeyboardKeys keyboardKeys, IFormatState formatState)
		{
			var html = string.Join(string.Empty, keyboardKeys.Keys.Select(k => $@"<kbd>{k}</kbd>"));
			renderer.Write(html);
		}

		public override IEnumerable<ICode> Css
		{
			get
			{
				yield return new Code("keyboard-keys", "0.0.1",
					() => Assembly.GetExecutingAssembly().GetFileContent("keyscss.css"));
			}
		}
	}
}
