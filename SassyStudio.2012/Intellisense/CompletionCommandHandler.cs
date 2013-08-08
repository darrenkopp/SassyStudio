using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using VSCommandIdConstants = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

namespace SassyStudio.Intellisense
{
    class CompletionCommandHandler : Commands.CommandTargetBase
    {
        private readonly ICompletionBroker CompletionBroker;
        public CompletionCommandHandler(IVsTextView adapter, IWpfTextView view, ICompletionBroker completionBroker)
            : base(adapter, view, typeof(VSCommandIdConstants).GUID, (uint)VSCommandIdConstants.TYPECHAR, (uint)VSCommandIdConstants.TAB, (uint)VSCommandIdConstants.RETURN, (uint)VSCommandIdConstants.BACKSPACE, (uint)VSCommandIdConstants.DELETE)
        {
            CompletionBroker = completionBroker;
        }

        private ICompletionSession CompletionSession { get; set; }

        protected override bool IsEnabled()
        {
            return true;
        }

        protected override bool Execute(uint commandId, uint execOptions, IntPtr pvaIn, IntPtr pvaOut)
        {
            char typed = char.MinValue;
            if (commandId == (uint)VSCommandIdConstants.TYPECHAR)
                typed = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);

            if (commandId == (uint)VSCommandIdConstants.RETURN || commandId == (uint)VSCommandIdConstants.TAB || char.IsWhiteSpace(typed))
            {
                if (InActiveCompletionSession())
                { 
                    if (CompletionSession.SelectedCompletionSet.SelectionStatus.IsSelected)
                    {
                        CompletionSession.Commit();

                        // pass space on
                        if (char.IsWhiteSpace(typed))
                            ExecuteNext(commandId, execOptions, pvaIn, pvaOut);

                        return true;
                    }
                    else
                    {
                        CompletionSession.Dismiss();
                    }
                }
            }

            var nextResult = ExecuteNext(commandId, execOptions, pvaIn, pvaOut);
            bool handled = false;
            if (IsCompletionCharacter(typed))
            {
                if (!InActiveCompletionSession())
                {
                    if (TriggerCompletion() && CompletionSession != null)
                        CompletionSession.Filter();
                }
                else
                {
                    CompletionSession.Filter();
                }

                handled = true;
            }
            else if (commandId == (uint)VSCommandIdConstants.DELETE || commandId == (uint)VSCommandIdConstants.BACKSPACE)
            {
                if (InActiveCompletionSession())
                    CompletionSession.Filter();

                handled = true;
            }

            if (handled)
                return true;

            return nextResult == VSConstants.S_OK;
        }

        private bool TriggerCompletion()
        {
            SnapshotPoint? caretPoint = TextView.Caret.Position.Point.GetPoint(textBuffer => (!textBuffer.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
            if (!caretPoint.HasValue)
                return false;

            var caret = caretPoint.Value;
            var tracking = caret.Snapshot.CreateTrackingPoint(caret.Position, PointTrackingMode.Positive);
            CompletionSession = CompletionBroker.CreateCompletionSession(TextView, tracking, true);
            CompletionSession.Dismissed += OnSessionDismissed;
            CompletionSession.Start();

            return true;
        }

        private bool IsCompletionCharacter(char typed)
        {
            if (char.IsLetter(typed))
                return true;

            switch (typed)
            {
                case '$':
                case '!':
                //case ':':
                case '@':
                    return true;
            }

            return char.IsLetterOrDigit(typed);
        }

        private bool InActiveCompletionSession()
        {
            return CompletionSession != null && !CompletionSession.IsDismissed;
        }

        private void OnSessionDismissed(object sender, EventArgs e)
        {
            CompletionSession.Dismissed -= OnSessionDismissed;
            CompletionSession = null;
        }
    }
}
