using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SassyStudio.Editor.Intellisense;

namespace SassyStudio.Editor
{
    public interface ISassEditor
    {
        event EventHandler<DocumentChangedEventArgs> DocumentChanged;

        ISassDocument Document { get; }
    }
}
