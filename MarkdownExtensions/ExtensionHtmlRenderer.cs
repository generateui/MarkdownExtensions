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
	// parses internal syntax of extensions
	public class ExtensionHtmlRenderer : HtmlRenderer
	{
        private readonly Dictionary<Type, IMarkdownObjectRenderer> _renderersPerType = new Dictionary<Type, IMarkdownObjectRenderer>();
        private IMarkdownObjectRenderer _previousRenderer;
        private Type _previousObjectType;
		private readonly ContainerBlock _containerBlock;
		private readonly RenderSettings _renderSettings;
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
		private readonly Dictionary<IExtensionBlock, IErrors> _blockErrors = new Dictionary<IExtensionBlock, IErrors>();
		private readonly Dictionary<IExtensionInline, IErrors> _inlineErrors = new Dictionary<IExtensionInline, IErrors>();
		private readonly HashSet<ICode> _csss = new HashSet<ICode>();
		private readonly HashSet<ICode> _javascripts = new HashSet<ICode>();

		public ExtensionHtmlRenderer(TextWriter writer, ContainerBlock containerBlock, RenderSettings renderSettings) : base(writer)
		{
			_containerBlock = containerBlock;
			_renderSettings = renderSettings;
			FolderManager = new FolderManager(renderSettings);
		}
		public ExtensionHtmlRenderer(TextWriter writer, ContainerBlock containerBlock, FolderManager folderManager) : base(writer)
		{
			_containerBlock = containerBlock;
			FolderManager = folderManager;
		}

		public FormatState FormatState { get; }
		public FolderManager FolderManager { get; }

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
					_blockErrors.Add(extensionBlock, parseResult.Errors);
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
					_inlineErrors.Add(extensionInline, parseResult.Errors);
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

		public void Validate(Container container)
		{
			var extensionBlocks = _containerBlock.GetRecursivelyOfType<IExtensionBlock>();
			foreach (var extensionBlock in extensionBlocks)
			{
				if (_blockErrors.ContainsKey(extensionBlock))
				{
					// don't check for model errors if parsing didn't succeed
					continue;
				}
				var extensionType = _extensionByBlockType[extensionBlock.GetType()];
				var extension = (IExtension)container.GetInstance(extensionType);
				var validator = extension.Validator;
				if (validator == null)
				{
					continue;
				}
				var model = _modelByBlock[extensionBlock];
				IErrors validationResult = validator.Validate(model, _renderSettings);
				if (validationResult.Errors.Any())
				{
					_blockErrors.Add(extensionBlock, validationResult);
				}
			}
			var extensionInlines = _containerBlock.GetInlinesRecursively();
			foreach (var extensionInline in extensionInlines)
			{
				if (_inlineErrors.ContainsKey(extensionInline))
				{
					// don't check for model errors if parsing didn't succeed
					continue;
				}
				var extensionType = _extensionByInlineType[extensionInline.GetType()];
				var extension = (IExtension)container.GetInstance(extensionType);
				var validator = extension.Validator;
				if (validator == null)
				{
					continue;
				}
				var model = _modelByInline[extensionInline];
				var validationResult = validator.Validate(model, _renderSettings);
				if (validationResult.Errors.Any())
				{
					_inlineErrors.Add(extensionInline, validationResult);
				}
			}
		}

		public override object Render(MarkdownObject markdownObject)
		{
			WriteInternal(markdownObject);
			return Writer;
		}

		/// <summary>
		/// Writes the specified Markdown object.
		/// </summary>
		/// <typeparam name="T">A MarkdownObject type</typeparam>
		/// <param name="obj">The Markdown object to write to this renderer.</param>
		/// <remarks>
		/// Write{T}(T obj) cannot be overridden. Therefore, copy 'n paste the complete impl
		/// of Render and modify it slightly so that errors are rendered.
		/// </remarks>
		public void WriteInternal<T>(T obj) where T : MarkdownObject
		{
			if (obj == null)
			{
				return;
			}
			bool isError = false;
			if (obj is IExtensionBlock block && _blockErrors.ContainsKey(block))
			{
				var errors = _blockErrors[block];
				var sb = new StringBuilder();
				sb.AppendLine("<ul class='error-list'>");
				foreach (var error in errors.Errors)
				{
					sb.AppendLine("<li class='error'>");
					var extension = _extensionByBlock[block];
					var extensionName = extension.GetType().Name;
					IParseError parseError = error as IParseError;
					if (parseError != null)
					{
						sb.Append($@"<span class='Range'>{parseError.Range}</span> ");
					}
					sb.Append($@"<span class='extension-name'>{extensionName}:</span>");
					sb.Append($@"<span class='message'>{error.Message}</span>");
					sb.AppendLine("</li>");
				}
				sb.AppendLine("</ul>");
				WriteLine(sb.ToString());
				isError = true;
			}
			if (obj is IExtensionInline inline && _inlineErrors.ContainsKey(inline))
			{
				// TODO: implement and design a nice inline error visual
				isError = true;
			}

			if (!isError)
			{
				// Calls before writing an object
				//var writeBefore = ObjectWriteBefore;
				//ObjectWriteBefore?.Invoke(this, obj);

				// Handle regular renderers
				var objectType = obj.GetType();
				IMarkdownObjectRenderer renderer = _previousObjectType == objectType ? _previousRenderer : null;
				if (renderer == null && !_renderersPerType.TryGetValue(objectType, out renderer))
				{
					for (int i = 0; i < ObjectRenderers.Count; i++)
					{
						var testRenderer = ObjectRenderers[i];
						if (testRenderer.Accept(this, obj))
						{
							_renderersPerType[objectType] = renderer = testRenderer;
							break;
						}
					}
				}
				if (renderer != null)
				{
					renderer.Write(this, obj);
				}
				else
				{
					var containerBlock = obj as ContainerBlock;
					if (containerBlock != null)
					{
						WriteChildrenInternal(containerBlock);
					}
					else
					{
						var containerInline = obj as ContainerInline;
						if (containerInline != null)
						{
							WriteChildrenInternal(containerInline);
						}
					}
				}

				_previousObjectType = objectType;
				_previousRenderer = renderer;
			}

			// Calls after writing an object
			//var writeAfter = ObjectWriteAfter;
			//writeAfter?.Invoke(this, obj);
		}
		/// <summary>
		/// Writes the children of the specified <see cref="ContainerBlock"/>.
		/// </summary>
		/// <param name="containerBlock">The container block.</param>
		public void WriteChildrenInternal(ContainerBlock containerBlock)
		{
			if (containerBlock == null)
			{
				return;
			}

			var children = containerBlock;
			for (int i = 0; i < children.Count; i++)
			{
				var saveIsFirstInContainer = IsFirstInContainer;
				var saveIsLastInContainer = IsLastInContainer;

				IsFirstInContainer = i == 0;
				IsLastInContainer = i + 1 == children.Count;
				WriteInternal(children[i]);

				IsFirstInContainer = saveIsFirstInContainer;
				IsLastInContainer = saveIsLastInContainer;
			}
		}

		/// <summary>
		/// Writes the children of the specified <see cref="ContainerInline"/>.
		/// </summary>
		/// <param name="containerInline">The container inline.</param>
		public void WriteChildrenInternal(ContainerInline containerInline)
		{
			if (containerInline == null)
			{
				return;
			}

			bool isFirst = true;
			var inline = containerInline.FirstChild;
			while (inline != null)
			{
				var saveIsFirstInContainer = IsFirstInContainer;
				var saveIsLastInContainer = IsLastInContainer;
				IsFirstInContainer = isFirst;
				IsLastInContainer = inline.NextSibling == null;

				Write(inline);
				inline = inline.NextSibling;

				IsFirstInContainer = saveIsFirstInContainer;
				IsLastInContainer = saveIsLastInContainer;

				isFirst = false;
			}
		}

		public new bool IsFirstInContainer { get; private set; }

		public new bool IsLastInContainer { get; private set; }

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
			if (_blockErrors.Any() || _inlineErrors.Any())
			{
				var errorCss = @"
					.error-list {
						list-style-type:none;
						border: 1px solid red;
						padding: 0.25em;
					}
					li.error:before {
						content: '\274c';
						margin-left: 0.25em;
					}
					.extension-name {
						color: black;
					}
					.error {
						color: red;
					}
					.range {
						font-family: 'consolas';
					}
				";
				sb.AppendLine(errorCss);
			}
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
					foreach (Inline inline in leafBlock.Inline)
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
