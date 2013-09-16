using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SassyStudio.Editor.Intellisense
{
    interface ICssSchema
    {
        IEnumerable<CssProperty> GetProperties(string prefix);

        IEnumerable<CssAtDirective> GetDirectives();

        IEnumerable<CssPseudo> GetPseudos();
    }

    class CssSchema : ICssSchema
    {
        readonly IDictionary<string, IBrowserReference> _Browsers = new Dictionary<string, IBrowserReference>(StringComparer.Ordinal);
        readonly IDictionary<string, CssAtDirective> _AtDirectives = new Dictionary<string, CssAtDirective>(StringComparer.Ordinal);
        readonly IDictionary<string, CssProperty> _Properties = new Dictionary<string, CssProperty>(StringComparer.Ordinal);
        readonly IDictionary<string, CssPseudo> _Pseudos = new Dictionary<string, CssPseudo>(StringComparer.Ordinal);

        public IEnumerable<CssProperty> GetProperties(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                return _Properties.Values;

            return _Properties.Values.Where(x => x.Name.StartsWith(prefix, StringComparison.Ordinal));
        }

        public IEnumerable<CssAtDirective> GetDirectives()
        {
            return _AtDirectives.Values;
        }

        public IEnumerable<CssPseudo> GetPseudos()
        {
            return _Pseudos.Values;
        }

        public static ICssSchema Parse(XDocument document, DirectoryInfo directory)
        {
            var schema = new CssSchema();
            ParseBrowsers(schema, document);
            ParseModules(schema, CompileModules(document, directory));

            return schema;
        }

        private static void ParseModules(CssSchema schema, Queue<FileInfo> modules)
        {
            while (modules.Count > 0)
            {
                var moduleFile = modules.Dequeue();
                if (!moduleFile.Exists) continue;

                var document = XDocument.Load(moduleFile.FullName);
                foreach (var element in document.Root.Elements())
                {
                    switch (element.Name.LocalName)
                    {
                        case "CssAtDirective": ParseDirective(schema, element); break;
                        case "CssProperty": ParseProperty(schema, element); break;
                        case "CssPropertyValue": ParsePropertyValue(schema, element); break;
                        case "CssPseudo": ParsePseudo(schema, element); break;
                        default: Logger.Log(string.Format("Unknown css schema element '{0}'", element.Name.LocalName)); break;
                    }
                }
            }
        }

        private static void ParseDirective(CssSchema schema, XElement element)
        {
            var name = element.Attribute("name");
            var description = element.Attribute("description");

            if (!schema._AtDirectives.ContainsKey(name.Value))
                schema._AtDirectives.Add(name.Value, new CssAtDirective(name.Value, Optional(description)));
        }

        private static void ParseProperty(CssSchema schema, XElement element)
        {
            var name = element.Attribute("name") ?? element.Attribute("_locID");
            // TODO: restriction
            // TODO: type
            var description = element.Attribute("description");

            if (!schema._Properties.ContainsKey(name.Value))
            {
                var property = new CssProperty(name.Value, Optional(description));
                schema._Properties.Add(name.Value, property);

                AddValues(property, element);
            }
        }

        private static void ParsePropertyValue(CssSchema schema, XElement element)
        {
            var propertyName = element.Attribute("type");
            CssProperty property;
            if (!schema._Properties.TryGetValue(propertyName.Value, out property))
                return;

            AddValues(property, element);
        }

        private static void AddValues(CssProperty property, XElement element)
        {
            foreach (var entry in element.Elements("entry"))
            {
                var name = entry.Attribute("value");
                var description = entry.Attribute("description");

                property.AddValue(new CssPropertyValue(name.Value, Optional(description)));
            }
        }

        private static void ParsePseudo(CssSchema schema, XElement element)
        {
            var name = element.Attribute("name");
            var description = element.Attribute("description");

            if (!schema._Pseudos.ContainsKey(name.Value))
                schema._Pseudos.Add(name.Value, new CssPseudo(name.Value, Optional(description)));
        }

        private static void ParseBrowsers(CssSchema schema, XDocument document)
        {
            foreach (var browser in document.Root.Elements("Browser"))
            {
                var name = browser.Attribute("name");
                var abbreviation = browser.Attribute("abbreviation");

                if (name != null && abbreviation != null && !schema._Browsers.ContainsKey(abbreviation.Value))
                    schema._Browsers.Add(abbreviation.Value, new BrowserReference(abbreviation.Value, name.Value));
            }
        }

        static Queue<FileInfo> CompileModules(XDocument document, DirectoryInfo directory)
        {
            var files = new Queue<FileInfo>();
            var basePath = directory.FullName;
            foreach (var moduleRef in document.Root.Elements("CssModuleRef"))
                files.Enqueue(new FileInfo(Path.Combine(basePath, moduleRef.Attribute("file").Value)));

            return files;
        }

        static string Optional(XAttribute attribute)
        {
            if (attribute != null)
                return attribute.Value;

            return null;
        }
    }
}
