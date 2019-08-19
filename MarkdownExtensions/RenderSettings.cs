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
		public IFolder OutputFolder { get; internal set; }
		public File SettingsFile { get; set; }

		// Folder or file the input markdown comes from
		// TODO: what in case of relative folder of wiki?
		public IFolder SourceFolder { get; set; }

        private readonly Dictionary<ExtensionName, IFolder> _folderByExtension = new Dictionary<ExtensionName, IFolder>();

		public TomlTable Settings { get; private set; }

		public static RenderSettings DefaultWiki()
		{
			return new RenderSettings
			{
				CssFolder = new Folder(new RelativeFolder("css")),
				JavascriptFolder = new Folder(new RelativeFolder("javascript")),
				ImageFolder = new Folder(new RelativeFolder("image")),
                OutputFolder = new Folder(new RelativeFolder("output")),
			};
		}

		public static RenderSettings DefaultFile()
		{
			return new RenderSettings
			{
				CssFolder = new Folder(new RelativeFolder("css")),
				JavascriptFolder = new Folder(new RelativeFolder("javascript")),
				ImageFolder = new Folder(new RelativeFolder("image")),
			};
		}
		public void SetSourceFolder(AbsoluteFolder absoluteFolder)
		{
			SourceFolder = new Folder(absoluteFolder);
            OutputFolder.SetAbsoluteRoot(absoluteFolder);
			CssFolder.SetAbsoluteRoot(OutputFolder.Absolute);
			JavascriptFolder.SetAbsoluteRoot(OutputFolder.Absolute);
			ImageFolder.SetAbsoluteRoot(OutputFolder.Absolute);
			SettingsFile = new File(SourceFolder, "settings.toml");

            CreateIfNeeded(OutputFolder);
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

		public void TryParseExtensionSettings(IEnumerable<IExtensionSettings> extensionSettings)
		{
			foreach (IExtensionSettings extensionSetting in extensionSettings)
			{
				extensionSetting.Parse(this, Settings);
			}
		}

		public void EnsureFoldersExist()
		{
			if (!Directory.Exists(ImageFolder.Absolute.FullPath))
			{
				Directory.CreateDirectory(ImageFolder.Absolute.FullPath);
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