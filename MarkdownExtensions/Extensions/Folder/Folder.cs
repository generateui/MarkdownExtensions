using System;

namespace MarkdownExtensions.Extensions.Folder
{
    public class Folder : IMarkdownExtension
    {
        private class SyntaxImpl : ISyntax
        {
            public IParseResult Parse(string text)
            {
                var lines = text.Split(
                    new[] { Environment.NewLine, "\n" },
                    StringSplitOptions.RemoveEmptyEntries);
                var root = new Model.Folder("root");
                var folders = new Model.Folder[10];
                folders[0] = root;
                int indent = 0;
                foreach (var line in lines)
                {
                    int spaces = 0;
                    int lineIndent = 0;
                    int position = 0;
                    for (int i = 0; i< line.Length; i++)
                    {
                        var c = line[i];
                        if (c == ' ')
                        {
                            spaces += 1;
                            position += 1;
                            if (spaces % 4 == 0)
                            {
                                lineIndent += 1;
                            }
                        }
                        else if (c == '\t')
                        {
                            position += 1;
                            lineIndent += 1;
                        }
                        else if (c == '-')
                        {
                            if (line[i + 2] == '[')
                            {
                                var start = position + 3;
                                var length = line.Length - position - 4;
                                var name = line.Substring(start, length);
                                var folder = new Model.Folder(name);
                                folders[lineIndent].Folders.Add(folder);
                                folders[lineIndent + 1] = folder;
                                indent = lineIndent;
                                goto endLineLoop;
                            }
                            else
                            {
                                var name = line.Substring(position + 2, line.Length - position - 2);
                                var file = new Model.File(name);
                                folders[lineIndent].Files.Add(file);
                                goto endLineLoop;
                            }
                        }
                    }
                    endLineLoop:;
                }
                return new ParseSuccess(root);
            }
        }

        public Folder()
        {
            Syntax = new SyntaxImpl();
            Formatter = new FolderFromDisk.FolderFromDiskFormatter();
        }

        public string Prefix => "folder";
        public IElementType Type => ElementType.Block;
        public Output Output => Output.Html;

        public ISyntax Syntax { get; }
        public IFormatter Formatter { get; }
        public IValidator Validator => null;

        public static MarkdownExtensionName NAME => "Folder from Markdown list";
        public MarkdownExtensionName Name => NAME;
    }
}
