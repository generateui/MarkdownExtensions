using Markdig.Syntax;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using MarkdownExtensions.Extensions.Snippet;
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
			var file = new File(_renderSettings.SourceFolder, model.FileName ?? "schema.json");
			//if (file.Exists() && !_renderSettings.ForceRefreshData)
			//{
			//	return;
			//}
			bool IncludeTable(Element e)
			{
				if (e.Stereotype != "table" && e.Type != "Enumeration")
				{
					return false;
				}
				return e.TaggedValue("IsMigrationApi");
			}
            bool IncludeEnum(Element e)
            {
                return e.Type == "Enumeration" && e.TaggedValue("IsMigrationApi");
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
            IEnumerable<Element> enumElements = null;
            if (model.EnumsPackagePath != null)
            {
                var enumPath = new Path(model.EnumsPackagePath);
                enumElements = _eaProvider.GetElements(enumPath, IncludeEnum, "DataModelApiEnums", true);
            }
            else
            {
                enumElements = tables.Where(t => t.Type == "Enumeration");
            }

            foreach (var table in enumElements)
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
				void transform(MarkdownDocument md) { md.IncreaseHeadingLevel(3); }
				var sanitizedNotes = table.Notes.Replace("\\r\\n", "\n");
				var notes = Helper.Converter2(table.Notes, transform, renderer.Pipeline);
				var tableSchema = new JSchema
				{
					Description = notes,
					Title = table.Name,
					Type = JSchemaType.Object,
					Format = "Date-time",
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
					var (schemaType, format) = ToJsonType(attribute);
					var columnSchema = new JSchema
					{
						Type = isEnum ? JSchemaType.Integer | JSchemaType.String : schemaType,
						Description = attribute.Notes,
						Format = format
					};
					if (isEnum)
					{
						string enumName = attribute.Name.Replace("EnumId", string.Empty);
						IList<JToken> @enum = enums[enumName];
						foreach (var enumValue in @enum)
						{
							columnSchema.Enum.Add(enumValue);
						}
						//var enumValueComments = new JSchema
						//{
							
						//};
						//JToken derpy = "";
						//enumValueComments.Properties.Add("enumValueName", (JToken)"");
						//columnSchema.Properties.Add("meta:enum", enumValueComments);
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
			using (System.IO.StreamWriter textWriter = System.IO.File.CreateText(file.AbsolutePath))
			using (var jsonWriter = new JsonTextWriter(textWriter))
			{
				jsonWriter.Formatting = Formatting.Indented;
				schema.WriteTo(jsonWriter);
				jsonWriter.Flush();
			}
		}

		private (JSchemaType SchemaType, string Format) ToJsonType(EaProvider.Attribute attribute)
		{
			switch (attribute.Type)
			{
				case "int": return (JSchemaType.Integer, null);
				case "smallint": return (JSchemaType.Integer, null);
				case "tinyint": return (JSchemaType.Integer, null);
				case "decimal": return (JSchemaType.Number, null);
				case "nvarchar": return (JSchemaType.String, null);
				case "nvarchar(max)": return (JSchemaType.String, null);
				case "varchar": return (JSchemaType.String, null);
				case "datetime": return (JSchemaType.String, "date-time");
				case "date": return (JSchemaType.String, "date");
				case "time": return (JSchemaType.String, "time");
				case "bigint": return (JSchemaType.Integer, null);
				case "bit": return (JSchemaType.Boolean, null);
			}
			throw new NotSupportedException();
		}
	}
}
