using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using System;
using System.IO;

namespace MarkdownExtension.EnterpriseArchitect.Diagram
{
	public class DiagramRenderer : BlockRendererBase<Diagram, DiagramBlock>
	{
		private readonly IEaProvider _provider;

		public DiagramRenderer(IEaProvider provider)
		{
			_provider = provider;
		}

		public override void Render(ExtensionHtmlRenderer renderer, Diagram diagram, IFormatState formatState)
		{
			FilePath filePath = _provider.GetDiagramFilePath(diagram.Name);
			if (File.Exists(filePath.Value))
			{
				var bytes = File.ReadAllBytes(filePath.Value);
				var base64 = Convert.ToBase64String(bytes);
				renderer.Write($@"<img src='data:image/png;base64,{base64}' />");
			}
			renderer.Write($@"<p>something went wrong retrieving the image {diagram.Name}</p>");
		}
	}
}
