using Markdig;
using Markdig.Renderers;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.ObjectText
{
	public partial class ObjectTextExtension : IExtension
	{
        public ObjectTextExtension(IEaProvider provider)
        {
            Parser = new ObjectTextSyntax();
			Transformer = new ObjectTextTransformer(provider);
        }

        public IParser Parser { get; }
        public IRenderer Renderer => null;
		public IValidator Validator => null;
		public ITransformer Transformer { get; }
		public static ExtensionName NAME => "EA object text";
        public ExtensionName Name => NAME;

		public void Setup(MarkdownPipelineBuilder pipeline)
		{
			pipeline.BlockParsers.Insert(0, new ObjectTextParser());
		}

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }
	}
}
