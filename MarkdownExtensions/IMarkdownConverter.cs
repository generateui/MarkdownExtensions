using CommonMark;
using CommonMark.Formatters;
using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MarkdownExtensions
{
    public class ConversionSettings
    {
        /// <summary>
        /// True to embed errors in the output html
        /// </summary>
        public bool ReportErrorsInHtml { get; set; }

        /// <summary>
        /// Forces extensions to re-query their data sources instead of using cached data
        /// </summary>
        public bool ForceRefreshData { get; set; }
    }

    public interface IMarkdownConverter
    {
        void Convert(TextReader source,
            TextWriter target,
            ConversionSettings settings = null,
            SourceSettings sourceSettings = null);

        void Convert(Block block,
            TextWriter target,
            ConversionSettings settings = null,
            SourceSettings sourceSettings = null);

        ICodeByName GetCss();
        ICodeByName GetJs();
    }

    public static class MarkdownConverterExtensions
    {
        public static string Convert(
            this IMarkdownConverter converter,
            string source,
            ConversionSettings settings = null,
            SourceSettings sourceSettings = null)
        {
            settings = settings ?? new ConversionSettings { ReportErrorsInHtml = true };
            sourceSettings = sourceSettings ?? new SourceSettings();
            using (var reader = new StringReader(source))
            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                converter.Convert(reader, writer, settings, sourceSettings);
                return writer.ToString();
            }
        }
    }

    public class MarkdownExtensionConverter : IMarkdownConverter
    {
        private class ExtensionObject
        {
            public IParseResult ParseResult { get; set; }
            public IMarkdownExtension Extension { get; set; }
        }
        private class Parsed
        {
            public Parsed(Dictionary<int, ExtensionObject> extensionsByPosition, List<(IMarkdownExtension extension, IError error)> errors)
            {
                ExtensionsByPosition = extensionsByPosition;
                Errors = errors;
            }
            public Dictionary<int, ExtensionObject> ExtensionsByPosition { get; }
            public List<(IMarkdownExtension Extension, IError Error)> Errors { get; }
        }
        private class ExtensionHtmlFormatter : HtmlFormatter
        {
            private readonly Parsed _parsed;
            private readonly HashSet<int> customFormatted = new HashSet<int>();
            private CodeByName _csses = new CodeByName();
            private CodeByName _jss = new CodeByName();
            private TextWriter _target;
            private FormatState _formatState = new FormatState();

            public ExtensionHtmlFormatter(
                TextWriter target,
                CommonMarkSettings settings,
                Parsed parsed)
                : base(target, settings)
            {
                _parsed = parsed;
                _target = target;
            }

            public ICodeByName GetCss() => _csses;
            public ICodeByName GetJs() => _jss;

            protected override void WriteInline(Inline inline, bool isOpening, bool isClosing, out bool ignoreChildNodes)
            {
                if (_parsed.ExtensionsByPosition.ContainsKey(inline.SourcePosition))
                {
                    if (!customFormatted.Contains(inline.SourcePosition))
                    {
                        customFormatted.Add(inline.SourcePosition);
                        var extensionObject = _parsed.ExtensionsByPosition[inline.SourcePosition];
                        var formatter = extensionObject.Extension.Formatter;
                        var result = formatter.Format(extensionObject.ParseResult.Object, _formatState);
                        if (result != null)
                        {
                            Write(result.Html.Value);
                            _csses.Add(formatter.GetCss());
                            _jss.Add(formatter.GetJs());
                        }
                        else
                        {
                            // TODO: report error
                        }
                    }
                    ignoreChildNodes = true;
                }
                else
                {
                    ignoreChildNodes = false;
                    base.WriteInline(inline, isOpening, isClosing, out ignoreChildNodes);
                }
            }

            protected override void WriteBlock(Block block, bool isOpening, bool isClosing, out bool ignoreChildNodes)
            {
                if (block.Tag == BlockTag.AtxHeading)
                {
                    _formatState.HeadingLevel = block.Heading.Level;
                }
                if (_parsed.ExtensionsByPosition.ContainsKey(block.SourcePosition))
                {
                    if (!customFormatted.Contains(block.SourcePosition))
                    {
                        customFormatted.Add(block.SourcePosition);
                        ExtensionObject extensionObject = _parsed.ExtensionsByPosition[block.SourcePosition];
                        var formatter = extensionObject.Extension.Formatter;
                        var result = formatter.Format(extensionObject.ParseResult.Object, _formatState);
                        if (result != null)
                        {
                            Write(result.Html.Value);
                            //if (result.Html != null)
                            //{
                            //}
                            //if (result.Markdown != null)
                            //{
                            //    var markdownConverter = _getMarkdownConverter();
                            //    using (var reader = new StringReader(result.Markdown.Value))
                            //    using (var writer = new StringWriter(CultureInfo.CurrentCulture))
                            //    {
                            //        var parseSettings = CommonMarkSettings.Default.Clone();
                            //        Block b = CommonMarkConverter.ProcessStage1(reader, parseSettings);
                            //        CommonMarkConverter.ProcessStage2(b, parseSettings);
                            //        markdownConverter.Convert(b, writer);
                            //        var html = writer.ToString();
                            //        Write(html);
                            //    }
                            //}
                            _csses.Add(formatter.GetCss());
                            _jss.Add(formatter.GetJs());
                        }
                        else
                        {
                            // TODO: report error
                        }
                    }
                    ignoreChildNodes = true;
                }
                else
                {
                    ignoreChildNodes = false;
                    base.WriteBlock(block, isOpening, isClosing, out ignoreChildNodes);
                }
            }

            internal void WriteErrors()
            {
                if (!_parsed.Errors.Any())
                {
                    return;
                }
                var sb = new StringBuilder();
                sb.AppendLine("<ul class='error-list'>");
                foreach (var error in _parsed.Errors)
                {
                    IParseError parseError = error.Error as IParseError;
                    sb.AppendLine("<li class='error'>");
                    var extensionName = error.Extension.GetType().Name;
                    if (parseError != null)
                    {
                        sb.Append($@"<span class='Range'>{parseError.Range}</span> ");
                    }
                    sb.Append($@"<span class='extension-name'>{extensionName}:</span> <span class='message'>{error.Error.Message}</span> ");
                    sb.Append($@"<span class='message'>{error.Error.Message}</span>");
                    sb.AppendLine("</li>");
                }
                sb.AppendLine("</ul>");
                _target.WriteLine(sb.ToString());
            }
        }

        private readonly IDictionary<string, IMarkdownExtension> _inlineExtensionByPrefix = new Dictionary<string, IMarkdownExtension>();
        private readonly IDictionary<string, IMarkdownExtension> _blockExtensionByPrefix = new Dictionary<string, IMarkdownExtension>();
        private ExtensionHtmlFormatter _formatter;
        private string _errorCss = string.Empty;
        private CommonMarkSettings _parseSettings;

        public MarkdownExtensionConverter(IEnumerable<IMarkdownExtension> extensions)
        {
            foreach (var extension in extensions)
            {
                if (extension.Type.Equals(ElementType.Inline))
                {
                    _inlineExtensionByPrefix.Add(extension.Prefix, extension);
                }
                if (extension.Type.Equals(ElementType.Block))
                {
                    _blockExtensionByPrefix.Add(extension.Prefix, extension);
                }
            }
            _parseSettings = CommonMarkSettings.Default.Clone();
            _parseSettings.AdditionalFeatures = CommonMarkAdditionalFeatures.PlaceholderBracket;
            _parseSettings.TrackSourcePosition = true;
        }

        public void Convert(TextReader source, TextWriter target, ConversionSettings settings = null, SourceSettings sourceSettings = null)
        {
            sourceSettings = sourceSettings ?? new SourceSettings();
            Block block = CommonMarkConverter.ProcessStage1(source, _parseSettings);
            CommonMarkConverter.ProcessStage2(block, _parseSettings);
            Convert(block, target, settings, sourceSettings);
        }

        public void Convert(Block block, TextWriter target, ConversionSettings settings = null, SourceSettings sourceSettings = null)
        {
            sourceSettings = sourceSettings ?? new SourceSettings();
            var extensionsByPosition = new Dictionary<int, ExtensionObject>();
            var errors = new List<(IMarkdownExtension extension, IError error)>();
            var replacements = new List<(IMarkdownExtension extension, Block original, Block replacement)>();
            var formatState = new FormatState();

            // [syntax:data]
            bool IsInlineExtension(EnumeratorEntry entry) =>
                entry.Inline != null && entry.Inline.Tag == InlineTag.Placeholder && entry.IsOpening;

            // ```syntax: ```
            bool IsBlockExtension(EnumeratorEntry entry) =>
                entry.Block != null && entry.Block.Tag == BlockTag.FencedCode && entry.IsOpening;

            EnumeratorEntry previous = null;
            foreach (EnumeratorEntry entry in block.AsEnumerable())
            {
                if (entry.Block != null && entry.Block.Tag == BlockTag.AtxHeading)
                {
                    formatState.HeadingLevel = entry.Block.Heading.Level;
                }
                if (IsInlineExtension(entry))
                {
                    var placeholder = entry.Inline;
                    var text = placeholder.TargetUrl;
                    var content = GetSyntax(text);
                    var prefix = content.Item1;
                    var code = content.Item2;
                    if (_inlineExtensionByPrefix.ContainsKey(prefix))
                    {
                        var extension = _inlineExtensionByPrefix[prefix];
                        IParseResult result = extension.Syntax.Parse(code);
                        var extensionObject = new ExtensionObject
                        {
                            ParseResult = result,
                            Extension = extension
                        };
                        if (result.Errors.Errors.Any())
                        {
                            result.Errors.Errors.ToList().ForEach(e => errors.Add((extension, e)));
                        }
                        else
                        {
                            var validator = extension.Validator;
                            IErrors validationResult = null;
                            if (validator != null)
                            {
                                validationResult = validator.Validate(extensionObject.ParseResult.Object, sourceSettings);
                            }
                            if (validationResult != null && validationResult.Errors != null && validationResult.Errors.Any())
                            {
                                validationResult.Errors.ToList().ForEach(e => errors.Add((extension, e)));
                            }
                            else
                            {
                                if (extension.Output == Output.Markdown)
                                {
                                    var markdownResult = extension.Formatter.Format(extensionObject.ParseResult.Object, formatState);
                                    using (var reader = new StringReader(markdownResult.Markdown.Value))
                                    using (var writer = new StringWriter(CultureInfo.CurrentCulture))
                                    {
                                        Block mdBlock = CommonMarkConverter.ProcessStage1(reader, _parseSettings);
                                        CommonMarkConverter.ProcessStage2(mdBlock, _parseSettings);
                                        //replacements.Add((extension, placeholder, mdBlock));
                                        var (first, last) = GetInlines(mdBlock);
                                        if (previous.Inline != null)
                                        {
                                            previous.Inline.NextSibling = first;
                                        }
                                        last.NextSibling = placeholder.NextSibling;
                                    }
                                }
                                else if (extension.Output == Output.Html)
                                {
                                    extensionsByPosition.Add(placeholder.SourcePosition, extensionObject);
                                }
                            }
                        }
                    }
                }
                if (IsBlockExtension(entry))
                {
                    var codeBlock = entry.Block;
                    var text = codeBlock.StringContent.ToString();
                    var prefix = codeBlock.FencedCodeData.Info.Trim();
                    prefix = prefix.Substring(0, prefix.Length - 1);
                    if (_blockExtensionByPrefix.ContainsKey(prefix))
                    {
                        var extension = _blockExtensionByPrefix[prefix];
                        var result = extension.Syntax.Parse(text);
                        var extensionObject = new ExtensionObject
                        {
                            ParseResult = result,
                            Extension = extension
                        };
                        if (result.Errors.Errors.Any())
                        {
                            result.Errors.Errors.ToList().ForEach(e => errors.Add((extension, e)));
                        }
                        else
                        {
                            var validator = extension.Validator;
                            IErrors validationResult = null;
                            if (validator != null)
                            {
                                validationResult = validator.Validate(extensionObject.ParseResult.Object, sourceSettings);
                            }
                            if (validationResult != null && validationResult.Errors != null && validationResult.Errors.Any())
                            {
                                validationResult.Errors.ToList().ForEach(e => errors.Add((extension, e)));
                            }
                            else
                            {
                                if (extension.Output == Output.Markdown)
                                {
                                    var markdownResult = extension.Formatter.Format(extensionObject.ParseResult.Object, formatState);
                                    using (var reader = new StringReader(markdownResult.Markdown.Value))
                                    using (var writer = new StringWriter(CultureInfo.CurrentCulture))
                                    {
                                        Block mdBlock = CommonMarkConverter.ProcessStage1(reader, _parseSettings);
                                        CommonMarkConverter.ProcessStage2(mdBlock, _parseSettings);
                                        replacements.Add((extension, codeBlock, mdBlock));
                                        if (previous.Block != null)
                                        {
                                            previous.Block.NextSibling = mdBlock;
                                        }
                                        mdBlock.NextSibling = codeBlock.NextSibling;
                                    }
                                }
                                else if (extension.Output == Output.Html)
                                {
                                    extensionsByPosition.Add(codeBlock.SourcePosition, extensionObject);
                                }
                            }
                        }
                    }
                }
                previous = entry;
            }

            var outputSettings = CommonMarkSettings.Default.Clone();
            var parsed = new Parsed(extensionsByPosition, errors);
            outputSettings.TrackSourcePosition = false;
            outputSettings.AdditionalFeatures = CommonMarkAdditionalFeatures.PlaceholderBracket;
            outputSettings.OutputDelegate = (doc, output, s) =>
            {
                _formatter = new ExtensionHtmlFormatter(output, s, parsed);
                _formatter.WriteErrors();
                _formatter.WriteDocument(doc);
            };
            CommonMarkConverter.ProcessStage3(block, target, outputSettings);

            if (errors.Any() && settings.ReportErrorsInHtml)
            {
                _errorCss = @"
.error-list {
    list-style-type:none;
    border: 1px solid red;
    padding: 0.25em;
}
li.error:before {
    content: '\274c';
    margin-left: 0.25em;
}

.error {
    color: red;
}
.range {
    font-family: 'consolas'; 
}
                    ";
            }
        }

        private Tuple<string, string> GetSyntax(string text)
        {
            var prefix = string.Empty;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ':')
                {
                    return Tuple.Create(prefix, text.Substring(i + 1));
                }
                prefix += text[i];
            }
            return null;
        }

        private (Inline First, Inline Last) GetInlines(Block block)
        {
            var elements = block.AsEnumerable().ToList();
            var first = elements[2];
            var last = elements[elements.Count - 3];
            return (first.Inline, last.Inline);
        }

        private static readonly MarkdownExtensionName NAME = "Core";
        public ICodeByName GetCss() => 
            CodeByName.Combine(new CodeByName(NAME, _errorCss), _formatter.GetCss());
        public ICodeByName GetJs() => _formatter.GetJs();
    }
}
