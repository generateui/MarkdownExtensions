using MarkdownExtensions;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace MarkdownExtension.BpmnGraph
{
	public class BpmnGraphRenderer : BlockRendererBase<BpmnGraph, BpmnGraphBlock>
	{
		public override void Render(ExtensionHtmlRenderer renderer, BpmnGraph bpmnGraph, IFormatState formatState)
		{
			var sb = new StringBuilder();
			var bpmnXml = File.ReadAllText(bpmnGraph.FileUri);
			// Loading BPMN xml from filesystem results in a CORS error
			sb.AppendLine($@"
				<div class='box'>
				  <div class='row content' id='canvas' style='height: 20em;'></div>
				</div>
				<script>
					var nav = document.getElementById('nav');
					var bpmnViewer = new BpmnJS({{ container: '#canvas' }});
					bpmnViewer.importXML(`{bpmnXml}`, function(err) {{
						if (err) {{
							return console.error('could not import BPMN 2.0 diagram', err);
						}}
						//var canvas = bpmnViewer.get('canvas');
						//canvas.zoom('fit-height');
					}});
				</script>");
			renderer.Write(sb.ToString());
		}

		public override IEnumerable<ICode> Css
		{
			get
			{
				yield return new Code("bpmn-graph", "0.0.1", () =>
					Assembly.GetExecutingAssembly().GetFileContent("bpmn-graph.css"));
			}
		}

		public override IEnumerable<ICode> Javascript
		{
			get
			{
				yield return new Code("bpmn-js", "4.0.3", () =>
					Assembly.GetExecutingAssembly().GetFileContent("bpmn-viewer.development.js"));
			}
		}
	}
}
