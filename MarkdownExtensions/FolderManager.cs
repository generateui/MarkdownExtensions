using System;
using System.Collections.Generic;
using System.IO;

namespace MarkdownExtensions
{
	public class FolderManager
	{
		private readonly RenderSettings _renderSettings;
		private readonly Dictionary<ExtensionName, IFolder> _folderByExtensionName = new Dictionary<ExtensionName, IFolder>();

		public RenderSettings RenderSettings => _renderSettings;

		public FolderManager(RenderSettings renderSettings)
		{
			_renderSettings = renderSettings;
		}

		public IFolder GetExtensionFolder(ExtensionName extensionName)
		{
			if (_folderByExtensionName.ContainsKey(extensionName))
			{
				var relativeFolder = new RelativeFolder(extensionName.Name);
				var folder = new Folder(RenderSettings.OutputFolder.Absolute, relativeFolder);
				_folderByExtensionName[extensionName] = folder;
				if (!Directory.Exists(folder.Absolute.FullPath))
				{
					Directory.CreateDirectory(folder.Absolute.FullPath);
				}
			}
			return _folderByExtensionName[extensionName];
		}

		public void RegisterCssFile(ExtensionName extensionName, ICode code)
		{

		}
		public void RegisterJavascriptFile(ExtensionName extensionName, ICode code)
		{

		}

		public File GetImageFile(string fileName)
		{
			var result = new File(RenderSettings.ImageFolder, fileName);
			return result;
		}

		public void RegisterImageFile(File file)
		{
			//var destination = Path.Combine(RenderSettings.ImageFolder.AbsolutePath, file.Name);
			//System.IO.File.Copy(file.AbsolutePath, file.Name);
		}
	}
}
