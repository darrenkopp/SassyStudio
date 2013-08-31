using System.Collections.Generic;

namespace SassyStudio.Editor.Intellisense
{
    public interface IIntellisenseCache
    {
        void Update(ISassStylesheet stylesheet, ITextProvider text = null);

        IEnumerable<ICompletionValue> GetVariables(int position = 0);
        IEnumerable<ICompletionValue> GetFunctions(int position = 0);
        IEnumerable<ICompletionValue> GetMixins(int position = 0); 
    }
}