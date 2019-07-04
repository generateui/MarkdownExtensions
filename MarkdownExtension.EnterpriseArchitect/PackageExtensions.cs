using MarkdownExtension.EnterpriseArchitect.EaProvider;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownExtension.EnterpriseArchitect
{
    public static class PackageExtensions
    {
        public static IEnumerable<EA.Element> GetElements(this EA.Package package, Func<EA.Element, bool> filter, List<EA.Element> items = null)
        {
            var result = items ?? new List<EA.Element>();
            foreach (object child in package.Elements)
            {
                var el = child as EA.Element;
                if (filter(el))
                {
                    result.Add(el);
                }
            }
            foreach (object child in package.Diagrams)
            {
                var diagram = child as EA.Diagram;
                foreach (var diagramObject in diagram.DiagramObjects)
                {
                    var el = diagramObject as EA.Element;
                    if (el != null)
                    {
                        System.Diagnostics.Debug.WriteLine(el.Name);
                    }

                }
            }
            foreach (var p in package.Packages)
            {
                var childPackage = p as EA.Package;
                childPackage.GetElements(filter, result);
            }
            return result;
        }

        public static List<EA.Package> GetPackages(this EA.Package package, Func<EA.Package, bool> filter, List<EA.Package> result = null)
        {
            result = result ?? new List<EA.Package>();
            foreach (var p in package.Packages)
            {
                var childPackage = p as EA.Package;
                if (filter(childPackage))
                {
                    result.Add(childPackage);
                }
                childPackage.GetPackages(filter, result);
            }
            return result;
        }

        public static EA.Package GetPackage(this EA.Package package, Path path)
        {
            EA.Package current = package;
            bool first = true;
            foreach (var part in path.Parts.Skip(1))
            {
                EA.Package childPackage = GetPackage(current, part);
                current = childPackage;
            }
            return current;
        }

        private static EA.Package GetPackage(EA.Package package, string childPackageName)
        {
            foreach (var p in package.Packages)
            {
                var childPackage = p as EA.Package;
                if (Equals(childPackage.Name, childPackageName))
                {
                    return childPackage;
                }
            }
            return null;
        }

        public static IEnumerable<EA.Diagram> GetDiagrams(this EA.Package package, Func<EA.Diagram, bool> filter, List<EA.Diagram> items = null)
        {
            var result = items ?? new List<EA.Diagram>();
            foreach (object child in package.Diagrams)
            {
                var diagram = child as EA.Diagram;
                if (filter(diagram))
                {
                    result.Add(diagram);
                }
            }
            foreach (var p in package.Packages)
            {
                var childPackage = p as EA.Package;
                childPackage.GetDiagrams(filter, result);
            }
            return result;
        }

        public static Path ToPath(this EA.Package package, EA.RepositoryClass repository)
        {
            var parts = new List<string> { package.Name };
            var parent = repository.GetPackageByID(package.ParentID);
            while (parent.ParentID != 0)
            {
                parts.Insert(0, parent.Name);
                parent = repository.GetPackageByID(package.ParentID);
            }
            return new Path(parts);
        }
    }
}
