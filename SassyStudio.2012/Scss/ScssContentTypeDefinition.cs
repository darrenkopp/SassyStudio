using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Utilities;

namespace SassyStudio
{
    static class ScssContentTypeDefinition
    {
        public const string ScssContentType = "SCSS";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [Export(typeof(ContentTypeDefinition))]
        [Name(ScssContentType)]
        [BaseDefinition("text"), BaseDefinition("intellisense")]
        public static ContentTypeDefinition IScssContentType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(ScssContentType)]
        [FileExtension(".scss")]
        public static FileExtensionToContentTypeDefinition IScssFileExtension { get; set; }
    }
}