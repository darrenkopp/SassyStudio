using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Lexing
{
    static class TextStreamExtensions
    {
        public static bool InUndoContext(this ITextStream stream, Func<int, bool> callback)
        {
            int start = stream.Position;
            bool result = callback(start);

            if (!result)
                stream.SeekTo(start);

            return result;
        }

        public static bool CompareOrdinal(this ITextStream stream, string value)
        {
            if (value == null) 
                return false;

            for (int i = 0; i < value.Length; i++)
                if (value[i] != stream.Peek(i))
                    return false;

                return true;
        }
    }
}
