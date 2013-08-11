using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio
{
    public enum SassClassifierType
    {
        Default,
        String,
        Comment,
        Keyword,
        UserFunctionDefinition,
        UserFunctionReference,
        MixinDefinition,
        MixinReference,
        VariableReference,
        VariableDefinition,
        ImportanceModifier,
        ParentReference,
        Interpolation,
        SystemFunction,
        FunctionBrace,
        Punctuation,
        CurlyBrace,
        Operator,
        ClassName,
        HexColor,
        IdName,
        ElementName,
        PseudoElement,
        PseudoClass,
        SquareBrace
    }
}
