using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Race_timer.Logic.Interfaces
{
    public interface IClockWindow
    {
        void TimerTickLogic();
        void TimerClickLogic();
        void OnTimerClick();
        void SetLabels(string name, string type);
        void SetImage(BitmapImage image);
        void SetCodeImage(BitmapSource image);
        void SetChildren(UserControl alignment);
    }
}
