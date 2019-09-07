using FM = MarkdownExtensions.Extensions.FolderModel;
using System;
using System.Collections.Generic;

namespace MarkdownExtensions.Extensions.FolderFromDisk
{
	// TODO: Support relative folders
	internal class FolderFromDiskParser : IParser
    {
		private readonly RenderSettings _renderSettings;

		public FolderFromDiskParser(RenderSettings renderSettings)
		{
			_renderSettings = renderSettings;
		}
        public IParseResult Parse(string text)
        {
            var path = text;
            if (System.IO.Directory.Exists(path))
            {
				FM.Folder f = EnumerateFolder(path);
                return new ParseSuccess(f);
            }
			var folder = new Folder(_renderSettings.SourceFolder.Absolute, new RelativeFolder(text));
			if (folder.Exists())
			{
				FM.Folder f = EnumerateFolder(folder.Absolute.FullPath);
				return new ParseSuccess(f);
			}
			else
            {
                return new ParseFailure(new ParseError(new Range(new Position(1, 1), new Position(1, 10)), "folder does not exist"));
            }
            throw new Exception();
        }

        private static FM.Folder EnumerateFolder(string path)
        {
            var files = new List<FM.File>();
            foreach (var file in System.IO.Directory.EnumerateFiles(path))
            {
                files.Add(new FM.File(System.IO.Path.GetFileName(file)));
            }
            var subFolders = new List<FM.Folder>();
            foreach (var subFolderPath in System.IO.Directory.EnumerateDirectories(path))
            {
                var subFolder = EnumerateFolder(subFolderPath);
                subFolders.Add(subFolder);
            }
            var name = System.IO.Path.GetFileName(path);
            return new FM.Folder(name, subFolders, files);
        }
    }
}
