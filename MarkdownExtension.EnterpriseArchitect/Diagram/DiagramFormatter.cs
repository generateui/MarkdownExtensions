using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System;
using System.IO;

namespace MarkdownExtension.EnterpriseArchitect.Diagram
{
	public class DiagramRenderer : BlockRendererBase<Diagram, DiagramBlock>
	{
		private readonly IEaProvider _provider;
		private readonly FormatSettings _formatSettings;

		public DiagramRenderer(IEaProvider provider, FormatSettings formatSettings)
		{
			_provider = provider;
			_formatSettings = formatSettings;
		}

		public override void Render(ExtensionHtmlRenderer renderer, Diagram diagram, IFormatState formatState)
		{
			if (diagram.Path != null)
			{
				var path = new EaProvider.Path(diagram.Path);
				RenderDiagram(path, renderer);
			}
			if (diagram.PackagePath != null)
			{
				var path = new EaProvider.Path(diagram.PackagePath);
				var paths = _provider.GetDiagramPaths(path);
				foreach(var diagramPath in paths)
				{
					RenderDiagram(diagramPath, renderer);
				}
			}
		}

		private void RenderDiagram(EaProvider.Path diagramPath, ExtensionHtmlRenderer renderer)
		{
			FilePath filePath = _provider.GetDiagramFilePath(diagramPath);
			if (File.Exists(filePath.Value))
			{
				if (_formatSettings.EmbedImages)
				{
					var bytes = File.ReadAllBytes(filePath.Value);
					var base64 = Convert.ToBase64String(bytes);
					renderer.Write($@"<img src='data:image/png;base64,{base64}' />");
				}
				else
				{
					var folder = renderer.RenderSettings.RelativeImageFolder;
					if (!string.IsNullOrEmpty(folder))
					{
						folder += @"/";
					}
					var uri = $@"{folder}{filePath.Value}";
					renderer.Write($@"<img src='{uri}' />");
					var absoluteFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), filePath.Value);
					renderer.RegisterImage(filePath.Value, absoluteFilePath);
				}
			}
			else
			{
				renderer.Write($@"<p>something went wrong retrieving the image [{diagramPath}]</p>");
			}
		}
	}
}
