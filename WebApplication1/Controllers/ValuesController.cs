using System.Collections.Generic;
using System.IO;
using System.Text;
using MarkdownExtensions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ExtensionHtmlRenderer _markdownConverter;

        public ValuesController(ExtensionHtmlRenderer markdownConverter)
        {
            _markdownConverter = markdownConverter;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/markdown/{
        [HttpGet("markdown/{prefix}")]
        public ActionResult<string> Get(string prefix, [FromBody] string markdown)
        {
            //var sourceSettings = new SourceSettings();
            var text = $@"```{prefix}:
{markdown}
```";
            //string html = _markdownConverter.Convert(text, sourceSettings: sourceSettings);
            //return html;
            return null;
        }

        // POST api/values/folder
        [HttpPost("{prefix}")]
        public string Post(string prefix)
        {
            string md = null;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                md = reader.ReadToEnd();
            }
            var text = $@"```{prefix}:
{md}
```";
            //var sourceSettings = new SourceSettings();
            //string html = _markdownConverter.Convert(text, sourceSettings: sourceSettings);
            //return html;
            return null;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
