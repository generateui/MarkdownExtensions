using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System;
using System.IO;

namespace MarkdownExtension.EnterpriseArchitect
{
    public class DiagramImage : IMarkdownExtension
    {
        private class Diagram
        {
            public string Name { get; set; }
        }
        private class DiagramSyntax : ISyntax
        {
            public IParseResult Parse(string text)
            {
                //return new ParseFailure(new ParseError(new Range(new Position(1, 1), new Position(1, 10)), "derpified"));
                return new ParseSuccess(new Diagram { Name = text });
            }
        }

        private class DiagramFormatter : IFormatter
        {
            private readonly IEaProvider _provider;

            public DiagramFormatter(IEaProvider provider)
            {
                _provider = provider;
            }
            public FormatResult Format(object root, IFormatState state)
            {
                var diagram = root as Diagram;
                FilePath filePath = _provider.GetDiagramFilePath(diagram.Name);

                if (File.Exists(filePath.Value))
                {
                    var bytes = File.ReadAllBytes(filePath.Value);
                    var base64 = Convert.ToBase64String(bytes);
                    return FormatResult.FromHtml($@"<img src='data:image/png;base64,{base64}' />");
                }
                return FormatResult.FromHtml($@"<p>something went wrong retrieving the image {diagram.Name}</p>");
            }

            public ICodeByName GetCss() => null;
            public ICodeByName GetJs() => null;
        }

        public DiagramImage(IEaProvider provider)
        {
            Syntax = new DiagramSyntax();
            Formatter = new DiagramFormatter(provider);
        }

        public string Prefix => "ea-diagram";
        public IElementType Type => ElementType.Inline;
        public Output Output => Output.Html;
        public ISyntax Syntax { get; }
        public IFormatter Formatter { get; }
        public static MarkdownExtensionName NAME => "EA diagram";
        public MarkdownExtensionName Name => NAME;
        public IValidator Validator => null;
    }
}
