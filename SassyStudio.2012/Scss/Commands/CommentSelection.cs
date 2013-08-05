using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSCommandIdConstants = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;
using SassyStudio.Commands;

namespace SassyStudio.Scss.Commands
{
    class CommentSelection : CommandTargetBase
    {
        readonly ITextBuffer Buffer;
        public CommentSelection(IVsTextView adapter, IWpfTextView textView)
            : base(adapter, textView, typeof(VSCommandIdConstants).GUID, (uint)VSCommandIdConstants.COMMENTBLOCK, (uint)VSCommandIdConstants.COMMENT_BLOCK, (uint)VSCommandIdConstants.UNCOMMENT_BLOCK, (uint)VSCommandIdConstants.UNCOMMENTBLOCK)
        {
            Buffer = textView.TextBuffer;
        }

        protected override bool IsEnabled()
        {
            return true;
        }

        protected override bool Execute(uint commandId, uint execOptions, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (TextView.Selection.IsEmpty) return false;


            var snapshot = Buffer.CurrentSnapshot;
            int start = TextView.Selection.Start.Position.Position;
            int end = TextView.Selection.End.Position.Position;

            using (var edit = Buffer.CreateEdit())
            {
                while (start < end)
                {
                    var line = snapshot.GetLineFromPosition(start);
                    var text = line.GetText();
                    switch (commandId)
                    {
                        case (uint)VSCommandIdConstants.COMMENTBLOCK:
                        case (uint)VSCommandIdConstants.COMMENT_BLOCK:
                        {
                            if (!string.IsNullOrEmpty(text))
                                edit.Insert(line.Start.Position, "//");

                            break;
                        }
                        case (uint)VSCommandIdConstants.UNCOMMENTBLOCK:
                        case (uint)VSCommandIdConstants.UNCOMMENT_BLOCK:
                        {
                            if (text.StartsWith("//", StringComparison.OrdinalIgnoreCase))
                                edit.Delete(line.Start.Position, 2);

                            break;
                        }
                        default: break;
                    }

                    start = line.EndIncludingLineBreak.Position;
                }

                edit.Apply();
            }

            return true;
        }
    }
}
