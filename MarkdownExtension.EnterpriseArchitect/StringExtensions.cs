using System;
using System.Collections.Generic;
using System.Text;

namespace MarkdownExtension.EnterpriseArchitect
{
    public static class StringExtensions
    {
        public static string FixNewlines(this string value) => value.Replace("\\r\\n", Environment.NewLine);
    }
}
