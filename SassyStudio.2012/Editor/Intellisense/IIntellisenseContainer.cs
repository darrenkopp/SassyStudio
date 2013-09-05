using System.Collections.Generic;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor.Intellisense
{
    interface IIntellisenseContainer
    {
        bool IsApplicableTo(int position);
        int Start { get; }
        int End { get; }
        void Add(ParseItem item, ITextProvider text);

        IEnumerable<ICompletionValue> GetVariables(int position);
        IEnumerable<ICompletionValue> GetFunctions(int position);
        IEnumerable<ICompletionValue> GetMixins(int position); 
    }
}