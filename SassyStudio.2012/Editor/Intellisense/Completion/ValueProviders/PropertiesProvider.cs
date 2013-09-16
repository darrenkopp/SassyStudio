using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionValueProvider))]
    class PropertiesProvider : ICompletionValueProvider
    {
        static readonly IEnumerable<ICompletionValue> NO_RESULTS = Enumerable.Empty<ICompletionValue>();
        readonly ICssSchemaManager SchemaManager;

        [ImportingConstructor]
        public PropertiesProvider(ICssSchemaManager schemaManager)
        {
            SchemaManager = schemaManager;
        }

        public IEnumerable<SassCompletionContextType> SupportedContexts
        {
            get
            {
                yield return SassCompletionContextType.PropertyDeclaration;
                yield return SassCompletionContextType.PropertyName;
                yield return SassCompletionContextType.PropertyValue;
            }
        }

        public IEnumerable<ICompletionValue> GetCompletions(SassCompletionContextType type, ICompletionContext context)
        {
            var schema = SchemaManager.CurrentSchema;
            if (schema != null)
            {
                switch (type)
                {
                    case SassCompletionContextType.PropertyDeclaration:
                    case SassCompletionContextType.PropertyName:
                        return GetPropertyNames(context, schema);
                    case SassCompletionContextType.PropertyValue:
                        return GetPropertyValues(context, schema);
                }
            }

            return NO_RESULTS;
        }

        private IEnumerable<ICompletionValue> GetPropertyValues(ICompletionContext context, ICssSchema schema)
        {
            var property = context.Current as PropertyDeclaration;
            if (property == null || property.Name == null || !IsPlainPropertyName(property.Name))
                return NO_RESULTS;

            // right now we just grab full text since we only support plain ol' properties.
            // in the future may support evaluating #{$name}-left style properties
            var propertyName = context.DocumentTextProvider.GetText(property.Name.Start, property.Name.Length);

            return schema.GetPropertyValues(propertyName).Select(x => new PropertyCompletionValue(x.Name, x.Description));
        }

        private bool IsPlainPropertyName(PropertyName name)
        {
            return name.Children.All(x => x is TokenItem);
        }

        public IEnumerable<ICompletionValue> GetPropertyNames(ICompletionContext context, ICssSchema schema)
        {
            return schema.GetProperties(null).Select(x => new PropertyCompletionValue(x.Name, x.Description));
        }
    }
}
