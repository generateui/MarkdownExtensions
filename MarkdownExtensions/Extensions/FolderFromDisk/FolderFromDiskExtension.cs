using Markdig;
using Markdig.Renderers;

// markdig syntax tree node (FencedCodeBlock)
// markdig extension
// extension
// internal code parser
// internal code representation
// internal code validator (using the object)
// formatter/renderer
//	- renderer can add js/css to ExtensionHtmlRenderer
//	- renderer can grab the object
//	- renderer can grab rendering state

/// <summary>
/// Rendering content takes the following steps:
/// 1. Parse markdown into markdown AST
/// 2. Parse extension syntax into an extension AST
/// 3. Validate extension AST
/// 4. In the case when generating markdown is enough, transform markdown AST
/// 5. Generate html using plain Markdig renderer
/// 
/// The following types are needed for above process:
/// 1. A MarkdownExtension extension (implement <see cref="IMarkdownExtension"/>)
/// 2. A Markdig extension (also implement <see cref="IMarkdownExtension"/>, as it 
///		derives from the Markdig extension)
/// 3. Markdig markdown AST node (deriving from <see cref="BlockExtensionParser{T}"/>
/// 4. A MarkdownExtension parser <see cref="MarkdownExtensions.IParser"/>
/// 5. A model or syntax tree the MarkdownExtension parser produces, this can be any class
/// 6. A validator validating the model or syntax tree the MarkdownExtension 
///		produces (optional) <see cref="MarkdownExtensions.IValidator"/>
/// 7. A transformer transforming the Markdig markdown AST (optional)
/// 8. A renderer rendering html using the model or syntax tree <see cref="MarkdownExtensions.IRenderer"/>
/// 
/// Though 7 and 8 are both optional, it is mandatory to have either.
/// </summary>
namespace MarkdownExtensions.Extensions.FolderFromDisk
{
	public sealed class FolderFromDiskExtension : IExtension
	{
		public static ExtensionName NAME => "Folder from disk";
        public FolderFromDiskExtension()
        {
            Parser = new FolderFromDiskParser();
            Renderer = new FolderFromDiskRenderer();
        }

        public ExtensionName Name => NAME;
        public IParser Parser { get; }
        public IRenderer Renderer { get; }
        public IValidator Validator => null;
		public ITransformer Transformer => null;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new BlockParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, new FolderFromDiskRenderer());
	}
}
