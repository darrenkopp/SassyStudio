using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionValueProvider))]
    class PropertiesProvider : ICompletionValueProvider
    {
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
            }
        }

        public IEnumerable<ICompletionValue> GetCompletions(SassCompletionContextType type, ICompletionContext context)
        {
            var schema = SchemaManager.CurrentSchema;
            if (schema == null)
                return Enumerable.Empty<ICompletionValue>();

            return schema.GetProperties(null).Select(x => new PropertyCompletionValue(x.Name, x.Description));
        }
    }
}
