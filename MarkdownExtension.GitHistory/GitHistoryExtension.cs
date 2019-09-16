using MarkdownExtensions;
using LibGit2Sharp;
using System.Linq;
using Markdig;
using Markdig.Renderers;

namespace MarkdownExtension.GitHistory
{
	public partial class GitHistoryExtension : IExtension
    {
        public GitHistoryExtension()
        {
            Parser = new GitHistorySyntax();
            Renderer = new GitHistoryFormatter();
        }

        public static ExtensionName NAME => "Git history in a table";
        public ExtensionName Name => NAME;
		public bool IsSummary => false;
		public IParser Parser { get; }
		public IRenderer Renderer { get; }
		public IValidator Validator => null;
		public ITransformer Transformer => null;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new GitHistoryParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
	public static class CommitExtensions
    {
        public static string ToText(this Commit commit)
        {
            var sha = new string(commit.Sha.Take(7).ToArray());
            return $@"🏷 {sha} {commit.Message} {commit.Author}";
        }
    }
}
