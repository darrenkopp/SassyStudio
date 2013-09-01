using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SassyStudio
{
    public class OutputMessageReceivedEventArgs : EventArgs
    {
        public string Message { get; internal set; }
    }

    public class OutputExceptionReceivedEventArgs : EventArgs
    {
        public string Message { get; internal set; }
        public Exception Error { get; internal set; }
    }

    public static class OutputLogger
    {
        public static event EventHandler<OutputMessageReceivedEventArgs> MessageReceived;
        public static event EventHandler<OutputExceptionReceivedEventArgs> ExceptionReceived;
        public static void Log(string message)
        {
            try
            {
                var handler = MessageReceived;
                if (handler != null)
                    handler(null, new OutputMessageReceivedEventArgs { Message = message });
            }
            catch
            {
                // error while logging, ignore
            }
        }

        public static void Log(Exception error, string message = null)
        {
            try
            {
                var handler = ExceptionReceived;
                if (handler != null)
                    handler(null, new OutputExceptionReceivedEventArgs { Error = error, Message = message });
            }
            catch
            {
                // error while logging, ignore
            }
        }
    }
}
