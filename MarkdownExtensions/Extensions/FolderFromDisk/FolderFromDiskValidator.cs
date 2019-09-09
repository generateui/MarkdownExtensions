using System;
using FM = MarkdownExtensions.Extensions.FolderModel;

namespace MarkdownExtensions.Extensions.FolderFromDisk
{
	internal class FolderFromDiskValidator : ValidatorBase<FM.Folder>
	{
		public override IErrors ValidateTyped(FM.Folder tree, RenderSettings renderSettings)
		{
			throw new Exception();
		}
	}
}
