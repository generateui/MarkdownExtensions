using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;

namespace MarkdownExtensions
{
	public interface IRenderer : IMarkdownObjectRenderer
	{
		IEnumerable<ICode> Css { get; }
		IEnumerable<ICode> Javascript { get; }
	}
	public interface IRenderer<TModel> : IRenderer
	{
		void Render(ExtensionHtmlRenderer renderer, TModel model, IFormatState formatState);
	}

	public abstract class InlineRendererBase<TModel, TInline> : IMarkdownObjectRenderer, IRenderer<TModel>
		where TModel : class
		where TInline : Inline, IExtensionInline
	{
		public bool Accept(RendererBase renderer, MarkdownObject obj) => obj is TInline;

		public virtual IEnumerable<ICode> Css { get; }
		public virtual IEnumerable<ICode> Javascript { get; }

		public virtual void Render(ExtensionHtmlRenderer renderer, TModel model, IFormatState formatState) { }
		public void Write(RendererBase renderer, MarkdownObject astNode)
		{
			var extensionHtmlRenderer = renderer as ExtensionHtmlRenderer;
			var extensionInline = astNode as IExtensionInline;
			var model = (TModel)extensionHtmlRenderer.GetInlineModel(extensionInline);
			Render(extensionHtmlRenderer, model, extensionHtmlRenderer.FormatState);
		}
	}

	public abstract class BlockRendererBase<TModel, TBlock> : IMarkdownObjectRenderer, IRenderer<TModel>
		where TModel : class
		where TBlock : FencedCodeBlock, IExtensionBlock
	{
		public bool Accept(RendererBase renderer, MarkdownObject obj) => obj is TBlock;

		public virtual IEnumerable<ICode> Css { get; }
		public virtual IEnumerable<ICode> Javascript { get; }

		public abstract void Render(ExtensionHtmlRenderer renderer, TModel model, IFormatState formatState);
		public void Write(RendererBase renderer, MarkdownObject astNode)
		{
			var extensionHtmlRenderer = renderer as ExtensionHtmlRenderer;
			var extensionBlock = astNode as IExtensionBlock;
			var model = (TModel)extensionHtmlRenderer.GetBlockModel(extensionBlock);
			Render(extensionHtmlRenderer, model, extensionHtmlRenderer.FormatState);
		}
	}

	/// <summary>
	/// Represents code used to render content in the generated html document
	/// </summary>
	/// This effectively lets markdown support javascript and css
	public interface ICode
	{
		string LibraryName { get; }
		string LibraryVersion { get; }
		string GetCode();
		// string Uri
	}
	public class Code : ICode
	{
		private readonly Func<string> _getCode;

		public Code(string libraryName, string libraryVersion, Func<string> getCode)
		{
			LibraryName = libraryName;
			LibraryVersion = libraryVersion;
			_getCode = getCode;
		}
		public string LibraryName { get; }
		public string LibraryVersion { get; }

		public override bool Equals(object obj)
		{
			return obj is Code code &&
				   LibraryName == code.LibraryName &&
				   LibraryVersion == code.LibraryVersion;
		}

		public string GetCode() => _getCode();

		public override int GetHashCode()
		{
			var hashCode = 1312102631;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LibraryName);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LibraryVersion);
			return hashCode;
		}
	}
}
