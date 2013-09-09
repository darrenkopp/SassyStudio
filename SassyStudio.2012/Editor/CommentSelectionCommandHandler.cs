using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;

namespace SassyStudio.Editor
{
    class CommentSelectionCommandHandler : VSCommandTarget<VSConstants.VSStd2KCmdID>
    {
        readonly ITextBuffer Buffer;
        public CommentSelectionCommandHandler(IVsTextView vsTextView, IWpfTextView textView)
            : base(vsTextView, textView)
        {
            Buffer = textView.TextBuffer;
        }

        protected override bool Execute(VSConstants.VSStd2KCmdID command, uint options, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (TextView.Selection.IsEmpty) return false;


            var snapshot = Buffer.CurrentSnapshot;
            int start = TextView.Selection.Start.Position.Position;
            int end = TextView.Selection.End.Position.Position;

            // TODO: pre-process the input and determine start position of left-most
            //       text just like visual studio does

            using (var edit = Buffer.CreateEdit())
            {
                while (start < end)
                {
                    var line = snapshot.GetLineFromPosition(start);
                    var text = line.GetText();
                    switch (command)
                    {
                        case VSConstants.VSStd2KCmdID.COMMENTBLOCK:
                        case VSConstants.VSStd2KCmdID.COMMENT_BLOCK:
                        {
                            if (!string.IsNullOrEmpty(text))
                                edit.Insert(line.Start.Position, "//");

                            break;
                        }
                        case VSConstants.VSStd2KCmdID.UNCOMMENTBLOCK:
                        case VSConstants.VSStd2KCmdID.UNCOMMENT_BLOCK:
                        {
                            if (text.StartsWith("//", StringComparison.OrdinalIgnoreCase))
                                edit.Delete(line.Start.Position, 2);

                            break;
                        }
                    }

                    start = line.EndIncludingLineBreak.Position;
                }

                edit.Apply();
            }

            return true;
        }

        protected override IEnumerable<VSConstants.VSStd2KCmdID> SupportedCommands
        {
            get
            {
                yield return VSConstants.VSStd2KCmdID.COMMENTBLOCK;
                yield return VSConstants.VSStd2KCmdID.COMMENT_BLOCK;
                yield return VSConstants.VSStd2KCmdID.UNCOMMENT_BLOCK;
                yield return VSConstants.VSStd2KCmdID.UNCOMMENTBLOCK;
            }
        }

        protected override VSConstants.VSStd2KCmdID ConvertFromCommandId(uint id)
        {
            return (VSConstants.VSStd2KCmdID)id;
        }

        protected override uint ConvertFromCommand(VSConstants.VSStd2KCmdID command)
        {
            return (uint)command;
        }
    }
}
