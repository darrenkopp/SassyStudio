using System.ComponentModel.Composition;

namespace SassyStudio.Editor.Intellisense
{
    interface ICssSchemaManager
    {
    }
    
    [Export(typeof(ICssSchemaManager))]
    class CssSchemaManager : ICssSchemaManager
    {
        readonly ICssSchemaLoader Loader;
        readonly ICssSchema Schema;

        [ImportingConstructor]
        public CssSchemaManager(ICssSchemaLoader loader)
        {
            Loader = loader;
            Schema = loader.Load();
        }
    }
}