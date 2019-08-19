using Markdig.Syntax;
using FM = MarkdownExtensions.Extensions.FolderModel;
using System;
using System.Collections.Generic;

namespace MarkdownExtensions.Extensions.FolderFromDisk
{
    internal abstract class FolderRenderer<TFolderBlock> : BlockRendererBase<FM.Folder, TFolderBlock>
		where TFolderBlock : FencedCodeBlock, IExtensionBlock
	{
		public override void Render(ExtensionHtmlRenderer renderer, FM.Folder model, IFormatState formatState)
		{
			void write(string value) => renderer.Write(value);
			Output(model, 1, write);
		}

		private void Output(FM.Folder folder, int indent, Action<string> write)
        {
			write(@"<ul class='folder-content'>");
			write($@"<li class='folder'>{folder.Name}</li>");
            foreach (FM.Folder subFolder in folder.Folders)
            {
                Output(subFolder, indent + 1, write);
            }
            foreach (FM.File file in folder.Files)
            {
				write($@"<li class='file'>{file.Name}</li>");
            }
			write("</ul>");
        }

		public override IEnumerable<ICode> Css
		{
			get
			{
				yield return new Code("folder-from-disk", "0.0.1", () => @"
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
			}
		}
	}
}
