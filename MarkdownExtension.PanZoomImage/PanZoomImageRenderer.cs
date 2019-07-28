using MarkdownExtensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MarkdownExtension.PanZoomImage
{
	public class PanZoomImageRenderer : BlockRendererBase<PanZoomImage, PanZoomImageBlock>
	{
        private int _id;

		public override void Render(ExtensionHtmlRenderer renderer, PanZoomImage panZoomImage, IFormatState formatState)
		{
			var bytes = System.IO.File.ReadAllBytes(panZoomImage.Uri);
			var base64 = Convert.ToBase64String(bytes);
			string html = $@"
				<div>
					<img src='data:image/png;base64,{base64}' id='{_id}'>
				</div>
                <script>
					var panzoomEl{_id} = document.getElementById('{_id}');
					var panzoom{_id} = panzoom(panzoomEl{_id}, {{ bounds: true }});
				</script>";
			_id++;
			renderer.Write(html);
		}

        public override IEnumerable<ICode> Javascript
		{
			get
			{
				yield return new Code(
					"panzoom", "8.0.0",
					() => Assembly.GetExecutingAssembly().GetFileContent("panzoom.min.js"));
			}
		}
    }
}
