using ModernSort.Services.Dialog;
using System.Windows;
using System.Windows.Input;

namespace ModernSort.CustomeControls.CustomUIControls
{
    public class BaseWindow : Window,IDialog
    {
        public BaseWindow()
        {

        }

        protected void HideButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;

        }

        protected void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }

        }

        protected void Window_DragHandler(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
