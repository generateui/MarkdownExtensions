using MarkdownExtensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MarkdownExtension.KeyboardKeys
{
    public class KeyboardKeys : IMarkdownExtension
    {
        private class KeyboardKeysTree
        {
            public List<string> Keys { get; set; }
        }
        private class SyntaxImpl : ISyntax
        {
            public IParseResult Parse(string text)
            {
                var keys = text.Split('+').Select(k => k.Trim()).ToList();
                return new ParseSuccess(new KeyboardKeysTree { Keys = keys });
            }
        }

        private class FormatterImpl : IFormatter
        {
            private string _css;

            public FormatResult Format(object root, IFormatState state)
            {
                var kk = root as KeyboardKeysTree;
                var html = string.Join(string.Empty, kk.Keys.Select(k => $@"<kbd>{k}</kbd>"));
                return FormatResult.FromHtml(html);
            }

            public ICodeByName GetCss()
            {
                if (_css == null)
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var resourceName = $@"{GetType().Namespace}.keyscss.css";
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        _css = reader.ReadToEnd();
                    }
                }
                return new CodeByName(NAME, _css);
            }
            public ICodeByName GetJs() => null;
        }

        public KeyboardKeys()
        {
            Syntax = new SyntaxImpl();
            Formatter = new FormatterImpl();
        }

        public string Prefix => "keys";
        public IElementType Type => ElementType.Inline;
        public Output Output => Output.Html;
        public ISyntax Syntax { get; }
        public IFormatter Formatter { get; }
        public static MarkdownExtensionName NAME => "Keyboard keys";
        public MarkdownExtensionName Name => NAME;
        public IValidator Validator => null;
    }
}
