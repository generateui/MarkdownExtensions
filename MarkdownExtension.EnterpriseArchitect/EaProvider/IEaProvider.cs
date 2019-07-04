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
        FilePath GetDiagramFilePath(string diagramName);
        Element GetElementByName(string elementName);
        IEnumerable<Element> GetElements(Func<Element, bool> filter);
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

        public FilePath GetDiagramFilePath(string diagramName)
        {
            if (!_formatSettings.ForceRefreshData)
            {
                var result = _jsonProvider.GetDiagramFilePath(diagramName);
                if (result != null)
                {
                    return result;
                }
            }
            return _eaProvider.GetDiagramFilePath(diagramName);
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
        private Dictionary<string, Diagram> _diagramsByName;
        private Dictionary<string, Element> _elementsByName;
        private List<Element> _elements;

        public FilePath GetDiagramFilePath(string diagramName)
        {
            var fromCache = GetDiagramFilePathFromCache(diagramName);
            if (fromCache != null)
            {
                return fromCache;
            }
            if (File.Exists("diagramList.json"))
            {
                DiagramList diagrams;
                try
                {
                    diagrams = _jsonSerializer.Value.DeserializeFromFile<DiagramList>("diagramList.json");
                }
                catch (JsonReaderException jre)
                {
                    // TODO: something with exception
                    return null;
                }
                _diagramsByName = diagrams.Diagrams.ToDictionary(x => x.Name, x => x);
                var fromCache1 = GetDiagramFilePathFromCache(diagramName);
                if (fromCache1 != null)
                {
                    return fromCache1;
                }
            }
            return null;
        }

        private FilePath GetDiagramFilePathFromCache(string diagramName)
        {
            if (_diagramsByName != null && _diagramsByName.ContainsKey(diagramName))
            {
                var diagram = _diagramsByName[diagramName];
                var filePath = System.IO.Path.GetTempPath() + diagram.Guid.ToString() + ".png";
                return new FilePath(filePath);
            }
            return null;
        }

        public Package GetElementsByPackage(Path path)
        {
            if (File.Exists("ea.json"))
            {
                Package package;
                try
                {
                    package = _jsonSerializer.Value.DeserializeFromFile<Package>("ea.json");
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
    }
    internal class EaProvider : IEaProvider
    {
        private EA.RepositoryClass _repository;
        private readonly Func<RepositoryWrapper> _getRepository;
        private List<Element> _elements;
        private readonly Lazy<JsonSerializer> _jsonSerializer = new Lazy<JsonSerializer>(() =>
        {
            var jsonSerializer = new JsonSerializer();
            jsonSerializer.Converters.Add(new PathConverter());
            return jsonSerializer;
        });

        public EaProvider(Func<RepositoryWrapper> getRepository)
        {
            _getRepository = getRepository;
        }

        public Package GetElementsByPackage(Path path)
        {
            _repository = _repository ?? _getRepository();
            EA.Package rootPackage = (EA.Package)_repository.Models.GetAt(0);
            EA.Package eaPackage = rootPackage.GetPackage(path);
            Package package = FromEaPackage(eaPackage);
            _jsonSerializer.Value.SerializeToFile(package, "ea.json");
            return package;
        }

        public Package FromEaPackage(EA.Package eaPackage)
        {
            var package = new Package
            {
                Id = eaPackage.PackageID,
                Name = eaPackage.Name,
                Elements = new List<Element>(),
                Packages = new List<Package>()
            };
            foreach (var e in eaPackage.Elements)
            {
                var eaElement = e as EA.Element;
                var element = new Element
                {
                    Id = eaElement.ElementID,
                    Name = eaElement.Name,
                    Notes = eaElement.Notes,
                    Stereotype = eaElement.Stereotype
                };
                package.Elements.Add(element);
            }
            foreach (var p in eaPackage.Packages)
            {
                var eaChildPackage = p as EA.Package;
                var childPackage = FromEaPackage(eaChildPackage);
                package.Packages.Add(childPackage);
            }
            return package;
        }

        public FilePath GetDiagramFilePath(string diagramName)
        {
            _repository = _repository ?? _getRepository();
            EA.Package rootPackage = (EA.Package)_repository.Models.GetAt(0);
            bool Filter(EA.Diagram diagram) => diagram.Name.Equals(diagramName);
            EA.Diagram eaDiagram = rootPackage.GetDiagrams(Filter).FirstOrDefault();
            var filePath = System.IO.Path.GetTempPath() + eaDiagram.DiagramGUID.ToString() + ".png";
            var project = _repository.GetProjectInterface();
            project.PutDiagramImageToFile(eaDiagram.DiagramGUID, filePath, 1);
            var package = _repository.GetPackageByID(eaDiagram.PackageID);
            var path = package.ToPath(_repository);
            var diagrams = new DiagramList
            {
                Diagrams = new List<Diagram>
                {
                    new Diagram
                    {
                        Id = eaDiagram.DiagramID,
                        Name = eaDiagram.Name,
                        Path = path,
                        Guid = Guid.Parse(eaDiagram.DiagramGUID)
                    }
                }
            };
            _jsonSerializer.Value.SerializeToFile(diagrams, "diagramList.json");
            return new FilePath(filePath);
        }

        public Element GetElementByName(string elementName)
        {
            _repository = _repository ?? _getRepository();
            var package = (EA.Package)_repository.Models.GetAt(0);
            Attribute CreateAttribute(EA.Attribute a) =>
                new Attribute
                {
                    Id = a.AttributeID,
                    Name = a.Name,
                    Notes = a.Notes
                };
            Element CreateElement(EA.Element e) =>
                new Element
                {
                    Id = e.ElementID,
                    Name = e.Name,
                    Notes = e.Notes,
                    Stereotype = e.Stereotype,
                    Attributes = e.Attributes
                        .Cast<EA.Attribute>()
                        .Select(CreateAttribute)
                        .ToList()
                };
            var elements = package.GetElements(e => true).Select(CreateElement).ToList();
            var elementsList = new ElementList { Elements = elements };
            _jsonSerializer.Value.SerializeToFile(elementsList, "elementList.json");
            var element = elements.FirstOrDefault(e => Equals(elementName, e.Name));
            return element;
        }

        private IEnumerable<Element> GetElements()
        {
            if (_elements != null)
            {
                return _elements;
            }
            _repository = _repository ?? _getRepository();
            var package = (EA.Package)_repository.Models.GetAt(0);
            Attribute CreateAttribute(EA.Attribute a) =>
                new Attribute
                {
                    Id = a.AttributeID,
                    Name = a.Name,
                    Notes = a.Notes
                };
            Element CreateElement(EA.Element e) =>
                new Element
                {
                    Id = e.ElementID,
                    Name = e.Name,
                    Notes = e.Notes,
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
    }

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
    public sealed class Element
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")] 
        public string Name { get; internal set; }

        [JsonProperty("stereotype")]
        public string Stereotype { get; internal set; }

        [JsonProperty("notes")]
        public string Notes { get; internal set; }

        [JsonProperty("attributes")]
        public List<Attribute> Attributes { get; internal set; }
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

    public sealed class Package
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("elements")]
        public List<Element> Elements { get; internal set; }

        [JsonProperty("packages")]
        public List<Package> Packages { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }
    }
}
