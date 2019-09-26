using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownExtension.EnterpriseArchitect.DatamodelApi
{
	internal class DatamodelApiRenderer : BlockRendererBase<DatamodelApi, DatamodelApiBlock>
	{
		private readonly IEaProvider _eaProvider;
		private readonly RenderSettings _renderSettings;

		public DatamodelApiRenderer(IEaProvider eaProvider, RenderSettings renderSettings)
		{
			_eaProvider = eaProvider;
			_renderSettings = renderSettings;
		}

		public override void Render(ExtensionHtmlRenderer renderer, DatamodelApi model, IFormatState formatState)
		{
			bool IncludeTable(Element e)
			{
				if (e.Stereotype != "table" && e.Type != "Enumeration")
				{
					return false;
				}
				return e.TaggedValue("IsMigrationApi");
			}
			var path = new Path(model.PackagePath);
			var tables = _eaProvider
				.GetElements(path, IncludeTable, "DataModelApiTables", true);

			var schema = new JSchema
			{
				SchemaVersion = new Uri("http://json-schema.org/draft-07/schema#"),
				Type = JSchemaType.Object,
				Title = "SOSPES Permit datamodel migration API",
			};
			var enums = new Dictionary<string, IList<JToken>>();
			var requiredTables = new List<string>();
			foreach (var table in tables.Where(t => t.Type == "Enumeration"))
			{
				var values = new List<JToken>();
				foreach (var enumValue in table.Attributes)
				{
					values.Add(enumValue.Name);
				}
				foreach (var enumValue in table.Attributes)
				{
					values.Add(enumValue.DefaultValue);
				}
				enums.Add(table.Name, values);
			}
			foreach (var table in tables.Where(t => t.Type != "Enumeration"))
			{
				var tableArraySchema = new JSchema
				{
					Type = JSchemaType.Array
				};
				var tableSchema = new JSchema
				{
					Description = table.Notes,
					Title = table.Name,
					Type = JSchemaType.Object,
				};
				tableArraySchema.Items.Add(tableSchema);
				var required = new List<string>();
				// 
				foreach (var attribute in table.Attributes)
				{
					if (!attribute.TaggedValue("IsMigrationApi"))
					{
						continue;
					}

					bool isEnum = attribute.Name.EndsWith("EnumId");
					var columnSchema = new JSchema
					{
						Type = isEnum ? JSchemaType.Integer | JSchemaType.String : ToJsonType(attribute),
						Description = attribute.Notes,
					};
					if (isEnum)
					{
						var enumName = attribute.Name.Replace("EnumId", string.Empty);
						var @enum = enums[enumName];
						foreach (var enumValue in @enum)
						{
							columnSchema.Enum.Add(enumValue);
						}
					}
					if (attribute.Length != 0)
					{
						columnSchema.MaximumLength = attribute.Length;
					}
					if (!attribute.Nullable)
					{
						required.Add(attribute.Name);
					}
					tableSchema.Properties.Add(attribute.Name, columnSchema);
				}
				if (table.TaggedValue("IncludeCreatedDateTimeInMigrationApi"))
				{
					var createdDateTime = new JSchema
					{
						Type = JSchemaType.String,
						Description = "Creation DateTime"
					};
					tableSchema.Properties.Add("CreatedDateTime", createdDateTime);
				}
				required.ForEach(r => tableSchema.Required.Add(r));
				schema.Properties.Add(table.Name + "List", tableArraySchema);
				requiredTables.Add(table.Name);
			}
			requiredTables.ForEach(rt => schema.Required.Add(rt));
			renderer.Write(schema.ToString());
			var folder = _renderSettings.GetExtensionFolder(FileNames.ENTERPRISE_ARCHITECT);
			var file = System.IO.Path.Combine(folder.Absolute.FullPath, "schema.json");
			using (var textWriter = System.IO.File.CreateText(file))
			using (var jsonWriter = new JsonTextWriter(textWriter))
			{
				jsonWriter.Formatting = Formatting.Indented;
				schema.WriteTo(jsonWriter);
				jsonWriter.Flush();
			}
		}

		private JSchemaType ToJsonType(EaProvider.Attribute attribute)
		{
			switch (attribute.Type)
			{
				case "int": return JSchemaType.Integer;
				case "smallint": return JSchemaType.Integer;
				case "tinyint": return JSchemaType.Integer;
				case "decimal": return JSchemaType.Number;
				case "nvarchar": return JSchemaType.String;
				case "nvarchar(max)": return JSchemaType.String;
				case "varchar": return JSchemaType.String;
				case "datetime": return JSchemaType.String;
				case "date": return JSchemaType.String;
				case "bigint": return JSchemaType.Integer;
				case "bit": return JSchemaType.Boolean;
			}
			throw new NotSupportedException();
		}
	}
}
