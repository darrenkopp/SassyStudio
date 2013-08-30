using System.Collections.Generic;

namespace SassyStudio.Editor.Intellisense
{
    public interface IIntellisenseCache
    {
        void Update(ISassStylesheet stylesheet, ITextProvider text = null);
    }
}