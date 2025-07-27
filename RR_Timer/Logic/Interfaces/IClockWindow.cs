using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Race_timer.Logic.Interfaces
{
    /// <summary>
    /// Interface for finish clock windows
    /// </summary>
    public interface IClockWindow
    {
        /// <summary>
        /// Switches between start times every 5 seconds on miniClockWindow and webViewMiniClockWindow
        /// </summary>
        void TimerTickLogic();

        /// <summary>
        /// Calls current contest timer or clock TimerClickLogic() method for updating time
        /// </summary>
        void TimerClickLogic();

        /// <summary>
        /// Method to update timer and type label with correct time
        /// Method called by ClockLogic timer
        /// </summary>
        void OnTimerClick();

        /// <summary>
        /// Method for setting the event name and type labels
        /// </summary>
        /// <param name="name">Event name</param>
        /// <param name="type">Event type</param>
        void SetLabels(string name, string type);

        /// <summary>
        /// Method for setting logo image
        /// </summary>
        /// <param name="image">Logo image</param>
        void SetImage(BitmapImage image);

        /// <summary>
        /// Method for setting QR code image
        /// </summary>
        /// <param name="image">QR code image</param>
        void SetCodeImage(BitmapSource image);

        /// <summary>
        /// Method for setting alignment of big clock window
        /// </summary>
        /// <param name="alignment">Selected alignment</param>
        void SetChildren(UserControl alignment);
    }
}
