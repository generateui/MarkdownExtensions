using Tomlyn.Model;
using Tomlyn.Syntax;

namespace MarkdownExtensions
{
	public interface IExtensionSettings
	{
		void Parse(RenderSettings renderSettings, TomlTable toml);
	}
}
