using System.Text;

namespace MarkdownExtensions.Console
{
    public static class StringBuilderExtensions
    {
        public static void AppendCode(this StringBuilder sb, ICodeByName codeByName)
        {
            foreach (var entry in codeByName)
            {
                sb.AppendLine(entry.Value);
            }
        }
    }
}
