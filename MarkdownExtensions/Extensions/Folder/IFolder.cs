﻿using System.Collections.Generic;

namespace MarkdownExtensions.Extensions.Folder
{
    public class Model
    {
        internal class File
        {
            internal File(string name)
            {
                Name = name;
            }
            public string Name { get; private set; }
        }
        internal class Folder
        {
            internal Folder(string path)
            {
                Name = path;
                Folders = new List<Folder>();
                Files = new List<File>();
            }
            internal Folder(string path, List<Folder> folders, List<File> files)
            {
                Name = path;
                Folders = folders;
                Files = files;
            }

            public string Name { get; }
            public List<Folder> Folders { get; }
            public List<File> Files { get; }
        }
    }
}
