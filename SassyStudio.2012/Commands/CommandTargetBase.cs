using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SassyStudio.Commands
{
    abstract class CommandTargetBase : IOleCommandTarget
    {
        private IOleCommandTarget _NextCommandTarget;
        protected readonly IWpfTextView TextView;
        protected readonly Guid CommandGroupId;
        protected readonly HashSet<uint> CommandIdSet;

        public CommandTargetBase(IVsTextView adapter, IWpfTextView textView, Guid commandGroup, params uint[] commandIds)
        {
            CommandGroupId = commandGroup;
            CommandIdSet = new HashSet<uint>(commandIds);
            TextView = textView;

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                // Add the target later to make sure it makes it in before other command handlers
                adapter.AddCommandFilter(this, out _NextCommandTarget);

            }), DispatcherPriority.ApplicationIdle, null);
        }

        protected abstract bool IsEnabled();
        protected abstract bool Execute(uint commandId, uint execOptions, IntPtr pvaIn, IntPtr pvaOut);

        protected virtual bool ExecuteNext(uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            Guid groupId = CommandGroupId;
            var result = _NextCommandTarget.Exec(ref groupId, nCmdID, nCmdexecopt, pvaIn, pvaOut);

            return result == VSConstants.S_OK;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == CommandGroupId && CommandIdSet.Contains(nCmdID))
            {
                bool result = Execute(nCmdID, nCmdexecopt, pvaIn, pvaOut);

                if (result)
                    return VSConstants.S_OK;
            }

            return _NextCommandTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == CommandGroupId)
            {
                for (int i = 0; i < cCmds; i++)
                {
                    if (CommandIdSet.Contains(prgCmds[i].cmdID))
                    {
                        if (IsEnabled())
                        {
                            prgCmds[i].cmdf = (uint)(OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED);
                            return VSConstants.S_OK;
                        }

                        prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_SUPPORTED;
                    }
                }
            }

            return _NextCommandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}
