using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio
{
    public interface ISassDocument
    {
        event EventHandler<StylesheetChangedEventArgs> StylesheetChanged;

        FileInfo Source { get; }
        ISassStylesheet Stylesheet { get; }

        ISassStylesheet Update(ISassStylesheet stylesheet);
    }
}
