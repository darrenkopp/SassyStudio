using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    static class ParseItemExtensions
    {
        public static void WriteTo(this ParseItem item, StringBuilder builder, ITextProvider text)
        {
            int end = item.End;
            for (int position = item.Start; position < end; position++)
            {
                builder.Append(text[position]);
            }
        }
    }
}
