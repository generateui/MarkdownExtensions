namespace MarkdownExtensions.Extensions.FolderFromDisk
{
    public class FolderFromDisk : IMarkdownExtension
    {
        public static MarkdownExtensionName NAME => "Folder from disk";
        public FolderFromDisk()
        {
            Syntax = new FolderFromDiskSyntax();
            Formatter = new FolderFromDiskFormatter();
        }

        public MarkdownExtensionName Name => NAME;
        public string Prefix => "folder-from-disk";
        public IElementType Type => ElementType.Inline;
        public Output Output => Output.Html;

        public ISyntax Syntax { get;}
        public IFormatter Formatter { get; }
        public IValidator Validator => null;
    }
}
