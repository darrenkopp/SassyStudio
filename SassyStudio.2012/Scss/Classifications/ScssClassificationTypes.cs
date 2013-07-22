using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SassyStudio.Scss.Classifications
{
    static class ScssClassificationTypes
    {
        public const string VariableDefinition = "scss_variable_definition";
        public const string VariableReference = "scss_variable_reference";
        public const string Keyword = "scss_keyword";
        public const string FunctionReference = "scss_function_reference";
        public const string UserFunctionDefinition = "scss_user_function_definition";
        public const string UserFunctionReference = "scss_user_function_reference";
        public const string MixinDefinition = "scss_mixin_declaration";
        public const string MixinReference = "scss_mixin_reference";
        public const string Comment = "scss_comment";
        public const string ImportanceModifier = "scss_importance_modifier";
        public const string Interpolation = "scss_interpolation";
        public const string StringValue = "scss_string";
        public const string ParentReference = "scss_parent_reference";
    }

    static class ScssClassificationTypeDefinitions
    {
        [Export, Name(ScssClassificationTypes.VariableDefinition), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal static ClassificationTypeDefinition ScssVariableDefinition = null;

        [Export, Name(ScssClassificationTypes.VariableReference), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal static ClassificationTypeDefinition ScssVariableReference = null;

        [Export, Name(ScssClassificationTypes.Keyword), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal static ClassificationTypeDefinition ScssKeyword = null;

        [Export, Name(ScssClassificationTypes.FunctionReference), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal static ClassificationTypeDefinition ScssFunctionReference = null;

        [Export, Name(ScssClassificationTypes.UserFunctionDefinition), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal static ClassificationTypeDefinition ScssUserFunctionDefinition = null;

        [Export, Name(ScssClassificationTypes.UserFunctionReference), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal static ClassificationTypeDefinition ScssUserFunctionReference = null;

        [Export, Name(ScssClassificationTypes.MixinDefinition), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal static ClassificationTypeDefinition ScssMixinDefinition = null;

        [Export, Name(ScssClassificationTypes.MixinReference), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal static ClassificationTypeDefinition ScssMixinReference = null;

        [Export, Name(ScssClassificationTypes.Comment), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal static ClassificationTypeDefinition ScssComment = null;

        [Export, Name(ScssClassificationTypes.ImportanceModifier), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal static ClassificationTypeDefinition ScssImportanceModifier = null;

        [Export, Name(ScssClassificationTypes.Interpolation), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal static ClassificationTypeDefinition ScssInterpolation = null;

        [Export, Name(ScssClassificationTypes.StringValue), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal static ClassificationTypeDefinition ScssStringValue = null;

        [Export, Name(ScssClassificationTypes.ParentReference), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal static ClassificationTypeDefinition ScssParentReference = null;
    }
}
