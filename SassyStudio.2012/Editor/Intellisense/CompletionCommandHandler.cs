using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using VSCommand = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

namespace SassyStudio.Editor.Intellisense
{
    class CompletionCommandHandler : VSCommandTarget<VSCommand>
    {
        readonly ICompletionBroker Broker;
        public CompletionCommandHandler(ICompletionBroker broker, IVsTextView vsTextView, IWpfTextView textView)
            : base(vsTextView, textView)
        {
            Broker = broker;
        }

        ICompletionSession Session { get; set; }
        bool IsIgnoring { get; set; }

        protected override bool Execute(VSCommand command, uint options, IntPtr pvaIn, IntPtr pvaOut)
        {
            char typed = char.MinValue;
            if (command == VSCommand.TYPECHAR)
                typed = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);

            if (command == VSCommand.CANCEL)
                return Cancel();

            // handle auto complete commands
            if (command == VSCommand.AUTOCOMPLETE || command == VSCommand.COMPLETEWORD)
                return AutoComplete();

            // show member list (ie show me completion right where i'm at)
            if (command == VSCommand.SHOWMEMBERLIST)
                return ShowMemberList();

            // attempt to complete session
            if (IsCompletionRequested(command, typed))
            {
                bool completed = Complete(force: (command == VSCommand.TAB));

                // if command is return or tab, then return completed result as command result
                // when completed is false, then next command will output those characters to document
                if (command == VSCommand.TAB || command == VSCommand.RETURN)
                    return completed;
            }

            // if we are ignoring command, then don't handle
            if (Ignore(command, typed))
                return false;

            // pass on command to next handler (aka send character to next command handler)
            var result = ExecuteNext(command, options, pvaIn, pvaOut);

            // filter on backspace / delete
            if (command == VSCommand.BACKSPACE || command == VSCommand.DELETE)
            {
                Filter(Session);
                return result;
            }

            // if a character was typed that starts a session, then show completion session
            if (command == VSCommand.TYPECHAR && IsStartCharacter(typed))
                ShowCompletion();

            // filter the session
            Filter(Session);

            return true;
        }

        private bool Cancel()
        {
            bool dismissed = Dismiss(Session);

            // ignore completion events until we break out of current context
            if (dismissed)
                IsIgnoring = true;

            return dismissed;
        }

        private bool AutoComplete()
        {
            if (ShowCompletion(adjust: true))
            {
                Session.Recalculate();
                Filter(Session);
                return Complete(force: true);
            }

            return false;
        }

        private bool ShowMemberList()
        {
            if (ShowCompletion(adjust: true))
            {
                Session.Recalculate();
                Filter(Session);
                return true;
            }

            return false;
        }

        private void Filter(ICompletionSession session)
        {
            if (IsSessionActive(session))
            {
                session.SelectedCompletionSet.Filter();
                session.SelectedCompletionSet.SelectBestMatch();
                //session.Recalculate();

                // collapse session
                if (session.SelectedCompletionSet.Completions.Count == 0)
                    session.Collapse();

                // REVIEW: should we do our own filtering so that it doesn't dismiss session?
                //session.SelectedCompletionSet.SelectBestMatch();
                //session.Recalculate();
            }
        }

        private bool ShowCompletion(bool adjust = false)
        {
            // don't start if already active
            if (IsSessionActive(Session))
                return false;

            var bufferPosition = TextView.Caret.Position.BufferPosition;
            var snapshot = bufferPosition.Snapshot;
            var position = bufferPosition.Position;
            var line = snapshot.GetLineFromPosition(position);

            // adjust the start position
            if (adjust)
                position = GetCompletionStart(snapshot, line, position);

            var session = Broker.CreateCompletionSession(TextView, snapshot.CreateTrackingPoint(position, PointTrackingMode.Positive), true);
            session.Dismissed += OnSessionDismissed;
            session.Start();

            // session will dismiss automatically if there aren't any valid options
            if (!session.IsDismissed)
            {
                // reset ignoring
                IsIgnoring = false;

                Session = session;
                return true;
            }

            return false;
        }

        private bool Complete(bool force = false)
        {
            if (IsSessionActive(Session))
            {
                // only commit if something selected or forced to commit
                if (Session.SelectedCompletionSet.SelectionStatus.IsSelected || force)
                {
                    Session.Commit();
                    return true;
                }

                // we couldn't commit, so dismiss
                Session.Dismiss();
            }

            return false;
        }

        private bool Dismiss(ICompletionSession session)
        {
            if (session != null && !session.IsDismissed)
            {
                session.Dismiss();
                return true;
            }

            return false;
        }

        private void OnSessionDismissed(object sender, EventArgs e)
        {
            var session = sender as ICompletionSession;
            session.Dismissed -= OnSessionDismissed;

            if (Session == session)
                Session = null;

            // unflag ignoring
            IsIgnoring = false;
        }

        private bool IsSessionActive(ICompletionSession session)
        {
            if (session == null)
                return false;

            return session.IsStarted && !session.IsDismissed;
        }

        private bool IsCompletionRequested(VSCommand command, char typed)
        {
            switch (command)
            {
                case VSCommand.TAB:
                case VSCommand.RETURN:
                    return true;
                case VSCommand.TYPECHAR:
                    return IsTerminalCharacter(typed);
                default:
                    return false;
            }
        }

        private bool IsStartCharacter(char typed)
        {
            switch (typed)
            {
                // variables
                case '$':
                case '!':
                // rules / directives
                case '@':
                // vendor prefixes / pseudo selectors
                case '-':
                case ':':
                // class selectors
                case '.':
                // id selectors
                case '#':
                    return true;
            }

            return char.IsLetterOrDigit(typed);
        }

        private bool IsTerminalCharacter(char typed)
        {
            switch (typed)
            {
                case ' ': return IsSessionActive(Session) && !Session.CompletionSets.Any(set => set.Completions.Any(x => x.DisplayText.Contains(' ')));
                default:
                    return !IsStartCharacter(typed);
            }
        }

        private int GetCompletionStart(ITextSnapshot snapshot, ITextSnapshotLine line, int position)
        {
            int start = position;
            while (start > line.Start.Position)
            {
                // stop once we hit whitespace
                if (char.IsWhiteSpace(snapshot[--start]))
                {
                    start++;
                    break;
                }
            }

            return Math.Max(start, line.Start.Position);
        }

        private bool Ignore(VSCommand command, char typed)
        {
            // if we are ignoring and get a whitespace character, then stop ignoring
            if (IsIgnoring && ShouldStopIgnoring(command, typed))
                IsIgnoring = false;

            return IsIgnoring;
        }

        private bool ShouldStopIgnoring(VSCommand command, char typed)
        {
            switch (command)
            {
                case VSCommand.RETURN:
                case VSCommand.TAB:
                    return true;
                default:
                    return IsSessionActive(Session) || char.IsWhiteSpace(typed);
            }
        }

        protected override IEnumerable<VSCommand> SupportedCommands
        {
            get
            {
                yield return VSCommand.CANCEL;

                // start / filter / complete commands
                yield return VSCommand.TYPECHAR;
                yield return VSCommand.BACKSPACE;
                yield return VSCommand.DELETE;

                // completion commands
                yield return VSCommand.RETURN;
                yield return VSCommand.TAB;

                // ctrl + space commands
                yield return VSCommand.COMPLETEWORD;
                yield return VSCommand.AUTOCOMPLETE;

                // ctrl + j commands
                yield return VSCommand.SHOWMEMBERLIST;
            }
        }

        protected override VSCommand ConvertFromCommandId(uint id)
        {
            return (VSCommand)id;
        }

        protected override uint ConvertFromCommand(VSCommand command)
        {
            return (uint)command;
        }
    }
}
