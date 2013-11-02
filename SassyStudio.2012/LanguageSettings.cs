using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace SassyStudio
{
    public class LanguageSettings
    {
        LANGPREFERENCES _Preferences;

        public LanguageSettings()
        {
            IVsTextManager textManager = (IVsTextManager)Package.GetGlobalService(typeof(SVsTextManager));
            LANGPREFERENCES[] preferences = new LANGPREFERENCES[1];
            preferences[0].guidLang = Guids.ScssLanguageService;

            if (textManager.GetUserPreferences(null, null, preferences, null) == 0)
                _Preferences = preferences[0];
        }

        public int FormatterIndentSize
        {
            get
            {
                if (!IsUsingSpaces)
                    return (int)(_Preferences.uIndentSize / _Preferences.uTabSize);

                return (int)_Preferences.uIndentSize;
            }
        }

        public int FormatterTabSize { get { return (int)_Preferences.uTabSize; } }

        public bool IsUsingSpaces
        {
            get
            {
                if (_Preferences.fInsertTabs != 0 && _Preferences.uTabSize != 0 && (_Preferences.uIndentSize % _Preferences.uTabSize) == 0)
                    return false;

                return true;
            }
        }
    }
}
