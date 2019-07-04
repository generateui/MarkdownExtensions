using System;

namespace MarkdownExtensions
{
    public static class StringExtensions
    {
        public static string[] ToLines(this string text)
        {
            return text.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
        }
    }
}
