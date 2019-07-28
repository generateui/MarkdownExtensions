using MarkdownExtensions.Extensions.FolderModel;
using System;
using System.Collections.Generic;

namespace MarkdownExtensions.Extensions.FolderFromDisk
{
	// TODO: Support relative folders
	internal class FolderFromDiskParser : IParser
    {
        public IParseResult Parse(string text)
        {
            var path = text;
            if (System.IO.Directory.Exists(path))
            {
                Folder folder = EnumerateFolder(path);
                return new ParseSuccess(folder);
            }
            else
            {
                return new ParseFailure(new ParseError(new Range(new Position(1, 1), new Position(1, 10)), "folder does not exist"));
            }
            throw new Exception();
        }

        private static Folder EnumerateFolder(string path)
        {
            var files = new List<File>();
            foreach (var file in System.IO.Directory.EnumerateFiles(path))
            {
                files.Add(new File(System.IO.Path.GetFileName(file)));
            }
            var subFolders = new List<Folder>();
            foreach (var subFolderPath in System.IO.Directory.EnumerateDirectories(path))
            {
                var subFolder = EnumerateFolder(subFolderPath);
                subFolders.Add(subFolder);
            }
            var name = System.IO.Path.GetFileName(path);
            return new Folder(name, subFolders, files);
        }
    }
}
