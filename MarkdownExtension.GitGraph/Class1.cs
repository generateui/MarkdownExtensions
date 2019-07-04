using MarkdownExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("GitGraphExtension.UnitTest")]
namespace MarkdownExtension.GitGraph
{
    public class GitGraph : IMarkdownExtension
    {
        public GitGraph()
        {
            Syntax = new Syntax();
            Formatter = new Formatter();
        }

        public string Prefix => "git-graph";
        public IElementType Type => ElementType.Block;
        public Output Output => Output.Html;

        public ISyntax Syntax { get; }
        public IFormatter Formatter { get; }
        public IValidator Validator => null;
        public static MarkdownExtensionName NAME => "Git graph using syntax";
        public MarkdownExtensionName Name => NAME;
    }

    internal class Formatter : IFormatter
    {
        public FormatResult Format(object root, IFormatState state)
        {
            // first commit should have a branch name
            var history = (Syntax.History)root;
            var script = new StringBuilder();
            var branches = new HashSet<Syntax.BranchName>();
            string currentBranch = null;
            foreach (var commit in history.Commits)
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
                var author = $@"author: {authorText}";

                string tag = null;
                if (commit.Tag != null)
                {
                    script.AppendLine($@"{currentBranch}.tag('{commit.Tag.Name}')");
                }
                var subject = $@"subject: '{commit.Title}'";
                var commitArgs = $@"{{{subject}, {author}}}";
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
            return FormatResult.FromHtml(sb.ToString());
        }

        public ICodeByName GetCss() => new CodeByName(GitGraph.NAME, @"
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

        public ICodeByName GetJs() => 
            new CodeByName(GitGraph.NAME, File.ReadAllText("gitgraph.umd.min.js"));
    }

    internal class Syntax : ISyntax
    {
        public IParseResult Parse(string text)
        {
            var lines = text.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
            var history = new History();
            foreach (var line in lines)
            {
                var commit = new Commit();
                BranchName branch1 = null;
                BranchName branch2 = null;
                var title = "";
                bool isMerge = false;
                bool isBranch = false;
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    if (c == '[')
                    {
                        var branch = BranchName.Parse(line, ref i);
                        if (branch1 != null)
                        {
                            branch2 = branch;
                        }
                        else
                        {
                            branch1 = branch;
                        }
                        continue;
                    }
                    if (c == '-' && line[i + 1] == '>')
                    {
                        isBranch = true;
                        i += 1;
                        continue;
                    }
                    if (c == '<' && line[i + 1] == '-')
                    {
                        isMerge = true;
                        i += 1;
                        continue;
                    }
                    if (c == '{')
                    {
                        commit.CommitId = CommitId.Parse(line, ref i);
                        continue;
                    }
                    if (c == '#')
                    {
                        commit.Tag = Tag.Parse(line, ref i);
                        continue;
                    }
                    if (c == '@')
                    {
                        commit.Author = Author.Parse(line, ref i);
                        continue;
                    }
                    title += c;
                }
                if (isBranch)
                {
                    commit.BranchName = branch1;
                    commit.Branch = branch2;
                }
                else if (isMerge)
                {
                    commit.BranchName = branch1;
                    commit.Merge = branch2;
                }
                else if (branch1 != null)
                {
                    commit.BranchName = branch1;
                }
                commit.Title = title.Trim();
                history.Commits.Add(commit);
            }
            return new ParseSuccess(history);
        }

        public class History
        {
            public List<Commit> Commits { get; set; } = new List<Commit>();

            public override bool Equals(object obj)
            {
                var other = obj as History;
                if (other == null)
                {
                    return false;
                }
                if (Commits.Count != other.Commits.Count)
                {
                    return false;
                }
                for (int i = 0; i < Commits.Count; i++)
                {
                    if (!Equals(Commits[i], other.Commits[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public class Commit
        {
            public CommitId CommitId { get; set; }
            public BranchName Branch { get; set; }
            public BranchName Merge { get; set; }
            public BranchName BranchName { get; set; }
            public Tag Tag { get; set; }
            public string Title { get; set; }
            public Author Author { get; set; }

            public override bool Equals(object obj)
            {
                var other = obj as Commit;
                if (other == null)
                {
                    return false;
                }
                return Equals(CommitId, other.CommitId) &&
                    Equals(Branch, other.Branch) &&
                    Equals(Merge, other.Merge) &&
                    Equals(BranchName, other.BranchName) &&
                    Equals(Tag, other.Tag) &&
                    Equals(Title, other.Title) &&
                    Equals(Author, other.Author);
            }
        }
        public class BranchName
        {
            public BranchName(string name)
            {
                Name = name;
            }
            public string Name { get; set; }
            public static BranchName Parse(string text, ref int i)
            {
                var name = "";
                i += 1;
                while (i < text.Length)
                {
                    var c = text[i];
                    if (c == ']')
                    {
                        return new BranchName(name);
                    }
                    else
                    {
                        name += c;
                    }
                    i += 1;
                }
                throw new Exception();
            }
            public override bool Equals(object obj)
            {
                var other = obj as BranchName;
                if (other == null)
                {
                    return false;
                }
                return Equals(Name, other.Name);
            }
        }
        public class Tag
        {
            public Tag(string name)
            {
                Name = name;
            }
            public string Name { get; set; }
            internal static Tag Parse(string text, ref int i)
            {
                var name = "";
                i += 1;
                while (i < text.Length)
                {
                    var c = text[i];
                    if (c == '#')
                    {
                        return new Tag(name);
                    }
                    else
                    {
                        name += c;
                    }
                    i += 1;
                }
                throw new Exception();
            }
            public override bool Equals(object obj)
            {
                var other = obj as Tag;
                if (other == null)
                {
                    return false;
                }
                return Equals(Name, other.Name);
            }
        }
        public class Author
        {
            public Author(string name, string email)
            {
                Name = name;
                Email = email;
            }

            public string Name { get; set; }
            public string Email { get; set; }

            internal static Author Parse(string text, ref int i)
            {
                i += 1; // assume first char is double quote
                var name = "";
                string email = "";
                bool inNameState = true;
                bool firstDoubleQuote = true;
                while (i < text.Length)
                {
                    var c = text[i];
                    if (c == '"' && firstDoubleQuote)
                    {
                        firstDoubleQuote = false;
                    }
                    else if (c == '"' && !firstDoubleQuote && text[i + 1] != '<')
                    {
                        return new Author(name, null);
                    }
                    else if (c == '"' && !firstDoubleQuote)
                    {
                        inNameState = false;
                    }
                    else if (c == '<')
                    {
                    }
                    else if (c == '>')
                    {
                        return new Author(name, email);
                    }
                    else
                    {
                        if (inNameState)
                        {
                            name += c;
                        }
                        else
                        {
                            email += c;
                        }
                    }
                    i += 1;
                }
                throw new Exception();
            }
            public override bool Equals(object obj)
            {
                var other = obj as Author;
                if (other == null)
                {
                    return false;
                }
                return Equals(Name, other.Name) && Equals(Email, other.Email);
            }
        }
        public class CommitId
        {
            public CommitId(string id)
            {
                Id = id;
            }
            public string Id { get; set; }
            internal static CommitId Parse(string text, ref int i)
            {
                var id = "";
                i += 1;
                while (i < text.Length)
                {
                    var c = text[i];
                    if (c == '}')
                    {
                        return new CommitId(id);
                    }
                    else if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'))
                    {
                        id += c;
                    }
                    else
                    {
                        throw new Exception();
                    }
                    i += 1;
                }
                throw new Exception();
            }
            public override bool Equals(object obj)
            {
                var other = obj as CommitId;
                if (other == null)
                {
                    return false;
                }
                return Equals(Id, other.Id);
            }
        }
    }
}
