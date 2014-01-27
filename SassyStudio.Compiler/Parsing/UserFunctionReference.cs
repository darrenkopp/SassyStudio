using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SassyStudio.Editor;

namespace SassyStudio.Compiler.Parsing
{
    class UserFunctionReference : Function, IResolvableToken
    {
        public UserFunctionReference() : base(SassClassifierType.UserFunctionReference)
        {
        }

        public ParseItem GetSourceToken()
        {
            return ReverseSearch.Find<UserFunctionDefinition>(this, x => x.FunctionName == FunctionName);
        }
    }
}
