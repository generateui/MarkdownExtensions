using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using SimpleInjector;
using System;
using System.Collections.Generic;
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

	// parses internal syntax of extensions
	public class ExtensionHtmlRenderer : HtmlRenderer
	{
		private readonly ContainerBlock _containerBlock;
		private readonly Dictionary<IExtensionBlock, object> _modelByBlock = new Dictionary<IExtensionBlock, object>();
		private readonly Dictionary<IExtensionInline, object> _modelByInline = new Dictionary<IExtensionInline, object>();
		private readonly Dictionary<Type, Type> _extensionByBlockType = new Dictionary<Type, Type>();
		private readonly Dictionary<Type, Type> _extensionByInlineType = new Dictionary<Type, Type>();
		private readonly Dictionary<IExtensionBlock, IExtension> _extensionByBlock =
			new Dictionary<IExtensionBlock, IExtension>();
		private readonly Dictionary<IExtensionInline, IExtension> _extensionByInline =
			new Dictionary<IExtensionInline, IExtension>();
		private readonly Dictionary<IExtensionBlock, ITransformer> _transformerByBlock =
			new Dictionary<IExtensionBlock, ITransformer>();
		private readonly Dictionary<IExtensionInline, ITransformer> _transformerByInline =
			new Dictionary<IExtensionInline, ITransformer>();
		private readonly List<(IMarkdownExtension Extension, IError Error)> _errors;
		private readonly HashSet<ICode> _csss = new HashSet<ICode>();
		private readonly HashSet<ICode> _javascripts = new HashSet<ICode>();

		public ExtensionHtmlRenderer(TextWriter writer, ContainerBlock containerBlock, RenderSettings renderSettings) : base(writer)
		{
			_containerBlock = containerBlock;
			RenderSettings = renderSettings;
		}

		public FormatState FormatState { get; }
		public RenderSettings RenderSettings { get; }

		public void RegisterImage(string fileName, string absoluteFilePath)
		{
			var newFullFilePath = Path.Combine(RenderSettings.AbsoluteImageFolder, fileName);
			if (!Directory.Exists(RenderSettings.AbsoluteImageFolder))
			{
				Directory.CreateDirectory(RenderSettings.AbsoluteImageFolder);
			}
			File.Copy(absoluteFilePath, newFullFilePath, true); // todo: only overwrite when force new
		}

		public void RegisterBlock<TBlock, TExtension>()
			where TBlock : IExtensionBlock
			where TExtension : IMarkdownExtension
		{
			var blockType = typeof(TBlock);
			var extensionType = typeof(TExtension);
			_extensionByBlockType[blockType] = extensionType;
		}

		public void RegisterInline<TInline, TExtension>()
			where TInline : IExtensionInline
			where TExtension : IMarkdownExtension
		{
			var inlineType = typeof(TInline);
			var extensionType = typeof(TExtension);
			_extensionByInlineType[inlineType] = extensionType;
		}

		/// <summary>
		/// Walks through all the Block extensions and parses the internal syntax of that block
		/// </summary>
		/// <param name="container"></param>
		public void Parse(Container container)
		{
			var extensionBlocks = _containerBlock.GetRecursivelyOfType<IExtensionBlock>();
			foreach (var extensionBlock in extensionBlocks)
			{
				var extensionType = _extensionByBlockType[extensionBlock.GetType()];
				var extension = (IExtension)container.GetInstance(extensionType);
				_extensionByBlock[extensionBlock] = extension;
				IParseResult parseResult = extension.Parser.Parse(extensionBlock.GetContent());
				if (parseResult.Errors != null && parseResult.Errors.Errors.Any())
				{
					parseResult.Errors.Errors.ToList().ForEach(e => _errors.Add((extension, e)));
				}
				else
				{
					_modelByBlock[extensionBlock] = parseResult.SyntaxTree;
				}
				if (extension.Transformer != null)
				{
					_transformerByBlock[extensionBlock] = extension.Transformer;
				}
				var csss = extension?.Renderer?.Css;
				if (csss != null)
				{
					foreach (var css in csss)
					{
						_csss.Add(css);
					}
				}
				var javascripts = extension?.Renderer?.Javascript;
				if (javascripts != null)
				{
					foreach (var javascript in javascripts)
					{
						_javascripts.Add(javascript);
					}
				}
			}
			var extensionInlines = _containerBlock.GetInlinesRecursively();
			foreach (var extensionInline in extensionInlines)
			{
				var extensionType = _extensionByInlineType[extensionInline.GetType()];
				var extension = (IExtension)container.GetInstance(extensionType);
				_extensionByInline[extensionInline] = extension;
				var content = extensionInline.Content;
				IParseResult parseResult = extension.Parser.Parse(content);
				if (parseResult.Errors != null && parseResult.Errors.Errors.Any())
				{
					parseResult.Errors.Errors.ToList().ForEach(e => _errors.Add((extension, e)));
				}
				else
				{
					_modelByInline[extensionInline] = parseResult.SyntaxTree;
				}
				if (extension.Transformer != null)
				{
					_transformerByInline[extensionInline] = extension.Transformer;
				}
				var csss = extension?.Renderer?.Css;
				if (csss != null)
				{
					foreach (var css in csss)
					{
						_csss.Add(css);
					}
				}
				var javascripts = extension?.Renderer?.Javascript;
				if (javascripts != null)
				{
					foreach (var javascript in javascripts)
					{
						_javascripts.Add(javascript);
					}
				}
			}
		}

		public void Validate()
		{
			// TODO: check if css/js dependencies dont collide on versioning
		}

		public object GetBlockModel(IExtensionBlock extensionBlock)
		{
			return _modelByBlock[extensionBlock];
		}
		public object GetInlineModel(IExtensionInline extensionInline)
		{
			return _modelByInline[extensionInline];
		}

		public void Transform()
		{
			foreach (var item in _transformerByBlock)
			{
				var extensionBlock = item.Key as FencedCodeBlock;
				var transformer = item.Value;
				if (transformer != null)
				{
					var model = GetBlockModel(extensionBlock as IExtensionBlock);
					transformer.Transform(this, extensionBlock, model);
				}
			}
		}
		public string CollectCss()
		{
			var sb = new StringBuilder();
			sb.AppendCode(_csss);
			return sb.ToString();
		}

		public string CollectJavascript()
		{
			var sb = new StringBuilder();
			sb.AppendCode(_javascripts);
			return sb.ToString();
		}
	}

	public static class ContainerBlockExtensions
	{
		public static IEnumerable<IExtensionInline> GetInlinesRecursively(this ContainerBlock block, List<IExtensionInline> list = null)
		{
			list = list ?? new List<IExtensionInline>();
			foreach (var child in block)
			{
				if (child is ContainerBlock containerBlock)
				{
					GetInlinesRecursively(containerBlock, list);
				}
				if (child is LeafBlock leafBlock && leafBlock.Inline != null)
				{
					foreach(Inline inline in leafBlock.Inline)
					{
						if (inline is IExtensionInline extensionInline)
						{
							list.Add(extensionInline);
						}
					}
				}
			}
			return list;
		}

		public static IEnumerable<T> GetRecursivelyOfType<T>(this ContainerBlock block, List<T> list = null)
			where T : IExtensionBlock
		{
			list = list ?? new List<T>();
			if (block is IExtensionBlock extensionBlock)
			{
				list.Add((T)extensionBlock);
			}
			foreach (var child in block)
			{
				if (child is ContainerBlock cb)
				{
					GetRecursivelyOfType(cb, list);
				}
				if (child is IExtensionBlock eb)
				{
					list.Add((T)eb);
				}
			}
			return list;
		}
	}
}

	//public class MarkdownExtensionConverter : IMarkdownConverter
//    {
//        private class ExtensionObject
//        {
//            public IParseResult ParseResult { get; set; }
//            public IMarkdownExtension Extension { get; set; }
//        }

//        private class ExtensionHtmlFormatter
//        {
//            private readonly List<(IMarkdownExtension Extension, IError Error)> _errors;
//            private readonly HashSet<int> customFormatted = new HashSet<int>();
//            private CodeByName _csses = new CodeByName();
//            private CodeByName _jss = new CodeByName();
//            private TextWriter _target;
//            private FormatState _formatState = new FormatState();

//            public ExtensionHtmlFormatter(
//                TextWriter target)
//            {
//                _target = target;
//            }

//            public ICodeByName GetCss() => _csses;
//            public ICodeByName GetJs() => _jss;

//            internal void WriteErrors()
//            {
//                if (!_errors.Any())
//                {
//                    return;
//                }
//                var sb = new StringBuilder();
//                sb.AppendLine("<ul class='error-list'>");
//                foreach (var error in _errors)
//                {
//                    IParseError parseError = error.Error as IParseError;
//                    sb.AppendLine("<li class='error'>");
//                    var extensionName = error.Extension.GetType().Name;
//                    if (parseError != null)
//                    {
//                        sb.Append($@"<span class='Range'>{parseError.Range}</span> ");
//                    }
//                    sb.Append($@"<span class='extension-name'>{extensionName}:</span> <span class='message'>{error.Error.Message}</span> ");
//                    sb.Append($@"<span class='message'>{error.Error.Message}</span>");
//                    sb.AppendLine("</li>");
//                }
//                sb.AppendLine("</ul>");
//                _target.WriteLine(sb.ToString());
//            }
//        }

//        private readonly IDictionary<string, IMarkdownExtension> _inlineExtensionByPrefix = new Dictionary<string, IMarkdownExtension>();
//        private readonly IDictionary<string, IMarkdownExtension> _blockExtensionByPrefix = new Dictionary<string, IMarkdownExtension>();
//        private ExtensionHtmlFormatter _formatter;
//        private string _errorCss = string.Empty;

//        public MarkdownExtensionConverter(IEnumerable<IMarkdownExtension> extensions)
//        {
//            foreach (var extension in extensions)
//            {
//                if (extension.Type.Equals(ElementType.Inline))
//                {
//                    _inlineExtensionByPrefix.Add(extension.Prefix, extension);
//                }
//                if (extension.Type.Equals(ElementType.Block))
//                {
//                    _blockExtensionByPrefix.Add(extension.Prefix, extension);
//                }
//            }
//        }

//        public void Convert(TextReader source, TextWriter target, ConversionSettings settings = null, SourceSettings sourceSettings = null)
//        {
//			var pipeline = new MarkdownPipelineBuilder()
//				.UseAdvancedExtensions()
//				.UseAutoLinks()
//				.Build();
//			var writer = new StringWriter();
//			var renderer = new HtmlRenderer(writer);
//			pipeline.Setup(renderer);

//			var document = Markdig.Markdown.Parse(source.ToString(), pipeline);
//			// run transforming extensions (i.e. the ones producing markdown)
//			// run validations
//			renderer.Render(document);
//			// do css + js collection
//			writer.Flush();

//		// [syntax:data]
//		// ```syntax: ```

////            if (errors.Any() && settings.ReportErrorsInHtml)
////            {
////                _errorCss = @"
////.error-list {
////    list-style-type:none;
////    border: 1px solid red;
////    padding: 0.25em;
////}
////li.error:before {
////    content: '\274c';
////    margin-left: 0.25em;
////}

////.error {
////    color: red;
////}
////.range {
////    font-family: 'consolas'; 
////}
////                    ";
////            }
//        }

//		private static readonly ExtensionName NAME = "Core";
//        public ICodeByName GetCss() => 
//            CodeByName.Combine(new CodeByName(NAME, _errorCss), _formatter.GetCss());
//        public ICodeByName GetJs() => _formatter.GetJs();
//    }
//}
