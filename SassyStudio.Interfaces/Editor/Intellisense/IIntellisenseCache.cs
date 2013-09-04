using System.Collections.Generic;

namespace SassyStudio.Editor.Intellisense
{
    public interface IIntellisenseCache
    {
        void Update(ISassStylesheet stylesheet, ITextProvider text = null);

        IEnumerable<ICompletionValue> GetVariables(int position = int.MaxValue);
        IEnumerable<ICompletionValue> GetFunctions(int position = int.MaxValue);
        IEnumerable<ICompletionValue> GetMixins(int position = int.MaxValue);
    }
}