using MarkdownExtensions.Extensions.Folder;
using System.Text;

namespace MarkdownExtensions.Extensions.FolderFromDisk
{
    internal class FolderFromDiskFormatter : IFormatter
    {
        public FormatResult Format(object obj, IFormatState state)
        {
            var root = (Model.Folder)obj;
            StringBuilder output = new StringBuilder();
            Output(root, output, 0);
            return FormatResult.FromHtml(output.ToString());
        }

        public ICodeByName GetCss() => new CodeByName(FolderFromDisk.NAME, @"
                ul.folder-content {
                    list-style: none;
                }
                ul li.file:before {
                  content: '\1F5CB';
                  margin: 0 0.5em;
                }
                ul li.folder:before {
                  content: '\1F4C1';
                  margin: 0 0.5em;
                }
            ");

        public ICodeByName GetJs() => null;

        private void Output(Model.Folder folder, StringBuilder output, int indent)
        {
            output.Append(@"<ul class='folder-content'>");
            output.Append($@"<li class='folder'>{folder.Name}</li>");
            foreach (var subFolder in folder.Folders)
            {
                Output(subFolder, output, indent + 1);
            }
            foreach (var file in folder.Files)
            {
                output.Append($@"<li class='file'>{file.Name}</li>");
            }
            output.Append("</ul>");
        }
    }
}
