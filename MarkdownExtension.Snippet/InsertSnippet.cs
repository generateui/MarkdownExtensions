using CommonMark;
using CommonMark.Syntax;
using MarkdownExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MarkdownExtension.Snippet
{
    public class Snippet : IMarkdownExtension
    {
        private class SnippetReference
        {
            public string FileName { get; set; }
            public string HeadingName { get; set; }
            public bool AsSiblingHeading { get; set; } // =
            public bool AsChildHeading { get; set; } // >
        }
        private class SyntaxImpl : ISyntax
        {
            public IParseResult Parse(string text)
            {
                var snippetReference = new SnippetReference();
                if (text.StartsWith("="))
                {
                    snippetReference.AsSiblingHeading = true;
                    text = text.Substring(1, text.Length - 1);
                }
                else if (text.StartsWith(">"))
                {
                    snippetReference.AsChildHeading = true;
                    text = text.Substring(1, text.Length - 1);
                }
                var split = text.Split(':');
                snippetReference.FileName = split[0];
                snippetReference.HeadingName = split[1];
                return new ParseSuccess(snippetReference);
            }
        }

        private class FormatterImpl : IFormatter
        {
            private readonly Func<IMarkdownConverter> _getMarkdownConverter;
            private IMarkdownConverter _markdownConverter;

            public FormatterImpl(Func<IMarkdownConverter> getMarkdownConverter)
            {
                _getMarkdownConverter = getMarkdownConverter;
            }
            public FormatResult Format(object root, IFormatState state)
            {
                var snippetReference = root as SnippetReference;

                if (!File.Exists(snippetReference.FileName))
                {
                    return null; // TODO: return error?
                }
                using (var reader = File.OpenText(snippetReference.FileName))
                using (var writer = new StringWriter(CultureInfo.CurrentCulture))
                {
                    var parseSettings = CommonMarkSettings.Default.Clone();
                    parseSettings.AdditionalFeatures = CommonMarkAdditionalFeatures.PlaceholderBracket;
                    parseSettings.TrackSourcePosition = true;
                    Block block = CommonMarkConverter.ProcessStage1(reader, parseSettings);
                    var heading = GetHeadingByName(block, snippetReference.HeadingName);
                    if (heading == null)
                    {
                        return null;
                    }
                    CommonMarkConverter.ProcessStage2(heading, parseSettings);
                    int? baseLevel = null;
                    if (snippetReference.AsChildHeading)
                    {
                        baseLevel = state.HeadingLevel + 1;
                    }
                    if (snippetReference.AsSiblingHeading)
                    {
                        baseLevel = state.HeadingLevel;
                    }
                    if (baseLevel != null)
                    {
                        SetHeadingLevel(block.AsEnumerable(), baseLevel.Value);
                    }
                    _markdownConverter = _markdownConverter ?? _getMarkdownConverter();
                    _markdownConverter.Convert(heading, writer);
                    return FormatResult.FromHtml(writer.ToString());
                }
            }

            private void SetHeadingLevel(IEnumerable<EnumeratorEntry> children, int level, int? minLevel = null)
            {
                bool first = true;
                foreach(var child in children)
                {
                    if (child.IsClosing) { continue; }
                    if (child.Block != null && child.Block.Heading.Level != 0)
                    {
                        if (minLevel == null)
                        {
                            minLevel = child.Block.Heading.Level;
                        }
                        int diff = child.Block.Heading.Level - minLevel.Value;
                        child.Block.Heading = new HeadingData(level + diff);
                    }
                    if (!first && child.Block != null && child.Block.FirstChild != null)
                    {
                        SetHeadingLevel(child.Block.AsEnumerable(), level, minLevel);
                    }
                    first = false;
                }
            }

            private Block GetHeadingByName(Block block, string name)
            {
                int level = 0;
                var items = new List<Block>();
                bool collect = false;
                Block last = null;
                foreach (var child in block.AsEnumerable())
                {
                    if (child.Block.Tag == BlockTag.AtxHeading)
                    {
                        if (collect && child.Block.Heading.Level <= level)
                        {
                            last.NextSibling = null;
#pragma warning disable 0618
                            Block e = new Block(BlockTag.Document, 1, 1, 0);
#pragma warning restore 0618
                            e.Document = new DocumentData();
                            e.Document.ReferenceMap = new Dictionary<string, Reference>();
                            e.Top = e;
                            e.FirstChild = items[0];
                            return e;
                        }
                        if (child.Block.StringContent.ToString() == name)
                        {
                            level = child.Block.Heading.Level;
                            collect = true;
                        }
                    }
                    if (collect)
                    {
                        items.Add(child.Block);
                    }
                    last = child.Block;
                }
                return null;
            }

            public ICodeByName GetCss() => _markdownConverter.GetCss();
            public ICodeByName GetJs() => _markdownConverter.GetJs();
        }

        public Snippet(Func<IMarkdownConverter> getMarkdownConverter)
        {
            Syntax = new SyntaxImpl();
            Formatter = new FormatterImpl(getMarkdownConverter);
        }

        public string Prefix => "md-part";
        public IElementType Type => ElementType.Inline;
        public Output Output => Output.Html;
        public ISyntax Syntax { get; }
        public IFormatter Formatter { get; }
        public IValidator Validator => null;
        public static MarkdownExtensionName NAME => "Markdown snippets";
        public MarkdownExtensionName Name => NAME;
    }
}
