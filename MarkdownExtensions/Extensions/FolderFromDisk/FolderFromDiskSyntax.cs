using MarkdownExtensions.Extensions.Folder;
using System;
using System.Collections.Generic;
using System.IO;

namespace MarkdownExtensions.Extensions.FolderFromDisk
{
    internal class FolderFromDiskSyntax : ISyntax
    {
        internal string Prefix => "folder";
        // TODO: Support relative folders

        public IParseResult Parse(string text)
        {
            var path = text;
            if (Directory.Exists(path))
            {
                Model.Folder folder = EnumerateFolder(path);
                return new ParseSuccess(folder);
            }
            else
            {
                return new ParseFailure(new ParseError(new Range(new Position(1, 1), new Position(1, 10)), "folder does not exist"));
            }
            throw new Exception();
        }

        private static Model.Folder EnumerateFolder(string path)
        {
            var files = new List<Model.File>();
            foreach (var file in Directory.EnumerateFiles(path))
            {
                files.Add(new Model.File(Path.GetFileName(file)));
            }
            var subFolders = new List<Model.Folder>();
            foreach (var subFolderPath in Directory.EnumerateDirectories(path))
            {
                var subFolder = EnumerateFolder(subFolderPath);
                subFolders.Add(subFolder);
            }
            var name = Path.GetFileName(path);
            return new Model.Folder(name, subFolders, files);
        }
    }
}
