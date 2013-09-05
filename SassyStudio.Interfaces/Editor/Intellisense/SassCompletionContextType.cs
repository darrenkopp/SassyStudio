using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio.Editor.Intellisense
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
        FunctionArgumentDefaultValue,
        FunctionArgument,
        FunctionArgumentName,
        FunctionArgumentValue,
        FunctionBody,

        IncludeDirective,
        IncludeDirectiveMixinName,
        IncludeDirectiveMixinArgument,
        IncludeDirectiveMixinArgumentName,
        IncludeDirectiveMixinArgumentValue,

        ImportDirective,
        ImportDirectiveFile,

        StringInterpolation,
        StringInterpolationValue,

        MixinDirective,
        MixinDirectiveMixinName,
        MixinDirectiveMixinArgumentDefaultValue,
        MixinBody,

        PropertyDeclaration,
        PropertyName,
        PropertyValue,

        VariableDeclaration,
        VariableName,
        VariableValue,
        VariableDefaultFlag,

        WhileLoopDirective,
        WhileLoopCondition
    }
}
