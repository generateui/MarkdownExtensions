using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownExtensions;
using Newtonsoft.Json;
using static MarkdownExtension.EnterpriseArchitect.Plugin;

namespace MarkdownExtension.EnterpriseArchitect.EaProvider
{
    // Can we have <Path, EaObject> supermap?
    public interface IEaProvider
    {
        Package GetElementsByPackage(Path path);
        FilePath GetDiagramFilePath(Path diagramPath);
        FilePath GetDiagramFilePath(Diagram diagram);
        Element GetElementByName(string elementName);
        IEnumerable<Element> GetElements(Func<Element, bool> filter);
        IEnumerable<Element> GetElements(Path packagePath, bool recursive = false);
		(Element bpmnWorkflow, IEnumerable<BpmnElement>) GetBpmnElements(Path bpmnElementPath);
		IEnumerable<Path> GetDiagramPaths(Path packagePath);
	}
    public class FilePath
    {
        public FilePath(string value)
        {
            Value = value;
        }
        public string Value { get; }
    }
	internal class CacheProvider : IEaProvider
	{
        private readonly EaProvider _eaProvider;
        private readonly JsonProvider _jsonProvider;
        private readonly FormatSettings _formatSettings;

        public CacheProvider(EaProvider eaProvider, JsonProvider jsonProvider, FormatSettings formatSettings)
        {
            _eaProvider = eaProvider;
            _jsonProvider = jsonProvider;
            _formatSettings = formatSettings;
        }

		public (Element bpmnWorkflow, IEnumerable<BpmnElement>) GetBpmnElements(Path bpmnElementPath)
		{
			return _eaProvider.GetBpmnElements(bpmnElementPath);
		}

		public FilePath GetDiagramFilePath(Path diagramPath)
        {
            if (!_formatSettings.ForceRefreshData)
            {
				var filePath = new FilePath($@"{diagramPath}.png");
				if (File.Exists(filePath.Value))
				{
					return filePath;
				}
            }
            return _eaProvider.GetDiagramFilePath(diagramPath);
        }

        public FilePath GetDiagramFilePath(Diagram diagram)
        {
            if (!_formatSettings.ForceRefreshData)
            {
                var result = _jsonProvider.GetDiagramFilePath(diagram);
                if (result != null)
                {
                    return result;
                }
            }
            return _eaProvider.GetDiagramFilePath(diagram);
        }

		public IEnumerable<Path> GetDiagramPaths(Path packagePath)
		{
			if (!_formatSettings.ForceRefreshData)
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
            if (!_formatSettings.ForceRefreshData)
            {
                var result = _jsonProvider.GetElementByName(elementName);
                if (result != null)
                {
                    return result;
                }
            }
            return _eaProvider.GetElementByName(elementName);
        }

        public IEnumerable<Element> GetElements(Func<Element, bool> filter)
        {
            if (!_formatSettings.ForceRefreshData)
            {
                var result = _jsonProvider.GetElements(filter);
                if (result != null)
                {
                    return result;
                }
            }
            return _eaProvider.GetElements(filter);
        }

		public IEnumerable<Element> GetElements(Path packagePath, bool recursive = false)
		{
			if (!_formatSettings.ForceRefreshData)
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
            if (!_formatSettings.ForceRefreshData)
            {
                var result = _jsonProvider.GetElementsByPackage(path);
                if (result != null)
                {
                    return result;
                }
            }
            return _eaProvider.GetElementsByPackage(path);
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
        private Dictionary<Path, Diagram> _diagramsByName;
        private Dictionary<string, Element> _elementsByName;
        private List<Element> _elements;
		private readonly Dictionary<Path, IEnumerable<Element>> _elementsByPath = new Dictionary<Path, IEnumerable<Element>>();
		private readonly Dictionary<Path, IEnumerable<Path>> _diagramPathsByPackagePath = new Dictionary<Path, IEnumerable<Path>>();

        public FilePath GetDiagramFilePath(Path diagramPath)
        {
			return new FilePath($@"{diagramPath}.png");
        }

        private FilePath GetDiagramFilePathFromCache(Path diagramPath)
        {
            if (_diagramsByName != null && _diagramsByName.ContainsKey(diagramPath))
            {
                var diagram = _diagramsByName[diagramPath];
                var filePath = System.IO.Path.GetTempPath() + diagram.Guid.ToString() + ".png";
                return new FilePath(filePath);
            }
            return null;
        }

        public Package GetElementsByPackage(Path path)
        {
            if (File.Exists($@"GetElementsByPackage-{path}.json"))
            {
                Package package;
                try
                {
                    package = _jsonSerializer.Value.DeserializeFromFile<Package>($@"GetElementsByPackage-{path}.json");
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
            if (File.Exists("elementList.json"))
            {
                var elementList = _jsonSerializer.Value.DeserializeFromFile<ElementList>("elementList.json");
                _elementsByName = elementList.Elements.ToDictionary(x => x.Name, x => x);
                if (_elementsByName != null && _elementsByName.ContainsKey(elementName))
                {
                    return _elementsByName[elementName];
                }
            }
            return null;
        }

        public IEnumerable<Element> GetElements(Func<Element, bool> filter)
        {
            if (_elements != null)
            {
                return _elements;
            }
            if (File.Exists("elementList.json"))
            {
                var elementList = _jsonSerializer.Value.DeserializeFromFile<ElementList>("elementList.json");
                return _elements = elementList.Elements;
            }
            return null;
        }

        public FilePath GetDiagramFilePath(Diagram diagram)
        {
            var filePath = System.IO.Path.GetTempPath() + diagram.Guid.ToString() + ".png";
            if (File.Exists(filePath))
            {
                return new FilePath(filePath);
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
			var fileName = $@"Elements - {packagePath}.json";
			if (File.Exists(fileName))
			{
				var elementList = _jsonSerializer.Value.DeserializeFromFile<List<Element>>(fileName);
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
			var fileName = $@"DiagramPaths - {packagePath}.json";
			if (File.Exists(fileName))
			{
				var diagramPathList = _jsonSerializer.Value.DeserializeFromFile<PathList>(fileName);
				return _diagramPathsByPackagePath[packagePath] = diagramPathList.Paths.Select(p => new Path(p));
			}
			return null;
		}
	}
	internal class EaProvider : IEaProvider
	{
        private RepositoryWrapper _repository;
        private List<Element> _elements;
        private readonly Lazy<JsonSerializer> _jsonSerializer = new Lazy<JsonSerializer>(() =>
        {
            var jsonSerializer = new JsonSerializer();
            jsonSerializer.Converters.Add(new PathConverter());
            return jsonSerializer;
        });

        public EaProvider(RepositoryWrapper repository)
        {
            _repository = repository;
        }

        public Package GetElementsByPackage(Path path)
        {
            EA.Package rootPackage = (EA.Package)_repository.Repository.Models.GetAt(0);
            EA.Package eaPackage = rootPackage.GetPackage(path);
            Package package = FromEaPackage(eaPackage, path);
            _jsonSerializer.Value.SerializeToFile(package, $@"GetElementsByPackage-{path}.json");
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

        public FilePath GetDiagramFilePath(Path diagramPath)
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
			var filePath = $@"{diagramPath}.png";
            var project = _repository.Repository.GetProjectInterface();
			var fullFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), filePath);
            project.PutDiagramImageToFile(eaDiagram.DiagramGUID, fullFilePath, 1);
            return new FilePath(filePath);
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
                            Attributes = e.Attributes
                                .Cast<EA.Attribute>()
                                .Select(CreateAttribute)
                                .ToList()
                        };

        private static Attribute CreateAttribute(EA.Attribute a) =>
                        new Attribute
                        {
                            Id = a.AttributeID,
                            Name = a.Name,
                            Notes = a.Notes.FixNewlines()
                        };
        private static Diagram CreateDiagram(EA.Diagram d, Path parentPath) =>
            new Diagram
            {
                Id = d.DiagramID,
                Name = d.Name,
                Notes = d.Notes.FixNewlines(),
                Path = parentPath.CreateChild(d.Name)
            };

        private IEnumerable<Element> GetElements()
        {
            if (_elements != null)
            {
                return _elements;
            }
            var package = (EA.Package)_repository.Repository.Models.GetAt(0);
            Attribute CreateAttribute(EA.Attribute a) =>
                new Attribute
                {
                    Id = a.AttributeID,
                    Name = a.Name,
                    Notes = a.Notes.FixNewlines()
                };
            Element CreateElement(EA.Element e) =>
                new Element
                {
                    Id = e.ElementID,
                    Name = e.Name,
                    Notes = e.Notes.FixNewlines(),
                    Stereotype = e.Stereotype,
                    Attributes = e.Attributes
                        .Cast<EA.Attribute>()
                        .Select(CreateAttribute)
                        .ToList()
                };
            return _elements = package.GetElements(e => true).Select(CreateElement).ToList();
        }

        public IEnumerable<Element> GetElements(Func<Element, bool> filter)
        {
            var elements = GetElements();
            return elements.Where(filter);
        }

        public FilePath GetDiagramFilePath(Diagram diagram)
        {
            var eaDiagram = _repository.Repository.GetDiagramByID(diagram.Id);
            var filePath = System.IO.Path.GetTempPath() + eaDiagram.DiagramGUID.ToString() + ".png";
            var project = _repository.Repository.GetProjectInterface();
            project.PutDiagramImageToFile(eaDiagram.DiagramGUID, filePath, 1);
            var diagrams = new DiagramList { Diagrams = new List<Diagram> { diagram } };
            _jsonSerializer.Value.SerializeToFile(diagrams, "diagramList.json");
            return new FilePath(filePath);
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

			if (recursive)
			{
				throw new NotSupportedException();
			}
			else
			{
				var elements = package.Elements
					.Cast<EA.Element>()
					.Select(CreateElement);
				var fileName = $@"Elements - {packagePath}.json";
				_jsonSerializer.Value.SerializeToFile(elements, fileName);
				return elements;
			}
		}

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
			var fileName = $@"DiagramPaths - {packagePath}.json";
			var pathList = new PathList { Paths = paths.Select(p => p.ToString()).ToList() };
			_jsonSerializer.Value.SerializeToFile(pathList, fileName);
			return paths;
		}
	}

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
    public class Element
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
    public sealed class Attribute
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }

        [JsonProperty("notes")]
        public string Notes { get; internal set; }
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
