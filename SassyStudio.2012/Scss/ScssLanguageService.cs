using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TextManager.Interop;

namespace SassyStudio.Scss
{
    [GuidAttribute(Guids.ScssLanguageServiceString)]
    class ScssLanguageService : IVsLanguageInfo, IVsLanguageTextOps
    {
        const int FAILED = 2147467263;
        public int GetCodeWindowManager(IVsCodeWindow pCodeWin, out IVsCodeWindowManager ppCodeWinMgr)
        {
            ppCodeWinMgr = null;
            return FAILED;
        }

        public int GetColorizer(IVsTextLines pBuffer, out IVsColorizer ppColorizer)
        {
            ppColorizer = null;
            return FAILED;
        }

        public int GetFileExtensions(out string pbstrExtensions)
        {
            pbstrExtensions = ".scss";
            return 0;
        }

        public int GetLanguageName(out string bstrName)
        {
            bstrName = "SCSS";
            return 0;
        }

        public int Format(IVsTextLayer pTextLayer, TextSpan[] ptsSel)
        {
            return FAILED;
        }

        public int GetDataTip(IVsTextLayer pTextLayer, TextSpan[] ptsSel, TextSpan[] ptsTip, out string pbstrText)
        {
            pbstrText = null;
            return FAILED;
        }

        public int GetPairExtent(IVsTextLayer pTextLayer, TextAddress ta, TextSpan[] pts)
        {
            return FAILED;
        }

        public int GetWordExtent(IVsTextLayer pTextLayer, TextAddress ta, WORDEXTFLAGS flags, TextSpan[] pts)
        {
            return FAILED;
        }
    }
}
