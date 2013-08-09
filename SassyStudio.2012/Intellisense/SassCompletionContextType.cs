using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Intellisense
{
    public enum SassCompletionContextType
    {
        None = 0,
        AtDirectiveName,
        AtDirectiveBlock,

        ConditionalDirective,
        ConditionalDirectiveExpression,

        CssDirective,

        EachDirective,
        EachDirectiveListValue,

        ExtendDirective,
        ExtendDirectiveReference,
        ExtendDirectiveOptionalFlag,

        ForLoopDirective,
        ForLoopVariable,
        ForLoopFromKeyword,
        ForLoopRangeKeyword,

        FunctionDirective,
        FunctionDeclaration,
        FunctionArgumentDeclaration,
        FunctionArgument,
        FunctionArgumentName,
        FunctionBody,

        IncludeDirective,
        IncludeDirectiveMixinName,
        IncludeDirectiveMixinArgument,
        IncludeDirectiveMixinArgumentName,
        IncludeDirectiveMixinArgumentValue,

        ImportDirective,
        ImportDirectiveFile,

        StringInterpolation,

        MixinDirective,
        MixinDirectiveMixinName,
        MixinDirectiveMixinArgumentValue,

        PropertyDeclaration,
        PropertyName,
        PropertyValue,

        VariableDeclaration,
        VariableName,
        VariableValue,

        WhileLoopDirective,
        WhileLoopCondition
    }
}
