using MarkdownExtensions;
using System;
using System.Reflection;

namespace MarkdownExtension.PanZoomImage
{
    public class PanZoomImage : IMarkdownExtension
    {
        private class PanZoomImageTree
        {
            public string Uri { get; set; }
        }

        private class SyntaxImpl : ISyntax
        {
            public IParseResult Parse(string text)
            {
                return new ParseSuccess(new PanZoomImageTree { Uri = text.Trim() });
            }
        }

        private class FormatterImpl : IFormatter
        {
            private int _id;
            public FormatResult Format(object root, IFormatState state)
            {
                var panZoomImage = root as PanZoomImageTree;
                var bytes = System.IO.File.ReadAllBytes(panZoomImage.Uri);
                var base64 = Convert.ToBase64String(bytes);
                string html = $@"
                    <img src='data:image/png;base64,{base64}' id='{_id}'>
                    <script>panzoom(document.getElementById('{_id}'));</script>";
                _id++;
                return FormatResult.FromHtml(html);
            }

            public ICodeByName GetCss() => null;
            public ICodeByName GetJs() => 
                new CodeByName(NAME, Assembly.GetExecutingAssembly().GetFileContent("panzoom.min.js"));
        }

        public string Prefix => "pan-zoom-image";
        public IElementType Type => ElementType.Block;
        public Output Output => Output.Html;
        public ISyntax Syntax { get; }
        public IFormatter Formatter { get; }
        public static MarkdownExtensionName NAME => "Pan & zoom an image";
        public MarkdownExtensionName Name => NAME;
        public IValidator Validator => null;

        public PanZoomImage()
        {
            Syntax = new SyntaxImpl();
            Formatter = new FormatterImpl();
        }
    }
}
