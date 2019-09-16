using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System;
using IO = System.IO;

namespace MarkdownExtension.EnterpriseArchitect.Diagram
{
	public class DiagramRenderer : BlockRendererBase<Diagram, DiagramBlock>
	{
		private readonly IEaProvider _provider;
		private readonly RenderSettings _renderSettings;

		public DiagramRenderer(IEaProvider provider, RenderSettings renderSettings)
		{
			_provider = provider;
			_renderSettings = renderSettings;
		}

		public override void Render(ExtensionHtmlRenderer renderer, Diagram diagram, IFormatState formatState)
		{
			if (diagram.Path != null)
			{
				var path = new Path(diagram.Path);
				RenderDiagram(path, renderer);
			}
			if (diagram.PackagePath != null)
			{
				var path = new Path(diagram.PackagePath);
				var paths = _provider.GetDiagramPaths(path);
				foreach(var diagramPath in paths)
				{
					RenderDiagram(diagramPath, renderer);
				}
			}
		}

		private void RenderDiagram(Path diagramPath, ExtensionHtmlRenderer renderer)
		{
			var folder = _renderSettings.ImageFolder;
			File file = _provider.GetDiagramFile(diagramPath, folder);
			if (IO.File.Exists(file.AbsolutePath))
			{
				if (_renderSettings.EmbedImages)
				{
					var bytes = IO.File.ReadAllBytes(file.AbsolutePath);
					var base64 = Convert.ToBase64String(bytes);
					renderer.Write($@"<img src='data:image/png;base64,{base64}' />");
				}
				else
				{
					renderer.Write($@"<img src='{file.RelativePath}' />");
				}
			}
			else
			{
				renderer.Write($@"<p>something went wrong retrieving the image [{diagramPath}]</p>");
			}
		}
	}
}
