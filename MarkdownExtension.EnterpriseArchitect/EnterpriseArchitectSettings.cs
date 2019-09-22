using MarkdownExtensions;
using Tomlyn.Model;

namespace MarkdownExtension.EnterpriseArchitect
{
	public class EnterpriseArchitectSettings : IExtensionSettings
	{
        private const string ENTERPRISE_ARCHITECT = "EnterpriseArchitect";
        private const string FILE = "file";

        public void Parse(RenderSettings renderSettings, TomlTable toml)
		{
			if (toml == null)
			{
				return;
			}
            bool hasEaSetting = toml.ContainsKey(ENTERPRISE_ARCHITECT);
			if (!hasEaSetting)
			{
				return;
			}
            var ea = toml[ENTERPRISE_ARCHITECT] as TomlTable;
			if (!ea.ContainsKey(FILE))
			{
				return;
			}
            var file = ea[FILE] as string;
			File = new File(renderSettings.SourceFolder, file);
		}

		public File File { get; private set; }
	}
}
