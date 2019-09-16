using JetBrains.Annotations;
using System;
using System.IO;

namespace MarkdownExtensions
{
	/// <summary>
	/// Represent a folder on disk with relative & absolute path support
	/// </summary>
	/// This enables passing a reference to a folder once and have the caller 
	/// decide to use relative or absolute paths
	public interface IFolder
	{
		/// <summary>
		/// Full path to folder on disk
		/// </summary>
		[CanBeNull] AbsoluteFolder Absolute { get; }

        [CanBeNull] RelativeFolder Relative { get; }
	}

	public class Folder : IFolder
	{
		public Folder(AbsoluteFolder absolute, RelativeFolder relative)
		{
			Absolute = new AbsoluteFolder(Path.Combine(absolute.FullPath, relative.Name));
			Relative = relative;
		}
		public Folder(AbsoluteFolder absoluteFolder)
		{
			Relative = null;
			Absolute = absoluteFolder;
		}
        public Folder(string name, IFolder parent)
        {
            Relative = new RelativeFolder(name);
            if (parent.Absolute != null)
            {
                Absolute = new AbsoluteFolder(Path.Combine(parent.Absolute.FullPath, name));
            }
        }

		public string Name => Relative.Name;
		public bool Exists() => Directory.Exists(Absolute.FullPath);

		public AbsoluteFolder Absolute { get; set; }
		public RelativeFolder Relative { get; }
	}

	public class AbsoluteFolder
	{
		public AbsoluteFolder(string fullPath)
		{
			FullPath = fullPath;
		}
		public string FullPath { get; }
	}
    public class RelativeFolder
    {
        private AbsoluteFolder _absolute;

        public RelativeFolder(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public RelativeFolder Parent { get; }

        public AbsoluteFolder Absolute
        {
            get
            {
                if (_absolute != null)
                {
                    return _absolute;
                }
                if (Parent != null)
                {
                    _absolute = new AbsoluteFolder(Path.Combine(Parent.Absolute.FullPath, Name));
                    return _absolute;
                }
                return null;
            }
            set => _absolute = value;
        }
    }

    public class File
	{
		private string _absolutePath;

		public File(IFolder folder, string name)
		{
			Folder = folder;
			Name = name;
		}

		public IFolder Folder { get; }
		public string Name { get; }
        public bool Exists() => System.IO.File.Exists(AbsolutePath);

		public string AbsolutePath
		{
			get
			{
				if (_absolutePath == null)
				{
					_absolutePath = Path.Combine(Folder.Absolute.FullPath, Name);
				}
				return _absolutePath;
			}
		}
		public string RelativePath
		{
			get
			{
				if (Folder == null || Folder.Relative == null)
				{
					return Name;
				}
				if (Folder.Relative != null)
				{
					return Path.Combine(Folder.Relative.Name, Name);
				}
				throw new ArgumentException("File has no folder");
			}
		}
	}
}
