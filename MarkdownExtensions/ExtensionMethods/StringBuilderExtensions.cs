using System.Collections.Generic;
using System.Text;

namespace MarkdownExtensions
{
    public static class StringBuilderExtensions
    {
        public static void AppendCode(this StringBuilder sb, IEnumerable<ICode> codes)
        {
            foreach (ICode entry in codes)
            {
				var code = entry.GetCode();
                sb.AppendLine(code);
            }
        }
    }
}
