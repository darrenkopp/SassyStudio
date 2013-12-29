using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using SassyStudio.Compiler.Parsing;

namespace SassyStudio.Editor
{
    class GoToDefinitionCommandHandler : VSCommandTarget<VSConstants.VSStd97CmdID>
    {
        readonly ITextBuffer Buffer;
        readonly ISassEditor Editor;
        public GoToDefinitionCommandHandler(IVsTextView vsTextView, IWpfTextView textView, ISassEditorManager manager)
            : base(vsTextView, textView)
        {
            Buffer = textView.TextBuffer;
            Editor = manager.Get(Buffer);
        }

        protected override bool Execute(VSConstants.VSStd97CmdID command, uint options, IntPtr pvaIn, IntPtr pvaOut)
        {
            var stylesheet = Editor.Document.Stylesheet;
            if (stylesheet != null)
            {
                var position = this.TextView.Caret.Position.BufferPosition.Position;
                var target = stylesheet.Children.FindItemContainingPosition(position);
                var file = GetFile(target);
                if (file != null)
                {
                    OpenFileInPreviewTab(file.FullName);
                    return true;
                }
            }

            return false;
        }

        private FileInfo GetFile(ParseItem target)
        {
            var current = target;
            while (current != null)
            {
                var importFile = current as ImportFile;
                if (importFile != null)
                    return importFile.Document != null ? importFile.Document.Source : null;

                current = current.Parent;
            }

            return null;
        }

        public static void OpenFileInPreviewTab(string file)
        {
            IVsNewDocumentStateContext context = null;

            try
            {
                IVsUIShellOpenDocument3 shell = SassyStudioPackage.GetGlobalService(typeof(SVsUIShellOpenDocument)) as IVsUIShellOpenDocument3;

                Guid reason = VSConstants.NewDocumentStateReason.Navigation;
                context = shell.SetNewDocumentState((uint)__VSNEWDOCUMENTSTATE.NDS_Provisional, ref reason);

                SassyStudioPackage.Instance.DTE.ItemOperations.OpenFile(file);
            }
            finally
            {
                if (context != null)
                    context.Restore();
            }
        }

        protected override IEnumerable<VSConstants.VSStd97CmdID> SupportedCommands
        {
            get
            {
                yield return VSConstants.VSStd97CmdID.GotoDefn;
            }
        }

        protected override VSConstants.VSStd97CmdID ConvertFromCommandId(uint id)
        {
            return (VSConstants.VSStd97CmdID)id;
        }

        protected override uint ConvertFromCommand(VSConstants.VSStd97CmdID command)
        {
            return (uint)command;
        }
    }
}
