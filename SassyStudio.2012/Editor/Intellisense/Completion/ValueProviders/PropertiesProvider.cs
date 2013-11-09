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

            return schema.GetPropertyValues(propertyName).Select(x => new PropertyValueCompletionValue(x.Name, x.Description));
        }

        private bool IsPlainPropertyName(PropertyName name)
        {
            return name.Children.All(x => x is TokenItem);
        }

        public IEnumerable<ICompletionValue> GetPropertyNames(ICompletionContext context, ICssSchema schema)
        {
            var prefix = ResolvePrefix(context);

            return schema.GetProperties(prefix).Select(x => new PropertyNameCompletionValue(x.Name, x.Description));
        }

        private string ResolvePrefix(ICompletionContext context)
        {
            var current = context.Current;
            var parts = new Stack<PropertyDeclaration>(1);
            while (current != null)
            {
                if (current is PropertyDeclaration)
                {
                    var property = current as PropertyDeclaration;
                    if (property.Name != null && property.Name.Fragments.All(x => x is TokenItem))
                        parts.Push(property);
                }

                current = current.Parent;
            }

            var builder = new StringBuilder();
            while (parts.Count > 0)
            {
                var part = parts.Pop();
                if (builder.Length > 0)
                    builder.Append("-");

                builder.Append(part.Name.GetName(context.DocumentTextProvider));
            }

            if (builder.Length == 0)
                return null;

            // if we aren't currently in "nested" block directly, then chop off current value
            if (!(context.Current is NestedPropertyBlock))
                return builder.ToString(0, LastHyphen(builder));

            return builder.ToString();
        }

        private int LastHyphen(StringBuilder builder)
        {
            for (int i = builder.Length - 1; i > 0; i--)
                if (builder[i] == '-')
                    return i;

            return builder.Length;
        }
    }
}
