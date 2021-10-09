using log4net.Appender;
using log4net.Core;
using System.Windows.Forms;
using Telerik.WinControls;

namespace Corno.Logger
{
    /// <summary>
    ///     Displays a MessageBox for all log messages.
    /// </summary>
    public class MessageBoxAppender : AppenderSkeleton
    {
        /// <summary>
        ///     This appender requires a <see cref="AppenderSkeleton.Layout" /> to be set.
        /// </summary>
        protected override bool RequiresLayout
        {
            get { return true; }
        }

        /// <summary>
        ///     Writes the logging event to a MessageBox
        /// </summary>
        protected override void Append(LoggingEvent loggingEvent)
        {
            var buttons = MessageBoxButtons.OK;
            var icon = RadMessageIcon.None;
            switch (loggingEvent.Level.Name)
            {
                case "DEBUG":
                    buttons = MessageBoxButtons.OK;
                    icon = RadMessageIcon.Exclamation;
                    break;
                case "WARN":
                case "INFO":
                    buttons = MessageBoxButtons.OK;
                    icon = RadMessageIcon.Info;
                    break;
                case "ERROR":
                    buttons = MessageBoxButtons.OK;
                    icon = RadMessageIcon.Error;
                    break;
                case "FATAL":
                    buttons = MessageBoxButtons.OK;
                    icon = RadMessageIcon.Error;
                    break;
            }

            var title = string.Format("{0} {1}",
                loggingEvent.Level.DisplayName,
                loggingEvent.LoggerName);

            //string message = string.Format(
            //   "{0}{1}{1}{2}{1}{1}(Yes to continue, No to debug)",
            //   RenderLoggingEvent(loggingEvent),
            //   Environment.NewLine,
            //   loggingEvent.LocationInformation.FullInfo);

            //DialogResult result = MessageBox.Show(message, title,
            //   MessageBoxButtons.YesNo);

            //if (result == DialogResult.No)
            //{
            //    Debugger.Break();
            //}

            RadMessageBox.Show(loggingEvent.RenderedMessage, title,
                buttons, icon);
        }
    }
}