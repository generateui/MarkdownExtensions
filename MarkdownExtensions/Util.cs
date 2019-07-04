using CommonMark;
using System.IO;

namespace MarkdownExtensions
{
    public class Util
    {
        public void WriteDebug(string text)
        {
            // Act
            var settings = CommonMarkSettings.Default.Clone();
            settings.OutputFormat = OutputFormat.SyntaxTree;
            settings.AdditionalFeatures = CommonMarkAdditionalFeatures.PlaceholderBracket;
            using (var reader = new StringReader(text))
            using (var writer = new StringWriter())
            {
                var document = CommonMarkConverter.ProcessStage1(reader, settings);
                CommonMarkConverter.ProcessStage2(document, settings);
                CommonMarkConverter.ProcessStage3(document, writer, settings);
                System.Diagnostics.Debug.WriteLine(writer.ToString());
            }
        }
    }
}
