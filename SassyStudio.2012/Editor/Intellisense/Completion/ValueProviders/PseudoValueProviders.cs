using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
{
    [Export(typeof(ICompletionValueProvider))]
    class PseudoValueProviders : ICompletionValueProvider
    {
        readonly ICssSchemaManager SchemaManager;

        [ImportingConstructor]
        public PseudoValueProviders(ICssSchemaManager schemaManager)
        {
            SchemaManager = schemaManager;
        }

        public IEnumerable<SassCompletionContextType> SupportedContexts
        {
            get
            {
                yield return SassCompletionContextType.PseudoClass;
                yield return SassCompletionContextType.PseudoElement;
                yield return SassCompletionContextType.PseudoFunction;
            }
        }

        public IEnumerable<ICompletionValue> GetCompletions(SassCompletionContextType type, ICompletionContext context)
        {
            var schema = SchemaManager.CurrentSchema;
            if (schema == null)
                return Enumerable.Empty<ICompletionValue>();

            return schema.GetPseudos().Select(x => new PseudoCompletionValue(x.Name, x.Description));
        }
    }
}
