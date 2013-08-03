using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SassyStudio.Compiler.Parsing
{
    class UserFunctionReference : Function
    {
        public UserFunctionReference() : base(SassClassifierType.UserFunctionReference)
        {
        }
    }
}
