using Markdig;
using Markdig.Renderers;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.RequirementText
{
	public partial class ObjectTextExtension : IExtension
    {
        public ObjectTextExtension(IEaProvider provider)
        {
            Parser = new RequirementTextSyntax();
            Renderer = new RequirementTextFormatter(provider);
        }

        public IParser Parser { get; }
        public IRenderer Renderer { get; }
		public IValidator Validator => null;
		public ITransformer Transformer => null;
		public static ExtensionName NAME => "EA requirement";
        public ExtensionName Name => NAME;

		public void Setup(MarkdownPipelineBuilder pipeline)
		{
			throw new System.NotImplementedException();
		}

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
		{
			throw new System.NotImplementedException();
		}
	}
}
