using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using VSCommandIdConstants = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

namespace SassyStudio.Intellisense
{
    class CompletionCommandHandler : Commands.CommandTargetBase
    {
        readonly ICompletionBroker CompletionBroker;

        public CompletionCommandHandler(IVsTextView adapter, IWpfTextView view, ICompletionBroker completionBroker)
            : base(adapter, view, typeof(VSCommandIdConstants).GUID, (uint)VSCommandIdConstants.TYPECHAR, (uint)VSCommandIdConstants.TAB, (uint)VSCommandIdConstants.RETURN, (uint)VSCommandIdConstants.BACKSPACE, (uint)VSCommandIdConstants.DELETE, (uint)VSCommandIdConstants.SHOWMEMBERLIST)
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
            var command = (VSCommandIdConstants)commandId;

            // handle ctrl + space
            if (command == VSCommandIdConstants.SHOWMEMBERLIST)
            {
                if (!InActiveCompletionSession())
                    TriggerCompletion();

                return true;
            }

            char typed = char.MinValue;
            if (command == VSCommandIdConstants.TYPECHAR)
                typed = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);

            // handle completion characters
            if (command == VSCommandIdConstants.RETURN || command == VSCommandIdConstants.TAB || char.IsWhiteSpace(typed))
            {
                if (InActiveCompletionSession())
                {
                    if (CompletionSession.SelectedCompletionSet.SelectionStatus.IsSelected)
                    {
                        CompletionSession.Commit();

                        // pass space character on
                        if (typed == ' ')
                            return false;
                    }
                    else
                    {
                        CompletionSession.Dismiss();
                    }

                    // swallow up return / tab keys
                    return true;
                }

                // pass on return / tab
                return false;
            }

            // pass on command to next handler
            var nextResult = ExecuteNext(commandId, execOptions, pvaIn, pvaOut);
            if (command == VSCommandIdConstants.TYPECHAR)
            {
                // attempt to start completion session
                if (!InActiveCompletionSession() && IsCompletionStartCharacter(typed))
                    TriggerCompletion();

                if (InActiveCompletionSession())
                    CompletionSession.Filter();
            }
            else if (command == VSCommandIdConstants.DELETE || command == VSCommandIdConstants.BACKSPACE)
            {
                if (InActiveCompletionSession())
                    CompletionSession.Filter();
            }

            return nextResult;
        }

        bool ShowCompletion(int start)
        {
            DismissSession(CompletionSession);

            var snapshot = TextView.TextSnapshot;
            var tracking = snapshot.CreateTrackingPoint(start, PointTrackingMode.Positive);

            // create our session
            var session = CompletionBroker.CreateCompletionSession(TextView, tracking, true);
            session.Dismissed += OnSessionDismissed;
            session.Start();

            CompletionSession = session;
            return !session.IsDismissed;
        }

        private bool TriggerCompletion()
        {
            var caretPoint = GetCaretPoint();
            if (caretPoint == null)
                return false;

            return ShowCompletion(caretPoint.Value.Position);
        }

        private void DismissSession(ICompletionSession session)
        {
            if (session != null && !session.IsDismissed)
                session.Dismiss();
        }

        private bool InActiveCompletionSession()
        {
            return CompletionSession != null && !CompletionSession.IsDismissed;
        }

        private void OnSessionDismissed(object sender, EventArgs e)
        {
            var source = sender as ICompletionSession;
            source.Dismissed -= OnSessionDismissed;

            if (source == CompletionSession)
                CompletionSession = null;
        }

        SnapshotPoint? GetCaretPoint()
        {
            return TextView.Caret.Position.Point.GetPoint(b => (!b.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
        }

        private bool IsCompletionCharacter(char typed)
        {
            switch (typed)
            {
                case '$':
                case '!':
                case ':':
                case '@':
                    return true;
            }

            return char.IsLetterOrDigit(typed);
        }

        private bool IsCompletionStartCharacter(char c)
        {
            switch (c)
            {
                case '$':
                case '!':
                case ':':
                case '@':
                case '-':
                    return true;
            }

            return char.IsLetterOrDigit(c);
        }
    }
}
