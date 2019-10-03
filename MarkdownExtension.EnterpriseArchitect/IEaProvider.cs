using System;
using System.Collections.Generic;
using System.Linq;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;
using Newtonsoft.Json;
using static MarkdownExtension.EnterpriseArchitect.Plugin;
using IO = System.IO;

namespace MarkdownExtension.EnterpriseArchitect.EaProvider
{
    // Can we have <Path, EaObject> supermap?
    public interface IEaProvider
    {
        Package GetElementsByPackage(Path path);
        File GetDiagramFile(Path diagramPath, IFolder folder);
        File GetDiagramFile(Diagram diagram, IFolder folder);
		bool IsValidDiagramPath(Path path, IFolder folder);
		Element GetElementByName(string elementName);
        IEnumerable<Element> GetElements(Path packagePath, Func<Element, bool> filter, string filterName, bool recursive = false);
        IEnumerable<Element> GetElements(Path packagePath, bool recursive = false);
		(Element bpmnWorkflow, IEnumerable<BpmnElement>) GetBpmnElements(Path bpmnElementPath);
		IEnumerable<Path> GetDiagramPaths(Path packagePath);
		bool IsValidPackagePath(Path packagePath, IFolder folder);
	}
    internal static class FileNames
    {
        internal const string ENTERPRISE_ARCHITECT = "EnterpriseArchitect";
        internal static string ELEMENT_LIST = "elementList.json";
        internal static string DIAGRAM_LIST = "diagramList.json";

		internal static File GetElementsByPackage(IFolder folder, Path packagePath) =>
			new File(folder, $@"Elements - {packagePath}.json");

		internal static File GetElementsFiltered(IFolder folder, Path packagePath, string filterName) =>
			new File(folder, $@"Elements ({filterName}) - {packagePath}.json");

		internal static File GetDiagramPathsByPackage(IFolder folder, Path packagePath) =>
            new File(folder, $@"DiagramPaths - {packagePath}.json");

        internal static File GetPackage(IFolder folder, Path path) =>
            new File(folder, $@"Package - {path}.json");

        internal static File GetDiagram(IFolder folder, string diagramName) =>
            new File(folder, $@"{diagramName}.json");
    }
    internal class CacheProvider : IEaProvider
	{
        private readonly EaProvider _eaProvider;
        private readonly JsonProvider _jsonProvider;
        private readonly RenderSettings _renderSettings;

        public CacheProvider(EaProvider eaProvider, JsonProvider jsonProvider, RenderSettings renderSettings)
        {
            _eaProvider = eaProvider;
            _jsonProvider = jsonProvider;
            _renderSettings = renderSettings;
        }

		public (Element bpmnWorkflow, IEnumerable<BpmnElement>) GetBpmnElements(Path bpmnElementPath)
		{
			return _eaProvider.GetBpmnElements(bpmnElementPath);
		}

		public File GetDiagramFile(Path diagramPath, IFolder folder)
		{
            if (!_renderSettings.ForceRefreshData)
            {
				var file = new File(folder, $@"{diagramPath}.png");
				if (IO.File.Exists(file.AbsolutePath))
				{
					return file;
				}
            }
            return _eaProvider.GetDiagramFile(diagramPath, folder);
        }

        public File GetDiagramFile(Diagram diagram, IFolder folder)
        {
            if (!_renderSettings.ForceRefreshData)
            {
                var result = _jsonProvider.GetDiagramFile(diagram, folder);
                if (result != null)
                {
                    return result;
                }
            }
            return _eaProvider.GetDiagramFile(diagram, folder);
        }

		public IEnumerable<Path> GetDiagramPaths(Path packagePath)
		{
			if (!_renderSettings.ForceRefreshData)
			{
				var result = _jsonProvider.GetDiagramPaths(packagePath);
				if (result != null)
				{
					return result;
				}
			}
			return _eaProvider.GetDiagramPaths(packagePath);
		}

		public Element GetElementByName(string elementName)
        {
            if (!_renderSettings.ForceRefreshData)
            {
                var result = _jsonProvider.GetElementByName(elementName);
                if (result != null)
                {
                    return result;
                }
            }
            return _eaProvider.GetElementByName(elementName);
        }

        public IEnumerable<Element> GetElements(Path packagePath, Func<Element, bool> filter, string filterName, bool recursive = false)
        {
            if (!_renderSettings.ForceRefreshData)
            {
                var result = _jsonProvider.GetElements(packagePath, filter, filterName, recursive);
                if (result != null)
                {
                    return result;
                }
            }
            return _eaProvider.GetElements(packagePath, filter, filterName, recursive);
        }

		public IEnumerable<Element> GetElements(Path packagePath, bool recursive = false)
		{
			if (!_renderSettings.ForceRefreshData)
			{
				var result = _jsonProvider.GetElements(packagePath, recursive);
				if (result != null)
				{
					return result;
				}
			}
			return _eaProvider.GetElements(packagePath, recursive);
		}

		public Package GetElementsByPackage(Path path)
        {
            if (!_renderSettings.ForceRefreshData)
            {
                var result = _jsonProvider.GetElementsByPackage(path);
                if (result != null)
                {
                    return result;
                }
            }
            return _eaProvider.GetElementsByPackage(path);
        }

		public bool IsValidDiagramPath(Path path, IFolder folder)
		{
			if (!_renderSettings.ForceRefreshData)
			{
				var file = _jsonProvider.GetDiagramFile(path, folder);
				return file.Exists();
			}
			return _eaProvider.IsValidDiagramPath(path, folder);
		}

		public bool IsValidPackagePath(Path packagePath, IFolder folder)
		{
			if (!_renderSettings.ForceRefreshData)
			{
				return _jsonProvider.IsValidPackagePath(packagePath, folder);
			}
			return _eaProvider.IsValidPackagePath(packagePath, folder);
		}
	}

	internal class JsonProvider : IEaProvider
	{
        private readonly Lazy<JsonSerializer> _jsonSerializer = new Lazy<JsonSerializer>(() =>
        {
            var jsonSerializer = new JsonSerializer();
            jsonSerializer.Converters.Add(new PathConverter());
            return jsonSerializer;
        });
        private readonly IFolder _folder;

        private Dictionary<string, Element> _elementsByName;
        private List<Element> _elements;
		private readonly Dictionary<Path, IEnumerable<Element>> _elementsByPath = new Dictionary<Path, IEnumerable<Element>>();
		private readonly Dictionary<Path, IEnumerable<Path>> _diagramPathsByPackagePath = new Dictionary<Path, IEnumerable<Path>>();
        private readonly RenderSettings _renderSettings;

        public JsonProvider(RenderSettings renderSettings)
        {
            _renderSettings = renderSettings;
             _folder = _renderSettings.GetExtensionFolder(FileNames.ENTERPRISE_ARCHITECT);
        }

        public File GetDiagramFile(Path diagramPath, IFolder folder)
        {
			return new File(folder, $@"{diagramPath}.png");
        }

        public Package GetElementsByPackage(Path path)
        {
            File file = FileNames.GetPackage(_folder, path);
            if (file.Exists())
            {
                Package package;
                try
                {
                    package = _jsonSerializer.Value.DeserializeFromFile<Package>(file.AbsolutePath);
                }
                catch (JsonReaderException jre)
                {
                    return null;
                }
                return package;
            }
            return null;
        }

        public Element GetElementByName(string elementName)
        {
            if (_elementsByName != null && _elementsByName.ContainsKey(elementName))
            {
                return _elementsByName[elementName];
            }
            var file = new File(_folder, FileNames.ELEMENT_LIST);
            if (IO.File.Exists(file.AbsolutePath))
            {
                var elementList = _jsonSerializer.Value.DeserializeFromFile<ElementList>(file.AbsolutePath);
                _elementsByName = elementList.Elements.ToDictionary(x => x.Name, x => x);
                if (_elementsByName != null && _elementsByName.ContainsKey(elementName))
                {
                    return _elementsByName[elementName];
                }
            }
            return null;
        }

        public IEnumerable<Element> GetElements(Path packagePath, Func<Element, bool> filter, string filterName, bool recursive = false)
        {
            if (_elements != null)
            {
                return _elements;
            }
            var file = FileNames.GetElementsFiltered(_folder, packagePath, filterName);
            if (file.Exists())
            {
                var elementList = _jsonSerializer.Value.DeserializeFromFile<ElementList>(file.AbsolutePath);
                return _elements = elementList.Elements;
            }
            return null;
        }

        public File GetDiagramFile(Diagram diagram, IFolder folder)
        {
            var fileName = diagram.Guid.ToString() + ".png";
			var file = new File(folder, fileName);
            if (IO.File.Exists(file.AbsolutePath))
            {
				return file;
            }
            return null;
        }

		public (Element bpmnWorkflow, IEnumerable<BpmnElement>) GetBpmnElements(Path bpmnElementPath)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Element> GetElements(Path packagePath, bool recursive = false)
		{
			if (_elementsByPath.ContainsKey(packagePath))
			{
				return _elementsByPath[packagePath];
			}
            var file = FileNames.GetElementsByPackage(_folder, packagePath);
			if (file.Exists())
			{
				var elementList = _jsonSerializer.Value.DeserializeFromFile<List<Element>>(file.AbsolutePath);
				return _elementsByPath[packagePath] = elementList;
			}
			return null;
		}

		public IEnumerable<Path> GetDiagramPaths(Path packagePath)
		{
			if (_diagramPathsByPackagePath.ContainsKey(packagePath))
			{
				return _diagramPathsByPackagePath[packagePath];
			}
            File file = FileNames.GetDiagramPathsByPackage(_folder, packagePath);
			if (file.Exists())
			{
				var diagramPathList = _jsonSerializer.Value.DeserializeFromFile<PathList>(file.AbsolutePath);
				return _diagramPathsByPackagePath[packagePath] = diagramPathList.Paths.Select(p => new Path(p));
			}
			return null;
		}

		public bool IsValidDiagramPath(Path path, IFolder folder)
		{
			var file = GetDiagramFile(path, folder);
			return file.Exists();
		}

		public bool IsValidPackagePath(Path packagePath, IFolder imageFolder)
		{
			File file = FileNames.GetPackage(_folder, packagePath);
			return file.Exists();
		}
	}
	internal class EaProvider : IEaProvider
	{
        private readonly RepositoryWrapper _repository;
		private readonly Lazy<JsonSerializer> _jsonSerializer = new Lazy<JsonSerializer>(() =>
        {
			var jsonSerializer = new JsonSerializer
			{
				Formatting = Formatting.Indented
			};
			jsonSerializer.Converters.Add(new PathConverter());
            return jsonSerializer;
        });
        private readonly IFolder _folder;

        public EaProvider(RepositoryWrapper repository, RenderSettings renderSettings)
        {
            _repository = repository;
             _folder = renderSettings.GetExtensionFolder(FileNames.ENTERPRISE_ARCHITECT);
        }

        public Package GetElementsByPackage(Path path)
        {
            EA.Package rootPackage = (EA.Package)_repository.Repository.Models.GetAt(0);
            EA.Package eaPackage = rootPackage.GetPackage(path);
            Package package = FromEaPackage(eaPackage, path);
            var file = FileNames.GetPackage(_folder, path);
            _jsonSerializer.Value.SerializeToFile(package, file.AbsolutePath);
            return package;
        }

        public Package FromEaPackage(EA.Package eaPackage, Path parentPath = null)
        {
            parentPath = parentPath ?? eaPackage.ToPath(_repository);
            var package = new Package
            {
                Id = eaPackage.PackageID,
                Name = eaPackage.Name,
                Elements = new List<Element>(),
                Packages = new List<Package>(),
                Diagrams = new List<Diagram>(),
                Path = parentPath,
            };
            foreach (var e in eaPackage.Elements)
            {
                var eaElement = e as EA.Element;
                package.Elements.Add(CreateElement(eaElement));
            }
            foreach (var p in eaPackage.Packages)
            {
                var eaChildPackage = p as EA.Package;
                var childPackage = FromEaPackage(eaChildPackage, package.Path);
                package.Packages.Add(childPackage);
            }
            foreach (var d in eaPackage.Diagrams)
            {
                var eaDiagram = d as EA.Diagram;
                var diagram = CreateDiagram(eaDiagram, package.Path);
                package.Diagrams.Add(diagram);
            }
            return package;
        }

        public File GetDiagramFile(Path diagramPath, IFolder folder)
        {
            EA.Package rootPackage = (EA.Package)_repository.Repository.Models.GetAt(0);
			var packagePath = diagramPath.RemoveLast(); // remove diagram part
			EA.Package package = rootPackage.GetPackage(packagePath);
			var diagramName = diagramPath.Parts.Last();
            bool Filter(EA.Diagram diagram) => diagram.Name.Equals(diagramName);
			EA.Diagram eaDiagram = package.Diagrams.Cast<EA.Diagram>().FirstOrDefault(Filter);
			if (eaDiagram == null)
			{
				throw new ArgumentException($@"Could not find diagram with path [{diagramPath}]");
			}
			var fileName = $@"{diagramPath}.png";
            var project = _repository.Repository.GetProjectInterface();
			var file = new File(folder, fileName);
            project.PutDiagramImageToFile(eaDiagram.DiagramGUID, file.AbsolutePath, 1);
			return file;
        }

        public Element GetElementByName(string elementName)
        {
            var package = (EA.Package)_repository.Repository.Models.GetAt(0);
            var elements = package.GetElements(e => true).Select(CreateElement).ToList();
            var elementsList = new ElementList { Elements = elements };
            _jsonSerializer.Value.SerializeToFile(elementsList, "elementList.json");
            var element = elements.FirstOrDefault(e => Equals(elementName, e.Name));
            return element;
        }

        private static Element CreateElement(EA.Element e) =>
                        new Element
                        {
                            Id = e.ElementID,
                            Name = e.Name,
                            Notes = e.Notes.FixNewlines(),
                            Stereotype = e.Stereotype,
							Type = e.Type,
                            Attributes = e.Attributes
                                .Cast<EA.Attribute>()
                                .Select(CreateAttribute)
                                .ToList(),
							TaggedValues = CreateTaggedValues(e)
                        };

		private static Dictionary<string, string> CreateTaggedValues(EA.Element e)
		{
			var result = new Dictionary<string, string>();
			foreach (var tv in e.TaggedValues.OfType<EA.TaggedValue>())
			{
				var name = tv.Name;
				var value = tv.Value;
				result[name] = value;
			}
			return result;
		}

		private static Dictionary<string, string> CreateTaggedValues(EA.Attribute a)
		{
			var nname = a.Name;
			var result = new Dictionary<string, string>();
			foreach (var tv in a.TaggedValues.OfType<EA.AttributeTag>())
			{
				var name = tv.Name;
				var value = tv.Value;
				result[name] = value;
			}
			return result;
		}

		private static Attribute CreateAttribute(EA.Attribute a)
		{
			var name = a.Name;
			return new Attribute
			{
				Id = a.AttributeID,
				Name = a.Name,
				Type = a.Type,
				Length = string.IsNullOrEmpty(a.Length) ? 0 : int.Parse(a.Length),
				Nullable = !a.AllowDuplicates,
				DefaultValue = a.Default,
				TaggedValues = CreateTaggedValues(a),
				Notes = a.Notes.FixNewlines()
			};
		}

		private static Diagram CreateDiagram(EA.Diagram d, Path parentPath) =>
            new Diagram
            {
                Id = d.DiagramID,
                Name = d.Name,
                Notes = d.Notes.FixNewlines(),
                Path = parentPath.CreateChild(d.Name)
            };

		public IEnumerable<Element> GetElements(Path packagePath, Func<Element, bool> filter, string filterName, bool recursive = false)
        {
			var package = GetPackage(packagePath);
			IEnumerable<Element> elements = null;
			if (recursive)
			{
				var p = FromEaPackage(package);
				elements = p.GetElementsRecursively();
			}
			else
			{
				EA.Package p = GetPackage(packagePath);
				elements = p.Elements
					.Cast<EA.Element>()
					.Select(CreateElement);
			}
			var filtered = elements.Where(filter);
			File file = FileNames.GetElementsFiltered(_folder, packagePath, filterName);
			var elementList = new ElementList { Elements = filtered.ToList() };
			_jsonSerializer.Value.SerializeToFile(elementList, file.AbsolutePath);
            return filtered;
        }

        public File GetDiagramFile(Diagram diagram, IFolder folder)
        {
            var eaDiagram = _repository.Repository.GetDiagramByID(diagram.Id);
            var file = FileNames.GetDiagram(_folder, diagram.Name);
            var project = _repository.Repository.GetProjectInterface();
            project.PutDiagramImageToFile(eaDiagram.DiagramGUID, file.AbsolutePath, 1);
            var diagrams = new DiagramList { Diagrams = new List<Diagram> { diagram } };
            _jsonSerializer.Value.SerializeToFile(diagrams, file.AbsolutePath);
            return file;
        }

		private EA.Package GetPackage(Path path)
		{
			string root = path.Parts.FirstOrDefault();
			EA.Package package = _repository.Repository.Models.Cast<EA.Package>().FirstOrDefault(p => p.Name == root);
			if (package == null)
			{
				return null;
			}
			foreach (string part in path.Parts.Skip(1))
			{
				var childPackage = package.Packages.Cast<EA.Package>().FirstOrDefault(p => p.Name == part);
				if (childPackage == null) // last part is the diagram
				{
					return package;
				}
				package = childPackage;
			}
			return package;
		}

		public (Element bpmnWorkflow, IEnumerable<BpmnElement>) GetBpmnElements(Path bpmnElementPath)
		{
			EA.Package package = GetPackage(bpmnElementPath);
			var elementName = bpmnElementPath.Parts.Last();
			var element = package.Elements.Cast<EA.Element>().FirstOrDefault(e => e.Name == elementName);
			var laneElements = element.Elements;
			var elements = new List<BpmnElement>();
			foreach (var laneElement in laneElements.Cast<EA.Element>())
			{
				var childElements = laneElement.GetBpmnElementsRecursively().ToList();
				childElements.ForEach(ce => ce.Lane = laneElement.Name);
				elements.AddRange(childElements);
			}
			return (CreateElement(element), elements);
		}

		public IEnumerable<Element> GetElements(Path packagePath, bool recursive)
		{
			EA.Package package = GetPackage(packagePath);

			IEnumerable<Element> elements = null;
			if (recursive)
			{
				var p = FromEaPackage(package);
				elements = p.GetElementsRecursively();
			}
			else
			{
				elements = package.Elements
					.Cast<EA.Element>()
					.Select(CreateElement);
			}
			var file = FileNames.GetElementsByPackage(_folder, packagePath);
			_jsonSerializer.Value.SerializeToFile(elements, file.AbsolutePath);
			return elements;
		}

		//private List<Package> GetChildPackages(Path packagePath, EA.Package package, List<Package> packages = null)
		//{
		//	packages = packages ?? new List<Package>();
		//	if (package == null)
		//	{
		//		return packages;
		//	}
		//	foreach (var childPackage in package.Packages.OfType<EA.Package>())
		//	{
		//		var childPath = packagePath.CreateChild(childPackage.Name);
		//		var cp = FromEaPackage(childPackage,)
		//	}
		//	return package;
		//}

		// TODO: support recursive
		public IEnumerable<Path> GetDiagramPaths(Path packagePath)
		{
			EA.Package package = GetPackage(packagePath);
			var paths = new List<Path>();
			foreach (var diagram in package.Diagrams.Cast<EA.Diagram>())
			{
				var path = packagePath.CreateChild(diagram.Name);
				paths.Add(path);
			}
            var file = FileNames.GetDiagramPathsByPackage(_folder, packagePath);
			var pathList = new PathList { Paths = paths.Select(p => p.ToString()).ToList() };
			_jsonSerializer.Value.SerializeToFile(pathList, file.AbsolutePath);
			return paths;
		}

		public bool IsValidDiagramPath(Path diagramPath, IFolder folder)
		{
			EA.Package rootPackage = (EA.Package)_repository.Repository.Models.GetAt(0);
			var packagePath = diagramPath.RemoveLast(); // remove diagram part
			EA.Package package = rootPackage.GetPackage(packagePath);
			var diagramName = diagramPath.Parts.Last();
			bool Filter(EA.Diagram diagram) => diagram.Name.Equals(diagramName);
			EA.Diagram eaDiagram = package.Diagrams.Cast<EA.Diagram>().FirstOrDefault(Filter);
			return eaDiagram == null;
		}

		public bool IsValidPackagePath(Path packagePath, IFolder imageFolder)
		{
			EA.Package package = GetPackage(packagePath);
			return package == null;
		}
	}

	/// <summary>
	/// Represents an object within EA by the path within the object browser
	/// </summary>
	/// <remarks>
	/// - paths are case sensitive - "ARS T&TT.SOSPES Permit" is different from "ARS T&TT.SOSPES PERMIT"
	/// - immutable
	/// </remarks>
	[JsonConverter(typeof(PathConverter))]
    public class Path
    {
        private readonly string _path;
        private readonly List<string> _parts = new List<string>();

        public Path(string path)
        {
            var split = path.Split('.');
            foreach (var part in split)
            {
                _parts.Add(part);
            }
            _path = path;
        }

        public Path(List<string> parts)
        {
            _parts = parts;
            _path = string.Join(".", parts);
        }

        public IEnumerable<string> Parts => _parts;
        public override string ToString() => _path;
        public Path CreateChild(string name) => new Path(new List<string>(_parts) { name });
		public Path RemoveLast()
		{
			var parts = new List<string>(_parts);
			parts.RemoveAt(_parts.Count - 1);
			return new Path(new List<string>(parts));
		}
    }
    public class PathConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Path);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            existingValue == null ? null : new Path(existingValue.ToString());

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => 
            (value as Path).ToString();
    }

	public interface IHasTaggedValues
	{
		Dictionary<string, string> TaggedValues { get; }
	}

	public static class HasTaggedValuesExtensions
	{
		public static bool TaggedValue(this IHasTaggedValues hasTaggedValues, string taggedValueName)
		{
			if (hasTaggedValues.TaggedValues == null)
			{
				return false;
			}
			if (!hasTaggedValues.TaggedValues.ContainsKey(taggedValueName))
			{
				return false;
			}
			if (hasTaggedValues.TaggedValues[taggedValueName].ToLower() != "true")
			{
				return false;
			}
			return true;
		}
	}

	public class Diagram
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("guid")]
        public Guid Guid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("path")]
        [JsonConverter(typeof(PathConverter))]
        public Path Path { get; set; }
    }
    public class ElementList
    {
        [JsonProperty("elements")]
        public List<Element> Elements { get; set; }
    }
    public class DiagramList
    {
        [JsonProperty("diagrams")]
        public List<Diagram> Diagrams { get; set; }
    }
    public class Element : IHasTaggedValues
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")] 
        public string Name { get; internal set; }

        [JsonProperty("stereotype")]
        public string Stereotype { get; internal set; }

		[JsonProperty("notes")]
		public string Notes { get; internal set; }

		[JsonProperty("type")]
		public string Type { get; internal set; }

		[JsonProperty("attributes")]
        public List<Attribute> Attributes { get; internal set; }

		[JsonProperty("alias")]
		public string Alias { get; set; }

		[JsonProperty("taggedValues")]
		public Dictionary<string, string> TaggedValues { get; internal set; }
	}
	public sealed class BpmnElement : Element
	{
		public class AliasComparer : IComparer<BpmnElement>
		{
			public int Compare(BpmnElement left, BpmnElement right)
			{
				if (string.IsNullOrEmpty(left.Alias) && string.IsNullOrEmpty(right.Alias))
				{
					return 0;
				}
				if (string.IsNullOrEmpty(left.Alias) && !string.IsNullOrEmpty(right.Alias))
				{
					return 1;
				}
				if (!string.IsNullOrEmpty(left.Alias) && string.IsNullOrEmpty(right.Alias))
				{
					return -1;
				}
				return string.Compare(left.Alias, right.Alias);
			}
		}

		[JsonProperty("lane")]
		public string Lane { get; set; }
	}
    public sealed class Attribute : IHasTaggedValues
    {
        [JsonProperty("id")]
        public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; internal set; }

		[JsonProperty("type")]
		public string Type { get; internal set; }

		[JsonProperty("notes")]
        public string Notes { get; internal set; }

		[JsonProperty("length")]
		public int Length { get; internal set; }

		[JsonProperty("nullable")]
		public bool Nullable { get; internal set; }

		[JsonProperty("defaultValue")]
		public string DefaultValue { get; internal set; }

		[JsonProperty("taggedValues")]
		public Dictionary<string, string> TaggedValues { get; internal set; }

	}
	public sealed class Lane
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; internal set; }

		[JsonProperty("notes")]
		public string Notes { get; internal set; }

		[JsonProperty("elements")]
		public List<Element> Elements { get; set; }
	}
    public sealed class Package
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("elements")]
        public List<Element> Elements { get; internal set; }

        [JsonProperty("packages")]
        public List<Package> Packages { get; internal set; }

        [JsonProperty("diagrams")]
        public List<Diagram> Diagrams { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("path")]
        [JsonConverter(typeof(PathConverter))]
        public Path Path { get; set; }
    }
	public sealed class PathList
	{
		[JsonProperty("paths")]
		public List<string> Paths { get; set; }
	}
    public static class PackageExtensions
    {
        public static IEnumerable<Element> GetElementsRecursively(this Package package, List<Element> elements = null)
        {
            elements = elements ?? new List<Element>();
            elements.AddRange(package.Elements);
            foreach(var childPackage in package.Packages)
            {
                GetElementsRecursively(childPackage, elements);
            }
            return elements;
        }
    }

	public static class ElementExtensions
	{
		public static IEnumerable<BpmnElement> GetBpmnElementsRecursively(this EA.Element element, List<BpmnElement> elements = null)
		{
			elements = elements ?? new List<BpmnElement>();
			elements.Add(new BpmnElement
			{
				Id = element.ElementID,
				Name = element.Name,
				Notes = element.Notes,
				Type = element.Type,
				Alias = element.Alias
			});
			foreach(var e in element.Elements.Cast<EA.Element>())
			{
				GetBpmnElementsRecursively(e, elements);
			}
			return elements;
		}
	}
}
