using System;
using System.Collections.Generic;
using System.IO;
using Tomlyn;
using Tomlyn.Model;

namespace MarkdownExtensions
{
    public class RenderSettings
	{
        /// <summary>
        /// Forces extensions to re-query their data sources instead of using cached data
        /// </summary>
        public bool ForceRefreshData { get; set; }

        /// <summary>
        /// True to embed errors in the output html
        /// </summary>
        public bool ReportErrorsInHtml { get; set; }

        /// <summary>
        /// True to embed images using base64 data: encoding
        /// </summary>
        public bool EmbedImages { get; set; }

        public IFolder ImageFolder { get; internal set; }
		public IFolder CssFolder { get; internal set; }
		public IFolder JavascriptFolder { get; internal set; }
		public IFolder MarkdownFolder { get; internal set; }
		public IFolder OutputFolder { get; internal set; }
		public File SettingsFile { get; set; }

		// Folder or file the input markdown comes from
		// TODO: what in case of relative folder of wiki?
		public IFolder SourceFolder { get; set; }

        private readonly Dictionary<ExtensionName, IFolder> _folderByExtension = new Dictionary<ExtensionName, IFolder>();

		public TomlTable Settings { get; private set; }

		public static RenderSettings DefaultWiki(AbsoluteFolder rootFolder)
		{
			var outputFolder = new Folder(rootFolder, new RelativeFolder("output"));
			var sourceFolder = new Folder(rootFolder);
			return new RenderSettings
			{
				SourceFolder = sourceFolder,
				OutputFolder = outputFolder,
				MarkdownFolder = new Folder(outputFolder.Absolute, new RelativeFolder("markdown")),
				CssFolder = new Folder(outputFolder.Absolute, new RelativeFolder("css")),
				JavascriptFolder = new Folder(outputFolder.Absolute, new RelativeFolder("javascript")),
				ImageFolder = new Folder(outputFolder.Absolute, new RelativeFolder("image")),
				SettingsFile = new File(sourceFolder, "settings.toml"),
			};
		}

		public static RenderSettings DefaultFile(AbsoluteFolder rootFolder)
		{
			var outputFolder = new Folder(rootFolder, new RelativeFolder("output"));
			var sourceFolder = new Folder(rootFolder);
			return new RenderSettings
			{
				SourceFolder = sourceFolder,
				OutputFolder = outputFolder,
				MarkdownFolder = new Folder(outputFolder.Absolute, new RelativeFolder("markdown")),
				CssFolder = new Folder(outputFolder.Absolute, new RelativeFolder("css")),
				JavascriptFolder = new Folder(outputFolder.Absolute, new RelativeFolder("javascript")),
				ImageFolder = new Folder(outputFolder.Absolute, new RelativeFolder("image")),
				SettingsFile = new File(sourceFolder, "settings.toml"),
			};
		}

		public void EnsureFoldersExist()
		{
            CreateIfNeeded(OutputFolder);
            CreateIfNeeded(MarkdownFolder);
            CreateIfNeeded(CssFolder);
            CreateIfNeeded(JavascriptFolder);
            CreateIfNeeded(ImageFolder);
        }

        private void CreateIfNeeded(IFolder folder)
        {
            if (!Directory.Exists(folder.Absolute.FullPath))
            {
                Directory.CreateDirectory(folder.Absolute.FullPath);
            }
        }

		public void TryParseSettingsFile()
		{
			if (System.IO.File.Exists(SettingsFile.AbsolutePath))
			{
				string content = System.IO.File.ReadAllText(SettingsFile.AbsolutePath);
                Settings = Tomlyn.Toml.Parse(content).ToModel();
			}
		}

		public static object DefaultFile()
		{
			throw new NotImplementedException();
		}

		public void TryParseExtensionSettings(IEnumerable<IExtensionSettings> extensionSettings)
		{
			foreach (IExtensionSettings extensionSetting in extensionSettings)
			{
				extensionSetting.Parse(this, Settings);
			}
		}

        public IFolder GetExtensionFolder(ExtensionName extensionName)
        {
            if (_folderByExtension.ContainsKey(extensionName))
            {
                return _folderByExtension[extensionName];
            }
            var folder = new Folder(extensionName.Name, OutputFolder);
            if (!Directory.Exists(folder.Absolute.FullPath))
            {
                Directory.CreateDirectory(folder.Absolute.FullPath);
            }
            _folderByExtension[extensionName] = folder;
            return folder;
        }
	}
}