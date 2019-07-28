using MarkdownExtensions;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GitGraphExtension.UnitTest")]
namespace MarkdownExtension.GitGraph
{
	internal partial class Syntax : IParser
    {
        public IParseResult Parse(string text)
        {
            var lines = text.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
            var history = new GitGraph();
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

    }
}
