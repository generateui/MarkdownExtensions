using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect
{
    public class ObjectText : IMarkdownExtension
    {
        public class Element
        {
            public string Name { get; set; }
        }

        private class ElementSyntax : ISyntax
        {
            public IParseResult Parse(string text)
            {
                // [ea:req-arch:Primary keys must be named Id]
                if (text.StartsWith("req-arch"))
                {
                    var value = text.Split(new[] { ':' }, 2)[1];
                    return new ParseSuccess(new Element { Name = value });
                }
                return null;
            }
        }

        private class ElementFormatter : IFormatter
        {
            private readonly IEaProvider _provider;

            public ElementFormatter(IEaProvider provider)
            {
                _provider = provider;
            }

            public FormatResult Format(object root, IFormatState state)
            {
                var element = root as Element;
                var el = _provider.GetElementByName(element.Name);
                if (el != null)
                {
                    return FormatResult.FromHtml(el.Notes); // todo: parse content and generate html recursively
                }
                return null;
            }

            public ICodeByName GetCss() => null;
            public ICodeByName GetJs() => null;
        }

        public ObjectText(IEaProvider provider)
        {
            Syntax = new ElementSyntax();
            Formatter = new ElementFormatter(provider);
        }

        public string Prefix => "ea";

        public IElementType Type => ElementType.Inline;
        public ISyntax Syntax { get; }
        public IFormatter Formatter { get; }
        public Output Output => Output.Html;
        public static MarkdownExtensionName NAME => "EA requirement";
        public MarkdownExtensionName Name => NAME;
        public IValidator Validator => null;
    }
}
