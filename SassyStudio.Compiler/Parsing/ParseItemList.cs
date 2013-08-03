using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    public class ParseItemList : List<ParseItem>
    {
        public ParseItemList()
            : base(0)
        {
        }

        public TokenItem AddCurrentAndAdvance(ITokenStream stream, SassClassifierType classifierType = SassClassifierType.Default)
        {
            var item = new TokenItem(stream.Current, classifierType);
            Add(item);

            stream.Advance();
            return item;
        }
    }
}
