using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using VSCommand = Microsoft.VisualStudio.VSConstants.VSStd2KCmdID;

namespace SassyStudio.Editor
{
    class FormatDocumentHandler : VSCommandTarget<VSCommand>
    {
        public FormatDocumentHandler(IVsTextView vsTextView, IWpfTextView textView)
            : base(vsTextView, textView)
        {
            IndentationCharacter = SassyStudioPackage.Instance.LanguageSettings.IsUsingSpaces ? ' ' : '\t';
            IndentationSize = SassyStudioPackage.Instance.LanguageSettings.FormatterIndentSize;
        }

        private char IndentationCharacter { get; set; }
        private int IndentationSize { get; set; }

        protected override bool Execute(VSCommand command, uint options, IntPtr pvaIn, IntPtr pvaOut)
        {
            using (var edit = TextView.TextBuffer.CreateEdit())
            {
                bool changed = false;
                int spaces = 0;
                foreach (var line in TextView.TextBuffer.CurrentSnapshot.Lines)
                {
                    var text = line.GetText();
                    if (spaces > 0)
                    {
                        int offset;
                        var end = IndexOfFirstNonWhitespaceCharacter(text, out offset);
                        if (end != -1 && (spaces + offset) > 0)
                        {
                            edit.Replace(line.Start.Position, end, new string(IndentationCharacter, spaces - offset));
                            changed = true;
                        }
                    }

                    for (int i = 0; i < text.Length; i++)
                    {
                        char c = text[i];
                        switch (c)
                        {
                            case '{':
                            case '}':
                                spaces = Math.Max(0, c == '{' ? spaces + IndentationSize : spaces - IndentationSize);
                                break;
                        }
                    }
                }

                if (changed)
                    edit.Apply();
            }

            return true;
        }

        int IndexOfFirstNonWhitespaceCharacter(string text, out int offset)
        {
            offset = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (!char.IsWhiteSpace(c))
                {
                    // if line starts with close paren, unindent
                    if (c == '}')
                        offset = IndentationSize;

                    return i;
                }
            }

            return -1;
        }

        protected override IEnumerable<VSCommand> SupportedCommands
        {
            get
            {
                yield return VSCommand.FORMATDOCUMENT;
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
