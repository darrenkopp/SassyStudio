using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Threading;
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
                var source = GetSourceToken(target);
                var file = FindFile(source);

                if (file != null)
                {
                    OpenFileInPreviewTab(file.FullName);

                    // if we aren't the entire document, then we want to jump to it's position
                    if (!(source is Stylesheet))
                        GoToPosition(source);
                    return true;
                }
            }

            return false;
        }

        private ParseItem GetSourceToken(ParseItem target)
        {
            var current = target;
            while (current != null)
            {
                var resolvable = current as IResolvableToken;
                if (resolvable != null)
                    return resolvable.GetSourceToken();

                current = current.Parent;
            }

            return null;
        }

        private void GoToPosition(ParseItem target)
        {
            try
            {
                var view = ExtensibilityHelper.GetCurentTextView();
                var textBuffer = ExtensibilityHelper.GetCurentTextBuffer();
                var span = new SnapshotSpan(textBuffer.CurrentSnapshot, target.Start, target.Length);
                var point = new SnapshotPoint(textBuffer.CurrentSnapshot, target.Start);

                view.ViewScroller.EnsureSpanVisible(span);
                view.Caret.MoveTo(point);
                //view.Selection.Select(span, false);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, "Failed to navigate to line.");
            }
        }

        private FileInfo FindFile(ParseItem item)
        {
            if (item == null)
                return null;

            var current = item;
            while (current != null)
            {
                var sheet = current as ISassStylesheet;
                if (sheet != null && sheet.Owner != null)
                    return sheet.Owner.Source;

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
