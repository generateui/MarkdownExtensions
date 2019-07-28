using MarkdownExtensions;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("GitGraphExtension.UnitTest")]
namespace MarkdownExtension.GitGraph
{
	public class GitGraphRenderer : BlockRendererBase<GitGraph, GitGraphBlock>
	{
		public override void Render(ExtensionHtmlRenderer renderer, GitGraph gitGraph, IFormatState formatState)
		{
			// first commit should have a branch name
			var script = new StringBuilder();
			var branches = new HashSet<BranchName>();
			string currentBranch = null;
			foreach (var commit in gitGraph.Commits)
			{
				if (!branches.Contains(commit.BranchName))
				{
					branches.Add(commit.BranchName);
					var branch = commit.BranchName.Name;
					currentBranch = branch;
					script.AppendLine($"const {branch} = gitGraph.branch('{branch}');");
				}
				if (commit.Branch != null && !branches.Contains(commit.Branch))
				{
					branches.Add(commit.Branch);
					var branch = commit.Branch.Name;
					currentBranch = branch;
					script.AppendLine($"const {branch} = gitGraph.branch('{branch}');");
				}
				string authorText = null;
				if (commit.Author == null)
				{
					authorText = "null";
				}
				else if (commit.Author != null && commit.Author.Email == null)
				{
					authorText = $@"'{commit.Author.Name}'";
				}
				else if (commit.Author != null && commit.Author.Email == null)
				{
					authorText = $@"'{commit.Author.Name} <{commit.Author.Email}>'";
				}
				string author = null;
				if (!string.IsNullOrEmpty(authorText))
				{
					author = $@"author: {authorText}";
				}

				string tag = null;
				if (commit.Tag != null)
				{
					script.AppendLine($@"{currentBranch}.tag('{commit.Tag.Name}')");
				}
				var subject = $@"subject: '{commit.Title}'";
				string commitArgs = null;
				if (author != null)
				{
					commitArgs = $@"{{{subject}, {author}}}";
				}
				else
				{
					commitArgs = $@"{{{subject}}}";
				}
				script.AppendLine($"{commit.BranchName.Name}.commit({commitArgs});");
				if (commit.Merge != null)
				{
					script.AppendLine($"{commit.Merge.Name}.merge('{commit.BranchName.Name}');");
				}
			}
			var sb = new StringBuilder();
			sb.Append($@"
                <div>
                    <div id='derpy'></div>
                    <script type='text/javascript'>
                        const graphContainer = document.getElementById('derpy');
                        const gitGraph = GitgraphJS.createGitgraph(graphContainer);
                        {script.ToString()}
                    </script>
                </div>");
			renderer.Write(sb.ToString());
		}

		public override IEnumerable<ICode> Css
		{
			get
			{
				yield return new Code("git-graph", "0.0.1", () => @"
					.gitgraph-tooltip {
					  position: absolute;
					  margin-top: -15px;
					  margin-left: 25px;
					  padding: 10px;
					  border-radius: 5px;
					  background: #EEE;
					  color: #333;
					  text-align: center;
					  font-size: 14px;
					  line-height: 20px;
					}
					.gitgraph-tooltip:after {
					  position: absolute;
					  top: 10px;
					  left: -18px;
					  width: 0;
					  height: 0;
					  border-width: 10px;
					  border-style: solid;
					  border-color: transparent;
					  border-right-color: #EEE;
					  content: "";
					}
					.gitgraph-detail {
					  position: absolute;
					  padding: 10px;
					  text-align: justify;
					  width: 600px;
					  display: none;
					}
				");
			}
		}

		public override IEnumerable<ICode> Javascript
		{
			get
			{
				yield return new Code("gitgraph.js", "1.3.0", 
					() => Assembly.GetExecutingAssembly().GetFileContent("gitgraph.umd.min.js"));
			}
		}
    }
}
