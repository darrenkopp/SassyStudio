using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Utilities;

namespace SassyStudio.Sass
{
    public class SassContentTypeDefinition
    {
        public const string SassContentType = "Sass";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [Export(typeof(ContentTypeDefinition))]
        [Name(SassContentType)]
        [BaseDefinition("text"), BaseDefinition("intellisense")]
        public static ContentTypeDefinition ISassContentType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(SassContentType)]
        [FileExtension(".sass")]
        public static FileExtensionToContentTypeDefinition ISassFileExtension { get; set; }
    }
}
