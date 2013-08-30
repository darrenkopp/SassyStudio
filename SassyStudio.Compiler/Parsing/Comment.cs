using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Compiler.Parsing
{
    public abstract class Comment : ComplexItem
    {
        public Comment()
        {
        }

        public TokenItem OpenComment { get; protected set; }
        public TokenItem CommentText { get; protected set; }
        public TokenItem CloseComment { get; protected set; }
    }
}
