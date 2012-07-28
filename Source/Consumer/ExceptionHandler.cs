using System;
using Disruptor;
using Common;

namespace Consumer
{
    /// <summary>
    /// Common Exception handler to handle any exeception thrown from any <see cref="IEventHandler"/>.
    /// </summary>
    internal class ExceptionHandler : IExceptionHandler
    {
        void IExceptionHandler.HandleEventException(System.Exception ex, long sequence, object evt)
        {
            Log.Write(String.Format(
                "Following execption occured when processing event #{0} of type {1}:\n\t{2}",
                    sequence, evt, ex.Message));
        }

        void IExceptionHandler.HandleOnStartException(System.Exception ex)
        {
            Log.Write(String.Format(
                "Following execption occured On Handler StartUp\n\t{1}", ex.Message));
        }

        void IExceptionHandler.HandleOnShutdownException(System.Exception ex)
        {
            Log.Write(String.Format(
                "Following execption occured On Handler StartUp\n\t{1}", ex.Message));
        }
    }
}
