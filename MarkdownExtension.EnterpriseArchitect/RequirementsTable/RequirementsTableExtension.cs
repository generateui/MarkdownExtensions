using Markdig;
using Markdig.Renderers;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.RequirementsTable
{
	public partial class RequirementsTableExtension : IExtension
    {
        public RequirementsTableExtension(IEaProvider provider)
        {
            Parser = new RequirementsTableSyntax();
            Renderer = new RequirementsTableRenderer(provider);
        }

        public IParser Parser { get; }
        public IRenderer Renderer { get; }
		public IValidator Validator => null;
		public ITransformer Transformer => null;
		public static ExtensionName NAME => "EA requirements in a table";
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
