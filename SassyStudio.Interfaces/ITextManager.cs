using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio
{
    public interface ITextManager
    {
        ITextProviderScope Open();
    }

    public interface ITextProviderScope : IDisposable
    {
        ITextProvider Text { get; }
    }
}
